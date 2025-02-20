using UnityEngine;
using TMPro;
using System.Collections.Generic;
public class HighScoreUI : MonoBehaviour
{
    public TextMeshProUGUI bestScoreText;

    void Start()
    {
        UpdateBestScore();
    }
    void Update()
    {
        UpdateBestScore(); // Refresh high scores continuously
    }
    public void UpdateBestScore()
    {
        float bestTime = PlayerPrefs.GetFloat("HighScore", float.MaxValue);

        if (bestTime == float.MaxValue)
        {
            bestScoreText.text = "Best Time: N/A";
        }
        else
        {
            bestScoreText.text = $"Best Time: {bestTime:F2} sec";
        }
    }
}
