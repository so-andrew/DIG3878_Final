using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum MouseMode
{
    Default,
    Place,
    Heal,
    Remove,
    Upgrade,
    UI
}

public class PlaceItemsDemo : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private LayerMask clickableLayerMask;
    [SerializeField] private LayerMask diseaseLayerMask;
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
                case MouseMode.Heal:
                    HandleMedicine(mousePosition);
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
        Debug.Log("Handle click");
        if (CanPlaceCurrentItem() && target.x != Mathf.Infinity)
        {
            if (CurrentItemType == Item.ItemType.Medicine)
            {
                //HandleMedicine(target);
            }
            else
            {
                GameManager.Instance.RemoveFromInventory(CurrentItemType, 1);
                GameManager.Instance.UpdateInventoryDisplay();
                GameObject placedItem = Instantiate(CurrentItemToPlace, target, Quaternion.Euler(RandomObjectRotation()), GameManager.Instance.placedItemParent);
                Instantiate(healthBarPrefab, target, Quaternion.identity, placedItem.transform);
            }
        }
    }

    private void HandleMedicine(Vector3 target)
    {
        Debug.Log("Handle medicine");
        if (CanPlaceCurrentItem() && target.x != Mathf.Infinity)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, clickableLayerMask))
            {
                GameObject clickableItem = raycastHit.transform.gameObject;
                Debug.Log(clickableItem.name);
                if (clickableItem.CompareTag("plant"))
                {
                    Health plantHealth = clickableItem.GetComponentInChildren<Health>();
                    if (plantHealth == null)
                    {
                        Debug.Log("ERROR: No health script in children?");
                    }
                    else
                    {
                        plantHealth.healDamage(100f); // Heal plant

                        // Update inventory
                        GameManager.Instance.RemoveFromInventory(CurrentItemType, 1);
                        GameManager.Instance.UpdateInventoryDisplay();

                        // Set mouse mode to default
                        GameManager.Instance.SetCurrentMouseMode(MouseMode.Default);

                        // Destroy plague originating at plant location
                        Collider[] hitColliders = Physics.OverlapSphere(clickableItem.transform.position, 1f);
                        foreach (var collider in hitColliders)
                        {
                            if (collider.gameObject.layer == LayerMask.NameToLayer("Disease"))
                            {
                                Destroy(collider.gameObject);
                            }
                        }
                    }

                }
            }
        }
    }

    private void HandlePickup(Vector3 target)
    {
        if (target.x != Mathf.Infinity)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, clickableLayerMask))
            {
                GameObject clickableItem = raycastHit.transform.gameObject;
                Debug.Log(clickableItem.name);
                if (clickableItem.CompareTag("Money"))
                {
                    GameManager.Instance.playerCurrency += 50f;
                    Destroy(clickableItem);
                }

                if (clickableItem.CompareTag("Medicine"))
                {
                    GameManager.Instance.AddToInventory(Item.ItemType.Medicine, 1);
                    GameManager.Instance.UpdateInventoryDisplay();
                    Destroy(clickableItem);
                }

            }
        }
    }

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
