using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    private Light sun;

    [SerializeField, Range(0.0f, 24.0f)]
    private float timeOfTheDay = 8.0f;

    [SerializeField, Range(1, 240)]
    private int dayLengthInMinutes = 8;

    [SerializeField]
    private float sunRotationSpeed = 0.1f;

    [Header("LightingPresets")]
    [SerializeField]
    private Gradient skyColor, equatorColor, sunColor;

    private void Awake()
    {
        sun = GetComponent<Light>();
        sunRotationSpeed = 24 / ((float) dayLengthInMinutes * 60);
    }

    //Editor
    private void OnValidate()
    {
        sun = GetComponent<Light>();
        sunRotationSpeed = 24 / ((float)dayLengthInMinutes * 60);

        UpdateSunRotation();
        UpdateLightning();

    }

    //Game
    private void Update()
    {
        UpdateSunRotation();
        UpdateLightning();

        timeOfTheDay += Time.deltaTime * sunRotationSpeed;

        if (timeOfTheDay > 24)
        {
            timeOfTheDay = 0.0f;
        }
    }

    private void UpdateSunRotation()
    {
        float sunRotation = Mathf.Lerp(-90, 270, timeOfTheDay / 24);
        sun.transform.rotation = Quaternion.Euler(sunRotation, -90, 0);
    }

    private void UpdateLightning()
    {
        float timeFraction = timeOfTheDay / 24;
        RenderSettings.ambientEquatorColor = equatorColor.Evaluate(timeFraction);
        RenderSettings.ambientSkyColor = skyColor.Evaluate(timeFraction);
        sun.color = sunColor.Evaluate(timeFraction);
    }
}
