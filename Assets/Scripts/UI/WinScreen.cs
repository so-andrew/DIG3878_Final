using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

        Button mainMenuButton = transform.Find("Button").GetComponent<Button>();
        //mainMenuButton.onClick.AddListener(OnWinScreenButtonClick);
    }

    public void OnWinScreenButtonClick()
    {
        SceneManager.LoadScene("MainMenu");
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
