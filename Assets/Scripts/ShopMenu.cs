using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

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
            CreateItemButton($"Plant {positionIndex}", itemTypes[positionIndex], Item.GetCost(itemTypes[positionIndex]), positionIndex);
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
        itemTransform.Find("ItemName").GetComponent<TMP_Text>().text = name;
        itemTransform.Find("ItemPrice").GetComponent<TMP_Text>().text = cost.ToString();

        itemTransform.GetComponent<Button>().onClick.AddListener(() => TryBuyItem(type));

        itemTransform.gameObject.SetActive(true);
    }

    private void TryBuyItem(Item.ItemType itemType)
    {
        // TODO: Check if user has enough funds to purchase
        // TODO: Add to inventory upon purchase

        GameManager.Instance.ItemPlacer.currentItemToPlace = Item.GetGameObject(itemType);
        GameManager.Instance.SetCurrentMouseMode(MouseMode.Place);
        GameManager.Instance.HideShopUI();
    }
}
