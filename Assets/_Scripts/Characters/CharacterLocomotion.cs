using HicSuntPixel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Character
{
    public class CharacterLocomotion : MonoBehaviour
    {
        HSPCameraManager _cameraManager;
        public CharacterManager _character; // Directly given by the Character Manager when creating snap point

        [Header("Ground Check & Jumping")]
        [SerializeField] protected LayerMask groundLayer;
        [SerializeField] protected float groundCheckSphereRadius = 1;
        [SerializeField] protected float gravityForce = -9.807f;
        [SerializeField] protected Vector3 yVelocity; // THIS IS THE "GRAVITY" FORCE as well
        [SerializeField] protected float groundYVelocity = -20; // The force at which the chracter sticks to the ground
        [SerializeField] protected float fallStartYVelocity = -5; // The force at which it begins to fall from a height

        protected bool fallingVelocityHasBeenSet = false;
        protected float inAirTimer = 0;

        protected virtual void Awake()
        {
        }

        protected virtual void Start()
        {
            if (!_cameraManager)
            {
                _cameraManager = FindObjectOfType<HSPCameraManager>();
            }
        }

        protected virtual void Update()
        {
            HandleJumpingAndFalling();
        }

        protected virtual void LateUpdate()
        {
            
        }

        private void HandleJumpingAndFalling()
        {
            if (_character.isGrounded)
            {
                // If not jumping
                if (yVelocity.y < 0)
                {
                    inAirTimer = 0;
                    fallingVelocityHasBeenSet = false;
                    yVelocity.y += groundYVelocity;
                }
            }
            else
            {
                // if not jumping, and falling velocity has not been set
                if (!_character.isJumping && !fallingVelocityHasBeenSet)
                {
                    fallingVelocityHasBeenSet = true;
                    yVelocity.y = fallStartYVelocity;
                }

                inAirTimer += Time.deltaTime;
                _character._animator.SetFloat("InAirTimer", inAirTimer);

                yVelocity.y += gravityForce * Time.deltaTime;
            }
            // There always needs to be some force
            _character._characterController.Move(yVelocity * Time.deltaTime);
            HandleGroundCheck();
        }

        protected void HandleGroundCheck()
        {
            _character.isGrounded = Physics.CheckSphere(_character.transform.position, groundCheckSphereRadius, groundLayer);
            
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