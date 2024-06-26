using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DayNightCycle : MonoBehaviour
{
    public static DayNightCycle instance;

    private Light sun;

    [SerializeField, Range(0.0f, 24.0f)]
    public float timeOfTheDay = 8.0f;

    [SerializeField, Range(1, 240)]
    public int dayLengthInMinutes = 8;

    [SerializeField]
    private float sunRotationSpeed = 0.1f;

    [Header("LightingPresets")]
    [SerializeField]
    private Gradient skyColor, equatorColor, sunColor;
    //private Gradient equatorColor, sunColor;

    [HideInInspector] public Color currentLightColor;

    private Vector2 cookieSize = new Vector2(200, 200);
    private UniversalAdditionalLightData lightData;

    private Vector2 dayLightSpan = new Vector2(6.0f, 18.0f);

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
        sun = GetComponent<Light>();
        lightData = GetComponent<UniversalAdditionalLightData>();
        sunRotationSpeed = 24 / ((float) dayLengthInMinutes * 60);
    }

    //Editor
    private void OnValidate()
    {
        sun = GetComponent<Light>();
        lightData = GetComponent<UniversalAdditionalLightData>();
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
        //Rotate Sun during the day
        //if (IsItDayTime())
        //{
            float sunRotation = Mathf.Lerp(-90, 270, timeOfTheDay / 24);
            sun.transform.rotation = Quaternion.Euler(sunRotation, -90, 0);
        //}
        //// During the night keep the "SUN" up and make it be a moon
        //else {
        //    float moonPosition = 45.0f;
        //    sun.transform.rotation = Quaternion.Euler(moonPosition, -90, 0);
        //}
        if (IsItDayTime())
        {
            sun.intensity = 1.0f;
        }
        else
        {
            sun.intensity = 0.0f;
        }
    }

    private void UpdateLightning()
    {
        float timeFraction = timeOfTheDay / 24;
        RenderSettings.ambientEquatorColor = equatorColor.Evaluate(timeFraction);
        RenderSettings.ambientSkyColor = skyColor.Evaluate(timeFraction);
        currentLightColor = sunColor.Evaluate(timeFraction);
        sun.color = currentLightColor;
    }

    #region Help Methods
    public bool IsItDayTime()
    {
        if (timeOfTheDay >= dayLightSpan.x && timeOfTheDay <= dayLightSpan.y)
        {
            return true;
        }

        return false;
    }

    public string GetHour()
    {
        string toReturn = "00";

        int hour = Mathf.FloorToInt(timeOfTheDay);
        hour %= 24;

        toReturn = hour.ToString("00");

        return toReturn;
    }

    public string GetMinute()
    {
        string toReturn = "00";

        int minute = (int)((timeOfTheDay - (int)timeOfTheDay) * 60);

        toReturn = minute.ToString("00");

        return toReturn;
    }

    public string GetTime()
    {
        return GetHour() + ":" + GetMinute();
    }
    #endregion Help Methods
}
