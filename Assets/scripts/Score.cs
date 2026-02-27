using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    public static Score Instance { get; private set; }

    public int score;
    public Text textLabel;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        score = PlayerPrefs.GetInt("score", 2900);
        UpdateLabel();
    }

    public void AddPoints(int points)
    {
        score += points;
        PlayerPrefs.SetInt("score", score);
        PlayerPrefs.Save();
        UpdateLabel();
    }

    public void getscore()
    {
        score = PlayerPrefs.GetInt("score", 2900);
        UpdateLabel();
    }

    void UpdateLabel()
    {
        if (textLabel != null)
            textLabel.text = "Apex Drift Score: " + score.ToString("N0");
    }
}

