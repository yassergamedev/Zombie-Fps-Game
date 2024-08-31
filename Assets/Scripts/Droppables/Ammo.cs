using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : MonoBehaviour
{
    public int ammoAmount = 30; // Amount of ammo this pickup gives
     

    public AudioClip audioClip;// Sound effect to play when the ammo is picked up
    private GameObject weapon;
    private GameObject player;
    private void Start()
    {
        //audioSource = GetComponent<AudioSource>();
        weapon = GameObject.FindGameObjectWithTag("Weapon");
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void PickAmmo()
    {

       
        weapon.GetComponent<Weapon>().ammoReserve += ammoAmount;
        player.GetComponent<PlayerUI>().UpdateAmmoText(
                weapon.GetComponent<Weapon>().currentAmmo,
                weapon.GetComponent<Weapon>().ammoReserve);

        weapon.GetComponent<AudioSource>().PlayOneShot(audioClip);


    }
}
