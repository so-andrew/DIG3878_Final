using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverAudio : MonoBehaviour
{
    public AudioClip loseSfx;
    [Range(0, 1)]
    public float loseSfxVolume = 1;

    IEnumerator Start()
    {
        BackgroundMusic.Instance.StopMusic();
        AudioManager.Instance.Play(loseSfx, loseSfxVolume);
        yield return new WaitForSeconds(loseSfx.length + 2.5f);
        BackgroundMusic.Instance.PlayMusic("gameOverTheme");
    }

}
