using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// AUTO-GENERATES visible 3D content at runtime.
/// Creates pedestrians, traffic objects, and drivers WITHOUT needing Inspector assignment.
/// This is the "make it look real" script.
/// </summary>
public class RuntimeWorldBuilder : MonoBehaviour
{
    [Header("Spawn Counts")]
    public int pedestrianCount = 8;
    public int trafficLightCount = 4;

    [Header("Spawn Area")]
    public float spawnRadius = 80f;
    public float roadY = 0.5f;

    void Start()
    {
        SpawnPedestrians();
        SpawnTrafficLights();
        PlaceDriversInAllCars();
        MakeEnvironmentDestructible();
    }

    // ───────────────────────── PEDESTRIANS ─────────────────────────
    private void SpawnPedestrians()
    {
        for (int i = 0; i < pedestrianCount; i++)
        {
            Vector3 pos = GetRandomNavMeshPosition();
            if (pos == Vector3.zero) continue;

            GameObject ped = CreateHumanoid("Pedestrian_" + i);
            ped.transform.position = pos;

            // Add NavMeshAgent for walking AI
            NavMeshAgent agent = ped.AddComponent<NavMeshAgent>();
            agent.speed = 1.2f;
            agent.radius = 0.3f;
            agent.height = 1.8f;

            // Add pedestrian interaction (shouting/talking)
            ped.AddComponent<PedestrianInteraction>();

            // Add wandering AI
            ped.AddComponent<PedestrianWander>();
        }

        Debug.Log("[WorldBuilder] Spawned " + pedestrianCount + " pedestrians.");
    }

    // ───────────────────────── TRAFFIC LIGHTS ─────────────────────────
    private void SpawnTrafficLights()
    {
        for (int i = 0; i < trafficLightCount; i++)
        {
            Vector3 pos = GetRandomNavMeshPosition();
            if (pos == Vector3.zero) continue;

            GameObject tl = CreateTrafficLight("TrafficLight_" + i);
            tl.transform.position = pos + Vector3.up * 2.5f;
            tl.AddComponent<TrafficLightSystem>();
        }

        Debug.Log("[WorldBuilder] Spawned " + trafficLightCount + " traffic lights.");
    }

    // ───────────────────────── DRIVERS IN CARS ─────────────────────────
    private void PlaceDriversInAllCars()
    {
        RCC_CarControllerV3[] cars = FindObjectsOfType<RCC_CarControllerV3>();
        foreach (var car in cars)
        {
            // Check if a driver already exists
            Transform existing = car.transform.Find("Driver");
            if (existing != null) continue;

            GameObject driver = CreateHumanoid("Driver");
            driver.transform.SetParent(car.transform);
            driver.transform.localPosition = new Vector3(-0.35f, 0.4f, 0.1f); // Driver seat offset
            driver.transform.localRotation = Quaternion.identity;
            driver.transform.localScale = Vector3.one * 0.9f;

            // Disable NavMeshAgent on drivers (they sit)
            NavMeshAgent nma = driver.GetComponent<NavMeshAgent>();
            if (nma != null) Destroy(nma);
        }

        Debug.Log("[WorldBuilder] Placed drivers in " + cars.Length + " cars.");
    }

