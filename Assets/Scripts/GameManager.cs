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
        playerCurrency = startingPlayerCurrency;
    }

    // Public variables
    public PlaceItemsDemo ItemPlacer;
    public Transform placedItemParent;
    public MouseMode CurrentMouseMode { get; private set; }
    public MouseMode PreviousMouseMode { get; private set; }
    public GameObject mainUI;
    public GameObject shopUI;
    public GameObject inventoryUI;
    public GameObject questUI;
    public GameObject questPopup;
    [SerializeField] private float startingPlayerCurrency = 500f;
    public float playerCurrency;
    public int HealCount { get; private set; }

    // Private variables
    private Dictionary<Item.ItemType, int> inventoryDict = new Dictionary<Item.ItemType, int>();
    private Dictionary<Item.ItemType, int> spawnedItems = new Dictionary<Item.ItemType, int>();
    private bool shopUIActive = false;
    private bool mainUIActive = true;
    private bool inventoryUIActive = false;
    private bool questUIActive = false;
    private bool questPopupActive = false;
    private float questPopupDesiredAlpha = 0f;
    private float questPopupCurrentAlpha = 0f;
    private float questPopupDisplayTime = 0f;

    void Start()
    {
        CurrentMouseMode = MouseMode.Default;
        shopUI.SetActive(shopUIActive);
        mainUI.SetActive(mainUIActive);
        inventoryUI.SetActive(inventoryUIActive);
        questUI.SetActive(questUIActive);
        questPopup.SetActive(questPopupActive);

        HealCount = 0;
    }

    void Update()
    {
        if (questPopupDisplayTime > 0f)
        {
            questPopupDisplayTime -= Time.deltaTime;
        }
        else
        {
            questPopupActive = false;
            questPopupDesiredAlpha = 0f;
        }
        SetQuestPopupAlpha();
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
        if (inventoryDict.TryGetValue(type, out int currentAmount))
        {
            return currentAmount;
        }
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

    public void IncrementHealCounter()
    {
        HealCount += 1;
    }

    // Return how many of x item has been placed
    public int GetSpawnedTotal(Item.ItemType type)
    {
        if (spawnedItems.TryGetValue(type, out int currentAmount))
        {
            return currentAmount;
        }
        return 0;
    }

    // Set current mouse mode
    public void SetCurrentMouseMode(MouseMode mouseMode)
    {
        // TODO: Logic checks
        PreviousMouseMode = CurrentMouseMode;
        CurrentMouseMode = mouseMode;
    }

    public void ButtonHoverEnter()
    {
        SetCurrentMouseMode(MouseMode.UI);
    }

    public void ButtonHoverExit()
    {
        SetCurrentMouseMode(PreviousMouseMode);
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
        CurrentMouseMode = MouseMode.UI;
        ToggleUI();
    }

    // Set inventory UI inactive
    public void HideInventoryUI()
    {
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

    // Set quest popup active for 5 seconds
    public void DisplayQuestPopup(string displayText, int progressAmount, int requiredAmount)
    {
        Transform questText = questPopup.transform.Find("QuestText");
        questText.GetComponent<TMP_Text>().text = displayText;

        GameObject completionPercentageText = questPopup.transform.Find("CompletionPercent").gameObject;
        GameObject completionImage = questPopup.transform.Find("CompleteImage").gameObject;
        float completionPercentage = progressAmount / requiredAmount;
        //Debug.Log("Percentage = " + completionPercentage);

        completionPercentageText.GetComponent<TMP_Text>().text = $"{progressAmount}/{requiredAmount}";

        if (completionPercentage >= 1.0)
        {
            completionPercentageText.SetActive(false);
            completionImage.SetActive(true);
        }
        else
        {
            completionPercentageText.SetActive(true);
            completionImage.SetActive(false);
        }
        questPopupActive = true;
        questPopup.SetActive(questPopupActive);
        questPopupDisplayTime = 5.0f;
        questPopupDesiredAlpha = 1.0f;
    }

    // Helper function to fade quest popup in/out
    public void SetQuestPopupAlpha()
    {
        if (questPopupCurrentAlpha != questPopupDesiredAlpha)
        {
            questPopupCurrentAlpha = Mathf.MoveTowards(questPopupCurrentAlpha, questPopupDesiredAlpha, 2.0f * Time.deltaTime);
            foreach (Transform child in questPopup.transform)
            {
                if (child.GetComponent<Image>() != null)
                {
                    Color currentColor = child.GetComponent<Image>().color;
                    child.GetComponent<Image>().color = new Color(currentColor.r, currentColor.g, currentColor.b, questPopupCurrentAlpha);
                }
                else if (child.GetComponent<TMP_Text>() != null)
                {
                    Color currentColor = child.GetComponent<TMP_Text>().color;
                    child.GetComponent<TMP_Text>().color = new Color(currentColor.r, currentColor.g, currentColor.b, questPopupCurrentAlpha);
                }
            }
        }
    }

    // Call InventoryMenu.GenerateButtons()
    public void UpdateInventoryDisplay()
    {
        inventoryUI.GetComponent<InventoryMenu>().GenerateButtons();
    }

    public void UpdateQuestDisplay()
    {
        questUI.GetComponent<QuestMenu>().GenerateQuests();
    }
}
