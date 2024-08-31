using UnityEngine;

public class SetResolution : MonoBehaviour
{
    public int defaultWidth = 1920;
    public int defaultHeight = 1080;
    public bool defaultFullscreen = true;

    void Start()
    {
        // Load resolution settings from PlayerPrefs
        int width = PlayerPrefs.GetInt("ScreenWidth", defaultWidth);
        int height = PlayerPrefs.GetInt("ScreenHeight", defaultHeight);
        bool fullscreen = PlayerPrefs.GetInt("ScreenFullscreen", defaultFullscreen ? 1 : 0) == 1;

        // Set the resolution
        Screen.SetResolution(1920, 1080, fullscreen);
    }

    public void SetScreenResolution(int width, int height, bool fullscreen)
    {
        // Set the resolution
        Screen.SetResolution(width, height, fullscreen);

        // Save the resolution settings to PlayerPrefs
        PlayerPrefs.SetInt("ScreenWidth", width);
        PlayerPrefs.SetInt("ScreenHeight", height);
        PlayerPrefs.SetInt("ScreenFullscreen", fullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }
}
