using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    private Slider slider;
    private TMP_Text indicator;

    //private Color healthGreen = new Color(58, 178, 81, 255);

    void Start()
    {
        slider = GetComponent<Slider>();
        indicator = transform.Find("Indicator").gameObject.GetComponent<TMP_Text>();
    }

    public void SetSliderValue(float value)
    {
        slider.value = value;
    }

    public void ToggleGainIndicator(float change)
    {
        //Debug.Log($"Change = {change}");
        if (change > 0.005f)
        {
            indicator.text = "+";
            indicator.color = Color.green;
        }
        else if (change < -0.005f)
        {
            indicator.text = "-";
            indicator.color = Color.red;
        }
        else
        {
            indicator.text = "~";
            indicator.color = Color.white;
        }
    }
}
