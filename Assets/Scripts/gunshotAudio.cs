using UnityEngine;

public class gunshotAudio : MonoBehaviour
{
    private audioManager audioManager;
  
    [SerializeField] private float volumeMultiplier = 1f;
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

    public void PlayGunShot()
    {
        //TODO:: Incorporate a random pitch method/setting in audio manager
        if (useRandomPitch)
        {
            audioManager.PlaySoundWithPitch("Gun Shot", minPitch, maxPitch);
        }
        else
        {
            audioManager.PlaySound("Gun Shot");
        }
    }
}
