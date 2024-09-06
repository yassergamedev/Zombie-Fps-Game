using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : Interactable
{
    private PlayerInteract playerInteract;
    public bool isOpen = false;
    public int points = 950;
    void Start()
    {
        // Find the PlayerInteract script on the player (assuming the player has a tag "Player")
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerInteract = player.GetComponent<PlayerInteract>();
        }
    }

    protected override void Interact()
    {
        if (isOpen)
        {
            return;
        }
        // Check if the player has more than 950 points
        if (playerInteract != null && playerInteract.points >= points)
        {
            // Subtract 950 points
            playerInteract.AddPoints(-950);

            // Trigger the gate's opening animation
            GetComponent<Animator>().SetTrigger("Open");
            GetComponent<AudioSource>().Play();
            promptMessage = "";
            isOpen = true;
        }
        else
        {
            Debug.Log("Not enough points to open the gate.");
        }
    }
}
