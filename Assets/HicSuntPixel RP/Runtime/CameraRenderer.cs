using System;
using UnityEngine;
using UnityEngine.Rendering;

public partial class CameraRenderer
{
    const string commandBufferName = "Render Camera";
    CommandBuffer commandBuffer = new CommandBuffer() { name = commandBufferName } ; // Buffer for geometry

    ScriptableRenderContext context;

    CullingResults cullingResults;
    Camera camera;

    static ShaderTagId unlitShaderTagId = new ShaderTagId("SRPDefaultUnlit");
    static ShaderTagId litShaderTagId = new ShaderTagId("CustomLit");

    Lighting lighting = new Lighting();

    public void Render (ScriptableRenderContext context, Camera camera, bool useDynamicBatching, bool useGPUInstancing, ShadowSettings shadowSettings)
    {
        this.context = context;
        this.camera = camera;

        PrepareBuffer(); //Gives a name to the Buffer for debug purpose (Frame Debugger)
        PrepareForSceneWindow(); //Draw UI on scene window
        if (!Cull(shadowSettings.maxDistance)) return; //Return if geometry fails to cull

        commandBuffer.BeginSample(SampleName);
        ExecuteBuffer();
        lighting.Setup(context, cullingResults, shadowSettings);
        commandBuffer.EndSample(SampleName);
        Setup();
        DrawVisibleGeometry(useDynamicBatching, useGPUInstancing);
        DrawUnsupportedShaders();
        DrawGizmos();

        lighting.Cleanup();
        Submit();
    }

    //Submits buffers to execution
    void Submit()
    {
        commandBuffer.EndSample(SampleName);
        ExecuteBuffer();
        context.Submit();
    }

    //Setups buffers
    void Setup()
    {
        context.SetupCameraProperties(this.camera);
        CameraClearFlags flags = camera.clearFlags;
        commandBuffer.ClearRenderTarget(flags <= CameraClearFlags.Depth, flags <= CameraClearFlags.Color, flags == CameraClearFlags.Color ? camera.backgroundColor.linear : Color.clear);

        commandBuffer.BeginSample(SampleName);
        ExecuteBuffer(); 
    }

    void DrawVisibleGeometry(bool useDynamicBatching, bool useGPUInstancing)
    {
        //Draw Opaques
        SortingSettings sortingSettings = new SortingSettings(this.camera)
        {
            criteria = SortingCriteria.CommonOpaque // Forces a front to back render of opaques
        };
        DrawingSettings drawingSettings = new DrawingSettings(unlitShaderTagId, sortingSettings)
        {
            enableDynamicBatching = useDynamicBatching,
            enableInstancing = useGPUInstancing
        };
        drawingSettings.SetShaderPassName(1, litShaderTagId);

        FilteringSettings filteringSettings = new FilteringSettings(RenderQueueRange.opaque); // Only tells the renderer to render opaques

        context.DrawRenderers(cullingResults, ref drawingSettings, ref filteringSettings);

        //Draw Skybox
        context.DrawSkybox(this.camera);

        //Draw Transparent
        sortingSettings.criteria = SortingCriteria.CommonTransparent; // Forces a back to froont render of transparents
        drawingSettings.sortingSettings = sortingSettings;
        filteringSettings.renderQueueRange = RenderQueueRange.transparent; // Only tells the renderer to render transparents

        context.DrawRenderers(cullingResults, ref drawingSettings, ref filteringSettings);
    }

    void ExecuteBuffer() 
    {
        context.ExecuteCommandBuffer(commandBuffer);
        commandBuffer.Clear();
    }

    /*
        Return false if culling fails
        Unity not only culls the geometry inside the fustrum, but also culls the light 
        that affects that geometry.
    */
    bool Cull (float maxShadowDistance)
    {
        if (this.camera.TryGetCullingParameters(out ScriptableCullingParameters p)) 
        {
            p.shadowDistance = Mathf.Min(maxShadowDistance, camera.farClipPlane);
            cullingResults = context.Cull(ref p);
            return true;
        }
        return false;
    }

}
