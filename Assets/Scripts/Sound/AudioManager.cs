using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioSource bgmSource; // 음악을 재생할 오디오 소스
    public AudioSource sfxSource; // 효과음을 재생할 오디오 소스

    public AudioClip[] bgmClips;  // bgm, 음악 소스 배열
    public AudioClip[] sfxClips; // 효과음 소스 배열

    private void Awake()    // singleton 구현
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    public void PlayBGM(int bgmIndex)   // bgm, 음악 소스 재생 
    {
        if (bgmIndex < 0 || bgmIndex >= bgmClips.Length)
        {
            Debug.LogWarning("Invalid bgm index!");
            return;
        }

        bgmSource.clip = bgmClips[bgmIndex];
        bgmSource.Play();
    }

    public void PlaySFX(int sfxIndex)   // 효과음 재생
    {
        if (sfxIndex < 0 || sfxIndex >= sfxClips.Length)
        {
            Debug.LogWarning("Invalid SFX index!");
            return;
        }

        sfxSource.clip = sfxClips[sfxIndex];
        sfxSource.Play();
    }

    public void SetBGMVolume(float volume)  // 음악 음량 조절
    {
        bgmSource.volume = volume;
    }

    public void SetSFXVolume(float volume)  // 효과음 음량 조절
    {
        sfxSource.volume = volume;
    }
}