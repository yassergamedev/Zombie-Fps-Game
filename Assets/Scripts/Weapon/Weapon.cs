using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float range = 100f;
    public float damage = 10f;
    public float fireRate = 0.1f;
    public int maxAmmo = 30;       // Maximum ammo in the magazine
    public int ammoReserve = 90;   // Total ammo in reserve
    public float reloadTime = 2f;
    public int pointsOnHit = 15;
    public AudioClip shootSound;
    public AudioClip reloadSound;
    public GameObject bulletHolePrefab;
    public GameObject muzzleFlashPrefab;
    public Transform muzzlePoint;
    public Light muzzleFlashLight;

    public GameObject bloodEffectPrefab; // Blood effect prefab for zombie hits

    public Animator weaponAnimator; // Optional: If each weapon has a unique animator

    public AudioSource audioSource;
    public int currentAmmo; // Current ammo count
    public PlayerUI playerUI; // Reference to PlayerUI script

    public Vector3 focusPosition = new Vector3(0, -0.2f, 0.5f);
    public Vector3 focusRotation = new Vector3(10, 0, 0);

    public Vector3 recoilAmount = new Vector3(0.1f, 0.1f, 0.2f); // Recoil movement in each axis
    public float recoilSpeed = 10f; // Speed of the recoil effect
    public float recoilReturnSpeed = 20f; // Speed of the return to the original position after recoil

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        muzzleFlashLight.enabled = false;

        currentAmmo = maxAmmo; // Initialize current ammo
        playerUI = FindObjectOfType<PlayerUI>(); // Find the PlayerUI component in the scene

        // Update the ammo text at the start
        playerUI.UpdateAmmoText(currentAmmo, ammoReserve); // Show current ammo and reserve ammo
    }

    public virtual void Shoot(Camera playerCamera, Vector3 rotationOffset, float shakeAmount, WeaponController controller)
    {
        if (currentAmmo > 0)
        {
            //controller.StartCoroutine(controller.ShakeCamera(playerCamera, shakeAmount));

            // Play shooting sound
            audioSource.PlayOneShot(shootSound);
            if (weaponAnimator != null)
            {
                weaponAnimator.Play("Shoot");
            }

            // Muzzle flash effect
            controller.StartCoroutine(controller.MuzzleFlash(muzzleFlashPrefab, muzzlePoint, muzzleFlashLight));

            // Raycast for shooting
            RaycastHit hit;
            if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, range))
            {
                Debug.Log(hit.transform.name);

                // Find the top parent of the hit object
                Transform hitTransform = hit.transform;
                while (hitTransform.parent != null)
                {
                    hitTransform = hitTransform.parent;
                }

                // Check if the top parent has the tag "Zombie"
                if (hitTransform.CompareTag("Zombie"))
                {
                    // Get the Zombie component from the top parent
                    Zombie zombie = hitTransform.GetComponent<Zombie>();
                    if (zombie != null)
                    {
                        float finalDamage = damage; // Default damage

                        // Check if the hit was on the head
                        if (hit.collider == zombie.headCollider) // Assume headCollider is a public Collider in the Zombie script
                        {
                            finalDamage *= zombie.headshotMultiplier; // Apply headshot multiplier
                        }

                        // Apply damage to the zombie
                        zombie.TakeDamage(finalDamage);

                        // Instantiate blood effect at the hit point
                        if (bloodEffectPrefab != null)
                        {
                            GameObject bloodEffect = Instantiate(bloodEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                            Destroy(bloodEffect, .5f); // Destroy blood effect after 0.5 seconds
                        }

                        if (zombie.health <= 60)
                        {
                            // Add points to the player
                            PlayerInteract playerInteract = playerCamera.GetComponentInParent<PlayerInteract>();
                            if (playerInteract != null)
                            {
                                playerInteract.AddPoints(pointsOnHit);
                            }
                        }
                    }
                }
                else
                {
                    // Instantiate bullet hole on hit surface with adjusted rotation
                    Quaternion rotation = Quaternion.LookRotation(hit.normal);
                    rotation *= Quaternion.Euler(rotationOffset);
                    GameObject bulletHole = Instantiate(bulletHolePrefab, hit.point, rotation);

                    // Destroy bullet hole after 5 seconds
                    Destroy(bulletHole, 5f);
                }
            }

            currentAmmo--; // Decrease ammo count
            playerUI.UpdateAmmoText(currentAmmo, ammoReserve); // Update the ammo text
        }
    }





    public void Reload()
    {
        int ammoToReload = maxAmmo - currentAmmo; // Calculate how much ammo is needed to fill the magazine

        if (ammoReserve > 0 && ammoToReload > 0)
        {
            ammoToReload = Mathf.Min(ammoToReload, ammoReserve); // Determine how much ammo can be reloaded

            currentAmmo += ammoToReload; // Add the reloaded ammo to the current ammo
            ammoReserve -= ammoToReload; // Subtract the used ammo from the reserve

            audioSource.PlayOneShot(reloadSound);
            if (weaponAnimator != null)
            {
                weaponAnimator.Play("Reload");
            }

            // Update the ammo text after reloading
            playerUI.UpdateAmmoText(currentAmmo, ammoReserve);
        }
        else
        {
            Debug.Log("No ammo in reserve or magazine already full.");
        }
    }
}
