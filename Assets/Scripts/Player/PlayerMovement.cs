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
    public Animator cameraAnimator; // Animator for the camera

    // Crosshair lines
    public RectTransform crosshairTop;
    public RectTransform crosshairBottom;
    public RectTransform crosshairLeft;
    public RectTransform crosshairRight;

    public float crosshairSpread = 20f; // Amount to spread the crosshair when running
    public float crosshairFocusSpeed = 5f; // Speed of crosshair movement

    private Vector2 defaultTopCrosshairPos;
    private Vector2 defaultBotCrosshairPos;
    private Vector2 defaultLeftCrosshairPos;
    private Vector2 defaultRightCrosshairPos;

    public GameObject weapon;

    public AudioSource runningSound;
    public AudioSource jumpSound;

    public bool isSprinting = false;

    public float raycastDistance = 0.5f; // Distance for raycasting to check for obstacles
    public AudioSource breathingSound;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerUI = GetComponent<PlayerUI>();
        currentSpeed = walkSpeed;

        weapon = GameObject.FindGameObjectWithTag("Weapon");

        // Store the default positions of the crosshair lines
        defaultTopCrosshairPos = crosshairTop.anchoredPosition;
        defaultBotCrosshairPos = crosshairBottom.anchoredPosition;
        defaultLeftCrosshairPos = crosshairLeft.anchoredPosition;
        defaultRightCrosshairPos = crosshairRight.anchoredPosition;
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

        // Check for obstacles
        if (!CanMoveInDirection(move))
        {
            move = Vector3.zero; // Stop movement if there's an obstacle
        }

        if ((x != 0 || z != 0) && !weaponController.isFocusing)
        {
            SpreadCrosshair(crosshairSpread / 2); // Spread the crosshair lines

            if (!weapon.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Run")
                && weapon.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
            {
                weapon.GetComponent<Animator>().Play("Run");
            }

            if (!runningSound.isPlaying)
            {
                runningSound.Play();
            }
        }
        else
        {
            if (weapon.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f && !weaponController.isFocusing)
            {
                weapon.GetComponent<Animator>().Play("Idle");
            }

            if (runningSound.isPlaying)
            {
                runningSound.Stop();
            }
        }

        // Handle Sprinting
if (Input.GetButton("Sprint") && weaponController.currentWeapon.isSprintable && !weaponController.isFocusing && (x != 0 || z != 0) )
{
    isSprinting = true;

    currentSpeed = Mathf.Lerp(currentSpeed, sprintSpeed, Time.deltaTime * 5f);  // Smoothly accelerate
    if (!cameraAnimator.GetCurrentAnimatorStateInfo(0).IsName("Running"))
        cameraAnimator.Play("Running"); // Trigger the running animation
    SpreadCrosshair(crosshairSpread); // Spread the crosshair lines
    
    if (runningSound.pitch <= 1)
        runningSound.pitch += 0.33f;

    // Play breathing sound
    if (!breathingSound.isPlaying)
    {
        breathingSound.Play();
    }
}
else
{
            Debug.Log("is not focusing");
    if (runningSound.pitch > 0.66f)
        runningSound.pitch -= 0.33f;
    isSprinting = false;
    currentSpeed = Mathf.Lerp(currentSpeed, walkSpeed, Time.deltaTime * 5f);  // Smoothly decelerate to normal speed

    cameraAnimator.Play("Breathing");

    FocusCrosshair(); // Focus the crosshair lines back

    // Stop breathing sound when not sprinting
    if (breathingSound.isPlaying)
    {
        breathingSound.Stop();
    }
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
            if (!jumpSound.isPlaying)
            {
                jumpSound.Play();
            }

        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private bool CanMoveInDirection(Vector3 direction)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, raycastDistance))
        {
            if (!hit.collider.isTrigger)
            {
                return false; // Obstacle is too close, and it's not a trigger, so cannot move
            }
        }
        return true; // No obstacles or only triggers, can move
    }


    private void SpreadCrosshair(float crosshairSpread)
    {
        // Spread the crosshair lines outward
        crosshairTop.anchoredPosition = Vector2.Lerp(crosshairTop.anchoredPosition, defaultTopCrosshairPos + Vector2.up * crosshairSpread, Time.deltaTime * crosshairFocusSpeed);
        crosshairBottom.anchoredPosition = Vector2.Lerp(crosshairBottom.anchoredPosition, defaultBotCrosshairPos + Vector2.down * crosshairSpread, Time.deltaTime * crosshairFocusSpeed);
        crosshairLeft.anchoredPosition = Vector2.Lerp(crosshairLeft.anchoredPosition, defaultLeftCrosshairPos + Vector2.left * crosshairSpread, Time.deltaTime * crosshairFocusSpeed);
        crosshairRight.anchoredPosition = Vector2.Lerp(crosshairRight.anchoredPosition, defaultRightCrosshairPos + Vector2.right * crosshairSpread, Time.deltaTime * crosshairFocusSpeed);
    }

    private void FocusCrosshair()
    {
        // Return the crosshair lines to their default positions
        crosshairTop.anchoredPosition = Vector2.Lerp(crosshairTop.anchoredPosition, defaultTopCrosshairPos, Time.deltaTime * crosshairFocusSpeed);
        crosshairBottom.anchoredPosition = Vector2.Lerp(crosshairBottom.anchoredPosition, defaultBotCrosshairPos, Time.deltaTime * crosshairFocusSpeed);
        crosshairLeft.anchoredPosition = Vector2.Lerp(crosshairLeft.anchoredPosition, defaultLeftCrosshairPos, Time.deltaTime * crosshairFocusSpeed);
        crosshairRight.anchoredPosition = Vector2.Lerp(crosshairRight.anchoredPosition, defaultRightCrosshairPos, Time.deltaTime * crosshairFocusSpeed);
    }

    public void HideCrosshair()
    {
        // Hide the crosshair lines when focusing
        crosshairTop.gameObject.SetActive(false);
        crosshairBottom.gameObject.SetActive(false);
        crosshairLeft.gameObject.SetActive(false);
        crosshairRight.gameObject.SetActive(false);
    }

    public void ShowCrosshair()
    {
        // Show the crosshair lines
        crosshairTop.gameObject.SetActive(true);
        crosshairBottom.gameObject.SetActive(true);
        crosshairLeft.gameObject.SetActive(true);
        crosshairRight.gameObject.SetActive(true);
    }
}
