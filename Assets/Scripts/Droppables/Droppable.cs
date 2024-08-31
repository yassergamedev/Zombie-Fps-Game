using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Droppable : MonoBehaviour
{
    // UnityEvent that gets triggered when the player collides with this object
    public UnityEvent onDropped;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
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

        // Destroy the droppable object
        Destroy(gameObject);
    }
}
