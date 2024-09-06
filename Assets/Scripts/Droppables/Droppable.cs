using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Droppable : MonoBehaviour
{
    // UnityEvent that gets triggered when the player collides with this object
    public UnityEvent onDropped;
    private AudioSource audioSource;
    public float destroyAfter = 0f;
    public float spawnTime = 20f;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        Destroy(this.gameObject, spawnTime);
    }
    private void OnTriggerEnter(Collider other)
    {

      
        // Check if the colliding object is the player
        if (other.CompareTag("Player"))
        {
            // Invoke the event
            onDropped.Invoke();

            // Start coroutine to destroy the object after the event is executed
            StartCoroutine(DestroyAfterEvent());
        }
    }

    private IEnumerator DestroyAfterEvent()
    {
        // Wait until the end of the frame to ensure onDropped is fully executed
        yield return new WaitForEndOfFrame();

        transform.localScale = Vector3.zero;
        // Destroy the droppable object
        Destroy(gameObject, destroyAfter);
    }
}
