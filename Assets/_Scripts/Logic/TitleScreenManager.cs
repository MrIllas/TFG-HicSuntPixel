using UnityEngine;
using Globals;
using TMPro;
using UnityEngine.UI;
namespace Menus
{
    public class TitleScreenManager : MonoBehaviour
    {
        public static TitleScreenManager instance;

        //SubMenus
        [Header("Main Menus")]
        [SerializeField] GameObject _mainMenu;
        [SerializeField] GameObject _loadMenu;
        [SerializeField] GameObject _settingsMenu;

        [Space(10)]

        // Settings
        [Header("Settings Menus")]
        [SerializeField] GameObject _videoSettingsMenu;
        [SerializeField] GameObject _audioSettingsMenu;
        [SerializeField] GameObject _otherSettingsMenu;

        [Space(10)]

        // Pop Ups
        [Header("Pop Ups")]
        [SerializeField] GameObject noSlotsPopUp;
        [SerializeField] GameObject deleteSlotPopUp;

        [Space(10)]

        // Buttons
        [Header("Buttons")]
        [SerializeField] Button noSlotsOkayButton;
        [SerializeField] Button deletePopUpConfirmButton;

        [Space(10)]

        // Sub menus 
        [Header("Video Settings")]
        [SerializeField] private TMP_Dropdown resolutionDropdown;
        [SerializeField] private Toggle fullscreenToggle;

        [Header("Audio Settings")]

        [Header("Other Settings")]
        [SerializeField] private Toggle automaticSavingToggle;

        [Header("Other")]
        [SerializeField] private TMP_Text _versionText;
        [SerializeField] GameObject blackBackground;

        

        SaveSlot currentSelectedSlot = SaveSlot.NO_SLOT;

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
        }

        private void Start()
        {
            //Set version text
            _versionText.text = SettingsManager.instance.version;
        }

        private void Update()
        {
            blackBackground.SetActive(_mainMenu.activeInHierarchy ? false : true);
        }

        public void StartNewGame()
        {
            WorldSaveGameManager.instance.AttemptToCreateNewGame();
        }

        #region Main Menu Functions
        public void OnStartNewGameButtonClick()
        {
           StartNewGame();
        }

        public void OnContinueButtonClick()
        {

        }

        public void OnLoadButtonClick()
        {
            _mainMenu.SetActive(false);
            _loadMenu.SetActive(true);
        }

        public void OnReturnFromLoadButtonClick()
        {
            _mainMenu.SetActive(true);
            _loadMenu.SetActive(false);

        }

        public void OnSettingsButtonClick()
        {
            _mainMenu.SetActive(false);
            _settingsMenu.SetActive(true);
        }

        public void OnQuitButtonClick()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }

        public void DisplayNoFreeCharacterSlotsPopUp()
        {
            noSlotsPopUp.SetActive(true);
            noSlotsOkayButton.Select();
        }

        public void CloseNoFreeCharacterSlotsPopUp()
        {
            noSlotsPopUp.SetActive(false);
        }

        #endregion

        #region Load Functions
        public void SelectCharacterSlot(SaveSlot characterSlot)
        {
            currentSelectedSlot = characterSlot;
        }

        public void SelectNoSlot()
        {
            currentSelectedSlot = SaveSlot.NO_SLOT;
        }

        public void AttemptToDeleteCharacterSlot()
        {
            _loadMenu.SetActive(false);
            if (currentSelectedSlot != SaveSlot.NO_SLOT)
            {
                deleteSlotPopUp.SetActive(true);
                deletePopUpConfirmButton.Select();
            }
        }

        public void DeleteCharacterSlot()
        {
            deleteSlotPopUp.SetActive(false);
            WorldSaveGameManager.instance.DeleteGameSlot(currentSelectedSlot);
            _loadMenu.SetActive(true);
        }

        public void CloseDeleteCharacterPopUp()
        {
            deleteSlotPopUp.SetActive(false);
            _loadMenu.SetActive(true);
        }
        #endregion

        #region Settings Menu Functions
        public void OnVideoSettingsButtonClick()
        {
            SettingsManager.instance.InitializeVideoUI(ref resolutionDropdown, ref fullscreenToggle);
            _videoSettingsMenu.SetActive(true);
            _settingsMenu.SetActive(false);
        }

        public void OnAudioSettingsButtonClick()
        {
            _audioSettingsMenu.SetActive(true);
            _settingsMenu.SetActive(false);
        }

        public void OnOtherSettingsButtonClick()
        {
            _otherSettingsMenu.SetActive(true);
            _settingsMenu.SetActive(false);

            automaticSavingToggle.isOn = WorldSaveGameManager.instance.saveAutomatically;
        }

        public void OnBackSettingsButtonClick()
        {
            _videoSettingsMenu.SetActive(false);
            _audioSettingsMenu.SetActive(false);
            _otherSettingsMenu.SetActive(false);
            _settingsMenu.SetActive(true);
        }
        public void OnBackToRootButtonClick()
        {
            _mainMenu.SetActive(true);
            _settingsMenu.SetActive(false);
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
    }
}

