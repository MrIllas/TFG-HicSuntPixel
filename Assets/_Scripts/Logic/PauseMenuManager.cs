using Globals;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Menus
{
    public class PauseMenuManager : MonoBehaviour
    {
        public static PauseMenuManager instance;

        public static bool _GameIsPaused = false;

        private Canvas _pauseMenuUI;

        //SubMenus
        [Header("SubMenus")]
        [SerializeField] private GameObject _pauseMenuPanel;
        [SerializeField] private GameObject _settingsMenuPanel;

        [Space(10)]

        // Settings
        [Header("Settings Panels")]
        [SerializeField] private GameObject _videoSettingsPanel;
        [SerializeField] private GameObject _audioSettingsPanel;
        [SerializeField] private GameObject _otherSettingsPanel;

        [Header("Other")]
        [SerializeField] private TMP_Text _versionText;
        [SerializeField] private TMP_Dropdown resolutionDropdown;
        [SerializeField] private Toggle fullscreenToggle;

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

            _pauseMenuUI = GetComponent<Canvas>();
            _pauseMenuUI.enabled = false;
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);

            //Sub (Disable GameObject if in main menu)
            SceneManager.activeSceneChanged += OnSceneChange;

            SettingsManager.instance.InitializeVideoUI(ref resolutionDropdown, ref fullscreenToggle);
            //InitializeResolutionDropdown();

            //Set version text
            _versionText.text = SettingsManager.instance.version;

            SwitchClear();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape)) 
            {
                _GameIsPaused = !_GameIsPaused;

                if (_GameIsPaused) Pause();               
                else Resume();
            }
        }

        void Resume()
        {
            SwitchClear();
            _pauseMenuUI.enabled = false;
            Time.timeScale = 1.0f;
        }

        void Pause()
        {
            _pauseMenuUI.enabled = true;

            Time.timeScale = 0.0f;
        }

        private void OnSceneChange(Scene oldScene, Scene newScene)
        {
            // Enables the pause menu if the player is not on the main menu
            if (newScene.buildIndex == WorldSaveGameManager.instance.GetMainMenuSceneIndex())
            {
                instance.enabled = false;
            }
            else
            {
                instance.enabled = true;
            }
        }

        // Basically sets everything to the satte its desired to be when the menu first opens
        private void SwitchClear()
        {
            //Submenus
            _settingsMenuPanel.SetActive(false);
            _pauseMenuPanel.SetActive(true);

            //Settings
            _videoSettingsPanel.SetActive(false);
            _audioSettingsPanel.SetActive(false);
            _otherSettingsPanel.SetActive(false);
        }

        #region Root Button Functions
        public void OnResumeButtonClick()
        {
            Resume();
            _GameIsPaused = false;
        }

        public void OnQuitToDesktopButtonClick()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }

        public void OnQuitToMainMenuButtonClick()
        {

        }

        public void OnSaveButtonClick()
        { 
        
        }

        public void OnOptionsButtonClick()
        {
            _settingsMenuPanel.SetActive(true);
            _pauseMenuPanel.SetActive(false);
        }
        #endregion

        #region Settings UI Functions

        //private void InitializeResolutionDropdown()
        //{
        //    resolutionDropdown.ClearOptions();
        //    resolutionDropdown.AddOptions(SettingsManager.instance.GetResolutionsStringList());
        //    resolutionDropdown.value = SettingsManager.instance.currentResolutionIndex;
        //    resolutionDropdown.RefreshShownValue();

        //    fullscreenToggle.isOn = SettingsManager.instance._isFullscreen;
        //}

        public void OnQuitSettingButtonClick()
        {
            _settingsMenuPanel.SetActive(false);
            _pauseMenuPanel.SetActive(true);
        }

        public void OnVideoSettingsButtonClick()
        {
            _videoSettingsPanel.SetActive(true);
            _audioSettingsPanel.SetActive(false);
            _otherSettingsPanel.SetActive(false);
        }

        public void OnAudioSettingsButtonClick()
        {
            _videoSettingsPanel.SetActive(false);
            _audioSettingsPanel.SetActive(true);
            _otherSettingsPanel.SetActive(false);
        }

        public void OnOtherSettingsButtonClick()
        {
            _videoSettingsPanel.SetActive(false);
            _audioSettingsPanel.SetActive(false);
            _otherSettingsPanel.SetActive(true);
        }

        public void OnSetResolutionDropdownClick(int resolutionIndex)
        {
            SettingsManager.instance.SetResolution(resolutionIndex);
        }

        public void OnFullscreenToggleClick(bool isFullscreen)
        {
            SettingsManager.instance.SetFullscreen(isFullscreen);
        }

        #endregion
    }
}