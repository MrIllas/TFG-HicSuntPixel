using Globals;
using SaveSystem;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Character.Player
{
    public class PlayerManager : CharacterManager
    {
#if UNITY_EDITOR
        [Header("Debug only")]
        [SerializeField] bool respawnCharacter = false;
        [SerializeField] bool switchRightWeapon = false;
#endif

        public string characterName = "";

        [HideInInspector] public PlayerAnimatorManager _playerAnimatorManager;
        [HideInInspector] public PlayerLocomotion _playerLocomotion;
        [HideInInspector] public PlayerEquipmentManager _equipmentManager;
        [HideInInspector] public PlayerInventoryManager _inventoryManager;

        protected override void Awake()
        {
            base.Awake();

            _playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
            _playerLocomotion = _snapPoint.GetComponent<PlayerLocomotion>();
            _statsManager = GetComponent<PlayerStatsManager>();
            _inventoryManager = GetComponent<PlayerInventoryManager>();
            _equipmentManager = GetComponent<PlayerEquipmentManager>();
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
            _statsManager.RegenerateStamina();
#if UNITY_EDITOR
            DebugMenu();
#endif
        }

        protected override void OnSpawn()
        {
            base.OnSpawn();

            PlayerInputManager.instance._player = this;
            WorldSaveGameManager.instance.player = this;

            // Updates UI when stats changes
            _statsManager.OnCurrentHealthChanged += PlayerUIManager.instance._playerUIHudManager.SetNewHealthValue;
            _statsManager.OnMaxHealthChanged += PlayerUIManager.instance._playerUIHudManager.SetMaxHealthValue;
            _statsManager.OnCurrentStaminaChanged += PlayerUIManager.instance._playerUIHudManager.SetNewStaminaValue;
            _statsManager.OnCurrentStaminaChanged += _statsManager.ResetStaminaRegenTimer;
            _statsManager.OnMaxStaminaChanged += PlayerUIManager.instance._playerUIHudManager.SetMaxStaminaValue;

            // Update Item model when the equiped item on a slot changes
            //_equipmentManager.OnRightHandItemIdChange += 

            //Stats
            _statsManager.OnSpawn();
        }

        public override IEnumerator DeathEvent(bool manuallySelectDeathAnimation = false)
        {
            PlayerUIManager.instance._playerHUDPopUpManager.SendYouAreDeadPopUp();


            return base.DeathEvent(manuallySelectDeathAnimation);
        }

        public override void ReviveCharacter()
        {
            base.ReviveCharacter();

            _statsManager.CurrentHealth = _statsManager.MaxHealth;
            _statsManager.CurrentStamina = _statsManager.MaxStamina;

            //Play revie animation
            _animatorManager.PlayTargetActionAnimation("Empty", false);
        }

        public void SaveGameDataToCurrentCharacterData(ref CharacterSaveData currentCharacterData)
        {
            currentCharacterData.sceneIndex = SceneManager.GetActiveScene().buildIndex;
            currentCharacterData.characterName = characterName;
            currentCharacterData.SavePosition(_snapPoint.transform.position);
            currentCharacterData.SaveOrientation(_snapPoint.transform.rotation.eulerAngles);

            _statsManager.OnSave(ref currentCharacterData);
        }

        public void LoadGameDataFromCurrentCharacterData(ref CharacterSaveData currentCharacterData)
        {
            characterName = currentCharacterData.characterName;

            // Load Position
            Vector3 position = Vector3.zero;
            currentCharacterData.LoadPosition(ref position);
            _snapPoint.transform.position = position;
            Vector3 rotation = Vector3.zero;
            currentCharacterData.LoadOrientation(ref rotation);
            _snapPoint.transform.eulerAngles = rotation;

            _statsManager.OnLoad(ref currentCharacterData);
        }

        // Since i'm using a snap point i need to call this animation function from here,
        // since the animation and the locomotion are in different GO
        public void ApplyJumpingVelocity()
        {
            _playerLocomotion.ApplyJumpingVelocity();

        }

#if UNITY_EDITOR
        private void DebugMenu()
        {
            if (respawnCharacter)
            {
                respawnCharacter = false;
                ReviveCharacter();
            }

            if (switchRightWeapon) 
            {
                switchRightWeapon = false;
                _equipmentManager.SwitchRightItem();
            }
        }
#endif
    }
}

