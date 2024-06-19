using UnityEngine;

namespace Character.Npc
{
    public class NpcState : ScriptableObject
    {
        public virtual NpcState Tick(NpcManager npcManager)
        {
            Debug.Log("We are running this state");
            return this;
        }
    }
}