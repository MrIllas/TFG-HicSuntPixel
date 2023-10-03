#ifndef CUSTOM_UNITY_INPUT_INCLUDED
#define CUSTOM_UNITY_INPUT_INCLUDED

/*
    THIS HLSL FILE IS USED TO STORE ALL THE STANDARD UNITY INPUT VARIABLES.
*/

CBUFFER_START(UnityPerDraw)
    float4x4 unity_ObjectToWorld; // Model matrix
    float4x4 unity_WorldToObject; // Invers of world matrix
    float4 unity_LODFade;
    real4 unity_WorldTransformParams;
CBUFFER_END

float4x4 unity_MatrixVP;    // View * Projection matrix
float4x4 unity_MatrixV;     // View Matrix
float4x4 unity_MatrixInvV;  // Invers view matrix
float4x4 unity_prev_MatrixM;    //
float4x4 unity_prev_MatrixIM;
float4x4 glstate_matrix_projection;


#endif