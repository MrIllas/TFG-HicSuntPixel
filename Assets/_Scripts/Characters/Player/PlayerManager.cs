using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Character.Player
{
    public class PlayerManager : CharacterManager
    {
        PlayerLocomotion _playerLocomotion;

        protected override void Awake()
        {
            base.Awake();

            _playerLocomotion = GetComponent<PlayerLocomotion>();
        }

        protected override void Update()
        {
            base.Update();

            _playerLocomotion.HandleAllMovement();
        }
    }
}

