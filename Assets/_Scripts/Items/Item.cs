using UnityEngine;

namespace Items
{
    public class Item : ScriptableObject
    {
        [Header("Item Information")]
        public int itemId;
        public string itemName;
        public Sprite icon;
        [TextArea] public string description;
     
    }
}