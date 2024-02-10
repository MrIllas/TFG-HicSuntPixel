using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherManager : MonoBehaviour
{

    [Header("Clouds")]
    [SerializeField]
    private CustomRenderTexture _cloudsRenderTexture;
    private Material _cloudsMaterial;

    [SerializeField]
    private Vector2 cloudsShadowSize = new Vector2(5, 7);

    [SerializeField, Range(0.01f, 1.0f)]
    private float cloudSpeed = 0.01f;

    [SerializeField]
    private Vector2 cloudDirection = new Vector2(1.0f, 1.0f);

    [SerializeField, Range(0.0f, 1.0f)]
    private float cloudDensity = 1.0f;

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
    }

    private void Awake()
    {
        OnValidate();
    }

    #region Clouds
    private void ValidateClouds()
    {
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

    }
    #endregion Wind
}
