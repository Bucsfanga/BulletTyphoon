using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private Vector3 moveDirection; // Direction platform moves
    [SerializeField] private float moveDistance; // Distance to move
    [SerializeField] private float moveSpeed; // Speed of movement

    private Vector3 startPos; // Platform loop start point
    private Vector3 targetPos; // Platform loop end point
    private bool movingForward = true;
    private Renderer platformRender;
    private Color originalColor;
    private Vector3 lastPosition;

    private HashSet<playerController> playersOnPlatform = new HashSet<playerController>(); // Track player

    void Start()
    {
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

            if (controller != null && !controller.isGrounded)
            {
                player.transform.SetParent(null);
                toRemove.Add(player);
            }
        }

        // Remove players when no longer grounded
        foreach (var player in toRemove)
        {
            playersOnPlatform.Remove(player);
        }
    }

    private void MovePlatform()
    {
        Vector3 previousPos = transform.position;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed *  Time.fixedDeltaTime);

        Vector3 platformMovement = transform.position - previousPos;

        foreach (var player in playersOnPlatform)
        {         
            player.transform.position += platformMovement;
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

        if (controller != null && !controller.isGrounded)
        {
            player.transform.SetParent(null);
        }
    }
}
