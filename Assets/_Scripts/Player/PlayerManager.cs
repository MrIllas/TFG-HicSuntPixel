using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Character.Player
{

    PlayerLocomotion _playerLocomotion;
    public class PlayerManager : CharacterManager
    {
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

