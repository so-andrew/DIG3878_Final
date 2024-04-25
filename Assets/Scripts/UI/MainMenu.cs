using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainMenu : MonoBehaviour
{
    private string address;
    private GameObject addressObject;
    private TMP_Text addressText;
    // Start is called before the first frame update
    void Start()
    {
        address = PlayerPrefs.GetString("Account");
        addressObject = transform.Find("Panel").Find("Address").gameObject;
        addressText = addressObject.GetComponent<TMP_Text>();
        if (address == null || address.Length == 0)
        {
            addressObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (address == null)
        {
            address = PlayerPrefs.GetString("Account");
            addressObject.SetActive(true);
        }
        addressText.text = "Wallet Address: " + address;
    }
}
