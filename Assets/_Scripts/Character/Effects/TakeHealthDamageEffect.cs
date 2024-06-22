using System.Collections.Generic;
using UnityEngine;

namespace Character
{
    [CreateAssetMenu(menuName = "Character Effects/Instant Effects/Take Health Damage")]
    public class TakeHealthDamageEffect : InstantCharacterEffect
    {
        [Header("Character Causing Damage")]
        public CharacterManager origin;

        [Header("Damage")]
        public int physicalDamage = 0;
        public int magicDamage = 0;
        public int fireDamage = 0;
        public int lightningDamage = 0;
        public int holyDamage = 0;

        [Header("Final Damage")]
        private int finalDamage = 0;

        [Header("Animation")]
        public bool playDamageAnimation = true;
        public bool manuallySelectDamageAnimation = false;
        public string damageAnimation;

        [Header("Sound FX")]
        public bool willPlayDamageSFX = true;
        public AudioClip elementalDamageSoundFX; // Used on top of regular SFX if there is elemental damage present

        [Header("Direction Damage Taken From")]
        public float angleHitFrom; // Used to detemine what damage animation to play
        public Vector3 contactPoint; // Used to determine where the blood effect instantiate

        public override void ProcessEffect(CharacterManager character)
        {
            base.ProcessEffect(character);

            // IF CHARACTER IS DEAD OR INVUNERABLE, DO NOT PROCESS MORE DAMAGE
            if (character._statsManager.IsDead) return;
            if (character._statsManager.IsInvunerable) return;

            // Calculate Damage
            CalculateDamage(character);
            // Check which directin damage came from

            // Play damage animation

            // Check for build ups (poison, bleed, etc)

            // Play damage sound FX

            PlayDamageVFX(character);

        }

        private void CalculateDamage(CharacterManager character) 
        {
            if (origin != null)
            {
                // Check for modifiers that could alter the damage (like a trinket)
            }

            // Check own flat damage defenses and substact them from the damage

            // Check for amour absorptions, and susbtract from the damage

            // Add all damage types together, and apply final damage

            finalDamage = Mathf.RoundToInt(physicalDamage + magicDamage + fireDamage + lightningDamage + holyDamage);

            //If the damage is less than 0, just hit 1hp
            if (finalDamage <= 0)
            {
                finalDamage = 1;
            }

            character._statsManager.CurrentHealth -= finalDamage;
        }

        private void PlayDamageVFX(CharacterManager character)
        {
            // If we have fir damage, play fire particles, lightning etc...

            character._characterEffectsManager.PlayBloodSplatterVFX(contactPoint);
        }
    }
}
