using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryMenu : MonoBehaviour
{
    private Transform container;
    private Transform itemTemplate;

    void Awake()
    {
        container = transform.Find("Container");
        itemTemplate = container.Find("InventoryItemTemplate");
        itemTemplate.gameObject.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        //int inventoryTotalItemTypes = GameManager.Instance.GetInventoryTotalItemTypes();
        GenerateButtons();
    }

    public void GenerateButtons()
    {
        //Debug.Log("Running GenerateButtons");
        int i = 0;
        if (container == null) return; // Container gameObject is not active; menu will update when shown again
        GameObject[] containerChildren = new GameObject[container.childCount];
        foreach (Transform child in container)
        {
            if (child.gameObject.name != "InventoryItemTemplate" && child.gameObject.name != "Background")
            {
                containerChildren[i] = child.gameObject;
                i++;
            }
        }

        foreach (GameObject obj in containerChildren)
        {
            Destroy(obj);
        }

        Dictionary<Item.ItemType, int>.KeyCollection keys = GameManager.Instance.GetInventoryKeys();
        int positionIndex = 0;
        foreach (Item.ItemType type in keys)
        {
            CreateInventoryButton(type, GameManager.Instance.GetInventoryItemAmount(type), positionIndex);
            positionIndex++;
        }
    }

    private void CreateInventoryButton(Item.ItemType type, int itemCount, int positionIndex)
    {
        Transform itemTransform = Instantiate(itemTemplate, container);
        RectTransform itemRectTransform = itemTransform.GetComponent<RectTransform>();

        float itemWidth = 175f;
        float horizontalOffset = -350f;
        itemRectTransform.anchoredPosition = new Vector2((itemWidth * positionIndex) + horizontalOffset, 0);

        Transform itemQuantity = itemTransform.Find("ItemQuantity");
        itemQuantity.GetComponent<TMP_Text>().text = itemCount.ToString();

        Transform tooltip = itemTransform.Find("Tooltip");
        tooltip.GetChild(1).GetComponent<TMP_Text>().text = Item.GetItemName(type);
        tooltip.gameObject.SetActive(false);

        itemTransform.GetComponent<Button>().onClick.AddListener(() => SetActiveItem(type));
        itemTransform.gameObject.SetActive(true);
    }

    private void SetActiveItem(Item.ItemType itemType)
    {
        //Debug.Log("Setting item " + itemType.ToString());
        GameManager.Instance.MouseClickHandler.SetCurrentItem(itemType);
        if (itemType == Item.ItemType.Medicine)
        {
            GameManager.Instance.SetPreviousMouseMode(MouseMode.Heal);
        }
        else
        {
            GameManager.Instance.SetPreviousMouseMode(MouseMode.Place);
        }
    }
}
