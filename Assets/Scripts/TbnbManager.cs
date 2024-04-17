using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Numerics;
using TMPro;

public class TbnbManager : MonoBehaviour
{
    // public TMP_Text playerCurrencyDisplay; // [SerializeField] wasn't working
    public TextMeshProUGUI playerCurrencyDisplay;

    string ConvertToDecimals(string weiString)
    {
        BigInteger wei = BigInteger.Parse(weiString);
        BigInteger decimals = BigInteger.Pow(10, 18); // 18 decimals
        decimal eth = (decimal)wei / (decimal)decimals;
        return eth.ToString();
    }

    async void Start()
    {
        string chain = "binance";
        string network = "testnet"; // mainnet goerli
        string account = PlayerPrefs.GetString("Account"); // otherwise BalanceOf returns array null error
        PlayerPrefs.SetString("ProjectID", "27395ce6-d08c-4d7f-aeae-e93f3f61e403");

        string balance = await EVM.BalanceOf(chain, network, account);
        balance = ConvertToDecimals(balance);

        playerCurrencyDisplay.text = balance;
    }
}
