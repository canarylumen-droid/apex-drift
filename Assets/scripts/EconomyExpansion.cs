using UnityEngine;
using System.Collections;

/// <summary>
/// Phase 3: Money Spray (Make It Rain).
/// Spawns currency particles that physically fall and attract NPCs.
/// </summary>
public class EconomyExpansion : MonoBehaviour
{
    public static EconomyExpansion Instance { get; private set; }

    public GameObject moneyParticlePrefab; // Resources/Models/MoneyBill

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void MakeItRain()
    {
        Debug.Log("[Economy] Making it rain!");
        
        // 1. Spawn Bills
        for (int i = 0; i < 50; i++)
        {
            GameObject bill = GameObject.CreatePrimitive(PrimitiveType.Quad);
            bill.name = "DollarBill";
            bill.transform.position = transform.position + Vector3.up * 2f + Random.insideUnitSphere * 0.5f;
            bill.transform.localScale = new Vector3(0.1f, 0.05f, 1f);
            
            Rigidbody rb = bill.AddComponent<Rigidbody>();
            rb.mass = 0.01f;
            rb.drag = 5f; // Air resistance
            
            Renderer r = bill.GetComponent<Renderer>();
            r.material.color = Color.green;

            Destroy(bill, 10f);
        }

        // 2. Alert Nearby NPCs
        Collider[] hits = Physics.OverlapSphere(transform.position, 20f);
        foreach (var hit in hits)
        {
            SocialNPC npc = hit.GetComponent<SocialNPC>();
            if (npc != null)
            {
                npc.SetSocialState(SocialNPC.SocialState.Dancing); // Temporary "Excited" state
                if (AdvancedProceduralTTS.Instance != null)
                    AdvancedProceduralTTS.Instance.Speak("Oh my god, money!", AdvancedProceduralTTS.Emotion.Happy);
            }
        }
    }
}
