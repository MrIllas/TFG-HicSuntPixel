Shader"Hidden/HicSuntPixel"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white"
    }

    SubShader
    {
        Pass
        {
            Name "HicSuntPixel"
            
            Tags
            {
                "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"
            }

            HLSLPROGRAM
            #pragma target 3.5
            #pragma vertex HSPPassVertex
            #pragma fragment HSPPassFragment

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "HicSuntPixelPass.hlsl"

            ENDHLSL
        }  
    }
}
