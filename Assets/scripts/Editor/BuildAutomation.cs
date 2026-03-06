using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;

/// <summary>
/// Phase 16: Build Automation Engine.
/// Allows the user to build an APK or AAB with one click.
/// Reduces the need to navigate complex Unity build settings.
/// </summary>
public class BuildAutomation : EditorWindow
{
    [MenuItem("Apex Drift/Build/Generate APK (Android)")]
    public static void BuildAPK()
    {
        string buildPath = "Builds/Android/ApexDrift_Goliath.apk";
        
        BuildPlayerOptions options = new BuildPlayerOptions();
        options.scenes = new[] { "Assets/Scenes/MainCity.unity" };
        options.locationPathName = buildPath;
        options.target = BuildTarget.Android;
        options.options = BuildOptions.None;

        Debug.Log("[Build] Starting APK Generation...");
        BuildReport report = BuildPipeline.BuildPlayer(options);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("[Build] Success! APK size: " + summary.totalSize + " bytes. Path: " + buildPath);
            EditorUtility.RevealInFinder(buildPath);
        }
        else
        {
            Debug.LogError("[Build] FAILED. Check console for errors.");
        }
    }

    [MenuItem("Apex Drift/Build/Generate AAB (Play Store)")]
    public static void BuildAAB()
    {
        EditorUserBuildSettings.buildAppBundle = true;
        BuildAPK();
    }
}
