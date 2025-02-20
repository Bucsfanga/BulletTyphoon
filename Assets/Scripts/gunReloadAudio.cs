using UnityEngine;

public class gunReloadAudio : MonoBehaviour
{
    private audioManager audioManager;

    [SerializeField] public bool useRandomPitch = true;
    [SerializeField] private float minPitch = 0.95f;
    [SerializeField] private float maxPitch = 1.05f;

    
    void Start()
    {
        audioManager = audioManager.instance;
    }

    
    void Update()
    {
        
    }
    public void PlayGunReload()
    {
        if (useRandomPitch)
        {
            audioManager.PlaySoundWithPitch("Gun Reload", minPitch, maxPitch);
        }
        else
        {
            audioManager.PlaySound("Gun Reload");
        }
        
    }
}
