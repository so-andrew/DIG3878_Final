using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainUI : MonoBehaviour
{
    public GameObject cryptoBalanceButton;

    void Start()
    {
        string account = PlayerPrefs.GetString("Account");
        if (cryptoBalanceButton != null && account != null && account.Length > 0)
        {
            cryptoBalanceButton.SetActive(true);
        }
        else
        {
            cryptoBalanceButton.SetActive(false);
        }
    }
}
