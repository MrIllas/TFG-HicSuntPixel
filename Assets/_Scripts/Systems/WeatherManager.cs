using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static WeatherManager;

public class WeatherManager : MonoBehaviour
{
    public struct Wind
    {
        public string name;
        public float windStrength;
        public float windDensity;
        public float cloudsSpeed;
    }

    public struct Weather
    {
        public string name;
        public float cloudDensity;
        public Wind wind;
        public bool changeWindDirection; // We will always change randomly the win direction, so the change of weather doesn't always look the same.
        public bool sameWindDirectionSkyGround;
        public bool rain;
    }

    public bool _nextWeather = false;

    //Flags
    [Header("Flags")]
    [SerializeField] private bool _dynamicWeather = true;
    [SerializeField] private bool _cloudsOnEditor = false;
    [SerializeField] private bool _windOnEditor = false;

    // Static Configuration
    [Header("Clouds")]
    [SerializeField] private CustomRenderTexture _cloudsRenderTexture;
    [SerializeField] private Vector2 cloudsShadowSize = new Vector2(5, 7);
    [SerializeField, Range(0.0f, 100.0f)] private float cloudSpeed = 0.001f;
    [SerializeField] public Vector2 cloudDirection = new Vector2(1.0f, 1.0f);
    [SerializeField, Range(0.0f, 1.0f)] public float cloudDensity = 1.0f;
    private Material _cloudsMaterial;

    [Header("Wind")]
    [SerializeField] private Material[] _windableMaterial;
    [SerializeField, Range(0.0f, 1.0f)] private float windStrength = 1.0f;
    [SerializeField] public Vector2 windDirection = new Vector2(1.0f, 1.0f);
    [SerializeField, Range(0.0f, 1.0f)] private float windDensity = 1.0f;


    // Dyanmic Weather
    Queue<Weather> weatherQueue = new Queue<Weather>();
    int weatherQueueSize = 4;
    [HideInInspector] public Weather currentWeather;
    bool transitioningWeather = false;

    Wind[] windConfiguration;
    Weather[] weatherConfiguration;

    private void Awake()
    {
        InitializeConfigurations();
        OnValidate();
    }

    private void OnValidate()
    {
        if (_cloudsRenderTexture == null) return;
        else
        {
            if (_cloudsMaterial == null)
            {
                _cloudsMaterial = _cloudsRenderTexture.material;
            }
        }

        if (!_dynamicWeather)
        {
            ValidateClouds();
            ValidateWind();
        }
    }

    private void Start()
    {
        // Set Initial Weather
        Weather w = weatherConfiguration[Random.Range(0, weatherConfiguration.Length)];
        w.wind = windConfiguration[Random.Range(0, windConfiguration.Length)];
        currentWeather = w;
        ForceNextWeather();

        // Fill weather list
        FillWeatherQueue();
    }

    private void Update()
    {
        DyanmicWeather();
    }

    #region Dynamic Weather
    private void DyanmicWeather()
    {
        if (_nextWeather)
        {
            currentWeather = weatherQueue.Dequeue();

            FillWeatherQueue();
            transitioningWeather = true;
            _nextWeather = false;
        }

        WeatherTransition();
    }

    private void WeatherTransition()
    {
        if (transitioningWeather)
        {
            ForceNextWeather();

            transitioningWeather = false;
        }
    }

    // Unlike weather transition, this is done in just one frame.
    private void ForceNextWeather()
    {
        SetDensity(currentWeather.cloudDensity);
        SetWind(currentWeather.wind, currentWeather.sameWindDirectionSkyGround);
    }

    private void FillWeatherQueue()
    {
        if (weatherQueue.Count < weatherQueueSize)
        {
            for (int i = 0;  i < (weatherQueueSize - weatherQueue.Count); i++)
            {
                int rand = Random.Range(0, weatherConfiguration.Length);

                Weather w = weatherConfiguration[rand];
                w.wind = windConfiguration[Random.Range(0, windConfiguration.Length)];
                weatherQueue.Enqueue(w);
            }
        }
    }

    #endregion

