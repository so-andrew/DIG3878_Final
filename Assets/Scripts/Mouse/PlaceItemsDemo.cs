using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum MouseMode
{
    Default,
    Place,
    Remove,
    Upgrade,
    UI
}

public class PlaceItemsDemo : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private LayerMask clickableLayerMask;
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

        Vector3 mousePosition;
        if (GameManager.Instance.CurrentMouseMode == MouseMode.Place)
        {
            mousePosition = PlaceItemRaycastCheck();
        }
        else
        {
            mousePosition = DefaultRaycastCheck();
        }

        MoveCursor(mousePosition);
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 100, Color.red);
        //Debug.Log(mousePosition);
        if (Input.GetMouseButtonDown(0))
        {
            switch (GameManager.Instance.CurrentMouseMode)
            {
                case MouseMode.Place:
                    HandleClickDemo(mousePosition);
                    break;
                case MouseMode.Default:
                    HandlePickup(mousePosition);
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

    private void HandlePickup(Vector3 target)
    {
        Debug.Log("Handle pickup");
        if (target.x != Mathf.Infinity)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            //Debug.DrawRay(ray.origin, ray.direction * 100, Color.red);
            //Debug.Log(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, clickableLayerMask))
            {
                GameObject clickableItem = raycastHit.transform.gameObject;
                if (raycastHit.transform == null)
                {
                    Debug.Log("No transform here");
                }
                Debug.Log(raycastHit);
                Debug.Log(raycastHit.transform);
                Debug.Log(clickableItem);
                Debug.Log(clickableItem.name);
                if (clickableItem.CompareTag("Money"))
                {
                    GameManager.Instance.playerCurrency += 50f;
                    Destroy(clickableItem);
                }
                if (clickableItem.CompareTag("Enemy"))
                {
                    Destroy(clickableItem);
                    //clickableItem.GetComponent<RandomMovement>().HandleEnemyClicked();
                }
            }
            else Debug.Log("Nothing?");
        }
        else Debug.Log("Infinity");
    }

    private bool CanPlaceCurrentItem()
    {
        return GameManager.Instance.GetInventoryItemAmount(CurrentItemType) > 0;
    }

    public void SetCurrentItem(Item.ItemType type)
    {
        CurrentItemType = type;
        CurrentItemToPlace = Item.GetGameObject(type);
    }

    private Vector3 RandomObjectRotation()
    {
        return new Vector3(0, UnityEngine.Random.Range(0f, 360f), 0);
    }

    private Vector3 DefaultRaycastCheck()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue))
        {
            return raycastHit.point;
        }
        else
        {
            return PlaceItemRaycastCheck();
        }
    }

    private Vector3 PlaceItemRaycastCheck()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, groundLayerMask))
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
