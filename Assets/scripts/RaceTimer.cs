using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Race Timer for Apex Drift: 3D Racing.
/// Tracks current lap time, total race time, and best times per track.
/// Attach to a UI object in the race scene.
/// </summary>
public class RaceTimer : MonoBehaviour
{
    public static RaceTimer Instance { get; private set; }

    [Header("UI Elements")]
    public Text currentTimeLabel;
    public Text lapTimeLabel;
    public Text bestTimeLabel;
    public Text newBestLabel; // Shows "NEW BEST!" text

    // Timing
    private float raceTime = 0f;
    private float lapTime = 0f;
    private float[] lapTimes = new float[3]; // Store each lap time
    private int currentLap = 1;
    private bool isRunning = false;

    // Best time for current track
    private float bestTime = 0f;
    private string bestTimeKey = "";

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // Generate unique key for this track
        bestTimeKey = "best_time_scene_" + UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        bestTime = PlayerPrefs.GetFloat(bestTimeKey, 0f);

        if (bestTimeLabel != null)
        {
            if (bestTime > 0)
                bestTimeLabel.text = "Best: " + FormatTime(bestTime);
            else
                bestTimeLabel.text = "Best: --:--";
        }

        if (newBestLabel != null)
            newBestLabel.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!isRunning) return;

        raceTime += Time.deltaTime;
        lapTime += Time.deltaTime;

        // Update UI
        if (currentTimeLabel != null)
            currentTimeLabel.text = FormatTime(raceTime);

        if (lapTimeLabel != null)
            lapTimeLabel.text = "Lap: " + FormatTime(lapTime);
    }

    // ===== CONTROL =====

    /// <summary>
    /// Start the timer. Call after "GO!" in gopannel.cs
    /// </summary>
    public void StartTimer()
    {
        isRunning = true;
        raceTime = 0f;
        lapTime = 0f;
        currentLap = 1;
    }

    /// <summary>
    /// Call when a lap is completed (from obj_tracker.cs)
    /// </summary>
    public void OnLapComplete(int lapNumber)
    {
        if (lapNumber > 0 && lapNumber <= lapTimes.Length)
            lapTimes[lapNumber - 1] = lapTime;

        lapTime = 0f;
        currentLap = lapNumber + 1;
    }

    /// <summary>
    /// Stop timer and check for best time. Call at race end.
    /// </summary>
    public void StopTimer()
    {
        isRunning = false;

        // Check for new best time
        if (bestTime <= 0 || raceTime < bestTime)
        {
            bestTime = raceTime;
            PlayerPrefs.SetFloat(bestTimeKey, bestTime);
            PlayerPrefs.Save();

            // Show "NEW BEST!" celebration
            if (newBestLabel != null)
            {
                newBestLabel.gameObject.SetActive(true);
                newBestLabel.text = "NEW BEST! " + FormatTime(bestTime);
            }

            if (bestTimeLabel != null)
                bestTimeLabel.text = "Best: " + FormatTime(bestTime);
        }
    }

    public void PauseTimer() { isRunning = false; }
    public void ResumeTimer() { isRunning = true; }

    public float GetRaceTime() { return raceTime; }
    public float GetLapTime() { return lapTime; }
    public float GetBestTime() { return bestTime; }

    // ===== FORMAT =====

    public static string FormatTime(float time)
    {
        int minutes = (int)(time / 60f);
        int seconds = (int)(time % 60f);
        int milliseconds = (int)((time * 100f) % 100f);
        return string.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, milliseconds);
    }
}
