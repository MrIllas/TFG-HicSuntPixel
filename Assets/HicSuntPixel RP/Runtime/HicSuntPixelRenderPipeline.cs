using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

/*
The Pipeline
*/


public class HicSuntPixelRenderPipeline : RenderPipeline 
{

    CameraRenderer renderer = new CameraRenderer();

    public HicSuntPixelRenderPipeline()
    {
        GraphicsSettings.useScriptableRenderPipelineBatching = true;
    }

    //Declared abstract, must exist but won't be used because the camera array requires allocating memory every frame.
    protected override void Render(ScriptableRenderContext context, Camera[] cameras)
    { 
    
    }

    //The alternative
    protected override void Render(ScriptableRenderContext context, List<Camera> cameras)
    {
        for (int i = 0; i < cameras.Count; ++i)
        {
            renderer.Render(context, cameras[i]);
        }
    }
}