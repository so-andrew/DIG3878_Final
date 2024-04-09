using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public PlaceItemsDemo ItemPlacer;
    public Transform placedItemParent;
    public MouseMode CurrentMouseMode { get; private set; }
    private MouseMode previousMouseMode;
    public GameObject mainUI;
    public GameObject shopUI;

    private bool shopUIActive = false;
    private bool mainUIActive = true;
    // Start is called before the first frame update
    void Start()
    {
        CurrentMouseMode = MouseMode.Default;
        shopUI.SetActive(shopUIActive);
        mainUI.SetActive(mainUIActive);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetCurrentMouseMode(MouseMode mouseMode)
    {
        // TODO: Logic checks
        CurrentMouseMode = mouseMode;
    }

    public void ShowShopUI()
    {
        shopUIActive = true;
        mainUIActive = !shopUIActive;
        CurrentMouseMode = MouseMode.Shop;
        shopUI.SetActive(shopUIActive);
        mainUI.SetActive(mainUIActive);
    }

    public void ButtonHoverEnter()
    {
        previousMouseMode = CurrentMouseMode;
        CurrentMouseMode = MouseMode.Default;
    }

    public void ButtonHoverExit()
    {
        CurrentMouseMode = previousMouseMode;
    }

    public void HideShopUI()
    {
        shopUIActive = false;
        mainUIActive = !shopUIActive;
        if (CurrentMouseMode != MouseMode.Place)
        {
            CurrentMouseMode = MouseMode.Default;
        }
        shopUI.SetActive(shopUIActive);
        mainUI.SetActive(mainUIActive);
    }
}
