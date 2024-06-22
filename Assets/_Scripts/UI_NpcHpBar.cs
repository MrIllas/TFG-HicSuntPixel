using Character.Npc;
using HicSuntPixel;
using TMPro;
using UnityEngine;

namespace UI
{
    public class UI_NpcHpBar : UI_StatBar
    {
        private NpcManager _npcManager;
        private Camera _cam;

        [SerializeField] bool displayName = false;
        [SerializeField] float hpBarHidingTime = 3.0f;
        [SerializeField] float currentTimer = 0;
        [SerializeField] int currentDamageTaken = 0;

        [SerializeField] TextMeshProUGUI npcName;
        [SerializeField] TextMeshProUGUI npcDamage;
        [SerializeField] TextMeshProUGUI npcHp;


        protected override void Awake()
        {
            base.Awake();

            _npcManager = GetComponentInParent<NpcManager>();
        }

        protected override void Start()
        {
            base.Start();

            _cam = HSPCameraManager.instance._renderCamera;
            gameObject.SetActive(false);
        }

        private void Update()
        {
            Vector3 localCamPosition = _cam.transform.localPosition + Vector3.forward * 200.0f;
            transform.LookAt(transform.position + _cam.transform.TransformPoint(localCamPosition));

            if (currentTimer > 0)
            {
                currentTimer -= Time.deltaTime;
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        public override void SetStat(int oldValue, int newValue)
        {
            float oldHp = _slider.value;

            if (displayName)
            {
                npcName.enabled = true;
                npcName.text = _npcManager.characterName;
            }

            _slider.maxValue = _npcManager._statsManager.MaxHealth;

            currentDamageTaken = Mathf.RoundToInt(oldHp - newValue);

            if (currentDamageTaken < 0)
            {
                currentDamageTaken = Mathf.Abs(currentDamageTaken);
                npcDamage.text = "+ " + currentDamageTaken.ToString();
            }
            else
            {
                npcDamage.text = "- " + currentDamageTaken.ToString();
            }

            _slider.value = newValue;
            npcHp.text = _slider.value.ToString("F0") + " / " + _slider.maxValue.ToString("F0");

            if (newValue != _npcManager._statsManager.MaxHealth)
            {
                currentTimer = hpBarHidingTime;
                gameObject.SetActive(true);
            }
        }

        public override void SetMaxStat(int maxValue)
        {
            base.SetMaxStat(maxValue);
        }
    }
}