using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryButtonClick : MonoBehaviour
{
    public void SetActiveItem(Item.ItemType itemType)
    {
        GameManager.Instance.MouseClickHandler.SetCurrentItem(itemType);
        GameManager.Instance.SetSelectedButton(transform.gameObject);

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
