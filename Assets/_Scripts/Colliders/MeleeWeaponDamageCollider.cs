using Character;
using Globals;
using UnityEngine;

public class MeleeWeaponDamageCollider : DamageCollider
{
    [Header("Attacking Character")]
    public CharacterManager characterCausingDamage;

    [Header("Weapon Attack Modifiers")]
    public float lightAttack01Modifier;

    protected override void Awake()
    {
        base.Awake();

        _collider.enabled = false;
    }

    protected override void OnTriggerEnter(Collider other)
    {
        CharacterManager target = other.GetComponentInParent<CharacterLocomotion>()._character;

        if (target == characterCausingDamage) return; //Prevent self damage;

        if (target != null)
        {
            target = other.GetComponent<CharacterLocomotion>()._character;
        }

        if (target != null)
        {
            contactPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);

            // Check if we can damage this target (Is It friendly?)

            //Check if target is blocking

            // Check if target is invunerable
            if (target._statsManager.IsDead) return;
            if (target._statsManager.IsInvunerable) return;

            // Damage
            DamageTarget(target);
        }
    }

    protected override void DamageTarget(CharacterManager target)
    {
        // We don't want to damage the same target more than once in a single attack
        if (charactersDamaged.Contains(target)) return;
        charactersDamaged.Add(target);

        TakeHealthDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeHealthDamageEffect);
        damageEffect.physicalDamage = physicalDamage;
        damageEffect.magicDamage = magicDamage;
        damageEffect.fireDamage = fireDamage;
        damageEffect.holyDamage = holyDamage;
        damageEffect.lightningDamage = lightningDamage;

        switch (characterCausingDamage._characterCombatManager.currentAttackType)
        {
            case AttackType.Light_Attack_01:
                ApplyAttackDamageModifiers(lightAttack01Modifier, damageEffect);
                break;
            default:
                break;
        }

        target._characterEffectsManager.ProcessInstantEffect(damageEffect);
    }

    private void ApplyAttackDamageModifiers(float modifier, TakeHealthDamageEffect damage)
    {
        damage.physicalDamage = (int) (physicalDamage * modifier);
        damage.magicDamage *= (int)(magicDamage * modifier);
        damage.fireDamage *= (int)(fireDamage * modifier);
        damage.holyDamage *= (int)(holyDamage * modifier);
        damage.lightningDamage *= (int)(lightningDamage * modifier);
    }
}


