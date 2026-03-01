using UnityEngine;
using System.Collections;

/// <summary>
/// Controls a traffic light cycle (Red → Green → Yellow → Red).
/// Changes light colors on child objects and can signal cars to stop/go.
/// </summary>
public class TrafficLightSystem : MonoBehaviour
{
    public float greenDuration = 10f;
    public float yellowDuration = 3f;
    public float redDuration = 8f;

    private Renderer redLight;
    private Renderer yellowLight;
    private Renderer greenLight;

    private Color dimRed = new Color(0.3f, 0, 0);
    private Color dimYellow = new Color(0.3f, 0.3f, 0);
    private Color dimGreen = new Color(0, 0.3f, 0);
    private Color brightRed = Color.red;
    private Color brightYellow = Color.yellow;
    private Color brightGreen = Color.green;

    void Start()
    {
        // Find light objects by name
        Transform r = transform.Find("RedLight");
        Transform y = transform.Find("YellowLight");
        Transform g = transform.Find("GreenLight");

        if (r != null) redLight = r.GetComponent<Renderer>();
        if (y != null) yellowLight = y.GetComponent<Renderer>();
        if (g != null) greenLight = g.GetComponent<Renderer>();

        StartCoroutine(LightCycle());
    }

    private IEnumerator LightCycle()
    {
        while (true)
        {
            // GREEN
            SetLights(dimRed, dimYellow, brightGreen);
            yield return new WaitForSeconds(greenDuration);

            // YELLOW
            SetLights(dimRed, brightYellow, dimGreen);
            yield return new WaitForSeconds(yellowDuration);

            // RED
            SetLights(brightRed, dimYellow, dimGreen);
            yield return new WaitForSeconds(redDuration);
        }
    }

    private void SetLights(Color r, Color y, Color g)
    {
        if (redLight != null) redLight.material.color = r;
        if (yellowLight != null) yellowLight.material.color = y;
        if (greenLight != null) greenLight.material.color = g;

        // Make active light emissive for glow effect
        if (redLight != null)
        {
            redLight.material.EnableKeyword("_EMISSION");
            redLight.material.SetColor("_EmissionColor", r * 2f);
        }
        if (yellowLight != null)
        {
            yellowLight.material.EnableKeyword("_EMISSION");
            yellowLight.material.SetColor("_EmissionColor", y * 2f);
        }
        if (greenLight != null)
        {
            greenLight.material.EnableKeyword("_EMISSION");
            greenLight.material.SetColor("_EmissionColor", g * 2f);
        }
    }
}
