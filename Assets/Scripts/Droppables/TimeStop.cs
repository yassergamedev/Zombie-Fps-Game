using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TimeStop : MonoBehaviour
{
    public float timeStopDuration = 10f;
    public AudioClip timeStopSound;
  

    private AudioSource audioSource;

    private PlayerUI playerUI;


    void Start()
    {

        playerUI = FindObjectOfType<PlayerUI>();

        if ( playerUI == null)
        {
            Debug.LogError("PlayerUI not found in the scene.");
        }

        audioSource = playerUI.gameObject.GetComponent<AudioSource>();
        
    }

    public void ActivateTimeStop()
    {
        if ( playerUI != null)
        {
            audioSource.clip = timeStopSound;
            audioSource.Play();

            playerUI.StartTimer(timeStopDuration, "Time Stop");
            playerUI.StartTimeStopTimer(timeStopDuration);

        }
    }

   
}
