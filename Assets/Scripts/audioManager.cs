using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class audioManager : MonoBehaviour
{
    public static audioManager instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource audioSourceTemplate;
    [SerializeField] private AudioSource backgroundAudioSource;
    private AudioSource[] audioSources;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip[] audioClips;
    [SerializeField] private AudioClip backgroundAudioClip;

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

    [Header("Audio Settings")]
    [SerializeField] private float defaultBackgroundVolume = 0.5f;
    [SerializeField] private float defaultSoundVolume = 1f;
    [SerializeField] private bool playMusicOnAwake = true;
    [SerializeField] private float fadeInDuration = 2f;
    [SerializeField] private float fadeOutDuration = 1f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            InitializeAudioSources();
            SetupBackgroundMusic();
        }
        else if (instance != this)
        {
            Debug.LogError($"Destroying audio manager on {gameObject.name} because instance already exists");
            Destroy(gameObject);
        }
    }

    private void InitializeAudioSources()
    {
        audioSources = new AudioSource[audioClips.Length];
        for (int i = 0; i < audioClips.Length; i++)
        {
            audioSources[i] = Instantiate(audioSourceTemplate, transform);
            audioSources[i].volume = defaultSoundVolume;
        }
    }

    private void SetupBackgroundMusic()
    {
        if (backgroundAudioSource == null)
        {
            backgroundAudioSource = gameObject.AddComponent<AudioSource>();
        }

        backgroundAudioSource.loop = true;
        backgroundAudioSource.playOnAwake = false;
        backgroundAudioSource.volume = defaultBackgroundVolume;

        if (backgroundAudioClip != null && playMusicOnAwake)
        {
            PlayBackgroundAudio(backgroundAudioClip.name);
        }
    }

    public void PlaySound(string name)
    {
        AudioSource audioSource = null;
        AudioClip audioClip = null;

        // Find available audio source
        foreach (AudioSource source in audioSources)
        {
            if (!source.isPlaying)
            {
                audioSource = source;
                break;
            }
        }

        if (audioSource == null)
        {
            Debug.LogWarning("No available audio sources");
            return;
        }

        // Find matching audio clip
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

        audioSource.clip = audioClip;
        audioSource.Play();
    }

    public void PlayBackgroundAudio(string name)
    {
        AudioClip musicClip = null;
        foreach (AudioClip clip in audioClips)
        {
            if (clip.name == name)
            {
                musicClip = clip;
                break;
            }
        }

        if (musicClip == null)
        {
            Debug.LogError($"Failed to find audio: {name}");
            return;
        }

        backgroundAudioSource.clip = musicClip;
        backgroundAudioSource.Play();
        StartCoroutine(FadeBackgroundAudio(fadeInDuration, defaultBackgroundVolume));
    }

    public void StopBackgroundAudio()
    {
        StartCoroutine(FadeBackgroundAudio(fadeOutDuration, 0f));
    }

    public void SetBackgroundAudioVolume(float volume)
    {
        backgroundAudioSource.volume = Mathf.Clamp01(volume);
    }

    public string GetRandomSound(List<string> names)
    {
        if (names == null || names.Count == 0)
        {
            Debug.LogError("Sound list is empty or null");
            return string.Empty;
        }
        return names[Random.Range(0, names.Count)];
    }

    public void PlayRandomDamageSound()
    {
        PlaySound(GetRandomSound(damageSounds));
    }

    public void PlayRandomDeathSound()
    {
        PlaySound(GetRandomSound(deathSounds));
    }

    public IEnumerator DelayPlaySound(string name, float delay)
    {
        yield return new WaitForSeconds(delay);
        PlaySound(name);
    }

    private IEnumerator FadeBackgroundAudio(float duration, float targetVolume)
    {
        float currentTime = 0;
        float start = backgroundAudioSource.volume;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            backgroundAudioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
            yield return null;
        }

        if (targetVolume == 0f)
        {
            backgroundAudioSource.Stop();
        }
    }
}
