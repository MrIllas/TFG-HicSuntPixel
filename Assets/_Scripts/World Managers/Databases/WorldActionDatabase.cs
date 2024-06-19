using Items;
using System.Linq;
using UnityEngine;

namespace Globals
{
    public class WorldActionDatabase : MonoBehaviour
    {
        public static WorldActionDatabase instance;

        [Header("Weapon Item Actions")]
        public ItemBasedAction[] weaponItemActions;

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

            for (int i = 0; i < weaponItemActions.Length; ++i)
            {
                weaponItemActions[i].actionId = i;
            }
        }

        public ItemBasedAction GetWeaponItemActionByID(int id)
        {
            return weaponItemActions.FirstOrDefault(action => action.actionId == id);
        }
    }
}