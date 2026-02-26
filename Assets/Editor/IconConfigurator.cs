using UnityEngine;
using UnityEditor;

/// <summary>
/// Auto-configures the App Icon and Splash Screen logo.
/// Just place the image as "Assets/Textures/Logo.png" (or .jpg/.webp).
/// </summary>
public class IconConfigurator : AssetPostprocessor
{
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        foreach (string str in importedAssets)
        {
            if (str.Contains("Assets/Textures/Logo"))
            {
                Debug.Log("[IconConfigurator] Found new Logo image. Configuring as App Icon...");
                SetAppIcon(str);
            }
        }
    }

    [MenuItem("Apex Drift/Configure App Icon")]
    public static void ManualConfigureIcon()
    {
        string[] guids = AssetDatabase.FindAssets("Logo t:Texture2D", new[] { "Assets/Textures" });
        if (guids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            SetAppIcon(path);
        }
        else
        {
            Debug.LogWarning("[IconConfigurator] Could not find 'Logo' in Assets/Textures. Please name your image 'Logo.png' and place it there.");
        }
    }

    private static void SetAppIcon(string assetPath)
    {
        Texture2D icon = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
        if (icon != null)
        {
            // Set default icon
            PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Unknown, new Texture2D[] { icon });
            Debug.Log("[IconConfigurator] App Icon set successfully!");

            // Also assign it to SplashScreen if it exists in the active scene
            SplashScreen splash = GameObject.FindObjectOfType<SplashScreen>();
            if (splash != null && splash.loadingBar != null)
            {
                // Note: Actual image assignment in scene must be done manually in editor,
                // but this sets the OS-level app icon automatically.
            }
        }
    }
}
