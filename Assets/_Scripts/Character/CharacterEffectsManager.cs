using Globals;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Character
{
    public class CharacterEffectsManager : MonoBehaviour
    {
        // Instant Effects (Damage, Heal)

        // Timed Effects (poison, tick heal)

        // Static effects (Adding/removing buffs from items)

        CharacterManager _character;

        [Header("VFX")]
        [SerializeField] GameObject bloodSplatterVFX;

        protected virtual void Awake()
        {
            _character = GetComponent<CharacterManager>();
        }

        public virtual void ProcessInstantEffect(InstantCharacterEffect effect)
        {
            if (effect is TakeHealthDamageEffect e)
            {
                if (e.playDamageAnimation) 
                {
                    _character._animatorManager.PlayTargetActionAnimation("Hit", false);
                }
            }

            effect.ProcessEffect(_character);
        }

        public void PlayBloodSplatterVFX(Vector3 contactPoint)
        {
            // Play the one set in the GO if ther is one, otherwise play a default
            if (bloodSplatterVFX != null) 
            {
                GameObject bloodSplatter = Instantiate(bloodSplatterVFX, contactPoint, Quaternion.identity);
            }
            else
            {
                GameObject bloodSplatter = Instantiate(WorldCharacterEffectsManager.instance.bloodSplatterVFX, contactPoint, Quaternion.identity);
            }
        }
    }
}
