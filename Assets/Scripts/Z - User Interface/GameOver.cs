using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameOver : MonoBehaviour
{
    public TextMeshProUGUI currentScoreText;
    public TextMeshProUGUI highScoreText;
    public GameManager gameManager;

    int highScore;
    int currentScore;

    /// <summary>Formats to high score</summary>
    public string FormatScore(int score)
    {
        int stage = 1 + (score / 10);
        return ("S" + stage + "-" + "L" + score).ToString();
    }

    // Set score when visible
    public void UpdateGameOverScore()
    {
        highScore = PlayerPrefs.GetInt("HighScore");
        currentScore = gameManager.currentScore;
        // high score section;
        // move this to when the player dies or quits!
        // replace this with some UI magic!
        if (currentScore > highScore)
        {
            highScore = currentScore;
            PlayerPrefs.SetInt("HighScore", highScore);
        }

        // Updates Game over UI text
        currentScoreText.text = FormatScore(gameManager.currentScore);
        highScoreText.text = "High Score: " + FormatScore(PlayerPrefs.GetInt("HighScore"));
    }

}
