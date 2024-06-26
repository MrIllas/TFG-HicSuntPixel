using UnityEngine;

namespace Items
{
    public class WeaponItem : DisplayableItem
    {
        // Animator Controller OVERRIDE ( Change the attack animations based on weapon on hand)
        [Header("Weapon Requirements")]
        public int levelREQ = 0;

        [Header("Weapon Base Damage")]
        public int physicalDamage = 0;
        public int magicDamage = 0;
        public int fireDamage = 0;
        public int lightningDamage = 0;
        public int holyDamage = 0;

        //[Header("Weapon Base Poise Damage")]
        //public float poiseDamage = 10;

        // Weapon modifiers
        [Header("Attack Modifiers")]
        public float lightAttack01Modifier = 1; //The combo step additional damage
        // Critical damage modifier

        [Header("Stamina Costs")]
        public int baseStaminaCost = 20; // Stamina used on weapon swing
        public float lightAttackStaminaCostMultiplier = 1; //The light attack additional cost
        // Light attack stamina modifier
        // heavy attack stamina modifier

        // Item Based action (F1, F2, F3)
    }
}