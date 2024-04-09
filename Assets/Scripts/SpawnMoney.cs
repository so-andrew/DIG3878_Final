using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnMoney : MonoBehaviour
{
    [SerializeField] GameObject moneyPrefab;
    [SerializeField] Vector3 offset = new Vector3(0, 1, 0);
    [SerializeField] float minInterval = 5f;        // Minimum time interval in seconds (for spawning money)
    [SerializeField] float maxInterval = 10f;       // Max time interval in seconds (for spawning money)
    [SerializeField] float moneyLifetime = 5f;      // Lifetime of money objects in seconds

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("Spawn", Random.Range(minInterval, maxInterval), Random.Range(minInterval, maxInterval));
    }

    void Spawn()
    {
        GameObject money = Instantiate(moneyPrefab, transform.position+offset, Quaternion.identity, transform);
        Destroy(money, moneyLifetime); // Destroy the money object after moneyLifetime seconds
    }

}
