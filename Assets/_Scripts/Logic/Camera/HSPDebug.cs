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

        private float fps;

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
        }

        private void OnGUI()
        {
            if (!_Debug) return;

            Fps();
            CameraData();
        }

        private void CameraData()
        {
            Vector2Int viewportResolution = new Vector2Int(Screen.width, Screen.height);

            string text = "Real / Viewport Res. = (" + _manager.realResolution.x + "x" + _manager.realResolution.y + ") | ";
            text += "(" + viewportResolution.x + "x" + viewportResolution.y + ")\n";

            GUI.Label(new Rect(0, 15, 500, 500), text);
        }

        private void Fps()
        {
            float newFPS = 1.0f / Time.smoothDeltaTime;
            fps = Mathf.Lerp(fps, newFPS, 0.005f);
            GUI.Label(new Rect(0, 0, 100, 100), "FPS: " + ((int)fps).ToString());
        }
    }
}