using SaveSystem;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace Character
{
    public class CharacterStatsManager : MonoBehaviour
    {
        CharacterManager _characterManager;

        #region Status Flags
        [Header("Status Flags")]
        
        [SerializeField] private bool isDead = false;
        public bool IsDead
        {
            get => isDead;
            set
            {
                if (isDead != value)
                {
                    OnDeadStateChange?.Invoke(isDead, value);
                    isDead = value;
                }
            }
        }
        public event Action<bool, bool> OnDeadStateChange;

        [SerializeField] private bool isInvunerable = false;
        public bool IsInvunerable
        {
            get => isInvunerable;
            set
            {
                if (isInvunerable != value)
                {
                    OnInvunerableStateChange?.Invoke(isInvunerable, value);
                    isInvunerable = value;
                }
            }
        }
        public event Action<bool, bool> OnInvunerableStateChange;

        #endregion

        #region Attributes
        [Header("Attributes")]
        [SerializeField] private int vitality;
        public int Vitality
        {
            get => vitality;
            set
            {
                if (vitality != value)
                {
                    OnVitalityChanged?.Invoke(vitality, value);
                    vitality = value;

                    MaxHealth = CalculateHealth(value);
                    CurrentHealth = MaxHealth;
                }
            }
        }
        public event Action<int, int> OnVitalityChanged;

        [SerializeField] private int endurance;
        public int Endurance
        {
            get => endurance;
            set
            {
                if (endurance != value)
                {
                    OnEnduranceChanged?.Invoke(endurance, value);
                    endurance = value;

                    MaxStamina = CalculateStamina(value);
                    CurrentStamina = MaxStamina;
                }
            }
        }
        public event Action<int, int> OnEnduranceChanged;
        #endregion

        #region Resources
        [Header("Resources")]

            #region Vitality(health)
        [SerializeField] private int currentHealth;
        public int CurrentHealth
        {
            get => currentHealth;
            set
            {
                if (currentHealth != value)
                {
                    
                    OnCurrentHealthChanged?.Invoke(currentHealth, value);
                    currentHealth = value;

                    if (currentHealth <= 0)
                    {
                        StartCoroutine(_characterManager.DeathEvent());
                    }

                    // Prevent overhealing
                    if (currentHealth > maxHealth) 
                    {
                        currentHealth = maxHealth;
                    }
                }
            }
        }
        public event Action<int, int> OnCurrentHealthChanged;

        [SerializeField] private int maxHealth;
        public int MaxHealth
        {
            get => maxHealth;
            set
            {
                if (maxHealth != value)
                {
                    OnMaxHealthChanged?.Invoke(maxHealth, value);
                    maxHealth = value;
                }
            }
        }
        public event Action<int, int> OnMaxHealthChanged;
        #endregion

            #region Endurance(stamina)
        [SerializeField] private float currentStamina;
        public float CurrentStamina
        {
            get => currentStamina;
            set
            {
                if (currentStamina != value)
                {
                    OnCurrentStaminaChanged?.Invoke(currentStamina, value);
                    currentStamina = value;
                }
            }
        }
        public event Action<float, float> OnCurrentStaminaChanged;

        [SerializeField] private int maxStamina;
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

            #endregion

        #endregion

        [Header("Stamina Regeneration")]
        [SerializeField] int staminaRegenerationAmount = 2;
        private float staminaRegenerationTimer = 0;
        [SerializeField] float staminaRegenerationDelay = 2;
        private float staminaTickTimer = 0;

        protected virtual void Awake()
        {
            _characterManager = GetComponent<CharacterManager>();
        }

        protected virtual void Start() 
        {
#if UNITY_EDITOR
            EventTriggerFromEditorStatsUpdateStart();
#endif
        }

        private void Update()
        {
#if UNITY_EDITOR
            EventTriggerFromEditorStatsUpdate();
#endif
        }

        // Functions

        public void OnSpawn()
        {
            MaxHealth = CalculateHealth(vitality);
            CurrentHealth = CalculateHealth(vitality); // Current health needs to come always after max health, otherwise it will become 0

            MaxStamina = CalculateStamina(endurance);
            CurrentStamina = CalculateStamina(endurance);
        }

        // TEMPORAL SOLUTION TO CREATING A NEW GAME (SETTING INITIAL STATS)
        public void OnNewGame(ref CharacterSaveData data)
        {
            vitality = 10;
            endurance = 10;
            OnSpawn();
            OnSave(ref data);
        }

        public void OnSave(ref CharacterSaveData data)
        {
            data.currentHealth = CurrentHealth;
            data.vitality = Vitality;

            data.currentStamina = CurrentStamina;
            data.endurance = Endurance;
        }

        public void OnLoad(ref CharacterSaveData data)
        {
            Vitality = data.vitality; // Attribute needs to come before their resources
            CurrentHealth = data.currentHealth;
            MaxHealth = CalculateHealth(data.vitality);

            Endurance = data.endurance;// Attribute needs to come before their resources
            CurrentStamina = data.currentStamina;
            MaxStamina = CalculateStamina(data.endurance);
        }

        public int CalculateHealth(int value)
        {
            float health = 0;
            health = value * 15;

            return Mathf.RoundToInt(health);
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

#if UNITY_EDITOR

        private int lastVitality, lastEndurance;

        private void EventTriggerFromEditorStatsUpdateStart()
        {
            lastVitality = vitality;
            lastEndurance = endurance;
        }

        private void EventTriggerFromEditorStatsUpdate()
        {
            if (lastVitality !=  Vitality)
            {
                lastVitality = Vitality;
                Vitality = 0;
                Vitality = lastVitality;
            }

            if (lastEndurance != Endurance) 
            {
                lastEndurance = Endurance;
                Endurance = 0;
                Endurance = lastEndurance;
            }
        }
#endif
    }
}
