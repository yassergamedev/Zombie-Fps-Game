using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InstaKill : MonoBehaviour
{
    public float instaKillDuration = 10f;
    public AudioClip instaKillSound;
    public int instaKillDamage = 9999;

    private AudioSource audioSource;
    private WeaponController weaponController;
    private PlayerUI playerUI;
    private float originalDamage;

    void Start()
    {
        weaponController = FindObjectOfType<WeaponController>();
        playerUI = FindObjectOfType<PlayerUI>();

        if (weaponController == null || playerUI == null)
        {
            Debug.LogError("WeaponController or PlayerUI not found in the scene.");
        }

        audioSource = playerUI.gameObject.GetComponent<AudioSource>();
        
    }

    public void ActivateInstaKill()
    {
        if (weaponController != null && playerUI != null)
        {
            audioSource.clip = instaKillSound;
            audioSource.Play();

            playerUI.StartTimer(instaKillDuration, "INSTAKILL");
            playerUI.StartInstaKillTimer(instaKillDuration,instaKillDamage);

        }
    }

   
}
