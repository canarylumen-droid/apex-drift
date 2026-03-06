using UnityEngine;

/// <summary>
/// Advanced Model Importer for Chaos City.
/// Automatically swaps primitive humanoids and cars with high-fidelity FBX models
/// once they are downloaded into the Assets/Models directory.
/// </summary>
public class RealisticModelManager : MonoBehaviour
{
    public static RealisticModelManager Instance { get; private set; }

    [Header("Model Prefabs (Assets/Resources/Models/)")]
    public GameObject protagonistPrefab; // Protagonist_HighPoly
    public GameObject swatPrefab;        // SWAT_Tactical
    public GameObject donorPrefab;       // Club_Dancer_A
    public GameObject bugattiPrefab;     // Bugatti_Chiron_Elite

    void Awake()
    {
        if (Instance == null) Instance = this;
        LoadModelsFromResources();
    }

    private void LoadModelsFromResources()
    {
        if (protagonistPrefab == null) protagonistPrefab = Resources.Load<GameObject>("Models/Humans/Protagonist_HighPoly");
        if (swatPrefab == null) swatPrefab = Resources.Load<GameObject>("Models/Humans/SWAT_Tactical");
        if (donorPrefab == null) donorPrefab = Resources.Load<GameObject>("Models/Humans/Club_Dancer_A");
        if (bugattiPrefab == null) bugattiPrefab = Resources.Load<GameObject>("Models/Vehicles/Bugatti_Chiron_Elite");
    }

    public GameObject SwapWithRealistic(GameObject primitiveObj, string type)
    {
        GameObject prefab = null;
        if (type == "CarDefault") prefab = carModel1Prefab;
        if (type == "HumanDefault") prefab = humanIdlePrefab;
        if (type == "HumanRival") prefab = humanIdlePrefab; // Reuse for now

        if (prefab == null) return primitiveObj;

        GameObject real = Instantiate(prefab, primitiveObj.transform.position, primitiveObj.transform.rotation);
        real.transform.SetParent(primitiveObj.transform.parent);
        
        // Add Social Component if it's a human
        if (type.Contains("Human"))
        {
            SocialNPC social = real.AddComponent<SocialNPC>();
            // If it was a rival, set to Aggressive
            if (type == "HumanRival") social.SetSocialState(SocialNPC.SocialState.Aggressive);
        }

        Destroy(primitiveObj);
        return real;
    }
}
