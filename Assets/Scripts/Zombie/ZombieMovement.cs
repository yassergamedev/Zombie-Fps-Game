using UnityEngine;

public class ZombieMovement : MonoBehaviour
{
    public float health = 100f;
    public float speed = 3.5f;
    public float gravity = 9.81f;

    private CharacterController characterController;
    private Vector3 moveDirection = Vector3.zero;

    public Transform playerTransform;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("Player not found. Make sure the player object is tagged 'Player'.");
        }
    }

    private void Update()
    {
        if (playerTransform != null)
        {
            // Calculate direction towards the player
            Vector3 direction = (playerTransform.position - transform.position).normalized;
            direction.y = 0;

            // Move the zombie towards the player
            moveDirection = direction * speed;

            // Apply gravity
            moveDirection.y -= gravity * Time.deltaTime;

            // Move the character controller
            characterController.Move(moveDirection * Time.deltaTime);
        }
    }
}
