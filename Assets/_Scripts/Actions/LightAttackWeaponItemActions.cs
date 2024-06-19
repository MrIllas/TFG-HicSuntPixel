using Character.Player;
using UnityEngine;

namespace Items
{
    [CreateAssetMenu(menuName = "Character Actions/Weapon Actions/Light Attack Action")]
    public class LightAttackWeaponItemActions : ItemBasedAction
    {
        [SerializeField] string lightAttack01 = "Main_Light_Attack_01";

        public override void AttemptToPerformAction(PlayerManager origin, DisplayableItem item)
        {
            base.AttemptToPerformAction(origin, item);

            // Check for stops (Anything that might stop the action) Like not having stamina
            if (origin._statsManager.CurrentStamina <= 0) return; // No stamina
            if (!origin.isGrounded) return; // On air

            PerformLightAttack(origin, item);
        }
    
        private void PerformLightAttack(PlayerManager origin, DisplayableItem weapon) 
        {
            if (origin._playerCombatManager.IsUsingRightHand) 
            {
                origin._animatorManager.PlayTargetAttackActionAnimation(AttackType.Light_Attack_01, lightAttack01, true);
                return;
            }

            if (origin._playerCombatManager.IsUsingLeftHand)
            {

                return;
            }
        }
    }
}