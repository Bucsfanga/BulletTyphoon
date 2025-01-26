using UnityEngine;

public class HelicopterSound : MonoBehaviour
{
    private AudioSource audioSource;
    public float maxDistance = 20f; // Maximum distance at which the sound starts playing
    public float maxVolume = 1f; // Maximum volume when player is closest
    private Transform playerTransform;

    void Start()
    {
        // Get the AudioSource component
        audioSource = GetComponent<AudioSource>();

        // Configure the AudioSource
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f; // 1 = fully 3D sound

        // Find the player (adjust the tag as needed)
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (playerTransform != null)
        {
            float distance = Vector3.Distance(transform.position, playerTransform.position);

            // Check if player is within range
            if (distance <= maxDistance)
            {
                // Start playing if not already playing
                if (!audioSource.isPlaying)
                {
                    audioSource.Play();
                }

                // Calculate volume based on distance
                float volume = Mathf.Lerp(maxVolume, 0f, distance / maxDistance);
                audioSource.volume = volume;
            }
            else
            {
                // Stop playing if player is too far
                if (audioSource.isPlaying)
                {
                    audioSource.Stop();
                }
            }
        }
    }
}