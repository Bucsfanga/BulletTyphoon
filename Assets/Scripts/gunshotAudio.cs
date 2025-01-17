using UnityEngine;

public class gunshotAudio : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource gunShotSource;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip gunShotClip;
    [SerializeField] private AudioClip gunClickClip;
    [SerializeField] private AudioClip gunReloadClip;

    [Header("Input Detection")]
    [SerializeField] private string fireButton = "Fire1";
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
        if(gunShotSource == null)
        {
            gunShotSource = gameObject.AddComponent<AudioSource>();
            gunShotSource.playOnAwake = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        gunShotSource.PlayOn
    }
}
