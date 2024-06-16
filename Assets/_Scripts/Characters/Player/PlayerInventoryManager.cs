using Items;
using UnityEngine;

namespace Character.Player
{
    public class PlayerInventoryManager : MonoBehaviour
    {
        public DisplayableItem currentRightHandItem;
        public DisplayableItem currentLeftHandItem;

        [Header("Quick Slots TESTING")]
        public WeaponItem[] weaponsInRightHandSlots = new WeaponItem[3];
        public int rightHandWeaponIndex = -1;

    }
}