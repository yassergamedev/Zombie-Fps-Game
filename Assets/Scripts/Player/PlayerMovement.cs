using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public float speed = 12f;
    public float crouchSpeed = 6f;  // Slower speed while crouching
    public float gravity = -9.81f;
    public float jumpHeight = 3f;

    public float crouchHeight = 1f;  // Height when crouched
    public float standingHeight = 2f;  // Normal height
    public float crouchTransitionSpeed = 5f;  // Speed of height change

    private Vector3 velocity;
    public bool isGrounded;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    private PlayerUI playerUI;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerUI = GetComponent<PlayerUI>();
    }

    void Update()
    {
        // Check if the player is grounded
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;  // Reset the fall velocity when grounded
        }

        // Movement input
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        // Crouch handling
        if (Input.GetButton("Crouch"))
        {
            controller.height = Mathf.Lerp(controller.height, crouchHeight, Time.deltaTime * crouchTransitionSpeed);
            controller.Move(move * crouchSpeed * Time.deltaTime);  // Move slower while crouching
        }
        else
        {
            controller.height = Mathf.Lerp(controller.height, standingHeight, Time.deltaTime * crouchTransitionSpeed);
            controller.Move(move * speed * Time.deltaTime);  // Normal movement speed
        }

        // Jumping
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        


        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
