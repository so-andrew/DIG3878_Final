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
        Debug.Log("Playing " + clip.name);
        audioSource.PlayOneShot(clip);
    }

    public void Play(AudioClip clip, float volume)
    {
        if (audioSource == null) return;
        Debug.Log("Playing " + clip.name);
        audioSource.PlayOneShot(clip, volume);
    }
}
