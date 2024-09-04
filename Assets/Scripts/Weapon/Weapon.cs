using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float range = 100f;
    public float damage = 10f;
    public float fireRate = 0.1f;
    public int maxAmmo = 30;
    public int ammoReserve = 90;
    public float reloadTime = 2f;
    public int pointsOnHit = 15;
    public AudioClip shootSound;
    public AudioClip reloadSound;
    public GameObject bulletHolePrefab;
    public GameObject muzzleFlashPrefab;
    public Transform muzzlePoint;
    public Light muzzleFlashLight;

    public GameObject bloodEffectPrefab;

    public Animator weaponAnimator;

    public AudioSource audioSource;
    public int currentAmmo;
    public PlayerUI playerUI;

    [Header("Recoil")]
    public float recoilX;
    public float recoilY;
    public MouseLook mouseLook;
    public Vector3 RecoilPush ;
    public Vector3 RecoilRotate;

    [Header ("Aim")]
    public Vector3 rotationOffset;
    public Vector3 positionOffset;

    [Header("Sprint")]
    public Vector3 sprintRotationOffset;
    public Vector3 sprintPositionOffset;
    public bool isSprintable;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        muzzleFlashLight.enabled = false;

        currentAmmo = maxAmmo;
        playerUI = FindObjectOfType<PlayerUI>();

     

        playerUI.UpdateAmmoText(currentAmmo, ammoReserve);

        mouseLook = FindObjectOfType<MouseLook>();
    }

    public virtual void Shoot(Camera playerCamera, Vector3 rotationOffset, float shakeAmount, WeaponController controller)
    {
        if (currentAmmo > 0)
        {
            // Play shoot sound
            audioSource.PlayOneShot(shootSound);

            // Play shooting animation if available
            if (weaponAnimator != null)
            {
                weaponAnimator.Play("Shoot");
            }

            // Apply recoil effect to mouse look
            mouseLook.AddRecoil(recoilX, recoilY);

            // Start the muzzle flash effect
            controller.StartCoroutine(controller.MuzzleFlash(muzzleFlashPrefab, muzzlePoint, muzzleFlashLight));

            // Recoil effect: Slightly rotate the weapon upwards and push it back
            StartCoroutine(ApplyWeaponRecoil());

            RaycastHit hit;
            if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, range))
            {
                Debug.Log(hit.transform.name);

                Transform hitTransform = hit.transform;
                while (hitTransform.parent != null)
                {
                    hitTransform = hitTransform.parent;
                }

                if (hitTransform.CompareTag("Zombie"))
                {
                    Debug.Log("Hit Zombie");
                    Zombie zombie = hitTransform.GetComponent<Zombie>();
                    if (zombie != null)
                    {
                        float finalDamage = damage;
                        Debug.Log("Hit place" + hit.collider.name);

                        // Instantiate the blood effect regardless of headshot
                        if (bloodEffectPrefab != null)
                        {
                            Transform parentTransform = hit.collider.transform.parent;
                            GameObject bloodEffect = Instantiate(bloodEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal), parentTransform);
                            Destroy(bloodEffect, 0.5f);
                        }

                        // Check if the hit collider is the zombie's head collider
                        if (hit.collider == zombie.headCollider)
                        {
                            Debug.Log("Headshot!");
                            finalDamage *= zombie.headshotMultiplier;

                            // Deactivate the head's collider and mesh renderer
                            hit.collider.gameObject.SetActive(false);
                        }

                        // Apply damage to the zombie
                        zombie.TakeDamage(finalDamage);

                        // Add points if the zombie is not dead
                        PlayerInteract playerInteract = playerCamera.GetComponentInParent<PlayerInteract>();
                        if (playerInteract != null)
                        {
                            if (!zombie.isDead)
                                playerInteract.AddPoints(pointsOnHit);
                        }
                    }
                }
                else if (hit.collider != null && !hit.collider.isTrigger) // Ensure the raycast hit a collider that is not a trigger
                {
                    Quaternion rotation = Quaternion.LookRotation(hit.normal);
                    rotation *= Quaternion.Euler(rotationOffset);
                    GameObject bulletHole = Instantiate(bulletHolePrefab, hit.point, rotation);

                    Destroy(bulletHole, 5f);
                }
            }

            currentAmmo--;
            playerUI.UpdateAmmoText(currentAmmo, ammoReserve);
        }
    }

    IEnumerator ApplyWeaponRecoil()
    {
        Vector3 originalPosition = transform.localPosition;
        Quaternion originalRotation = transform.localRotation;

        // Define the target position and rotation for recoil
        Vector3 recoilPosition = originalPosition + RecoilPush; // Push weapon back slightly
        Quaternion recoilRotation = originalRotation * Quaternion.Euler(RecoilRotate); // Slightly rotate weapon upwards

        // Apply recoil
        float elapsedTime = 0f;
        float recoilDuration = 0.1f;
        while (elapsedTime < recoilDuration)
        {
            transform.localPosition = Vector3.Lerp(originalPosition, recoilPosition, elapsedTime / recoilDuration);
            transform.localRotation = Quaternion.Lerp(originalRotation, recoilRotation, elapsedTime / recoilDuration);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Return to original position and rotation after recoil
        elapsedTime = 0f;
        while (elapsedTime < recoilDuration)
        {
            transform.localPosition = Vector3.Lerp(recoilPosition, originalPosition, elapsedTime / recoilDuration);
            transform.localRotation = Quaternion.Lerp(recoilRotation, originalRotation, elapsedTime / recoilDuration);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure weapon returns to original position and rotation fully
        transform.localPosition = originalPosition;
        transform.localRotation = originalRotation;
    }


    public void Reload()
    {
        int ammoToReload = maxAmmo - currentAmmo;

        if (ammoReserve > 0 && ammoToReload > 0)
        {
            ammoToReload = Mathf.Min(ammoToReload, ammoReserve);

            currentAmmo += ammoToReload;
            ammoReserve -= ammoToReload;

            audioSource.PlayOneShot(reloadSound);
            if (weaponAnimator != null)
            {
                weaponAnimator.Play("Reload");
            }

            playerUI.UpdateAmmoText(currentAmmo, ammoReserve);
        }
        else
        {
            Debug.Log("No ammo in reserve or magazine already full.");
        }
    }
}