    /// <summary>
    /// Creates a realistic-looking humanoid from primitives.
    /// Randomized skin tones, clothing, hair, shoes, hands, neck.
    /// Uses smooth metallic materials for a clean 3D look.
    /// </summary>
    private GameObject CreateHumanoid(string name)
    {
        GameObject root = new GameObject(name);

        // Randomize appearance
        Color skinTone = GetRandomSkinTone();
        Color shirtColor = GetRandomClothingColor();
        Color pantsColor = GetRandomPantsColor();
        Color shoeColor = Random.value > 0.5f ? Color.black : new Color(0.3f, 0.15f, 0.05f);
        Color hairColor = GetRandomHairColor();
        bool isTall = Random.value > 0.5f;
        float heightMult = isTall ? 1.05f : 0.95f;

        // ─── TORSO (Upper body) ───
        GameObject torso = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        torso.name = "Torso";
        torso.transform.SetParent(root.transform);
        torso.transform.localPosition = new Vector3(0, 1.05f * heightMult, 0);
        torso.transform.localScale = new Vector3(0.38f, 0.42f, 0.22f);
        ApplySmooth(torso, shirtColor);

        // ─── LOWER TORSO / HIPS ───
        GameObject hips = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        hips.name = "Hips";
        hips.transform.SetParent(root.transform);
        hips.transform.localPosition = new Vector3(0, 0.68f * heightMult, 0);
        hips.transform.localScale = new Vector3(0.36f, 0.18f, 0.22f);
        ApplySmooth(hips, pantsColor);

        // ─── NECK ───
        GameObject neck = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        neck.name = "Neck";
        neck.transform.SetParent(root.transform);
        neck.transform.localPosition = new Vector3(0, 1.48f * heightMult, 0);
        neck.transform.localScale = new Vector3(0.08f, 0.06f, 0.08f);
        ApplySmooth(neck, skinTone);

        // ─── HEAD ───
        GameObject head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        head.name = "Head";
        head.transform.SetParent(root.transform);
        head.transform.localPosition = new Vector3(0, 1.62f * heightMult, 0);
        head.transform.localScale = new Vector3(0.22f, 0.26f, 0.22f);
        ApplySmooth(head, skinTone);

        // ─── HAIR ───
        GameObject hair = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        hair.name = "Hair";
        hair.transform.SetParent(head.transform);
        hair.transform.localPosition = new Vector3(0, 0.35f, -0.05f);
        hair.transform.localScale = new Vector3(1.08f, 0.6f, 1.05f);
        ApplySmooth(hair, hairColor);

        // ─── LEFT UPPER ARM ───
        GameObject lUpperArm = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        lUpperArm.name = "L_UpperArm";
        lUpperArm.transform.SetParent(root.transform);
        lUpperArm.transform.localPosition = new Vector3(-0.28f, 1.15f * heightMult, 0);
        lUpperArm.transform.localScale = new Vector3(0.09f, 0.2f, 0.09f);
        ApplySmooth(lUpperArm, shirtColor);

        // ─── LEFT FOREARM ───
        GameObject lForearm = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        lForearm.name = "L_Forearm";
        lForearm.transform.SetParent(root.transform);
        lForearm.transform.localPosition = new Vector3(-0.28f, 0.82f * heightMult, 0);
        lForearm.transform.localScale = new Vector3(0.08f, 0.18f, 0.08f);
        ApplySmooth(lForearm, skinTone);

        // ─── LEFT HAND ───
        GameObject lHand = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        lHand.name = "L_Hand";
        lHand.transform.SetParent(root.transform);
        lHand.transform.localPosition = new Vector3(-0.28f, 0.62f * heightMult, 0);
        lHand.transform.localScale = new Vector3(0.07f, 0.04f, 0.05f);
        ApplySmooth(lHand, skinTone);

        // ─── RIGHT UPPER ARM ───
        GameObject rUpperArm = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        rUpperArm.name = "R_UpperArm";
        rUpperArm.transform.SetParent(root.transform);
        rUpperArm.transform.localPosition = new Vector3(0.28f, 1.15f * heightMult, 0);
        rUpperArm.transform.localScale = new Vector3(0.09f, 0.2f, 0.09f);
        ApplySmooth(rUpperArm, shirtColor);

        // ─── RIGHT FOREARM ───
        GameObject rForearm = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        rForearm.name = "R_Forearm";
        rForearm.transform.SetParent(root.transform);
        rForearm.transform.localPosition = new Vector3(0.28f, 0.82f * heightMult, 0);
        rForearm.transform.localScale = new Vector3(0.08f, 0.18f, 0.08f);
        ApplySmooth(rForearm, skinTone);

        // ─── RIGHT HAND ───
        GameObject rHand = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        rHand.name = "R_Hand";
        rHand.transform.SetParent(root.transform);
        rHand.transform.localPosition = new Vector3(0.28f, 0.62f * heightMult, 0);
        rHand.transform.localScale = new Vector3(0.07f, 0.04f, 0.05f);
        ApplySmooth(rHand, skinTone);

        // ─── LEFT THIGH ───
        GameObject lThigh = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        lThigh.name = "L_Thigh";
        lThigh.transform.SetParent(root.transform);
        lThigh.transform.localPosition = new Vector3(-0.1f, 0.42f * heightMult, 0);
        lThigh.transform.localScale = new Vector3(0.13f, 0.22f, 0.13f);
        ApplySmooth(lThigh, pantsColor);

        // ─── LEFT SHIN ───
        GameObject lShin = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        lShin.name = "L_Shin";
        lShin.transform.SetParent(root.transform);
        lShin.transform.localPosition = new Vector3(-0.1f, 0.12f * heightMult, 0);
        lShin.transform.localScale = new Vector3(0.1f, 0.2f, 0.1f);
        ApplySmooth(lShin, pantsColor);

        // ─── LEFT SHOE ───
        GameObject lShoe = GameObject.CreatePrimitive(PrimitiveType.Cube);
        lShoe.name = "L_Shoe";
        lShoe.transform.SetParent(root.transform);
        lShoe.transform.localPosition = new Vector3(-0.1f, 0.03f, 0.04f);
        lShoe.transform.localScale = new Vector3(0.1f, 0.06f, 0.18f);
        ApplySmooth(lShoe, shoeColor);

        // ─── RIGHT THIGH ───
        GameObject rThigh = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        rThigh.name = "R_Thigh";
        rThigh.transform.SetParent(root.transform);
        rThigh.transform.localPosition = new Vector3(0.1f, 0.42f * heightMult, 0);
        rThigh.transform.localScale = new Vector3(0.13f, 0.22f, 0.13f);
        ApplySmooth(rThigh, pantsColor);

        // ─── RIGHT SHIN ───
        GameObject rShin = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        rShin.name = "R_Shin";
        rShin.transform.SetParent(root.transform);
        rShin.transform.localPosition = new Vector3(0.1f, 0.12f * heightMult, 0);
        rShin.transform.localScale = new Vector3(0.1f, 0.2f, 0.1f);
        ApplySmooth(rShin, pantsColor);

        // ─── RIGHT SHOE ───
        GameObject rShoe = GameObject.CreatePrimitive(PrimitiveType.Cube);
        rShoe.name = "R_Shoe";
        rShoe.transform.SetParent(root.transform);
        rShoe.transform.localPosition = new Vector3(0.1f, 0.03f, 0.04f);
        rShoe.transform.localScale = new Vector3(0.1f, 0.06f, 0.18f);
        ApplySmooth(rShoe, shoeColor);

        // Ensure shadow casting on ALL parts
        foreach (Renderer r in root.GetComponentsInChildren<Renderer>())
        {
            r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            r.receiveShadows = true;
        }

        return root;
    }

