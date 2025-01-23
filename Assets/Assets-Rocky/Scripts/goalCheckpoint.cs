using UnityEngine;

public class goalCheckpoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Check if object entering collider is player
        if (other.CompareTag("Player"))
        {
            // Access Game Manager instance and update goalCheckpoint
            if (GameManager.instance != null)
            {
                GameManager.instance.goalCheckpoint++;
                GameManager.instance.updateGameWinCondition(0);
                Debug.Log("Goal checkpoint has been reached! Current goal count: " +  GameManager.instance.goalCheckpoint);
            }
            else
            {
                Debug.LogError("Game Manager instance is null");
            }
        }
    }
}
