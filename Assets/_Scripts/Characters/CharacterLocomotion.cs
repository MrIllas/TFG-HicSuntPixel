using HicSuntPixel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Character
{
    public class CharacterLocomotion : MonoBehaviour
    {
        HSPCameraManager _cameraManager;

        protected virtual void Awake()
        {

        }

        private void Start()
        {
            if (!_cameraManager)
            {
                _cameraManager = FindObjectOfType<HSPCameraManager>();
            }
        }

        protected void LateUpdate()
        {
            
        }

        #region Snap Point
        private Vector3 SnapPosition(Vector3 wp)
        {
            float pixelSize = 2.0f * _cameraManager._renderCamera.orthographicSize / _cameraManager.realResolution.y;
            Transform RCTransform = _cameraManager._renderCamera.transform;

            Vector3 aux = RCTransform.InverseTransformDirection(wp);
            aux = Vector3Int.RoundToInt(aux / pixelSize);

            Vector3 snappedVec = aux * pixelSize;

            Vector3 toReturn;
            toReturn = snappedVec.x * RCTransform.right;
            toReturn += snappedVec.y * RCTransform.up;
            toReturn += snappedVec.z * RCTransform.forward;

            return toReturn;
        }
        #endregion

    }
}