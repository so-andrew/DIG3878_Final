using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverAudio : MonoBehaviour
{
    public AudioClip loseSfx;
    [Range(0, 1)]
    public float loseSfxVolume = 1;
    // Start is called before the first frame update
    void Start()
    {
        AudioManager.Instance.Play(loseSfx, loseSfxVolume);
    }

}
