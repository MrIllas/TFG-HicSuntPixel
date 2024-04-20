using UnityEngine;
using UnityEngine.Rendering.Universal;

public class HicSuntPixelFeature : ScriptableRendererFeature
{

    [System.Serializable]
    public class HicSuntPixelPassSettings
    {
        public string mainCameraName = "Main Camera";
        public string viewportCameraName = "Viewport Camera";

        public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;

        public Vector2Int realResolution; // The real game resolution, the resolution to which the game is being rendered to the main camera frame buffer
        public Vector2Int viewportResolution; // The final resolution that the player recieves

        public Vector2 margin = new Vector2(0,0);
        public Vector2 scale = new Vector2Int(1,1);

        private RenderTexture renderTexture;

        public RenderTexture GetRenderTexture()
        {
            // Creates a render texture if there is none or Generates a new render texture with the new dimensions
            if (renderTexture == null || renderTexture.width != realResolution.x || renderTexture.height != realResolution.y)
            {
                CreateRenderTexture();
            }

            return renderTexture;
        }

        private void CreateRenderTexture()
        {
            if (renderTexture != null) renderTexture.Release();
            renderTexture = new RenderTexture(realResolution.x, realResolution.y, 32, RenderTextureFormat.ARGB32);
            renderTexture.filterMode = FilterMode.Point;
            renderTexture.Create();
        }
    }

    [SerializeField] public HicSuntPixelPassSettings _settings;
    private HicSuntPixelPass _hspPass;

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
#if UNITY_EDITOR
        if (renderingData.cameraData.isSceneViewCamera) return;
#endif
        renderer.EnqueuePass(_hspPass);
    }

    public override void Create()
    {
        _hspPass = new HicSuntPixelPass(_settings);
    }
}
