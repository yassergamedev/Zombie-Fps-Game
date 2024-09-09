using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using System.Collections.Generic;
using UnityEngine.Rendering;

public class SettingsMenu : MonoBehaviour
{
    public Slider sensitivitySlider; // Reference to the Slider component for mouse sensitivity
    public TMP_Dropdown resolutionDropdown; // Reference to the Dropdown component for resolution
    public Toggle fullscreenToggle; // Reference to the Toggle component for fullscreen
    public Slider volumeSlider; // Reference to the Slider component for sound volume

    public Animator transitionAnimator; // Reference to the Animator for the transition image

    private SetResolution resolutionManager; // Reference to the SetResolution script

    private Resolution[] resolutions;

    void Start()
    {
        // Get the SetResolution component
        resolutionManager = FindObjectOfType<SetResolution>();

        // Load and set initial values for UI elements
        sensitivitySlider.value = PlayerPrefs.GetFloat("MouseSensitivity", 1f);
        volumeSlider.value = PlayerPrefs.GetFloat("Volume", 1f); // Load volume setting
        AudioListener.volume = volumeSlider.value; // Adjust the volume of all audio sources in the scene
        // Populate resolutions dropdown
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = resolutions.Length - 1, j = 0; i >= 0; i--, j++)
        {
            string option = resolutions[j].width + " x " + resolutions[j].height;
            options.Add(option);

            if (resolutions[j].width == Screen.currentResolution.width &&
                resolutions[j].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = j;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = PlayerPrefs.GetInt("ResolutionIndex", currentResolutionIndex);
        resolutionDropdown.RefreshShownValue();

        fullscreenToggle.isOn = PlayerPrefs.GetInt("Fullscreen", 1) == 1;

        // Add listeners for UI changes
        sensitivitySlider.onValueChanged.AddListener(SetMouseSensitivity);
        resolutionDropdown.onValueChanged.AddListener(SetResolution);
        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
        volumeSlider.onValueChanged.AddListener(SetVolume);
    }

    // Function to set the mouse sensitivity
    public void SetMouseSensitivity(float sensitivity)
    {
        PlayerPrefs.SetFloat("MouseSensitivity", sensitivity);
    }

    // Function to set the resolution
    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];

        PlayerPrefs.SetInt("ResolutionIndex", resolutionIndex);
        resolutionManager.SetScreenResolution(resolution.width, resolution.height, fullscreenToggle.isOn);
    }

    // Function to set fullscreen
    public void SetFullscreen(bool isFullscreen)
    {
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
        resolutionManager.SetScreenResolution(Screen.width, Screen.height, isFullscreen);
    }

    // Function to set the volume
    public void SetVolume(float volume)
    {
        PlayerPrefs.SetFloat("Volume", volume);
        AudioListener.volume = volume; // Adjust the volume of all audio sources in the scene
    }

    // Function to change scenes with a transition
    public void ChangeScene(string sceneName)
    {
        StartCoroutine(LoadSceneWithTransition(sceneName));
    }

    public void ChangeSceneNormal(string sceneName)
    {
        Time.timeScale = 1;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene(sceneName);
    }

    // Coroutine to handle the transition and scene loading
    private IEnumerator LoadSceneWithTransition(string sceneName)
    {
        transitionAnimator.SetTrigger("Fade In");

        // Wait until the animation has finished (assuming the transition animation length is 1 second)
        yield return new WaitForSeconds(2);

        // Load the new scene
        SceneManager.LoadScene(sceneName);
    }

    public IEnumerator QuitGame()
    {
        transitionAnimator.SetTrigger("Fade In");

        // Wait until the animation has finished (assuming the transition animation length is 1 second)
        yield return new WaitForSeconds(2);

        Application.Quit();
    }

    public void QuitG()
    {
        StartCoroutine(QuitGame());
    }
}
