using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;

////////////////////// REFERENCES ///////////////////////////////
// https://www.cyanilux.com/tutorials/custom-renderer-features //
/// /////////////////////////////////////////////////////////////

//
// A feature that turns on the depth, normals and color textures regardless of the pipeline settings.
//
public class DepthNormalsFeature : ScriptableRendererFeature
{
    public class DepthNormalsRenderPass : ScriptableRenderPass
    {
        private ProfilingSampler m_ProfilingSampler; // A profiler to record the render pass into the unity's profiler

        public DepthNormalsRenderPass(string name)
        {
            m_ProfilingSampler = new ProfilingSampler(name);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) 
        {
            CommandBuffer cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, m_ProfilingSampler))
            {
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();
            }
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }

        // Nothing to clean
        public override void OnCameraCleanup(CommandBuffer cmd) { }
    }

    private DepthNormalsRenderPass m_ScriptablePass;

    public override void Create()
    {
        m_ScriptablePass = new DepthNormalsRenderPass(name);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        m_ScriptablePass.ConfigureInput(ScriptableRenderPassInput.Depth | ScriptableRenderPassInput.Normal | ScriptableRenderPassInput.Color);
        renderer.EnqueuePass(m_ScriptablePass);
    }
}