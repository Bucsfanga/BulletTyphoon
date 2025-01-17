using UnityEngine;

public class gunshotAudio : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource gunAudioSource;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip gunShotClip;
    [SerializeField] private AudioClip gunClickClip;
    [SerializeField] private AudioClip gunReloadClip;

    [Header("Input Detection")]
    [SerializeField] private string fireButton = "Shoot";
    [SerializeField] private string reloadButton = "Reload";
    [SerializeField] private bool useInputDetection = true;

    [Header("Audio Settings")]
    [SerializeField] private float volumeMultiplier = 1f;
    [SerializeField] private bool useRandomPitch = true;
    [SerializeField] private float minPitch = 0.95f;
    [SerializeField] private float maxPitch = 1.05f;

    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        // If the audio source is not set, add an audio source to the game object
        if (gunAudioSource == null)
        {
            gunAudioSource = gameObject.AddComponent<AudioSource>();
            gunAudioSource.playOnAwake = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown(fireButton))
        {
            PlayGunShot();
        }
    }

    void PlayGunShot()
    {
        if (useRandomPitch)
        {
            gunAudioSource.pitch = Random.Range(minPitch, maxPitch);
        }
        gunAudioSource.PlayOneShot(gunShotClip, volumeMultiplier);
    }

    void PlayGunReload()
    { 
    if(Input.GetButtonDown(reloadButton))
        {
            gunAudioSource.PlayOneShot(gunReloadClip);
        }
    }
}
