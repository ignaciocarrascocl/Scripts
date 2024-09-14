// ScoreManager.cs
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [Header("Score Settings")]
    public int scoreGood = 100;
    public int scoreGreat = 200;
    public int scoreMeh = 50;
    public float streakMultiplier = 1.5f;

    [Header("Ranges")]
    public float greatRange = 0.1f;
    public float goodRange = 0.3f;
    public float mehRange = 0.5f;

    [Header("UI Elements")]
    public TextMeshProUGUI scoreText;

    [Header("Feedback Prefabs")]
    public GameObject particleMiss;
    public GameObject particleMeh;
    public GameObject particleGood;
    public GameObject particleGreat;

    [Header("Player Reference")]
    public Transform playerTransform;

    private int totalScore = 0;
    private int currentStreak = 0;

    public static ScoreManager instance;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if(playerTransform == null)
        {
            Debug.LogError("Player Transform not assigned in ScoreManager.");
            return;
        }

        UpdateScoreUI();
    }

    public void RegisterMiss(Vector3 position)
    {
        currentStreak = 0;
        SpawnFeedback(particleMiss, position);
        UpdateScoreUI();
    }

    public void RegisterHit(string hitType, Vector3 position, float accuracy)
    {
        int scoreToAdd = 0;
        GameObject feedback = null;

        switch(hitType.ToLower())
        {
            case "great":
                currentStreak++;
                if(currentStreak > 2)
                {
                    scoreToAdd = Mathf.RoundToInt(scoreGreat * streakMultiplier);
                }
                else
                {
                    scoreToAdd = scoreGreat;
                }
                feedback = particleGreat;
                break;
            case "good":
                scoreToAdd = scoreGood;
                currentStreak = 0;
                feedback = particleGood;
                break;
            case "meh":
                scoreToAdd = scoreMeh;
                currentStreak = 0;
                feedback = particleMeh;
                break;
        }

        totalScore += scoreToAdd;
        SpawnFeedback(feedback, position);
        UpdateScoreUI();
    }

    private void SpawnFeedback(GameObject feedbackPrefab, Vector3 position)
    {
        if(feedbackPrefab != null)
        {
            Instantiate(feedbackPrefab, position, Quaternion.identity);
        }
    }

    private void UpdateScoreUI()
    {
        if(scoreText != null)
        {
            scoreText.text = $"Score: {totalScore}\nStreak: {currentStreak}";
        }
    }
}
