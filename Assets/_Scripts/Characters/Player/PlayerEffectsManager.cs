using UnityEngine;

namespace Character.Player
{
    public class PlayerEffectsManager : CharacterEffectsManager
    {
        [Header("Debug Delete later")]
        [SerializeField] InstantCharacterEffect effectToTest;
        [SerializeField] bool processEffect = false;

        private void Update()
        {
            if (processEffect) 
            {
                processEffect = false;
                // Needs to make a copy (instantiate) so it can be modified (liike in CalculateStaminaDamage) without altering the original (values)
                InstantCharacterEffect effect = Instantiate(effectToTest);
                ProcessInstantEffect(effect);
            }
        }
    }
}