using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disease : MonoBehaviour
{
    [SerializeField] float expansionSpeed = 0.5f;   // Rate at which disease spreads
    [SerializeField] float maxRadius = 15f;         // Max spread of disease
    [SerializeField] float rotationSpeed = 1f;

    float timer = 0f;
    float damageInterval = 1f; // Interval of 1 second

    List<GameObject> plantsInZone = new List<GameObject>();

    private GameObject model;

    void Start()
    {
        model = transform.Find("Aura").gameObject;
    }

    void ExpandRadius()
    {
        float currentRadius = transform.localScale.x;

        if (currentRadius < maxRadius)
        {
            currentRadius += expansionSpeed * Time.deltaTime;
            transform.localScale = Vector3.one * currentRadius;
            GetComponent<Collider>().transform.localScale = Vector3.one * currentRadius;
        }
    }

    void RotateModel()
    {
        model.transform.Rotate(rotationSpeed * Time.deltaTime * Vector3.up);
    }

    // Damage each plant in zone
    void DamagePlants()
    {
        foreach (GameObject plant in plantsInZone)
        {
            Health plantHealth = plant.GetComponentInChildren<Health>();

            if (plantHealth != null)
            {
                plantHealth.takeDamage(5);
            }
            else
            {
                Debug.LogWarning("Health script not found on child object!");
            }
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("plant"))
        {
            plantsInZone.Add(collider.gameObject);
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("plant"))
        {
            plantsInZone.Remove(collider.gameObject);
        }
    }

    void Update()
    {
        if (!GameManager.Instance.GameSimulationActive) return;
        ExpandRadius();
        RotateModel();

        // Cause damage every second
        timer += Time.deltaTime;
        if (timer >= damageInterval)
        {
            DamagePlants();
            timer = 0f;
        }
    }
}
