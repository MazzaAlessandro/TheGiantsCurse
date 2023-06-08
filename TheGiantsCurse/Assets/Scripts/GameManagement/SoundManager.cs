using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [SerializeField] private AudioSource musicSource, effectSource;

    private bool playingMusic = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
        
    }

    public void PlayEffect(AudioClip clip)
    {
        effectSource.PlayOneShot(clip);
    }

    public void PlayMultipleTimes(AudioClip clip, float clipDuration, float times)
    {
        StartCoroutine(Multiple(clip, clipDuration, times));
    }

    private IEnumerator Multiple(AudioClip clip, float clipDuration, float times) {
        int counter = 0;

        while (counter < times)
        {
            effectSource.PlayOneShot(clip);
            yield return new WaitForSeconds(clipDuration);
            counter++;
        }
    }

    public void PlayMusic(AudioClip clip)
    {
        if (playingMusic)
        {
            musicSource.Stop();
            musicSource.clip = clip;
            musicSource.Play();
        }
        else
        {
            playingMusic = true;
            musicSource.clip = clip;
        }
        
    }

    public void SetMasterVolume(float volume)
    {
        AudioListener.volume = volume;
    }

    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
    }

    public void SetEffectVolume(float volume)
    {
        effectSource.volume = volume;
    }
}
