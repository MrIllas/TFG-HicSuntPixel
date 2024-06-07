using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Globals
{
    public class WorldSaveGameManager : MonoBehaviour
    {
        public static WorldSaveGameManager instance;

        [SerializeField] int worldSceneIndex = 1;
        public int GetWorldSceneIndex() => worldSceneIndex;
        [SerializeField] int mainMenuSceneIndex = 0;
        public int GetMainMenuSceneIndex() => mainMenuSceneIndex;
        

        public void Awake()
        {
            // Singleton creator
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
        }

        public IEnumerator LoadNewGame()
        {
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(worldSceneIndex);

            yield return null;
        }
    }
}