using HicSuntPixel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Character
{
    public class CharacterManager : MonoBehaviour
    {
        public CharacterController _characterController;

        HSPCameraManager _cameraManager;
        public GameObject _snapPoint;

        [HideInInspector] public Animator animator;

        protected virtual void Awake()
        {
            //_characterController = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();

            CreateSnapPoint();
            _characterController = _snapPoint.GetComponent<CharacterController>();
        }

        protected virtual void Start()
        {
            OnSpawn();

            if (!_cameraManager)
            {
                _cameraManager = FindObjectOfType<HSPCameraManager>();
            }
        }

        protected virtual void Update()
        {

        }

        protected void LateUpdate()
        {
            UpdateTransform();
            
        }

        protected virtual void OnSpawn()
        {

        }

        #region SNAP POINT Generation

        private void UpdateTransform()
        {
            transform.position = SnapPosition(_snapPoint.transform.position);
            transform.eulerAngles = _snapPoint.transform.eulerAngles;
        }

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

        protected void CreateSnapPoint()
        {
            _snapPoint = new GameObject(transform.name + " (Snap Point)");

            _snapPoint.transform.parent = transform.parent;

            _snapPoint.transform.localPosition = transform.localPosition;
            _snapPoint.transform.localRotation = transform.localRotation;
            _snapPoint.transform.localScale = transform.localScale;

            // GIVE COMPONENTS
            // Character/Player Locomotion
            var oCL = gameObject.GetComponent<CharacterLocomotion>();
            if (oCL)
            {
                var pCL = _snapPoint.AddComponent(oCL.GetType());
                CopyValues(oCL, pCL);
                Destroy(oCL);
            }

            // Character Controller
            var oCC = gameObject.GetComponent<CharacterController>();
            if(oCC)
            {
                CharacterController pCC = _snapPoint.AddComponent<CharacterController>();
                CopyCharacterController(oCC, pCC);
                Destroy(oCC);
            }
        }

        private void CopyValues(Component from, Component to)
        {
            var json = JsonUtility.ToJson(from);
            JsonUtility.FromJsonOverwrite(json, to);
        }

        private void CopyCharacterController(CharacterController from, CharacterController to)
        {
            to.center = from.center;
            to.radius = from.radius;
            to.height = from.height;
            to.slopeLimit = from.slopeLimit;
            to.stepOffset = from.stepOffset;
            to.skinWidth = from.skinWidth;
            to.minMoveDistance = from.minMoveDistance;
            to.detectCollisions = from.detectCollisions;
            to.enableOverlapRecovery = from.enableOverlapRecovery;
            // Add more properties as needed
        }
        #endregion
    }
}

