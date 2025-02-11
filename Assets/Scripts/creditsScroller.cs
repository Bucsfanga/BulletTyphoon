using UnityEngine;
using TMPro;
using System.Collections;
using Unity.VisualScripting;
using NUnit.Framework.Constraints;

public class creditsScroller : MonoBehaviour
{
    [SerializeField] RectTransform creditsText;
    [SerializeField] float scrollSpeed, offscreenTop, resetDelay;

    private Vector3 startPos;
    private bool isScrolling = false;

    void Start()
    {
        startPos = creditsText.anchoredPosition; // Save starting position
    }

    public void startScrolling()
    {
        if (!isScrolling)
        {
            StartCoroutine(scrollCredits());
        }
    }

    private IEnumerator scrollCredits()
    {
        isScrolling = true;

        CanvasGroup canvasGroup = creditsText.gameObject.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = creditsText.gameObject.AddComponent<CanvasGroup>();
        }

        while (true)
        {
            creditsText.anchoredPosition += Vector2.up * scrollSpeed * 0.1f; // Move up

            // Fade out as it nears offscreen
            if (creditsText.anchoredPosition.y >= offscreenTop - 200f)
            {
                canvasGroup.alpha = Mathf.Clamp01(1 - (creditsText.anchoredPosition.y - (offscreenTop - 200f)) / 200f);
            }

            // Reset when offscreen
            if (creditsText.anchoredPosition.y >= offscreenTop)
            {
                yield return new WaitForSeconds(resetDelay);

                creditsText.anchoredPosition = startPos; // Reset position
                canvasGroup.alpha = 1f; // Reset alpha
            }

            yield return null;
        }
    }

    public void resetCredits()
    {
        // Reset credits to starting position
        creditsText.anchoredPosition = startPos;

        // Reset alpha if CanvasGroup is attached
        CanvasGroup canvasGroup = creditsText.gameObject.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
        }

        isScrolling = false;
    }
}
