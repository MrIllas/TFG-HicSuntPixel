using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WeatherManager : MonoBehaviour
{

    [Header("Clouds")]
    [SerializeField]
    private bool _cloudsOnEditor = false;

    [SerializeField]
    private CustomRenderTexture _cloudsRenderTexture;
    private Material _cloudsMaterial;

    [SerializeField]
    private Vector2 cloudsShadowSize = new Vector2(5, 7);

    [SerializeField, Range(0.001f, 0.05f)]
    private float cloudSpeed = 0.001f;

    [SerializeField]
    private Vector2 cloudDirection = new Vector2(1.0f, 1.0f);

    [SerializeField, Range(0.0f, 1.0f)]
    private float cloudDensity = 1.0f;


    [Header("Wind")]

    [SerializeField]
    private bool _windOnEditor = false;

    [SerializeField]
    private Material[] _windableMaterial;

    [SerializeField, Range(0.0f, 1.0f)]
    private float windStrength = 1.0f;

    [SerializeField]
    private Vector2 windDirection = new Vector2(1.0f, 1.0f);

    [SerializeField, Range(0.0f, 1.0f)]
    private float windDensity = 1.0f;

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

        ValidateClouds();
        ValidateWind();
    }

    private void Awake()
    {
        OnValidate();
    }

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
}
