using UnityEngine;

public class spawner : MonoBehaviour
{
    [SerializeField] GameObject objectToSpawn;
    [SerializeField] int numToSpawn;
    [SerializeField] int timeBetweenSpawns;
    [SerializeField] Transform[] spawnPos;

    float spawnTimer;

    int spawnCount;

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
        int spawnInt = Random.Range(0, spawnPos.Length);
        spawnTimer = 0;
        Instantiate(objectToSpawn, spawnPos[spawnInt].position, spawnPos[spawnInt].rotation);
        spawnCount++;
        GameManager.instance.updateGameGoal(1);
    }
}
