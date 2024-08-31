using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public Transform playerBody;
    public float sensitivityMultiplier = 1000f; // Factor to multiply the sensitivity by
    public float mouseSensitivity;
    private float xRotation = 0f;

    void Start()
    {
        // Retrieve mouse sensitivity from PlayerPrefs, with a default value of 1f
        mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 1f) * sensitivityMultiplier;
        Debug.Log(mouseSensitivity);
        // Lock the cursor to the center of the screen
        Cursor.lockState = CursorLockMode.Locked;
    }
    public void SetMouseSensitivity(float sens)
    {
        mouseSensitivity = sens * sensitivityMultiplier;
    }
    void Update()
    {
        // Mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Rotate the camera up and down
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Rotate the player body left and right
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
