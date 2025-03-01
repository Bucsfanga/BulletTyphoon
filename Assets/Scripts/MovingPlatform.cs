using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Unity.VisualScripting;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private Vector3 moveDirection; // Direction platform moves
    [SerializeField] private float moveDistance; // Distance to move
    [SerializeField] private float moveSpeed; // Speed of movement
    [SerializeField] private Collider triggerCollider;

    private Vector3 startPos; // Platform loop start point
    private Vector3 targetPos; // Platform loop end point
    private bool movingForward = true;
    private Renderer platformRender;
    private Color originalColor;
    private Vector3 lastPosition;
    

    private HashSet<playerController> playersOnPlatform = new HashSet<playerController>(); // Track player

    void Start()
    {
        Collider[] colliders = GetComponents<Collider>();

        foreach (Collider col in colliders)
        {
            if (col.isTrigger)
            {
                triggerCollider = col;
                break;
            }
        }

        startPos = transform.position;
        targetPos = startPos + (moveDirection.normalized * moveDistance);
        lastPosition = transform.position;

        platformRender = GetComponent<Renderer>();
        if (platformRender != null)
        {
            originalColor = platformRender.material.color;
            platformRender.material.color = Color.green; // Green tint during gameplay
        }
    }

    void FixedUpdate()
    {
        MovePlatform();

        // Detach player if no longer standing on platform
        List<playerController> toRemove = new List<playerController>();

        foreach (var player in playersOnPlatform)
        {
            CharacterController controller = player.GetComponent<CharacterController>();

            if (controller != null)
            {
                bool stillOnPlatform = controller.isGrounded && isPlayerOnPlatform(player);

                Collider playerCollider = player.GetComponent<Collider>(); // Get player collider
                bool insideBounds = triggerCollider.bounds.Contains(player.transform.position);

                if (!stillOnPlatform || !insideBounds)
                {
                    player.transform.SetParent(null);
                    toRemove.Add(player);
                }                
            }
        }

        // Remove players when no longer grounded
        foreach (var player in toRemove)
        {
            playersOnPlatform.Remove(player);
        }
    }

    private bool isPlayerOnPlatform(playerController player)
    {
        if(player.transform.parent != transform)
        {
            return false;
        }

        RaycastHit hit;
        Vector3 rayStart = player.transform.position + Vector3.up * 0.2f; // Start raycast slightly above player

        if (Physics.Raycast(rayStart, Vector3.down, out hit, 1.0f))
        {
            player.jumpCount = 0;
            return hit.collider.gameObject == gameObject; // ensure hit object is a platform
        }
        return false;
    }

    private void MovePlatform()
    {
        Vector3 previousPos = transform.position;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed *  Time.fixedDeltaTime);

        Vector3 platformMovement = transform.position - previousPos;
        Vector3 adjustedMovement = new Vector3(platformMovement.x, 0, platformMovement.z);

        foreach (var player in playersOnPlatform)
        {
            CharacterController controller = player.GetComponent<CharacterController>();

            if (controller != null && controller.isGrounded && !player.isJumping)
            {
                player.controller.Move(adjustedMovement);
            }
        }

        if (Vector3.Distance(transform.position, targetPos) < 0.1f)
        {
            movingForward = !movingForward;
            targetPos = movingForward ? startPos + (moveDirection.normalized * moveDistance) : startPos;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            other.transform.SetParent(transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            playerController player = other.GetComponent<playerController>();

            if (player != null)
            {
                StartCoroutine(checkIfGrounded(player));
            }
        }
    }

    IEnumerator checkIfGrounded(playerController player)
    {
        yield return new WaitForSeconds(0.1f);

        CharacterController controller = player.GetComponent<CharacterController>();

        if (controller != null && !isPlayerOnPlatform(player))
        {
            player.transform.SetParent(null);
        }
    }
}
