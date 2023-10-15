using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

/*
 HANDLE & PLACE to store settings  
 */

[CreateAssetMenu(menuName = "Rendering/Hic Sunt Pixel Render Pipeline")]
public class HicSuntPixelRenderPipelineAsset : RenderPipelineAsset 
{

    [SerializeField] ShadowSettings shadows = default;

    [SerializeField] private bool useDynamicBatching = true;
    [SerializeField] private bool useGPUInstancing = true;
    [SerializeField] private bool useSRPBatcher = true;

    //Gives unity the Render Pipeline
    protected override RenderPipeline CreatePipeline()
    {
        return new HicSuntPixelRenderPipeline(this.useDynamicBatching, this.useGPUInstancing, this.useSRPBatcher, shadows);
    }
}
