using System.Collections;
using UnityEngine;

public class lightningCloudStrike : MonoBehaviour
{
    [SerializeField] int numberOfStrikes;
    [SerializeField] float strikeInterval;
    [SerializeField] float strikeRadius;
    [SerializeField] GameObject lightningPrefab;
    [SerializeField] GameObject strikeIndicatorPrefab;

    private Transform player;
    private int activeStrikes;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = gameManager.instance.player.transform;
        if (player == null)
        {
            Debug.LogError("player reference is null");
            Destroy(gameObject);
            return;
        }

        StartCoroutine(strikeSequence());
    }

    IEnumerator strikeSequence()
    {
        for (int i = 0; i < numberOfStrikes; i++)
        {
            Vector3 playerPosition = player.position;
            Vector3 cloudPosition = playerPosition + Vector3.up * 30f; // Cloud above player
            Vector3 indicatorPosition = new Vector3(playerPosition.x, 0.01f, playerPosition.z);

            // Spawn strike indicator           
            GameObject indicator = Instantiate(strikeIndicatorPrefab, indicatorPosition, Quaternion.identity);
            indicator.transform.localScale = new Vector3(strikeRadius * 2, 0.1f, strikeRadius * 2);

            // Spawn lightning  bolt
            GameObject lightningBolt = Instantiate(lightningPrefab, cloudPosition, Quaternion.identity);
            activeStrikes++;

            // Adjust lightning bolt's collider radius to match strike radius
            SphereCollider collider = lightningBolt.GetComponent<SphereCollider>();
            if (collider != null)
            {
                collider.radius = strikeRadius * 5;
            }

            // Attach indicator to lightning bolt
            StartCoroutine(destroyBoltWithIndicator(indicator, lightningBolt));

            // Wait before next strike
            yield return new WaitForSeconds(strikeInterval);
        }

        while( activeStrikes > 0)
        {
            yield return null;
        }

        Destroy(gameObject); // Destroy cloud once strikes are exhausted
    }

    IEnumerator destroyBoltWithIndicator(GameObject indicator, GameObject lightningBolt)
    {
        while (lightningBolt != null)
        {
            yield return null;
        }

        if (indicator != null)
        {
            Destroy(indicator);
        }

        activeStrikes--;
    }
}
