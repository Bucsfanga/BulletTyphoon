using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class floodMap : MonoBehaviour
{
    [SerializeField] float floodHeight, floodDuration, floodStartTime, floodingSpeed;
    [SerializeField] int warningDuration;
    [SerializeField] TMP_Text incomingWarningText, outgoingWarningText;
    [SerializeField] GameObject submergedOverlay;
   
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private bool isFlooding = false;
    private Transform player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPosition = transform.position;
        targetPosition = new Vector3(startPosition.x, startPosition.y + floodHeight, startPosition.z);

        if (submergedOverlay != null)
        {
            submergedOverlay.SetActive(false);
        }

        // Start flood sequence
        StartCoroutine(floodSequence());
    }

    IEnumerator floodSequence()
    {
        yield return new WaitForSeconds(floodStartTime); // Delay until flood start time

        // Display warning countdown
        for (int i = warningDuration; i > 0; i--)
        {
            if (incomingWarningText != null)
            {
                incomingWarningText.text = $"Warning: Map will flood in {Mathf.CeilToInt(i)} seconds!";
            }
            yield return new WaitForSeconds(1f);
        }

        // Clear warning text
        if (incomingWarningText != null)
        {
            incomingWarningText.text = "";
        }
        isFlooding = true;

        float elapsedTime = 0f;
        while (elapsedTime < floodDuration)
        {
            // Gradually move the water up to the target height
            transform.position = Vector3.Lerp(startPosition, targetPosition, (elapsedTime / floodDuration) * floodingSpeed);

            updateSubmergedOverlay(); // Check if player is submerged

            //Check if we are within the last warning duration's seconds
            if(elapsedTime >= floodDuration - warningDuration && outgoingWarningText != null && string.IsNullOrEmpty(outgoingWarningText.text))
            {
                StartCoroutine(displayOutgoingWarning());
            }
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure water reaches the target height
        transform.position = targetPosition;

        // Pause briefly at flood height
        yield return new WaitForSeconds(floodDuration);

        // Flood recedes to starting position
        elapsedTime = 0f;
        while (elapsedTime < floodDuration)
        {
            // Gradually move the water up to the target height
            transform.position = Vector3.Lerp(targetPosition, startPosition, (elapsedTime / floodDuration) * floodingSpeed);
            updateSubmergedOverlay();
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure water returns to start position
        transform.position = startPosition;

        if (submergedOverlay != null)
        {
            submergedOverlay.SetActive(false);
        }

        isFlooding = false;
    }

    void updateSubmergedOverlay()
    {
        player = gameManager.instance.player.transform;
        if (player != null && submergedOverlay != null)
        {
            // Check if player's y position is below water level
            if (player.position.y < transform.position.y)
            {
                submergedOverlay.SetActive(true);
            }
            else 
            {
                submergedOverlay.SetActive(false);
            }
        }
    }

    IEnumerator displayOutgoingWarning()
    {
        // Display warning countdown
        for (int i = warningDuration; i > 0; i--)
        {
            if (outgoingWarningText != null)
            {
                outgoingWarningText.text = $"Warning: Flood will recede in {Mathf.CeilToInt(i)} seconds!";
            }
            yield return new WaitForSeconds(1f);
        }

        // Clear warning text
        if (outgoingWarningText != null)
        {
            outgoingWarningText.text = "";
        }
    }
}
