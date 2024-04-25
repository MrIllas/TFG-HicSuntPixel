using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace HicSuntPixel
{
    public class HSPDebug : MonoBehaviour
    {
        public bool _Debug = false;

        //Unity Screen Render Data signleton
        [SerializeField] private HSPCameraManager _manager;
        [SerializeField] private WeatherManager _weather;

        private float fps;
        private string BuildNumber = "1";
        private string v;

        Vector2Int viewportResolution;

        private void Awake()
        {
            ResourceRequest request = Resources.LoadAsync("Build", typeof(BuildScriptableObject));
            request.completed += Request_completed;
        }

        private void Start()
        {
            // Disable vSync
            QualitySettings.vSyncCount = 0;
            // Make the game run as fast as possible
            Application.targetFrameRate = 300;
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F10)) _Debug = !_Debug;
            
            if (_Debug)
            {
                viewportResolution = new Vector2Int(Screen.width, Screen.height);
            }
        }

        private void OnGUI()
        {
            if (!_Debug) return;

            Fps();
            BuildVersion();
            CameraData();
            WeatherData();
        }

        private void Request_completed(AsyncOperation op)
        {
            BuildScriptableObject buildScriptableObject = ((ResourceRequest)op).asset as BuildScriptableObject;

            if (buildScriptableObject == null)
            {
                Debug.LogError("Build Scriptable Object not found in resources directory! Check Build log for errors.");
            }
            else
            {
                v = "v. " + Application.version + "." + buildScriptableObject.BuildNumber;
            }
        }

        private void CameraData()
        {
            string text = "Real / Viewport Res. = (" + _manager.realResolution.x + "x" + _manager.realResolution.y + ") | ";
            text += "(" + viewportResolution.x + "x" + viewportResolution.y + ")\n";

            GUI.Label(new Rect(5, 30, 500, 500), text);
        }

        private void WeatherData()
        {
            if (_weather == null) return;

            string text = "Weather -> " + _weather.currentWeather.name + "\n";
            text += "Wind -> " + _weather.currentWeather.wind.name + "\n";
            text += "Sky WD -> " + _weather.cloudDirection + "\n";
            text += "Ground WD -> " + _weather.windDirection + "\n";



            GUI.Label(new Rect(viewportResolution.x - 160, 15, 500, 500), text);
        }

        private void Fps()
        {
            float newFPS = 1.0f / Time.smoothDeltaTime;
            fps = Mathf.Lerp(fps, newFPS, 0.005f);
            GUI.Label(new Rect(5, 0, 100, 100), "FPS: " + ((int)fps).ToString());
        }

        private void BuildVersion()
        {
            GUI.Label(new Rect(5, 15, 100, 100), v);
        }
    }
}