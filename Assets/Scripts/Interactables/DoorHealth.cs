using System.Collections;
using UnityEngine;

public class DoorHealth : MonoBehaviour
{
    public float damageDuration = 5f; // Duration for which the door will be damaged
    private Animator doorAnimator; // Animator for the door
    private bool isDamaged = false; // Flag to check if the door is currently being damaged
    private float damageTimer = 0f; // Timer to keep track of duration
    private Door door;
    private AudioSource bangDoor;
    private void Start()
    {
        doorAnimator = GetComponent<Animator>();   
        door= GetComponent<Door>();
        bangDoor = GetComponent<AudioSource>();
        }
   

    public IEnumerator DamageDoor()
    {
        if (!bangDoor.isPlaying)
        {
            bangDoor.Play();
        }
        
        // Wait for the specified duration
        yield return new WaitForSeconds(damageDuration);

        // Set the "Down" trigger to make the door go down
        if (doorAnimator != null)
        {
            doorAnimator.SetTrigger("Down");
        }

        // Optionally, reset the damage state if needed
        isDamaged = false;
        door.isFixed = false;
        GetComponent<Collider>().isTrigger = false;
    }
}
