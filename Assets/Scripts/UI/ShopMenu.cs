using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public class ShopMenu : MonoBehaviour
{
    [SerializeField] private Transform container;
    private Transform itemTemplate;

    void Awake()
    {
        //container = transform.Find("Container");
        itemTemplate = container.Find("ShopItemTemplate");
        itemTemplate.gameObject.SetActive(false);
    }

    void Start()
    {

        GenerateShop();
    }

    public void GenerateShop()
    {
        if (container == null) return;
        GameObject[] containerChildren = new GameObject[container.childCount];
        int i = 0;
        foreach (Transform child in container)
        {
            if (child.gameObject.name != "ShopItemTemplate")
            {
                containerChildren[i] = child.gameObject;
                i++;
            }
        }

        foreach (GameObject obj in containerChildren)
        {
            Destroy(obj);
        }

        Item.ItemType[] itemTypes = { Item.ItemType.Plant1, Item.ItemType.Plant2, Item.ItemType.Plant3 };
        foreach (Item.ItemType itemType in itemTypes)
        {
            CreateItemButton(itemType);
        }
    }

    private void CreateItemButton(Item.ItemType type)
    {
        string name = Item.GetItemName(type);
        int cost = Item.GetCost(type);

        Transform itemTransform = Instantiate(itemTemplate, container);
        Transform itemName = itemTransform.Find("ItemName");
        itemName.GetComponent<TMP_Text>().text = name;

        Transform itemPrice = itemTransform.Find("ItemPrice");
        itemPrice.GetComponent<TMP_Text>().text = cost.ToString();
        if (cost > GameManager.Instance.playerCurrency)
        {
            itemPrice.GetComponent<TMP_Text>().color = Color.red;
        }
        else
        {
            itemPrice.GetComponent<TMP_Text>().color = Color.white;
        }

        Transform itemImg = itemTransform.Find("ItemImg");
        itemImg.GetComponent<Image>().sprite = Item.GetSprite(type);

        itemTransform.GetComponent<Button>().onClick.AddListener(() => TryBuyItem(type));
        itemTransform.gameObject.SetActive(true);
    }

    private void TryBuyItem(Item.ItemType itemType)
    {
        int itemCost = Item.GetCost(itemType);
        // Check if user has enough funds to purchase
        if (itemCost > GameManager.Instance.playerCurrency)
        {
            // TODO: Provide visual feedback when user cannot purchase item
            return;
        }

        // Remove currency from user
        GameManager.Instance.ChangePlayerCurrency(-1.0f * itemCost);

        // Add to inventory
        // TODO: Allow user to purchase quantity > 1
        GameManager.Instance.AddToInventory(itemType, 1);
        Debug.Log($"Added {itemType} to inventory; current count: {GameManager.Instance.GetInventoryItemAmount(itemType)}");
        UpdateShopDisplay();
        GameManager.Instance.UpdateInventoryDisplay();
    }

    public void UpdateShopDisplay()
    {
        foreach (Transform child in container)
        {
            Transform itemPrice = child.Find("ItemPrice");
            bool validInt = int.TryParse(itemPrice.GetComponent<TMP_Text>().text, out int cost);
            if (validInt)
            {
                if (cost > GameManager.Instance.playerCurrency)
                {
                    itemPrice.GetComponent<TMP_Text>().color = Color.red;
                }
                else
                {
                    itemPrice.GetComponent<TMP_Text>().color = Color.white;
                }
            }
            else
            {
                Debug.LogError("Cost string is not int");
            }
        }
    }
}
