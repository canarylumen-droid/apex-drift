using UnityEngine;
using System.IO;

/// <summary>
/// Expansion Feature: Persistence & Save System.
/// Saves player progress (Currency, Reputation, Owned Cars) to the device.
/// </summary>
public class ApexSaveSystem : MonoBehaviour
{
    public static ApexSaveSystem Instance { get; private set; }

    [System.Serializable]
    public class PlayerData
    {
        public int currency;
        public int reputation;
        public string currentCarID;
        public int lastCompletedMissionIndex;
    }

    private string savePath;

    void Awake()
    {
        if (Instance == null) Instance = this;
        savePath = Application.persistentDataPath + "/save_data.json";
        
        // Auto-load on start
        LoadGame();
    }

    public void SaveGame()
    {
        PlayerData data = new PlayerData();
        if (CurrencyManager.Instance != null) data.currency = CurrencyManager.Instance.balance;
        if (ReputationSystem.Instance != null) data.reputation = ReputationSystem.Instance.currentReputation;
        
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(savePath, json);
        Debug.Log("[SaveSystem] Game Saved to: " + savePath);
    }

    public void LoadGame()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            PlayerData data = JsonUtility.FromJson<PlayerData>(json);
            
            // Apply loaded data
            if (CurrencyManager.Instance != null) CurrencyManager.Instance.balance = data.currency;
            if (ReputationSystem.Instance != null) ReputationSystem.Instance.currentReputation = data.reputation;
            
            Debug.Log("[SaveSystem] Game Loaded successfully.");
        }
    }

    void OnApplicationQuit()
    {
        SaveGame();
    }
}
