using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
        audioSource = GetComponent<AudioSource>();
    }

    public void Play(AudioClip clip)
    {
        if (audioSource == null) return;
        audioSource.PlayOneShot(clip);
    }

    public void Play(AudioClip clip, float volume)
    {
        if (audioSource == null) return;
        audioSource.PlayOneShot(clip, volume);
    }
}
