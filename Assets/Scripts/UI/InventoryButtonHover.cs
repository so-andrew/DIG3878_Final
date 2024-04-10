using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryButtonHover : MonoBehaviour
{
    public void InventoryButtonHoverEnter(){
        Transform tooltip = transform.Find("Tooltip");
        tooltip.gameObject.SetActive(true);
        GameManager.Instance.ButtonHoverEnter();
    }

    public void InventoryButtonHoverExit(){
        Transform tooltip = transform.Find("Tooltip");
        tooltip.gameObject.SetActive(false);
        if (!(GameManager.Instance.CurrentMouseMode == MouseMode.Place && GameManager.Instance.PreviousMouseMode != MouseMode.Place)){
           GameManager.Instance.SetCurrentMouseMode(GameManager.Instance.PreviousMouseMode);
        }
    }
}
