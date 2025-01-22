using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class floodMap : MonoBehaviour
{
    [SerializeField] float floodLevelHeight, floodPauseDuration, floodRisingDuration, floodingSpeed, floodStartTime, maxFloodHeight;
    [SerializeField] int warningDuration;
    [SerializeField] TMP_Text incomingWarningText;
    [SerializeField] GameObject submergedOverlay;
   
    private Vector3 startPosition;
    private Vector3 currentTargetPosition;
    private bool isFlooding;
    private Transform player;

    private void Update()
    {
        updateSubmergedOverlay();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPosition = transform.position;
        currentTargetPosition = startPosition;

        if (submergedOverlay != null) // Check for submerged overlay
        {
            submergedOverlay.SetActive(false);
        }

        // Start flood sequence
        StartCoroutine(floodSequence());
    }

    IEnumerator floodSequence()
    {
        yield return new WaitForSeconds(floodStartTime); // Delay until flood start time

        while (currentTargetPosition.y < startPosition.y + maxFloodHeight)
        {
            // Set next target height for flood level
            currentTargetPosition = new Vector3(startPosition.x, Mathf.Min(currentTargetPosition.y + floodLevelHeight, startPosition.y + maxFloodHeight), startPosition.z);

            // Display warning before flood rises and play flood warning sound
            yield return StartCoroutine(audioManager.instance.DelayPlaySound("WarningSirenFinal", 0));
            yield return StartCoroutine(displayWarning(incomingWarningText, "Warning: Map will flood in {0} seconds!", warningDuration));
            

            isFlooding = true;

            // Gradually raise water level to target height
            float elapsedTime = 0f;
            Vector3 previousPosition = transform.position;

            while (elapsedTime < floodRisingDuration)
            {
                transform.position = Vector3.Lerp(previousPosition, currentTargetPosition, elapsedTime / floodRisingDuration);
                updateSubmergedOverlay(); // Check if player is submerged

                elapsedTime += Time.deltaTime * floodingSpeed;
                yield return null;
            }

            transform.position = currentTargetPosition; // Ensure water reaches target position
            yield return new WaitForSeconds(floodPauseDuration); // Pause water level at target height
        }

        isFlooding = false; // Flood reaches maximum height
        
        // Clear submerged overlay
        if (submergedOverlay != null)
        {
            submergedOverlay.SetActive(false);
        }
    }

    void updateSubmergedOverlay()
    {
        player = GameManager.instance.player.transform;
        if (player != null && submergedOverlay != null)
        {
            // Check if player's y position is below water level
            submergedOverlay.SetActive(player.position.y < transform.position.y);
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
