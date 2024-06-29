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
            if (Input.GetKey(KeyCode.W)) panInput.y = 1;
            else if (Input.GetKey(KeyCode.S)) panInput.y = -1;
            else panInput.y = 0;

            if (Input.GetKey(KeyCode.D)) panInput.x = 1;
            else if (Input.GetKey(KeyCode.A)) panInput.x = -1;
            else panInput.x = 0;


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
            
            _snapPoint.position = _target.position;
            _manager._rotationPoint.transform.position = _target.position;    
        }
        #endregion

        #region COMMON
        private void Orbit()
        {
            float angle = 0;

            if (Input.GetKeyDown(KeyCode.Q))
            {
                targetAngle += 45.0f;
                _manager._rotating = true;
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                targetAngle -= 45.0f;
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
            scrollInput = Input.GetAxis("Mouse ScrollWheel");

            if (scrollInput == 0.0f) return;

            float newZoom = _manager.GetViewportZoom();
            newZoom -= scrollInput * zoomStrength; 
            _manager.ZoomViewport(Mathf.Clamp(newZoom, minZoom, maxZoom));
        }
        #endregion COMMON

        #region Logic

        private bool Approximately(float a, float b, float range)
        {
            return Mathf.Abs(a - b) <= range;
        }
        #endregion
    }
}