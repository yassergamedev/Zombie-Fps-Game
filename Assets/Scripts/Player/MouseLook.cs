using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public Transform playerBody;
    public float sensitivityMultiplier = 1000f;
    public float mouseSensitivity;
    private float xRotation = 0f;

    public float smoothTime = 0.1f;
    private Vector3 currentRotation;
    private Vector3 rotationVelocity = Vector3.zero;
    private float yRotation;

    // Recoil variables
    public Vector2 recoilAmount = new Vector2(0.1f, 0.1f); // Adjusted X and Y recoil amounts
    public float recoilReturnSpeed = 2f; // Speed at which recoil returns to normal
    public float recoilRandomness = 0.1f; // Amount of randomness to add to recoil
    private Vector2 currentRecoil = Vector2.zero;
    private Vector2 targetRecoil = Vector2.zero;
    private WeaponController controller;
    void Start()
    {
        mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 1f) * sensitivityMultiplier;
        Cursor.lockState = CursorLockMode.Locked;

        currentRotation = transform.localEulerAngles;
        yRotation = playerBody.eulerAngles.y;
        controller = FindAnyObjectByType<WeaponController>();
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

        // Apply recoil (reduce recoil over time)
        
            currentRecoil = Vector2.Lerp(currentRecoil, Vector2.zero, Time.deltaTime * recoilReturnSpeed);
        
        

        // Apply mouse input along with recoil
        yRotation += mouseX - currentRecoil.y * Time.deltaTime;
        xRotation -= mouseY - currentRecoil.x * Time.deltaTime;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Smoothly interpolate the rotation for the camera along X-axis
        Vector3 targetRotation = new Vector3(xRotation, yRotation, 0f);
         currentRotation = Vector3.SmoothDamp(currentRotation, targetRotation, ref rotationVelocity, smoothTime);
      
        

        // Apply rotation to the camera and player body
        transform.localRotation = Quaternion.Euler(currentRotation.x, 0f, 0f);
        playerBody.localRotation = Quaternion.Euler(0f, currentRotation.y, 0f);
        if(mouseSensitivity != PlayerPrefs.GetFloat("MouseSensitivity", 1f) * sensitivityMultiplier)
                mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 1f) * sensitivityMultiplier;
    }

    public void AddRecoil(float recoilX, float recoilY)
    {
        // Add randomness to recoil
        float randomRecoilX =  recoilX;
        float randomRecoilY = Random.Range(-recoilY, recoilY);

        targetRecoil = Vector2.Lerp(targetRecoil, new Vector2(randomRecoilX, randomRecoilY), Time.deltaTime * recoilReturnSpeed);
        currentRecoil = Vector2.Lerp(currentRecoil, targetRecoil, Time.deltaTime * recoilReturnSpeed);
    }
}
