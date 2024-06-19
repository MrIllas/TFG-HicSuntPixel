using HicSuntPixel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class CharacterHUDManager : MonoBehaviour
    {
        private Camera _cam;
        [SerializeField] UI_StatBar healthBar;

        private void Start()
        {
            _cam = HSPCameraManager.instance._renderCamera;
        }

        private void Update()
        {
            Vector3 localCamPosition = _cam.transform.localPosition + Vector3.forward * -200.0f; 
            transform.rotation = Quaternion.LookRotation(transform.position - _cam.transform.TransformPoint(localCamPosition));
        }

        public void SetNewHealthValue(int oldValue, int newValue)
        {
            healthBar.SetStat(oldValue, newValue);
        }

        public void SetMaxHealthValue(int oldValue, int newValue)
        {
            healthBar.SetMaxStat(newValue);
        }

        public void SetNewStaminaValue(float oldValue, float newValue)
        {
            healthBar.SetStat((int)oldValue, (int) newValue);
        }

        public void SetMaxStaminaValue(int oldValue, int newValue)
        {
            healthBar.SetMaxStat(newValue);
        }
    }
}