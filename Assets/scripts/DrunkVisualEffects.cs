using UnityEngine;

/// <summary>
/// Enhances the DrunkDrivingSystem with visual camera effects 
/// and chromatic-aberration simulation using overlay UI.
/// </summary>
public class DrunkVisualEffects : MonoBehaviour
{
    [Header("Screen Overlay")]
    public float overlayIntensity = 0.3f;

    private GameObject drunkOverlay;
    private Material overlayMat;

    void Start()
    {
        // Create a full-screen drunk overlay using a world-space quad
        drunkOverlay = GameObject.CreatePrimitive(PrimitiveType.Quad);
        drunkOverlay.name = "DrunkOverlay";
        drunkOverlay.transform.SetParent(Camera.main != null ? Camera.main.transform : transform);
        drunkOverlay.transform.localPosition = new Vector3(0, 0, 0.5f);
        drunkOverlay.transform.localScale = new Vector3(2f, 1.5f, 1f);

        // Create a translucent green/blur material
        overlayMat = new Material(Shader.Find("Standard"));
        overlayMat.SetFloat("_Mode", 3); // Transparent
        overlayMat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        overlayMat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        overlayMat.SetInt("_ZWrite", 0);
        overlayMat.DisableKeyword("_ALPHATEST_ON");
        overlayMat.EnableKeyword("_ALPHABLEND_ON");
        overlayMat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        overlayMat.renderQueue = 3000;
        overlayMat.color = new Color(0.2f, 0.4f, 0.1f, 0f); // Start invisible

        drunkOverlay.GetComponent<Renderer>().material = overlayMat;
        drunkOverlay.GetComponent<Collider>().enabled = false; // No collision
        drunkOverlay.SetActive(false);
    }

    void Update()
    {
        bool isDrunk = CinematicInteractionManager.Instance != null && CinematicInteractionManager.Instance.isDrunk;

        if (isDrunk)
        {
            drunkOverlay.SetActive(true);

            // Pulsing drunk effect
            float pulse = (Mathf.Sin(Time.time * 2f) + 1f) * 0.5f;
            float alpha = Mathf.Lerp(0.05f, overlayIntensity, pulse);
            overlayMat.color = new Color(0.2f, 0.4f, 0.1f, alpha);

            // Slight screen wobble
            if (Camera.main != null)
            {
                float wobbleX = Mathf.Sin(Time.time * 1.3f) * 0.5f;
                float wobbleZ = Mathf.Cos(Time.time * 0.9f) * 0.3f;
                Camera.main.transform.localEulerAngles = new Vector3(wobbleX, 0, wobbleZ);
            }
        }
        else
        {
            if (drunkOverlay != null) drunkOverlay.SetActive(false);

            // Reset camera wobble
            if (Camera.main != null)
                Camera.main.transform.localEulerAngles = Vector3.zero;
        }
    }
}
