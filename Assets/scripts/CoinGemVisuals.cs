using UnityEngine;

/// <summary>
/// Phase 12: Coin & Gem Visual Refiner.
/// Attaches to world-space currency pickups to make them rotate and glow.
/// </summary>
public class CoinGemVisuals : MonoBehaviour
{
    public bool isGem = false;
    public float rotationSpeed = 100f;
    public float bobSpeed = 1f;
    public float bobAmount = 0.2f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
        // Apply Material Color based on type
        Renderer r = GetComponent<Renderer>();
        if (r != null)
        {
            r.material.color = isGem ? Color.cyan : Color.yellow;
            r.material.EnableKeyword("_EMISSION");
            r.material.SetColor("_EmissionColor", isGem ? Color.cyan * 1.5f : Color.yellow * 1.5f);
        }
    }

    void Update()
    {
        // 1. Rotation
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        // 2. Bobbing movement
        float newY = startPos.y + Mathf.Sin(Time.time * bobSpeed) * bobAmount;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (isGem) CurrencyManager.Instance.AddGems(1);
            else CurrencyManager.Instance.AddCoins(10);
            
            // Phase 12: Visual burst
            if (LuxuryHUDManager.Instance != null)
                LuxuryHUDManager.Instance.TriggerCoinBurst(Camera.main.WorldToScreenPoint(transform.position));

            Destroy(gameObject);
        }
    }
}
