using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class HicSuntPixelPass : ScriptableRenderPass
{
    private HicSuntPixelFeature.HicSuntPixelPassSettings _settings;

    private RenderTargetIdentifier colorBuffer;

    public HicSuntPixelPass(HicSuntPixelFeature.HicSuntPixelPassSettings settings)
    {
        this._settings = settings;
        this.renderPassEvent = settings.renderPassEvent;
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        CommandBuffer cmd = CommandBufferPool.Get();

        using(new ProfilingScope(cmd, new ProfilingSampler("Hic Sunt Pixel Pass")))
        {
            if (renderingData.cameraData.camera.name == _settings.viewportCameraName)
            {
                cmd.Blit(_settings.GetRenderTexture(), colorBuffer, _settings.scale, _settings.margin);
            }
        }

        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        if (renderingData.cameraData.camera.name == _settings.mainCameraName)
        {
            renderingData.cameraData.camera.targetTexture = _settings.GetRenderTexture();
        }
        else if (renderingData.cameraData.camera.name == _settings.viewportCameraName)
        {
            colorBuffer = renderingData.cameraData.renderer.cameraColorTargetHandle;
        }
    }

    public override void OnCameraCleanup(CommandBuffer cmd)
    {
        
    }
}
