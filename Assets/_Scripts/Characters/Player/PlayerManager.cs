using Globals;
using SaveSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Character.Player
{
    public class PlayerManager : CharacterManager
    {
        public string characterName = "";

        [HideInInspector] public PlayerAnimatorManager _playerAnimatorManager;
        [HideInInspector] public PlayerLocomotion _playerLocomotion;

        protected override void Awake()
        {
            base.Awake();
       
            _playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
            _playerLocomotion = _snapPoint.GetComponent<PlayerLocomotion>();
        }

        protected override void Start()
        {
            base.Start();
            //_playerLocomotion = GetComponent<Snapper>().GetReferenceComponent<PlayerLocomotion>();
        
        }

        protected override void Update()
        {
            base.Update();

            _playerLocomotion.HandleAllMovement();

            if (Input.GetKeyDown(KeyCode.Space)) // Press Space to set the new position
            {
                _snapPoint.transform.position= new Vector3(0,0,0);
            }
        }

        protected override void OnSpawn()
        {
            base.OnSpawn();

            PlayerInputManager.instance._player = this;
            WorldSaveGameManager.instance.player = this;
        }
    
        public void SaveGameDataToCurrentCharacterData(ref CharacterSaveData currentCharacterData)
        {
            currentCharacterData.sceneIndex = SceneManager.GetActiveScene().buildIndex;
            currentCharacterData.characterName = characterName;
            currentCharacterData.SavePosition(_snapPoint.transform.position);
        }

        public void LoadGameDataFromCurrentCharacterData(ref CharacterSaveData currentCharacterData)
        {
            characterName = currentCharacterData.characterName;

            // Load Position
            Vector3 position = Vector3.zero;
            currentCharacterData.LoadPosition(ref position);
            _snapPoint.transform.position = position;
        }
    }
}

