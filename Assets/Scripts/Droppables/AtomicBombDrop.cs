using System.Collections;
using UnityEngine;

public class AtomicBombDrop : MonoBehaviour
{
    [SerializeField] private GameObject atomicBombPrefab;

 private Camera mainCamera;
 private Camera topCamera;
    [SerializeField] private float timeSlowFactor = 0.1f;
    [SerializeField] private AudioClip bombFallSound;
    [SerializeField] private AudioClip bombImpactSound;
    [SerializeField] private float explosionRadius = 20.0f;
    [SerializeField] private float explosionForce = 500.0f;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GameObject.FindGameObjectWithTag("PlayerUI").GetComponent<AudioSource>();
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        topCamera = GameObject.FindGameObjectWithTag("TopCamera").GetComponent<Camera>();
    }

    public void ActivateAtomicBomb()
    {
        StartCoroutine(AtomicBombSequence());
    }

    private IEnumerator AtomicBombSequence()
    {
        // Slow down time
        Time.timeScale = timeSlowFactor;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;

        // Spawn the bomb above the player and switch camera
        GameObject bomb = Instantiate(atomicBombPrefab, transform.position + Vector3.up * 5, Quaternion.LookRotation(Vector3.down));

        bomb.GetComponent<AtomicBomb>().mainCamera = mainCamera;
        bomb.GetComponent<AtomicBomb>().topCamera = topCamera;
       
        mainCamera.gameObject.SetActive(false);
        topCamera.gameObject.SetActive(false);


        // Play bomb falling sound
        audioSource.clip = bombFallSound;
        audioSource.Play();

      
        yield return new WaitForSeconds(2.0f);

        // Switch back to the main camera after the explosion
        topCamera.gameObject.SetActive(true);
        mainCamera.gameObject.SetActive(true);
       
        // Reset time scale
       
    }
}
