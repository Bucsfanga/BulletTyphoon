using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;

public class audioManager : MonoBehaviour
{
    //Using singleton method to make sure we only have one instance of the audio manager
    public static audioManager instance;

    //Audio mixer reference
    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;

    //Exposed mixer parameter names
    private const string MasterVolumeParam = "MasterVolume";
    private const string BackgroundVolumeParam = "BackgroundVolume";
    private const string SFXVolumeParam = "SFXVolume";
    private const string MusicVolumeParam = "MusicVolume";

    //Labeled section for the audio sources and create audio sources array
    [Header("Audio Sources")]
    [SerializeField] private AudioSource audioSourceTemplate;
    [SerializeField] private AudioSource backgroundAudioSource;
    [SerializeField] private AudioSource mainMenuMusicSource;
    [SerializeField] private AudioSource loseMenuMusicSource;
    private AudioSource[] audioSources;

    //Labeled section for the audio clips and create audio clips array as well as background audio clip slot
    [Header("Audio Clips")]
    [SerializeField] private AudioClip[] audioClips;
    [SerializeField] private AudioClip backgroundAudioClip;
    [SerializeField] private AudioClip mainMenuMusicClip;
    [SerializeField] private AudioClip loseMenuMusicClip;

    #region Sound Collections
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
    #endregion


    //Labeled section for the audio settings. Created separate background and sound volumes, as well as a boolean to play music on awake, and fade in/out durations
    [Header("Audio Settings")]
    [SerializeField][Range(0,1)] float BackgroundVolume;
    [SerializeField][Range(0, 1)] float MusicVolume;
    [SerializeField][Range(0, 1)] float SoundVolume;
    [SerializeField] private bool playBackgroundOnAwake = true;
    [SerializeField] private bool playMenuMusicOnAwake = true;
    [SerializeField] private float fadeInDuration = 2f;
    [SerializeField] private float fadeOutDuration = 2f;

    [Header("UI Sounds")]
    [SerializeField] private AudioClip uiClickSound;

    

    private void Awake()
    {
       
        //Singleton method
        if (instance == null)
        {
            instance = this;

            LoadSavedVolumeSettings();

            //Initialize appropriate number of audio sources and set up the background audio
            InitializeAudioSources();
            SetupBackgroundAudio();
            SetUpMainMenuMusic();
            SetUpLoseMenuMusic();
        }
        else if (instance != this)
        {
            //Debug.LogError($"Destroying audio manager on {gameObject.name} because instance already exists");
            Destroy(gameObject); 
        }      
    }

    private float ConvertToDecibel(float vol)
    {
        return vol > 0 ? 20f * Mathf.Log10(vol) : -80f;
    }

    private void InitializeAudioSources()
    {
        //initialize audio source array with the same number of elements as the audio clips array
        audioSources = new AudioSource[audioClips.Length];
        //Loop through the array and instantiate audio sources with the template on each created source and set default volume
        for (int i = 0; i < audioClips.Length; i++)
        {
            audioSources[i] = Instantiate(audioSourceTemplate, transform);
            audioSources[i].volume = SoundVolume;

            //Assign to the SFX Mixer group
            if(audioMixer != null)
            {
                var sfxGroup = audioMixer.FindMatchingGroups("SFX");
                if (sfxGroup.Length > 0)
                {
                    audioSources[i].outputAudioMixerGroup = sfxGroup[0];
                }
            }
        }
    }

    private void LoadSavedVolumeSettings()
    {
        // Load saved volumes with default of 1f if not found
        float masterVol = PlayerPrefs.GetFloat("MasterVolume", 0.5f);
        float musicVol = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        float sfxVol = PlayerPrefs.GetFloat("SFXVolume", 0.5f);
        float bgVol = PlayerPrefs.GetFloat("BackgroundVolume", 0.5f);

        SetMasterVolume(masterVol);
        SetMusicVolume(musicVol);
        SetSFXVolume(sfxVol);
        SetBackgroundVolume(bgVol);
    }

    #region Main Menu Music
    public void SetUpMainMenuMusic()
    {
        //If the main menu music source gets removed, add a new one
        if(mainMenuMusicSource == null)
        {
            mainMenuMusicSource = gameObject.AddComponent<AudioSource>();
        }

        //Set the music setting
        mainMenuMusicSource.loop = true;
        SetMusicVolume(MusicVolume);

        if (audioMixer != null)
        {
            var musicGroup = audioMixer.FindMatchingGroups("Music");
            if (musicGroup.Length > 0)
            {
                mainMenuMusicSource.outputAudioMixerGroup = musicGroup[0];
            }

            if (mainMenuMusicClip != null && playMenuMusicOnAwake)
            {
                PlayMainMenuMusicAudio(mainMenuMusicClip.name);
            }
        }
    }

    public void PlayMainMenuMusicAudio(string name)
    {
        //Set the background audio source clip to the background audio clip, play the background audio, and fade in the audio
        mainMenuMusicSource.clip = mainMenuMusicClip;
        mainMenuMusicSource.Play();
        StartCoroutine(FadeMenuMusic(fadeInDuration, MusicVolume));
    }

    //Stop the music and fade out the audio
    public void StopMenuMusic()
    {
            StartCoroutine(FadeMenuMusic(fadeOutDuration, 0f));
    }

    
    private IEnumerator FadeMenuMusic(float duration, float targetVolume)
    {
        //Set up timer and start volume variables
        float currentTime = 0;
        float start = mainMenuMusicSource.volume;

        //While the current time is less than the duration, increase the timer and lerp the volume from the start to the target volume
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            mainMenuMusicSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
            yield return null;
        }

