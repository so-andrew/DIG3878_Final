using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    public static BackgroundMusic Instance;
    private AudioSource audioSource;

    public AudioClip mainTheme;
    [Range(0, 1)]
    public float mainThemeVolume = 1f;
    public AudioClip gameOverTheme;
    [Range(0, 1)]
    public float gameOverVolume = 1f;
    public AudioClip level1Theme;
    [Range(0, 1)]
    public float level1Volume = 1f;
    public AudioClip level2Theme;
    [Range(0, 1)]
    public float level2Volume = 1f;
    public AudioClip lowHealthTheme;
    [Range(0, 1)]
    public float lowHealthVolume = 1f;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
        audioSource = GetComponent<AudioSource>();
    }

    public void StopMusic()
    {
        audioSource.Stop();
    }

    public void PlayMusic(string s)
    {
        switch (s)
        {
            case "mainTheme":
                audioSource.clip = mainTheme;
                audioSource.volume = mainThemeVolume;
                audioSource.Play();
                break;
            case "gameOverTheme":
                audioSource.clip = gameOverTheme;
                audioSource.volume = gameOverVolume;
                audioSource.Play();
                break;
            case "level1Theme":
                audioSource.clip = level1Theme;
                audioSource.volume = level1Volume;
                audioSource.Play();
                break;
            case "level2Theme":
                audioSource.clip = level2Theme;
                audioSource.volume = level2Volume;
                audioSource.Play();
                break;
            case "lowHealthTheme":
                audioSource.clip = lowHealthTheme;
                audioSource.volume = lowHealthVolume;
                audioSource.Play();
                break;
            default:
                Debug.Log("No such AudioClip");
                break;
        }
    }
}
