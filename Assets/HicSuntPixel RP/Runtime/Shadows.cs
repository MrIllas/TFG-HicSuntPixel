using UnityEngine;
using UnityEngine.Rendering;

public class Shadows
{
    const string bufferName = "Shadows";

    CommandBuffer buffer = new CommandBuffer { name = bufferName };

    ScriptableRenderContext context;

    CullingResults cullingResults;

    ShadowSettings settings;

#region Directional Shadows

    static int dirShadowAtlasId = Shader.PropertyToID("_DirectionalShadowAtlas");
    static int dirShadowMatricesId = Shader.PropertyToID("_DirectionalShadowMatrices");
    static int cascadeCountId = Shader.PropertyToID("_CascadeCount");
    static int cascadeCullingSpheresId = Shader.PropertyToID("_CascadeCullingSpheres");
    static int cascadeDataId = Shader.PropertyToID("_CascadeData");
    static int shadowDistanceFadeId = Shader.PropertyToID("_ShadowDistanceFade");

    static Vector4[] cascadeCullingSpheres = new Vector4[maxCascades], 
                     cascadeData = new Vector4[maxCascades];

    struct ShadowedDirectionalLight
    {
        public int visibleLightIndex;
        public float slopeScaleBias;
        public float nearPlaneOffset;
    }
    const int maxShadowDirectionalLightCount = 4, maxCascades = 4;
    int shadowedDirectionalLightCount; //Keeps track of how many directional shadows exist

    static Matrix4x4[] dirShadowMatrices = new Matrix4x4[maxShadowDirectionalLightCount * maxCascades];

    ShadowedDirectionalLight[] ShadowedDirectionalLights = new ShadowedDirectionalLight[maxShadowDirectionalLightCount];

    public Vector3 ReserveDirectionalShadows (Light light, int visibleLightIndex)
    {
        if (shadowedDirectionalLightCount < maxShadowDirectionalLightCount && light.shadows != LightShadows.None  && light.shadowStrength > 0.0f && cullingResults.GetShadowCasterBounds(visibleLightIndex, out Bounds b))
        {
            ShadowedDirectionalLights[shadowedDirectionalLightCount] = new ShadowedDirectionalLight
            {
                visibleLightIndex = visibleLightIndex,
                slopeScaleBias = light.shadowBias,
                nearPlaneOffset = light.shadowNearPlane
            };

            return new Vector3(light.shadowStrength, settings.directional.cascadeCount * shadowedDirectionalLightCount++, light.shadowNormalBias);
        }
        return Vector3.zero;
    }

    void RenderDirectionalShadows ()
    {
        int atlasSize = (int) settings.directional.atlasSize;
        buffer.GetTemporaryRT(dirShadowAtlasId, atlasSize, atlasSize, 32, FilterMode.Bilinear, RenderTextureFormat.Shadowmap);

        buffer.SetRenderTarget(dirShadowAtlasId, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store);
        buffer.ClearRenderTarget(true, false, Color.clear);
        buffer.BeginSample(bufferName);
        ExecuteBuffer();

        int tiles = shadowedDirectionalLightCount * settings.directional.cascadeCount;
        int split = tiles <= 1 ? 1 : tiles <= 4 ? 2 : 4;
        int tileSize = atlasSize / split;

        for (int i = 0; i < shadowedDirectionalLightCount; ++i)
        {
            RenderDirectionalShadows(i, split, tileSize);
        }

        buffer.SetGlobalInt(cascadeCountId, settings.directional.cascadeCount);
        buffer.SetGlobalVectorArray(cascadeCullingSpheresId, cascadeCullingSpheres);
        buffer.SetGlobalMatrixArray(dirShadowMatricesId, dirShadowMatrices);
        float f = 1f - settings.directional.cascadeFade;
        buffer.SetGlobalVector(shadowDistanceFadeId, new Vector4(1.0f / settings.maxDistance, 1.0f / settings.distanceFade, 1.0f / (1.0f - f * f)));
        buffer.SetGlobalVectorArray(cascadeDataId, cascadeData);
        buffer.EndSample(bufferName);
        ExecuteBuffer();
    }

