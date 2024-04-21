using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    float health = 100f;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
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
        }
    }
}
