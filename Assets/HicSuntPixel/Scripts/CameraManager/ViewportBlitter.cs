using UnityEngine;
using UnityEngine.Rendering.Universal;

[ExecuteAlways]
public class ViewportBlitter : MonoBehaviour
{
    [SerializeField] private UniversalRendererData _data = null;
    [SerializeField] public Camera _renderCamera;


    [SerializeField]
    public Vector2Int referenceResolution = new Vector2Int(640, 360);

    public int pixelMargin = 4;
    public float orthographicSize;
    public Vector2Int cameraResolution;
    private Vector2 pixelValue;
    public Vector2 renderOffsetInPixels;

    private HicSuntPixelFeature _hspFeature;

 #region Debug Only
    [Header("Debug Only")]
    [SerializeField]
    [Range(-1.0f, 1.0f)]
    private float marginX = 0;

    [SerializeField]
    [Range(-1.0f, 1.0f)]
    private float marginY = 0;

    [SerializeField]
    [Range(-1.0f, 1.0f)]
    private float pixelScaleX = 1;
    [SerializeField]
    [Range(-1.0f, 1.0f)]
    private float pixelScaleY = 1;
 #endregion Debug Only

    void Start()
    {
        SetFeature();
    }

    void Update()
    {
        ReCenter();
    }

    private void OnValidate()
    {
        TryGetFeature();
        SetFeature();
        _data.SetDirty();
    }

    #region Feature
    private void SetFeature()
    {
        cameraResolution = new Vector2Int(referenceResolution.x + pixelMargin, referenceResolution.y + pixelMargin);
        _hspFeature._settings.cameraResolution = cameraResolution;
        _hspFeature._settings.screenResolution = new Vector2Int(Screen.width + 12, Screen.height + 12);

        CalculatePixelValue();
        CameraScale();
        CameraCenter();

        _hspFeature._settings.margin = new Vector2(marginX, marginY);
        _hspFeature._settings.scale = new Vector2(pixelScaleX, pixelScaleY);

    }

    private void TryGetFeature()
    {
        if (_hspFeature != null) return;

        ScriptableRendererFeature feature = _data.rendererFeatures.Find((f) => f.name == "HicSuntPixelFeature");
    
        if (feature != null)
        {
            _hspFeature = feature as HicSuntPixelFeature;
        }
    }
    #endregion Feature

    private void ReCenter()
    {
        //X Axis
        if (renderOffsetInPixels.x >= 1 || renderOffsetInPixels.x <= -1)
        {
            float value = Mathf.Sign(renderOffsetInPixels.x) == -1 ? 1 : -1;

            renderOffsetInPixels.x += value;
            _renderCamera.transform.Translate(pixelValue.y * 10.0f * -value, 0, 0);
        }

        //Y Axis
        if (renderOffsetInPixels.y >= 1 || renderOffsetInPixels.y <= -1)
        {
            float value = Mathf.Sign(renderOffsetInPixels.y) == -1 ? 1 : -1;

            renderOffsetInPixels.y += value;

            _renderCamera.transform.Translate(0, pixelValue.y * 10.0f * -value, 0);
        }
    }

    public void PanViewport (float x, float y)
    {
        renderOffsetInPixels.x += x;
        renderOffsetInPixels.y -= y;

        marginX = pixelValue.x * ((pixelMargin / 2.0f) + renderOffsetInPixels.x);
        marginY = pixelValue.y * ((pixelMargin / 2.0f) + renderOffsetInPixels.y);

        SetMargin();
    }

    private void SetMargin()
    {
        _hspFeature._settings.margin = new Vector2Int((int) marginX, (int) marginY);
    }

    private void CalculatePixelValue()
    {
        orthographicSize = _renderCamera.orthographicSize;

        pixelValue = new Vector2((float)orthographicSize * 2.0f / cameraResolution.x * 0.10f, (float)orthographicSize * 2.0f / cameraResolution.y * 0.10f);
    }

    private void CameraScale()
    {
        //Scale
        pixelScaleX = 1 - (pixelValue.x * pixelMargin);
        pixelScaleY = 1 - (pixelValue.y * pixelMargin);
    }

    private void CameraCenter()
    {
        //Center
        marginX = pixelValue.x * pixelMargin / 2.0f;
        marginY = pixelValue.y * pixelMargin / 2.0f;
    }
}
