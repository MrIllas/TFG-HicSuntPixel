using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;

namespace Character.Player
{
    public class PlayerUIHudManager : MonoBehaviour
    {
        [SerializeField] UI_StatBar staminaBar;

        public void SetNewStaminaValue(float oldValue, float newValue)
        {
            staminaBar.SetStat(newValue);
        }

        public void SetMaxStaminaValue(int oldValue, int newValue)
        {
            staminaBar.SetMaxStat(newValue);
        }
    }
}

