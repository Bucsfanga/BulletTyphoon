using UnityEngine;
using System.Collections;

public class weatherController : MonoBehaviour
{
    [SerializeField] Material stormSkybox;
    [SerializeField] Material clearSkybox;
    [SerializeField] Color stormAmbientLight = Color.gray;
    [SerializeField] float stormFogDensity = 0.02f;
    [SerializeField] private RainManager rainManager;
    [SerializeField] float transitionDuration = 2f; // Duration of the transition

    private void Start()
    {
        // Set the storm effects as the default ambiance
        InitializeStormEffects();
        StartCoroutine(transitionToSkybox(clearSkybox, stormSkybox));
    }

    private void InitializeStormEffects()
    {
        // Set storm skybox and effects
        RenderSettings.skybox = stormSkybox;

        if (stormSkybox.HasProperty("_Exposure"))
        {
            stormSkybox.SetFloat("_Exposure", 1f);
        }

        RenderSettings.ambientLight = stormAmbientLight;
        RenderSettings.fog = true;
        RenderSettings.fogDensity = stormFogDensity;

        if (rainManager != null)
        {
            rainManager.StartRain();
        }
    }

    private IEnumerator transitionToSkybox(Material fromSkybox, Material toSkybox)
    {
        float elapsedTime = 0f;

        // Set the initial skybox
        RenderSettings.skybox = fromSkybox;

        // Ensure materials are set to their initial states
        fromSkybox.SetFloat("_Exposure", 1f);
        toSkybox.SetFloat("_Exposure", 0f);

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / transitionDuration;

            // Interpolate exposure values for smooth blending
            fromSkybox.SetFloat("_Exposure", Mathf.Lerp(1f, 0f, t));
            toSkybox.SetFloat("_Exposure", Mathf.Lerp(0f, 1f, t));

            yield return null; // Wait for the next frame
        }

        // Ensure final states are set
        fromSkybox.SetFloat("_Exposure", 0f);
        toSkybox.SetFloat("_Exposure", 1f);

        // Set the active skybox to the target material
        RenderSettings.skybox = toSkybox;
    }
}
