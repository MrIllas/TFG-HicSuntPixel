using UnityEngine;
using HicSuntPixel;
using UnityEngine.UI;

namespace HicSuntPixel
{
    public class HSPDebugCanvas : MonoBehaviour
    {
        [HideInInspector] public HSPCameraManager _cameraManager;
        [HideInInspector] public CameraController _cameraController;

        [Header("Toggles")]
        [SerializeField] private Toggle _snapToggle;
        [SerializeField] private Toggle _cameraSmoothingToggle;
        [SerializeField] private Toggle _objectsSnapToggle;

        [Header("Pixel Scale Slider")]
        [SerializeField] private Text _pixelScaleText;
        [SerializeField] private Slider _pixelScaleSlider;
        private string pixelScaleBasePrompt = "Original res.: ";

        [Header("Camera Speed Slider")]
        [SerializeField] private Text _cameraSpeedText;
        [SerializeField] private Slider _cameraSpeedSlider;
        private string cameraSpeedBasePrompt = "Camera speed: ";

        [Header("Object Snapping")]
        [SerializeField] private bool objectSnapping = true;
        [SerializeField] private Snapper[] objectsSnapper;

        [Header("Controls Group")]
        [SerializeField] private GameObject controlsGroup;

        private float fps = 0;


        void Start()
        {
            _cameraManager = HSPCameraManager.instance;
            _cameraController = HSPCameraManager.instance.GetComponent<CameraController>();

            Initialize();
            _cameraManager.gameObject.SetActive(true);
        }

        private void OnGUI()
        {
            Fps();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F2))
            {
                controlsGroup.SetActive(!controlsGroup.activeInHierarchy);
            }
        }

        private void Initialize()
        {
            _snapToggle.isOn = _cameraManager._snap;
            _cameraSmoothingToggle.isOn = _cameraManager._subPixelSnap;

            _pixelScaleText.text = pixelScaleBasePrompt + _cameraManager.realResolution.x + " x " + _cameraManager.realResolution.y;
            _pixelScaleSlider.maxValue = 1080;
            _pixelScaleSlider.value = _cameraManager.aspectScale;
            _pixelScaleSlider.minValue = 9;

            _cameraSpeedText.text = cameraSpeedBasePrompt + _cameraController.panSpeed;
            _cameraSpeedSlider.maxValue = 10;
            _cameraSpeedSlider.value = _cameraController.panSpeed;
            _cameraSpeedSlider.minValue = 0.1f;

            if (objectsSnapper.Length == 0)
            {
                _objectsSnapToggle.enabled = false;
            }
        }

        public void OnSnapToggle(bool value)
        {
            _cameraManager._snap = value;

            _cameraSmoothingToggle.interactable = value;
        }

        public void OnSmoothingToggle(bool value)
        {
            _cameraManager._subPixelSnap = value;
        }

        public void OnPixelScaleSlider(float value)
        {
            _cameraManager.aspectScale = Mathf.RoundToInt(value);
            _pixelScaleText.text = pixelScaleBasePrompt + _cameraManager.realResolution.x + " x " + _cameraManager.realResolution.y;
            _cameraManager.OnValidate();
        }

        public void OnCameraSpeedSlidier(float value)
        {
            _cameraController.panSpeed = Mathf.RoundToInt(value);
            _cameraSpeedText.text = cameraSpeedBasePrompt + _cameraController.panSpeed;
        }

        public void OnObjectSnappingToggle(bool value)
        {
            foreach (Snapper obj in objectsSnapper)
            {
                if (obj != null) obj.enabled = value;
            }
        }

        private void Fps()
        {
            if (Time.smoothDeltaTime > 0)
            {
                float newFPS = 1.0f / Time.unscaledDeltaTime;
                fps = Mathf.Lerp(fps, newFPS, 0.005f);
            }

            GUI.Label(new Rect(5, 0, 500, 500), "FPS: " + ((int)fps).ToString());
        }
    }
}