using UnityEngine;


namespace SaveSystem
{
    [System.Serializable]
    public class SettingsSaveData
    {
        [Header("Video Settings")]
            [Header("Resolution")]
            public int resolutionIndex = 0;

            [Header("Fullscreen")]
            public bool fullscreen = true;


        [Space(10.0f)]

        [Header("Audio Settings")]


        [Space(10.0f)]

        [Header("Other Settings")]
            [Header("Automatic Saving System")]
            public bool saveAutomatically = true;
    }
}