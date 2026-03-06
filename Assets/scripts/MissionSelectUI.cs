using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Phase 14: Mission Select UI.
/// Premium scrolling list for choosing Story Missions.
/// </summary>
public class MissionSelectUI : MonoBehaviour
{
    public static MissionSelectUI Instance { get; private set; }

    [Header("Gallery Settings")]
    public float scrollSpeed = 10f;
    public RectTransform galleryContainer;
    private int selectedIndex = 0;
    private Vector2 targetPos;

    void Awake()
    {
        if (Instance == null) Instance = this;
        CreateUIResources();
    }

    void Update()
    {
        // Smooth horizontal scrolling for the gallery
        if (galleryContainer != null)
        {
            float targetX = -selectedIndex * 800f; // Assume 800px width per item
            targetPos = new Vector2(targetX, galleryContainer.anchoredPosition.y);
            galleryContainer.anchoredPosition = Vector2.Lerp(galleryContainer.anchoredPosition, targetPos, Time.deltaTime * scrollSpeed);
        }

        // PC Controls: Left/Right arrows or A/D
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) ScrollGallery(-1);
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) ScrollGallery(1);
    }

    public void ScrollGallery(int direction)
    {
        int totalItems = (MissionController.Instance != null) ? MissionController.Instance.availableMissions.Count : 5;
        selectedIndex = Mathf.Clamp(selectedIndex + direction, 0, totalItems - 1);
        
        if (CinematicAudioManager.Instance != null)
            CinematicAudioManager.Instance.PlayCinematicSound("UI_Swipe", 0.5f);
    }

    public void ShowMissionSelector()
    {
        if (missionPanel != null) missionPanel.SetActive(true);
        PopulateMissions();
    }

    private void PopulateMissions()
    {
        if (MissionController.Instance == null) return;

        // Clear existing
        foreach (Transform child in missionListContent) Destroy(child.gameObject);

        int index = 0;
        string[] previewModels = { "Protagonist_HighPoly", "VIP_Client_Gold", "SWAT_Tactical_Lead", "Gangster_Boss_Elite", "Club_Dancer_Neon_A" };

        foreach (var mission in MissionController.Instance.availableMissions)
        {
            GameObject item = Instantiate(missionItemPrefab, missionListContent);
            
            // Phase 21: Bind unique high-poly Unreal model per mission
            string modelName = previewModels[index % previewModels.Length];
            AssetAutoBinder.Instance.BindHighPolyModel(item, modelName); 

            item.GetComponentInChildren<Text>().text = mission.title.ToUpper();
            int thisIndex = index;
            item.GetComponent<Button>().onClick.AddListener(() => {
                SelectMission(thisIndex);
            });
            index++;
        }
    }

    public void SelectMission(int index)
    {
        Debug.Log("[MillionDollarUI] Starting Mission: " + index);
        MissionController.Instance.StartMission(index);
        HideMissionSelector();
    }

    public void HideMissionSelector()
    {
        if (missionPanel != null) missionPanel.SetActive(false);
    }
}
