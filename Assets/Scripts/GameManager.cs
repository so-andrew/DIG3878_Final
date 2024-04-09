using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        playerCurrency = startingPlayerCurrency;
    }

    public PlaceItemsDemo ItemPlacer;
    public Transform placedItemParent;
    public MouseMode CurrentMouseMode { get; private set; }
    private MouseMode previousMouseMode;
    public GameObject mainUI;
    public GameObject shopUI;
    public GameObject inventoryUI;
    [SerializeField] private float startingPlayerCurrency = 500f;
    public float playerCurrency;

    private Dictionary<Item.ItemType, int> inventoryDict = new Dictionary<Item.ItemType, int>();

    private bool shopUIActive = false;
    private bool mainUIActive = true;
    private bool inventoryUIActive = false;

    void Start()
    {
        CurrentMouseMode = MouseMode.Default;
        shopUI.SetActive(shopUIActive);
        mainUI.SetActive(mainUIActive);
        inventoryUI.SetActive(inventoryUIActive);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddToInventory(Item.ItemType type, int amount){
        if(inventoryDict.TryGetValue(type, out int currentAmount)){
            inventoryDict[type] += amount;
        }
        else {
            inventoryDict.Add(type, amount);
        }
    }

    public void RemoveFromInventory(Item.ItemType type, int amount){
        if(inventoryDict.TryGetValue(type, out int currentAmount)){
            inventoryDict[type] -= amount;
            if(inventoryDict[type] <= 0){
                inventoryDict.Remove(type);
            }
        }
    }

    public int GetInventoryAmount(Item.ItemType type){
        if(inventoryDict.TryGetValue(type, out int currentAmount)){
            return inventoryDict[type];
        }
        else return 0;
    }

    public void SetCurrentMouseMode(MouseMode mouseMode)
    {
        // TODO: Logic checks
        CurrentMouseMode = mouseMode;
    }

    public void ShowShopUI()
    {
        shopUIActive = true;
        mainUIActive = !shopUIActive;
        inventoryUIActive = !shopUIActive;
        CurrentMouseMode = MouseMode.Shop;
        ToggleUI();
    }

    public void ButtonHoverEnter()
    {
        previousMouseMode = CurrentMouseMode;
        CurrentMouseMode = MouseMode.Default;
    }

    public void ButtonHoverExit()
    {
        CurrentMouseMode = previousMouseMode;
    }

    public void HideShopUI()
    {
        shopUIActive = false;
        mainUIActive = !shopUIActive;
        inventoryUIActive = !shopUIActive;
        CurrentMouseMode = MouseMode.Default;
        ToggleUI();
    }

    public void ShowInventoryUI()
    {
        inventoryUIActive = true;
        mainUIActive = !inventoryUIActive;
        shopUIActive = !shopUIActive;
        CurrentMouseMode = MouseMode.Default;
        ToggleUI();
    }

    public void HideInventoryUI(){
        inventoryUIActive = false;
        mainUIActive = !inventoryUIActive;
        shopUIActive = !shopUIActive;
        CurrentMouseMode = MouseMode.Default;
        ToggleUI();
    }

    private void ToggleUI(){
        mainUI.SetActive(mainUIActive);
        shopUI.SetActive(shopUIActive);
        inventoryUI.SetActive(inventoryUIActive);
    }
}
