using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

public class MouseClick : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private string[] clickableLayers;
    private LayerMask clickableLayerMask;
    private LayerMask plantLayerMask;
    private LayerMask noPlantLayerMask;
    [SerializeField] private LayerMask diseaseLayerMask;
    [SerializeField] private GameObject healthBarPrefab;

    //[SerializeField] private GameManager gameManager;
    public GameObject CurrentItemToPlace { get; private set; }
    public Item.ItemType CurrentItemType { get; private set; }

    [Header("Cursors")]
    private MeshRenderer indicator;
    public Texture2D defaultCursor;
    public Texture2D pickupCursor;
    public Texture2D medicineCursor;

    [Header("Sound Effects")]
    public AudioClip medicinePickupSfx;
    [Range(0, 1)]
    public float medicinePickupSfxVolume = 1f;
    public AudioClip coinPickupSfx;
    [Range(0, 1)]
    public float coinPickupSfxVolume = 1f;
    public AudioClip plantSfx;
    [Range(0, 1)]
    public float plantSfxVolume = 1f;

    private Light indicatorLight;

    void Start()
    {
        if (!mainCamera) Debug.Log("No camera assigned in PlaceItemsDemo");
        indicator = GetComponent<MeshRenderer>();
        if (GameManager.Instance.level == 1)
        {
            indicatorLight = transform.Find("Light").GetComponent<Light>();
        }
        Cursor.SetCursor(defaultCursor, new Vector2(3, 3), CursorMode.Auto);
        clickableLayerMask = LayerMask.GetMask(clickableLayers);
        string[] layersExcludingPlant = clickableLayers.Where(layer => layer != "Plant").ToArray();
        noPlantLayerMask = LayerMask.GetMask(layersExcludingPlant);
        plantLayerMask = LayerMask.GetMask("Plant");
    }

    void Update()
    {
        // Show indicator if in place mode
        indicator.enabled = GameManager.Instance.CurrentMouseMode == MouseMode.Place;
        if (GameManager.Instance.level == 1) indicatorLight.enabled = GameManager.Instance.CurrentMouseMode == MouseMode.Place;

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

        HandleCursor();
        MoveCursor(mousePosition);
        // Handle input
        if (Input.GetMouseButtonDown(0))
        {
            switch (GameManager.Instance.CurrentMouseMode)
            {
                case MouseMode.Place:
                    HandlePlace(mousePosition);
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

    private void HandleCursor()
    {
        if (GameManager.Instance.CurrentMouseMode == MouseMode.UI)
        {
            Cursor.SetCursor(defaultCursor, new Vector2(3, 3), CursorMode.Auto);
        }
        else
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, clickableLayerMask))
            {
                GameObject clickableItem = raycastHit.transform.gameObject;
                if (GameManager.Instance.CurrentMouseMode == MouseMode.Heal)
                {
                    if (clickableItem.CompareTag("plant"))
                    {
                        Cursor.SetCursor(medicineCursor, new Vector2(5, 5), CursorMode.Auto);
                    }
                    else Cursor.SetCursor(defaultCursor, new Vector2(3, 3), CursorMode.Auto);
                }
                else if (GameManager.Instance.CurrentMouseMode == MouseMode.Default)
                {
                    if (clickableItem.CompareTag("Money") || clickableItem.CompareTag("Medicine"))
                    {
                        Cursor.SetCursor(pickupCursor, new Vector2(5, 5), CursorMode.Auto);
                    }
                    else Cursor.SetCursor(defaultCursor, new Vector2(3, 3), CursorMode.Auto);
                }
            }
            else Cursor.SetCursor(defaultCursor, new Vector2(3, 3), CursorMode.Auto);
        }
    }

    // Handle item placement
    private void HandlePlace(Vector3 target)
    {
        if (CanPlaceCurrentItem() && target.x != Mathf.Infinity)
        {
            bool resetMousePlacementMode = false;
            if (GameManager.Instance.GetInventoryItemAmount(CurrentItemType) == 1)
            {
                resetMousePlacementMode = true;
            }

            // Update inventory
            GameManager.Instance.RemoveFromInventory(CurrentItemType, 1);
            GameManager.Instance.UpdateInventoryDisplay();

            // Spawn item
            GameObject placedItem = Instantiate(CurrentItemToPlace, target, Quaternion.Euler(RandomObjectRotation()), GameManager.Instance.placedItemParent);
            Instantiate(healthBarPrefab, target, Quaternion.identity, placedItem.transform);

            // Increment spawn counter
            GameManager.Instance.IncrementSpawnCounter(CurrentItemType);
            GameManager.Instance.IncrementPlantCounter();

            AudioManager.Instance.Play(plantSfx, plantSfxVolume);

            // If there are no more of current item, reset mouse mode to default
            if (resetMousePlacementMode)
            {
                GameManager.Instance.SetCurrentMouseMode(MouseMode.Default);
            }
        }
    }

    private void HandleMedicine(Vector3 target)
    {
        if (CanPlaceCurrentItem() && target.x != Mathf.Infinity)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, plantLayerMask))
            {
                GameObject clickableItem = raycastHit.transform.gameObject;
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

                        // Update heal count
                        GameManager.Instance.IncrementHealCounter();

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
            if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, noPlantLayerMask))
            {
                GameObject clickableItem = raycastHit.transform.gameObject;
                //Debug.Log($"{clickableItem.name} - {clickableItem.tag}");

                // Check if item is money
                if (clickableItem.CompareTag("Money"))
                {
                    GameManager.Instance.ChangePlayerCurrency(20f);
                    GameManager.Instance.IncrementCoinCollectCounter();
                    AudioManager.Instance.Play(coinPickupSfx, coinPickupSfxVolume);
                    Destroy(clickableItem);
                }
                //Check if item is Enemy
                if (clickableItem.CompareTag("Enemy"))
                {
                    clickableItem.GetComponent<EnemyMovement>().HandleEnemyClicked();
                    clickableItem.GetComponent<EnemyHealth>().TakeDamage(20);
                }
                // Check if item is medicine
                else if (clickableItem.CompareTag("Medicine"))
                {
                    GameManager.Instance.AddToInventory(Item.ItemType.Medicine, 1);
                    GameManager.Instance.UpdateInventoryDisplay();
                    AudioManager.Instance.Play(medicinePickupSfx, medicinePickupSfxVolume);
                    Destroy(clickableItem);
                }
            }
        }
    }

    // Check if user has enough of current item in inventory to be placed
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
        return new Vector3(0, UnityEngine.Random.Range(135f, 225f), 0);
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
