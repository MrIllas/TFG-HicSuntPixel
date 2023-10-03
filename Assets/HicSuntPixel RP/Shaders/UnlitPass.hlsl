#ifndef CUSTOM_UNLIT_PASS_INCLUDED
#define CUSTOM_UNLIT_PASS_INCLUDED

#include "../ShaderLibrary/Common.hlsl"

//CBUFFER_START (UnityPerMaterial)
//    float4 _BaseColor;
//CBUFFER_END

UNITY_INSTANCING_BUFFER_START(UnityPerMaterial)
    UNITY_DEFINE_INSTANCED_PROP(float4, _BaseColor)
UNITY_INSTANCING_BUFFER_END(UnityPerMaterial)

struct Attributes
{
    UNITY_VERTEX_INPUT_INSTANCE_ID

    float3 positionOS : POSITION;  
};

struct Varyings
{
    UNITY_VERTEX_INPUT_INSTANCE_ID

    float4  positionCS : SV_POSITION;
};

Varyings UnlitPassVertex(Attributes input)
{
    Varyings output;
    UNITY_TRANSFER_INSTANCE_ID(input, output);
    
    float3 positionWS = TransformObjectToWorld(input.positionOS);
    output.positionCS = TransformWorldToHClip(positionWS);
    
    return output;
}

float4 UnlitPassFragment(Varyings input) : SV_TARGET
{
    UNITY_SETUP_INSTANCE_ID(input);
    return UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial, _BaseColor);
}

#endif