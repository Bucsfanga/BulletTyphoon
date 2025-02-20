using UnityEngine;

public class gunClickAudio : MonoBehaviour
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
    public void PlayGunClick()
    {
        if (useRandomPitch)
        {
            audioManager.PlaySoundWithPitch("Gun Click", minPitch, maxPitch);
        }
        else
        {
            audioManager.PlaySound("Gun Click");
        }

    }
}
