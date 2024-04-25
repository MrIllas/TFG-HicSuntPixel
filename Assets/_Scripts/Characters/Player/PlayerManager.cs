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

            _playerLocomotion = GetComponent<PlayerLocomotion>();
            _playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
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

