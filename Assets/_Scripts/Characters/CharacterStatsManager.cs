using Character.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Character
{

    public class CharacterStatsManager : MonoBehaviour
    {
        CharacterManager _characterManager;
        //Stats
        public int endurance;
        //public int Endurance
        //{
        //    get => endurance;
        //    set
        //    {
        //        if (endurance != value)
        //        {
        //            endurance = value;
        //            OnEnduranceChanged?.Invoke(endurance);
        //        }
        //    }
        //}

        //public event Action<int> OnEnduranceChanged;

        // Stamina
        private float currentStamina;
        public float CurrentStamina
        {
            get => currentStamina;
            set
            {
                if (currentStamina != value)
                {
                    OnCurrentStaminaChanged?.Invoke(currentStamina, value); //Returns the percentual value
                    currentStamina = value;  
                }
            }
        }
        public event Action<float, float> OnCurrentStaminaChanged;

        private int maxStamina;
        public int MaxStamina
        {
            get => maxStamina;
            set
            {
                if (maxStamina != value)
                {
                    OnMaxStaminaChanged?.Invoke(maxStamina, value);
                    maxStamina = value;
                }
            }
        }
        public event Action<int, int> OnMaxStaminaChanged;


        [Header("Stamina Regeneration")]
        [SerializeField] int staminaRegenerationAmount = 2;
        private float staminaRegenerationTimer = 0;
        [SerializeField] float staminaRegenerationDelay = 2;
        private float staminaTickTimer = 0;

        protected virtual void Awake()
        {
            _characterManager = GetComponent<CharacterManager>();
        }

        // Functions

        public void OnSpawn()
        {
            CurrentStamina = CalculateStamina(endurance);
            MaxStamina = CalculateStamina(endurance);
        }

        public int CalculateStamina(int value)
        {
            float stamina = 0;
            stamina = value * 10;

            return Mathf.RoundToInt(stamina);
        }

        // Regenerates stamina when not sprinting
        public virtual void RegenerateStamina()
        {
            if (_characterManager.isSprinting) return;

            if (_characterManager.isPerformingAction) return;

            staminaRegenerationTimer += Time.deltaTime;

            if (staminaRegenerationTimer >= staminaRegenerationDelay)
            {
                if (CurrentStamina < MaxStamina)
                {
                    staminaTickTimer = staminaTickTimer + Time.deltaTime;

                    if (staminaTickTimer >= 0.1)
                    {
                        staminaTickTimer = 0;
                        CurrentStamina += staminaRegenerationAmount;
                    }
                }
            }
        }

        public virtual void ResetStaminaRegenTimer(float oldValue, float newValue)
        {
            // Only change if the action reduced the stamina
            if (newValue < oldValue)
            {
                staminaRegenerationTimer = 0;
            }
        }
    }
}
