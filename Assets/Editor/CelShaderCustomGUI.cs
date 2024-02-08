//#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CelShaderCustomGUI : ShaderGUI
{
    bool outlineSettings, outlineDebug;

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        GUILayout.Space(20);

        //Albedo
        EditorGUILayout.LabelField("Textures", EditorStyles.boldLabel);
        MaterialProperty mainTex = FindProperty("_MainTex", properties);
        MaterialProperty albedoColor = FindProperty("_MainColor", properties);
        EditorGUILayout.BeginHorizontal();
        materialEditor.TexturePropertySingleLine(new GUIContent("Albedo"), mainTex);
        materialEditor.ColorProperty(albedoColor, "");
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10);

        //Normal
        MaterialProperty normalsToggle = FindProperty("_Normal_Throughout_Texture", properties);
        materialEditor.ShaderProperty(normalsToggle, "Use normal map");

        if (normalsToggle.floatValue > 0.0f)
        {
            MaterialProperty normalMap = FindProperty("_NormalMap", properties);
            materialEditor.TexturePropertySingleLine(new GUIContent("Normal"), normalMap);
            if (normalMap.textureValue != null)
            {
                MaterialProperty normalStrength = FindProperty("_NormalStrength", properties);
                materialEditor.ShaderProperty(normalStrength, "Normal Strength");
            }
        }

        GUILayout.Space(10);

        //Smoothness
        MaterialProperty smoothnessMap = FindProperty("_SmoothnessMap", properties);
        MaterialProperty smoothness = FindProperty("_Smoothness", properties);
        EditorGUILayout.BeginHorizontal();
        materialEditor.TexturePropertySingleLine(new GUIContent("Smoothness"), smoothnessMap);
        materialEditor.ShaderProperty(smoothness, "");
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10);

        //Emission
        MaterialProperty emissionToggle = FindProperty("_Emission", properties);
        materialEditor.ShaderProperty(emissionToggle, "Emission");

        if (emissionToggle.floatValue > 0)
        {
        }
        else
        {
            GUI.enabled = false;
        }

        MaterialProperty emissionMap = FindProperty("_EmissionMap", properties);
        materialEditor.TexturePropertySingleLine(new GUIContent("Emission Map"), emissionMap);
        EditorGUILayout.BeginHorizontal();
        MaterialProperty emissionColor = FindProperty("_EmissionColor", properties);
        materialEditor.ColorProperty(emissionColor, "Emission Color");
        EditorGUILayout.EndHorizontal();

        GUI.enabled = true;
        GUILayout.Space(20);

        //Outlines
        EditorGUILayout.LabelField("Outline", EditorStyles.boldLabel);

        MaterialProperty outlinesToggle = FindProperty("_Outlines", properties);
        materialEditor.ShaderProperty(outlinesToggle, "Outlines");

        if (outlinesToggle.floatValue > 0) 
        {
            MaterialProperty outlineThickness = FindProperty("_OutlineThickness", properties);
            materialEditor.ShaderProperty(outlineThickness, "Thickness");

            outlineSettings = EditorGUILayout.BeginFoldoutHeaderGroup(outlineSettings, "Outline Fine-Tunning Settings");
            if (outlineSettings)
            {
                MaterialProperty depthThreshold = FindProperty("_DepthThreshold", properties);
                materialEditor.ShaderProperty(depthThreshold, "Depth Threshold");
                MaterialProperty normalsThreshold = FindProperty("_NormalsThreshold", properties);
                materialEditor.ShaderProperty(normalsThreshold, "Normals Threshold");

            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            //outlineDebug = EditorGUILayout.BeginFoldoutHeaderGroup(outlineDebug, "Debug");
            //if (outlineDebug)
            //{
            //    MaterialProperty depthEdgeStrength = FindProperty("_DepthEdgeStrength", properties);
            //    materialEditor.ShaderProperty(depthEdgeStrength, "Depth Edge Strength");

            //    MaterialProperty normalEdgeStrength = FindProperty("_NormalEdgeStrength", properties);
            //    materialEditor.ShaderProperty(normalEdgeStrength, "Normal Edge Strength");

            //    MaterialProperty normalEdgeBias = FindProperty("_NormalEdgeBias", properties);
            //    materialEditor.ShaderProperty(normalEdgeBias, "Normal Edge Bias");
            //}

            //EditorGUILayout.EndFoldoutHeaderGroup();
        }

        GUILayout.Space(20);
        //Modifiers
        EditorGUILayout.LabelField("Modifiers", EditorStyles.boldLabel);

        MaterialProperty edgeSpecular = FindProperty("_EdgeSpecular", properties);
        materialEditor.ShaderProperty(edgeSpecular, "Edge Specular");

        MaterialProperty edgeRim = FindProperty("_EdgeRim", properties);
        materialEditor.ShaderProperty(edgeRim, "Edge Rim");

        MaterialProperty rimThreshold = FindProperty("_RimThreshold", properties);
        materialEditor.ShaderProperty(rimThreshold, "Rim Threshold");
    }
}
//#endif