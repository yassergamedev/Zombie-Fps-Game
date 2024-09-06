using System.Collections;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] private Image whiteOverlay; // Reference to the white overlay UI

    private AudioSource audioSource;

    void Start()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>().health = 9999;
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.useGravity = true;

        whiteOverlay = GameObject.FindGameObjectWithTag("White Overlay").GetComponent<Image>();
        audioSource = GetComponent<AudioSource>();
        GameUI = GameObject.FindGameObjectWithTag("GameUI");
        GameUI.SetActive(false);

        if (whiteOverlay != null)
        {
            whiteOverlay.color = new Color(1, 1, 1, 0); // Fully transparent
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Atomic Collided with " + other.tag);
        if (other.CompareTag("Ground") || other.CompareTag("Player"))
        {
            StartCoroutine(TriggerExplosion());
        }
    }

    private IEnumerator TriggerExplosion()
    {
        bombCamera.transform.SetParent(null);

        Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        audioSource.clip = bombImpactSound;
        audioSource.Play();

        StartCoroutine(CameraShake(0.5f, 0.5f));
        StartCoroutine(FlashWhiteScreen(0.5f, 4.5f));

        // Find all zombies in the scene
        Zombie[] allZombies = FindObjectsOfType<Zombie>();

        foreach (Zombie zombie in allZombies)
        {
            Rigidbody rb = zombie.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Apply explosion force (adjust position to center of explosion)
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, 1.0f, ForceMode.Impulse);
            }

            // Kill the zombie by calling the Die or TakeDamage method
            zombie.TakeDamage(zombie.health); 
        }

        Time.timeScale = 1.0f;
        Time.fixedDeltaTime = 0.02f;

        yield return new WaitForSeconds(cameraSwitchDelay);

        bombCamera.gameObject.SetActive(false);
        topCamera.gameObject.SetActive(true);
        mainCamera.gameObject.SetActive(true);
        GameUI.SetActive(true);

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

    private IEnumerator FlashWhiteScreen(float flashDuration, float fadeDuration)
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>().health = 100;
        if (whiteOverlay == null) yield break;

        whiteOverlay.color = new Color(1, 1, 1, 1); // Fully white
        yield return new WaitForSeconds(flashDuration);

        float elapsedTime = 0.0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, elapsedTime / fadeDuration);
            whiteOverlay.color = new Color(1, 1, 1, alpha);
            yield return null;
        }

        whiteOverlay.color = new Color(1, 1, 1, 0);
    }
}
