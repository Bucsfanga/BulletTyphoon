using UnityEngine;
using UnityEngine.AI;

public class destructiblePlatform : MonoBehaviour
{
    [SerializeField] int destroyCountdown;
    [SerializeField] int respawnTimer;
    [SerializeField] GameObject platform;

    private bool isBreakingDown;
    private float destroyCountdownTimer;
    private Vector3 origPos;
    private Renderer platformRender;
    private Color origColor;
    private NavMeshObstacle navObstacle;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        destroyCountdownTimer = destroyCountdown;
        origPos = transform.position;
        platformRender = GetComponent<Renderer>();
        navObstacle = GetComponent<NavMeshObstacle>();

        if (platformRender != null )
        {
            origColor = platformRender.material.color;
        }
        if (navObstacle != null)
        {
            navObstacle.carving = false; // Disable carving on start
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isBreakingDown)
        {
            destroyCountdownTimer -= Time.deltaTime;

            // Change color of platform based on time remaining
            if (destroyCountdownTimer <= 2f)
                platformRender.material.color = Color.red;
            else
                platformRender.material.color = Color.yellow;

            if (destroyCountdownTimer <= 0f)
            {
                destroyPlatform();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isBreakingDown)
        {
            initiateBreakdown();
        }
    }

    void initiateBreakdown()
    {
        isBreakingDown = true;
        destroyCountdownTimer = destroyCountdown; // Reset timer
    }

    void destroyPlatform()
    {
        isBreakingDown = false;
        destroyCountdownTimer = destroyCountdown;

        if (navObstacle != null)
        {
            navObstacle.carving = true; // Enable carving on destruction
        }

        platform.SetActive(false); // Hide platform

        Invoke(nameof(respawnPlatform), respawnTimer); // Start respawn process
    }

    void respawnPlatform()
    {
        platform.SetActive(true);
        transform.position = origPos; // Reset position for moving platforms
        platformRender.material.color = origColor;

        if (navObstacle != null)
        {
            navObstacle.carving = false; // Disable carving on respawn
        }
    }
}
