using UnityEngine;

public class RainManager : MonoBehaviour
{
    [Header("Rain Settings")]
    [SerializeField] private GameObject rainDrop;
    [SerializeField] private int numberOfRaindrops = 1000;
    [SerializeField] private float spawnHeight = 20f;
    [SerializeField] private float spawnAreaWidth = 30f;
    [SerializeField] private float spawnAreaLength = 30f;
    [SerializeField] private float rainSpeed = 10f;
    [SerializeField] private float minScale = 0.5f;
    [SerializeField] private float maxScale = 1.5f;

    private GameObject[] raindrops;
    private bool isRaining = false;
    private Transform playerTransform;

    private void Start()
    {
        // Initialize the rain pool
        raindrops = new GameObject[numberOfRaindrops];
        for (int i = 0; i < numberOfRaindrops; i++)
        {
            raindrops[i] = Instantiate(rainDrop, Vector3.zero, Quaternion.Euler(0, 0, 0));
            raindrops[i].transform.parent = transform;
            raindrops[i].SetActive(false);

            // Randomize the scale of each raindrop
            float randomScale = Random.Range(minScale, maxScale);
            raindrops[i].transform.localScale = new Vector3(randomScale, randomScale, randomScale);
        }

        // Find the player
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
                // Move the raindrop down
                raindrop.transform.Translate(Vector3.down * rainSpeed * Time.deltaTime, Space.World);

                // If the raindrop is below ground level, reset its position
                if (raindrop.transform.position.y < 0)
                {
                    RepositionRaindrop(raindrop);
                }
            }
        }
    }

    private void RepositionRaindrop(GameObject raindrop)
    {
        // Calculate a random position within the spawn area, centered on the player
        float randomX = Random.Range(-spawnAreaWidth / 2, spawnAreaWidth / 2) + playerTransform.position.x;
        float randomZ = Random.Range(-spawnAreaLength / 2, spawnAreaLength / 2) + playerTransform.position.z;

        raindrop.transform.position = new Vector3(randomX, spawnHeight, randomZ);
    }

    public void StartRain()
    {
        isRaining = true;
        foreach (GameObject raindrop in raindrops)
        {
            RepositionRaindrop(raindrop);
            raindrop.SetActive(true);
        }
    }

    public void StopRain()
    {
        isRaining = false;
        foreach (GameObject raindrop in raindrops)
        {
            raindrop.SetActive(false);
        }
    }
}
