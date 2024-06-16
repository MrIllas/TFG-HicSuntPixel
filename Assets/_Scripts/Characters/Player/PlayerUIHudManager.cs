using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;

namespace Character.Player
{
    public class PlayerUIHudManager : MonoBehaviour
    {
        [SerializeField] UI_StatBar healthBar;
        [SerializeField] UI_StatBar staminaBar;

        public void RefreshHUD()
        {
            healthBar.gameObject.SetActive(false);
            healthBar.gameObject.SetActive(true);

            staminaBar.gameObject.SetActive(false);
            staminaBar.gameObject.SetActive(true);
        }

        public void SetNewHealthValue(int oldValue, int newValue)
        {
            healthBar.SetStat(newValue);
        }

        public void SetMaxHealthValue(int oldValue, int newValue)
        {
            healthBar.SetMaxStat(newValue);
        }

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

