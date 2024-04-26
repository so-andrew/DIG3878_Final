using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiseaseManager : MonoBehaviour
{
    [SerializeField] GameObject plagePrefab;
    [SerializeField] float minInterval = 5f;       // Minimum time interval in seconds (for spawning disease)
    [SerializeField] float maxInterval = 10f;       // Max time interval in seconds (for spawning disease)

    float startTime;                                // Record the time when the game starts for logging

    void Start()
    {
        startTime = Time.time;

        // Call the method at random intervals within the specified range
        InvokeRepeating("SpawnPlague", Random.Range(minInterval, maxInterval), Random.Range(minInterval, maxInterval));
    }

    void SpawnPlague()
    {
        float elapsedTime = Time.time - startTime;
        //Debug.Log($"[{elapsedTime:F2} seconds] started plague!!");

        // Choose a random plant to get disease
        Transform placedItemParentTransform = GameManager.Instance.placedItemParent.transform;
        int numPlants = placedItemParentTransform.childCount;

        if (numPlants > 0) // No disease if no plants are placed
        {
            int randomPlantIndex = Random.Range(0, numPlants);

            // Get chosen plant
            GameObject chosenPlant = placedItemParentTransform.GetChild(randomPlantIndex).gameObject;
            Health plantHealth = chosenPlant.GetComponentInChildren<Health>();

            // Check if plant has been recently healed (serves as a reinfection cooldown)
            if (!plantHealth.recentlyHealed)
            {
                // Get position of the chosen plant
                Vector3 startPosition = chosenPlant.transform.position;

                // Instantiate disease (no rotation)
                Instantiate(plagePrefab, startPosition, Quaternion.identity);
            }
            else
            {
                Debug.Log("This plant has been recently healed, skipping");
            }
        }
    }
}
