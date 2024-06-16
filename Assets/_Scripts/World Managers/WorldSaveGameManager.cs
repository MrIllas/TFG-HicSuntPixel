using Character;
using Character.Player;
using HicSuntPixel;
using Menus;
using SaveSystem;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Globals
{
    public class WorldSaveGameManager : MonoBehaviour
    {
        public static WorldSaveGameManager instance;

        [SerializeField] public PlayerManager player;

        [Header("World Scene Index ")]
        [SerializeField] int worldSceneIndex = 1;
        public int GetWorldSceneIndex() => worldSceneIndex;

        [Header("Main Menu Scene Index")]
        [SerializeField] int mainMenuSceneIndex = 0;
        public int GetMainMenuSceneIndex() => mainMenuSceneIndex;

        [Header("SAVE / LOAD")]
        public float automaticSaveInterval = 10.0f;
        public bool saveAutomatically = true;
        private bool automaticSaveCoroutineStarted = false;
        [SerializeField] bool saveGame;
        [SerializeField] bool loadGame;

        [Header("Save Data Writer")]
        private SaveFileDataWriter _dataWritter;

        [Header("Current Chracter Data")]
        public SaveSlot currentSlot;
        public CharacterSaveData currentCharacterData;
        private string saveFileName;

        [Header("Character Slots")]
        public CharacterSaveData[] characterSlots = new CharacterSaveData[3];

        public void Awake()
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
            DontDestroyOnLoad(gameObject);

            PreloadSlots();
        }

        public bool CurrentlyInTitleScreen()
        {
            return SceneManager.GetActiveScene().buildIndex == mainMenuSceneIndex;
        }

        public string GetFileNameBySlot(SaveSlot saveSlot)
        {
            string toReturn = "";
            switch (saveSlot)
            {
                case SaveSlot.Slot_01:
                    toReturn = "characterSlot_01";
                    break;
                case SaveSlot.Slot_02:
                    toReturn = "characterSlot_02";
                    break;
                case SaveSlot.Slot_03:
                    toReturn = "characterSlot_03";
                    break;
            }

            return toReturn;
        }

        private void Update()
        {
            if (saveGame) 
            {
                saveGame = false;
                SaveGame();
            }

            if (loadGame)
            {  
                loadGame = false; 
                LoadGame();
            }
            UpdatePlayTime();

            if(!automaticSaveCoroutineStarted && saveAutomatically && player != null)
            {
                StartCoroutine(AutomaticSaving());
                automaticSaveCoroutineStarted = true;
            }
        }

        public void ReturnToTitleScene()
        {
            currentSlot = SaveSlot.NO_SLOT;
            currentCharacterData = null;
            saveFileName = "";

            StartCoroutine(LoadBackToTitleScene());
        }

        public void AttemptToCreateNewGame()
        {
            _dataWritter = new SaveFileDataWriter();
            _dataWritter.saveDataDirectoryPath = Application.persistentDataPath;
            // Check to see if we can create a new save file (check for other existing files first)

            for (int i = 0; i < characterSlots.Length; i++) 
            {
                _dataWritter.saveFileName = GetFileNameBySlot(SaveSlot.Slot_01 + i);
                if (!_dataWritter.CheckFileExistence())
                {
                    currentSlot = SaveSlot.Slot_01 + i;
                    currentCharacterData = new CharacterSaveData();
                    NewGame();
                    return;
                }
            }

            // If there are no new slots
            TitleScreenManager.instance.DisplayNoFreeCharacterSlotsPopUp();
        }

        public void NewGame()
        {
            // Save the current file under a file name depending on which slot we are usin
            saveFileName = GetFileNameBySlot(currentSlot);

            _dataWritter = new SaveFileDataWriter();
            // Generally works on multiple machine types
            _dataWritter.saveDataDirectoryPath = Application.persistentDataPath;
            _dataWritter.saveFileName = saveFileName;

            CharacterStatsManager stats = new CharacterStatsManager();
            stats.OnNewGame(ref currentCharacterData);

            // Write that info onto a json file, saved to this machine
            _dataWritter.CreateNewSaveFile(currentCharacterData);

            StartCoroutine(LoadWorldScene());
        }

        public void LoadGame()
        {
            // Load a previous file, with a file name depending on which slot we are using
            saveFileName = GetFileNameBySlot(currentSlot);

            _dataWritter = new SaveFileDataWriter();
            // Generally works on multiple machine types
            _dataWritter.saveDataDirectoryPath = Application.persistentDataPath;
            _dataWritter.saveFileName = saveFileName;
            currentCharacterData = _dataWritter.LoadSaveFile<CharacterSaveData>();

            StartCoroutine(LoadWorldScene());
        }

        public bool SaveGame()
        {
            // Save the current file under a file name depending on which slot we are usin
            saveFileName = GetFileNameBySlot(currentSlot);

            _dataWritter = new SaveFileDataWriter();
            // Generally works on multiple machine types
            _dataWritter.saveDataDirectoryPath = Application.persistentDataPath;
            _dataWritter.saveFileName = saveFileName;

            // Pass the players info, from game, to their save file
            player.SaveGameDataToCurrentCharacterData(ref currentCharacterData);

            // Write that info onto a json file, saved to this machine
            return _dataWritter.CreateNewSaveFile(currentCharacterData);
        }

        public IEnumerator SaveGameAsync(System.Action<bool, float> callback)
        {
            float startTime = Time.realtimeSinceStartup;

            // Save the current file under a file name depending on which slot we are using
            saveFileName = GetFileNameBySlot(currentSlot);

            _dataWritter = new SaveFileDataWriter();
            // Generally works on multiple machine types
            _dataWritter.saveDataDirectoryPath = Application.persistentDataPath;
            _dataWritter.saveFileName = saveFileName;

            // Pass the players info, from game, to their save file
            player.SaveGameDataToCurrentCharacterData(ref currentCharacterData);

            // Simulate saving to file in chunks
            bool saveSuccessful = false;
            yield return new WaitForEndOfFrame(); // Simulate waiting for an end of frame
            saveSuccessful = _dataWritter.CreateNewSaveFile(currentCharacterData);

            // Call the callback function with the result
            callback?.Invoke(saveSuccessful, Time.realtimeSinceStartup - startTime);
        }

        public void DeleteGameSlot(SaveSlot characterSlot)
        {
            // Chose a file to delete base on name
            _dataWritter = new SaveFileDataWriter();
            _dataWritter.saveDataDirectoryPath = Application.persistentDataPath;
            _dataWritter.saveFileName = GetFileNameBySlot(characterSlot); ;
            _dataWritter.DeleteSaveFile();

        }

        // Load all slots profiles on device when starting game
        private void PreloadSlots()
        {
            _dataWritter = new SaveFileDataWriter();
            _dataWritter.saveDataDirectoryPath = Application.persistentDataPath;

            for (int i = 0; i < characterSlots.Length; ++i)
            {
                _dataWritter.saveFileName = GetFileNameBySlot(SaveSlot.Slot_01 + i);
                characterSlots[i] = _dataWritter.LoadSaveFile<CharacterSaveData>();
            }
        }

        private void UpdatePlayTime()
        {
            if (player == null) return;
            if (PauseMenuManager._GameIsPaused) return;

            currentCharacterData.secondsPlayed += Time.deltaTime;
        }

        // Ultimately loads the saved scene and passes on the save data
        public IEnumerator LoadWorldScene()
        {
            // Loads the saved scene
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(currentCharacterData.sceneIndex);

            while (!loadOperation.isDone) 
            {
                yield return null;
            }
            
            player.LoadGameDataFromCurrentCharacterData(ref currentCharacterData);
        }

        public IEnumerator LoadBackToTitleScene()
        {
            CameraController.instance.gameObject.SetActive(false); // Need this to not get "Error: NO TARGET SET TO THE CAMERA."

            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(mainMenuSceneIndex);

            while(!loadOperation.isDone)
            {
                yield return null;
            }

            // Clear instances
            CameraController.ClearInstance();
            PauseMenuManager.ClearInstance();
            PlayerUIManager.ClearInstance();

            ResetGameState();
        }

        private void ResetGameState()
        {
            Time.timeScale = 1.0f;
            PreloadSlots();
        }

        public IEnumerator AutomaticSaving()
        {
            float elapsedTime = 0.0f;
            while (player != null)
            {
                elapsedTime += Time.deltaTime;
               
                if (elapsedTime > automaticSaveInterval)
                {
                    if (PauseMenuManager.instance != null)
                    {
                        Debug.Log("Saving automatically!");
                        StartCoroutine(SaveGameAsync(PauseMenuManager.instance.OnSaveComplete));
                    }
                    elapsedTime = 0.0f;
                }

                if (!saveAutomatically)
                {
                    automaticSaveCoroutineStarted = false;
                    yield break;
                }

                
                yield return null;
            }
            automaticSaveCoroutineStarted = false;
        }
    }
}