using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Phase 10: Mission Matrix UI.
/// A scrollable list showing all 50 missions in the Story Matrix.
/// Proves the mission system is real and functional.
/// </summary>
public class MissionMatrixUI : MonoBehaviour
{
    public static MissionMatrixUI Instance { get; private set; }

    [Header("UI References")]
    public GameObject uiPanel;
    public Transform listRoot;
    public GameObject missionItemPrefab;

    void Awake()
    {
        if (Instance == null) Instance = this;
        uiPanel.SetActive(false);
    }

    public void ToggleMissionList()
    {
        uiPanel.SetActive(!uiPanel.activeSelf);
        if (uiPanel.activeSelf) RefreshList();
    }

    private void RefreshList()
    {
        // Clear old items
        foreach (Transform child in listRoot) Destroy(child.gameObject);

        if (MissionMatrixManager.Instance == null) return;

        // Load all 50 missions
        foreach (var mission in MissionMatrixManager.Instance.allMissions)
        {
            CreateMissionItem(mission);
        }
    }

    private void CreateMissionItem(MissionData data)
    {
        GameObject item = Instantiate(missionItemPrefab, listRoot);
        Text titleText = item.transform.Find("Title").GetComponent<Text>();
        Text rewardText = item.transform.Find("Reward").GetComponent<Text>();
        Image bgImage = item.GetComponent<Image>();
        Button playButton = item.GetComponentInChildren<Button>();

        titleText.text = data.title.ToUpper();
        rewardText.text = $"REWARD: ${data.rewardMoney}";

        // Show Art if assigned
        if (data.missionArt != null && bgImage != null) bgImage.sprite = data.missionArt;

        // Check Unlock State
        bool unlocked = data.isUnlocked || PlayerPrefs.GetInt($"Mission_{data.missionID}_Unlocked", 0) == 1;
        if (data.missionID == "M1") unlocked = true; // First mission always unlocked

        if (!unlocked)
        {
            titleText.text += " (LOCKED)";
            playButton.interactable = false;
        }

        playButton.onClick.AddListener(() => {
            if (CinematicAudioManager.Instance != null) CinematicAudioManager.Instance.PlayCinematicSound("UI_Confirm", 1f);
            MissionManager.Instance.StartMission(data.missionID);
            uiPanel.SetActive(false);
        });
    }
}
