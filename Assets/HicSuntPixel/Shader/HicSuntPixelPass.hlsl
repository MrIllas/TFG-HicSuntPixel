#ifndef HICSUNTPIXEL_PASS_INCLUDED
#define HICSUNTPIXEL_PASS_INCLUDED

struct Attributes
{
    float4 positionOS : POSITION;
    float2 uv : TEXCOORD0;
};

struct Varyings
{
    float4 positionHCS : SV_POSITION;
    float2 uv : TEXCOORD0;
};

TEXTURE2D(_MainTex);
float4 _MainTex_TexelSize;
float4 _MainTex_ST;

SamplerState sampler_point_clamp;

uniform float2 _BlockCount;
uniform float2 _BlockSize;
uniform float2 _HalfBlockSize;

Varyings HSPPassVertex(Attributes input)
{
    Varyings output;
    output.positionHCS = TransformObjectToHClip(input.positionOS.xyz);
    output.uv = TRANSFORM_TEX(input.uv, _MainTex);
    
    return output;
}

float4 HSPPassFragment(Varyings input) : SV_TARGET
{
    float2 blockPos = floor(input.uv * _BlockCount);
    float2 blockCenter = blockPos * _BlockSize + _HalfBlockSize;
    
    float4 tex = SAMPLE_TEXTURE2D(_MainTex, sampler_point_clamp, blockCenter);
    
    return tex;
}

#endif