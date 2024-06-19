using Globals;
using Items;
using System;
using UnityEngine;

namespace Character
{
    public class CharacterCombatManager : MonoBehaviour
    {
        protected CharacterManager _characterManager;

        [Header("Target Lock On")]
        public Transform myLockTarget;
        public bool isTargetLocked = false;
        public CharacterManager currentTarget;

        [Header("Attack type")]
        public AttackType currentAttackType;

        protected virtual void Awake()
        {
            _characterManager = GetComponent<CharacterManager>();
        }

        public void SetTarget(CharacterManager target)
        {
            if (target != null)
            {
                currentTarget = target;
            }
            else
            {
                currentTarget = null;
            }
        }
    }
}