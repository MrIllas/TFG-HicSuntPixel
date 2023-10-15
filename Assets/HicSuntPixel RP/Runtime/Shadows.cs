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

    struct ShadowedDirectionalLight
    {
        public int visibleLightIndex;
    }
    const int maxShadowDirectionalLightCount = 4;
    int shadowedDirectionalLightCount; //Keeps track of how many directional shadows exist

    static Matrix4x4[] dirShadowMatrices = new Matrix4x4[maxShadowDirectionalLightCount];

    ShadowedDirectionalLight[] ShadowedDirectionalLights = new ShadowedDirectionalLight[maxShadowDirectionalLightCount];

    public Vector2 ReserveDirectionalShadows (Light light, int visibleLightIndex)
    {
        if (shadowedDirectionalLightCount < maxShadowDirectionalLightCount && light.shadows != LightShadows.None  && light.shadowStrength > 0.0f && cullingResults.GetShadowCasterBounds(visibleLightIndex, out Bounds b))
        {
            ShadowedDirectionalLights[shadowedDirectionalLightCount] = new ShadowedDirectionalLight { visibleLightIndex = visibleLightIndex};

            return new Vector2(light.shadowStrength, shadowedDirectionalLightCount++);
        }
        return Vector2.zero;
    }

    void RenderDirectionalShadows ()
    {
        int atlasSize = (int) settings.directional.atlasSize;
        buffer.GetTemporaryRT(dirShadowAtlasId, atlasSize, atlasSize, 32, FilterMode.Bilinear, RenderTextureFormat.Shadowmap);

        buffer.SetRenderTarget(dirShadowAtlasId, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store);
        buffer.ClearRenderTarget(true, false, Color.clear);
        buffer.BeginSample(bufferName);
        ExecuteBuffer();

        int split = shadowedDirectionalLightCount <= 1 ? 1 : 2;
        int tileSize = atlasSize / split;

        for (int i = 0; i < shadowedDirectionalLightCount; ++i)
        {
            RenderDirectionalShadows(i, split, tileSize);
        }

        buffer.SetGlobalMatrixArray(dirShadowMatricesId, dirShadowMatrices);
        buffer.EndSample(bufferName);
        ExecuteBuffer();
    }

    void RenderDirectionalShadows (int i, int split, int tileSize)
    {
        ShadowedDirectionalLight light = ShadowedDirectionalLights[i];
        var shadowSettings = new ShadowDrawingSettings(cullingResults, light.visibleLightIndex, BatchCullingProjectionType.Orthographic);
        
        Matrix4x4 viewMatrix;
        Matrix4x4 projectionMatrix;
        ShadowSplitData splitData;

        cullingResults.ComputeDirectionalShadowMatricesAndCullingPrimitives
        (
            light.visibleLightIndex, 0, 1, Vector3.zero, tileSize, 0.0f,
            out viewMatrix, out projectionMatrix, out splitData
        );

        shadowSettings.splitData = splitData;
        dirShadowMatrices[i] = ConvertToAtlasMatrix(projectionMatrix * viewMatrix, SetTileViewport(i, split, tileSize), split);
        buffer.SetViewProjectionMatrices(viewMatrix, projectionMatrix);
        ExecuteBuffer();
        context.DrawShadows(ref shadowSettings);
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