    /// <summary>
    /// Creates a visible traffic light.
    /// Pole (cylinder) + housing + 3 emissive lights.
    /// </summary>
    private GameObject CreateTrafficLight(string name)
    {
        GameObject root = new GameObject(name);
        root.AddComponent<DestructibleProp>().breakForce = 25f; // Traffic lights are sturdy

        // Pole
        GameObject pole = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        pole.name = "Pole";
        pole.transform.SetParent(root.transform);
        pole.transform.localPosition = Vector3.zero;
        pole.transform.localScale = new Vector3(0.08f, 2.5f, 0.08f);
        ApplySmooth(pole, new Color(0.35f, 0.35f, 0.35f), 0.8f);

        // Housing
        GameObject housing = GameObject.CreatePrimitive(PrimitiveType.Cube);
        housing.name = "Housing";
        housing.transform.SetParent(root.transform);
        housing.transform.localPosition = new Vector3(0, 2.5f, 0);
        housing.transform.localScale = new Vector3(0.35f, 0.9f, 0.2f);
        ApplySmooth(housing, new Color(0.08f, 0.08f, 0.08f), 0.9f);

        // Visor on top
        GameObject visor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        visor.name = "Visor";
        visor.transform.SetParent(root.transform);
        visor.transform.localPosition = new Vector3(0, 2.92f, 0.05f);
        visor.transform.localScale = new Vector3(0.38f, 0.04f, 0.25f);
        ApplySmooth(visor, new Color(0.08f, 0.08f, 0.08f));

        // Red Light
        GameObject red = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        red.name = "RedLight";
        red.transform.SetParent(root.transform);
        red.transform.localPosition = new Vector3(0, 2.8f, 0.11f);
        red.transform.localScale = Vector3.one * 0.18f;
        ApplySmooth(red, Color.red);

        // Yellow Light
        GameObject yellow = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        yellow.name = "YellowLight";
        yellow.transform.SetParent(root.transform);
        yellow.transform.localPosition = new Vector3(0, 2.5f, 0.11f);
        yellow.transform.localScale = Vector3.one * 0.18f;
        ApplySmooth(yellow, Color.yellow);

        // Green Light
        GameObject green = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        green.name = "GreenLight";
        green.transform.SetParent(root.transform);
        green.transform.localPosition = new Vector3(0, 2.2f, 0.11f);
        green.transform.localScale = Vector3.one * 0.18f;
        ApplySmooth(green, Color.green);

        return root;
    }

