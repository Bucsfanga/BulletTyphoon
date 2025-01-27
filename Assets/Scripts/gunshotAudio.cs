using UnityEngine;

public class gunshotAudio : MonoBehaviour
{
    private audioManager audioManager;

    

    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioManager = audioManager.instance;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayGunShot()
    {
        //TODO:: Incorporate a random pitch method/setting in audio manager
        if (audioManager.useRandomPitch)
        {
            gunaudiosource.pitch = random.range(minpitch, maxpitch);
        }
        audioManager.PlaySound("Gun Shot");
    }

   public void PlayGunReload()
    {
        audioManager.PlaySound("Gun Reload");
    }
}
