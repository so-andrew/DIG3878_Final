using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public class ShopMenu : MonoBehaviour
{
    private Transform container;
    private Transform itemTemplate;

    void Awake()
    {
        container = transform.Find("Container");
        itemTemplate = container.Find("ShopItemTemplate");
        itemTemplate.gameObject.SetActive(false);
    }

    void Start()
    {
        int positionIndex;
        Item.ItemType[] itemTypes = { Item.ItemType.Plant1, Item.ItemType.Plant2, Item.ItemType.Plant3 };
        for (positionIndex = 0; positionIndex < 3; positionIndex++)
        {
            CreateItemButton(Item.GetItemName(itemTypes[positionIndex]), itemTypes[positionIndex], Item.GetCost(itemTypes[positionIndex]), positionIndex);
        }
        //CreateItemButton($"Tool {positionIndex}", Item.ItemType.Plant, Item.GetCost(Item.ItemType.Tool), positionIndex);

    }

    private void CreateItemButton(string name, Item.ItemType type, int cost, int positionIndex)
    {
        Transform itemTransform = Instantiate(itemTemplate, container);
        RectTransform itemRectTransform = itemTransform.GetComponent<RectTransform>();

        float itemHeight = 225f;
        float verticalOffset = 350f;
        itemRectTransform.anchoredPosition = new Vector2(0, (-itemHeight * positionIndex) + verticalOffset);
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
        GameManager.Instance.playerCurrency -= itemCost;

        // Add to inventory
        // TODO: Allow user to purchase quantity > 1
        GameManager.Instance.AddToInventory(itemType, 1);
        Debug.Log($"Added {itemType} to inventory; current count: {GameManager.Instance.GetInventoryItemAmount(itemType)}");
        UpdateShopDisplay();
        GameManager.Instance.UpdateInventoryDisplay();

        // GameManager.Instance.ItemPlacer.currentItemToPlace = Item.GetGameObject(itemType);
        // GameManager.Instance.SetCurrentMouseMode(MouseMode.Place);
        // GameManager.Instance.HideShopUI();
    }

    private void UpdateShopDisplay()
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
