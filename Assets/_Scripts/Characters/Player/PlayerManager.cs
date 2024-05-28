using UnityEngine;

namespace Character.Player
{
    public class PlayerManager : CharacterManager
    {
        [HideInInspector] public PlayerAnimatorManager _playerAnimatorManager;
        [HideInInspector] public PlayerLocomotion _playerLocomotion;

        protected override void Awake()
        {
            base.Awake();

           
            _playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
        }

        protected override void Start()
        {
            base.Start();
            _playerLocomotion = GetComponent<Snapper>().GetReferenceComponent<PlayerLocomotion>();
        }

        protected override void Update()
        {
            base.Update();

            _playerLocomotion.HandleAllMovement();
        }

        protected override void OnSpawn()
        {
            base.OnSpawn();

            PlayerInputManager.instance._player = this;
        }
    }
}

