using UnityEngine;
using System.Collections;

public class RainManager : MonoBehaviour
{
    [Header("Rain Settings")]
    [SerializeField] private GameObject rainDropPrefab;
    [SerializeField] private int numberOfRaindrops = 1000;
    [SerializeField] private float spawnHeight = 20f;
    [SerializeField] private float spawnAreaWidth = 30f;
    [SerializeField] private float spawnAreaLength = 30f;
    [SerializeField] private float rainSpeed = 10f;
    [SerializeField] private float rainStartDelay = 3f;

    [Header("Collision Settings")]
    [SerializeField] private LayerMask ignoreCollisionLayers; // Set this in inspector to include layers

    [Header("Raindrop Scale")]
    [SerializeField] private Vector3 baseScale = new Vector3(0.04f, 0.2f, 0.04f);
    [SerializeField] private float scaleVariation = 0.2f;

    private GameObject[] raindrops;
    private bool isRaining = false;
    private bool isWaitingToRain = false;
    private Transform playerTransform;

    private void Start()
    {
        // Initialize the rain pool
        raindrops = new GameObject[numberOfRaindrops];
        for (int i = 0; i < numberOfRaindrops; i++)
        {
            raindrops[i] = Instantiate(rainDropPrefab, Vector3.zero, Quaternion.Euler(0, 0, 0));
            raindrops[i].transform.parent = transform;

            // Add a Rigidbody if it doesn't exist
            if (raindrops[i].GetComponent<Rigidbody>() == null)
            {
                Rigidbody rb = raindrops[i].AddComponent<Rigidbody>();
                rb.useGravity = false;
                rb.isKinematic = true;
            }

            // Add RaindropCollision script to each raindrop
            if (raindrops[i].GetComponent<RaindropCollision>() == null)
            {
                raindrops[i].AddComponent<RaindropCollision>().Initialize(this);
            }

            raindrops[i].SetActive(false);

            float randomVariation = 1f + Random.Range(-scaleVariation, scaleVariation);
            raindrops[i].transform.localScale = new Vector3(
                baseScale.x * randomVariation,
                baseScale.y * randomVariation,
                baseScale.z * randomVariation
            );
        }

        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if (isRaining)
        {
            UpdateRaindrops();
        }
    }

    private void UpdateRaindrops()
    {
        foreach (GameObject raindrop in raindrops)
        {
            if (raindrop.activeSelf)
            {
                // Check for collisions before moving
                Vector3 movement = Vector3.down * rainSpeed * Time.deltaTime;
                RaycastHit hit;

                // Raycast to detect obstacles, ignore specified layers
                if (Physics.Raycast(raindrop.transform.position, Vector3.down, out hit, movement.magnitude, ~ignoreCollisionLayers))
                {
                    // If we hit something that's not in our ignore layers, reposition the raindrop
                    RepositionRaindrop(raindrop);
                }
                else
                {
                    // Move the raindrop if no collision detected
                    raindrop.transform.Translate(movement, Space.World);

                    // If the raindrop is below ground level, reset its position
                    if (raindrop.transform.position.y < 0)
                    {
                        RepositionRaindrop(raindrop);
                    }
                }
            }
        }
    }

    private void RepositionRaindrop(GameObject raindrop)
    {
        float randomX = Random.Range(-spawnAreaWidth / 2, spawnAreaWidth / 2) + playerTransform.position.x;
        float randomZ = Random.Range(-spawnAreaLength / 2, spawnAreaLength / 2) + playerTransform.position.z;

        raindrop.transform.position = new Vector3(randomX, spawnHeight, randomZ);
    }

    public void StartRain()
    {
        if (!isWaitingToRain && !isRaining)
        {
            StartCoroutine(StartRainWithDelay());
        }
    }

    private IEnumerator StartRainWithDelay()
    {
        isWaitingToRain = true;
        yield return new WaitForSeconds(rainStartDelay);

        isWaitingToRain = false;
        isRaining = true;

        foreach (GameObject raindrop in raindrops)
        {
            RepositionRaindrop(raindrop);
            raindrop.SetActive(true);
        }
    }

    public void StopRain()
    {
        if (isWaitingToRain)
        {
            StopAllCoroutines();
            isWaitingToRain = false;
        }

        isRaining = false;
        foreach (GameObject raindrop in raindrops)
        {
            raindrop.SetActive(false);
        }
    }
}

// Add this as a new script (RaindropCollision.cs)
public class RaindropCollision : MonoBehaviour
{
    private RainManager rainManager;

    public void Initialize(RainManager manager)
    {
        rainManager = manager;
    }

    private void OnTriggerEnter(Collider other)
    {
        // If the raindrop collides with anything, reposition it
        // The layer mask in the main script will prevent this from triggering with ignored objects
        if (rainManager != null)
        {
            gameObject.SetActive(false);
            gameObject.SetActive(true); // This will trigger repositioning through the main script
        }
    }
}