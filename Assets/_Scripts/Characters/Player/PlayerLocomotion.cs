using UnityEngine;
using HicSuntPixel;

namespace Character.Player
{
    public class PlayerLocomotion : CharacterLocomotion
    {
        [SerializeField] PlayerManager _player;

        // TAKEN FROM PlayerInputManager
        [HideInInspector] public float verticalMovement;
        [HideInInspector] public float horizontalMovement;
        [HideInInspector] public float moveAmount;

        [Header("Movement Settings")]
        private Vector3 moveDirection;
        private Vector3 targetRotationDirection = Vector3.zero;
        [SerializeField] float walkingSpeed = 2.0f;
        [SerializeField] float runningSpeed = 5.0f;
        [SerializeField] float sprintingSpeed = 8.0f;
        [SerializeField] float rotationSpeed = 50.0f;
        [SerializeField] int sprintingStaminaCost = 20;

        [Header("Dash & Jump")]
        private Vector3 jumpDirection;
        private Vector3 dashDirection;
        [SerializeField] float jumpHeight = 1.0f;
        [SerializeField] float dashDuration = 0.5f;
        [SerializeField] float dashSpeed = 2.0f;
        //[SerializeField] float jumpForwardSpeed = 5.0f;
        //[SerializeField] float freeFallSpeed = 2.0f;
        [SerializeField] int dashStaminaCost = 25;
        [SerializeField] int jumpStaminaCost = 25;

        private float dashTimer = 0.0f;

        protected override void Awake()
        {
            base.Awake();

            //_player = GetComponent<PlayerManager>();
            //Debug.Log(GetComponent<Referencer>().name);
           // _player = GetComponent<Referencer>().GetReferenceComponent<PlayerManager>();
        }

        protected override void Start()
        {
            base.Awake();
            //Debug.Log(GetComponent<Referencer>().name);
            //_player = GetComponent<Referencer>().GetReferenceComponent<PlayerManager>();
        }

        protected override void Update()
        {
            base.Update();

        }

        protected override void LateUpdate()
        {
            base.LateUpdate();
        }

        public void HandleAllMovement()
        {
            HandleGroundedMovement();
            HandleRotation();
            HandleJumpingMovement();
            HandleFreeFallMovement();
            HandleDashMovement();
            //Grounded
            //Aerial
            //Roatation
            //Falling
        }

        private void GetMovementValues()
        {
            verticalMovement = PlayerInputManager.instance.verticalInput;
            horizontalMovement = PlayerInputManager.instance.horizontalInput;
            moveAmount = PlayerInputManager.instance.moveAmount;

            // CLAMP THE MOVEMENTS FOR ANIMATIONS
        }

        private void HandleGroundedMovement()
        {
            GetMovementValues();

            if (!_player.canMove) return;

            // MOVEMENT BASED ON CAMERA 
            moveDirection = CameraController.instance.cameraObject.transform.forward * verticalMovement;
            moveDirection += CameraController.instance.cameraObject.transform.right * horizontalMovement;
            moveDirection.y = 0.0f; // Don't need to move the camera up & down
            moveDirection.Normalize();

            if (_player.isSprinting) 
            {
                _player._characterController.Move(moveDirection * sprintingSpeed * Time.deltaTime);
            }
            else
            {
                //RUNNING
                if (moveAmount > 0.5f)
                {
                    _player._characterController.Move(moveDirection * runningSpeed * Time.deltaTime);
                }
                else if (moveAmount <= 0.5f)
                { //WALKING
                    _player._characterController.Move(moveDirection * walkingSpeed * Time.deltaTime);
                }
            }
        }

        // The movement mid-air while jumping
        private void HandleJumpingMovement()
        {
            if (_player.isJumping)
            {
                _player._characterController.Move(jumpDirection * runningSpeed * Time.deltaTime);
            }
        }

        //The movement mid-air while falling
        private void HandleFreeFallMovement()
        {
            if (!_player.isGrounded)
            {
                Vector3 freeFallDirection;

                freeFallDirection = CameraController.instance.cameraObject.transform.forward * PlayerInputManager.instance.verticalInput;
                freeFallDirection += CameraController.instance.cameraObject.transform.right * PlayerInputManager.instance.horizontalInput;

                _player._characterController.Move(freeFallDirection * walkingSpeed * Time.deltaTime);
            }
        }

        private void HandleRotation()
        {
            if (!_player.canRotate) return;
            targetRotationDirection = CameraController.instance.cameraObject.transform.forward * verticalMovement;
            targetRotationDirection += CameraController.instance.cameraObject.transform.right * horizontalMovement;
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

        public void HandleSprinting()
        {
            if (_player.isPerformingAction)
            {
                _player.isSprinting = false;
            }

            if (_player._playerStatsManager.CurrentStamina <= 0)
            {
                _player.isSprinting = false;
                return;
            }

            // If we are moving, sprinting false
            if (moveAmount >= 0.5f)
            {
                _player.isSprinting = true;
            }
            else
            {// If not moving set sprinting to false
                _player.isSprinting = false;
            }

            if (_player.isSprinting) 
            {
                _player._playerStatsManager.CurrentStamina -= sprintingStaminaCost * Time.deltaTime;
            }
        }

        private void HandleDashMovement()
        {
            //if(dashDuration >= dashTimer)
            //{
            //    _character._characterController.Move(dashDirection * dashSpeed * Time.deltaTime);
            //    dashTimer += Time.deltaTime;
            //}
            //else
            //{
            //    dashTimer = 0;
            //}
        }

        public void AttemptToPerformDash()
        {
            if (_player.isPerformingAction) return;
            if (_player._playerStatsManager.CurrentStamina <= 0) return;

            // When moveing
            if (moveAmount > 0)
            {
                dashDirection = CameraController.instance.cameraObject.transform.forward * verticalMovement;
                dashDirection += CameraController.instance.cameraObject.transform.right * horizontalMovement;
                dashDirection.y = 0;

                Quaternion rotation = Quaternion.LookRotation(dashDirection);
                _player.transform.rotation = rotation;

                _player._playerAnimatorManager.PlayTargetActionAnimation("Dash", true, true);
            }
            else
            {  // Perform a frontal dash if stationary
                _player._playerAnimatorManager.PlayTargetActionAnimation("Back_Step", true, true);
            }

            _player._playerStatsManager.CurrentStamina -= dashStaminaCost;
        }

        public void AttemptToJump()
        {
            if (_player.isPerformingAction)return;
            if (_player._playerStatsManager.CurrentStamina <= 0) return;
            if (_player.isJumping) return;
            if (!_player.isGrounded) return;

            _player._playerAnimatorManager.PlayTargetActionAnimation("Jump_Start", false, false);

            _player.isJumping = true;

            _player._playerStatsManager.CurrentStamina -= jumpStaminaCost;

            jumpDirection = CameraController.instance.cameraObject.transform.forward * verticalMovement;
            jumpDirection += CameraController.instance.cameraObject.transform.right * horizontalMovement;
            jumpDirection.y = 0;

            if (jumpDirection != Vector3.zero)
            {
                //Jump Direction(distance) depending if the player is running walking or less
                if (_player.isSprinting)
                {
                    jumpDirection *= 1;
                }
                else if (PlayerInputManager.instance.moveAmount > 0.5f)
                {
                    jumpDirection *= 0.5f;
                }
                else if (PlayerInputManager.instance.moveAmount <= 0.5f)
                {
                    jumpDirection *= 0.25f;
                }
            }
        }

        public void ApplyJumpingVelocity()
        {
            yVelocity.y = Mathf.Sqrt(jumpHeight * -2 * gravityForce);
        }
    }
}