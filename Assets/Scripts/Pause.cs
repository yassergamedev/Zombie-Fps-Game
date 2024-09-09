using System.Drawing;
using UnityEngine;
using UnityEngine.UI;
public class Pause : MonoBehaviour
{
    public GameObject pauseMenu; // Assign the PauseMenu GameObject in the Inspector
    public GameObject[] gameObjectsToDeactivate; // Assign GameObjects to deactivate in the Inspector

    private bool isPaused = false;

    private PlayerUI playerUI;
    private void Start()
    {
        playerUI = FindAnyObjectByType<PlayerUI>();
    }
    void Update()
    {
        // Toggle pause state when Escape key is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    void PauseGame()
    {
        if(FindObjectOfType<PlayerHealth>().health > 0)
        {
            isPaused = true;
            // Show the pause menu
            pauseMenu.SetActive(true);
            // Pause the game
            Time.timeScale = 0;
            // Deactivate specified game objects
            SetGameObjectsActive(false);
            // Ensure cursor is visible
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            playerUI.PauseTimers();
            AudioListener.volume = 0f;
        }
       
        
    }

    public void ResumeGame()
    {
        isPaused = false;
        // Hide the pause menu
        pauseMenu.SetActive(false);
        // Resume the game
        Time.timeScale = 1;
        // Reactivate specified game objects
        SetGameObjectsActive(true);
        // Hide cursor if needed
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        playerUI.ResumeTimers();
        GameObject.FindGameObjectWithTag("White Overlay").GetComponent<Image>().color = new UnityEngine.Color(1, 1, 1, 0);
        StartCoroutine(playerUI.gameObject.GetComponent<PlayerHealth>().HealthRegenDelay());
        AudioListener.volume = PlayerPrefs.GetFloat("Volume", 1f);
    }

    void SetGameObjectsActive(bool isActive)
    {
        foreach (GameObject obj in gameObjectsToDeactivate)
        {
            obj.SetActive(isActive);
           
        }
    }
}
