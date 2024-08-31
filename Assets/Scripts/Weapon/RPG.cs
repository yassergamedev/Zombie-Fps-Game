using System.Net.Sockets;
using UnityEngine;

public class RPG : Weapon
{
    public GameObject rocketPrefab;  // Prefab of the rocket to be instantiated
    public GameObject explosionPrefab; // Prefab of the explosion effect
    public float rocketSpeed = 30f;  // Speed of the rocket
    public AudioClip explosionSound; // Sound effect for the explosion
    public float explosionShakeAmount = 2f; // Amount of camera shake on explosion

    public override void Shoot(Camera playerCamera, Vector3 rotationOffset, float shakeAmount, WeaponController controller)
    {
        if (currentAmmo > 0)
        {
            controller.StartCoroutine(controller.ShakeCamera(playerCamera, shakeAmount));

            // Play shooting sound
            audioSource.PlayOneShot(shootSound);
            if (weaponAnimator != null)
            {
                weaponAnimator.Play("Shoot");
            }

            // Muzzle flash effect
            controller.StartCoroutine(controller.MuzzleFlash(muzzleFlashPrefab, muzzlePoint, muzzleFlashLight));

            // Instantiate the rocket at the muzzle point
            GameObject rocket = Instantiate(rocketPrefab, muzzlePoint.position, muzzlePoint.rotation);

            // Set the rocket's velocity in the forward direction of the player camera
            Rigidbody rocketRb = rocket.GetComponent<Rigidbody>();
            rocketRb.velocity = playerCamera.transform.forward * rocketSpeed;

            // Add a Rocket script to handle collision and explosion
            Rocket rocketScript = rocket.AddComponent<Rocket>();
            rocketScript.Initialize(explosionPrefab, explosionSound, explosionShakeAmount, controller, playerCamera);

            currentAmmo--; // Decrease ammo count
            playerUI.UpdateAmmoText(currentAmmo, ammoReserve); // Update the ammo text
        }
        else
        {
            Reload();
        }
    }
}
