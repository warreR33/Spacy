using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;

public static class GameStats
{
    public static int LastScore { get; set; }
}

public class DefeatHUDController : MonoBehaviour
{
    public TMP_Text currentScoreText;
    public TMP_Text highScoreText;


    void Start()
    {
        int score = GameStats.LastScore;
        currentScoreText.text = $"Score: \n{score}";
        LoadHighScore();
    }

    async void LoadHighScore()
    {
        int highScore = await CloudSaveSystem.LoadHighScoreAsync();
        highScoreText.text = $"HighScore: \n{highScore}";
    }
}
