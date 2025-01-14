using UnityEngine;
using System.Collections;

public class weatherController : MonoBehaviour
{
    [SerializeField] Material stormSkybox;
    [SerializeField] Material clearSkybox;
    [SerializeField] Light directionalLight;
    [SerializeField] Color stormAmbientLight = Color.gray;
    [SerializeField] Color clearAmbientLight = Color.white;
    [SerializeField] float stormFogDensity = 0.02f;
    [SerializeField] float clearFogDensity = 0.0f;
    [SerializeField] float transitionDuration = 2f; // Duration of the transition

    private Color originalAmbientLight;
    private float originalLightIntensity;

    private void Start()
    {
        // Save original settings
        originalAmbientLight = RenderSettings.ambientLight;
        originalLightIntensity = directionalLight.intensity;

        // this ensures the clear skybox is set as the active skybox on start up.
        RenderSettings.skybox = clearSkybox;

        if (clearSkybox.HasProperty("_Exposure"))
        {
            clearSkybox.SetFloat("_Exposure", 1f);
        }
        if (stormSkybox.HasProperty("_Exposure"))
        {
            stormSkybox.SetFloat("_Exposure", 0f);
        }

        RenderSettings.ambientLight = clearAmbientLight;
        RenderSettings.fog = false; // Enable fog in case it's needed
        RenderSettings.fogDensity = clearFogDensity;

    }

    public void startStorm()
    {
        StartCoroutine(transitionToSkybox(clearSkybox, stormSkybox));
        RenderSettings.ambientLight = stormAmbientLight;
        RenderSettings.fog = true;
        RenderSettings.fogDensity = stormFogDensity;

        if (directionalLight != null)
        {
            directionalLight.intensity *= 0.5f; // Dim light
        }
    }

    public void endStorm()
    {
        StartCoroutine(transitionToSkybox(stormSkybox, clearSkybox));
        RenderSettings.ambientLight = originalAmbientLight;
        RenderSettings.fog = false;

        if (directionalLight != null)
        {
            directionalLight.intensity = originalLightIntensity; // Restore light
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
