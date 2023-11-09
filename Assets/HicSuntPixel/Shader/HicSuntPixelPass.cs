using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class HicSuntPixelPass : ScriptableRenderPass
{
    private HicSuntPixelFeature.HicSuntPixelPassSettings _settings;

    private RenderTargetIdentifier colorBuffer;
    private RenderTargetIdentifier pixelBuffer;
    private int pixelBufferID = Shader.PropertyToID("_PixelBuffer");

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
            if (renderingData.cameraData.camera.name == "Main Camera")
            {
                cmd.Blit(colorBuffer, pixelBuffer);
            }
            else if (renderingData.cameraData.camera.name == "Viewport Camera")
            {
                Debug.Log("Scale-> " + _settings.scale + " | Margin-> " + _settings.margin);
                cmd.Blit(pixelBuffer, colorBuffer, _settings.scale, _settings.margin);
            }
        }

        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        colorBuffer = renderingData.cameraData.renderer.cameraColorTargetHandle;
        RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor;

        descriptor.width = _settings.cameraResolution.x;
        descriptor.height = _settings.cameraResolution.y;

        cmd.GetTemporaryRT(pixelBufferID, descriptor, FilterMode.Point);

        pixelBuffer = new RenderTargetIdentifier(pixelBufferID);
    }

    public override void OnCameraCleanup(CommandBuffer cmd)
    {
        if (cmd == null) throw new System.ArgumentNullException("cmd");

        cmd.ReleaseTemporaryRT(pixelBufferID);
    }
}
