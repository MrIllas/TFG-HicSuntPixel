using UnityEngine;

namespace Items
{
    public class DisplayableItem : Item
    {
        [Header("3D Model")]
        public GameObject model;

        [Header("Actions")]
        public ItemBasedAction action_btn_1;
    }
}