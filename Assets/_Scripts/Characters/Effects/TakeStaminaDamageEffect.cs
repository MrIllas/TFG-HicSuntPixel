using UnityEngine;

namespace Character
{
    [CreateAssetMenu(menuName = "Character Effects/Instant Effects/Take Stamina Damage")]
    public class TakeStaminaDamageEffect : InstantCharacterEffect
    {

        public float staminaDamage;

        public override void ProcessEffect(CharacterManager character)
        {
            base.ProcessEffect(character);

            CalculateStaminaDamage(character);
        }

        private void CalculateStaminaDamage(CharacterManager character) 
        {
            // Compare the base stamina damage against other player effects/modifiers 
            // Change value before effect (Examaple if the character has a buff that reduces stamina damage taken)
            character._statsManager.CurrentStamina -= staminaDamage;
        }
    }
}
