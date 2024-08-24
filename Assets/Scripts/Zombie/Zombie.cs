using System.Collections;
using UnityEngine;

public class Zombie : MonoBehaviour
{
    public float health = 100f; // The zombie's health
    public GameObject bloodEffectPrefab; // Blood effect to instantiate when hit
    public Animator zombieAnimator; // Reference to the Animator component
    public string hitAnimationTrigger = "Hit"; // The name of the trigger parameter for the hit animation
    public string dieAnimationTrigger = "Die"; // The name of the trigger parameter for the death animation
    public float attackRange = 2f; // Range at which zombie attacks the player
    public float damage = 10f; // Damage dealt to the player
    public float attackCooldown = 2f; // Time between attacks
    private float lastAttackTime = 0f; // Tracks the time of the last attack
    public Transform playerTransform; // Reference to the player's transform
    private PlayerHealth playerHealth; // Reference to the player's health script

    public float gravity = -9.81f; // Custom gravity value
    public float groundCheckDistance = 0.4f; // Distance to check for the ground
    public Transform groundCheck;
    public LayerMask groundMask; // Layer mask for ground detection
    private bool isGrounded; // To check if the zombie is on the ground
    private Vector3 velocity; // Stores the current velocity of the zombie
    private bool hasPlayedHitAnimation = false; // Flag to track if the hit animation has been played
    void Start()
    {
        // Find the player by tag and get its transform
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            playerHealth = player.GetComponent<PlayerHealth>();
        }
        else
        {
            Debug.LogError("Player not found. Make sure the player object is tagged 'Player'.");
        }
    }

    void Update()
    {
        if (playerTransform != null)
        {
            // Check if the zombie is grounded
            isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckDistance, groundMask);

            // If the zombie is grounded and its velocity is downward, reset the velocity
            if (isGrounded && velocity.y < 0)
            {
                //velocity.y = -2f; // Slight downward velocity to keep the zombie grounded
            }
            else
            {
                // Apply gravity
                velocity.y += gravity * Time.deltaTime;
                transform.position += velocity * Time.deltaTime;
            }

            

            // Rotate the zombie to always face the player
            Vector3 direction = (playerTransform.position - transform.position).normalized;
            direction.y = 0; // Keep the zombie's rotation only on the horizontal plane
            transform.rotation = Quaternion.LookRotation(direction);

            // Move towards the player
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
            if (distanceToPlayer > attackRange)
            {
                
            }
            else
            {
                // Attack the player
                AttackPlayer();
            }
        }
    }

    // Method to handle attacking the player
    void AttackPlayer()
    {
        // Check if enough time has passed since the last attack
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            if (zombieAnimator != null)
            {
                zombieAnimator.Play("Attack"); // Set an attack trigger in the Animator
            }

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }

            // Update the last attack time to the current time
            lastAttackTime = Time.time;
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        // Check if health is below 50% and the hit animation has not been played yet
        if (health <= 50f && !hasPlayedHitAnimation)
        {
            if (zombieAnimator != null)
            {
                zombieAnimator.Play(hitAnimationTrigger);
            }
            hasPlayedHitAnimation = true; // Set the flag to true so the animation doesn't play again
        }

        if (bloodEffectPrefab != null)
        {
            Instantiate(bloodEffectPrefab, transform.position, Quaternion.identity);
        }

        if (health <= 0f)
        {
            Die();
        }
    }

    // Method to handle the zombie's death
    void Die()
    {
        if (zombieAnimator != null)
        {
            zombieAnimator.Play(dieAnimationTrigger, 0, 0f);
            StartCoroutine(WaitForDeathAnimation());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    IEnumerator WaitForDeathAnimation()
    {
        AnimatorStateInfo stateInfo = zombieAnimator.GetCurrentAnimatorStateInfo(0);
        float animationLength = stateInfo.length;
        yield return new WaitForSeconds(animationLength);
        Destroy(gameObject);
    }
}
