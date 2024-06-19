using Globals;
using Items;
using System;
using UnityEngine;

namespace Character.Player
{
    public class PlayerCombatManager : CharacterCombatManager
    {
        PlayerManager _playerManager;


        private DisplayableItem currentItemBeingUsed; //Item performing action
        public int CurrentItemBeingUsed
        {
            get => currentItemBeingUsed.itemId;
            set
            {
                OnCurrentItemBeingUsedChanged?.Invoke(currentItemBeingUsed.itemId, value);
                currentItemBeingUsed = Instantiate(WorldItemDatabase.instance.GetWeaponById(value));
            }
        }
        public event Action<int, int> OnCurrentItemBeingUsedChanged;

        private bool isUsingRightHand = false;
        public bool IsUsingRightHand
        {
            get => isUsingRightHand;
            set
            {
                OnIsUsingRightHandChanged?.Invoke(IsUsingRightHand, value);
                isUsingRightHand = value;

                if (value)
                {
                    IsUsingLeftHand = false; // If is using another hand action, cancel it
                    PerformItemBasedAction(_playerManager._playerInventoryManager.currentRightHandItem.action_btn_1, _playerManager._playerInventoryManager.currentRightHandItem);
                }
            }
        }
        public event Action<bool, bool> OnIsUsingRightHandChanged;

        private bool isUsingLeftHand = false;
        public bool IsUsingLeftHand
        {
            get => isUsingLeftHand;
            set
            {
                OnIsUsingLeftHandChanged?.Invoke(isUsingLeftHand, value);
                isUsingLeftHand = value;

                if (value)
                {
                    IsUsingRightHand = false; // If is using another hand action, cancel it
                    PerformItemBasedAction(_playerManager._playerInventoryManager.currentLeftHandItem.action_btn_1, _playerManager._playerInventoryManager.currentLeftHandItem);
                }
            }
        }
        public event Action<bool, bool> OnIsUsingLeftHandChanged;

        protected override void Awake()
        {
            base.Awake();

            _playerManager = GetComponent<PlayerManager>();
        }

        public void PerformItemBasedAction(ItemBasedAction itemAction, DisplayableItem itemPerformingAction)
        {
            itemAction.AttemptToPerformAction(_playerManager, itemPerformingAction);
        }

        public virtual void DrainStaminaBasedOnAttack()
        {
            float staminaDeducted = 0;

            if (currentItemBeingUsed == null) return;

            switch (currentAttackType)
            {
                case AttackType.Light_Attack_01:
                    staminaDeducted = WorldItemDatabase.instance.GetWeaponById(CurrentItemBeingUsed).baseStaminaCost * WorldItemDatabase.instance.GetWeaponById(CurrentItemBeingUsed).lightAttackStaminaCostMultiplier;
                    break;
            }

            if (staminaDeducted < 0) staminaDeducted = 0;
            _playerManager._statsManager.CurrentStamina -= staminaDeducted;
        }
    }
}