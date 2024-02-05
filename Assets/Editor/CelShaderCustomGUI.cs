//#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CelShaderCustomGUI : ShaderGUI
{
    GUIContent Label = new GUIContent();
    GUILayoutOption[] options;
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
        MaterialProperty normalMap = FindProperty("_NormalMap", properties);
        materialEditor.TexturePropertySingleLine(new GUIContent("Normal"), normalMap);
        if(normalMap.textureValue != null)
        {
            MaterialProperty normalStrength = FindProperty("_NormalStrength", properties);
            materialEditor.ShaderProperty(normalStrength, "Normal Strength");
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