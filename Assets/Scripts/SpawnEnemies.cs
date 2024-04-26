using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemies : MonoBehaviour
{
    [SerializeField] GameObject enemy;
    [SerializeField] int maxEnemies;

    List<GameObject> enemySpawnPoints = new List<GameObject>();
    int numSpawnPoints;

    float timer = 0f;
    float spawnInterval = 10f;

    void Start()
    {
        foreach (GameObject spawnPt in GameObject.FindGameObjectsWithTag("EnemySpawnPoint"))
        {
            enemySpawnPoints.Add(spawnPt);
            numSpawnPoints++;
        }
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval && GameObject.FindGameObjectsWithTag("Enemy").Length < maxEnemies)
        {
            SpawnEnemy();
            timer = 0f;
        }
    }

    void SpawnEnemy()
    {
        int index = Random.Range(0, numSpawnPoints);
        GameObject spawnPt = enemySpawnPoints[index];
        if (spawnPt != null)
        {
            Instantiate(enemy, spawnPt.transform.position, spawnPt.transform.rotation);
        }
        else
        {
            Debug.Log("Problem getting spawn point");
        }

    }
}
