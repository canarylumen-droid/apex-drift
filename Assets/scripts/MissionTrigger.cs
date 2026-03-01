using UnityEngine;

/// <summary>
/// A visible 3D trigger that starts a mission when the car drives through it.
/// Looks like a spinning "Mission Icon" in the world.
/// </summary>
public class MissionTrigger : MonoBehaviour
{
    public MissionManager.MissionType missionType;
    public Color iconColor = Color.yellow;

    private void Start()
    {
        // Visuals: Create a spinning cube icon
        GameObject icon = GameObject.CreatePrimitive(PrimitiveType.Cube);
        icon.transform.SetParent(transform);
        icon.transform.localPosition = Vector3.zero;
        icon.transform.localScale = Vector3.one * 0.8f;
        
        Renderer r = icon.GetComponent<Renderer>();
        r.material = new Material(Shader.Find("Standard"));
        r.material.color = iconColor;
        r.material.EnableKeyword("_EMISSION");
        r.material.SetColor("_EmissionColor", iconColor * 0.5f);

        Destroy(icon.GetComponent<BoxCollider>()); // Just visual
        
        // Trigger area
        SphereCollider sc = gameObject.AddComponent<SphereCollider>();
        sc.isTrigger = true;
        sc.radius = 3f;
    }

    private void Update()
    {
        // Spin the icon
        transform.Rotate(Vector3.up * 90f * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<RCC_CarControllerV3>() != null)
        {
            if (MissionManager.Instance != null)
            {
                MissionManager.Instance.StartMission(missionType);
                gameObject.SetActive(false); // One-time use
            }
        }
    }
}
