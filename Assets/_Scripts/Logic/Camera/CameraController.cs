using UnityEngine;

namespace HicSuntPixel
{
    public class CameraController : MonoBehaviour
    {
        public static CameraController instance;

        public enum CameraSetting
        {
            TestingCamera,
            PanCamera,
            FollowCamera
        }

        [SerializeField] public CameraSetting _setting = CameraSetting.PanCamera;

        private HSPCameraManager _manager;
        private PlayerControls _controls;
        private Transform _snapPoint;
        [HideInInspector] public Camera cameraObject; // The main camera, the camera that is used by the Player's locomotion script as reference for direction

        // -- FLY CAMERA SETTINGS --
        [Header("FLY CAMERA SETTINGS")]
        public float panSpeed = 120.0f;

        // -- FOLLOW CAMERA SETTINGS --
        [Header("FOLLOW CAMERA SETTINGS")]
        [SerializeField] public Transform _target;
        [SerializeField] private float smoothSpeed = 1.0f;
        private Vector3 offset;

        // -- PanCamera --
        [Header("TEST MODE SETTINGS")]
        public float circularRadius = 0.0f;
        public float circularSpeed = 0.2f;
        private float circAngle = 0.0f;
        private const float TAU = Mathf.PI * 2;

        // -- General Settings --
        [Header("GENERAL SETTINGS")]
        //Zoom
        [Header("Zoom")]
        [SerializeField] private float zoomStrength = 1.0f;
        [Range(0.0f, 1.0f)] public float minZoom = 0.1f;
        [Range(0.0f, 1.0f)] public float maxZoom = 1.0f;
        
        //Orbit
        [Header("Orbit")]
        [SerializeField] private float rotationSpeed = 5.0f;
        private float targetAngle = 0.0f;
        private float currentAngle = 0.0f;

        //Inputs
        private float scrollInput;
        private float orbitInput;
        private Vector2 panInput;

        public static void ClearInstance()
        {
            if (instance != null)
            {
                Destroy(instance.gameObject);
                instance = null;
            }
        }

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

            _manager = GetComponentInChildren<HSPCameraManager>();
            cameraObject = _manager._renderCamera;
        }

        void Start()
        {
            DontDestroyOnLoad(gameObject);

            _snapPoint = _manager._snapPoint;
        }

        private void OnEnable()
        {
            SetInputs();
        }

        void Update()
        {
            switch (_setting)
            {
                default:
                case CameraSetting.TestingCamera:
                {
                    TestMode();

                    Orbit();
                    Zoom();
                }
                break;
                case CameraSetting.PanCamera:
                {
                    Pan();

                    Orbit();
                    Zoom();
                }
                break;
                case CameraSetting.FollowCamera:
                {
                    FollowTarget();

                    Orbit();
                    Zoom();
                }
                break;
            }
        }

        #region TEST MODE

        private void TestMode()
        {
            if (circularRadius <= 0) return;
            circAngle -= TAU * circularSpeed * Time.deltaTime;

            Vector3 newPosition = new Vector3 (Mathf.Cos(circAngle) * circularRadius,
                                                Mathf.Sin(circAngle) * circularRadius,
                                                _manager._snapPoint.localPosition.z);
            _manager._snapPoint.localPosition = newPosition;
        }

        #endregion Test Mode
        #region PAN MODE
        private void Pan()
        {
            Vector3 horizontal = Vector3.zero;
            Vector3 vertical = Vector3.zero;

            Vector3 horizontalPan = new Vector3(_snapPoint.forward.x, 0, _snapPoint.forward.z);
            Vector3 verticalPan = new Vector3(_snapPoint.transform.right.x, 0, _snapPoint.right.z);

            vertical += verticalPan * panInput.x;
            horizontal += horizontalPan * panInput.y;

            Vector3 pos = (horizontal + vertical) * panSpeed * Time.deltaTime;
            _snapPoint.position += pos;
            _manager._rotationPoint.transform.position += pos;
        }        

        #endregion
        #region FOLLOW MODE
        private void FollowTarget()
        {
            if (!_target)
            {
                Debug.LogError("Error: NO TARGET SET TO THE CAMERA."); 
                return;
            }

            //if (!_manager._rotating)
            //{
               // _snapPoint.position = Vector3.Lerp(_snapPoint.position, _target.position, smoothSpeed * Time.deltaTime);
               _snapPoint.position = _target.position;
                _manager._rotationPoint.transform.position = _target.position;
           // }
            
        }
        #endregion

        #region COMMON
        private void Orbit()
        {
            float angle = 0;

            if (_controls.Camera.Orbit.WasPerformedThisFrame())
            {
                targetAngle += orbitInput * 45.0f;

                _manager._rotating = true;
            }

            if (targetAngle != currentAngle)
            {
                if (Approximately(currentAngle, targetAngle, 1.0f)) // Snap on proximity to diminnish jittering
                {
                    angle = targetAngle - currentAngle;
                    targetAngle = 0;
                    currentAngle = 0;
                    _manager._rotating = false;
                }
                else
                {
                    angle = Mathf.Lerp(currentAngle, targetAngle, rotationSpeed * Time.deltaTime);
                    angle -= currentAngle;
                    currentAngle += angle;
                }
                _manager._snapPoint.RotateAround(_manager._rotationPoint.position, Vector3.up, angle);
                _manager._renderCamera.transform.rotation = _manager._snapPoint.transform.rotation;
            }
        }

        private void Zoom()
        {
            if (!_controls.Camera.Zoom.IsPressed()) return;

            float newZoom = _manager.GetViewportZoom();
            newZoom -= (scrollInput / 120) * zoomStrength; // 120 because is the value of the scroll
            _manager.ZoomViewport(Mathf.Clamp(newZoom, minZoom, maxZoom));
        }
        #endregion COMMON

        #region Logic
        private void SetInputs()
        {
            if (_controls == null)
            {
                _controls = new PlayerControls();

                _controls.Camera.Pan.performed += i => panInput = i.ReadValue<Vector2>();
                _controls.Camera.Orbit.performed += i => orbitInput = i.ReadValue<float>();
                _controls.Camera.Zoom.performed += i => scrollInput = i.ReadValue<Vector2>().y;
            }
            _controls.Enable();
        }

        private bool Approximately(float a, float b, float range)
        {
            return Mathf.Abs(a - b) <= range;
        }
        #endregion
    }
}