using UnityEngine;
using System.Collections;

public class weatherController : MonoBehaviour
{
    public Material stormSkybox;
    public Material clearSkybox;
    public Light directionalLight;
    public Color stormAmbientLight = Color.gray;
    public Color clearAmbientLight = Color.white;
    public float stormFogDensity = 0.02f;
    public float clearFogDensity = 0.0f;
    public float transitionDuration = 2f;

    private Color originalAmbientLight;
    private float originalLightIntensity;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {  // saving the original settings
        originalAmbientLight = RenderSettings.ambientLight;
        originalLightIntensity = directionalLight.intensity;
    }

    public void startStorm()
    {
        RenderSettings.skybox = stormSkybox;
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
        RenderSettings.skybox = clearSkybox;
        RenderSettings.ambientLight = originalAmbientLight;
        RenderSettings.fog = false;

        if (directionalLight != null)
        {
            directionalLight.intensity = originalLightIntensity; // Restore light
        }
    }

   private IEnumerator FadeSkybox(Material fromSkybox, Material toSkybox)
    {
        float elapsedTime = 0f;

        // Set the initial skybox
        RenderSettings.skybox = fromSkybox;
        float fromExposure = fromSkybox.GetFloat("_Exposure");
        float toExposure = toSkybox.GetFloat("_Exposure");

        // Start blending
        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / transitionDuration;

            fromSkybox.SetFloat("_Exposure", Mathf.Lerp(fromExposure, 0f, t));
            toSkybox.SetFloat("_Exposure", Mathf.Lerp(0f, toExposure, t));

            yield return null;
        }

        // Ensure the final values are set
        fromSkybox.SetFloat("_Exposure", 0f);
        toSkybox.SetFloat("_Exposure", 1f);

        // Set the new skybox
        RenderSettings.skybox = toSkybox;
    }
}

