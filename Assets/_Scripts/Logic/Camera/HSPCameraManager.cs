using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace HicSuntPixel
{
    [ExecuteAlways]
    public class HSPCameraManager : MonoBehaviour
    {
        //Links
        HicSuntPixelFeature _hspFeature;

        [SerializeField] private    UniversalRendererData   _renderData = null;
        [SerializeField] public     Camera                  _renderCamera;
        [SerializeField] public     Camera                  _viewportCamera;
        [SerializeField] public     Transform               _snapPoint;
        [SerializeField] public     Transform               _rotationPoint;

        //Flags
        [Header("FLAGS")]
            public  bool _snap          = true;
            public  bool _subPixelSnap  = true;
        
            [HideInInspector] 
            public  bool _rotating      = false;

        [Header("SETTINGS")]
            public int aspectScale = 270;
            [Range(0.0f, 1.0f)][SerializeField] private float viewportZoom = 1.0f;

            [HideInInspector]public Vector2Int realResolution = new Vector2Int(640, 360);
            [HideInInspector]public Vector2Int viewportResolution = Vector2Int.zero;
            Vector2 pixelSize;
            Vector2 viewportPixelSize = Vector2.zero;
            Vector2Int totalPixelsOfMargin = new Vector2Int(2, 2);
            Vector2 pixelOffset;

        //GETTERS
        public float GetViewportZoom() { return viewportZoom; }

        //////////////////

        private void Awake()
        {

//#if UNITY_STANDALONE && !UNITY_EDITOR
//            OnValidate();
//#endif

            //TryGetFeature();
            
        }

        private void OnValidate()
        {
            TryGetFeature();
            Calculate();
            SetFeature();
            if (_renderData != null) _renderData.SetDirty();
        }

        public void ZoomViewport(float newZoom)
        {
            viewportZoom = newZoom;
            _hspFeature._settings.scale = GetViewportScale();
            _hspFeature._settings.margin = GetViewportOffset();
        }

        private void Update()
        {
            //Detect if the screen is resized
            DetectViewportResize(true);
        }

        private void LateUpdate()
        {
            Snapping();

#if UNITY_EDITOR
            if (_hspFeature != null) 
            {
                _hspFeature._settings.scale = GetViewportScale();
                _hspFeature._settings.margin = GetViewportOffset();
            }
#endif
        }

        #region Viewport Snapping
        private Vector2 GetViewportOffset()
        {
            if (!_subPixelSnap)
                return new Vector2((1 - viewportZoom) / 2, (1 - viewportZoom) / 2);

            if (_subPixelSnap)
            {
                Vector2 snapTransformPosition = _renderCamera.WorldToViewportPoint(_snapPoint.position);
                pixelOffset = (snapTransformPosition - new Vector2(0.5f, 0.5f));
            }

            return new Vector2(pixelOffset.x + ((1 - viewportZoom) / 2), pixelOffset.y + ((1 - viewportZoom) / 2));
        }

        private Vector2 GetViewportScale()
        {
            if (!_subPixelSnap)
            {
                return Vector2.one;
            }

            Vector2 viewportScale;

            viewportScale.x = 1 - (viewportPixelSize.x * totalPixelsOfMargin.x);
            viewportScale.y = 1 - (viewportPixelSize.y * totalPixelsOfMargin.y);

            return (viewportScale * viewportZoom);
        }
        #endregion
        #region Render Camera Snapping
        public void Snapping()
        {
            if (_snap && !_rotating)
                _renderCamera.transform.position = SnapPosition(_snapPoint.position);
            else
                _renderCamera.transform.position = _snapPoint.position;
        }

        private Vector3 SnapPosition(Vector3 wp)
        {
            Vector3 aux = _renderCamera.transform.InverseTransformDirection(wp);
            aux = Vector3Int.RoundToInt(aux / pixelSize.y);

            Vector3 snappedVec = aux * pixelSize.y;

            Vector3 toReturn;
            toReturn = snappedVec.x * _renderCamera.transform.right;
            toReturn += snappedVec.y * _renderCamera.transform.up;
            toReturn += snappedVec.z * _renderCamera.transform.forward;

            return toReturn;
        }
        #endregion

        #region Render Feature
        private void SetFeature()
        {
            if (_hspFeature != null)
            {
                _hspFeature._settings.realResolution = realResolution;
                _hspFeature._settings.viewportResolution = viewportResolution;
            }
        }

        private void TryGetFeature()
        {
            if (_hspFeature != null && _renderData != null) return;

            ScriptableRendererFeature feature = null;
            try
            {
                feature = _renderData.rendererFeatures.Find((f) => f.name == "HicSuntPixelFeature");
            }
            catch (System.NullReferenceException e){}
                

            if (feature != null)
            {
                _hspFeature = feature as HicSuntPixelFeature;
            }
        }

        #endregion Feature

        #region Auxiliar Functions

        // Calculates everything that needs to be calculated each time there is a variation in data
        private void Calculate()
        {
            totalPixelsOfMargin.y = Mathf.FloorToInt(totalPixelsOfMargin.x * (1.0f / _renderCamera.aspect));
            Vector2Int reference = new Vector2Int(Mathf.RoundToInt(_viewportCamera.aspect * aspectScale), Mathf.RoundToInt(aspectScale));
            realResolution = new Vector2Int(reference.x + totalPixelsOfMargin.x, reference.y + totalPixelsOfMargin.y);


            if (_renderCamera != null)
            {
                pixelSize.y = 2.0f * _renderCamera.orthographicSize / realResolution.y;
                pixelSize.x = 2.0f * _renderCamera.orthographicSize / realResolution.x;

                viewportPixelSize = new Vector2(0.2f * _renderCamera.orthographicSize / realResolution.x,
                0.2f * _renderCamera.orthographicSize / realResolution.y);
            }
        }

        private void DetectViewportResize(bool setFeature = false)
        {
            if (Screen.width != viewportResolution.x || Screen.height != viewportResolution.y)
            {
                viewportResolution.x = Screen.width;
                viewportResolution.y = Screen.height;

                Calculate();
                SetFeature();
            }
        }
        #endregion
    }
}