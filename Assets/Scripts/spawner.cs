using UnityEngine;

public class spawner : MonoBehaviour
{
    [SerializeField] GameObject[] objectToSpawn;
    [SerializeField] int numToSpawn;
    [SerializeField] int timeBetweenSpawns;
    [SerializeField] Transform[] spawnPos;
    [SerializeField] bool spawnRandomly; // Toggle to spawn at positions randomly or sequentially

    float spawnTimer;

    int spawnCount;
    int sequentialSpawnIndex = 0;

    bool startSpawning;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawnCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        spawnTimer += Time.deltaTime;

        if (startSpawning)
        {
            if(spawnCount < numToSpawn && spawnTimer >= timeBetweenSpawns)
            {
                spawn();
                GameManager.instance.updateGameGoal(1);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            startSpawning = true;
        }
    }

    void spawn()
    {
        int spawnInt;
        if (spawnRandomly == true)
        {
            spawnInt = Random.Range(0, spawnPos.Length); // Choose random position
        }
        else
        {
            spawnInt = sequentialSpawnIndex;
            sequentialSpawnIndex = (sequentialSpawnIndex + 1) % spawnPos.Length; // Loop through spawn positions
        }


        int objectInt = Random.Range(0, objectToSpawn.Length); // Choose random enemy

        spawnTimer = 0; // Reset spawn timer
        Instantiate(objectToSpawn[objectInt], spawnPos[spawnInt].position, spawnPos[spawnInt].rotation);
        spawnCount++;
    }
}
