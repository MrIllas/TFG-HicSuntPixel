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


#define SHADOW_CLAMP_VALUE 0.15f
#define LIGHT_CLAMP_VALUE 1.00f

struct EdgeConstants
{
    float Diffuse;
    float Specular;
    float SpecularOffset;
    float DistanceAttenuation;
    float ShadowAttenuation;
    float Rim;
    float RimOffset;
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
    // Attenuation
    float shadowAttenuationSmoothstepped = smoothstep(0.0f, surface.ec.ShadowAttenuation, light.shadowAttenuation);
    float distanceAttenuationSmoothstepped = smoothstep(0.0f, surface.ec.DistanceAttenuation, light.distanceAttenuation);
    
    float attenuation = shadowAttenuationSmoothstepped * distanceAttenuationSmoothstepped;
    
    // Diffuse
    float diffuse = saturate(dot(surface.normal, light.direction));
    diffuse *= attenuation;

    // Apply stepping to diffuse
    float steps = 3.0; // Number of steps
    diffuse = round(diffuse * steps) / steps;

    // Specular
    float3 h = normalize(light.direction + surface.view);
    float specular = saturate(dot(surface.normal, h));
    specular = pow(specular, surface.shininess);
    specular *= surface.smoothness;

    // Apply stepping to specular
    specular = round(specular * steps) / steps;

    // Rim
    float rim = 1 - dot(surface.view, surface.normal);
    rim *= pow(diffuse, surface.rimThreshold);

    // Apply stepping to rim
    rim = round(rim * steps) / steps;

    return light.color * (diffuse + specular);
}

void LightingCelShaded_float(float Smoothness, float RimThreshold, float3 Position, float3 Normal, float3 View,
                             float EdgeDiffuse, float EdgeSpecular, float EdgeSpecularOffset, float EdgeDistanceAttenuation, float EdgeShadowAttenuation, float EdgeRim, float EdgeRimOffset,
                             float3 CloudsCookies, bool Clamp,
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
    
    surface.ec.Diffuse = 1.0f; //EdgeDiffuse;
    surface.ec.Specular = EdgeSpecular;
    surface.ec.SpecularOffset = EdgeSpecularOffset;
    surface.ec.DistanceAttenuation = 0.03f; //EdgeDistanceAttenuation;
    surface.ec.ShadowAttenuation = EdgeShadowAttenuation;
    surface.ec.Rim = EdgeRim;
    surface.ec.RimOffset = EdgeRimOffset;

    // Calculate Shadow Coord
#if SHADOWS_SCREEN
    float4 clipPos = TransformWorldToHClip(Position);
    float4 shadowCoord = ComputeScreenPos(clipPos);
#else
    float4 shadowCoord = TransformWorldToShadowCoord(Position);
#endif
    
    //Color = CloudsCookies;
    
    Light mainLight = GetMainLight(shadowCoord, Position, shadowCoord); //shadowCoord since I need to give a float4 but I don't use it
    //Color *= CalculateCelShading(mainLight, surface);
    Color = CalculateCelShading(mainLight, surface);
    
    int pixelLightCount = GetAdditionalLightsCount();
    for (int i = 0; i < pixelLightCount; ++i)
    {
        Light light = GetAdditionalLight(i, Position, 1);
        Color += CalculateCelShading(light, surface);
    }
#endif
    
    if (Clamp)
    {
        Color = clamp(Color, 0.15, 1.0); // Clamp color for lighter shadows
    }
    else
    {
        Color = Color;
    }

}

void SimpleLightingCelShaded_float(float Smoothness, float3 Position, float3 Normal, float3 View, 
                                    float3 CloudsCookies, bool Clamp,
                                    out float3 Color)
{ 
    LightingCelShaded_float(Smoothness, 0.0f, Position, Normal, View, 0.001f, 0.0f, 0.0f, 0.75f, 1.0f, 0.0f, 0.0f, CloudsCookies, Clamp, Color);
}
/////


