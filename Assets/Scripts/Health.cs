using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    int health = 100;
    [SerializeField] Slider slider;
    [SerializeField] Camera playerCamera;
    [SerializeField] Vector3 offset = new Vector3(0, 1, 0);

    // Start is called before the first frame update
    void Start()
    {
        // Find the main camera in the scene and assign it to playerCamera
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        if (playerCamera == null)
        {
            Debug.LogError("Main camera not found in the scene!");
        }

        UpdateSlider();
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = playerCamera.transform.rotation; // health bar always face camera
        transform.position = transform.parent.position + offset; // health bar positioned according to parent object (plant) location
    }

    public void takeDamage(int damage)
    {
        if (health > 0)
        {
            if (damage > health)
            {
                // TODO: die
                health = 0;
            }
            else
            {
                health -= damage;
            }
            UpdateSlider();
        }
    }

    void UpdateSlider()
    {
        slider.value = (float)health / 100;
    }
}
