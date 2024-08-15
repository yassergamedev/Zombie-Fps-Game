using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;  

public class PlayerHealth : MonoBehaviour
{
    public Slider healthSlider;      // UI Slider for current health
    public Slider backHealthSlider;  // UI Slider for delayed health

    private float health;
    private float lerpTimer;
    public float maxHealth = 100f;
    public float chipSpeed = 2f;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        health = Mathf.Clamp(health, 0, maxHealth);
        UpdateHealthUI();
        if (Input.GetKeyDown(KeyCode.Y))
        {
            TakeDamage(Random.Range(0, maxHealth));
            
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
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        lerpTimer = 0f;
    }
}
