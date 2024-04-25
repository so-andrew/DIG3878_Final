using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class WinScreen : MonoBehaviour
{
    private Transform plants;
    private Transform heals;
    private Transform coins;
    private Transform enemies;

    private TMP_Text plantCount;
    private TMP_Text healCount;
    private TMP_Text coinCount;
    private TMP_Text enemyCount;

    private bool claimedReward = false;
    private bool ShowBlockchainPopup;
    private GameObject blockchainPopup;
    private GameObject claimRewardButton;
    private TbnbManager tbnbManager;
    public string transactionHash;
    public string responseHash;

    // Start is called before the first frame update
    void Start()
    {
        Transform stats = transform.Find("Stats");
        plants = stats.Find("Plants");
        heals = stats.Find("Heals");
        coins = stats.Find("Coins");
        enemies = stats.Find("Enemies");

        plantCount = plants.Find("PlantsPlacedCount").GetComponent<TMP_Text>();
        healCount = heals.Find("TimesHealedCount").GetComponent<TMP_Text>();
        coinCount = coins.Find("CoinsCollectedCount").GetComponent<TMP_Text>();
        enemyCount = enemies.Find("EnemiesSquishedCount").GetComponent<TMP_Text>();

        claimRewardButton = transform.Find("ClaimRewardButton").gameObject;
        RectTransform winScreenRect = GetComponent<RectTransform>();

        if (PlayerPrefs.GetString("Account") == "")
        {
            claimRewardButton.SetActive(false);
            winScreenRect.sizeDelta = new Vector2(winScreenRect.sizeDelta.x, winScreenRect.sizeDelta.y - 100);
        }
        else
        {
            claimRewardButton.SetActive(true);
        }

        tbnbManager = GameManager.Instance.GetComponent<TbnbManager>();

        ShowBlockchainPopup = false;
        blockchainPopup = transform.Find("Popup").gameObject;
        blockchainPopup.SetActive(ShowBlockchainPopup);

        // Button mainMenuButton = transform.Find("MainMenuButton").GetComponent<Button>();
    }

    public void OnPopupClose()
    {
        ShowBlockchainPopup = false;
        blockchainPopup.SetActive(ShowBlockchainPopup);
    }

    public void OnPopupOpen()
    {
        tbnbManager.RewardCurrency();
        claimedReward = true;
        claimRewardButton.SetActive(false);
    }

    public void ShowPopup()
    {
        ShowBlockchainPopup = true;
        blockchainPopup.SetActive(ShowBlockchainPopup);
        blockchainPopup.transform.Find("Hash").GetComponent<TMP_Text>().text = "Transaction Hash: " + transactionHash;
    }

    // Update is called once per frame
    void Update()
    {
        plantCount.text = GameManager.Instance.PlantCount.ToString();
        healCount.text = GameManager.Instance.HealCount.ToString();
        coinCount.text = GameManager.Instance.CoinCollectedCount.ToString();
        enemyCount.text = GameManager.Instance.EnemiesSquishedCount.ToString();
    }
}
