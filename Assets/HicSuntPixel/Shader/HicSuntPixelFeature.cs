using UnityEngine;
using UnityEngine.Rendering.Universal;

public class HicSuntPixelFeature : ScriptableRendererFeature
{

    [System.Serializable]
    public class HicSuntPixelPassSettings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        public Vector2Int cameraResolution = new Vector2Int(644, 364);
        public Vector2Int screenResolution = new Vector2Int(1920, 1080);
        public Vector2 margin = new Vector2(0,0);
        public Vector2 scale = new Vector2Int(1,1);
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
