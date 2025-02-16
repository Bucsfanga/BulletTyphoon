using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class floodMap : MonoBehaviour
{
    [SerializeField] float floodLevelHeight, floodPauseDuration, floodRisingDuration, floodingSpeed, floodStartTime, maxFloodHeight;
    [SerializeField] int warningDuration;

    private Vector3 startPosition;
    private Vector3 currentTargetPosition;
    private bool isFlooding;
    private Transform player;

    private GameObject submergedOverlay;
    private TMP_Text incomingWarningText;
    private float currentFloodHeight;
    private bool isFloodInitialized = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Start flood sequence
        initializeFloodMap();
        StartCoroutine(floodSequence());
    }

    private void Update()
    {
        if (isFloodInitialized)
        {
            updateSubmergedOverlay();
        }      
    }

    private void initializeFloodMap()
    {
        if (GameManager.instance != null)
        {
            // Get references from GameManager script
            player = GameManager.instance.player.transform;
            submergedOverlay = GameManager.instance.submergedOverlay;
            incomingWarningText = GameManager.instance.incomingWarningText;
        }
        else
        {
            Debug.LogError("GameManager instance is null!");
        }

        startPosition = transform.parent.position;
        currentTargetPosition = startPosition;
        currentFloodHeight = startPosition.y;

        if (submergedOverlay != null) // Check for submerged overlay
        {
            submergedOverlay.SetActive(false);
        }

        //Debug.Log($"Flood map initialized. Start Position Y: {startPosition.y}, Current Flood Height: {currentFloodHeight}");
    }

    IEnumerator floodSequence()
    {
        yield return new WaitForSeconds(floodStartTime); // Delay until flood start time
        isFloodInitialized = true;

        while (currentTargetPosition.y < startPosition.y + maxFloodHeight)
        {
            // Set next target height for flood level
            currentTargetPosition = new Vector3(startPosition.x, Mathf.Min(currentTargetPosition.y + floodLevelHeight, startPosition.y + maxFloodHeight), startPosition.z);

            // Display warning before flood rises and play flood warning sound
            yield return StartCoroutine(audioManager.instance.DelayPlaySound("WarningSirenFinal", 0));
            GameManager.instance.hud.GetComponent<NoticeBanner>().Notice(0);
            yield return StartCoroutine(displayWarning(incomingWarningText, "Warning: Map will flood in {0} seconds!", warningDuration));

            isFlooding = true;

            // Gradually raise water level to target height
            float elapsedTime = 0f;
            Vector3 previousPosition = transform.parent.position;

            while (elapsedTime < floodRisingDuration)
            {
                transform.parent.position = Vector3.Lerp(previousPosition, currentTargetPosition, elapsedTime / floodRisingDuration);

                updateCurrentFloodHeight(transform.parent.position.y);            

                elapsedTime += Time.deltaTime * floodingSpeed;
                yield return null;
            }

            transform.parent.position = currentTargetPosition; // Ensure water reaches target position
            updateCurrentFloodHeight(transform.parent.position.y);
            
            yield return new WaitForSeconds(floodPauseDuration); // Pause water level at target height
        }

        isFlooding = false; // Flood reaches maximum height

        // Clear submerged overlay
        if (submergedOverlay != null)
        {
            submergedOverlay.SetActive(false);
        }
    }

    private void updateCurrentFloodHeight(float newHeight)
    {
        if (newHeight > currentFloodHeight)
        {
            currentFloodHeight = newHeight;
        }     
    }

    void updateSubmergedOverlay()
    {
        if (player == null)
        {
            Debug.LogError("Player not set.");
            return;
        }

        if (player != null && submergedOverlay != null)
        {
            // Check if player's y position is below water level
            bool isUnderwater = player.position.y + 1 < currentFloodHeight;
            submergedOverlay.SetActive(isUnderwater);
            //Debug.Log($"Player Y: {player.position.y}, Water Y: {currentFloodHeight}, Underwater: {isUnderwater}");
        }
    }

    IEnumerator displayWarning(TMP_Text warningText, string message, int duration)
    {
        // Display warning countdown
        for (int i = duration; i > 0; i--)
        {
            if (warningText != null)
            {
                warningText.text = string.Format(message, i);

            }
            yield return new WaitForSeconds(1f);
        }

        // Clear warning text
        if (warningText != null)
        {
            warningText.text = "";
        }
    }
}
