using UnityEngine;
using Character;
using Globals;
using System.Collections.Generic;

public class DamageCollider : MonoBehaviour
{
    [Header("Collider")]
    protected Collider _collider;

    [Header("Damage")]
    public int physicalDamage = 0;
    public int magicDamage = 0;
    public int fireDamage = 0;
    public int lightningDamage = 0;
    public int holyDamage = 0;

    [Header("Contact Point")]
    protected Vector3 contactPoint;

    [Header("Chracter Damaged")]
    protected List<CharacterManager> charactersDamaged = new List<CharacterManager>();


    protected virtual void Awake()
    {
        if (_collider == null)
        {
            _collider = GetComponent<Collider>();
        }
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        CharacterManager target = other.GetComponentInParent<CharacterLocomotion>()._character;

        if (target != null ) 
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

    protected virtual void DamageTarget(CharacterManager target)
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

        target._characterEffectsManager.ProcessInstantEffect(damageEffect);
    }

    public virtual void EnableDamageCollider()
    {
        _collider.enabled = true;
    }

    public virtual void DisableDamageCollider() 
    {
        _collider.enabled = false;
        charactersDamaged.Clear();
    }
}