float3 CalculateCelShadingNonStepped(Light light, SurfaceVariables surface)
{
    // Attenuation
    float shadowAttenuationSmoothstepped = smoothstep(0.0f, surface.ec.ShadowAttenuation, light.shadowAttenuation);
    float distanceAttenuationSmoothstepped = smoothstep(0.0f, surface.ec.DistanceAttenuation, light.distanceAttenuation);
    
    float attenuation = shadowAttenuationSmoothstepped * distanceAttenuationSmoothstepped;
    
    //Diffuse
    float3 diffuse = saturate(dot(surface.normal, light.direction));
    diffuse *= attenuation;
    
    //Specular
    float3 h = SafeNormalize(light.direction + surface.view);
    float specular = saturate(dot(surface.normal, h));
    specular = pow(specular, surface.shininess);
    specular *= diffuse * surface.smoothness;

    // Rim
    float rim = 1 - dot(surface.view, surface.normal);
    rim *= pow(diffuse, surface.rimThreshold);
    
    diffuse = smoothstep(0.0f, surface.ec.Diffuse, diffuse);
    specular = surface.smoothness * smoothstep((1 - surface.smoothness) * surface.ec.Specular + surface.ec.Specular, surface.ec.Specular + surface.ec.SpecularOffset, specular);
    rim = surface.smoothness * smoothstep(surface.ec.Rim - 0.5f * surface.ec.RimOffset, surface.ec.Rim + 0.5f * surface.ec.RimOffset, rim);
    return light.color * (diffuse + max(specular, rim));
}


void LightingCelShadedNonStepped_float(float Smoothness, float RimThreshold, float3 Position, float3 Normal, float3 View,
                             float EdgeDiffuse, float EdgeSpecular, float EdgeSpecularOffset, float EdgeDistanceAttenuation, float EdgeShadowAttenuation, float EdgeRim, float EdgeRimOffset,
                             float3 CloudsCookies, bool Clamp,
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
    
    surface.ec.Diffuse = EdgeDiffuse;
    surface.ec.Specular = EdgeSpecular;
    surface.ec.SpecularOffset = EdgeSpecularOffset;
    surface.ec.DistanceAttenuation = EdgeDistanceAttenuation;
    surface.ec.ShadowAttenuation = EdgeShadowAttenuation;
    surface.ec.Rim = EdgeRim;
    surface.ec.RimOffset = EdgeRimOffset;

    // Calculate Shadow Coord
#if SHADOWS_SCREEN
    float4 clipPos = TransformWorldToHClip(Position);
    float4 shadowCoord = ComputeScreenPos(clipPos);
#else
    float4 shadowCoord = TransformWorldToShadowCoord(Position);
#endif
    
    //Color = CloudsCookies;
    
    Light mainLight = GetMainLight(shadowCoord, Position, shadowCoord); //shadowCoord since I need to give a float4 but I don't use it
    //Color *= CalculateCelShading(mainLight, surface);
    Color = CalculateCelShadingNonStepped(mainLight, surface);
    
    int pixelLightCount = GetAdditionalLightsCount();
    for (int i = 0; i < pixelLightCount; ++i)
    {
        Light light = GetAdditionalLight(i, Position, 1);
        Color += CalculateCelShadingNonStepped(light, surface);
    }
#endif
    
    if (Clamp)
    {
        Color = clamp(Color, 0.15, 1.0); // Clamp color for lighter shadows
    }
    else
    {
        Color = Color;
    }

}

void GodRaysLighting_float(float Smoothness, float3 Position, float3 Normal, float3 View,
                                    float3 CloudsCookies, bool Clamp,
                                    out float3 Color)
{
    LightingCelShadedNonStepped_float(Smoothness, 0.0f, Position, Normal, View, 0.001f, 0.0f, 0.0f, 0.75f, 1.0f, 0.0f, 0.0f, CloudsCookies, Clamp, Color);
}

#endif
#endif