using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Globals;
using TMPro;
using UnityEngine.UI;
namespace Menus
{
    public class MainMenuManager : MonoBehaviour
    {
        //SubMenus
        [Header("SubMenus")]
        [SerializeField] GameObject _rootButtonsHolder;
        [SerializeField] GameObject _settingsButtonHolder;

        [Space(10)]

        // Settings
        [Header("Settings Panels")]
        [SerializeField] GameObject _videoSettingsPanel;
        [SerializeField] GameObject _audioSettingsPanel;
        [SerializeField] GameObject _otherSettingsPanel;

        [Space(10)]

        [Header("Other")]
        [SerializeField] private TMP_Text _versionText;
        [SerializeField] private TMP_Dropdown resolutionDropdown;
        [SerializeField] private Toggle fullscreenToggle;

        private void Start()
        {
            SettingsManager.instance.InitializeVideoUI(ref resolutionDropdown, ref fullscreenToggle);

            //Set version text
            _versionText.text = SettingsManager.instance.version;
        }

        #region Root Button Functions
        public void OnStartNewGameButtonClick()
        {
            StartCoroutine(Globals.WorldSaveGameManager.instance.LoadNewGame());
        }

        public void OnContinueButtonClick()
        {

        }

        public void OnLoadButtonClick()
        {

        }

        public void OnSettingsButtonClick()
        {
            _rootButtonsHolder.SetActive(false);
            _settingsButtonHolder.SetActive(true);
        }

        public void OnQuitButtonClick()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
        #endregion

        #region Settings Menu Buttons
        public void OnVideoSettingsButtonClick()
        {
            _videoSettingsPanel.SetActive(true);
            _settingsButtonHolder.SetActive(false);
        }

        public void OnAudioSettingsButtonClick()
        {
            _audioSettingsPanel.SetActive(true);
            _settingsButtonHolder.SetActive(false);
        }

        public void OnOtherSettingsButtonClick()
        {
            _otherSettingsPanel.SetActive(true);
            _settingsButtonHolder.SetActive(false);
        }

        public void OnBackSettingsButtonClick()
        {
            _videoSettingsPanel.SetActive(false);
            _audioSettingsPanel.SetActive(false);
            _otherSettingsPanel.SetActive(false);
            _settingsButtonHolder.SetActive(true);
        }
        public void OnBackToRootButtonClick()
        {
            _rootButtonsHolder.SetActive(true);
            _settingsButtonHolder.SetActive(false);
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

