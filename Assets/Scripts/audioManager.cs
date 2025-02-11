using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class audioManager : MonoBehaviour
{
    //Using singleton method to make sure we only have one instance of the audio manager
    public static audioManager instance;

    //Labeled section for the audio sources and create audio sources array
    [Header("Audio Sources")]
    [SerializeField] private AudioSource audioSourceTemplate;
    [SerializeField] private AudioSource backgroundAudioSource;
    private AudioSource[] audioSources;

    //Labeled section for the audio clips and create audio clips array as well as background audio clip slot
    [Header("Audio Clips")]
    [SerializeField] private AudioClip[] audioClips;
    [SerializeField] private AudioClip backgroundAudioClip;

    //Labeled section for the types of sounds that have multiple clips (death, damage taken, etc) and create lists for damage and death sounds
    [Header("Sound Collections")]
    [SerializeField]
    private List<string> damageSounds = new List<string>
    {
        "Player Hurt 1", "Player Hurt 2", "Player Hurt 3",
        "Player Hurt 4", "Player Hurt 5", "Player Hurt 6"
    };
    [SerializeField]
    private List<string> deathSounds = new List<string>
    {
        "Death 1", "Death 2", "Death 3", "Death 4", "Death 5"
    };
    [SerializeField]
    private List<string> jumpSounds = new List<string>
    {
        "Player Jump 1", "Player Jump 2", "Player Jump 3"
    };
    [SerializeField]
    private List<string> footStepSounds = new List<string>
    {
        "Footsteps 1", "Footsteps 2", "Footsteps 3",
        "Footsteps 6", "Footsteps 5", "Footsteps 4",
        "Footsteps 7", "Footsteps 8", "Footsteps 9"
    };


    //Labeled section for the audio settings. Created separate background and sound volumes, as well as a boolean to play music on awake, and fade in/out durations
    [Header("Audio Settings")]
    [SerializeField] private float defaultBackgroundVolume = 0.5f;
    [SerializeField] private float defaultSoundVolume = 1f;
    [SerializeField] private bool playBackgroundOnAwake = true;
    [SerializeField] private float fadeInDuration = 2f;
    [SerializeField] private float fadeOutDuration = 1f;
    

    private void Awake()
    {
       
        //Singleton method
        if (instance == null)
        {
            instance = this;
            //Initialize appropriate number of audio sources and set up the background audio
            InitializeAudioSources();
            SetupBackgroundAudio();
        }
        else if (instance != this)
        {
            //Debug.LogError($"Destroying audio manager on {gameObject.name} because instance already exists");
            Destroy(gameObject);
        }
    }

    private void InitializeAudioSources()
    {
        //initialize audio source array with the same number of elements as the audio clips array
        audioSources = new AudioSource[audioClips.Length];
        //Loop through the array and instantiate audio sources with the template on each created source and set default volume
        for (int i = 0; i < audioClips.Length; i++)
        {
            audioSources[i] = Instantiate(audioSourceTemplate, transform);
            audioSources[i].volume = defaultSoundVolume;
        }
    }

    private void SetupBackgroundAudio()
    {
        //If the background audio source gets removed somehow , add a new one
        if (backgroundAudioSource == null)
        {
            backgroundAudioSource = gameObject.AddComponent<AudioSource>();
        }

        //Set the audio source seetings automatically
        backgroundAudioSource.loop = true;
        backgroundAudioSource.volume = defaultBackgroundVolume;

        //Play the background audio on awake if the boolean is set to true (togglable in the inspector for testing)
        if (backgroundAudioClip != null && playBackgroundOnAwake)
        {
            PlayBackgroundAudio(backgroundAudioClip.name);
        }
    }

    public void PlaySound(string name)
    {
        //Create audio source and clip variables
        AudioSource audioSource = GetAvailableAudioSource();
        AudioClip audioClip = null;

        

        // Loop through audio clips and find matching audio clip to input name
        foreach (AudioClip clip in audioClips)
        {
            if (clip.name == name)
            {
                audioClip = clip;
                break;
            }
        }
        //If no audio clip is found, log an error and return
        if (audioClip == null)
        {
            Debug.LogError($"Failed to find sound: {name}");
            return;
        }

        //Set the audio source clip to the found clip and play the sound
        audioSource.clip = audioClip;
        audioSource.Play();
    }

    public void PlaySoundWithPitch(string name, float minPitch, float maxPitch)
    {
        AudioSource audioSource = GetAvailableAudioSource();
        AudioClip audioClip = null;

        if (audioSource == null) return;

        // Find the clip
        foreach (AudioClip clip in audioClips)
        {
            if (clip.name == name)
            {
                audioClip = clip;
                break;
            }
        }

        if (audioClip == null)
        {
            Debug.LogError($"Failed to find sound: {name}");
            return;
        }

        // Set pitch and play
        audioSource.pitch = Random.Range(minPitch, maxPitch);
        audioSource.clip = audioClip;
        audioSource.Play();

    }

    public void PlayBackgroundAudio(string name)
    {
        //Set the background audio source clip to the background audio clip, play the background audio, and fade in the audio
        backgroundAudioSource.clip = backgroundAudioClip;
        backgroundAudioSource.Play();
        StartCoroutine(FadeBackgroundAudio(fadeInDuration, defaultBackgroundVolume));
    }

    //Stop the background audio and fade out the audio (for use in the pause menu or when the game ends - intend to use end game music)
    public void StopBackgroundAudio()
    {
        StartCoroutine(FadeBackgroundAudio(fadeOutDuration, 0f));
    }

    //Getter for the background audio volume
    public float GetBackgroundAudioVolume()
    {
        return backgroundAudioSource.volume;
    }

    //Setter for the background audio volume - must be b/w 0 and 1
    public void SetBackgroundAudioVolume(float volume)
    {
        backgroundAudioSource.volume = Mathf.Clamp01(volume);
    }

    public string GetRandomSound(List<string> names)
    {
        //Handle exception for empty or null list otherwise use Random.Rnage between 0 and the count of the list
        if (names == null || names.Count == 0)
        {
            Debug.LogError("Sound list is empty or null");
            return string.Empty;
        }
        return names[Random.Range(0, names.Count)];
    }

    //For easy plug in use in the playerController class when the player takes damage
    public void PlayRandomDamageSound()
    {
        PlaySound(GetRandomSound(damageSounds));
    }

    //For easy plug in use in the playerController class when the player dies
    public void PlayRandomDeathSound()
    {
        PlaySound(GetRandomSound(deathSounds));
    }

    public void PlayRandomJumpSound()
    {
        PlaySound(GetRandomSound(jumpSounds));
    }

    public void PlayRandomFootstepSound()
    {
        PlaySound(GetRandomSound(footStepSounds));
    }

    //For delayed sound effects such as thunder, crack of lightning, or explosion. Not currently used in the project.
    public IEnumerator DelayPlaySound(string name, float delay)
    {
        yield return new WaitForSeconds(delay);
        PlaySound(name);
    }

    //Back the background audio in and out taking a length of time and a volume
    private IEnumerator FadeBackgroundAudio(float duration, float targetVolume)
    {
        //Set up timer and start volume variables
        float currentTime = 0;
        float start = backgroundAudioSource.volume;

        //While the current time is less than the duration, increase the timer and lerp the volume from the start to the target volume
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            backgroundAudioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
            yield return null;
        }

        //Stop the background audio if the target volume is 0
        if (targetVolume == 0f)
        {
            backgroundAudioSource.Stop();
        }
    }

    public AudioSource GetAvailableAudioSource()
    {
        // Loop through sources and find the first available audio source (one that isn't playing)
        foreach (AudioSource source in audioSources)
        {
            if (!source.isPlaying)
            {
                return source;
            }
        }
        //If no audio source is available, log a warning and return
            Debug.LogWarning("No available audio sources");
            return null;
    }
}

//TODO::

//Add volume sliders for jump, footsteps, and other sound effects

//Add a method to play footsteps sounds on an animation event
//Add a PlayRandomShootSound method
//Add a PlayGunClickSound method
//Add a PlayGunReloadSound method
//Add a method to play ambient water slosh sound during the flood
//Add a methods to play a damage and dying sound for the enemies
//Add a method to play a sound when the player picks up a health pack