using UnityEngine;
using HicSuntPixel;
using UnityEngine.UI;
using TMPro;
using static UnityEngine.Rendering.DebugUI;
using Globals;

public class DebugCanvasManager : MonoBehaviour
{
    [HideInInspector] public HSPCameraManager _cameraManager;
    [HideInInspector] public CameraController _cameraController;
    [HideInInspector] public DayNightCycle _dayManager;

    [Header("Toggles")]
    [SerializeField] private Toggle _snapToggle;
    [SerializeField] private Toggle _cameraSmoothingToggle;
    [SerializeField] private Toggle _objectsSnapToggle;

    [Header("Pixel Scale Slider")]
    [SerializeField] private TextMeshProUGUI _pixelScaleText;
    [SerializeField] private Slider _pixelScaleSlider;
    private string pixelScaleBasePrompt = "Original res.: ";

    [Header("Camera Speed Slider")]
    [SerializeField] private TextMeshProUGUI _cameraSpeedText;
    [SerializeField] private Slider _cameraSpeedSlider;
    private string cameraSpeedBasePrompt = "Camera speed: ";

    [Header("Object Snapping")]
    [SerializeField] private bool objectSnapping = true;
    [SerializeField] private Snapper[] objectsSnapper;

    [Header("Time Slider")]
    [SerializeField] private TextMeshProUGUI _timeText;
    [SerializeField] private Slider _timeSlider;
    private string timeBasePrompt = "Time: ";

    void Start()
    {
        _cameraManager = HSPCameraManager.instance;
        _cameraController = HSPCameraManager.instance.GetComponent<CameraController>();
        _dayManager = DayNightCycle.instance;

        Initialize();
        _cameraManager.gameObject.SetActive(true);
    }

    void Update()
    {
        _timeSlider.value = _dayManager.timeOfTheDay;
        _timeText.text = timeBasePrompt + _dayManager.GetTime();
    }


    private void Initialize()
    {
        _snapToggle.isOn = _cameraManager._snap;
        _cameraSmoothingToggle.isOn = _cameraManager._subPixelSnap;

        _pixelScaleText.text = pixelScaleBasePrompt + _cameraManager.realResolution.x +" x "+_cameraManager.realResolution.y;
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

        _timeText.text = timeBasePrompt + _dayManager.GetTime();
        _timeSlider.maxValue = 24.0f;
        _timeSlider.value = _dayManager.timeOfTheDay;
        _timeSlider.minValue = 0.0f;
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
            obj.enabled = value;
        }
    }

    public void OnTimeSlider(float value)
    {
        _dayManager.timeOfTheDay = value;
        _timeText.text = timeBasePrompt + _dayManager.GetTime();
    }

    public void OnExitButtonClick()
    {
        StartCoroutine(WorldSaveGameManager.instance.LoadBackToTitleScene());
        _pixelScaleSlider.value = 450;
        _cameraManager.aspectScale = 450;
    }
}
