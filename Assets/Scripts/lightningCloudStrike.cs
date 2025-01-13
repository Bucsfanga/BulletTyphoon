using System.Collections;
using UnityEngine;

public class lightningCloudStrike : MonoBehaviour
{
    public bool IsTargetingPlayer { get; set; }
    [SerializeField] int numberOfStrikes;
    [SerializeField] float strikeInterval;
    [SerializeField] float strikeRadius;
    [SerializeField] GameObject lightningPrefab;
    [SerializeField] GameObject strikeIndicatorPrefab;
    [SerializeField] GameObject ground;

    private Transform player;
    private int activeStrikes;
    private Vector3 groundMin, groundMax;

    public float StrikeRadius => strikeRadius; // Public getter for strike radius
    public Vector3 GroundMin
    {
        get => groundMin;
        set => groundMin = value;
    }
    public Vector3 GroundMax
    {
        get => groundMax;
        set => groundMax = value;
    }

    public GameObject Ground => ground;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = gameManager.instance.player.transform;
        if (ground == null)
        {
            Debug.LogError("Map boundary reference is null");
            return;
        }

        Transform groundTransform = ground.transform;

        float halfWidth = groundTransform.localScale.x * 5f;
        float halfLength = groundTransform.localScale.z * 5f;
        groundMin = new Vector3(groundTransform.position.x - halfWidth, groundTransform.position.y, groundTransform.position.z - halfLength);
        groundMax = new Vector3(groundTransform.position.x + halfWidth, groundTransform.position.y, groundTransform.position.z + halfLength);

        StartCoroutine(strikeSequence());
    }

    IEnumerator strikeSequence()
    {
        // Determine the strike position once, directly below the cloud's center
        Vector3 strikePosition = getStrikePosition(transform.position);
        Vector3 cloudPosition = transform.position;

        // Spawn strike indicator
        GameObject indicator = Instantiate(strikeIndicatorPrefab, strikePosition, Quaternion.identity);
        indicator.transform.localScale = new Vector3(strikeRadius * 2, 0.1f, strikeRadius * 2);

        for (int i = 0; i < numberOfStrikes; i++)
        {
            // Spawn lightning bolt at the cloud's position
            GameObject lightningBolt = Instantiate(lightningPrefab, cloudPosition, Quaternion.identity);
            activeStrikes++;

            // Initialize damageRadius in Damage script
            Damage damageScript = lightningBolt.GetComponent<Damage>();
            if (damageScript != null)
            {
                damageScript.setDamageRadius(StrikeRadius);
            }

            // Adjust lightning bolt's collider radius to match strike radius
            SphereCollider collider = lightningBolt.GetComponent<SphereCollider>();
            if (collider != null)
            {
                collider.radius = strikeRadius * 5;
            }

            // Attach indicator to lightning bolt
            StartCoroutine(destroyBoltIfObstructed(lightningBolt, strikePosition, indicator));

            // Wait before spawning the next bolt
            yield return new WaitForSeconds(strikeInterval);
        }

        // Wait for all active strikes to finish before destroying the cloud
        while (activeStrikes > 0)
        {
            yield return null;
        }

        if (indicator != null)
        {
            Destroy(indicator);
        }

        Destroy(gameObject);
    }

    Vector3 getStrikePosition(Vector3 origin)
    {
        // Drop downward ray cast above player to detect surface
        RaycastHit hit;
        Vector3 rayOrigin = origin + Vector3.up * 50;

        int layerMask = LayerMask.GetMask("Default", "Structure", "Ground");

        if (Physics.Raycast(rayOrigin, Vector3.down, out hit, Mathf.Infinity, layerMask))
        {
            if (hit.collider != null)
            {
                return hit.point; // Return point of contact with surface
            }
        }

        return new Vector3(origin.x, ground.transform.position.y, origin.z); // Place on ground by default
    }

    IEnumerator destroyBoltIfObstructed(GameObject lightningBolt, Vector3 strikePosition, GameObject indicator)
    {
        while (lightningBolt != null)
        {
            // Destroy the bolt if it falls below or at the indicator's Y position
            if (lightningBolt.transform.position.y <= indicator.transform.position.y)
            {
                Destroy(lightningBolt);

                // Destroy the associated indicator
                if (activeStrikes == 0 && indicator != null)
                {
                    Destroy(indicator);
                }

                activeStrikes--;
                yield break;
            }

            yield return null;
        }

        // If the bolt is destroyed elsewhere, destroy the associated indicator
        if (activeStrikes == 0 && indicator != null)
        {
            Destroy(indicator);
        }

        activeStrikes--;
    }
}
