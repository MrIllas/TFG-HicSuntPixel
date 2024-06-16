using UI;
using UnityEngine;

namespace Character.Player
{
    public class PlayerUIManager : MonoBehaviour
    {
        public static PlayerUIManager instance;

        [HideInInspector] public PlayerUIHudManager _playerUIHudManager;
        [HideInInspector] public PlayerHUDPopUpManager _playerHUDPopUpManager;

        public static void ClearInstance()
        {
            if (instance != null)
            {
                Destroy(instance.gameObject);
                instance = null;
            }
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

            _playerUIHudManager = GetComponentInChildren<PlayerUIHudManager>();
            _playerHUDPopUpManager = GetComponentInChildren<PlayerHUDPopUpManager>();
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}

