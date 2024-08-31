using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int healthAmount = 20; // Amount of health this pickup gives

    public AudioClip audioClip; // Sound effect to play when the health is picked up
    private GameObject player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void PickHealth()
    {
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        playerHealth.RestoreHealth(healthAmount);

        playerHealth.UpdateHealthUI();

        player.GetComponent<AudioSource>().PlayOneShot(audioClip);
    }
}
