using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AtomicBombDrop : MonoBehaviour
{
    [SerializeField] private AudioClip bombImpactSound;
    [SerializeField] private float explosionRadius = 20.0f;
    [SerializeField] private float explosionForce = 500.0f;
    [SerializeField] private Image whiteOverlay; // Reference to the white overlay UI
    [SerializeField] private float fadeDuration = 4.5f; // Duration for the fade effect
    private AudioSource audioSource;
    private PlayerInteract playerInteract;
    private Collider bombCollider;

    private void Start()
    {
        // Setup the audio source, bomb collider, and white overlay
        audioSource = GetComponent<AudioSource>();
        bombCollider = GetComponent<Collider>();
        whiteOverlay = GameObject.FindGameObjectWithTag("White Overlay").GetComponent<Image>();
        playerInteract = FindAnyObjectByType<PlayerInteract>();

        if (whiteOverlay != null)
        {
            whiteOverlay.color = new Color(1, 1, 1, 0); // Fully transparent at start
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Disable the bomb's collider to prevent further triggers
            bombCollider.enabled = false;
            StartCoroutine(AtomicBombSequence());
        }
    }

    private IEnumerator AtomicBombSequence()
    {
        // Play the explosion sound
        if (bombImpactSound != null)
        {
            audioSource.clip = bombImpactSound;
            audioSource.Play();
        }

        // Flash the screen white
        StartCoroutine(FlashWhiteScreen());

        // Find and kill all zombies
        Zombie[] allZombies = FindObjectsOfType<Zombie>();
        foreach (Zombie zombie in allZombies)
        {
            Rigidbody rb = zombie.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, 1.0f, ForceMode.Impulse);
            }

            // Kill the zombie by dealing damage equal to its health
            zombie.TakeDamage(zombie.health);
        }

        // Award points to the player
        playerInteract.AddPoints(allZombies.Length * 50);

        // Wait until the white overlay fades out before destroying the bomb object
        yield return new WaitForSeconds(fadeDuration + 0.5f); // Wait for fade to finish before removing the bomb

        // Optionally destroy the bomb game object after the effect completes
        Destroy(gameObject);
    }

    private IEnumerator FlashWhiteScreen()
    {
        if (whiteOverlay == null) yield break;

        // Make the screen fully white
        whiteOverlay.color = new Color(1, 1, 1, 1); // Fully white
        yield return new WaitForSeconds(0.5f); // Keep it white for half a second

        // Gradually fade back to normal
        float elapsedTime = 0.0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, elapsedTime / fadeDuration);
            whiteOverlay.color = new Color(1, 1, 1, alpha);
            yield return null;
        }

        // Make the screen fully transparent again
        whiteOverlay.color = new Color(1, 1, 1, 0);
    }
}
