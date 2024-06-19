using Character.Player;
using UnityEngine;

namespace Items
{
    [CreateAssetMenu(menuName = "Character Actions/Item Actions/Test Action")]
    public class ItemBasedAction : ScriptableObject
    {
        public int actionId;

        public virtual void AttemptToPerformAction(PlayerManager origin, DisplayableItem item)
        {
            origin._playerCombatManager.CurrentItemBeingUsed = item.itemId;

            //Debug.Log("Action has fired!");
        }
    }
}