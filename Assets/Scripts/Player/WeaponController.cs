using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public Camera playerCamera;

    public float shakeAmount = 0.1f;
    public Vector3 rotationOffset;

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
    private Weapon currentWeapon;
    private int currentAmmo;
    private bool isReloading = false;

    private float nextTimeToFire = 0f;

    void Start()
    {
        originalPosition = weaponHolder.localPosition;
        originalRotation = weaponHolder.localRotation;
        playerCamera.fieldOfView = defaultFOV;

        SwitchWeapon(0);
    }

    void Update()
    {
        if (isReloading) return;

        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire && currentAmmo > 0)
        {
            nextTimeToFire = Time.time + currentWeapon.fireRate;
            currentWeapon.Shoot(playerCamera, rotationOffset, shakeAmount, this);
            currentAmmo--;
        }

        if (Input.GetButton("Fire2"))
        {
            FocusWeapon(true);
        }
        else
        {
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
            weaponHolder.localPosition = Vector3.Lerp(weaponHolder.localPosition, focusPosition, Time.deltaTime * focusSpeed);
            weaponHolder.localRotation = Quaternion.Lerp(weaponHolder.localRotation, Quaternion.Euler(focusRotation), Time.deltaTime * focusSpeed);
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, zoomedFOV, Time.deltaTime * zoomSpeed);
        }
        else
        {
            weaponHolder.localPosition = Vector3.Lerp(weaponHolder.localPosition, originalPosition, Time.deltaTime * focusSpeed);
            weaponHolder.localRotation = Quaternion.Lerp(weaponHolder.localRotation, originalRotation, Time.deltaTime * focusSpeed);
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, defaultFOV, Time.deltaTime * zoomSpeed);
        }
    }

    void SwitchWeapon(int weaponIndex)
    {
        if (weapons.Count > 0)
        {
            weapons[currentWeaponIndex].SetActive(false);
        }

        currentWeaponIndex = weaponIndex;
        weapons[currentWeaponIndex].SetActive(true);

        currentWeapon = weapons[currentWeaponIndex].GetComponent<Weapon>();
        currentAmmo = currentWeapon.maxAmmo;
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
        Vector3 originalPosition = playerCamera.transform.localPosition;
        for (int i = 0; i < 5; i++)
        {
            playerCamera.transform.localPosition = originalPosition + Random.insideUnitSphere * shakeAmount;
            yield return null;
        }
        playerCamera.transform.localPosition = originalPosition;
    }

    public IEnumerator MuzzleFlash(GameObject muzzleFlashPrefab, Transform muzzlePoint, Light muzzleFlashLight)
    {
        muzzleFlashLight.enabled = true;
        GameObject muzzleFlash = Instantiate(muzzleFlashPrefab, muzzlePoint.position, muzzlePoint.rotation);
        Destroy(muzzleFlash, 0.05f);
        yield return new WaitForSeconds(0.05f);
        muzzleFlashLight.enabled = false;
    }
}
