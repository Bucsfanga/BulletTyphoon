using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class audioManager : MonoBehaviour
{
    //Use a singleton pattern
    public static audioManager instance;
    //Create an array of audioclips for use in the inspector
    public AudioClip[] AudioClips;
    //Use a template audio source var as temp
    public AudioSource audioSourceTemplate;
    //Initialize an array of audiosources  (number of audio clips)
    public AudioSource[] audioSources;
    //Audio source for background music
    public AudioSource backgroundAudioSource;

    public List<string> damageSounds = new List<string> { "Player Hurt 1", "Player Hurt 2", "Player Hurt 3", "Player Hurt 4", "Player Hurt 5", "Player Hurt 6" };
    public List<string> deathSounds = new List<string> {"Death 1", "Death 2", "Death 3", "Death 4", "Death 5"};


private void Awake()
    {
        //Singleton method instance assignment
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("Should not have more than 1 audio manager");
        }

        //Create dedicated background audio source, set it to loop, and not play on awake
        backgroundAudioSource = gameObject.AddComponent<AudioSource>();
        backgroundAudioSource.loop = true;
        backgroundAudioSource.playOnAwake = false;

        audioSources = new AudioSource[AudioClips.Length];
        //Make a copy of the audioSourceTemplate for the size of the audioSources array
        for (int i = 0; i < AudioClips.Length; i++)
        {
            audioSources[i] = Instantiate(audioSourceTemplate, transform);
        }
    }

    //Background audio functions
    public void PlayBackgroundAudio(string name)
    {
        AudioClip audioClip = null;
        foreach (AudioClip clip in AudioClips)
        {
            if (clip.name == name)
            {
                audioClip = clip;
                break;
            }
        }
        if (audioClip == null)
        {
            Debug.LogError("Failed to find sound: " + name);
            return;
        }

        backgroundAudioSource.clip = audioClip;
        backgroundAudioSource.Play();
    }
    public void StopBackgroundAudio()
    {
        backgroundAudioSource.Stop();
    }



    public void PlaySound(string name)
    {
        //Need an audio source to play the audio clip (need 2 variables)
        AudioSource audioSource = null;
        AudioClip audioClip = null;
        //Need to find an available audio source to play clip on
        foreach (AudioSource source in audioSources)
        {
            if (!source.isPlaying)
            {
                audioSource = source;
                break;
            }
        }
        if (audioSource == null)
            return;
        //Find the audio clip from input
        foreach (AudioClip clip in AudioClips)
        {
            if (clip.name == name)
            {
                audioClip = clip;
                break;
            }
        }
        if (audioClip == null)
        {
            Debug.LogError("Failed to find sound: " + name);
            return;
        }
        audioSource.clip = audioClip;
        audioSource.Play();
    }

    //Created since we have multiple sets of sounds that it would be best to pick a random sound from (6 damaging sounds, etc.)
    public string GetRandomSound(List<string> names)
    {
        return names[Random.Range(0, names.Count)];
    }

    public IEnumerator DelayPlaySound(string name, float delay)
    {
        PlaySound(name);
        yield return new WaitForSeconds(delay);
        PlaySound(name);
    }
}
