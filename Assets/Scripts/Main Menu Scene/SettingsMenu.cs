using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using System.Collections.Generic;

public class SettingsMenu : MonoBehaviour
{
    public Slider sensitivitySlider; // Reference to the Slider component
    public TMP_Dropdown resolutionDropdown; // Reference to the Dropdown component
    public Toggle fullscreenToggle; // Reference to the Toggle component

    public Animator transitionAnimator; // Reference to the Animator for the transition image

    private SetResolution resolutionManager; // Reference to the SetResolution script

    private Resolution[] resolutions;

    void Start()
    {
        // Get the SetResolution component
        resolutionManager = FindObjectOfType<SetResolution>();

        // Load and set initial values for UI elements
        sensitivitySlider.value = PlayerPrefs.GetFloat("MouseSensitivity", 1f);

        // Populate resolutions dropdown
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
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
        StartCoroutine (QuitGame());
    }
}
