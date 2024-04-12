using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HicSuntPixel;

namespace Character.Player
{
    public class PlayerLocomotion : CharacterLocomotion
    {
        PlayerManager _player;

        // TAKEN FROM PlayerInputManager
        public float verticalMovement;
        public float horizontalMovement;
        public float moveAmount;

        private Vector3 moveDirection;
        private Vector3 targetRotationDirection = Vector3.zero;
        [SerializeField] float walkingSpeed = 1.0f;
        [SerializeField] float runningSpeed = 2.0f;
        [SerializeField] float rotationSpeed = 50.0f;

        protected override void Awake()
        {
            base.Awake();

            _player = GetComponent<PlayerManager>();
        }

        public void HandleAllMovement()
        {
            HandleGroundedMovement();
            HandleRotation();
            //Grounded
            //Aerial
            //Roatation
            //Falling
        }

        private void GetVerticalAndHorizontaInputs()
        {
            verticalMovement = PlayerInputManager.instance.verticalInput;
            horizontalMovement = PlayerInputManager.instance.horizontalInput;

            // CLAMP THE MOVEMENTS FOR ANIMATIONS
        }

        private void HandleGroundedMovement()
        {
            GetVerticalAndHorizontaInputs();

            // MOVEMENT BASED ON CAMERA 
            moveDirection = CameraManager.instance.transform.forward * verticalMovement;
            moveDirection += CameraManager.instance.transform.right * horizontalMovement;
            moveDirection.y = 0.0f; // Don't need to move the camera up & down
            moveDirection.Normalize();

            //RUNNING
            if (PlayerInputManager.instance.moveAmount > 0.5f)
            {
                _player._characterController.Move(moveDirection * Time.deltaTime * runningSpeed);
            }
            else if (PlayerInputManager.instance.moveAmount <= 0.5f)
            { //WALKING
                _player._characterController.Move(moveDirection * Time.deltaTime * walkingSpeed);
            }
        }

        private void HandleRotation()
        {
            targetRotationDirection = CameraManager.instance.cameraObject.transform.forward * verticalMovement;
            targetRotationDirection += CameraManager.instance.cameraObject.transform.right * horizontalMovement;
            targetRotationDirection.y = 0.0f;
            targetRotationDirection.Normalize();

            if (targetRotationDirection == Vector3.zero) 
            {
                targetRotationDirection = transform.forward;
            }
        
            Quaternion newRotation = Quaternion.LookRotation(targetRotationDirection);
            Quaternion targetRotation = Quaternion.Slerp(transform.rotation, newRotation, rotationSpeed * Time.deltaTime);
            transform.rotation = targetRotation;
        }
    }

}