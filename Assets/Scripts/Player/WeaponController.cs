using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class WeaponController : MonoBehaviour
{
    public Camera playerCamera;

    public float shakeAmount = 0.1f;

    [Header("Focus Settings")]
    public float focusSpeed = 5f;

    private Vector3 originalPosition;
    private Quaternion originalRotation;

    public float zoomedFOV = 30f;
    public float defaultFOV = 60f;
    public float zoomSpeed = 5f;

    [Header("Weapon Switching")]
    public List<GameObject> weapons;
    private int currentWeaponIndex = 0;
    public Weapon currentWeapon;
    private Transform weaponTransform;
    private int currentAmmo;
    private bool isReloading = false;

    private float nextTimeToFire = 0f;
    public GameObject player;

    private PlayerUI currentPlayerUI;
    private PlayerMovement playerMovement;
    public GameObject crossHair;

    private Vector3 recoilPosition;
    private Quaternion recoilRotation;
    private Vector3 originalCameraPosition;
    private Quaternion originalCameraRotation;
    public bool isRecoiling = false;
    public bool isFocusing = false;
    void Start()
    {
       

        // Store the original camera position and rotation
        originalCameraPosition = playerCamera.transform.localPosition;
        originalCameraRotation = playerCamera.transform.localRotation;

        playerCamera.fieldOfView = defaultFOV;

        SwitchWeapon(0);

        currentPlayerUI = player.GetComponent<PlayerUI>();
        playerMovement = player.GetComponent<PlayerMovement>();
        currentPlayerUI.UpdateAmmoText(currentWeapon.maxAmmo, currentWeapon.maxAmmo);
        weaponTransform = currentWeapon.gameObject.transform;
        // Store the original position and rotation from the weaponHolder
        originalPosition = weaponTransform.localPosition;
        originalRotation = weaponTransform.localRotation;
    }

    void Update()
    {
        if (isReloading) return;

        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire && currentAmmo > 0)
        {
            if (!playerMovement.isSprinting) {
                nextTimeToFire = Time.time + currentWeapon.fireRate;
                currentWeapon.Shoot(playerCamera, Vector3.zero, shakeAmount, this);
                currentAmmo--;
            }
           

            // Apply recoil when shooting
            //ApplyRecoil(currentWeapon.recoilAmount);
        }
        else
        {
            if (currentAmmo <= 0)
            {
                StartCoroutine(Reload());
            }
        }

        if (Input.GetButton("Fire2") )
        {
            // Check if the current weapon is an RPG
            if (!(currentWeapon is RPG))
            {
                crossHair.SetActive(false);
                isFocusing = true;

                if (!playerMovement.isSprinting)
                {
                    FocusWeapon(true);
                    currentWeapon.weaponAnimator.Play("Idle");
                    isFocusing = true;
                }
            }


        }
        else
        {
            crossHair.SetActive(true);
           // currentWeapon.weaponAnimator.SetTrigger("Idle");
            FocusWeapon(false);
            isFocusing = false;
        }

        if (Input.GetButtonDown("SwitchWeapon"))
        {
            SwitchWeapon((currentWeaponIndex + 1) % weapons.Count);
        }

        if (Input.GetButtonDown("Reload") && !isReloading && currentAmmo < currentWeapon.maxAmmo)
        {
            StartCoroutine(Reload());
        }
        if (playerMovement.isSprinting)
        {
            WeaponSprintPosition();
        }
        else
        {
            WeaponSprintPosition();
        }
    }
    void WeaponSprintPosition()
    {
        if (playerMovement.isSprinting)
        {
            // Calculate target position and rotation when focusing
            Vector3 targetPosition = currentWeapon.sprintPositionOffset;
            Quaternion targetRotation = Quaternion.Euler(currentWeapon.sprintRotationOffset);

            // Smoothly interpolate position and rotation
            weaponTransform.localPosition = Vector3.Slerp(weaponTransform.localPosition, targetPosition, Time.deltaTime * focusSpeed);
            weaponTransform.localRotation = Quaternion.Slerp(weaponTransform.localRotation, targetRotation, Time.deltaTime * focusSpeed);
            crossHair.SetActive(false);

        }
        else
        {
            if (!isFocusing)
            {
                // Smoothly reset to the original position and rotation
                weaponTransform.localPosition = Vector3.Slerp(weaponTransform.localPosition, originalPosition, Time.deltaTime * focusSpeed);
                weaponTransform.localRotation = Quaternion.Slerp(weaponTransform.localRotation, originalRotation, Time.deltaTime * focusSpeed);
                crossHair.SetActive(true);
            }
           

        }
    }
    void FocusWeapon(bool isFocusing)
    {
        if (isFocusing)
        {
            // Calculate target position and rotation when focusing
            Vector3 targetPosition = currentWeapon.positionOffset;
            Quaternion targetRotation =  Quaternion.Euler(currentWeapon.rotationOffset);

            // Smoothly interpolate position and rotation
            weaponTransform.localPosition = Vector3.Slerp(weaponTransform.localPosition, targetPosition, Time.deltaTime * focusSpeed);
            weaponTransform.localRotation = Quaternion.Slerp(weaponTransform.localRotation, targetRotation, Time.deltaTime * focusSpeed);

            // Optionally, you can Slerp the camera's field of view for a zoom effect
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, zoomedFOV, Time.deltaTime * zoomSpeed);
            playerCamera.gameObject.GetComponent<Animator>().speed = 0;
        }
        else
        {
            // Smoothly reset to the original position and rotation
           weaponTransform.localPosition = Vector3.Slerp(weaponTransform.localPosition, originalPosition, Time.deltaTime * focusSpeed);
            weaponTransform.localRotation = Quaternion.Slerp(weaponTransform.localRotation, originalRotation, Time.deltaTime * focusSpeed);

            //Reset the camera's field of view
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, defaultFOV, Time.deltaTime * zoomSpeed);
            playerCamera.enabled = true;
            playerCamera.gameObject.GetComponent<Animator>().speed = 1;
        }
    }


    public void SwitchWeapon(int weaponIndex)
    {
        if (weapons.Count > 0)
        {
            weapons[currentWeaponIndex].SetActive(false);
           
        }

        currentWeaponIndex = weaponIndex;
        weapons[currentWeaponIndex].SetActive(true);

        currentWeapon = weapons[currentWeaponIndex].GetComponent<Weapon>();
        currentAmmo = currentWeapon.maxAmmo;
        currentWeapon.ammoReserve = currentWeapon.normalAmmoReserve;
        player.GetComponent<PlayerMovement>().weapon = GameObject.FindGameObjectWithTag("Weapon");
        weaponTransform = currentWeapon.gameObject.transform;
        // Store the original position and rotation from the weaponHolder
        originalPosition = weaponTransform.localPosition;
        originalRotation = weaponTransform.localRotation;
    }

    IEnumerator Reload()
    {
        isReloading = true;
        currentWeapon.Reload(this);
        yield return new WaitForSeconds(currentWeapon.reloadTime);
        currentAmmo = currentWeapon.maxAmmo;
        isReloading = false;
    }

    public IEnumerator ShakeCamera(Camera playerCamera, float shakeAmount)
    {
        Vector3 originalPosition = playerCamera.gameObject.transform.localPosition;
        for (int i = 0; i < 5; i++)
        {
            playerCamera.gameObject.transform.localPosition = originalPosition + Random.insideUnitSphere * shakeAmount;
            yield return null;
        }
        playerCamera.gameObject.transform.localPosition = originalPosition;
    }

    public IEnumerator MuzzleFlash(GameObject muzzleFlashPrefab, Transform muzzlePoint, Light muzzleFlashLight)
    {
        muzzleFlashLight.enabled = true;
        GameObject muzzleFlash = Instantiate(muzzleFlashPrefab, muzzlePoint.position + new Vector3(0,-0.8f,0), muzzlePoint.rotation);
        Destroy(muzzleFlash, 0.05f);
        yield return new WaitForSeconds(0.05f);
        muzzleFlashLight.enabled = false;
    }
    public void ApplyRecoil(Vector3 recoilAmount)
    {
        // Calculate the target rotation for recoil
        Quaternion recoilTargetRotation = originalCameraRotation * Quaternion.Euler(recoilAmount);

        // Apply the recoil rotation instantly
        playerCamera.gameObject.transform.localRotation = recoilTargetRotation;

        //   recoil flag to true if needed for other logic
        isRecoiling = true;


    }
   

}
