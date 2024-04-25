using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class BuildIncrementer : IPreprocessBuildWithReport
{
    public int callbackOrder => 1;

    public void OnPreprocessBuild(BuildReport report)
    {
        BuildScriptableObject buildScriptableObject = ScriptableObject.CreateInstance<BuildScriptableObject>();

        // buildScriptableObject =
        //PlayerSettings.Android

        PlayerSettings.macOS.buildNumber = IncementBuildNumber(PlayerSettings.macOS.buildNumber);
        buildScriptableObject.BuildNumber = PlayerSettings.macOS.buildNumber;

        AssetDatabase.DeleteAsset("Assets/Resources/Build.asset"); // Delete old
        AssetDatabase.CreateAsset(buildScriptableObject, "Assets/Resources/Build.asset"); // Create the new one with correct build number before build starts
        AssetDatabase.SaveAssets();
    }

    private string IncementBuildNumber(string buildNumber)
    {
        int.TryParse(buildNumber, out var outputBuildNumber);

        return (outputBuildNumber + 1).ToString();
    }
}
