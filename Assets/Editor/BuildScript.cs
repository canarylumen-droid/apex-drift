#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;

/// <summary>
/// Build Script for Apex Drift: 3D Racing.
/// Run from command line:
///   Unity.exe -batchmode -nographics -projectPath "path" -executeMethod BuildScript.BuildAPK -quit
///   Unity.exe -batchmode -nographics -projectPath "path" -executeMethod BuildScript.BuildAAB -quit
/// </summary>
public class BuildScript
{
    // ===== BUILD APK (for testing) =====
    public static void BuildAPK()
    {
        ConfigurePlayerSettings();

        string outputPath = "Builds/ApexDrift.apk";
        EnsureDirectory(outputPath);

        string[] scenes = GetScenes();

        BuildPipeline.BuildPlayer(new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = outputPath,
            target = BuildTarget.Android,
            options = BuildOptions.None
        });

        Debug.Log("[BuildScript] APK built at: " + outputPath);
    }

    // ===== BUILD AAB (for Play Store) =====
    public static void BuildAAB()
    {
        ConfigurePlayerSettings();

        // Enable AAB format
        EditorUserBuildSettings.buildAppBundle = true;

        string outputPath = "Builds/ApexDrift.aab";
        EnsureDirectory(outputPath);

        string[] scenes = GetScenes();

        BuildPipeline.BuildPlayer(new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = outputPath,
            target = BuildTarget.Android,
            options = BuildOptions.None
        });

        Debug.Log("[BuildScript] AAB built at: " + outputPath);
    }

    // ===== CONFIGURATION =====
    static void ConfigurePlayerSettings()
    {
        // Branding
        PlayerSettings.productName = GameConfig.GAME_NAME;
        PlayerSettings.companyName = GameConfig.DEVELOPER;
        PlayerSettings.applicationIdentifier = GameConfig.PACKAGE_NAME;
        PlayerSettings.bundleVersion = GameConfig.VERSION;
        PlayerSettings.Android.bundleVersionCode = GameConfig.VERSION_CODE;

        // Android settings
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel24; // Android 7.0
        PlayerSettings.Android.targetSdkVersion = (AndroidSdkVersions)35; // Android 15+ (API 35)

        // Keystore
        PlayerSettings.Android.useCustomKeystore = false;
        // PlayerSettings.Android.keystoreName = "apexdrift.keystore";
        // PlayerSettings.Android.keystorePass = "ApexDrift2024!";
        // PlayerSettings.Android.keyaliasName = "apexdrift";
        // PlayerSettings.Android.keyaliasPass = "ApexDrift2024!";

        // Graphics
        PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, new[] {
            UnityEngine.Rendering.GraphicsDeviceType.OpenGLES3,
            UnityEngine.Rendering.GraphicsDeviceType.Vulkan
        });

        // ARM64 for Play Store requirement
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64 | AndroidArchitecture.ARMv7;

        // Internet for AdMob (game is offline-only otherwise)
        PlayerSettings.Android.forceInternetPermission = true;

        Debug.Log("[BuildScript] Player settings configured for " + GameConfig.GAME_NAME);
    }

    // ===== HELPERS =====
    static string[] GetScenes()
    {
        // Get all scenes from build settings
        EditorBuildSettingsScene[] buildScenes = EditorBuildSettings.scenes;
        string[] scenePaths = new string[buildScenes.Length];
        for (int i = 0; i < buildScenes.Length; i++)
        {
            scenePaths[i] = buildScenes[i].path;
        }
        return scenePaths;
    }

    static void EnsureDirectory(string filePath)
    {
        string dir = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);
    }
}
#endif
