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

    // ───────────────────────── 3D MODEL BUILDERS ─────────────────────────

    /// <summary>
    /// Creates a visible humanoid figure out of primitive shapes.
    /// This is a PLACEHOLDER until Mixamo models are imported.
    /// Head (sphere), body (capsule), arms (cylinders).
    /// </summary>
    private GameObject CreateHumanoid(string name)
    {
        GameObject root = new GameObject(name);

        // Body (capsule)
        GameObject body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        body.name = "Body";
        body.transform.SetParent(root.transform);
        body.transform.localPosition = new Vector3(0, 1f, 0);
        body.transform.localScale = new Vector3(0.4f, 0.5f, 0.3f);
        SetColor(body, new Color(0.2f, 0.3f, 0.6f)); // Dark blue clothing

        // Head (sphere)
        GameObject head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        head.name = "Head";
        head.transform.SetParent(root.transform);
        head.transform.localPosition = new Vector3(0, 1.65f, 0);
        head.transform.localScale = Vector3.one * 0.25f;
        SetColor(head, new Color(0.85f, 0.7f, 0.55f)); // Skin tone

        // Left Arm
        GameObject lArm = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        lArm.name = "LeftArm";
        lArm.transform.SetParent(root.transform);
        lArm.transform.localPosition = new Vector3(-0.3f, 1.1f, 0);
        lArm.transform.localScale = new Vector3(0.1f, 0.3f, 0.1f);
        SetColor(lArm, new Color(0.85f, 0.7f, 0.55f));

        // Right Arm
        GameObject rArm = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        rArm.name = "RightArm";
        rArm.transform.SetParent(root.transform);
        rArm.transform.localPosition = new Vector3(0.3f, 1.1f, 0);
        rArm.transform.localScale = new Vector3(0.1f, 0.3f, 0.1f);
        SetColor(rArm, new Color(0.85f, 0.7f, 0.55f));

        // Left Leg
        GameObject lLeg = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        lLeg.name = "LeftLeg";
        lLeg.transform.SetParent(root.transform);
        lLeg.transform.localPosition = new Vector3(-0.12f, 0.35f, 0);
        lLeg.transform.localScale = new Vector3(0.12f, 0.35f, 0.12f);
        SetColor(lLeg, new Color(0.15f, 0.15f, 0.15f)); // Dark pants

        // Right Leg
        GameObject rLeg = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        rLeg.name = "RightLeg";
        rLeg.transform.SetParent(root.transform);
        rLeg.transform.localPosition = new Vector3(0.12f, 0.35f, 0);
        rLeg.transform.localScale = new Vector3(0.12f, 0.35f, 0.12f);
        SetColor(rLeg, new Color(0.15f, 0.15f, 0.15f));

        // Ensure shadow casting on all parts
        foreach (Renderer r in root.GetComponentsInChildren<Renderer>())
        {
            r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            r.receiveShadows = true;
        }

        return root;
    }

    /// <summary>
    /// Creates a visible traffic light out of primitives.
    /// Pole (cylinder) + 3 lights (spheres: red, yellow, green).
    /// </summary>
    private GameObject CreateTrafficLight(string name)
    {
        GameObject root = new GameObject(name);

        // Pole
        GameObject pole = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        pole.name = "Pole";
        pole.transform.SetParent(root.transform);
        pole.transform.localPosition = Vector3.zero;
        pole.transform.localScale = new Vector3(0.08f, 2.5f, 0.08f);
        SetColor(pole, Color.gray);

        // Housing
        GameObject housing = GameObject.CreatePrimitive(PrimitiveType.Cube);
        housing.name = "Housing";
        housing.transform.SetParent(root.transform);
        housing.transform.localPosition = new Vector3(0, 2.5f, 0);
        housing.transform.localScale = new Vector3(0.35f, 0.9f, 0.2f);
        SetColor(housing, new Color(0.15f, 0.15f, 0.15f));

        // Red Light
        GameObject red = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        red.name = "RedLight";
        red.transform.SetParent(root.transform);
        red.transform.localPosition = new Vector3(0, 2.8f, 0.11f);
        red.transform.localScale = Vector3.one * 0.18f;
        SetColor(red, Color.red);

        // Yellow Light
        GameObject yellow = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        yellow.name = "YellowLight";
        yellow.transform.SetParent(root.transform);
        yellow.transform.localPosition = new Vector3(0, 2.5f, 0.11f);
        yellow.transform.localScale = Vector3.one * 0.18f;
        SetColor(yellow, Color.yellow);

        // Green Light
        GameObject green = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        green.name = "GreenLight";
        green.transform.SetParent(root.transform);
        green.transform.localPosition = new Vector3(0, 2.2f, 0.11f);
        green.transform.localScale = Vector3.one * 0.18f;
        SetColor(green, Color.green);

        return root;
    }

    // ───────────────────────── HELPERS ─────────────────────────

    private void SetColor(GameObject obj, Color color)
    {
        Renderer r = obj.GetComponent<Renderer>();
        if (r != null)
        {
            r.material = new Material(Shader.Find("Standard"));
            r.material.color = color;
        }
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
