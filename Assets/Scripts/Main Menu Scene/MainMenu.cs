using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class MainMenu : MonoBehaviour
{
    public TMP_Text highestWaveText; // Reference to the UI Text component

    void Start()
    {
        // Get the highest wave from PlayerPrefs
        int highestWave = PlayerPrefs.GetInt("WaveMax", 0);

        // Update the UI Text component with the highest wave value
        highestWaveText.text =  highestWave.ToString();

        // Apply saved settings
        ApplySettings();
    }

    public void ApplySettings()
    {
        // Apply Resolution
        int resolutionIndex = PlayerPrefs.GetInt("ResolutionIndex", 0);
        Resolution[] resolutions = Screen.resolutions;
        if (resolutionIndex >= 0 && resolutionIndex < resolutions.Length)
        {
            Resolution resolution = resolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        }

        // Apply Mouse Sensitivity
        float mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 1f);
        GameObject.FindWithTag("MainCamera")?.GetComponent<MouseLook>().SetMouseSensitivity(mouseSensitivity);

        // Apply Fullscreen Mode
        bool isFullScreen = PlayerPrefs.GetInt("FullScreen", 1) == 1;
        Screen.fullScreen = isFullScreen;
    }
}
