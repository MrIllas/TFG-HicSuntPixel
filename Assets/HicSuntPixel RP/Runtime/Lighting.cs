using UnityEngine;
using UnityEngine.Rendering;
using Unity.Collections;

public class Lighting
{
    const string bufferName = "Lighting";

    //Commands to gpu
    CommandBuffer buffer = new CommandBuffer {name = bufferName};

    CullingResults cullingResults;

#region Directional Light Data
    const int maxDirLightCount = 4;

    //Scene directional light id
    static int dirLightCountId = Shader.PropertyToID("_DirectionalLightCount");
    static int dirLightColorsId = Shader.PropertyToID("_DirectionalLightColors");
    static int dirLightDirectionsId = Shader.PropertyToID("_DirectionalLightDirections");
    static int dirLightShadowDataId = Shader.PropertyToID("_DirectionalLightShadowData");

    static Vector4[] dirLightColors = new Vector4[maxDirLightCount];
    static Vector4[] dirLightDirections = new Vector4[maxDirLightCount];
    static Vector4[] dirLightShadowData = new Vector4[maxDirLightCount];
#endregion Directional Light Data

    Shadows shadows = new Shadows();

    public void Setup (ScriptableRenderContext context, CullingResults cullingResults, ShadowSettings shadowSettings)
    {
        this.cullingResults = cullingResults;
        buffer.BeginSample(bufferName);

        shadows.Setup(context, cullingResults, shadowSettings);
        SetupLights();
        shadows.Render();
        
        buffer.EndSample(bufferName);
        context.ExecuteCommandBuffer(buffer);

        buffer.Clear();
    }

    public void Cleanup ()
    {
        shadows.Cleanup();
    }


    void SetupLights()
    {
        NativeArray<VisibleLight> visibleLights = cullingResults.visibleLights;
        
        int dirLightCount = 0;
        for (int i = 0; i < visibleLights.Length; ++i)
        {
            if (visibleLights[i].lightType == LightType.Directional)
            {
                VisibleLight vs = visibleLights[i];
                SetupDirectionalLight(dirLightCount++, ref vs);
            
                if (dirLightCount >= maxDirLightCount) break; //Breaks when reached max light supported
            }
        }

        buffer.SetGlobalInt(dirLightCountId, dirLightCount);
        buffer.SetGlobalVectorArray(dirLightColorsId, dirLightColors);
        buffer.SetGlobalVectorArray(dirLightDirectionsId, dirLightDirections);
        buffer.SetGlobalVectorArray(dirLightShadowDataId, dirLightShadowData);
    }

    //Access scene's main light (directional light) amd sends its data to the GPU
    void SetupDirectionalLight(int i, ref VisibleLight visibleLight)
    {
        //Light light = RenderSettings.sun;
        //buffer.SetGlobalVector(dirLightColorId, light.color.linear * light.intensity); //light = color * intensity
        //buffer.SetGlobalVector(dirLightDirectionId, -light.transform.forward);
        dirLightColors[i] = visibleLight.finalColor;
        dirLightDirections[i] = -visibleLight.localToWorldMatrix.GetColumn(2);
        dirLightShadowData[i] = shadows.ReserveDirectionalShadows(visibleLight.light, i);
    }
}
