using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Numerics;
using TMPro;
using System.Threading.Tasks;
using System;

public class TbnbManager : MonoBehaviour
{
    // public TMP_Text playerCurrencyDisplay; // [SerializeField] wasn't working
    // public TextMeshProUGUI tbnbBalanceDisplayPrefab;
    public TextMeshProUGUI tbnbBalanceDisplayInstance;

    // set chain: ethereum, moonbeam, polygon etc
    string chain = "binance";
    // set network mainnet, testnet
    string network = "testnet";

    // TextMeshProUGUI tbnbBalanceDisplayInstance;

    string ConvertToDecimals(string weiString)
    {
        BigInteger wei = BigInteger.Parse(weiString);
        BigInteger decimals = BigInteger.Pow(10, 18); // 18 decimals
        decimal eth = (decimal)wei / (decimal)decimals;
        return eth.ToString();
    }

    public async void CheckBalance()
    {
        // if (tbnbBalanceDisplayInstance != null)
        // {
        //     // If a balance display already exists, cancel the destruction routine
        //     // and destroy it immediately
        //     CancelInvoke(nameof(DestroyBalanceDisplay));
        //     Destroy(tbnbBalanceDisplayInstance.gameObject);
        // }

        if (tbnbBalanceDisplayInstance.gameObject.activeSelf) // if gameObject is active
        {
            CancelInvoke("DeactivateBalanceDisplay"); // stop the 5 second deactivate timer
        }
        
        string account = PlayerPrefs.GetString("Account"); // probably should move this outside for speed

        string balance = await EVM.BalanceOf(chain, network, account);
        balance = ConvertToDecimals(balance);
        Debug.Log("Check balance: " + balance);

        // tbnbBalanceDisplayInstance = Instantiate(tbnbBalanceDisplayPrefab);
        tbnbBalanceDisplayInstance.text = "TBNB Balance: " + balance;
        tbnbBalanceDisplayInstance.gameObject.SetActive(true);

        Invoke("DeactivateBalanceDisplay", 5f);
        // Invoke(nameof(DestroyBalanceDisplay), 5);
        // Destroy(tbnbBalanceDisplayInstance, 5); // Kill after 5 seconds
    }

    void DeactivateBalanceDisplay()
    {
        tbnbBalanceDisplayInstance.gameObject.SetActive(false);
    }

    // void DestroyBalanceDisplay()
    // {
    //     // Destroy the balance display
    //     if (tbnbBalanceDisplayInstance != null)
    //     {
    //         Destroy(tbnbBalanceDisplayInstance.gameObject);
    //         tbnbBalanceDisplayInstance = null;
    //     }
    // }

    void Start()
    {
        PlayerPrefs.SetString("ProjectID", "27395ce6-d08c-4d7f-aeae-e93f3f61e403"); // Required for ChainSafe functions to work (otherwise BalanceOf returns array null error)

        PlayerPrefs.SetString("Chain", "binance");
        PlayerPrefs.SetString("Network", "testnet");
    }

    public async void RewardCurrency()
    {
        // private key of account
        string privateKey = "0e4f00d307d22fb2137dc7527ab03aeb71fbcb1c564dff5ac3797ce15119fbb5"; // Developer wallet
        // account of player        
        string account = Web3PrivateKey.Address(privateKey); // Developer wallet
        // account to send to
        string to = PlayerPrefs.GetString("Account");
        // value in wei
        string value = "100000000000000"; // 0.0001 tBNB
        // optional rpc url
        string rpc = "";

        string chainId = await EVM.ChainId(chain, network, rpc);
        string gasPrice = await EVM.GasPrice(chain, network, rpc);
        string data = "";
        string gasLimit = "75000";
        string transaction = await EVM.CreateTransaction(chain, network, account, to, value, data, gasPrice, gasLimit, rpc);
        Debug.Log("transaction: " + transaction);
        string signature = Web3PrivateKey.SignTransaction(privateKey, transaction, chainId);
        string response = await EVM.BroadcastTransaction(chain, network, account, to, value, data, signature, gasPrice, gasLimit, rpc);
        Debug.Log("response: " + response);

        // string txConfirmed = await EVM.TxStatus(chain, network, transaction);
        // Debug.Log("txConfirmed: " + txConfirmed); // success, fail, pending

        // if pending, wait check again and loop until not pending? max 20 second?
        // call update display currency
        // WaitForTxConfirmation(response); // this is stuck on pending
    }

    public async void ChargeCurrency()
    {
        // account to send to
        string to = "0xBD3aB50BCD451b8A900A6aBf430698FF3bB91248";
        // amount in wei to send
        string value = "1000000000000000"; // 0.001 tBNB
        // gas limit OPTIONAL
        string gasLimit = "";
        // optional rpc url
        string rpc = "";
        // gas price OPTIONAL
        string gasPrice = await EVM.GasPrice(chain, network, rpc);
        // connects to user's browser wallet (metamask) to send a transaction
        try {
            string response = await Web3GL.SendTransaction(to, value, gasLimit, gasPrice);
            Debug.Log("Playment response: " + response);
        } catch (Exception e) {
            Debug.LogException(e, this);
        }
    }

    // public async void ChargeCurrency2()
    // {
    //     // https://chainlist.org/
    //     string chainId = "12"; // tbnb
    //     // account to send to
    //     string to = "0xBD3aB50BCD451b8A900A6aBf430698FF3bB91248";
    //     // value in wei
    //     string value = "1000000000000000";
    //     // data OPTIONAL
    //     string data = "";
    //     // gas limit OPTIONAL
    //     string gasLimit = "";
    //     // gas price OPTIONAL
    //     string gasPrice = "";
    //     // send transaction
    //     string response = await Web3Wallet.SendTransaction(chainId, to, value, data, gasLimit, gasPrice);
    //     print(response);
    // }

    // Doesn't work
    // Update currency display when transaction finishes
    // public async void WaitForTxConfirmation(string transaction) // Doesn't work
    // {
    //     float MaxWaitTimeSeconds = 20f;
        
    //     string txStatus = await EVM.TxStatus(chain, network, transaction);
    //     Debug.Log("transaction status: " + txStatus);
                
    //     if (txStatus.Equals("pending"))
    //     {
    //         Debug.Log("passed check!!");
    //         // If transaction is pending, wait and check again until it's confirmed or max wait time is reached
    //         float elapsedTime = 0f;
    //         Debug.Log(elapsedTime < MaxWaitTimeSeconds);
    //         while (txStatus.Equals("pending") && elapsedTime < MaxWaitTimeSeconds)
    //         {
    //             Debug.Log("Entered the loop");
    //             elapsedTime += Time.deltaTime;
    //             txStatus = await EVM.TxStatus(chain, network, transaction);
    //             Debug.Log(txStatus);
    //             Debug.Log(elapsedTime);
    //         }
    //     }

    //     Debug.Log("final tx status: " + txStatus);
    //     // Implement your logic to update the display currency based on the transaction status
    //     // For example:
    //     if (txStatus.Equals("success"))
    //     {
    //         Debug.Log("Transaction succeeded. Update display currency.");
    //         UpdateCurrencyDisplay();
    //     }
    //     else if (txStatus.Equals("fail"))
    //     {
    //         Debug.Log("Transaction failed.");
    //     }
    //     else if (txStatus.Equals("pending"))
    //     {
    //         Debug.Log("Transaction is still pending after maximum wait time.");
    //     }
    // }
}
