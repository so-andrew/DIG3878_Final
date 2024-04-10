using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MouseMode
{
    Default,
    Place,
    Remove,
    Upgrade,
    Shop
}

public class PlaceItemsDemo : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private GameObject healthBarPrefab;
    //[SerializeField] private GameManager gameManager;
    public GameObject CurrentItemToPlace { get; private set; }
    public Item.ItemType CurrentItemType { get; private set; }

    private MeshRenderer indicator;

    void Start()
    {
        if (!mainCamera) Debug.Log("No camera assigned in PlaceItemsDemo");
        indicator = GetComponent<MeshRenderer>();
    }

    void Update()
    {
        // Show indicator if in place mode
        indicator.enabled = GameManager.Instance.CurrentMouseMode == MouseMode.Place;

        Vector3 mousePosition = RaycastCheck();
        MoveCursor(mousePosition);
        if (Input.GetMouseButtonDown(0))
        {
            switch (GameManager.Instance.CurrentMouseMode)
            {
                case MouseMode.Place:
                    HandleClickDemo(mousePosition);
                    break;
                default:
                    break;
            }
        }
    }

    private void MoveCursor(Vector3 target)
    {
        if (target.x != Mathf.Infinity)
        {
            transform.position = target;
        }
    }

    private void HandleClickDemo(Vector3 target)
    {
        if (CanPlaceCurrentItem() && target.x != Mathf.Infinity)
        {
            GameManager.Instance.RemoveFromInventory(CurrentItemType, 1);
            GameManager.Instance.UpdateInventoryDisplay();
            GameObject placedItem = Instantiate(CurrentItemToPlace, target, Quaternion.Euler(RandomObjectRotation()), GameManager.Instance.placedItemParent);
            Instantiate(healthBarPrefab, target, Quaternion.identity, placedItem.transform);
        }
    }

    private bool CanPlaceCurrentItem(){
        return GameManager.Instance.GetInventoryItemAmount(CurrentItemType) > 0;
    }

    public void SetCurrentItem(Item.ItemType type){
        CurrentItemType = type;
        CurrentItemToPlace = Item.GetGameObject(type);
    }

    private Vector3 RandomObjectRotation()
    {
        return new Vector3(0, UnityEngine.Random.Range(0f, 360f), 0);
    }

    private Vector3 RaycastCheck()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, layerMask))
        {
            //Debug.DrawRay(mainCamera.transform.position, raycastHit.point, Color.red);
            //Debug.Log(raycastHit.point);
            return raycastHit.point;
        }
        else
        {
            return Vector3.positiveInfinity;
        }
    }
}
