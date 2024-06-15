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

        #endregion
    }
}

