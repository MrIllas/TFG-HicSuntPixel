using UnityEngine;
using UnityEngine.Rendering.Universal;

[ExecuteAlways]
public class ViewportBlitter : MonoBehaviour
{
    [SerializeField] private UniversalRendererData _data = null;
    [SerializeField] public Camera _renderCamera;
    [HideInInspector] public Camera _ViewportCamera;


    [SerializeField]
    public Vector2Int referenceResolution = new Vector2Int(640, 360);
    public int pixelMargin = 4;
    [HideInInspector] int pixelMarginY = 1;

    private Vector2Int screenSize;

    [HideInInspector] public float orthographicSize;
    [HideInInspector] public Vector2Int cameraResolution;
    [SerializeField] private Vector2 pixelValue;
    [HideInInspector] public Vector2 renderOffsetInPixels;

    [Range(0.0f, 1.0f)][SerializeField] private float viewportZoom = 1.0f; 

    private HicSuntPixelFeature _hspFeature;

    //GETTERS
    public float GetViewportZoom() { return viewportZoom; }

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

    private void Awake()
    {
        screenSize = new Vector2Int(Screen.width, Screen.height);

        _ViewportCamera = GetComponent<Camera>();

    }

    void Start()
    {
        SetFeature();

        CalculatePixelMarginY();
    }

    void Update()
    {
        //Must be done first
        DetectScreenResize();

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
        cameraResolution = new Vector2Int(referenceResolution.x + pixelMargin, referenceResolution.y + pixelMarginY);

        _hspFeature._settings.cameraResolution = cameraResolution;
        //_hspFeature._settings.screenResolution = new Vector2Int(Screen.width + 12, Screen.height + 12);
        _hspFeature._settings.screenResolution = new Vector2Int(screenSize.x, screenSize.y);

        CalculatePixelMarginY();
        CalculatePixelValue();
        CameraScale();
        CameraCenter();

        SetMargin();
        SetScale();
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
        marginY = pixelValue.y * ((pixelMarginY / 2.0f) + renderOffsetInPixels.y);

        SetMargin();
    }

    public void ZoomViewport(float newZoom)
    {
        viewportZoom = newZoom;
        SetMargin();
        SetScale();
    }

    private void SetMargin()
    {
        _hspFeature._settings.margin = new Vector2(marginX+ ((1 - viewportZoom) / 2), marginY + ((1 - viewportZoom) / 2));
    }

    private void SetScale()
    {
        _hspFeature._settings.scale = new Vector2(pixelScaleX * viewportZoom, pixelScaleY * viewportZoom);
    }

    private void CalculatePixelValue()
    {
        orthographicSize = _renderCamera.orthographicSize;

        pixelValue = new Vector2((float)orthographicSize * 2.0f / cameraResolution.x * 0.10f, (float)orthographicSize * 2.0f / cameraResolution.y * 0.10f);
    }

    private void CameraScale()
    {
        //Scale
        //pixelScaleX = (1 - (pixelValue.x * pixelMargin));
        pixelScaleX = 1.0f;
        pixelScaleY = (1 - (pixelValue.y * pixelMarginY));
    }

    private void CameraCenter()
    {
        //Center
        marginX = pixelValue.x * pixelMargin / 2.0f;
        marginY = pixelValue.y * pixelMarginY / 2.0f;
    }

    private void CalculatePixelMarginY()
    {
        pixelMarginY = Mathf.FloorToInt(pixelMargin * (1.0f / _renderCamera.aspect));
    }

    private void DetectScreenResize()
    {
        if (Screen.width != screenSize.x || Screen.height != screenSize.y)
        {
            //Debug.Log("The Screen has been resized from ("+screenSize.x+", "+screenSize.y+") to ("+Screen.width+", "+Screen.height+").");
            screenSize.x = Screen.width;
            screenSize.y = Screen.height;

            OnValidate();
        }
    }
}