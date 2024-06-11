using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;


namespace SaveSystem
{
    public class SaveFileDataWriter
    {
        public string saveDataDirectoryPath = "";
        public string saveFileName = "";

        // Before we create a new file, we must check to see if one of this chracter slot already exists (max 10 character slots)
        public bool CheckFileExistence()
        {
            if (File.Exists(Path.Combine(saveDataDirectoryPath, saveFileName)))
            {
                return true;
            }
            return false;
        }

        // Used to delete chracter save files
        public void DeleteSaveFile()
        {
            File.Delete(Path.Combine(saveDataDirectoryPath, saveFileName));
        }

        // Used to create a new file upon starting a new game
        public bool CreateNewSaveFile(CharacterSaveData characterData)
        {
            // Make a path to save the file (A location on the machine)
            string savePath = Path.Combine(saveDataDirectoryPath, saveFileName);

            try
            {
                // Create the directory the file will be written to, if it does not already exists
                Directory.CreateDirectory(Path.GetDirectoryName(savePath));
                Debug.Log("Creating save file, at save path: " + savePath);

                // Serialize the C# game data object into JSON
                string dataToStore = JsonUtility.ToJson(characterData, true);

                // Write the file to our system
                using (FileStream stream = new FileStream(savePath, FileMode.Create))
                {
                    using (StreamWriter fileWriter = new StreamWriter(stream))
                    {
                        fileWriter.Write(dataToStore);
                    }
                }
                return true;
            }
            catch (Exception e) 
            {
                Debug.LogError("ERROR WHILIST TRYING TO SAVE CHARACTER DATA, GAME NOT SAVED" + savePath + "\n" + e);
                return false;
            }
        }

        // Used to load a save file upon loading a previous game
        public CharacterSaveData LoadSaveFile()
        {
            CharacterSaveData data = null;
            // Make a path to save the file (A location on the machine)
            string loadPath = Path.Combine(saveDataDirectoryPath, saveFileName);

            if (File.Exists(loadPath))
            {
                try
                {
                    string dataToLoad = "";

                    using (FileStream stream = new FileStream(loadPath, FileMode.Open))
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            dataToLoad = reader.ReadToEnd();
                        }
                    }

                    // Deserialize the data from JSON back to unity
                    data = JsonUtility.FromJson<CharacterSaveData>(dataToLoad);
                }
                catch (Exception e)
                {
                    Debug.Log("File is blank!");
                }
            }

            return data;
        }
    }
}
