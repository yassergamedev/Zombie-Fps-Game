using System.Collections;
using UnityEngine;

public class AtomicBomb : MonoBehaviour
{
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private float explosionRadius = 20.0f;
    [SerializeField] private float explosionForce = 500.0f;
    [SerializeField] private AudioClip bombImpactSound;
    [SerializeField] public Camera bombCamera;
    [SerializeField] public Camera mainCamera;
    [SerializeField] public Camera topCamera;
    [SerializeField] public GameObject GameUI;
    [SerializeField] private float cameraSwitchDelay = 1.0f;

    private AudioSource audioSource;

    void Start()
    {
        // Ensure the bomb has a Rigidbody component to fall due to gravity
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.useGravity = true;

    
        // Get the audio source from the bomb itself
        audioSource = GetComponent<AudioSource>();
        GameUI = GameObject.FindGameObjectWithTag("GameUI");
        GameUI.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Atomic Collided with " + other.tag);
        // Check if the bomb hit the ground or a valid surface
        if (other.CompareTag("Ground") || other.CompareTag("Player"))
        {
            // Trigger the explosion
            StartCoroutine(TriggerExplosion());
        }
    }

    private IEnumerator TriggerExplosion()
    {
        // Decouple the camera from the bomb and stop it from following the bomb
        bombCamera.transform.SetParent(null);

        // Instantiate the explosion prefab
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        // Play the impact sound
        audioSource.clip = bombImpactSound;
        audioSource.Play();

        // Start the camera shake coroutine
        StartCoroutine(CameraShake(0.5f, 0.5f)); // Adjust duration and magnitude as needed

        // Find all zombies within the explosion radius
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider nearbyObject in colliders)
        {
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            Zombie zombie = nearbyObject.GetComponent<Zombie>();

            if (rb != null)
            {
                // Apply upward force to zombies
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, 1.0f, ForceMode.Impulse);
            }

            if (zombie != null)
            {
                // Kill the zombie
                zombie.TakeDamage(zombie.health);
            }
        }

      
        Time.timeScale = 1.0f;
        Time.fixedDeltaTime = 0.02f;
        // Wait for the camera switch delay to allow the explosion to be captured
        yield return new WaitForSeconds(cameraSwitchDelay);

        // Switch back to the main camera
        bombCamera.gameObject.SetActive(false);
        topCamera.gameObject.SetActive(true);
        mainCamera.gameObject.SetActive(true);
        GameUI.SetActive(true);
        // Destroy the bomb after explosion (delay destruction to allow camera capture)
        Destroy(gameObject, cameraSwitchDelay);
    }

    private IEnumerator CameraShake(float duration, float magnitude)
    {
        Vector3 originalPosition = bombCamera.transform.localPosition;

        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            bombCamera.gameObject.transform.localPosition = new Vector3(x, y, originalPosition.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        bombCamera.gameObject.transform.localPosition = originalPosition;
    }

    private IEnumerator SwitchBackToMainCamera()
    {
        Time.timeScale = 1.0f;
        Time.fixedDeltaTime = 0.02f;
        // Wait for the camera switch delay to allow the explosion to be captured
        yield return new WaitForSeconds(cameraSwitchDelay);

        // Switch back to the main camera
        bombCamera.gameObject.SetActive(false);
        mainCamera.gameObject.SetActive(true);
    }
}
