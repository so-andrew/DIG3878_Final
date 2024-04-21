using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CurrencyDisplay : MonoBehaviour
{
    private TMP_Text playerCurrencyDisplay;
    // Start is called before the first frame update
    void Start()
    {
        playerCurrencyDisplay = transform.GetChild(1).GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        playerCurrencyDisplay.text = GameManager.Instance.playerCurrency.ToString();
    }
}
