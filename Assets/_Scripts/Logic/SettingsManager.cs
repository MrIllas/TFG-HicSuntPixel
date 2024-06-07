using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager instance;

    //Video
    public bool _isFullscreen = true;
    public int currentResolutionIndex = 0;

    Resolution[] resolutions;
    public Resolution[] GetResolutions() => resolutions;

    // Info
    private string BuildNumber = "1";
    public string version = "v.0.0";

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Get Version info
        ResourceRequest request = Resources.LoadAsync("Build", typeof(BuildScriptableObject));
        request.completed += Request_completed;

        // Gather information about what resolutions can be used
        resolutions = Screen.resolutions;
        _isFullscreen = Screen.fullScreen;
    }

    #region Audio Related
    public void SetVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);
    }
    #endregion

    #region Video Related
    public void SetFullscreen(bool isFullscreen)
    {
        _isFullscreen = isFullscreen;
        Screen.fullScreen = isFullscreen;
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, _isFullscreen);
    }

    public List<string> GetResolutionsStringList()
    {
        List<string> options = new List<string>();
        for (int i = 0; i < resolutions.Length; ++i)
        {
            options.Add(resolutions[i].width + " x " + resolutions[i].height);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }
        return options;
    }
    #endregion

    private void Request_completed(AsyncOperation op)
    {
        BuildScriptableObject buildScriptableObject = ((ResourceRequest)op).asset as BuildScriptableObject;

        if (buildScriptableObject == null)
        {
            Debug.LogError("Build Scriptable Object not found in resources directory! Check Build log for errors.");
        }
        else
        {
            version = "v. " + Application.version + "." + buildScriptableObject.BuildNumber;

        }
    }
}