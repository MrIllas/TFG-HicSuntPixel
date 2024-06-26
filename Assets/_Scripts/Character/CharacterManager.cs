using Character.Player;
using HicSuntPixel;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;

namespace Character
{
    public class CharacterManager : MonoBehaviour
    {
        [HideInInspector] public CharacterController _characterController;
        [HideInInspector] public CharacterLocomotion _characterLocomotion;
        [HideInInspector] public CharacterAnimatorManager _animatorManager;
        [HideInInspector] public CharacterCombatManager _characterCombatManager;
        [HideInInspector] public CharacterEffectsManager _characterEffectsManager;
        [HideInInspector] public CharacterStatsManager _statsManager;

        protected HSPCameraManager _cameraManager;
        public GameObject _snapPoint;

        [HideInInspector] public Animator _animator;

        [Header("Status")]
        public string characterName = "";
        public Faction faction = Faction.Faction01;
        public CreatureType creatureType = CreatureType.Humanoid;

        [Header("Flags")]
        public bool isPerformingAction = false;
        public bool isJumping = false;
        public bool isDashing = false;
        public bool isGrounded = true;
        public bool isSprinting = false;
        public bool applyRootMotion = false;
        public bool canRotate = true;
        public bool canMove = true;

        

        private void OnDestroy()
        {
            Destroy(_snapPoint);
        }

        protected virtual void Awake()
        {
            _animator = GetComponent<Animator>();
            if (_animator == null) _animator = GetComponentInChildren<Animator>();
            _characterLocomotion = GetComponent<CharacterLocomotion>();
            _statsManager = GetComponent<CharacterStatsManager>();
            _characterEffectsManager = GetComponent<CharacterEffectsManager>();
            _animatorManager = GetComponent<CharacterAnimatorManager>();
            _characterCombatManager = GetComponent<CharacterCombatManager>();

            CreateSnapPoint();
            _characterController = _snapPoint.GetComponent<CharacterController>();
        }

        protected virtual void Start()
        {
            IgnoreMyOwnColliders();

            _cameraManager = HSPCameraManager.instance;
        }

        protected virtual void Update()
        {
            _animator.SetBool("isGrounded", isGrounded);
        }

        protected virtual void FixedUpdate()
        {

        }

        protected void LateUpdate()
        {
            UpdateTransform();
            
        }

        protected virtual void OnEnable()
        {

        }

        protected virtual void OnDisable()
        {

        }

        public virtual void OnSpawn()
        {
            //_characterStatsManager.OnCurrentHealthChanged += _characterHUDManager.SetNewHealthValue;
            //_characterStatsManager.OnMaxHealthChanged += PlayerUIManager.instance._playerUIHudManager.SetMaxHealthValue;
            //_characterStatsManager.OnCurrentStaminaChanged += PlayerUIManager.instance._playerUIHudManager.SetNewStaminaValue;
            //_characterStatsManager.OnCurrentStaminaChanged += _characterStatsManager.ResetStaminaRegenTimer;
            //_characterStatsManager.OnMaxStaminaChanged += PlayerUIManager.instance._playerUIHudManager.SetMaxStaminaValue;
        }

        public virtual void OnDespawn()
        {

        }

        public virtual IEnumerator DeathEvent(bool manuallySelectDeathAnimation = false)
        {
            _statsManager.CurrentHealth = 0;
            _statsManager.IsDead = true;

            // Reset Any flags that require it

            if (!manuallySelectDeathAnimation)
            {
                _animatorManager.PlayTargetActionAnimation("Player_Death", true);
            }

            yield return new WaitForSeconds(5);
        }

        private void IgnoreMyOwnColliders()
        {
            Collider characterControllerCollider = GetComponent<Collider>();
            Collider[] damageableCharacterColliders = GetComponentsInChildren<Collider>();
            
            List<Collider> ignoreColliders = new List<Collider>();

            foreach (var collider in damageableCharacterColliders) 
            {
                ignoreColliders.Add(collider);
            }

            ignoreColliders.Add(characterControllerCollider);

            foreach(var collider in ignoreColliders)
            {
                foreach (var otherCollider in ignoreColliders)
                {
                    Physics.IgnoreCollision(collider, otherCollider);
                }
            }
        }

        public virtual void ReviveCharacter()
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

            _snapPoint.layer = gameObject.layer;

            // GIVE COMPONENTS
            // Character/Player Locomotion
            var oCL = gameObject.GetComponent<CharacterLocomotion>();
            
            if (oCL)
            {
                var pCL = _snapPoint.AddComponent(oCL.GetType());
                CopyValues(oCL, pCL);
                Destroy(oCL);
                _snapPoint.GetComponent<CharacterLocomotion>()._character = this;
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

