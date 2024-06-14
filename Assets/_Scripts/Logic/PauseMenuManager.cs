using Globals;
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

        //SubMenus
        [Header("Main Menus")]
        [SerializeField] private GameObject _pauseMenu;
        [SerializeField] private GameObject _mainMenu;
        [SerializeField] private GameObject _settingsMenu;

        [Space(10)]

        // Settings
        [Header("Settings Menus")]
        [SerializeField] private GameObject _videoSettingsMenu;
        [SerializeField] private GameObject _audioSettingsMenu;
        [SerializeField] private GameObject _otherSettingsMenu;

        [Space(10)]

        [Header("Pop Ups")]
        [SerializeField] private UI_Save_Game_Notification _saveNotificationPopUp;

        [Space(10)]

        // Sub menus 
        [Header("Video Settings")]
        //[SerializeField] private TMP_Dropdown resolutionDropdown;
        //[SerializeField] private Toggle fullscreenToggle;

        [Header("Audio Settings")]

        [Header("Other Settings")]
        [SerializeField] private Toggle automaticSavingToggle;

        [Header("Other")]
        [SerializeField] private TMP_Text _versionText;
        [SerializeField] private TMP_Dropdown resolutionDropdown;
        [SerializeField] private Toggle fullscreenToggle;

        public static void ClearInstance()
        {
            if (instance != null)
            {
                Destroy(instance.gameObject);
                instance = null;
            }
            _GameIsPaused = false;
        }

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

            _pauseMenu.SetActive(false);
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);

            //Sub (Disable GameObject if in main menu)
            SceneManager.activeSceneChanged += OnSceneChange;

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
            _pauseMenu.SetActive(false);
            Time.timeScale = 1.0f;
        }

        void Pause()
        {
            _pauseMenu.SetActive(true);

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
            _settingsMenu.SetActive(false);
            _mainMenu.SetActive(true);

            //Settings
            _videoSettingsMenu.SetActive(false);
            _audioSettingsMenu.SetActive(false);
            _otherSettingsMenu.SetActive(false);
        }

        #region Root Button Functions
        public void OnResumeButtonClick()
        {
            Resume();
            _GameIsPaused = false;
        }

        public void OnQuitToDesktopButtonClick()
        {
            if (WorldSaveGameManager.instance.SaveGame())
            {
                #if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
                #else
                    Application.Quit();
                #endif
            }
        }

        public void OnQuitToMainMenuButtonClick()
        {
            if (WorldSaveGameManager.instance.SaveGame())
            {
                WorldSaveGameManager.instance.ReturnToTitleScene();
            }
        }

        // Save Game
        public void OnSaveButtonClick()
        {
            StartCoroutine(WorldSaveGameManager.instance.SaveGameAsync(OnSaveComplete));
        }

        public void OnOptionsButtonClick()
        {
            _settingsMenu.SetActive(true);
            _mainMenu.SetActive(false);
        }

        #endregion

        #region Settings UI Functions

        public void OnQuitSettingButtonClick()
        {
            _settingsMenu.SetActive(false);
            _videoSettingsMenu.SetActive(false);
            _audioSettingsMenu.SetActive(false);
            _otherSettingsMenu.SetActive(false);
            _mainMenu.SetActive(true);
        }

        public void OnVideoSettingsButtonClick()
        {
            SettingsManager.instance.InitializeVideoUI(ref resolutionDropdown, ref fullscreenToggle);
            _videoSettingsMenu.SetActive(true);
            _audioSettingsMenu.SetActive(false);
            _otherSettingsMenu.SetActive(false);
        }

        public void OnAudioSettingsButtonClick()
        {
            _videoSettingsMenu.SetActive(false);
            _audioSettingsMenu.SetActive(true);
            _otherSettingsMenu.SetActive(false);
        }

        public void OnOtherSettingsButtonClick()
        {
            _videoSettingsMenu.SetActive(false);
            _audioSettingsMenu.SetActive(false);
            _otherSettingsMenu.SetActive(true);

            automaticSavingToggle.isOn = WorldSaveGameManager.instance.saveAutomatically;
        }

        public void OnSetResolutionDropdownClick(int resolutionIndex)
        {
            SettingsManager.instance.SetResolution(resolutionIndex);
        }

        public void OnFullscreenToggleClick(bool isFullscreen)
        {
            SettingsManager.instance.SetFullscreen(isFullscreen);
        }

        public void OnAutomaticSavingToggleClick(bool value)
        {
            WorldSaveGameManager.instance.saveAutomatically = value;
        }

        #endregion

        // SAVE CALLBACK
        public void OnSaveComplete(bool success, float time)
        {
            if (success)
            {
                StartCoroutine(_saveNotificationPopUp.ShowPopup(time));
            }

        }
    }
}