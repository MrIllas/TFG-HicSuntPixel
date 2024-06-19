using Character.Npc;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Globals
{
    [System.Serializable]
    public struct Npc
    {
        public GameObject npcToInstance;
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 scale;
    }

    public class WorldNpcManager : MonoBehaviour
    {
        public static WorldNpcManager instance;

#if UNITY_EDITOR
        [Header("Debug")]
        [SerializeField] bool enableNpcs = false;
        [SerializeField] bool disableNpcs = false;
        [SerializeField] bool respawnNpcs = false;
        [SerializeField] bool despawnNpcs = false;
#endif

        [Header("Npcs")]
        [SerializeField] Npc[] npcs;
        [SerializeField] List<GameObject> spawnedNpcs;

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
            StartCoroutine(WaitForSceneToLoadThenSpawnNpcs());
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (enableNpcs)
            {
                enableNpcs = false;
                EnableAllNpcs();
            }

            if (disableNpcs)
            {
                disableNpcs = false;
                DisableAllNpcs();
            }

            if (respawnNpcs) 
            {
                respawnNpcs = false;
                SpawnAllNpcs();
            }

            if (despawnNpcs)
            {
                despawnNpcs = false;
                DespawnAllNpcs();
            }
        }
#endif

        private IEnumerator WaitForSceneToLoadThenSpawnNpcs()
        {
            while (!SceneManager.GetActiveScene().isLoaded)
            {
                yield return null;
            }

            SpawnAllNpcs();
        }

        private void SpawnAllNpcs()
        {
            foreach(var npc in npcs) 
            {
                GameObject toInstance = Instantiate(npc.npcToInstance);
                NpcManager aux = toInstance.GetComponent<NpcManager>();

                if (aux._snapPoint != null) 
                {
                    aux._snapPoint.transform.position = npc.position;
                    aux._snapPoint.transform.rotation = Quaternion.Euler(npc.rotation);
                    aux._snapPoint.transform.localScale = npc.scale;
                }
                else
                {
                    toInstance.transform.position = npc.position;
                    toInstance.transform.rotation = Quaternion.Euler(npc.rotation);
                    toInstance.transform.localScale = npc.scale;
                }

                

                toInstance.GetComponent<NpcManager>().OnSpawn();
                spawnedNpcs.Add(toInstance);
            }
        }

        private void DespawnAllNpcs()
        {
            foreach(var npc in spawnedNpcs)
            {
                npc.GetComponent<NpcManager>().OnDespawn();
            }
            spawnedNpcs.Clear();
        }

        private void EnableAllNpcs()
        {
            foreach (var npc in spawnedNpcs)
            {
                npc.SetActive(true);
            }
        }

        private void DisableAllNpcs()
        {
            foreach (var npc in spawnedNpcs)
            {
                npc.SetActive(false);
            }
        }
    }
}