using Globals;
using Menus;
using SaveSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UI
{
    public class UI_Character_Save_Slot : MonoBehaviour
    {
        SaveFileDataWriter saveFileWriter;

        [Header("Game Slot")]
        public SaveSlot slot;

        [Header("Character Info")]
        public TextMeshProUGUI characterName;
        public TextMeshProUGUI timePlayed;

        private void OnEnable()
        {
            LoadSaveSlots();
        }

        private void LoadSaveSlots()
        {
            saveFileWriter = new SaveFileDataWriter();
            saveFileWriter.saveDataDirectoryPath = Application.persistentDataPath;

            saveFileWriter.saveFileName = WorldSaveGameManager.instance.GetFileNameBySlot(slot);

            // If the file exists get information
            if (saveFileWriter.CheckFileExistence())
            {
                characterName.text = WorldSaveGameManager.instance.characterSlots[(int) slot].characterName;
                timePlayed.text = GetStringTime(WorldSaveGameManager.instance.characterSlots[(int) slot].secondsPlayed);
            }
            // If it doesn't, disable it
            else
            {
                gameObject.SetActive(false);
            }
        }

        public void LoadGameFromCharacterSlot()
        {
            WorldSaveGameManager.instance.currentSlot = slot;
            WorldSaveGameManager.instance.LoadGame();
        }

        public void SelectCurrentSlot()
        {
            TitleScreenManager.instance.SelectCharacterSlot(slot);
        }

        private string GetStringTime(float seconds)
        {
            int hours = Mathf.FloorToInt(seconds / 3600); 
            int minutes = Mathf.FloorToInt((seconds % 3600) / 60); 
            int secs = Mathf.FloorToInt(seconds % 60);

            // Format the time as HH:MM:SS
            return string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, secs);
        }
    }
}