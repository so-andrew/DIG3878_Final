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

        // Use raycast to get mouse position
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

        // Handle input
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

    // Handle item placement
    private void HandleClickDemo(Vector3 target)
    {
        if (CanPlaceCurrentItem() && target.x != Mathf.Infinity)
        {
            GameManager.Instance.RemoveFromInventory(CurrentItemType, 1); // Remove 1 of current item from inventory
            GameManager.Instance.UpdateInventoryDisplay(); // Update inventory display

            // Spawn item
            GameObject placedItem = Instantiate(CurrentItemToPlace, target, Quaternion.Euler(RandomObjectRotation()), GameManager.Instance.placedItemParent);
            Instantiate(healthBarPrefab, target, Quaternion.identity, placedItem.transform);
            GameManager.Instance.IncrementSpawnCounter(CurrentItemType); // Increment spawn counter
        }
    }

    // Handle interaction with entities
    private void HandlePickup(Vector3 target)
    {
        if (target.x != Mathf.Infinity)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, clickableLayerMask))
            {
                GameObject clickableItem = raycastHit.transform.gameObject;
                if (clickableItem.CompareTag("Money"))
                {
                    GameManager.Instance.playerCurrency += 50f;
                    Destroy(clickableItem);
                }
            }
        }
    }

    // Helper function to determine whether selected item can be placed
    private bool CanPlaceCurrentItem()
    {
        return GameManager.Instance.GetInventoryItemAmount(CurrentItemType) > 0;
    }

    // Set item to be placed
    public void SetCurrentItem(Item.ItemType type)
    {
        CurrentItemType = type;
        CurrentItemToPlace = Item.GetGameObject(type);
    }

    // Return random rotation vector
    private Vector3 RandomObjectRotation()
    {
        return new Vector3(0, UnityEngine.Random.Range(0f, 360f), 0);
    }

    // Used to determine if mouse is hovering over a clickable object
    private Vector3 DefaultRaycastCheck()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, clickableLayerMask))
        {
            return raycastHit.point;
        }
        else
        {
            return Vector3.positiveInfinity;
        }
    }

    // Used to determine if mouse is hovering over ground (for placing objects)
    private Vector3 PlaceItemRaycastCheck()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, groundLayerMask))
        {
            return raycastHit.point;
        }
        else
        {
            return Vector3.positiveInfinity;
        }
    }
}
