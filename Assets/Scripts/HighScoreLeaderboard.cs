using UnityEngine;
using TMPro;
using System.Collections.Generic;
public class HighScoreLeaderboard : MonoBehaviour
{
    public TextMeshProUGUI leaderboardText;
    public GameObject leaderboardPanel; 

    void Start()
    {
        leaderboardPanel.SetActive(false); // Hide leaderboard until level ends
    }

    public void UpdateLeaderboard()
    {
        leaderboardPanel.SetActive(true);

        List<float> highScores = GameManager.instance.LoadHighScores();

        if (highScores.Count == 0)
        {
            leaderboardText.text = "No High Scores Yet";
            return;
        }

        string scoreText = "Top 10 Speed Run Scores:\n";
        for (int i = 0; i < highScores.Count; i++)
        {
            scoreText += $"{i + 1}. {highScores[i]:F2} sec\n";
        }

        leaderboardText.text = scoreText;
    }

}
