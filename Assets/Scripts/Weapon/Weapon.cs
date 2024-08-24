using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float range = 100f;
    public float damage = 10f;
    public float fireRate = 0.1f;
    public int maxAmmo = 30;
    public float reloadTime = 2f;

    public AudioClip shootSound;
    public AudioClip reloadSound;
    public GameObject bulletHolePrefab;
    public GameObject muzzleFlashPrefab;
    public Transform muzzlePoint;
    public Light muzzleFlashLight;

    public Animator weaponAnimator; // Optional: If each weapon has a unique animator

    private AudioSource audioSource;


    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        muzzleFlashLight.enabled = false;
    }

    public void Shoot(Camera playerCamera, Vector3 rotationOffset, float shakeAmount, WeaponController controller)
    {
        controller.StartCoroutine(controller.ShakeCamera(playerCamera, shakeAmount));

        // Play shooting sound
        audioSource.PlayOneShot(shootSound);

        // Muzzle flash effect
        controller.StartCoroutine(controller.MuzzleFlash(muzzleFlashPrefab, muzzlePoint, muzzleFlashLight));

        // Raycast for shooting
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, range))
        {
            Debug.Log(hit.transform.name);

            // Apply damage if the target is a zombie
            Zombie zombie = hit.transform.GetComponent<Zombie>();
            if (zombie != null)
            {
                zombie.TakeDamage(damage);
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
    }

    public void Reload()
    {
        audioSource.PlayOneShot(reloadSound);
        if (weaponAnimator != null)
        {
            weaponAnimator.Play("Reload");
        }
    }

}
