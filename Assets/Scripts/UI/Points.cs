using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Points : MonoBehaviour
{
    public static Points Instance { get; private set; }

    public int score = 0;
    public TMP_Text scoreText;

    public int pointsPerSecond = 1;
    private float timer = 0f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        UpdateScoreUI();
    }

    void Update()
    {
        if (GameProgressManager.Instance != null && GameProgressManager.Instance.isInTutorial)
        return;

        timer += Time.deltaTime;
        if (timer >= 1f)
        {
            AddPoints(pointsPerSecond);
            timer = 0f;
        }
    }

    public void AddPoints(int amount)
    {
        score += amount;
        UpdateScoreUI();
        GameProgressManager.Instance?.CheckProgress(score);
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"{score}";
        }
    }

    public void ResetScore()
    {
        score = 0;
        UpdateScoreUI();
    }
    
}
