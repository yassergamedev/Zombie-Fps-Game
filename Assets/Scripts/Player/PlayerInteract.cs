using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    private Camera cam;
    [SerializeField]
    private float distance = 3.0f;
    [SerializeField]
    private LayerMask mask;
    private PlayerUI playerUI;
    private PlayerMovement playerMovement;
    public int points;

    void Start()
    {
        cam = Camera.main;
        playerUI = GetComponent<PlayerUI>();
        playerMovement = GetComponent<PlayerMovement>();
        UpdatePointsUI(0);
    }

    void Update()
    {
        playerUI.UpdateText(string.Empty);

        // Create a ray at the center of the camera, shooting outwards
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * distance);
        RaycastHit hitInfo; // Variable to store our collision information
        if (Physics.Raycast(ray, out hitInfo, distance, mask))
        {
            if (hitInfo.collider.GetComponent<Interactable>() != null)
            {
                Interactable interactable = hitInfo.collider.GetComponent<Interactable>();
                playerUI.UpdateText(interactable.promptMessage);

                // Interacting
                if (Input.GetButtonDown("Interact") && playerMovement.isGrounded)
                {
                    interactable.BaseInteract();
                }
            }
        }
    }

    public void AddPoints(int amount)
    {
        points += amount;
        UpdatePointsUI(amount);
    }

    private void UpdatePointsUI(int amount)
    {
        if (playerUI != null)
        {
            playerUI.UpdatePointsText(points, amount);
        }
    }
}
