using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UI_StatBar : MonoBehaviour
    {
        private Slider _slider;


        private void Awake()
        {
            _slider = GetComponent<Slider>();
        }

        public virtual void SetStat(float newValue)
        {
            _slider.value = newValue;
        }

        public virtual void SetMaxStat(int maxValue)
        {
            _slider.maxValue = maxValue;
            _slider.value = maxValue;
        }
    }
}