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

        CharacterManager character;

        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();
        }

        public virtual void ProcessInstantEffect(InstantCharacterEffect effect)
        {
            effect.ProcessEffect(character);
        }
    }
}
