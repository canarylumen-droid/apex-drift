using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Phase 15: Crowd Manager.
/// Handles high-density NPC spawning for events.
/// </summary>
public class CrowdManager : MonoBehaviour
{
    public static CrowdManager Instance { get; private set; }

    public GameObject npcPrefab; // Realistic human prefab from Resources
    public int maxCrowdSize = 50;

    private List<GameObject> crowdMembers = new List<GameObject>();

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    /// <summary>
    /// Spawns a cluster of NPCs for a specific social event.
    /// </summary>
    public void SpawnEventCrowd(Vector3 center, float radius, int size)
    {
        if (RealisticModelManager.Instance == null || RealisticModelManager.Instance.humanIdlePrefab == null) return;

        int actualSize = Mathf.Min(size, maxCrowdSize);
        for (int i = 0; i < actualSize; i++)
        {
            Vector2 randomPoint = Random.insideUnitCircle * radius;
            Vector3 spawnPos = center + new Vector3(randomPoint.x, 0, randomPoint.y);
            
            GameObject npc = Instantiate(RealisticModelManager.Instance.humanIdlePrefab, spawnPos, Quaternion.identity);
            SocialNPC social = npc.AddComponent<SocialNPC>();
            social.SetSocialState(SocialNPC.SocialState.Idle);
            
            crowdMembers.Add(npc);
        }
    }

    public void ClearCrowd()
    {
        foreach (var npc in crowdMembers) if (npc != null) Destroy(npc);
        crowdMembers.Clear();
    }
}
