using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public AudioClip deathSfx;
    [Range(0, 1)]
    public float deathClipVolume = 1;
    public AudioClip splatSfx;
    [Range(0, 1)]
    public float splatClipVolume = 1;
    private Animator animator;

    float health = 100f;
    bool deathAnimPlayed = false;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
        {
            if (!deathAnimPlayed)
            {
                deathAnimPlayed = true;
                GameManager.Instance.IncrementEnemyCounter();
                AudioManager.Instance.Play(deathSfx, deathClipVolume);
                animator.Play("Die");
            }
        }
    }

    public void TakeDamage(float damage)
    {
        if (health > 0)
        {
            if (damage > health)
            {
                health = 0;
            }
            else
            {
                health -= damage;
            }
            AudioManager.Instance.Play(splatSfx, splatClipVolume);
        }
    }
}
