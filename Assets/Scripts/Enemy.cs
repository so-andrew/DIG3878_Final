using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    void OnTriggerStay(Collider collider)
    {
        if (collider.CompareTag("plant"))
        {
            Debug.Log("Enemy damage plant");
            DamagePlant(collider.gameObject);
        }
    }

    void DamagePlant(GameObject plant)
    {
        Health plantHealth = plant.GetComponentInChildren<Health>();
        if (plantHealth != null)
        {
            plantHealth.takeDamage(0.1f);
        }
        else
        {
            Debug.LogWarning("Health script not found on child object!");
        }
    }
}
