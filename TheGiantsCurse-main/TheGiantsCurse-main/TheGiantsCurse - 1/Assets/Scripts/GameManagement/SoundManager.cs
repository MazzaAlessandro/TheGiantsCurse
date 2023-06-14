using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public enum Phases { MENU, PUZZLE, RACING};

    public static SoundManager instance;

    [SerializeField] private AudioSource musicSource, effectSource;

    [SerializeField] private AudioClip menuMusic, puzzleMusic, racingMusic;

    private bool playingMusic = false;
    private Phases currPhase;
    private float currVolume = 1;

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

    public void PlayMusic(Phases phase)
    {
        if (playingMusic)
        {
            if (phase != currPhase)
            {
                musicSource.Stop();
                musicSource.clip = GetRespectiveClip(phase);
                musicSource.Play();
                currPhase = phase;
            }
        }
        else
        {
            playingMusic = true;
            musicSource.clip = GetRespectiveClip(phase);
            musicSource.Play();
            currPhase = phase;
        }
        
    }

    private AudioClip GetRespectiveClip(Phases phase)
    {
        switch (phase)
        {
            case Phases.MENU:
                return menuMusic;
            case Phases.PUZZLE:
                return puzzleMusic;
            case Phases.RACING:
                return racingMusic;
            default:
                return menuMusic;
        }
    }

    public void SetMasterVolume(float volume)
    {
        AudioListener.volume = volume;
        currVolume = volume;
    }

    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
        currVolume = volume;
    }

    public void SetEffectVolume(float volume)
    {
        effectSource.volume = volume;
        currVolume = volume;
    }

    public float GetCurrVolume()
    {
        return currVolume;
    }
}
