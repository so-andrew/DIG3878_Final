using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnMedicine : MonoBehaviour
{
    [SerializeField] GameObject medicinePrefab;

    [SerializeField] float minInterval = 5f;        // Minimum time interval in seconds (for spawning meds)
    [SerializeField] float maxInterval = 10f;       // Max time interval in seconds (for spawning meds)
    [SerializeField] float medsLifetime = 5f;       // Lifetime of medicine in seconds
    [SerializeField] Vector3 offset = new Vector3(0, 2, 0);

    [SerializeField] GameObject boundingPlane;


    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("Spawn", Random.Range(minInterval, maxInterval), Random.Range(minInterval, maxInterval));
    }

    void Spawn()
    {
        // Get the mesh renderer of the plane object
        MeshRenderer planeRenderer = boundingPlane.GetComponent<MeshRenderer>();

        if (planeRenderer != null)
        {
            // Get the bounds of the plane object
            Bounds bounds = planeRenderer.bounds;
            
            // Randomly select a position within the bounds of the plane
            Vector3 randomPosition = new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y),
                Random.Range(bounds.min.z, bounds.max.z)
            );

            // Spawn the object at the random position
            GameObject medicine = Instantiate(medicinePrefab, randomPosition+offset, medicinePrefab.transform.rotation);
            Destroy(medicine, medsLifetime); // Destroy the medicine object after medsLifetime seconds

        }
        else
        {
            Debug.LogError("Plane object does not have a MeshRenderer component.");
        }
    }
}
