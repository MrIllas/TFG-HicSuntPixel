using Globals;
using Menus;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Character.Player
{
    public class PlayerInputManager : MonoBehaviour
    {
        public static PlayerInputManager instance;

        public PlayerManager _player;

        PlayerControls _controls;

        [Header("Player Movement Input")]
        [SerializeField] Vector2 movementInput;
        [SerializeField] public float verticalInput;
        [SerializeField] public float horizontalInput;
        [SerializeField] public float moveAmount;

        [Header("Player Action Input")]
        [SerializeField] bool dashInput = false;
        [SerializeField] bool sprintInput = false;
        [SerializeField] bool jumpInput = false;
        [SerializeField] bool action01Input = false;
        [SerializeField] bool interactInput = false;

        [Header("Lock Target Input")]
        [SerializeField] bool targetLockInput = false;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);

            //When the scene changes, run the following logic
            SceneManager.activeSceneChanged += OnSceneChange;
        }

        private void OnDestroy()
        {
            // If we destroy this object, unsubscribe the event
            SceneManager.activeSceneChanged -= OnSceneChange;
        }

        private void OnSceneChange(Scene oldScene, Scene newScene)
        {
            // Enable player controls if we are loading into the world, otherwise disble them
            if (newScene.buildIndex == WorldSaveGameManager.instance.GetWorldSceneIndex())
            {
                instance.enabled = true;
            }
            else
            {
                instance.enabled = false;
            }
        }

        private void OnEnable()
        {
            if (_controls == null)
            {
                _controls = new PlayerControls();

                _controls.PlayerMovement.Move.performed += i => movementInput = i.ReadValue<Vector2>();

                _controls.PlayerActions.Dash.performed += i => dashInput = true; // On button tap or V
                _controls.PlayerActions.Jump.performed += i => jumpInput = true; // On Space
                _controls.PlayerActions.Sprint.performed += i => sprintInput = true; // On button Hold
                _controls.PlayerActions.Sprint.canceled += i => sprintInput = false; // On button hold release

                _controls.PlayerActions.Interact.performed += i => interactInput = true; 

                _controls.PlayerActions.Lock.performed += i => targetLockInput = true;
                _controls.PlayerActions.Action01.performed += i => action01Input = true;
            }
            _controls.Enable();
        }

        private void Update()
        {
            HandleAllInputs();
        }

        private void HandleAllInputs()
        {
            HandleMovementInput();
            HandleSprintingInput();
            HandleDashInput();
            HandleJumpInput();

            HandleInteractInput();
            HandleTargetLockInput();
            HandleAction01Input();
        }

        #region Movement
        private void HandleMovementInput()
        {
            verticalInput = movementInput.y;
            horizontalInput = movementInput.x;

            // GET ABSOLUTE NUMBER
            moveAmount = Mathf.Clamp01(Mathf.Abs(verticalInput) + Mathf.Abs(horizontalInput));

            // Clamp for walk and running
            if (moveAmount <= 0.5 && moveAmount > 0)
            {
                moveAmount = 0.5f;
            }
            else if (moveAmount > 0.5 && moveAmount <= 1)
            {
                moveAmount = 1.0f;
            }

            if (_player == null) return;

            // WHY 0 ON HORIZONTAL? CUZ WE ONLY WANT NON-STRAFING MOVEMENT WHEN NOT LOCKED
            _player._playerAnimatorManager.UpdateAnimatorMovementParameters(0, moveAmount, _player.isSprinting);

            // IF WE ARE LOCKED, PASS HORIZONTAL MOVEMENT AS WELL
        }
        #endregion

        #region Action

        public void HandleInteractInput()
        {
            if (interactInput)
            {
                interactInput = false;

                _player._playerInteractionManager.Interact();
            }
        }

        public void HandleSprintingInput()
        {
            if (sprintInput) 
            {
                _player._playerLocomotion.HandleSprinting();
            }
            else
            {
                _player.isSprinting = false;
            }
        }

        public void HandleDashInput()
        {
            if (dashInput) 
            {
                dashInput = false;

                _player._playerLocomotion.AttemptToPerformDash();
            }
        }

        public void HandleJumpInput()
        {
            if (jumpInput)
            {
                jumpInput = false;

                _player._playerLocomotion.AttemptToJump();
            }
        }

        private void HandleTargetLockInput()
        {
            if (_player._playerCombatManager.isTargetLocked)
            {
                if (_player._playerCombatManager.currentTarget != null) return;

                if (_player._playerCombatManager.currentTarget._statsManager.IsDead)
                {
                    _player._playerCombatManager.isTargetLocked = false;
                }
            }
            if (targetLockInput && _player._playerCombatManager.isTargetLocked)
            {
                targetLockInput = false;
                //Disable
                return;
            }

            if (targetLockInput && !_player._playerCombatManager.isTargetLocked)
            {
                targetLockInput = false;
                //Enable

                return;
            }
        }

        public void HandleAction01Input()
        {
            if (action01Input)
            {
                action01Input = false;

                //TODO:  If ui weapon is open, do nothing

                // On Set true, the action is performed (Handled by a setter)
                _player._playerCombatManager.IsUsingRightHand = true;
            }
        }

        #endregion
    }
}

