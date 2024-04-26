using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public GameObject mainPanel;
    public GameObject creditsPanel;
    private string address;
    private GameObject addressObject;
    private TMP_Text addressText;
    // Start is called before the first frame update
    void Start()
    {
        address = PlayerPrefs.GetString("Account");
        addressObject = mainPanel.transform.Find("Address").gameObject;
        addressText = addressObject.GetComponent<TMP_Text>();
        if (address == null || address.Length == 0)
        {
            addressObject.SetActive(false);
        }
        mainPanel.SetActive(true);
        creditsPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (address == null)
        {
            address = PlayerPrefs.GetString("Account");
            addressObject.SetActive(true);
        }
        if (addressObject.activeSelf)
        {
            addressText.text = "Wallet Address: " + address;
        }
    }

    public void ShowCredits()
    {
        creditsPanel.SetActive(true);
        mainPanel.SetActive(false);
    }

    public void HideCredits()
    {
        creditsPanel.SetActive(false);
        mainPanel.SetActive(true);
    }
}
