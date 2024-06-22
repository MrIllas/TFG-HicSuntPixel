using UnityEngine;
using UnityEditor;

public class AnimationImportProcessor : AssetPostprocessor
{
    void OnPostprocessModel(GameObject g)
    {
       
    }

    private void OnPostprocessAnimation(GameObject root, AnimationClip clip)
    {
        SetConstantTangents(clip);
    }

    private void SetConstantTangents(AnimationClip clip)
    {
        Debug.Log("Processing new imported animation clip!");
        var bindings = AnimationUtility.GetCurveBindings(clip);
        foreach (var binding in bindings)
        {
            AnimationCurve curve = AnimationUtility.GetEditorCurve(clip, binding);
            if (curve != null)
            {
                for (int i = 0; i < curve.keys.Length; i++)
                {
                    AnimationUtility.SetKeyLeftTangentMode(curve, i, AnimationUtility.TangentMode.Constant);
                    AnimationUtility.SetKeyRightTangentMode(curve, i, AnimationUtility.TangentMode.Constant);
                }
                AnimationUtility.SetEditorCurve(clip, binding, curve);
            }
        }
    }
}