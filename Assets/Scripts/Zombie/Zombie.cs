using System.Collections;
using UnityEngine;
using UnityEngine.AI;


public class Zombie : MonoBehaviour
{
    public float health = 100f;
    public GameObject bloodEffectPrefab;
    public Animator zombieAnimator;
    public float attackRange = 2f;
    public float damage = 10f;
    public float attackCooldown = 2f;
    private float lastAttackTime = 0f;
    public float detectionRange = 10f;
    public float chaseSpeed = 3.5f;
    public float idleSpeed = 1.0f;
    public float patrolRadius = 10f;
    public int numberOfPatrolPoints = 3;

    public Transform playerTransform;
    private PlayerHealth playerHealth;

    private NavMeshAgent navMeshAgent;

    private bool hasPlayedHitAnimation = false;
    public bool isAwareOfPlayer = false;
    private Vector3[] patrolPoints;
    private int currentPatrolIndex = 0;
    private bool isPatrolling = false;
    private bool isRunning = false;

    // Ground Check
    public Transform groundCheck; // Reference to the ground check object
    public float groundCheckRadius = 0.3f; // Radius of the sphere collider for ground check
    public LayerMask groundLayer; // The ground layer the zombie can walk on
    private bool isGrounded;

    //Time Stop Properties
    private bool isFrozen = false;
    private float originalSpeed;

    // Dropping items
    public GameObject[] droppablePrefabs; // Array of items that the zombie can drop
    public float dropChance = 0.5f; // Chance to drop an item

    // Ragdoll components
    private Collider[] ragdollColliders;
    private Rigidbody[] ragdollRigidbodies;
    private Collider mainCollider;

    // Audio components
    public AudioSource idleSound;
    public AudioSource hitSound;
    public AudioSource runSound;
    public AudioSource dieSound;

    public event System.Action OnZombieKilled;
    private bool isDead = false;

    public float headshotMultiplier = 2.5f; // Multiplier for headshot damage
    public Collider headCollider;
    public bool activateRagDollOnDeath;
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        mainCollider = GetComponent<Collider>();

