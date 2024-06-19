using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Character.Npc
{
    [CreateAssetMenu(menuName = "NPC/States/Idle")]
    public class Npc : NpcState
    {
        public override NpcState Tick(NpcManager npcManager)
        {
            if (npcManager._characterCombatManager.currentTarget != null)
            {
                // Return the pursue target
                Debug.Log("Target found");
                return this;
            }
            else
            {
                Debug.Log("Searching for a Target");
                npcManager._npcCombatManager.FindATargetViaLineOfSight(npcManager);

                return this;
            }
        }

       
    }
}