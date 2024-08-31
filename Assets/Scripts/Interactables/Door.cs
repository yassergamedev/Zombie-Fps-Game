using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // For accessing UI elements like the slider

public class Door : Interactable
{
    public int requiredPoints = 1000; // Points required to fix the door
    public float holdTime = 3f; // Time the player needs to hold "E" to fix the door

    private bool isFixing = false; // Track if the player is in the process of fixing the door
    private float currentHoldTime = 0f; // Track the current hold time

    [SerializeField]
    private Slider loadingSlider; // Reference to the UI slider

    private PlayerInteract playerInteract; // Reference to the player's interaction script
    private Animator doorAnimator; // Reference to the door's animator

    void Start()
    {
        // Ensure the loading slider is invisible at the start
        if (loadingSlider != null)
        {
            loadingSlider.value = 0f;
            loadingSlider.gameObject.SetActive(false);
        }

        playerInteract = FindObjectOfType<PlayerInteract>(); // Find the PlayerInteract component in the scene
        doorAnimator = GetComponent<Animator>(); // Get the Animator component attached to the door
    }

    void Update()
    {
        // Check if the player is holding "E" and is within range
        if (isFixing)
        {
            if (Input.GetKey(KeyCode.E))
            {
                currentHoldTime += Time.deltaTime;

                // Update the slider value
                if (loadingSlider != null)
                {
                    loadingSlider.value = currentHoldTime / holdTime;
                }

                // Check if the player has held "E" long enough
                if (currentHoldTime >= holdTime)
                {
                    FixDoor();
                    isFixing = false; // Stop the fixing process
                }
            }
            else
            {
                // Reset the fixing process if the player releases "E"
                ResetFixing();
            }
        }
    }

    protected override void Interact()
    {
        // Check if the player has enough points
        if (playerInteract != null && playerInteract.points >= requiredPoints)
        {
            // Start the fixing process
            isFixing = true;
            currentHoldTime = 0f;

            // Show the loading slider
            if (loadingSlider != null)
            {
                loadingSlider.gameObject.SetActive(true);
            }
        }
        else
        {
            // Optionally, provide feedback that the player doesn't have enough points
            Debug.Log("Not enough points to fix the door.");
        }
    }

    private void FixDoor()
    {
        // Play the repair animation
        if (doorAnimator != null)
        {
            doorAnimator.SetTrigger("Repair");
        }

        // Subtract points from the player
        if (playerInteract != null)
        {
            playerInteract.AddPoints(-requiredPoints);
        }

        // Hide the loading slider
        if (loadingSlider != null)
        {
            loadingSlider.gameObject.SetActive(false);
        }

        Debug.Log("Door repair started!");
    }

    private void ResetFixing()
    {
        // Reset the fixing process
        isFixing = false;
        currentHoldTime = 0f;

        // Hide the loading slider
        if (loadingSlider != null)
        {
            loadingSlider.value = 0f;
            loadingSlider.gameObject.SetActive(false);
        }
    }
}
