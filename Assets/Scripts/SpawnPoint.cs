using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public static SpawnPoint instance;
    public Transform spawnPosition;

    private void Awake()
    {
        instance = this;
        spawnPosition = transform; // Store spawn position
    }
}
