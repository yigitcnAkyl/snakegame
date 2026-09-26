using UnityEngine;

public static class Score
{
    private const string HIGH_SCORE_KEY = "Snake_HighScore";
    private static int score;

    public static void ResetScore()
    {
        score = 0;
    }

    public static void AddScore(int amount = 100)
    {
        score += amount;
        if (score > GetHighScore())
        {
            PlayerPrefs.SetInt(HIGH_SCORE_KEY, score);
            PlayerPrefs.Save();
        }
    }

    public static int GetScore()
    {
        return score;
    }

    public static int GetHighScore()
    {
        return PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0);
    }
}
