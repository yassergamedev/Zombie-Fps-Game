using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : Interactable
{
    private PlayerInteract playerInteract;

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
        // Check if the player has more than 950 points
        if (playerInteract != null && playerInteract.points >= 950)
        {
            // Subtract 950 points
            playerInteract.AddPoints(-950);

            // Trigger the gate's opening animation
            GetComponent<Animator>().SetTrigger("Open");
        }
        else
        {
            Debug.Log("Not enough points to open the gate.");
        }
    }
}
