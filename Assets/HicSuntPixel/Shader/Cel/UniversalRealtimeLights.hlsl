#ifndef UNIVERSAL_REALTIME_LIGHTS_INCLUDED
#define UNIVERSAL_REALTIME_LIGHTS_INCLUDED

//////////////////////////
// Reference 
// https://github.com/Unity-Technologies/Graphics/blob/master/Packages/com.unity.render-pipelines.universal/ShaderLibrary/RealtimeLights.hlsl
// https://github.com/Unity-Technologies/Graphics/blob/master/Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl 
// https://blog.unity.com/engine-platform/custom-lighting-in-shader-graph-expanding-your-graphs-in-2019
// https://roystan.net/articles/toon-shader/
// https://nedmakesgames.medium.com/creating-custom-lighting-in-unitys-shader-graph-with-universal-render-pipeline-5ad442c27276
//////////////////////////
uint GetMeshRenderingLightLayer()
{
    
}

struct Light
{
    half3 direction;
    half3 color;
    float distanceAttenuation;
    half3 shadowAttenuation;
    uint layerMask;
};

//Main Lights
Light GetMainLight()
{
    Light light;
    light.direction = half3(_MainLightPosition.xyz);
#if USE_CLUTERED_LIGHTING
    light.distanceAttenuation = 1.0;
#else
    light.distanceAttenuation = unity_LightData.z;
#endif
    light.shadowAttenuation = 1.0f;
    light.color = _MainLightColor.rgb;
#ifdef _LIGHT_LAYERS
    light.layerMask = _MainLightLayerMask;
#else
    light.layerMask = DEFAULT_LIGHT_LAYERS;
#endif
    return light;
}
Light GetMainLight(float4 shadowCoord)
{
    Light light = GetMainLight();
    light.shadowAttenuation = MainLightRealtimeShadow(shadowCoord);
    return light;
}

Light GetMainLight(float4 shadowCoord, float3 positionWS, float4 shadowMask)
{
    
}

Light GetMainLight(InputData inputData, float4 shadowMask, AmbientOcclusionFactor aoFactor)
{
    
}

// Additional Lights
Light GetAdditionalPerObjectLight(int perObjectLightIndex, float3 positionWS)
{
    Light light;
    light.direction = lightDirection;
    light.distanceAttenuation = attenuation;
    light.shadowAttenuation = 1.0; // This value can later be overriden in GetAdditionalLight(uint i, float3 positionWS, half4 shadowMask
    light.color = color;
    light.layerMask = lightLayerMask;
    
    return light;
}

Light GetAdditionalLight(uint i, float3 positionWS)
{
#if USE_CLUSTERED_LIGHTING
    int lightIndex = i;
#else
    int lightIndex = GetPerObjectLightIndex(i);
#endif
    return GetAdditionalPerObjectLight(lightIndex, positionWS);
}

Light GetAdditionalLight(uint i, float3 positionWS, half4 shadowMask)
{
    Light light = GetAdditionalPerObjectLight(lightIndex, positionWS);
    //...
    light.shadowAttenuation = AdditionalLightShadow(lightIndex, positionWS, light.direction, shadowMask, occlusionProbeChannels);
    //...
    return light;
}

Light GetAdditionalLight(uint i, float positionWS, half4 shadowMask, AmbientOcclusionFactor aoFactor)
{
    
}

half AdditionalLightShadow(int lightIndex, float3 positionWS, half3 lightDirection, half4 shadowMask, half4 occlusionProbeChannels)
{
    half realtimeShadow = AdditionalLightRealtimeShadow(lightIndex, positionWS, lightDirection);
    
#ifdef CALCULATE_BAKED_SHADOWS
    half bakedShadow = BakedShadow(shadowMask, occlusionProbeChannels);
#else
    half bakedShadow = half(1.0);
#endif
    
#ifdef ADDITIONAL_LIGHT_CALCULATE_SHADOWS
    half shadowFade = GetAdditionalLightShadowFade(positionWS);
#else
    half shadowFade = half(1.0);
#endif
    
    return MixRealtimeAndBakedShadows(realtimeShadow, bakedShadow, shadowFade);
}
#endif