using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public MouseMode PreviousMouseMode { get; private set; }
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
            inventoryDict[type] = currentAmount + amount;
        }
        else {
            inventoryDict.Add(type, amount);
        }
    }

    public void RemoveFromInventory(Item.ItemType type, int amount){
        if(inventoryDict.TryGetValue(type, out int currentAmount)){
            inventoryDict[type] = currentAmount - amount;
            if(inventoryDict[type] <= 0){
                inventoryDict.Remove(type);
            }
        }
    }

    public int GetInventoryItemAmount(Item.ItemType type){
        if(inventoryDict.TryGetValue(type, out int currentAmount)){
            return currentAmount;
        }
        return 0;
    }

    public int GetInventoryTotalItemTypes(){
        return inventoryDict.Count;
    }

    public Dictionary<Item.ItemType, int>.KeyCollection GetInventoryKeys(){
        return inventoryDict.Keys;
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
        CurrentMouseMode = MouseMode.Shop;
        ToggleUI();
    }

    public void ButtonHoverEnter()
    {
        PreviousMouseMode = CurrentMouseMode;
        CurrentMouseMode = MouseMode.Default;
    }

    public void ButtonHoverExit()
    {
        CurrentMouseMode = PreviousMouseMode;
    }

    public void HideShopUI()
    {
        shopUIActive = false;
        mainUIActive = !shopUIActive;
        CurrentMouseMode = MouseMode.Default;
        ToggleUI();
    }

    public void ShowInventoryUI()
    {
        inventoryUIActive = true;
        mainUIActive = !inventoryUIActive;
        CurrentMouseMode = MouseMode.Default;
        ToggleUI();
    }

    public void HideInventoryUI(){
        inventoryUIActive = false;
        mainUIActive = !inventoryUIActive;
        CurrentMouseMode = MouseMode.Default;
        ToggleUI();
    }

    private void ToggleUI(){
        mainUI.SetActive(mainUIActive);
        shopUI.SetActive(shopUIActive);
        inventoryUI.SetActive(inventoryUIActive);
    }

    public void UpdateInventoryDisplay(){
        inventoryUI.GetComponent<InventoryMenu>().GenerateButtons();
    }
}
