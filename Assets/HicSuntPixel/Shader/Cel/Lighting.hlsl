#ifndef LIGHTING_INCLUDED
#define LIGHTING_INCLUDED

#include "UniversalRealtimeLights.hlsl"

#pragma multi_compile _MAIN_LIGHT_SHADOWS
#pragma multi_compile _MAIN_LIGHT_SHADOWS_CASCADE
#pragma multi_compile _SHADOWS_SOFT
#pragma multi_compile _ADDITIONAL_LIGHTS
#pragma multi_compile _ADDITIONAL_LIGHT_SHADOWS
#pragma multi_compile _LIGHT_COOKIES


#ifndef SHADERGRAPH_PREVIEW

struct EdgeConstants
{
    float edgeSpecular;
    float edgeRim;
};

struct SurfaceVariables
{
    float smoothness;
    float shininess;
    float rimThreshold;
    float3 normal;
    float3 view;
    EdgeConstants ec;
    
};
float3 CalculateCelShading(Light light, SurfaceVariables surface)
{
    //Attenuation
    float shadowAttenuationSmoothstepped = smoothstep(0.0f, 0.001f, light.shadowAttenuation);
    float distanceAttenuationSmoothstepped = smoothstep(0.0f, 0.001f, light.distanceAttenuation);
    
    float attenuation = shadowAttenuationSmoothstepped * distanceAttenuationSmoothstepped;
    
    //Diffuse
    float3 diffuse = saturate(dot(surface.normal, light.direction));
    diffuse *= attenuation; 
    
    //Specular
    float3 h = SafeNormalize(light.direction + surface.view);
    float specular = saturate(dot(surface.normal, h));
    specular = pow(specular, surface.shininess);
    specular *= diffuse * surface.smoothness;
    
    //Rim
    float rim = 1 - dot(surface.view, surface.normal);
    rim *= pow(diffuse, surface.rimThreshold);
    
    
    diffuse = smoothstep(0.0f, 0.001f, diffuse);
    specular = surface.smoothness * smoothstep((1 - surface.smoothness) * surface.ec.edgeSpecular + surface.ec.edgeSpecular, surface.ec.edgeSpecular, specular);
    rim = surface.smoothness * smoothstep(surface.ec.edgeRim, surface.ec.edgeRim, rim);
    return light.color * (diffuse + max(specular, rim));
}
#endif

void LightingCelShadedOutlines_float(float Smoothness, float RimThreshold, float3 Position, float3 Normal, float3 View,
                             float EdgeSpecular, float EdgeRim,
                            out float3 Color)
{
#if defined (SHADERGRAPH_PREVIEW)
    Color = float3(0.5f, 0.5f, 0.5f);
#else
    //Initialize and populate Surface
    SurfaceVariables surface;
    surface.smoothness = Smoothness;
    surface.shininess = exp2(10 * Smoothness + 1);
    surface.rimThreshold = RimThreshold;
    surface.normal = normalize(Normal);
    surface.view = SafeNormalize(View); //normalize(View);
    
    surface.ec.edgeSpecular = EdgeSpecular;
    surface.ec.edgeRim = EdgeRim;

    // Calculate Shadow Coord
#if SHADOWS_SCREEN
    float4 clipPos = TransformWorldToHClip(Position);
    float4 shadowCoord = ComputeScreenPos(clipPos);
#else
    float4 shadowCoord = TransformWorldToShadowCoord(Position);
#endif
    
    Light mainLight = GetMainLight(shadowCoord, Position, shadowCoord); //shadowCoord since I need to give a float4 but I don't use it
    Color = CalculateCelShading(mainLight, surface);
#endif
    
    Color = clamp(Color, 0.20f, 1.0f); // Clamp color for lighter shadows
}

void LightingCelShaded_float(float Smoothness, float RimThreshold, float3 Position, float3 Normal, float3 View,
                             float EdgeSpecular, float EdgeRim,
                            out float3 Color)
{
#if defined (SHADERGRAPH_PREVIEW)
    Color = float3(0.5f, 0.5f, 0.5f);
#else
    //Initialize and populate Surface
    SurfaceVariables surface;
    surface.smoothness = Smoothness;
    surface.shininess = exp2(10 * Smoothness + 1);
    surface.rimThreshold = RimThreshold;
    surface.normal = normalize(Normal);
    surface.view = SafeNormalize(View); //normalize(View);
    
    surface.ec.edgeSpecular = EdgeSpecular;
    surface.ec.edgeRim = EdgeRim;

    // Calculate Shadow Coord
#if SHADOWS_SCREEN
    float4 clipPos = TransformWorldToHClip(Position);
    float4 shadowCoord = ComputeScreenPos(clipPos);
#else
    float4 shadowCoord = TransformWorldToShadowCoord(Position);
#endif
    
    Light mainLight = GetMainLight(shadowCoord, Position, shadowCoord); //shadowCoord since I need to give a float4 but I don't use it
    Color = CalculateCelShading(mainLight, surface);
    
    int pixelLightCount = GetAdditionalLightsCount();
    for (int i = 0; i < pixelLightCount; ++i)
    {
        Light light = GetAdditionalLight(i, Position, 1);
        Color += CalculateCelShading(light, surface);
    }
#endif
    
    Color = clamp(Color, 0.20f, 1.0f); // Clamp color for lighter shadows
}

void SimpleLightingCelShaded_float(float Smoothness, float3 Position, float3 Normal, float3 View, 
                                    out float3 Color)
{ 
    LightingCelShaded_float(Smoothness, 0.0f, Position, Normal, View, 0.0f, 0.0f, Color);
}

#endif