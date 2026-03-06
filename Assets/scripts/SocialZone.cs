using UnityEngine;

/// <summary>
/// Phase 10: Social Zone.
/// A trigger area that forces NPCs into specific social states (e.g., Club, Park, Protest).
/// </summary>
public class SocialZone : MonoBehaviour
{
    public enum ZoneType { Club, Park, ProtestArea, MakeOutCorner }
    
    [Header("Zone Configuration")]
    public ZoneType zoneType = ZoneType.Club;
    public float interactionBoost = 1.5f;
    public AudioClip backgroundAmbience;
    
    private AudioSource ambienceSource;

    void Start()
    {
        if (HighlifeSocialManager.Instance != null)
            HighlifeSocialManager.Instance.RegisterZone(this);

        if (backgroundAmbience != null)
        {
            ambienceSource = gameObject.AddComponent<AudioSource>();
            ambienceSource.clip = backgroundAmbience;
            ambienceSource.loop = true;
            ambienceSource.spatialBlend = 1.0f;
            ambienceSource.Play();
        }

        // Add a trigger if not present
        if (GetComponent<Collider>() == null)
        {
            SphereCollider sc = gameObject.AddComponent<SphereCollider>();
            sc.isTrigger = true;
            sc.radius = 10f;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        SocialNPC npc = other.GetComponent<SocialNPC>();
        if (npc != null)
        {
            ApplyZoneBehavior(npc);
        }
    }

    private void ApplyZoneBehavior(SocialNPC npc)
    {
        switch (zoneType)
        {
            case ZoneType.Club:
                npc.SetSocialState(SocialNPC.SocialState.Dancing);
                break;
            case ZoneType.MakeOutCorner:
                npc.SetSocialState(SocialNPC.SocialState.MatureInteraction);
                break;
            case ZoneType.ProtestArea:
                npc.SetSocialState(SocialNPC.SocialState.Protesting);
                break;
        }
    }

    void OnDestroy()
    {
        if (HighlifeSocialManager.Instance != null)
            HighlifeSocialManager.Instance.UnregisterZone(this);
    }
}
