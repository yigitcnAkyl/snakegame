using System;
using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    private TextMeshProUGUI scoreText;

    private void Awake()
    {
        scoreText = GetComponent<TextMeshProUGUI>();
        Score.ResetScore();
        UpdateScoreText();
    }

    private void Start()
    {
        Snake snake = FindFirstObjectByType<Snake>();
        if (snake != null)
        {
            snake.OnAteFood += Snake_OnAteFood;
            snake.OnDied += Snake_OnDied;
        }
    }

    private void Snake_OnAteFood(object sender, EventArgs e)
    {
        Score.AddScore(100);
        UpdateScoreText();
    }

    private void Snake_OnDied(object sender, EventArgs e)
    {
        gameObject.SetActive(false);
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = Score.GetScore().ToString();
        }
    }
}