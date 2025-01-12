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

    public float StrikeRadius => strikeRadius; // Public getter for strike radius


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
            Vector3 cloudPosition = playerPosition + Vector3.up * 40f; // Cloud above player
            Vector3 strikePosition = getStrikePosition(playerPosition);

            // Spawn strike indicator           
            GameObject indicator = Instantiate(strikeIndicatorPrefab, strikePosition, Quaternion.identity);
            indicator.transform.localScale = new Vector3(strikeRadius * 2, 0.1f, strikeRadius * 2);

            // Spawn lightning  bolt
            GameObject lightningBolt = Instantiate(lightningPrefab, cloudPosition, Quaternion.identity);
            activeStrikes++;

            // Initialize damageRadius in Damage script
            Damage damageScript = lightningBolt.GetComponent<Damage>();
            if (damageScript != null )
            {
                damageScript.setDamageRadius(StrikeRadius); // Pass strike radius to damage radius
            }

            // Adjust lightning bolt's collider radius to match strike radius
            SphereCollider collider = lightningBolt.GetComponent<SphereCollider>();
            if (collider != null)
            {
                collider.radius = strikeRadius * 5;
            }

            // Attach indicator to lightning bolt
            StartCoroutine(destroyBoltIfObstructed(lightningBolt, strikePosition, indicator));

            // Wait before next strike
            yield return new WaitForSeconds(strikeInterval);
        }

        while( activeStrikes > 0)
        {
            yield return null;
        }

        Destroy(gameObject); // Destroy cloud once strikes are exhausted
    }

    Vector3 getStrikePosition(Vector3 origin)
    {
        // Drop downward ray cast above player to detect surface
        RaycastHit hit;
        Vector3 rayOrigin = origin + Vector3.up * 50;

        int layerMask = LayerMask.GetMask("Default", "Structure");

        if (Physics.Raycast(rayOrigin, Vector3.down, out hit, Mathf.Infinity, layerMask))
        {
            return hit.point; // Return point of contact with surface
        }

        return origin; // If no obstruction target player
    }

    IEnumerator destroyBoltIfObstructed(GameObject lightningBolt, Vector3 strikePosition, GameObject indicator)
    {
        RaycastHit hit;
        Vector3 lightningStartPosition = lightningBolt.transform.position;

        // Use ray cast to detect obstruction
        if (Physics.Raycast(lightningStartPosition, (strikePosition - lightningStartPosition).normalized, out hit, Mathf.Infinity))
        {
            if (hit.collider is BoxCollider)// Destroy bolt on contact with box collider
            {
                // Update indicator to spawn at obstruction point
                indicator.transform.position = hit.point;

                // Stop lightning bolt from visually passing through the structure
                lightningBolt.transform.position = hit.point;

                Destroy(lightningBolt);

                if (indicator != null)
                {
                    Destroy(indicator);
                }

                activeStrikes--;
                yield break;
            }
        }

        // Wait for lightning to be destroyed naturally
        while (lightningBolt != null)
        {
            if (lightningBolt.transform.position.y < indicator.transform.position.y)
            {
                Destroy(lightningBolt);

                if (indicator != null)
                {
                    Destroy(indicator);
                }

                activeStrikes--;
                yield break;
            }

            yield return null;
        }

        if (indicator != null)
        {
            Destroy(indicator);
        }

        activeStrikes--;
    }

}
