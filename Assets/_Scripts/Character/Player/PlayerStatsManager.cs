using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Character.Player
{
    public class PlayerStatsManager : CharacterStatsManager
    {

        PlayerManager _player;

        protected override void Awake()
        {
            base.Awake();

            _player = GetComponent<PlayerManager>();
        }

        protected override void Start()
        {
            base.Start();

            //OnSpawn();
        }
    }
}
