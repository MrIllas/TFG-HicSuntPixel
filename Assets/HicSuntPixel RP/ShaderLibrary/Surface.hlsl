#ifndef CUSTOM_SURFACE_INCLUDED
#define CUSTOM_SURFACE_INCLUDED

/*
    THIS HLSL FILE IS USED TO STORE THE SURFACE PROPERTIES THAT SIMULATES THE INTERACTION OF THE LIGHT ON THE MATERIAL'S SURFACE.
*/

struct Surface
{
    float3 normal;
    float3 viewDirection;
    float3 color;
    float alpha;
    float metallic;
    float smoothness;
};

#endif