    void RenderDirectionalShadows (int i, int split, int tileSize)
    {
        ShadowedDirectionalLight light = ShadowedDirectionalLights[i];
        var shadowSettings = new ShadowDrawingSettings(cullingResults, light.visibleLightIndex, BatchCullingProjectionType.Orthographic);
        int cascadeCount = settings.directional.cascadeCount;
        int tileOffset = i * cascadeCount;
        Vector3 ratios = settings.directional.CascadeRatios;

        for (int it = 0; it < cascadeCount; ++it) 
        {
            Matrix4x4 viewMatrix;
            Matrix4x4 projectionMatrix;
            ShadowSplitData splitData;

            cullingResults.ComputeDirectionalShadowMatricesAndCullingPrimitives
            (
                light.visibleLightIndex, it, cascadeCount, ratios, tileSize, light.nearPlaneOffset,
                out viewMatrix, out projectionMatrix, out splitData
            );

            shadowSettings.splitData = splitData;
            if (i == 0)
            {
                SetCascadeData(it, splitData.cullingSphere, tileSize);
            }

            int tileIndex = tileOffset + it;
            dirShadowMatrices[tileIndex] = ConvertToAtlasMatrix(projectionMatrix * viewMatrix, SetTileViewport(tileIndex, split, tileSize), split);
            buffer.SetViewProjectionMatrices(viewMatrix, projectionMatrix);
            // buffer.SetGlobalDepthBias(0.0f, 3.0f);
            buffer.SetGlobalDepthBias(0.0f, light.slopeScaleBias);
            ExecuteBuffer();
            context.DrawShadows(ref shadowSettings);
            buffer.SetGlobalDepthBias(0.0f, 0.0f);
        }
    }

    void SetCascadeData (int i, Vector4 cullingSphere, float tileSize)
    {
        float texelSize = 2f * cullingSphere.w / tileSize;
        cullingSphere.w *= cullingSphere.w;
        cascadeCullingSpheres[i] = cullingSphere;
        cascadeData[i] = new Vector4(1.0f / cullingSphere.w, texelSize * 1.4142136f);
    }

#endregion Directional Shadows

    public void Setup (ScriptableRenderContext context, CullingResults cullingResults, ShadowSettings settings)
    {
        this.context = context;
        this.cullingResults = cullingResults;
        this.settings = settings;
        shadowedDirectionalLightCount = 0;
    }

    void ExecuteBuffer()
    {
        context.ExecuteCommandBuffer(buffer);
        buffer.Clear();
    }

    public void Render ()
    {
        if (shadowedDirectionalLightCount > 0)
        {
            RenderDirectionalShadows();
        }
        //else
        //{
        //    buffer.GetTemporaryRT(dirShadowAtlasId, 1, 1, 32, FilterMode.Bilinear, RenderTextureFormat.Shadowmap);
        //}
    }

    public void Cleanup ()
    {
        buffer.ReleaseTemporaryRT(dirShadowAtlasId);
        ExecuteBuffer();
    }

    Vector2 SetTileViewport (int i, int split, float tileSize)
    {
        Vector2 offset = new Vector2(i % split, i / split);
        buffer.SetViewport(new Rect(offset.x * tileSize, offset.y * tileSize, tileSize, tileSize));
    
        return offset;
    }

    Matrix4x4 ConvertToAtlasMatrix (Matrix4x4 m, Vector2 offset, int split)
    {
        if (SystemInfo.usesReversedZBuffer)
        {
            m.m20 = -m.m20;
            m.m21 = -m.m21;
            m.m22 = -m.m22;
            m.m23 = -m.m23;
        }

        float scale = 1.0f / split;
        m.m00 = (0.5f * (m.m00 + m.m30) + offset.x * m.m30) * scale;
        m.m01 = (0.5f * (m.m01 + m.m31) + offset.x * m.m31) * scale;
        m.m02 = (0.5f * (m.m02 + m.m32) + offset.x * m.m32) * scale;
        m.m03 = (0.5f * (m.m03 + m.m33) + offset.x * m.m33) * scale;
        m.m10 = (0.5f * (m.m10 + m.m30) + offset.y * m.m30) * scale;
        m.m11 = (0.5f * (m.m11 + m.m31) + offset.y * m.m31) * scale;
        m.m12 = (0.5f * (m.m12 + m.m32) + offset.y * m.m32) * scale;
        m.m13 = (0.5f * (m.m13 + m.m33) + offset.y * m.m33) * scale;
        m.m20 = 0.5f * (m.m20 + m.m30);
        m.m21 = 0.5f * (m.m21 + m.m31);
        m.m22 = 0.5f * (m.m22 + m.m32);
        m.m23 = 0.5f * (m.m23 + m.m33);

        return m;
    }

}
