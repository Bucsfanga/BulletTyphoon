using UnityEngine;
using System.Collections;

public class lightningCloudSpawner : MonoBehaviour
{
    [SerializeField] GameObject lightningCloud;
    [SerializeField] float cloudSpawnInterval, spawnRadius, initialDelay, spawnHeight;
    [SerializeField] bool isTargetingPlayer;
    [SerializeField] weatherController weatherController;

    private Transform player;
    private lightningCloudStrike cloudStrikeInstance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameManager.instance.player.transform;
        if (player == null)
        {
            //Debug.LogError("Player reference is null");
            return;
        }

        setSpawnHeight();
        StartCoroutine(spawnLightningCloud());
    }

    IEnumerator spawnLightningCloud()
    {
        yield return new WaitForSeconds(initialDelay);

        while (true)
        {
            Vector3 spawnPosition = isTargetingPlayer ? player.position + Vector3.up * spawnHeight
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
        if (player == null)
        {
            //Debug.LogError("Player reference is null for random spawn position.");
            return Vector3.zero;
        }

        // Generate a random point within spawn radius of player
        float angle = Random.Range(0f, Mathf.PI * 2f); // Random angle in radians
        float distance = Random.Range(0f, spawnRadius); // Random distance within spawn radius
        float randomX = player.position.x + Mathf.Cos(angle) * distance;
        float randomZ = player.position.z + Mathf.Sin(angle) * distance;

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

    float setSpawnHeight()
    {
        if (spawnHeight < 50f)
        {
            float newSpawnHeight = 50f;
            spawnHeight = newSpawnHeight;
            return spawnHeight;
        }

        return spawnHeight;
    }
}
