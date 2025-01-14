using UnityEngine;
using System.Collections;

public class lightningCloudSpawner : MonoBehaviour
{
    [SerializeField] GameObject lightningCloud;
    [SerializeField] float cloudSpawnInterval;
    [SerializeField] float initialDelay;

    private Transform player;
    private GameObject activeCloud;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = gameManager.instance.player.transform;
        if (player == null)
        {
            Debug.LogError("Player reference is null");
            return;
        }

        StartCoroutine(spawnLightningCloud());
    }



    IEnumerator spawnLightningCloud()
    {
        yield return new WaitForSeconds(initialDelay);

        while (true)
        {
            if (activeCloud == null)
            {

                // Spawn cloud at a random position above player
                Vector3 spawnPosition = player.position + Vector3.up * 20f;
                activeCloud = Instantiate(lightningCloud, spawnPosition, Quaternion.identity);

                while (activeCloud != null)
                {
                    yield return null;
                }

                // Wait for next cloud respawn
                yield return new WaitForSeconds(cloudSpawnInterval);
            }
            else
            {
                yield return null;
            }
        }
    }
}