    #region Clouds
    private void ValidateClouds()
    {
#if UNITY_EDITOR
        if (!EditorApplication.isPlaying && !_cloudsOnEditor)
        {
            _cloudsMaterial.SetFloat("_Density", 0);
            return;
        }       
#endif
        _cloudsMaterial.SetFloat("_ShadowSize1", cloudsShadowSize[0]);
        _cloudsMaterial.SetFloat("_ShadowSize2", cloudsShadowSize[1]);
        _cloudsMaterial.SetFloat("_Speed", cloudSpeed);
        _cloudsMaterial.SetVector("_Direction", cloudDirection);
        _cloudsMaterial.SetFloat("_Density", cloudDensity);
    }
    #endregion Clouds

    #region Wind
    private void ValidateWind()
    {
#if UNITY_EDITOR
        if (!EditorApplication.isPlaying && !_windOnEditor)
        {
            for (int j = 0; j < _windableMaterial.Length; ++j)
            {
                _windableMaterial[j].SetFloat("_WindDensity", 0);
            }
            return;
        }
#endif

        for (int i = 0; i < _windableMaterial.Length; ++i)
        {
            _windableMaterial[i].SetFloat("_WindDensity", windDensity);
            _windableMaterial[i].SetFloat("_WindStrength", windStrength);
            _windableMaterial[i].SetVector("_WindDirection", windDirection);
        }
    }
    #endregion Wind

    #region Help Methods

    private void InitializeConfigurations()
    {
        windConfiguration = new Wind[]
        {
            new Wind { name = "Calm", windStrength = 0.1f, windDensity = 0.2f, cloudsSpeed = 2.0f},
            new Wind { name = "Breeze", windStrength = 0.5f, windDensity = 0.5f, cloudsSpeed = 3.0f },
            new Wind { name = "Windy", windStrength = 1.0f, windDensity = 1.0f, cloudsSpeed = 4.0f }
        };

        weatherConfiguration = new Weather[]
        {
                new Weather { name = "Clear", cloudDensity = 0.0f, changeWindDirection = false, sameWindDirectionSkyGround = false, rain = false },// wind = windConfiguration[0] },
                new Weather { name = "Ptly. Cloudy", cloudDensity = 0.2f, changeWindDirection = true, sameWindDirectionSkyGround = false, rain = false }, //, wind = windConfiguration[1] },
                new Weather { name = "Cloudy", cloudDensity = 0.4f, changeWindDirection = true, sameWindDirectionSkyGround = false, rain = false }, //, wind = windConfiguration[1] },
                new Weather { name = "Overcast", cloudDensity = 1.0f, changeWindDirection = true, sameWindDirectionSkyGround = true, rain = false } //, wind = windConfiguration[2] },
        };
    }

    private void SetDensity(float density)
    {
        if (density == 1)
        {
            _cloudsMaterial.SetFloat("_ShadowSize1", cloudsShadowSize[0]);
            _cloudsMaterial.SetFloat("_ShadowSize2", cloudsShadowSize[1]+1);
        }
        else
        {
            _cloudsMaterial.SetFloat("_ShadowSize1", cloudsShadowSize[0]);
            _cloudsMaterial.SetFloat("_ShadowSize2", cloudsShadowSize[1]);
        }

        cloudDensity = density;
        _cloudsMaterial.SetFloat("_Density", density);
    }

    private void SetWind(Wind wind, bool sameDirectionSkyGround = false)
    {
        Vector2 dir = new Vector2(Random.Range(-1, 1), Random.Range(-1, 1));
        cloudDirection = dir;
        windDirection = dir;

        // Set Clouds Wind
        _cloudsMaterial.SetFloat("_Speed", wind.cloudsSpeed);      
        _cloudsMaterial.SetVector("_Direction", dir);

        // Set Wind Wind
        if (sameDirectionSkyGround)
        {
            dir = new Vector2(dir.y, dir.x); // Values need to be swapped if we want same direction
        }
        else
        {
            dir = new Vector2(Random.Range(-1, 1), Random.Range(-1, 1));
            windDirection = dir;
        }
            
        for (int i = 0; i < _windableMaterial.Length; ++i)
        {

            _windableMaterial[i].SetVector("_WindDirection", dir);

            _windableMaterial[i].SetFloat("_WindDensity", wind.windDensity);
            _windableMaterial[i].SetFloat("_WindStrength", wind.windStrength);

        }
    }
    #endregion
}
