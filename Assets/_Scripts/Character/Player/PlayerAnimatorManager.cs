using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Character.Player
{
    public class PlayerAnimatorManager : CharacterAnimatorManager
    {
        PlayerManager _player;

        protected override void Awake()
        {
            base.Awake();

            _player = GetComponent<PlayerManager>();
        }

        private void OnAnimatorMove()
        {
            Vector3 velocity = _player._animator.deltaPosition;
            _player._characterController.Move(velocity);
            _player.transform.rotation *= _player._animator.deltaRotation;
        }
    }
}

