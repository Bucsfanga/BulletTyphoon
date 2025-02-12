using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private Vector3 moveDirection = Vector3.right; // Default movement direction
    [SerializeField] private float moveDistance = 5f; // Distance to move
    [SerializeField] private float moveSpeed = 2f; // Speed of movement

    private Vector3 startPos;
    private Vector3 targetPos;
    private bool movingForward = true;
    private Renderer platformRender;
    private Color originalColor;

    void Start()
    {
        startPos = transform.position;
        targetPos = startPos + (moveDirection.normalized * moveDistance);

        platformRender = GetComponent<Renderer>();
        if (platformRender != null)
        {
            originalColor = platformRender.material.color;
            platformRender.material.color = Color.green; // Green tint during gameplay
        }
    }

    void Update()
    {
        MovePlatform();
    }

    private void MovePlatform()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPos) < 0.1f)
        {
            movingForward = !movingForward;
            targetPos = movingForward ? startPos + (moveDirection.normalized * moveDistance) : startPos;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.parent = transform; // Attach player to platform
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.parent = null; // Detach player when leaving
        }
    }
}
