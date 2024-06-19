using UnityEngine;

namespace HicSuntPixel
{
    public class HSPDebug : MonoBehaviour
    {
        public bool _Debug = false;

        //Unity Screen Render Data signleton
        [SerializeField] private HSPCameraManager _manager;
        [SerializeField] private WeatherManager _weather;
        [SerializeField] private DayNightCycle _dayNightCycle;

        private float fps = 0;
        private string BuildNumber = "0";
        private string version;

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
            if (Time.smoothDeltaTime > 0)
            {
                float newFPS = 1.0f / Time.unscaledDeltaTime;
                fps = Mathf.Lerp(fps, newFPS, 0.005f);
            }

            if (Input.GetKeyDown(KeyCode.F10)) _Debug = !_Debug;
            
            if (_Debug)
            {
                viewportResolution = new Vector2Int(Screen.width, Screen.height);
            }

            if (Input.GetKeyDown(KeyCode.F1))
            {
                _manager._snap = !_manager._snap;
            }
            if (Input.GetKeyDown(KeyCode.F2))
            {
                _manager._subPixelSnap = !_manager._subPixelSnap;
            }
            if (_dayNightCycle != null) 
            {
                if (Input.GetKeyDown(KeyCode.F3))
                {
                    _dayNightCycle.timeOfTheDay += 1.0f;
                }
            } 
        }

        private void OnGUI()
        {
            if (!_Debug) return;

            Fps();
            BuildVersion();
            CameraData();
            WeatherData();

#if UNITY_STANDALONE && !UNITY_EDITOR
            string text = "F1 - Pixel Snaping On/Off \n";
            text += "F2 - Snap smoothing \n";
            text += "F3 - Advance an hour \n";
            GUI.Label(new Rect(5, 45, 500, 500), text);
#endif
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
                version = "v. " + Application.version + "." + buildScriptableObject.BuildNumber;
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
            text += "\n";
            text += _dayNightCycle.GetTime() + "\n";
            text += "Day Length -> " + _dayNightCycle.dayLengthInMinutes + " min \n";


            GUI.Label(new Rect(viewportResolution.x - 160, 15, 500, 500), text);
        }

        private void Fps()
        {
            GUI.Label(new Rect(5, 0, 500, 500), "FPS: " + ((int)fps).ToString());
        }

        private void BuildVersion()
        {
            GUI.Label(new Rect(5, 15, 500, 500), version);
        }
    }
}