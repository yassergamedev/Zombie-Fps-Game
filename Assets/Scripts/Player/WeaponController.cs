using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public Camera playerCamera;

    public float shakeAmount = 0.1f;

    [Header("Focus Settings")]
    public Transform weaponHolder;
    public Vector3 focusPosition = new Vector3(0, -0.2f, 0.5f);
    public Vector3 focusRotation = new Vector3(10, 0, 0);
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
    private bool isRecoiling = false;
    void Start()
    {
        // Store the original position and rotation from the weaponHolder
        originalPosition = weaponHolder.localPosition;
        originalRotation = weaponHolder.localRotation;

        // Store the original camera position and rotation
        originalCameraPosition = playerCamera.transform.localPosition;
        originalCameraRotation = playerCamera.transform.localRotation;

        playerCamera.fieldOfView = defaultFOV;

        SwitchWeapon(0);

        currentPlayerUI = player.GetComponent<PlayerUI>();
        playerMovement = player.GetComponent<PlayerMovement>();
        currentPlayerUI.UpdateAmmoText(currentWeapon.maxAmmo, currentWeapon.maxAmmo);
    }

    void Update()
    {
        if (isReloading) return;

        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire && currentAmmo > 0)
        {
            nextTimeToFire = Time.time + currentWeapon.fireRate;
            currentWeapon.Shoot(playerCamera, Vector3.zero, shakeAmount, this);
            currentAmmo--;

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
            crossHair.SetActive(false);
            FocusWeapon(true);
        }
        else
        {
            crossHair.SetActive(true);
            FocusWeapon(false);
        }

        if (Input.GetButtonDown("SwitchWeapon"))
        {
            SwitchWeapon((currentWeaponIndex + 1) % weapons.Count);
        }

        if (Input.GetButtonDown("Reload") && !isReloading && currentAmmo < currentWeapon.maxAmmo)
        {
            StartCoroutine(Reload());
        }
    }

    void FocusWeapon(bool isFocusing)
    {
        if (isFocusing)
        {
            weaponHolder.localPosition = Vector3.Lerp(weaponHolder.localPosition, currentWeapon.focusPosition, Time.deltaTime * focusSpeed);
            weaponHolder.localRotation = Quaternion.Lerp(weaponHolder.localRotation, Quaternion.Euler(currentWeapon.focusRotation), Time.deltaTime * focusSpeed);
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, zoomedFOV, Time.deltaTime * zoomSpeed);
        }
        else
        {
            weaponHolder.localPosition = Vector3.Lerp(weaponHolder.localPosition, originalPosition, Time.deltaTime * focusSpeed);
            weaponHolder.localRotation = Quaternion.Lerp(weaponHolder.localRotation, originalRotation, Time.deltaTime * focusSpeed);
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, defaultFOV, Time.deltaTime * zoomSpeed);
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
        player.GetComponent<PlayerMovement>().weapon = GameObject.FindGameObjectWithTag("Weapon");
    }

    IEnumerator Reload()
    {
        isReloading = true;
        currentWeapon.Reload();
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
        GameObject muzzleFlash = Instantiate(muzzleFlashPrefab, muzzlePoint.position, muzzlePoint.rotation);
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
    public void RecoilMath()
    {

    }

}
