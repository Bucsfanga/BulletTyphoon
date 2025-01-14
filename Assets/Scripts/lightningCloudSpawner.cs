using UnityEngine;
using System.Collections;

public class lightningCloudSpawner : MonoBehaviour
{
    [SerializeField] GameObject lightningCloud;
    [SerializeField] float cloudSpawnInterval;
    [SerializeField] float initialDelay;
    [SerializeField] bool isTargetingPlayer;

    private Transform player;
    private lightningCloudStrike cloudStrikeInstance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameManager.instance.player.transform;
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
            Vector3 spawnPosition = isTargetingPlayer ? player.position + Vector3.up * 50f
                : getRandomSpawnPosition();

            GameObject cloud = Instantiate(lightningCloud, spawnPosition, Quaternion.identity);

            lightningCloudStrike cloudScript = cloud.GetComponent<lightningCloudStrike>();
            if (cloudScript != null)
            {
                cloudScript.IsTargetingPlayer = isTargetingPlayer;
                cloudScript.GroundMin = calculateGroundMin();
                cloudScript.GroundMax = calculateGroundMax();
            }

            yield return new WaitForSeconds (cloudSpawnInterval);
        }
    }

    Vector3 getRandomSpawnPosition()
    {
        Vector3 groundMin = calculateGroundMin();
        Vector3 groundMax = calculateGroundMax();

        float randomX = Random.Range(groundMin.x, groundMax.x);
        float randomZ = Random.Range(groundMin.z, groundMax.z);
        float spawnHeight = 50f; // Fixed height above ground

        return new Vector3(randomX, spawnHeight, randomZ);
    }

    Vector3 calculateGroundMin()
    {
        GameObject ground = lightningCloud.GetComponent<lightningCloudStrike> ().Ground;
        Transform groundTransform = ground.transform;

        float halfWidth = groundTransform.localScale.x * 5f;
        float halfLength = groundTransform.localScale.z * 5f;

        return new Vector3(groundTransform.position.x - halfWidth, groundTransform.position.y,
            groundTransform.position.z - halfLength);
    }

    Vector3 calculateGroundMax()
    {
        GameObject ground = lightningCloud.GetComponent<lightningCloudStrike>().Ground;
        Transform groundTransform = ground.transform;

        float halfWidth = groundTransform.localScale.x * 5f;
        float halfLength = groundTransform.localScale.z * 5f;

        return new Vector3(groundTransform.position.x + halfWidth, groundTransform.position.y,
            groundTransform.position.z + halfLength);
    }
}
