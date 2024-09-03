using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    public float swayAmount = 0.02f;            // How much the weapon sways
    public float maxSwayAmount = 0.05f;         // Maximum sway amount
    public float swaySmoothness = 6f;           // How smooth the sway effect is
    public float tiltAmount = 2f;               // How much the weapon tilts
    public float maxTiltAmount = 5f;            // Maximum tilt amount

    public float jumpTiltMultiplier = 5f;       // How much the weapon tilts due to jumping

    public Transform playerTransform;           // Reference to the player's Transform

    private Vector3 initialPosition;            // Initial position of the weapon
    private Quaternion initialRotation;         // Initial rotation of the weapon
    private float lastPlayerY;                  // Last frame's player Y position
    private float verticalVelocity;             // Calculated vertical velocity

    void Start()
    {
        // Store the initial position and rotation of the weapon
        initialPosition = transform.localPosition;
        initialRotation = transform.localRotation;

        // Initialize lastPlayerY with the player's initial Y position
        if (playerTransform != null)
        {
            lastPlayerY = playerTransform.position.y;
        }
    }

    void Update()
    {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // Calculate target sway position based on mouse input
        Vector3 targetPosition = new Vector3(
            Mathf.Clamp(-mouseX * swayAmount, -maxSwayAmount, maxSwayAmount),
            Mathf.Clamp(-mouseY * swayAmount, -maxSwayAmount, maxSwayAmount),
            0f
        );

        // Calculate vertical velocity based on change in player's Y position
        if (playerTransform != null)
        {
            float currentPlayerY = playerTransform.position.y;
            verticalVelocity = currentPlayerY - lastPlayerY;
            lastPlayerY = currentPlayerY;
        }

        // Calculate target tilt rotation based on mouse input
        Quaternion targetRotation = Quaternion.Euler(
            Mathf.Clamp(mouseY * tiltAmount, -maxTiltAmount, maxTiltAmount),
            Mathf.Clamp(-mouseX * tiltAmount, -maxTiltAmount, maxTiltAmount),
            0f
        );

        // Add jump tilt rotation based on the vertical velocity along the X-axis
        targetRotation *= Quaternion.Euler(-verticalVelocity * jumpTiltMultiplier, 0f, 0f);

        // Interpolate weapon rotation to create smooth tilt
        transform.localRotation = Quaternion.Slerp(transform.localRotation, initialRotation * targetRotation, Time.deltaTime * swaySmoothness);

        // Interpolate weapon position to create smooth sway (ignoring jump)
        transform.localPosition = Vector3.Lerp(transform.localPosition, initialPosition + targetPosition, Time.deltaTime * swaySmoothness);
    }
}
