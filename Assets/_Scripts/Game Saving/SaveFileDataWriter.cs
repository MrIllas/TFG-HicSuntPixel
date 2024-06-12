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

        // Create a new save file with any type of data
        public bool CreateNewSaveFile<T>(T data)
        {
            string savePath = Path.Combine(saveDataDirectoryPath, saveFileName);

            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(savePath));
                Debug.Log("Creating save file at save path: " + savePath);

                string dataToStore = JsonUtility.ToJson(data, true);

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
                if (data.GetType() == typeof(CharacterSaveData))
                {
                    Debug.LogError("ERROR WHILE TRYING TO SAVE SLOT DATA, GAME NOT SAVED: " + savePath + "\n" + e);
                }
                else if (data.GetType() == typeof(SettingsSaveData)) 
                {
                    Debug.LogError("ERROR WHILIST TRYING TO SAVE SETTINGS DATA, SETTINGS NOT SAVED" + savePath + "\n" + e);
                }
                
                return false;
            }
        }

        // Load a save file with any type of data
        public T LoadSaveFile<T>() where T : class
        {
            T data = null;
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

                    data = JsonUtility.FromJson<T>(dataToLoad);
                }
                catch (Exception e)
                {
                    if (data.GetType() == typeof(CharacterSaveData))
                    {
                        Debug.LogError("ERROR WHILE LOADING SAVE FILE: " + loadPath + "\n" + e);
                    }
                    else if (data.GetType() == typeof(SettingsSaveData))
                    {
                        Debug.LogError("ERROR WHILE LOADING SETTINGS DATA: " + loadPath + "\n" + e);
                    }
                }
            }

            return data;
        }

    }
}
