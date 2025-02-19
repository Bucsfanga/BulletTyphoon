using UnityEngine;
using System.Collections;
public class CameraShake : MonoBehaviour
{
    public static CameraShake instance; // Singleton reference

    [SerializeField] private float shakeDuration = 0.2f;
    [SerializeField] private float shakeMagnitude = 0.2f;

    private Transform camTransform;
    private Vector3 originalPosition;

    void Awake()
    {
        instance = this; // Singleton pattern
        camTransform = Camera.main.transform; // Get the main camera transform
        originalPosition = camTransform.localPosition;
    }

    public void TriggerShake(float duration = -1, float magnitude = -1)
    {
        if (duration <= 0) duration = shakeDuration;
        if (magnitude <= 0) magnitude = shakeMagnitude;
        StartCoroutine(Shake(duration, magnitude));
    }

    private IEnumerator Shake(float duration, float magnitude)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            Vector3 randomOffset = Random.insideUnitSphere * magnitude;
            camTransform.localPosition = originalPosition + randomOffset;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        camTransform.localPosition = originalPosition; // Reset to original position
    }
}
