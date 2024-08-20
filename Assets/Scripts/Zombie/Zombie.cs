using System.Collections;
using UnityEngine;

public class Zombie : MonoBehaviour
{
    public float health = 100f; // The zombie's health
    public GameObject bloodEffectPrefab; // Blood effect to instantiate when hit
    public Animator zombieAnimator; // Reference to the Animator component
    public string hitAnimationTrigger = "Hit"; // The name of the trigger parameter for the hit animation
    public string dieAnimationTrigger = "Die"; // The name of the trigger parameter for the death animation

    public Transform playerTransform; // Reference to the player's transform

    void Update()
    {
        if (playerTransform != null)
        {
            // Rotate the zombie to always face the player
            Vector3 direction = (playerTransform.position - transform.position).normalized;
            direction.y = 0; // Keep the zombie's rotation only on the horizontal plane
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    // Method to handle damage
    public void TakeDamage(float damage)
    {
        // Reduce the zombie's health
        health -= damage;

        // Play the hit animation
        if (zombieAnimator != null)
        {
            zombieAnimator.SetTrigger(hitAnimationTrigger);
        }

        // Instantiate the blood effect at the zombie's position
        if (bloodEffectPrefab != null)
        {
            Instantiate(bloodEffectPrefab, transform.position, Quaternion.identity);
        }

        // Check if the zombie's health is depleted
        if (health <= 0f)
        {
            Die();
        }
    }

    // Method to handle the zombie's death
    void Die()
    {
        // Play the death animation immediately
        if (zombieAnimator != null)
        {
            // Interrupt any current animation and play the death animation immediately
            zombieAnimator.Play(dieAnimationTrigger, 0, 0f);
            StartCoroutine(WaitForDeathAnimation());
        }
        else
        {
            // If there's no animator, destroy the object immediately
            Destroy(gameObject);
        }
    }

    // Coroutine to wait for the death animation to finish
    IEnumerator WaitForDeathAnimation()
    {
        // Get the length of the death animation clip
        AnimatorStateInfo stateInfo = zombieAnimator.GetCurrentAnimatorStateInfo(0);
        float animationLength = stateInfo.length;

        // Wait for the animation to finish
        yield return new WaitForSeconds(animationLength);

        // Destroy the zombie game object after the animation is done
        Destroy(gameObject);
    }
}