        // Initialize ragdoll
        InitializeRagdoll();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            playerHealth = player.GetComponent<PlayerHealth>();
        }
        else
        {
            Debug.LogError("Player not found. Make sure the player object is tagged 'Player'.");
        }

        GeneratePatrolPoints();
    }

    void Update()
    {
        if (!isFrozen)
        {
            // Ground Check
            isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);

            if (playerTransform != null && isGrounded)
            {
                Vector3 direction = (playerTransform.position - transform.position).normalized;
                direction.y = 0;
                float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

                if (distanceToPlayer <= detectionRange)
                {
                    isAwareOfPlayer = true;
                    isPatrolling = false;
                }

                if (isAwareOfPlayer)
                {
                    if (!isRunning)
                    {
                        zombieAnimator.SetTrigger("Run");
                        runSound.Play();
                        isRunning = true;
                        navMeshAgent.speed = chaseSpeed;
                    }

                    if (navMeshAgent.isOnNavMesh)
                    {
                        navMeshAgent.SetDestination(playerTransform.position);
                    }
                    else
                    {
                        Debug.LogWarning("Zombie is not on a NavMesh.");
                    }

                    AlignToGround();
                    if (distanceToPlayer <= attackRange)
                    {
                        AttackPlayer();
                    }
                }
                else
                {
                    if (!isPatrolling)
                    {
                        zombieAnimator.SetTrigger("Walk");
                        idleSound.Play();
                        navMeshAgent.speed = idleSpeed;
                        StartCoroutine(Patrol());
                    }
                }
            }
        }
    }

    void AlignToGround()
    {
        if (navMeshAgent.isOnNavMesh)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hit, 2f))
            {
                Quaternion targetRotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            }
        }
    }

    void AttackPlayer()
    {
        if (isDead) return; // Prevent attack if zombie is dead

        if (Time.time >= lastAttackTime + attackCooldown)
        {
            if (zombieAnimator != null)
            {
                if (navMeshAgent.isOnNavMesh)
                {
                    navMeshAgent.isStopped = false; // Ensure the zombie keeps moving
                    navMeshAgent.speed = chaseSpeed / 2; // Slow down the zombie while attacking
                }

                string[] attackAnimations = { "Attack", "Punch", "Kick", "KickHard" };
                string selectedAttack = attackAnimations[Random.Range(0, attackAnimations.Length)];
                zombieAnimator.Play(selectedAttack);

                StartCoroutine(DealDamageAfterAnimation(selectedAttack));

                lastAttackTime = Time.time;
            }
        }
    }



    IEnumerator DealDamageAfterAnimation(string animationName)
    {
        if (zombieAnimator != null)
        {
            AnimatorStateInfo stateInfo = zombieAnimator.GetCurrentAnimatorStateInfo(0);

            yield return new WaitForSeconds(stateInfo.length / 2); // Wait until halfway through the animation

            if (playerHealth != null && Vector3.Distance(transform.position, playerTransform.position) <= attackRange)
            {
                playerHealth.TakeDamage(damage);
            }

            if (navMeshAgent.isOnNavMesh)
            {
                navMeshAgent.speed = chaseSpeed; // Restore the normal chase speed after the attack
                navMeshAgent.isStopped = false;
            }
            isRunning = false;
        }
    }


    public void TakeDamage(float damage)
    {
        isAwareOfPlayer = true;
        isPatrolling = false;
        health -= damage;


        if (health <= 50f && !hasPlayedHitAnimation)
        {
            if (zombieAnimator != null)
            {
                zombieAnimator.Play("Hit");
                isRunning = false;
                hitSound.Play();
            }
            hasPlayedHitAnimation = true;
        }

        if (health <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true; // Zombie is dead

        // Disable the main collider and activate ragdoll
        mainCollider.enabled = false;
        if (activateRagDollOnDeath)
            ActivateRagdoll();
        else
            zombieAnimator.SetTrigger("Die");

        // Drop an item with a chance
        TryDropItem();

        // Invoke the OnZombieKilled event
        OnZombieKilled?.Invoke();

        // Destroy the zombie after some time to allow the ragdoll to settle
        Destroy(gameObject, 10f);
    }


    void TryDropItem()
    {
        if (droppablePrefabs.Length > 0 && Random.value <= dropChance)
        {
            int randomIndex = Random.Range(0, droppablePrefabs.Length);
            Instantiate(droppablePrefabs[randomIndex], transform.position, Quaternion.identity);
        }
    }

    void InitializeRagdoll()
    {
        ragdollColliders = GetComponentsInChildren<Collider>(true);
        ragdollRigidbodies = GetComponentsInChildren<Rigidbody>(true);

        foreach (Collider collider in ragdollColliders)
        {
            if (collider != mainCollider)
            {
                collider.enabled = false;
            }
        }

        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            rb.isKinematic = true;
        }
    }

    void ActivateRagdoll()
    {
        zombieAnimator.enabled = false;
        navMeshAgent.enabled = false;

        foreach (Collider collider in ragdollColliders)
        {
            collider.enabled = true;
        }

        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            rb.isKinematic = false;
        }
    }

    public void ReactToSound(Vector3 soundLocation)
    {
        if (!isAwareOfPlayer)
        {
            isAwareOfPlayer = true;
            navMeshAgent.SetDestination(soundLocation);
            navMeshAgent.speed = chaseSpeed;
        }
    }

    void GeneratePatrolPoints()
    {
        patrolPoints = new Vector3[numberOfPatrolPoints];
        for (int i = 0; i < numberOfPatrolPoints; i++)
        {
            Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
            randomDirection += transform.position;
            randomDirection.y = transform.position.y; // Keep patrol points at the same height
            patrolPoints[i] = randomDirection;
        }
    }

    IEnumerator Patrol()
    {
        isPatrolling = true;
        int patrolIndex = 0;
        while (true)
        {
            if (patrolPoints.Length == 0) yield break;

            navMeshAgent.SetDestination(patrolPoints[patrolIndex]);

            while (Vector3.Distance(transform.position, patrolPoints[patrolIndex]) > 1f)
            {
                yield return null;
            }

            patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
            yield return new WaitForSeconds(2f);
        }
    }

    public void Freeze()
    {
        isFrozen = true;
        originalSpeed = navMeshAgent.speed;
        navMeshAgent.speed = 0f;
    }

    public void Unfreeze()
    {
        isFrozen = false;
        navMeshAgent.speed = originalSpeed;
    }
}
