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

    private Health plantHealth;
    private bool spawning = true;

    // Start is called before the first frame update
    void Start()
    {
        plantHealth = transform.GetComponentInChildren<Health>();
        BeginSpawning();
    }

    void Update()
    {
        SpawnCheck();
    }

    private void SpawnCheck()
    {
        // If money is not currently spawning and plant is healthy enough, begin spawning
        if (!spawning && plantHealth.health >= 80f)
        {
            BeginSpawning();
            spawning = true;
        }
        // If money is spawning and plant is not healthy enough, stop spawning
        else if (spawning && plantHealth.health < 80f)
        {
            CancelInvoke();
            spawning = false;
        }
    }

    private void BeginSpawning()
    {
        InvokeRepeating("Spawn", Random.Range(minInterval, maxInterval), Random.Range(minInterval, maxInterval));
    }

    void Spawn()
    {
        GameObject money = Instantiate(moneyPrefab, transform.position + offset, Quaternion.identity, transform);
        Destroy(money, moneyLifetime); // Destroy the money object after moneyLifetime seconds
    }

}
