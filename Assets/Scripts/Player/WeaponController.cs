using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public Camera playerCamera;

    public float range = 100f;
    public float damage = 10f;
    public float fireRate = 0.1f;
    public AudioClip shootSound;
    public GameObject bulletHolePrefab;
    [Header("Rotation for bullet hole")]
    public float x, y, z;

    public GameObject muzzleFlashPrefab;
    public Transform muzzlePoint;
    public Light muzzleFlashLight;
    public float shakeAmount = 0.1f;


    private AudioSource audioSource;
    private float nextTimeToFire = 0f;
    [Header("Focus Settings")]
    public Transform weaponHolder; // Reference to the Weapon Holder object
    public Vector3 focusPosition = new Vector3(0, -0.2f, 0.5f); // Position when aiming
    public Vector3 focusRotation = new Vector3(10, 0, 0); // Rotation when aiming
    public float focusSpeed = 5f; // Speed of transition
    private Vector3 originalPosition;
    private Quaternion originalRotation;

    public float zoomedFOV = 30f; // Field of View when aiming
    public float defaultFOV = 60f; // Default Field of View
    public float zoomSpeed = 5f; // Speed of FOV transition

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        muzzleFlashLight.enabled = false; // Initially turn off the muzzle flash light

        originalPosition = weaponHolder.localPosition; // Store the original position of the weapon holder
        originalRotation = weaponHolder.localRotation; // Store the original rotation of the weapon holder
        playerCamera.fieldOfView = defaultFOV; // Set the default FOV
    }

    void Update()
    {
        // Handle shooting
        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + fireRate;
            Shoot();
        }

        // Handle focus
        if (Input.GetButton("Fire2"))
        {
            FocusWeapon(true);
        }
        else
        {
            FocusWeapon(false);
        }
    }

    void FocusWeapon(bool isFocusing)
    {
        if (isFocusing)
        {
            // Move weapon holder towards the focus position and rotation smoothly
            weaponHolder.localPosition = Vector3.Lerp(weaponHolder.localPosition, focusPosition, Time.deltaTime * focusSpeed);
            weaponHolder.localRotation = Quaternion.Lerp(weaponHolder.localRotation, Quaternion.Euler(focusRotation), Time.deltaTime * focusSpeed);

            // Smoothly zoom the camera
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, zoomedFOV, Time.deltaTime * zoomSpeed);
        }
        else
        {
            // Move weapon holder back to its original position and rotation smoothly
            weaponHolder.localPosition = Vector3.Lerp(weaponHolder.localPosition, originalPosition, Time.deltaTime * focusSpeed);
            weaponHolder.localRotation = Quaternion.Lerp(weaponHolder.localRotation, originalRotation, Time.deltaTime * focusSpeed);

            // Smoothly return to the default camera FOV
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, defaultFOV, Time.deltaTime * zoomSpeed);
        }
    }

    void Shoot()
    {
        // Play shooting sound
        //audioSource.PlayOneShot(shootSound);

        // Shake the camera slightly
        StartCoroutine(ShakeCamera());

        // Muzzle flash effect
        StartCoroutine(MuzzleFlash());

        // Raycast for shooting
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, range))
        {
            Debug.Log(hit.transform.name);

            // Apply damage if the target has a health component
            Enemy target = hit.transform.GetComponent<Enemy>();
            if (target != null)
            {
                target.Health -= damage;
            }

            // Instantiate bullet hole on hit surface with adjusted rotation
            Quaternion rotation = Quaternion.LookRotation(hit.normal);
            rotation *= Quaternion.Euler(x, y, z); // Adjust rotation as needed
            GameObject bulletHole = Instantiate(bulletHolePrefab, hit.point, rotation);

            // Destroy bullet hole after 5 seconds
            Destroy(bulletHole, 5f);
        }
    }

    IEnumerator ShakeCamera()
    {
        Vector3 originalPosition = playerCamera.transform.localPosition;
        for (int i = 0; i < 5; i++)
        {
            playerCamera.transform.localPosition = originalPosition + Random.insideUnitSphere * shakeAmount;
            yield return null;
        }
        playerCamera.transform.localPosition = originalPosition;
    }

    IEnumerator MuzzleFlash()
    {
        // Enable light and instantiate muzzle flash prefab
        muzzleFlashLight.enabled = true;
        GameObject muzzleFlash = Instantiate(muzzleFlashPrefab, muzzlePoint.position, muzzlePoint.rotation);
        Destroy(muzzleFlash, 0.05f); // Destroy the flash quickly

        yield return new WaitForSeconds(0.05f); // Duration of the flash

        // Disable light
        muzzleFlashLight.enabled = false;
    }
}
