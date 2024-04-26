using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryButtonHover : MonoBehaviour
{
    public void InventoryButtonHoverEnter()
    {
        Transform tooltip = transform.Find("Tooltip");
        tooltip.gameObject.SetActive(true);
        GameManager.Instance.SetCurrentMouseMode(MouseMode.UI);
    }

    public void InventoryButtonHoverExit()
    {
        Transform tooltip = transform.Find("Tooltip");
        tooltip.gameObject.SetActive(false);
        if (ShouldSetToPreviousMouseMode())
        {
            GameManager.Instance.SetCurrentMouseMode(GameManager.Instance.PreviousMouseMode);
        }
    }

    private bool ShouldSetToPreviousMouseMode()
    {
        return GameManager.Instance.CurrentMouseMode == MouseMode.UI && GameManager.Instance.CurrentMouseMode != GameManager.Instance.PreviousMouseMode;
    }
}
