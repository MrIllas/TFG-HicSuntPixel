#ifndef OUTLINE_INCLUDED
#define OUTLINE_INCLUDED

TEXTURE2D(_CameraColorTexture);
SAMPLER(sampler_CameraColorTexture);
float4 _CameraColorTexture_TexelSize;

TEXTURE2D(_CameraDepthTexture);
SAMPLER(sampler_CameraDepthTexture);

TEXTURE2D(_CameraNormalsTexture); 
SAMPLER(sampler_CameraNormalsTexture);

float GetDepth(float2 uv)
{
    return SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, uv).r;
}

float3 DecodeNormal(float4 enc)
{
    float kScale = 1.7777;
    float3 nn = enc.xyz * float3(2 * kScale, 2 * kScale, 0) + float3(-kScale, -kScale, 1);
    float g = 2.0 / dot(nn.xyz, nn.xyz);
    float3 n;
    n.xy = g * nn.xy;
    n.z = g - 1;
    return n;
}

float3 GetNormal(float2 uv)
{
    return DecodeNormal(SAMPLE_TEXTURE2D(_CameraNormalsTexture, sampler_CameraNormalsTexture, uv));
}

float OutlinesDepth(float2 uvs[4], float depth, float DepthThreshold, out float depthDifference)
{
    float depths[4];
    
    depthDifference = 0.0f;
    
    [unroll]
    for (int i = 0; i < 4; ++i)
    {
        depths[i] = GetDepth(uvs[i]);
        depthDifference += depth - depths[i]; // depths[i] - depth;
    }
    
    float depthEdge = step(DepthThreshold, depthDifference);
    
    return depthEdge;
}

float OutlinesNormals(float2 uvs[4], float3 normal, float3 NormalEdgeBias, float NormalsThreshold)
{
    //Get Normal Edge Indicator
    float3 normals[4];
    float dotSum = 0.0f;
    
    [unroll]
    for (int j = 0; j < 4; ++j)
    {
        normals[j] = GetNormal(uvs[j]);
        float3 normalDiff = normal - normals[j];
        
        //Edge pixels should yield to faces closer to the bias diretion
        float normalBiasDiff = dot(normalDiff, NormalEdgeBias);
        float normalIndicator = smoothstep(-0.01f, 0.01f, normalBiasDiff); //step(0, normalBiasDiff);
        
        /*
        //Only the shallower pixel should detect the normal edge
        float depthDiff = depths[ii] - depth;
        float depthIndicator = step(0, depthDiff * 0.25f, 0.0025f);
        */
        
        dotSum += dot(normalDiff, normalDiff) * normalIndicator; // * deothIndicator;
    }
    float indicator = sqrt(dotSum);
    float normalEdge = step(NormalsThreshold, indicator);
    
    return (normalEdge);
}


void Outlines_float(float2 UV, float Thickness, float DepthThreshold, float NormalsThreshold, float3 NormalEdgeBias, float DepthEdgeStrength, float NormalEdgeStrength,
                    out float Out, out float OutDepth, out float OutNormals)
{
    float2 texelSize = _CameraColorTexture_TexelSize.xy;
    
    float depth = GetDepth(UV);
    float3 normal = GetNormal(UV);
    
    float2 uvs[4]; // = { float2(0.0f, 0.0f), float2(0.0f, 0.0f), float2(0.0f, 0.0f), float2(0.0f, 0.0f) };
    
    uvs[0] = (UV + float2(0.0f, texelSize.y) * Thickness);
    uvs[1] = (UV - float2(0.0f, texelSize.y) * Thickness);
    uvs[2] = (UV + float2(texelSize.x, 0.0f) * Thickness);
    uvs[3] = (UV - float2(texelSize.x, 0.0f) * Thickness);
    float depthDifference = 0.0f;
    
    float depthEdge = OutlinesDepth(uvs, depth, DepthThreshold, depthDifference);
    float normalEdge = OutlinesNormals(uvs, normal, NormalEdgeBias, NormalsThreshold);
    
    // Refuse normal outline if the depthEdge is negative, make it a depth edge if it is above the threshold
    OutDepth = DepthEdgeStrength * depthEdge; //1 - step(DepthEdgeStrength * depthEdge, 0);
    OutNormals = NormalEdgeStrength * normalEdge; //1 - step(NormalEdgeStrength * normalEdge, 0);
    Out = depthDifference < 0 ? 0 : (depthEdge > 0.0f ? (OutDepth) : (OutNormals)); //DepthEdgeStrength * depthEdge; 
}
#endif