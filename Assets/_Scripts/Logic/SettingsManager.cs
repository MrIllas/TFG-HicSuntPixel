using SaveSystem;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager instance;

    Resolution[] resolutions;
    public Resolution[] GetResolutions() => resolutions;

    // Info
    private string BuildNumber = "0";
    public string version = "v.0.0";

    [Header("Settings Saving")]
    public string settingsFileName = "settings";
    public SettingsSaveData data = new SettingsSaveData();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            // Get Version info
            ResourceRequest request = Resources.LoadAsync("Build", typeof(BuildScriptableObject));
            request.completed += Request_completed;

            // Gather information about what resolutions can be used
            resolutions = Screen.resolutions;

            LoadSettings();
            SetSettingsOnInitialization();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        SaveSettings();
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
        data.fullscreen = isFullscreen;
        Screen.fullScreen = isFullscreen;
    }

    public void SetResolution(int resolutionIndex)
    {
        data.resolutionIndex = resolutionIndex;
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, data.fullscreen);
    }

    public List<string> GetResolutionsStringList()
    {
        List<string> options = new List<string>();
        for (int i = 0; i < resolutions.Length; ++i)
        {
            options.Add(resolutions[i].width + " x " + resolutions[i].height + " @" + resolutions[i].refreshRateRatio);

            //if (resolutions[i].width == Screen.currentResolution.width &&
            //    resolutions[i].height == Screen.currentResolution.height && 
            //    resolutions[i].refreshRateRatio.ToString() == Screen.currentResolution.refreshRateRatio.ToString())
            //{
            //    Debug.Log("Hey-> " + resolutions[i].width + " x " + resolutions[i].height + " @" + resolutions[i].refreshRateRatio);
            //    Debug.Log("Ho-> " + Screen.currentResolution.width + " x " + Screen.currentResolution.height + " @" + Screen.currentResolution.refreshRateRatio.ToString());
            //    data.resolutionIndex = i;
            //}
        }
        return options;
    }
    #endregion

    #region Settings UI Functions

    public void InitializeVideoUI(ref TMP_Dropdown resolutionDropdown, ref Toggle fullscreenToggle)
    {
        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(GetResolutionsStringList());
        resolutionDropdown.value = data.resolutionIndex;
        resolutionDropdown.RefreshShownValue();

        fullscreenToggle.isOn = data.fullscreen;
    }

    public void OnSetResolutionDropdownClick(int resolutionIndex)
    {
        SetResolution(resolutionIndex);
    }

    public void OnFullscreenToggleClick(bool isFullscreen)
    {
        SetFullscreen(isFullscreen);
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

    #region Load / Save Settings

    public void SaveSettings()
    {
        SaveFileDataWriter writter = new SaveFileDataWriter();

        // Save to the Game folder
        writter.saveDataDirectoryPath = Application.dataPath;
        writter.saveFileName = settingsFileName;

        writter.CreateNewSaveFile(data);
    }

    public void LoadSettings()
    {
        SaveFileDataWriter writter = new SaveFileDataWriter();

        // Save to the Game folder
        writter.saveDataDirectoryPath = Application.dataPath;
        writter.saveFileName = settingsFileName;

        SettingsSaveData aux = writter.LoadSaveFile<SettingsSaveData>();
        if (aux != null) data = aux;
        else
        {
            //Set an starting resolution, otherwise 
            data.resolutionIndex = resolutions.Length - 1; 
        }
    }

    private void SetSettingsOnInitialization()
    {
        SetResolution(data.resolutionIndex);
        SetFullscreen(data.fullscreen);
    }
    #endregion
}