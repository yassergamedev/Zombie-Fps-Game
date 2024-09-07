using UnityEngine;

public class Rocket : MonoBehaviour
{
    private GameObject explosionPrefab;
    private AudioClip explosionSound;
    private float explosionShakeAmount;
    private WeaponController weaponController;
    private Camera playerCamera;
    public float explosionForce = 700f; // Force applied to ragdoll
    public float explosionRadius = 5f;  // Radius of explosion effect

    private bool hasExploded = false;

    public void Initialize(GameObject explosionPrefab, AudioClip explosionSound, float explosionShakeAmount, WeaponController controller, Camera camera)
    {
        this.explosionPrefab = explosionPrefab;
        this.explosionSound = explosionSound;
        this.explosionShakeAmount = explosionShakeAmount;
        this.weaponController = controller;
        this.playerCamera = camera;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (hasExploded) return; // Prevent multiple explosions

        hasExploded = true;

        // Instantiate the explosion effect at the collision point
        GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        Destroy(explosion, 3f); // Destroy the explosion effect after 3 seconds

        // Play the explosion sound
        AudioSource.PlayClipAtPoint(explosionSound, transform.position);

        // Shake the camera
        weaponController.StartCoroutine(weaponController.ShakeCamera(playerCamera, explosionShakeAmount));

        // Handle the explosion force and damage
        HandleExplosionEffects();

        // Destroy the rocket
        Destroy(gameObject);
    }
 

    private void HandleExplosionEffects()
    {
        // Get all nearby objects within the explosion radius
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider nearbyObject in colliders)
        {
            // Apply explosion force to any Rigidbody within the explosion radius
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }

            // Check if the object is a zombie
            Zombie zombie = nearbyObject.GetComponent<Zombie>();
            if (zombie != null)
            {
                // Make the zombie die instantly
                zombie.TakeDamage(zombie.health);

            }
        }
    }

    private void ApplyRagdoll(Zombie zombie)
    {
        // Assuming the Zombie script has a method to enable ragdoll
        //zombie.EnableRagdoll();

        // Apply additional explosion force to the ragdoll
        Rigidbody[] ragdollRigidbodies = zombie.GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, 1f, ForceMode.Impulse);
        }
    }
}