    /// <summary>
    /// Searches for existing objects that look like road props (fences, poles, boxes) 
    /// and adds the DestructibleProp script to them automatically.
    /// </summary>
    private void MakeEnvironmentDestructible()
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject go in allObjects)
        {
            string n = go.name.ToLower();
            if (n.Contains("fence") || n.Contains("pole") || n.Contains("barrel") || n.Contains("crate") || n.Contains("wall") || n.Contains("barrier"))
            {
                if (go.GetComponent<DestructibleProp>() == null && go.GetComponent<Renderer>() != null)
                {
                    go.AddComponent<DestructibleProp>();
                }
            }
        }
    }

    // ───────────────────────── MATERIALS ─────────────────────────

    /// <summary>
    /// Applies a smooth, slightly metallic Standard shader material.
    /// Looks much cleaner than flat colors.
    /// </summary>
    private void ApplySmooth(GameObject obj, Color color, float metallic = 0.15f)
    {
        Renderer r = obj.GetComponent<Renderer>();
        if (r == null) return;

        Material mat = new Material(Shader.Find("Standard"));
        mat.color = color;
        mat.SetFloat("_Metallic", metallic);
        mat.SetFloat("_Glossiness", 0.45f); // Smooth but not mirror
        r.material = mat;
    }

    // ───────────────────────── RANDOMIZERS ─────────────────────────

    private Color GetRandomSkinTone()
    {
        Color[] tones = new Color[]
        {
            new Color(0.96f, 0.84f, 0.72f), // Light
            new Color(0.87f, 0.72f, 0.53f), // Medium
            new Color(0.76f, 0.57f, 0.38f), // Tan
            new Color(0.55f, 0.38f, 0.26f), // Brown
            new Color(0.36f, 0.22f, 0.14f), // Dark
        };
        return tones[Random.Range(0, tones.Length)];
    }

    private Color GetRandomClothingColor()
    {
        Color[] colors = new Color[]
        {
            new Color(0.15f, 0.15f, 0.5f),  // Navy
            new Color(0.6f, 0.1f, 0.1f),    // Dark Red
            new Color(0.1f, 0.4f, 0.15f),   // Forest Green
            new Color(0.9f, 0.9f, 0.9f),    // White
            new Color(0.08f, 0.08f, 0.08f), // Black
            new Color(0.4f, 0.2f, 0.5f),    // Purple
            new Color(0.85f, 0.6f, 0.1f),   // Orange/Gold
            new Color(0.2f, 0.5f, 0.7f),    // Teal
        };
        return colors[Random.Range(0, colors.Length)];
    }

    private Color GetRandomPantsColor()
    {
        Color[] colors = new Color[]
        {
            new Color(0.12f, 0.12f, 0.18f), // Dark navy
            new Color(0.08f, 0.08f, 0.08f), // Black
            new Color(0.3f, 0.28f, 0.25f),  // Khaki
            new Color(0.2f, 0.15f, 0.1f),   // Brown
            new Color(0.35f, 0.35f, 0.4f),  // Gray
        };
        return colors[Random.Range(0, colors.Length)];
    }

    private Color GetRandomHairColor()
    {
        Color[] colors = new Color[]
        {
            new Color(0.05f, 0.03f, 0.02f), // Black
            new Color(0.3f, 0.18f, 0.08f),  // Dark Brown
            new Color(0.55f, 0.35f, 0.15f), // Light Brown
            new Color(0.85f, 0.7f, 0.4f),   // Blonde
            new Color(0.5f, 0.1f, 0.05f),   // Auburn
        };
        return colors[Random.Range(0, colors.Length)];
    }

    private Vector3 GetRandomNavMeshPosition()
    {
        Vector3 randomDir = Random.insideUnitSphere * spawnRadius;
        randomDir.y = 0;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDir, out hit, spawnRadius, NavMesh.AllAreas))
        {
            return hit.position;
        }
        return Vector3.zero;
    }
}
