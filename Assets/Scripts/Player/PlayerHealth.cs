using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("HealthBar")]
    public Slider healthSlider;      // UI Slider for current health
    public Slider backHealthSlider;  // UI Slider for delayed health

    public float health;
    private float lerpTimer;
    public float maxHealth = 100f;
    public float chipSpeed = 2f;

    [Header("Damage Overlay")]
    public Image overlay;
    public float duration; // How long the image stays fully opaque
    public float fadeSpeed; // How quickly the image will fade

    private float durationTimer; // Timer to check against the duration

    [Header("Death Settings")]
    public Animator playerAnimator;  // Animator for the player's death animation
    public AudioSource deathSound;   // Audio source for the death sound
    public Animator fadeInAnimator;  // Animator for fade-in effect
    public GameObject deathScreen;   // UI for the death screen
    public GameObject[] objectsToDeactivate; // Objects to deactivate on death

    private bool isDead = false;
    private Coroutine healthRegenCoroutine; // Store the coroutine for health regeneration

    void Start()
    {
        health = maxHealth;
        overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, 0);
        deathScreen.SetActive(false); // Hide the death screen initially
    }

    void Update()
    {
        if (isDead) return;

        health = Mathf.Clamp(health, 0, maxHealth);
        UpdateHealthUI();

        if (overlay.color.a > 0)
        {
            if (health < 30)
                return;
            durationTimer += Time.deltaTime;
            if (durationTimer > duration)
            {
                // fade the image
                float tempAlpha = overlay.color.a;
                tempAlpha -= Time.deltaTime * fadeSpeed;
                overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, tempAlpha);
            }
        }
    }

    public void UpdateHealthUI()
    {
        float fillA = healthSlider.value;
        float fillB = backHealthSlider.value;
        float hFraction = health / maxHealth;

        if (fillB > hFraction)
        {
            healthSlider.value = hFraction;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipSpeed;
            backHealthSlider.value = Mathf.Lerp(fillB, hFraction, percentComplete);
        }
        else if (fillB < hFraction)
        {
            healthSlider.value = hFraction;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipSpeed;
            backHealthSlider.value = Mathf.Lerp(fillB, hFraction, percentComplete);
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        health -= damage;
        lerpTimer = 0f;
        overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, 0.5f);
        durationTimer = 0f;

        // Stop any ongoing health regeneration
        if (healthRegenCoroutine != null)
        {
            StopCoroutine(healthRegenCoroutine);
        }

        // Start the health regeneration coroutine after 1 second
        healthRegenCoroutine = StartCoroutine(HealthRegenDelay());

        if (health <= 0 && !isDead)
        {
            Die();
        }
    }

    IEnumerator HealthRegenDelay()
    {
        // Wait for 1 second before starting health regeneration
        yield return new WaitForSeconds(1f);

        // Continue regenerating health while the player is not dead and health is not full
        while (!isDead && health < maxHealth)
        {
            RestoreHealth(5f); // Restore a fixed amount per tick, adjust as needed
            yield return new WaitForSeconds(0.5f); // Adjust the interval between health restoration ticks
        }
    }

    public void RestoreHealth(float healAmount)
    {
        if (isDead) return;

        health += healAmount;
        health = Mathf.Clamp(health, 0, maxHealth);
        lerpTimer = 0f;
    }

    void Die()
    {
        isDead = true;

        // Play death animation
        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger("Die");
        }

        // Play death sound
        if (deathSound != null)
        {
            deathSound.Play();
        }

        // Start fade-in animation
        if (fadeInAnimator != null)
        {
            fadeInAnimator.SetTrigger("Fade In");
            StartCoroutine(WaitForFadeIn());
        }
    }

    IEnumerator WaitForFadeIn()
    {
        // Wait for the fade-in animation to finish
        yield return new WaitForSeconds(2);

        // Show the death screen UI
        if (deathScreen != null)
        {
            deathScreen.SetActive(true);
        }
        // Deactivate specified objects
        foreach (GameObject obj in objectsToDeactivate)
        {
            obj.SetActive(false);
        }

        // Stop time
        Time.timeScale = 0f;

        // Show the cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
