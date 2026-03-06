using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Phase 5: Dynamic Mission Icons.
/// Handles HUD icons that track targets on the map with rotation and smooth movement.
/// </summary>
public class MissionIconManager : MonoBehaviour
{
    public static MissionIconManager Instance { get; private set; }

    [Header("Icon Settings")]
    public RectTransform missionIconPrefab;
    public Transform playerTransform;

    private Dictionary<Transform, RectTransform> activeIcons = new Dictionary<Transform, RectTransform>();

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void TrackTarget(Transform target, Sprite iconSprite)
    {
        if (activeIcons.ContainsKey(target)) return;

        GameObject iconObj = new GameObject("MissionIcon_" + target.name);
        iconObj.transform.SetParent(PremiumHUD.Instance.transform);
        RectTransform rt = iconObj.AddComponent<RectTransform>();
        Image img = iconObj.AddComponent<Image>();
        img.sprite = iconSprite;
        rt.sizeDelta = new Vector2(40, 40);

        activeIcons.Add(target, rt);
    }

    void Update()
    {
        if (playerTransform == null && Camera.main != null) playerTransform = Camera.main.transform;
        if (playerTransform == null) return;

        foreach (var pair in activeIcons)
        {
            UpdateIconPosition(pair.Key, pair.Value);
        }
    }

    private void UpdateIconPosition(Transform target, RectTransform iconRT)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(target.position);
        
        // If target is behind camera
        if (screenPos.z < 0)
        {
            iconRT.gameObject.SetActive(false);
            return;
        }

        iconRT.gameObject.SetActive(true);
        iconRT.position = screenPos;

        // Rotation logic: Point the icon towards the target relative to player heading
        Vector3 targetDir = target.position - playerTransform.position;
        float angle = Mathf.Atan2(targetDir.x, targetDir.z) * Mathf.Rad2Deg;
        iconRT.rotation = Quaternion.Euler(0, 0, -angle + playerTransform.eulerAngles.y);
    }

    public void StopTracking(Transform target)
    {
        if (activeIcons.ContainsKey(target))
        {
            Destroy(activeIcons[target].gameObject);
            activeIcons.Remove(target);
        }
    }
}
