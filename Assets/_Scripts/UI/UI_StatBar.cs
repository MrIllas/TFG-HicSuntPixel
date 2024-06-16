using Character.Player;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UI_StatBar : MonoBehaviour
    {
        private Slider _slider;
        private RectTransform _rectTransform;
        [SerializeField] private TextMeshProUGUI label;

        [Header("Options")]
        [SerializeField] protected bool scaleBarLengthWithStats = true;
        [SerializeField] protected float widthScaleMultiplier = 1.0f;

        private void Awake()
        {
            _slider = GetComponent<Slider>();
            _rectTransform = GetComponent<RectTransform>();
        }

        public virtual void SetStat(float newValue)
        {
            _slider.value = newValue;
            SetLabel();
        }

        public virtual void SetMaxStat(int maxValue)
        {
            _slider.maxValue = maxValue;
            _slider.value = maxValue;
            SetLabel();

            if (scaleBarLengthWithStats)
            {
                // Scale the transform of this object
                _rectTransform.sizeDelta = new Vector2(maxValue * widthScaleMultiplier, _rectTransform.sizeDelta.y);
                PlayerUIManager.instance._playerUIHudManager.RefreshHUD();
            }
        }

        private void SetLabel()
        {
            label.text = _slider.value.ToString("F0") + " / " + _slider.maxValue.ToString("F0");
        }
    }
}