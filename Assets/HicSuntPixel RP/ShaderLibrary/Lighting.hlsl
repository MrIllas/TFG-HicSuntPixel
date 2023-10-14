#ifndef CUSTOM_LIGHTING_INCLUDED
#define CUSTOM_LIGHTING_INCLUDED

/*
    THIS HLSL FILE IS USED TO STORE THE LIGHTING FUNCTIONS.
*/


/*
    Calculates how much incoming light thre is for a given surface and light.
*/
float3 IncomingLight (Surface surface, Light light)
{
    //saturate clamps negative dot products to zero.
    return saturate(dot(surface.normal, light.direction)) * light.color;
}


/*
    Returns the incoming light multiplied with the surface color.
*/
float3 GetLighting (Surface surface, BRDF brdf, Light light)
{
    return IncomingLight(surface, light) * DirectBRDF(surface, brdf, light);
}

/*
    Returns the final lighting "color" for a surface and a light.
*/
float3 GetLighting (Surface surface, BRDF brdf)
{
    float3 toReturn = 0.0f;
    for (int i = 0; i < GetDirectionalLightCount(); ++i)
    {
        toReturn += GetLighting(surface, brdf, GetDirectionalLight(i));
    }
    return toReturn;
}






#endif