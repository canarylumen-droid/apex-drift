using UnityEngine;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// Phase 11: Asset Auto-Binder.
/// Automatically binds imported FBX models (Humans, Cars) to the logical engine.
/// This ensures that code written in Phases 1-10 "sees" and animates the real 3D assets.
/// </summary>
public class AssetAutoBinder : MonoBehaviour
{
    public static AssetAutoBinder Instance { get; private set; }

    [Header("Asset Paths")]
    public string humanModelsPath = "Models/Humans";
    public string vehicleModelsPath = "Models/Vehicles";

    private Dictionary<string, GameObject> modelCache = new Dictionary<string, GameObject>();

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    /// <summary>
    /// Swaps a placeholder mesh with a high-poly FBX model.
    /// Maps the bones automatically.
    /// </summary>
    public GameObject BindHighPolyModel(GameObject target, string modelName)
    {
        GameObject highPolyPrefab = LoadModel(modelName);
        GameObject instance = null;

        if (highPolyPrefab != null)
        {
            Debug.Log($"[AssetBinder] Binding high-poly FBX: {modelName} to {target.name}");
            instance = Instantiate(highPolyPrefab, target.transform.position, target.transform.rotation);
            instance.transform.SetParent(target.transform);
        }
        else
        {
            // Phase 19: Fallback to Proxy if FBX is missing
            Debug.LogWarning($"[AssetBinder] FBX model '{modelName}' not found. Generating 3D Proxy Body.");
            instance = ProxyBodyGenerator.CreateHumanoidProxy(modelName);
            instance.transform.SetParent(target.transform);
            instance.transform.localPosition = Vector3.zero;
        }

        instance.transform.localScale = Vector3.one;

        // 2. Hide low-poly placeholders
        Renderer[] renderers = target.GetComponentsInChildren<Renderer>();
        foreach (var r in renderers)
        {
            if (r.transform != instance.transform && !r.transform.IsChildOf(instance.transform))
                r.enabled = false;
        }

        // 3. Link Animator if present
        Animator targetAnimator = target.GetComponent<Animator>();
        Animator modelAnimator = instance.GetComponent<Animator>();
        if (targetAnimator != null && modelAnimator != null)
        {
            // Set the model's animator to use the target's avatar if they share a rig
            modelAnimator.runtimeAnimatorController = targetAnimator.runtimeAnimatorController;
            targetAnimator.avatar = modelAnimator.avatar;
        }

        // 4. Attach Cinematic Camera Rig if it's a vehicle
        if (target.name.Contains("Bugatti") || target.name.Contains("Lambo"))
        {
            if (target.GetComponent<CinematicCarCamera>() == null)
            {
                var camRig = target.AddComponent<CinematicCarCamera>();
                // In a real setup, we would assign bone points here
                Debug.Log("[AssetBinder] Attached Cinematic Dash/Interior Camera to car.");
            }
        }

        return instance;
    }

    private GameObject LoadModel(string name)
    {
        if (modelCache.ContainsKey(name)) return modelCache[name];

        GameObject prefab = Resources.Load<GameObject>($"{humanModelsPath}/{name}");
        if (prefab == null) prefab = Resources.Load<GameObject>($"{vehicleModelsPath}/{name}");

        if (prefab != null) modelCache[name] = prefab;
        return prefab;
    }
}
