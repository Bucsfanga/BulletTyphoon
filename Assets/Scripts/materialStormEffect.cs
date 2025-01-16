using UnityEngine;
using System.Collections;

public class MaterialStormEffect : MonoBehaviour
{
    [SerializeField] private Material targetMaterial;
    [SerializeField] private float transitionDuration = 2f;
    [SerializeField] private float delayBeforeEffect = 10f; // 10 second delay

    // Original material properties
    private float originalSmoothness;
    private float originalMetallic;

    // Target values for storm effect
    [SerializeField] private float stormSmoothness = 1f;
    [SerializeField] private float stormMetallic = 1f;

    private void Start()
    {
        if (targetMaterial != null)
        {
            // Reset and store original values
            targetMaterial.SetFloat("_Metallic", 0f);
            targetMaterial.SetFloat("_Glossiness", 0f); // Using _Glossiness instead of _Smoothness

            originalMetallic = 0f;
            originalSmoothness = 0f;
        }
    }

    private void OnEnable()
    {
        // Reset values when the script is enabled
        if (targetMaterial != null)
        {
            targetMaterial.SetFloat("_Metallic", 0f);
            targetMaterial.SetFloat("_Glossiness", 0f);
        }
    }

    public void OnStormBegin()
    {
        if (targetMaterial != null)
        {
            StartCoroutine(StormMaterialEffect());
        }
    }

    public void OnStormEnd()
    {
        if (targetMaterial != null)
        {
            StartCoroutine(RevertMaterialEffect());
        }
    }

    private IEnumerator StormMaterialEffect()
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delayBeforeEffect);

        float elapsedTime = 0f;
        float startSmoothness = targetMaterial.GetFloat("_Glossiness");
        float startMetallic = targetMaterial.GetFloat("_Metallic");

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / transitionDuration;

            // Smoothly interpolate between current and target values
            float currentSmoothness = Mathf.Lerp(startSmoothness, stormSmoothness, t);
            float currentMetallic = Mathf.Lerp(startMetallic, stormMetallic, t);

            // Apply the values
            targetMaterial.SetFloat("_Glossiness", currentSmoothness);
            targetMaterial.SetFloat("_Metallic", currentMetallic);

            yield return null;
        }

        // Ensure final values are set
        targetMaterial.SetFloat("_Glossiness", stormSmoothness);
        targetMaterial.SetFloat("_Metallic", stormMetallic);
    }

    private IEnumerator RevertMaterialEffect()
    {
        float elapsedTime = 0f;
        float startSmoothness = targetMaterial.GetFloat("_Glossiness");
        float startMetallic = targetMaterial.GetFloat("_Metallic");

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / transitionDuration;

            // Smoothly interpolate back to original values
            float currentSmoothness = Mathf.Lerp(startSmoothness, originalSmoothness, t);
            float currentMetallic = Mathf.Lerp(startMetallic, originalMetallic, t);

            // Apply the values
            targetMaterial.SetFloat("_Glossiness", currentSmoothness);
            targetMaterial.SetFloat("_Metallic", currentMetallic);

            yield return null;
        }

        // Ensure original values are set
        targetMaterial.SetFloat("_Glossiness", originalSmoothness);
        targetMaterial.SetFloat("_Metallic", originalMetallic);
    }
}