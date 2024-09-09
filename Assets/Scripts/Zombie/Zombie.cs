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
    public float chaseSpeed = 3.5f;
    public float idleSpeed = 1.0f;
    public Vector3 rotationOffset;
    public Transform playerTransform;
    private PlayerHealth playerHealth;

    private NavMeshAgent navMeshAgent;

    private bool hasPlayedHitAnimation = false;
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
    public AudioSource attackSound;
    public AudioSource dieSound;

    public event System.Action OnZombieKilled;
    public bool isDead = false;

    public float headshotMultiplier = 2.5f; // Multiplier for headshot damage
    public Collider headCollider;
    public bool activateRagDollOnDeath;
    public float raycastDistance = 0.5f; // Distance for raycasting to check for obstacles
    private WaveSystem waveSystem;
    public float attackDelay = 10f;
    public bool isPassive;
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        mainCollider = GetComponent<Collider>();
        waveSystem = FindObjectOfType<WaveSystem>();
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

        // Disable NavMeshAgent auto-movement
        navMeshAgent.updateRotation = false;
        originalSpeed = navMeshAgent.speed;
    }

    private bool CanMoveInDirection(Vector3 direction)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, raycastDistance))
        {
            if (hit.collider.CompareTag("Player"))
            {
                return false; // player is too close, and it's not a trigger, so cannot move
            }
        }
        return true; // No player or only triggers, can move
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

                if (distanceToPlayer <= attackRange)
                {
                    // Stop the zombie when it's within attack range
                    StopZombie();
                    AttackPlayer();
                }
                else
                {
                    // If the zombie is out of attack range, chase the player
                    ChasePlayer(direction);
                }
            }
        }
    }

    void ChasePlayer(Vector3 direction)
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

        // Manually sync the position and rotation of the NavMeshAgent with the Root Motion
        if (navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance && CanMoveInDirection(direction))
        {
            transform.position = navMeshAgent.nextPosition;
        }

        // Manually rotate the zombie to face the direction it's moving
        if (direction != Vector3.zero)
        {
            // Calculate the desired rotation towards the direction with the rotation offset applied
            Quaternion targetRotation = Quaternion.LookRotation(direction) * Quaternion.Euler(rotationOffset);

            // Smoothly rotate towards the target rotation with the offset
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }

    }

    void StopZombie()
    {
        if (isRunning)
        {
            if (navMeshAgent.isOnNavMesh)
            {
                navMeshAgent.isStopped = true; // Stops the NavMeshAgent from moving
            }
            isRunning = false;
        }
    }

    void AttackPlayer()
    {
        if (isDead) return; // Prevent attack if zombie is dead

        if (Time.time >= lastAttackTime + attackCooldown)
        {
            if (zombieAnimator != null)
            {
             
                
                zombieAnimator.Play("Kick");

                StartCoroutine(DealDamageAfterAnimation("Kick"));

                lastAttackTime = Time.time;
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

   



    IEnumerator DealDamageAfterAnimation(string animationName)
    {
        if (zombieAnimator != null)
        {
            AnimatorStateInfo stateInfo = zombieAnimator.GetCurrentAnimatorStateInfo(0);

            if (isPassive)
            {
                yield return new WaitForSeconds(stateInfo.length / attackDelay); // Wait until 4th way through the animation
            }
            else
            {
                yield return new WaitForSeconds(stateInfo.length / 2); // Wait until halfway through the animation
            }
            

            if (playerHealth != null && Vector3.Distance(transform.position, playerTransform.position) <= attackRange)
            {
                playerHealth.TakeDamage(damage);
                attackSound.Play();
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

        health -= damage;


        if (health <= 50f && !hasPlayedHitAnimation)
        {
            if (zombieAnimator != null)
            {
                zombieAnimator.Play("Hit");
                if (!isFrozen)
                {
                    navMeshAgent.speed /= 2;
                }
                
                hitSound.Play();
            }
            hasPlayedHitAnimation = true;
        }
        else
        {
            if (!isFrozen)
            {
                navMeshAgent.speed = chaseSpeed;
            }
        }

        if (health <= 0f)
        {
            Die();
            zombieAnimator.speed = 1f;
        }
    }

    void Die()
    {
        if(isDead) return; // In case shot again while dead, returns


        isDead = true; // Zombie is dead

        waveSystem.remainingZombies -= 1;
        // Disable the main collider and activate ragdoll
        mainCollider.enabled = false;
        if (activateRagDollOnDeath)
            ActivateRagdoll();
        else
            zombieAnimator.Play("Die");

        // Drop an item with a chance
        TryDropItem();
        navMeshAgent.speed = 0;
        

        // Destroy the zombie after some time to allow the ragdoll to settle
        Destroy(gameObject, 4f);
    }


    void TryDropItem()
    {
        if (droppablePrefabs.Length > 0 && Random.value <= dropChance)
        {
            int randomIndex = Random.Range(0, droppablePrefabs.Length);
            Instantiate(droppablePrefabs[randomIndex], transform.position + new Vector3(0,1,0), Quaternion.identity);
        }
    }

    void InitializeRagdoll()
    {
        ragdollColliders = GetComponentsInChildren<Collider>(true);
        ragdollRigidbodies = GetComponentsInChildren<Rigidbody>(true);

        foreach (Collider collider in ragdollColliders)
        {
            if (collider != mainCollider && collider.name != "head")
            {
                collider.enabled = false;
            }
        }

        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            rb.isKinematic = true;
        }
    }

    public void ActivateRagdoll()
    {
        // Disable animator
        zombieAnimator.enabled = false;

        // Enable ragdoll rigidbodies and colliders
        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            rb.isKinematic = false; // Allow physics to take over
            rb.detectCollisions = true;
        }

        foreach (Collider col in ragdollColliders)
        {
            col.enabled = true;
        }

        
        ragdollRigidbodies[0].AddForce(transform.forward , ForceMode.Impulse);
    }




    public void Freeze()
    {
        isFrozen = true;

        // Pause the animator by setting its speed to 0
        if (zombieAnimator != null)
        {
            zombieAnimator.speed = 0f;
        }

        // Store the original speed and stop the NavMeshAgent
        
        navMeshAgent.speed = 0f;
    }

    public void Unfreeze()
    {
        isFrozen = false;

        // Unpause the animator by restoring its speed to 1
        if (zombieAnimator != null)
        {
            zombieAnimator.speed = 1f;
        }

        // Restore the NavMeshAgent's speed
        navMeshAgent.speed = originalSpeed;
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Door"))
        {
            StopZombie(); // Stop the zombie when it first hits the door
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Door"))
        {
            AnimatorStateInfo stateInfo = zombieAnimator.GetCurrentAnimatorStateInfo(0);

            // Check if the current animation has completed
            if (stateInfo.normalizedTime >= 1.0f)
            {
                zombieAnimator.Play("Kick"); // Play the "Kick" animation
                StartCoroutine(other.GetComponent<DoorHealth>().DamageDoor());
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Door"))
        {
            ResumeChasingPlayer(); // Continue chasing the player once the zombie exits the trigger
        }
    }

    void ResumeChasingPlayer()
    {
        if (navMeshAgent.isOnNavMesh)
        {
            navMeshAgent.isStopped = false; // Resume NavMeshAgent movement
            navMeshAgent.SetDestination(playerTransform.position); // Reassign player as the target destination
            zombieAnimator.SetTrigger("Run"); // Switch back to the "Run" animation
            isRunning = true;
        }
    }

}