        //Stop the background audio if the target volume is 0
        if (targetVolume == 0f)
        {
            mainMenuMusicSource.Stop();
        }
    }
    #endregion

    #region Lose Menu Music
    public void SetUpLoseMenuMusic()
    {
        //If the lose menu music source gets removed, add a new one
        if (loseMenuMusicSource == null)
        {
            loseMenuMusicSource = gameObject.AddComponent<AudioSource>();
        }

        //Set the music setting automatically
        loseMenuMusicSource.loop = true;
        SetLoseMenuMusicVolume(MusicVolume);
    }
    public void PlayLoseMenuMusicAudio()
    {
        //Set the audio source clip to the audio clip, play the audio, and fade in the audio
        loseMenuMusicSource.clip = loseMenuMusicClip;
        loseMenuMusicSource.Play();
        StartCoroutine(FadeLoseMenuMusic(fadeInDuration, MusicVolume));
    }
    //Setter for the background audio volume - must be b/w 0 and 1
    public void SetLoseMenuMusicVolume(float volume)
    {
        MusicVolume = Mathf.Clamp01(volume);
        loseMenuMusicSource.volume = MusicVolume;
    }
    private IEnumerator FadeLoseMenuMusic(float duration, float targetVolume)
    {
        //Set up timer and start volume variables
        float currentTime = 0;
        float start = loseMenuMusicSource.volume;

        //While the current time is less than the duration, increase the timer and lerp the volume from the start to the target volume
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            loseMenuMusicSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
            yield return null;
        }

        //Stop the background audio if the target volume is 0
        if (targetVolume == 0f)
        {
            loseMenuMusicSource.Stop();
        }
    }
    #endregion

    #region Background
    private void SetupBackgroundAudio()
    {
        //If the background audio source gets removed somehow , add a new one
        if (backgroundAudioSource == null)
        {
            backgroundAudioSource = gameObject.AddComponent<AudioSource>();
        }

        //Set the audio source seetings automatically
        backgroundAudioSource.loop = true;
        backgroundAudioSource.volume = BackgroundVolume;

        if (audioMixer != null)
        {
            var backgroundGroup = audioMixer.FindMatchingGroups("Background");
            if(backgroundGroup.Length > 0)
            {
                backgroundAudioSource.outputAudioMixerGroup = backgroundGroup[0];
            }
        }

        //Play the background audio on awake if the boolean is set to true (togglable in the inspector for testing)
        if (backgroundAudioClip != null && playBackgroundOnAwake)
        {
            PlayBackgroundAudio(backgroundAudioClip.name);
        }
    }

    public void PlayBackgroundAudio(string name)
    {
        //Set the background audio source clip to the background audio clip, play the background audio, and fade in the audio
        backgroundAudioSource.clip = backgroundAudioClip;
        backgroundAudioSource.Play();
        StartCoroutine(FadeBackgroundAudio(fadeInDuration, BackgroundVolume));
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
        BackgroundVolume = Mathf.Clamp01(volume);
        backgroundAudioSource.volume = BackgroundVolume;

        if (audioMixer != null)
        {
            audioMixer.SetFloat(BackgroundVolumeParam, ConvertToDecibel(BackgroundVolume));
        }
    }

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
    #endregion

    #region Play Sounds
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
          //  Debug.LogError($"Failed to find sound: {name}");
            return;
        }

        //Set the audio source clip to the found clip and play the sound
        audioSource.clip = audioClip;
        audioSource.Play();
    }

    public void PlayUIClick()
    {
        PlaySound("UI - Button Popup");
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
          //  Debug.LogError($"Failed to find sound: {name}");
            return;
        }

        // Set pitch and play
        audioSource.pitch = Random.Range(minPitch, maxPitch);
        audioSource.clip = audioClip;
        audioSource.Play();

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
    #endregion

    #region Getters
    public string GetRandomSound(List<string> names)
    {
        //Handle exception for empty or null list otherwise use Random.Rnage between 0 and the count of the list
        if (names == null || names.Count == 0)
        {
          //  Debug.LogError("Sound list is empty or null");
            return string.Empty;
        }
        return names[Random.Range(0, names.Count)];
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
        //    Debug.LogWarning("No available audio sources");
            return null;
    }

    public float GetVolumeFromMixer(string parameterName)
    {
        float volume;
        if(audioMixer != null && audioMixer.GetFloat(parameterName, out volume))
        {
            return Mathf.Pow(10f, volume / 20f);
        }
        return 1f;
    }
    #endregion

    #region Setters
    public void SetMasterVolume(float volume)
    {
        float dbValue = ConvertToDecibel(volume);
        audioMixer.SetFloat(MasterVolumeParam, dbValue);
        PlayerPrefs.SetFloat("MasterVolume", volume);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float volume)
    {
        float dbValue = ConvertToDecibel(volume);
        audioMixer.SetFloat(SFXVolumeParam, dbValue);
        PlayerPrefs.SetFloat("SFXVolume", volume);
        PlayerPrefs.Save();
    }
   
    public void SetMusicVolume(float volume)
    {
        MusicVolume = Mathf.Clamp01(volume);

        // Update the menu music source volume
        if (mainMenuMusicSource != null)
            mainMenuMusicSource.volume = MusicVolume;

        // Update the lose menu music source volume
        if (loseMenuMusicSource != null)
            loseMenuMusicSource.volume = MusicVolume;

        // Update the mixer
        if (audioMixer != null)
        {
            float dbValue = ConvertToDecibel(MusicVolume);
            audioMixer.SetFloat(MusicVolumeParam, dbValue);
        }

        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
    }

    public void SetBackgroundVolume(float volume)
    {
        float dbValue = ConvertToDecibel(volume);
        audioMixer.SetFloat(BackgroundVolumeParam, dbValue);
        PlayerPrefs.SetFloat("BackgroundVolume", volume);
        PlayerPrefs.Save();
    }
    #endregion
}

//TODO::