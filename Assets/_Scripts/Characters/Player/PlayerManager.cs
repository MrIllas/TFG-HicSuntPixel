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
        [HideInInspector] public PlayerStatsManager _playerStatsManager;

        protected override void Awake()
        {
            base.Awake();
       
            _playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
            _playerLocomotion = _snapPoint.GetComponent<PlayerLocomotion>();
            _playerStatsManager = GetComponent<PlayerStatsManager>();
        }

        protected override void Start()
        {
            base.Start();
            //_playerLocomotion = GetComponent<Snapper>().GetReferenceComponent<PlayerLocomotion>();
        
        }

        protected override void Update()
        {
            base.Update();

            // Player movement
            _playerLocomotion.HandleAllMovement();

            // REGEN STAMINA
            _playerStatsManager.RegenerateStamina();
        }

        protected override void OnSpawn()
        {
            base.OnSpawn();

            PlayerInputManager.instance._player = this;
            WorldSaveGameManager.instance.player = this;

            // Automatically sets UI values when the value of the stat changes
            _playerStatsManager.OnCurrentStaminaChanged += PlayerUIManager.instance._playerUIHudManager.SetNewStaminaValue;
            _playerStatsManager.OnCurrentStaminaChanged += _playerStatsManager.ResetStaminaRegenTimer; 
            _playerStatsManager.OnMaxStaminaChanged += PlayerUIManager.instance._playerUIHudManager.SetMaxStaminaValue;

            _playerStatsManager.OnSpawn();
            //PlayerUIManager.instance._playerUIHudManager.SetMaxStaminaValue(_playerStatsManager.MaxStamina); // Needs to set one game starts
            
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

        // Since i'm using a snap point i need to call this animation function from here,
        // since the animation and the locomotion are in different GO
        public void ApplyJumpingVelocity()
        {
            _playerLocomotion.ApplyJumpingVelocity();

        }
    }
}

