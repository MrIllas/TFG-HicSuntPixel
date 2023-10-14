#ifndef CUSTOM_LIGHT_INCLUDED
#define CUSTOM_LIGHT_INCLUDED

/*
    THIS HLSL FILE IS USED FOR THE INTERACTING LIGHT DATA0.
*/

//Stores the directional light of the scene
#define MAX_DIRECTIONAL_LIGHT_COUNT 4
CBUFFER_START(_CustomLight)
    int _DirectionalLightCount;
    float4 _DirectionalLightColors[MAX_DIRECTIONAL_LIGHT_COUNT];
    float4 _DirectionalLightDirections[MAX_DIRECTIONAL_LIGHT_COUNT];
CBUFFER_END

struct Light
{
    float3 color;
    float3 direction;
};

int GetDirectionalLightCount()
{
    return _DirectionalLightCount;
}

Light GetDirectionalLight(int i)
{
    Light light;
    light.color = _DirectionalLightColors[i].rgb;
    light.direction = _DirectionalLightDirections[i].xyz;
    return light;
}

#endif