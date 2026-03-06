using UnityEngine;
using UnityEditor;
using System.IO;

public class BulkFBXImporter : AssetPostprocessor
{
    // Watches the specific Import Queue folder to handle massive batches
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        foreach (string str in importedAssets)
        {
            if (str.Contains("Import_Queue") && (str.EndsWith(".fbx", System.StringComparison.OrdinalIgnoreCase) || str.EndsWith(".gltf", System.StringComparison.OrdinalIgnoreCase)))
            {
                Debug.Log("BulkFBXImporter Processing: " + str);
                ProcessImportedModel(str);
            }
        }
    }

    static void ProcessImportedModel(string path)
    {
        ModelImporter importer = assetImporter as ModelImporter;
        if (importer != null)
        {
            // Auto configure animation rig to Humanoid if it's a character (simple heuristic: folder name)
            if (path.ToLower().Contains("human") || path.ToLower().Contains("citizen") || path.ToLower().Contains("police"))
            {
                importer.animationType = ModelImporterAnimationType.Human;
                importer.avatarSetup = ModelImporterAvatarSetup.CreateFromThisModel;
            }

            // Auto Extract Materials
            importer.materialImportMode = ModelImporterMaterialImportMode.ImportViaMaterialDescription;
            importer.SaveAndReimport();
            
            // Generate Prefab automatically
            CreatePrefabFromModel(path);
        }
    }

    static void CreatePrefabFromModel(string modelPath)
    {
        GameObject baseObject = AssetDatabase.LoadAssetAtPath<GameObject>(modelPath);
        if (baseObject != null)
        {
            string prefabPath = modelPath.Replace(".fbx", ".prefab").Replace(".gltf", ".prefab");
            
            // Check if prefab already exists
            if (AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) == null)
            {
                GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(baseObject);
                
                // Add basic colliders (heuristic based on name)
                if (modelPath.ToLower().Contains("car") || modelPath.ToLower().Contains("building"))
                {
                    instance.AddComponent<BoxCollider>();
                }
                else if (modelPath.ToLower().Contains("human"))
                {
                    instance.AddComponent<CapsuleCollider>();
                }

                PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);
                GameObject.DestroyImmediate(instance);
                Debug.Log("Generated Prefab: " + prefabPath);
            }
        }
    }
}
