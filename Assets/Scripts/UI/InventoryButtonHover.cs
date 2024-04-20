using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryButtonHover : MonoBehaviour
{
    public void InventoryButtonHoverEnter()
    {
        Transform tooltip = transform.Find("Tooltip");
        tooltip.gameObject.SetActive(true);
        //GameManager.Instance.ButtonHoverEnter();
        GameManager.Instance.SetCurrentMouseMode(MouseMode.UI);
    }

    public void InventoryButtonHoverExit()
    {
        Transform tooltip = transform.Find("Tooltip");
        tooltip.gameObject.SetActive(false);
        if (ShouldSetToPreviousMouseMode())
        {
            //Debug.Log($"Setting current mouse mode to {GameManager.Instance.PreviousMouseMode}");
            GameManager.Instance.SetCurrentMouseMode(GameManager.Instance.PreviousMouseMode);
        }
    }

    private bool ShouldSetToPreviousMouseMode()
    {
        // bool returnValue = false;
        // if (GameManager.Instance.CurrentMouseMode == MouseMode.UI && GameManager.Instance.PreviousMouseMode == MouseMode.Heal)
        // {
        //     returnValue = true;
        // }
        // if (GameManager.Instance.CurrentMouseMode == MouseMode.UI && GameManager.Instance.PreviousMouseMode == MouseMode.Place)
        // {
        //     returnValue = true;
        // }
        // return returnValue;

        return (GameManager.Instance.CurrentMouseMode == MouseMode.UI && GameManager.Instance.PreviousMouseMode == MouseMode.Heal) || (GameManager.Instance.CurrentMouseMode == MouseMode.UI && GameManager.Instance.PreviousMouseMode == MouseMode.Place);
        //return true;
    }
}
