using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoBox : Interactable
{
    public int requiredPoints = 500; // Points required to open the ammo box
   

    public GameObject randomWeapon;
    private GameObject currentRandomWeapon;
    public bool isOpened = false; // Track if the box has been opened
    private PlayerInteract playerInteract; // Reference to the player's interaction script
    private Animator boxAnimator; // Reference to the box's animator

    void Start()
    {
        playerInteract = FindObjectOfType<PlayerInteract>(); // Find the PlayerInteract component in the scene
        boxAnimator = GetComponentInParent<Animator>(); // Get the Animator component attached to the box
        currentRandomWeapon = Instantiate(randomWeapon, transform.parent);
        currentRandomWeapon.SetActive(false);
    }

    protected override void Interact()
    {
        if (isOpened)
        {
            Debug.Log("The ammo box has already been opened.");
            return;
        }

        if (playerInteract != null && playerInteract.points >= requiredPoints)
        {
            // Deduct points from the player
            playerInteract.AddPoints(-requiredPoints);

            // Play the opening animation
            if (boxAnimator != null)
            {
                boxAnimator.SetTrigger("Open");
            }

            // Start the weapon shuffling 
            currentRandomWeapon.SetActive(true);

            // Mark the box as opened
            isOpened = true;
        }
        else
        {
            Debug.Log("Not enough points to open the ammo box.");
        }
    }

    public void Close()
    {
        isOpened = false;
        if (boxAnimator != null)
        {
            boxAnimator.SetTrigger("Close");
        }
        currentRandomWeapon = Instantiate(randomWeapon, transform.parent);
        currentRandomWeapon.SetActive(false);
    }

 

}
