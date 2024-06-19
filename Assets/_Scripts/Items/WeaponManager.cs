using Character;
using Items;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public MeleeWeaponDamageCollider _collider;

    private void Awake()
    {
        _collider = GetComponentInChildren<MeleeWeaponDamageCollider>();
    }

    public void SetWeaponDamage(CharacterManager character, WeaponItem weapon)
    {
        _collider.characterCausingDamage = character;
        _collider.physicalDamage = weapon.physicalDamage;
        _collider.magicDamage = weapon.magicDamage;
        _collider.fireDamage = weapon.fireDamage;
        _collider.lightningDamage = weapon.lightningDamage;
        _collider.holyDamage = weapon.holyDamage;

        _collider.lightAttack01Modifier = weapon.lightAttack01Modifier;
    }
}
