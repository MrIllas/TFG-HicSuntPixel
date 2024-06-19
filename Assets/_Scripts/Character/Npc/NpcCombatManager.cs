using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Character.Npc
{
    public class NpcCombatManager : CharacterCombatManager
    {
        [Header("Detection")]
        [SerializeField] float detectionRadius = 5.0f;
        [SerializeField] float minimumDetectionAngle = -35.0f;
        [SerializeField] float maximumDetectionAngle = 35.0f;

        public void FindATargetViaLineOfSight(NpcManager npcManager)
        {
            if (currentTarget != null) return;

            Collider[] colliders = Physics.OverlapSphere(npcManager.transform.position, detectionRadius, WorldUtilityManager.instance.GetCharacterLayers());
        
            for (int i = 0; i < colliders.Length; i++) 
            {
                CharacterManager target = colliders[i].transform.GetComponent<CharacterLocomotion>()._character;

                if (target == null) continue;
                if (target == npcManager) continue;
                if (target._statsManager.IsDead) continue;

                if (WorldUtilityManager.instance.IsThisTargetMyEnemy(_characterManager.faction, target.faction))
                {
                    Vector3 targetDir = target.transform.position - npcManager.transform.position;
                    float viewAngle = Vector3.Angle(targetDir, npcManager.transform.forward);
                
                    if (viewAngle > minimumDetectionAngle && viewAngle < maximumDetectionAngle) 
                    {
                        if (Physics.Linecast(npcManager._characterCombatManager.myLockTarget.position, target._characterCombatManager.myLockTarget.position))
                        {
                            Debug.DrawLine(npcManager._characterCombatManager.myLockTarget.position, target._characterCombatManager.myLockTarget.position, Color.red, WorldUtilityManager.instance.GetEnviornemntalLayers());
                            Debug.Log("Blocked");
                        }
                        else
                        {
                            npcManager._characterCombatManager.SetTarget(target);
                            Debug.Log(gameObject.name + " is locked target '"+target.gameObject.name+"'");
                        }
                    }
                }

            }
        }
    }
}