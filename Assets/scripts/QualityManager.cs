using UnityEngine;

/// <summary>
/// Phase 16: Quality & Mastering Manager.
/// Handles resolution scaling, 4K post-process switching, and mobile optimization.
/// </summary>
public class QualityManager : MonoBehaviour
{
    public static QualityManager Instance { get; private set; }

    public enum QualityMode { FastMobile, HighDef, Cinematic4K }
    public QualityMode currentMode = QualityMode.HighDef;

    void Awake()
    {
        if (Instance == null) Instance = this;
        ApplyQualitySettings();
    }

    public void SetQuality(QualityMode mode)
    {
        currentMode = mode;
        ApplyQualitySettings();
    }

    private void ApplyQualitySettings()
    {
        switch (currentMode)
        {
            case QualityMode.FastMobile:
                QualitySettings.SetQualityLevel(1);
                Application.targetFrameRate = 60;
                break;
            case QualityMode.HighDef:
                QualitySettings.SetQualityLevel(3);
                Application.targetFrameRate = 60;
                break;
            case QualityMode.Cinematic4K:
                QualitySettings.SetQualityLevel(5);
                Application.targetFrameRate = 120;
                // High-End specific boosts
                QualitySettings.anisotropicFiltering = AnisotropicFiltering.Enable;
                QualitySettings.shadowDistance = 200f;
                break;
        }
    }
}
