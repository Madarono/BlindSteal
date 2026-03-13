using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryItems : MonoBehaviour
{
    public Image itself;
    public int amount;
    public TextMeshProUGUI amountVisual;
    public int blockID;

    public void Refresh()
    {
        amountVisual.text = amount.ToString();
    }
}
