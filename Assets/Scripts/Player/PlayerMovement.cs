using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public float walkSpeed = 6f;     // Normal walking speed
    public float sprintSpeed = 12f;  // Sprinting speed
    public float crouchSpeed = 3f;   // Slower speed while crouching
    public float gravity = -9.81f;
    public float jumpHeight = 3f;

    public float crouchHeight = 1f;  // Height when crouched
    public float standingHeight = 2f;  // Normal height
    public float crouchTransitionSpeed = 5f;  // Speed of height change

    public float bobFrequency = 1.5f;  // Frequency of bobbing (up and down movement)
    public float bobAmplitude = 0.1f;  // Amplitude of bobbing (how much the player bobs)

    private Vector3 velocity;
    public bool isGrounded;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    private PlayerUI playerUI;
    private float currentSpeed;
    public WeaponController weaponController;
    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerUI = GetComponent<PlayerUI>();
        currentSpeed = walkSpeed;
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


        // Check if the player is moving horizontally or vertically
        if (x != 0 || z != 0)
        {
            weaponController.currentWeapon.weaponAnimator.SetTrigger("Run");
        }
        else
        {
            weaponController.currentWeapon.weaponAnimator.SetTrigger("Idle");
        }

            // Handle Sprinting
            if (Input.GetButton("Sprint"))
        {
            currentSpeed = Mathf.Lerp(currentSpeed, sprintSpeed, Time.deltaTime * 5f);  // Smoothly accelerate
        }
        else
        {
            currentSpeed = Mathf.Lerp(currentSpeed, walkSpeed, Time.deltaTime * 5f);  // Smoothly decelerate to normal speed
        }

        // Crouch handling
        if (Input.GetButton("Crouch"))
        {
            controller.height = Mathf.Lerp(controller.height, crouchHeight, Time.deltaTime * crouchTransitionSpeed);
            controller.Move(move * crouchSpeed * Time.deltaTime);  // Move slower while crouching
        }
        else
        {
            controller.height = Mathf.Lerp(controller.height, standingHeight, Time.deltaTime * crouchTransitionSpeed);
            controller.Move(move * currentSpeed * Time.deltaTime);  // Normal or sprint movement speed
        }

        // Apply bobbing effect when moving
        if (move.magnitude > 0 && isGrounded)
        {
            float bobOffset = Mathf.Sin(Time.time * bobFrequency) * bobAmplitude;
            controller.transform.localPosition = new Vector3(controller.transform.localPosition.x, standingHeight + bobOffset, controller.transform.localPosition.z);
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
