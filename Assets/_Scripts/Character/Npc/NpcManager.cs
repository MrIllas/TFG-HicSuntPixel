
using UI;
using UnityEngine;

namespace Character.Npc
{
    public class NpcManager : CharacterManager
    {
        [Header("UI")]
        public bool hasFloatingHpBar = true;
        [HideInInspector] public UI_NpcHpBar _npcHpBar;


        [HideInInspector] public NpcCombatManager _npcCombatManager;

        [Header("Current State")]
        [SerializeField] NpcState currentState;

        protected override void Awake()
        {
            base.Awake();

            _npcCombatManager = GetComponent<NpcCombatManager>();


            _npcHpBar = GetComponentInChildren<UI_NpcHpBar>();
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            ProcessStateMachine();
        }

        private void ProcessStateMachine()
        {
            NpcState nextState = null;

            if (currentState != null) 
            {
                nextState = currentState.Tick(this); ;
            }

            if (nextState != null) 
            {
                currentState = nextState;
            }
        }

        public override void OnSpawn()
        {
            _statsManager.OnSpawn();
        }

        public override void OnDespawn()
        {
            Destroy(gameObject);
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (hasFloatingHpBar)  _statsManager.OnCurrentHealthChanged += _npcHpBar.SetStat;

        }

        protected override void OnDisable() 
        {
            base.OnDisable();

            if (hasFloatingHpBar) _statsManager.OnCurrentHealthChanged -= _npcHpBar.SetStat;
        }
    }
}