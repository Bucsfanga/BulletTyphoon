using UnityEngine;

public class weatherController : MonoBehaviour
{
    public Material stormSkybox;
    public Material clearSkybox;
    public Light directionalLight;
    public Color stormAmbientLight = Color.gray;
    public Color clearAmbientLight = Color.white;
    public float stormFogDensity = 0.02f;
    public float clearFogDensity = 0.0f;

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
}
