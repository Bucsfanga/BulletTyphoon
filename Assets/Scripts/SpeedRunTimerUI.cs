using TMPro;
using UnityEngine;

public class SpeedRunTimerUI : MonoBehaviour
{
    public TextMeshProUGUI timerText;

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance != null && GameManager.instance.isTimerRunning)
        {
            float elapsedTime = Time.time - GameManager.instance.levelStartTime;
            timerText.text = $"Time: {elapsedTime:F2} sec";
        }
    }
}
