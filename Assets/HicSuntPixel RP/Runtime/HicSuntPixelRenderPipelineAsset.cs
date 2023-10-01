using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

/*
 HANDLE & PLACE to store settings  
 */

[CreateAssetMenu(menuName = "Rendering/Hic Sunt Pixel Render Pipeline")]
public class HicSuntPixelRenderPipelineAsset : RenderPipelineAsset 
{

    //Gives unity the Render Pipeline
    protected override RenderPipeline CreatePipeline()
    {
        return new HicSuntPixelRenderPipeline();
    }
}
