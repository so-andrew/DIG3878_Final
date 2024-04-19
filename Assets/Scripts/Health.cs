using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public float health = 100f;
    public float healCooldownTime = 60f;
    public bool recentlyHealed = false;
    private float healTimer = 0f;
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

        UpdateHealTimer();
    }

    private void UpdateHealTimer()
    {
        if (healTimer > 0) healTimer -= Time.deltaTime;
        else recentlyHealed = false;
    }

    public void takeDamage(float damage)
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

    public void healDamage(float healAmount)
    {
        if (health + healAmount > 100f) health = 100f;
        else
        {
            health += healAmount;
            recentlyHealed = true;
            healTimer = healCooldownTime;
        }
        UpdateSlider();
    }

    void UpdateSlider()
    {
        slider.value = health / 100;
    }
}
