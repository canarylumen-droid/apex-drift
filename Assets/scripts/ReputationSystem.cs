using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Phase 6: Reputation System.
/// Tracks player reputation points and handles unlocking premium vehicles based on progression.
/// </summary>
public class ReputationSystem : MonoBehaviour
{
    public static ReputationSystem Instance { get; private set; }

    [Header("Reputation Settings")]
    public int currentReputation = 0;
    
    [System.Serializable]
    public class UnlockableVehicle
    {
        public string vehicleName;
        public int requiredReputation;
        public bool isUnlocked;
    }

    public List<UnlockableVehicle> progressionUnlocks = new List<UnlockableVehicle>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        InitializeUnlocks();
    }

    private void InitializeUnlocks()
    {
        progressionUnlocks.Add(new UnlockableVehicle { vehicleName = "Cybertruck", requiredReputation = 500, isUnlocked = false });
        progressionUnlocks.Add(new UnlockableVehicle { vehicleName = "Bugatti", requiredReputation = 2000, isUnlocked = false });
    }

    public void AddReputation(int amount)
    {
        currentReputation += amount;
        Debug.Log("[Reputation] Earned " + amount + " rep! Total: " + currentReputation);
        CheckForUnlocks();
    }

    private void CheckForUnlocks()
    {
        foreach (var vehicle in progressionUnlocks)
        {
            if (!vehicle.isUnlocked && currentReputation >= vehicle.requiredReputation)
            {
                vehicle.isUnlocked = true;
                Debug.Log("[Reputation] UNLOCKED: " + vehicle.vehicleName + "!");
                
                // Trigger global HUD notification
                if (PremiumHUD.Instance != null && PremiumHUD.Instance.notificationText != null)
                {
                    PremiumHUD.Instance.ShowNotification("UNLOCKED: " + vehicle.vehicleName.ToUpper());
                }
            }
        }
    }
}
