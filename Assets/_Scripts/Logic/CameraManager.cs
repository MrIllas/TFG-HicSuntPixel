using UnityEngine;

namespace HicSuntPixel
{
    public class CameraManager : MonoBehaviour
    {
        public static CameraManager instance;

        public enum CameraSetting
        {
            FlyCamera,
            FollowCamera
        }

        [SerializeField] public CameraSetting setting = CameraSetting.FlyCamera;

        // -- FLY CAMERA SETTINGS --
        [Header("FLY CAMERA SETTINGS")]
        public float panSpeed = 120.0f;

        // -- FOLLOW CAMERA SETTINGS --
        [Header("FOLLOW CAMERA SETTINGS")]
        [SerializeField] public Transform _target;
        [SerializeField] private float smoothSpeed = 1.0f;
        private Vector3 offset;

        // -- General Settings --
        [Header("GENERAL SETTINGS")]
        //Zoom
        [Header("Zoom")]
        [SerializeField] private float zoomSensitivity = 1.0f;
        [Range(1.0f, 100.0f)] public float zoomSpeed = 25.0f;
        [Range(0.0f, 1.0f)] public float minZoom = 0.1f;
        [Range(0.0f, 1.0f)] public float maxZoom = 1.0f;
        private float scrollInput;

        //Orbit
        [Header("Orbit")]
        [SerializeField] private float mouseSensitivity = 2.0f;
        [SerializeField] private float rotationSpeed = 5.0f;
        private float targetAngle = 45.0f;
        private float currentAngle = 0.0f;
        private float orbitInput;

        [HideInInspector] public Camera cameraObject; // The main camera, the camera that is used by the Player's locomotion script
        ViewportBlitter _viewport;

        PlayerControls _controls;

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

            _viewport = GetComponentInChildren<ViewportBlitter>();
            cameraObject = transform.GetChild(0).GetComponent<Camera>(); // The index of get child should be of the camera that rendres the world.
        }

        void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            if (_controls == null)
            {
                _controls = new PlayerControls();

                _controls.Camera.Orbit.performed += i => orbitInput = i.ReadValue<float>();
                _controls.Camera.Zoom.performed += i => scrollInput = i.ReadValue<Vector2>().y;
            }
            _controls.Enable();
        }

        void LateUpdate()
        {
            switch(setting)
            {
                default:
                case CameraSetting.FlyCamera:
                {
                    Pan();
                    Orbit();
                    Zoom();
                }
                    break;
                case CameraSetting.FollowCamera:
                {
                    Orbit();
                    FollowTarget();
                    Zoom();
                }
                    break;
            }
        }

        #region FLY CAMERA
        private void Pan()
        {
            Vector2 pan = new Vector2(0.0f, 0.0f);

            if (Input.GetKey(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                pan.x += 1 * panSpeed;
            }
            else if (Input.GetKey(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                pan.x -= 1 * panSpeed;
            }

            if (Input.GetKey(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                pan.y -= panSpeed;
            }
            else if (Input.GetKey(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                pan.y += panSpeed;
            }

            _viewport.PanViewport(pan.x * Time.deltaTime, pan.y * Time.deltaTime);
        }        
        #endregion
        #region FOLLOW CAMERA

        private void HandleAllCameraActions()
        {
            if (_target != null)
            {
                // FOLLOW TARGET
                // ROTATE AROUND THE TARGET
                // COLLIDE WITH OBJECTS
            }
        }

        private void FollowTarget()
        {
            if (!_target)
            {
                Debug.LogAssertion("NO TARGET SET FOR THE CAMERA."); 
                return;
            }

            Vector3 pos = _target.position + offset;
            transform.position = Vector3.Lerp(transform.position, pos, smoothSpeed * Time.deltaTime);
        }
        #endregion

        #region COMMON

        private void Orbit()
        {
            //float mouseX = Input.GetAxis("Mouse X");

            //float rotationInput = InputActio
            //Click and drag to rotate camera pivot
            //if (Input.GetMouseButton(1))
            //{
            //    targetAngle += mouseX * mouseSensitivity;
            //}
            //else
            //{   //Let go the mouse; the camera pivot snaps to an increment of 45
            //    targetAngle = Mathf.Round(targetAngle / 45);
            //    targetAngle *= 45;
            //}

            if (_controls.Camera.Orbit.WasPerformedThisFrame())
            {
                targetAngle += orbitInput * 45.0f;
            }            

            //Angle correction 0 to 360
            if (targetAngle < 0.0f) targetAngle += 360.0f;
            if (targetAngle > 360.0f) targetAngle -= 360.0f;

            currentAngle = Mathf.LerpAngle(transform.eulerAngles.y, targetAngle, rotationSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(30, currentAngle, 0);
        }

        private void Zoom()
        {
            //float scrollInput = Input.mouseScrollDelta.y;

            //scrollInput = Input.GetAxis("Mouse ScrollWheel");

            float newZoom = _viewport.GetViewportZoom();
            newZoom -= scrollInput * zoomSensitivity;
            _viewport.ZoomViewport(Mathf.Clamp(newZoom, minZoom, maxZoom));

            //if (scrollInput != 0.0f)
            //{
            //    float newZoom = _viewport.GetViewportZoom();
            //    newZoom += -scrollInput * zoomSpeed * Time.deltaTime;
            //    _viewport.ZoomViewport(Mathf.Clamp(newZoom, minZoom, maxZoom));
            //}
        }

        #endregion COMMON
    }
}