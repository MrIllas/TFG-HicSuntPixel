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

            //_player = GetComponent<PlayerManager>();
            //Debug.Log(GetComponent<Referencer>().name);
           // _player = GetComponent<Referencer>().GetReferenceComponent<PlayerManager>();
        }

        protected void Start()
        {
            Debug.Log(GetComponent<Referencer>().name);
            _player = GetComponent<Referencer>().GetReferenceComponent<PlayerManager>();
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

            // MOVEMENT BASED ON CAMERA 
            moveDirection = CameraController.instance.cameraObject.transform.forward * verticalMovement;
            moveDirection += CameraController.instance.cameraObject.transform.right * horizontalMovement;
            moveDirection.y = 0.0f; // Don't need to move the camera up & down
            moveDirection.Normalize();

            //RUNNING
            if (moveAmount > 0.5f)
            {
                _player._characterController.Move(moveDirection * Time.deltaTime * runningSpeed);
            }
            else if (moveAmount <= 0.5f)
            { //WALKING
                _player._characterController.Move(moveDirection * Time.deltaTime * walkingSpeed);
            }
        }

        private void HandleRotation()
        {
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
    }
}