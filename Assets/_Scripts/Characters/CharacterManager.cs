using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Character
{
    public class CharacterManager : MonoBehaviour
    {
        public CharacterController _characterController;

        public GameObject _snapPoint;

        [HideInInspector] public Animator animator;

        protected virtual void Awake()
        {
            DontDestroyOnLoad(this);

            //_characterController = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();

            CreateSnapPoint();
            _characterController = _snapPoint.GetComponent<CharacterController>();
        }

        protected virtual void Start()
        {
            OnSpawn();
        }

        protected virtual void Update()
        {

        }

        protected virtual void OnSpawn()
        {

        }

        #region SNAP POINT Generation

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

