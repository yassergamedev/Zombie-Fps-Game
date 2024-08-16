using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;  

public class PlayerHealth : MonoBehaviour
{
    [Header("HealthBar")]
    public Slider healthSlider;      // UI Slider for current health
    public Slider backHealthSlider;  // UI Slider for delayed health

    private float health;
    private float lerpTimer;
    public float maxHealth = 100f;
    public float chipSpeed = 2f;

    [Header("Damage Overlay")]
    public Image overlay;
    public float duration; //how long the image stays fully opaque
    public float fadeSpeed; //how quickly the image will fade

    private float durationTimer; //Timer to check against the duration
    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, 0);
    }

    // Update is called once per frame
    void Update()
    {
        health = Mathf.Clamp(health, 0, maxHealth);
        UpdateHealthUI();

        if(overlay.color.a > 0)
        {
            if (health < 30)
                return;
            durationTimer += Time.deltaTime;
            if(durationTimer > duration)
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
        health -= damage;
        lerpTimer = 0f;
        overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, 1);
        durationTimer = 0f;
    }
    public void RestoreHealth(float healAmount)
    {
        health += healAmount;
        health = Mathf.Clamp(health, 0, maxHealth);
        lerpTimer = 0f;
        
    }
}
