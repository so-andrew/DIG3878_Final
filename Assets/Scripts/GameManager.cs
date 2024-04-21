using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
        PlayerCurrency = startingPlayerCurrency;
    }

    // Public variables
    [Header("Level Number")]
    [Range(0, 1)]
    [Tooltip("Level is zero-indexed (i.e. level = 0 is the first level).")]
    public int level = 0;


    [Header("Mouse Handler")]
    [Tooltip("Script that handles mouse click functions.")]
    public MouseClick MouseClickHandler;

    [Header("Game Objects")]
    [Tooltip("Transform that contains all placed plants.")]
    public Transform placedItemParent;

    [Header("UI Elements")]
    public GameObject mainUI;
    public GameObject shopUI;
    public GameObject inventoryUI;
    public GameObject questUI;
    public QuestPopup questPopup;
    public HealthUI healthUI;

    [Header("Player Currency")]
    [SerializeField] private float startingPlayerCurrency = 500f;

    [Header("Player Health")]
    public float gameHealth = 100f;
    public float maxPlayerHealth = 100f;
    [Tooltip("This factor is multiplied by the percentage of sick plants to determine health drop rate.")]
    public float healthDecreaseFactor = 1f;
    [Tooltip("This factor is multiplied by the percentage of healthy plants to determine health regen rate.")]
    public float healthIncreaseFactor = 0.5f;

    // Public get, private set variables
    public float PlayerCurrency { get; private set; }
    public int HealCount { get; private set; }
    public int CoinCollectedCount { get; private set; }
    public bool GameSimulationActive { get; private set; }
    public GameObject SelectedButton { get; private set; }
    public MouseMode CurrentMouseMode { get; private set; }
    public MouseMode PreviousMouseMode { get; private set; }
    public TMP_Text mouseModeDisplay;
    public TMP_Text previousMouseModeDisplay;


    // Private variables
    private Dictionary<Item.ItemType, int> inventoryDict = new Dictionary<Item.ItemType, int>();
    private Dictionary<Item.ItemType, int> spawnedItems = new Dictionary<Item.ItemType, int>();
    private bool shopUIActive = false;
    private bool mainUIActive = true;
    private bool inventoryUIActive = false;
    private bool questUIActive = false;
    private bool gameWin = false;

    void Start()
    {
        // Set active UIs (by default only mainUI)
        shopUI.SetActive(shopUIActive);
        mainUI.SetActive(mainUIActive);
        inventoryUI.SetActive(inventoryUIActive);
        questUI.SetActive(questUIActive);

        // Initialize game variables
        CurrentMouseMode = MouseMode.Default;
        GameSimulationActive = true;
        HealCount = 0;
    }

    void Update()
    {
        TrackGameHealth();
        CheckIfWin();
        if (mouseModeDisplay && previousMouseModeDisplay)
        {
            mouseModeDisplay.text = CurrentMouseMode.ToString();
            previousMouseModeDisplay.text = PreviousMouseMode.ToString();
        }
    }

    private void CheckIfWin()
    {
        if (gameHealth > 0f && QuestManager.Instance.GetCompletedQuestCount() == QuestManager.Instance.GetTotalQuestCount())
        {
            if (!gameWin)
            {
                Debug.Log("Game win");
            }
            gameWin = true;
            GameSimulationActive = false;
            LevelComplete();
        }
    }

    private void LevelComplete()
    {
        switch (level)
        {
            case 0:
                // Go to level 2 scene
                // OR show win screen with button to go to main menu (level select)
                break;
            case 1:
                // show win screen with button to go to main menu
                break;
        }
    }

    private void GameOver()
    {
        // Go to game over screen
        Debug.Log("Game over");
    }

    private void TrackGameHealth()
    {
        if (!GameSimulationActive) return;
        // Get how many plants are spawned
        int childCount = placedItemParent.childCount;
        int lowHealthPlants = 0;

        // Check how many of them are low health
        foreach (Transform child in placedItemParent)
        {
            Health childHealthScript = child.GetComponentInChildren<Health>();
            if (childHealthScript.health < childHealthScript.maxHealth * 0.25f)
            {
                lowHealthPlants++;
            }
        }
        //if (lowHealthPlants > 0) Debug.Log($"low health plants: {lowHealthPlants}/{childCount}");

        float netHealthChange = 0;
        if (childCount > 0)
        {
            float amountDecrease = (float)lowHealthPlants / childCount * healthDecreaseFactor * Time.deltaTime;
            float amountIncrease = (float)(childCount - lowHealthPlants) / childCount * healthIncreaseFactor * Time.deltaTime;

            //Debug.Log($"Net health change = {amountIncrease - amountDecrease}");
            netHealthChange = amountIncrease - amountDecrease;
            gameHealth = Mathf.Min(100f, gameHealth + netHealthChange);
        }

        UpdateHealthSlider(netHealthChange);
        if (gameHealth <= 0f)
        {
            GameOver();
        }
    }

    // Add x amount of item to inventory dictionary
    public void AddToInventory(Item.ItemType type, int amount)
    {
        if (inventoryDict.TryGetValue(type, out int currentAmount))
        {
            inventoryDict[type] = currentAmount + amount;
        }
        else
        {
            inventoryDict.Add(type, amount);
        }
    }

    // Remove x amount of item from inventory dictionary
    public void RemoveFromInventory(Item.ItemType type, int amount)
    {
        if (inventoryDict.TryGetValue(type, out int currentAmount))
        {
            inventoryDict[type] = currentAmount - amount;
            if (inventoryDict[type] <= 0)
            {
                inventoryDict.Remove(type);
            }
        }
    }

    // Return how many of x item player has in inventory
    public int GetInventoryItemAmount(Item.ItemType type)
    {
        if (inventoryDict.TryGetValue(type, out int currentAmount)) return currentAmount;
        return 0;
    }

    // Return how many different items player has (size of dict)
    public int GetInventoryTotalItemTypes()
    {
        return inventoryDict.Count;
    }

    // Return collection containing the types of items in inventory
    public Dictionary<Item.ItemType, int>.KeyCollection GetInventoryKeys()
    {
        return inventoryDict.Keys;
    }

    // Increments counter that tracks how many of each item has been placed
    public void IncrementSpawnCounter(Item.ItemType type)
    {
        if (spawnedItems.TryGetValue(type, out int currentAmount))
        {
            spawnedItems[type] = currentAmount + 1;
        }
        else
        {
            spawnedItems.Add(type, 1);
        }
    }

    // Tracks number of heals
    public void IncrementHealCounter()
    {
        HealCount += 1;
    }

    // Tracks number of coins collected
    public void IncrementCoinCollectCounter()
    {
        CoinCollectedCount += 1;
    }

    public void ChangePlayerCurrency(float amount)
    {
        PlayerCurrency = Mathf.Max(0, PlayerCurrency + amount);
        UpdateShopDisplay();
    }
    // Return how many of x item has been placed
    public int GetSpawnedTotal(Item.ItemType type)
    {
        if (spawnedItems.TryGetValue(type, out int currentAmount)) return currentAmount;
        return 0;
    }

    public void SetPreviousMouseMode(MouseMode mouseMode)
    {
        // Debug.Log($"Setting previous to {mouseMode}");
        PreviousMouseMode = mouseMode;
    }

    // Set current mouse mode
    public void SetCurrentMouseMode(MouseMode mouseMode)
    {
        // Debug.Log($"Setting mouse mode to {mouseMode}, current = {CurrentMouseMode}, previous = {PreviousMouseMode}");
        // Supposed to prevent mousemode being stuck on UI
        if (CurrentMouseMode != MouseMode.UI) PreviousMouseMode = CurrentMouseMode;
        CurrentMouseMode = mouseMode;
    }

    // Called when mouse enters UI button
    public void ButtonHoverEnter()
    {
        SetCurrentMouseMode(MouseMode.UI);
    }

    // Called when mouse enters UI button
    public void ButtonHoverExit()
    {
        SetCurrentMouseMode(PreviousMouseMode);
    }

    // Highlight selected button in inventory 
    public void SetSelectedButton(GameObject button)
    {
        if (SelectedButton != null)
        {
            SelectedButton.transform.Find("Background").GetComponent<Image>().color = Color.white;
        }
        SelectedButton = button;
        SelectedButton.transform.Find("Background").GetComponent<Image>().color = new Color32(182, 226, 140, 255);
    }

    // Clear selected button highlight
    public void ClearSelectedButton()
    {
        if (SelectedButton != null)
        {
            SelectedButton.transform.Find("Background").GetComponent<Image>().color = Color.white;
        }
        SelectedButton = null;
    }

    // Set shop UI active
    public void ShowShopUI()
    {
        shopUIActive = true;
        mainUIActive = !shopUIActive;
        CurrentMouseMode = MouseMode.UI;
        ToggleUI();
    }

    // Set shop UI inactive
    public void HideShopUI()
    {
        shopUIActive = false;
        mainUIActive = !shopUIActive;
        CurrentMouseMode = MouseMode.Default;
        ToggleUI();
    }

    // Set inventory UI active
    public void ShowInventoryUI()
    {
        inventoryUIActive = true;
        mainUIActive = !inventoryUIActive;
        CurrentMouseMode = MouseMode.Default;
        ToggleUI();
    }

    // Set inventory UI inactive
    public void HideInventoryUI()
    {
        ClearSelectedButton();
        inventoryUIActive = false;
        mainUIActive = !inventoryUIActive;
        CurrentMouseMode = MouseMode.Default;
        ToggleUI();
    }

    // Set quest UI active
    public void ShowQuestUI()
    {
        questUIActive = true;
        mainUIActive = !questUIActive;
        CurrentMouseMode = MouseMode.UI;
        ToggleUI();
    }

    // Set quest UI inactive
    public void HideQuestUI()
    {
        questUIActive = false;
        mainUIActive = !questUIActive;
        CurrentMouseMode = MouseMode.Default;
        ToggleUI();
    }

    // Helper function to toggle different UI elements
    private void ToggleUI()
    {
        mainUI.SetActive(mainUIActive);
        shopUI.SetActive(shopUIActive);
        inventoryUI.SetActive(inventoryUIActive);
        questUI.SetActive(questUIActive);
    }

    // Call InventoryMenu.GenerateButtons()
    public void UpdateInventoryDisplay()
    {
        inventoryUI.GetComponent<InventoryMenu>().GenerateButtons();
    }

    // Call Questmenu.GenerateQuests()
    public void UpdateQuestDisplay()
    {
        questUI.GetComponent<QuestMenu>().GenerateQuests();
    }

    public void UpdateShopDisplay()
    {
        shopUI.GetComponent<ShopMenu>().GenerateShop();
    }

    // Update HealthUI
    private void UpdateHealthSlider(float change)
    {
        healthUI.ToggleGainIndicator(change);
        healthUI.SetSliderValue(gameHealth);
    }
}
