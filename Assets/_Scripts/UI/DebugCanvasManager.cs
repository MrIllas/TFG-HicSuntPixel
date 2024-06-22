using UnityEngine;
using HicSuntPixel;
using UnityEngine.UI;

public class DebugCanvasManager : MonoBehaviour
{
    [HideInInspector] public HSPCameraManager _cameraManager;

    [Header("Toggles")]
    [SerializeField] private Toggle _snapToggle;
    [SerializeField] private Toggle _cameraSmoothingToggle;

    void Start()
    {
        _cameraManager = HSPCameraManager.instance;

        Initialize();
    }

    void Update()
    {
        
    }


    private void Initialize()
    {
        _snapToggle.isOn = _cameraManager._snap;
        _cameraSmoothingToggle.isOn = _cameraManager._subPixelSnap;

    }

    public void OnSnapToggle(bool value)
    {
        _cameraManager._snap = value;
    }

    public void OnSmoothingToggle(bool value)
    {
        _cameraManager._subPixelSnap = value;
    }

}
