using Items;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Globals
{
    public class WorldItemDatabase : MonoBehaviour
    {
        public static WorldItemDatabase instance;

        [SerializeField] public WeaponItem unarmedWeapon;
        [Header("Weapons")]
        [SerializeField] private List<WeaponItem> weapons = new List<WeaponItem>();

        // A list of every item in the game
        [Header("Items")]
        private List<Item> items = new List<Item>();

        private void Awake()
        {
            if (instance == null) 
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            DontDestroyOnLoad(gameObject);

            // Add all weapons to the list of items
            foreach (var weapon in weapons) 
            {
                items.Add(weapon);
            }

            // Assign all of our items a unique item id
            for (int i = 0; i < items.Count; ++i)
            {
                items[i].itemId = i;
            }
        }

        public WeaponItem GetWeaponById(int id)
        {
            return weapons.FirstOrDefault(weapon => weapon.itemId == id);
        }
    }
}