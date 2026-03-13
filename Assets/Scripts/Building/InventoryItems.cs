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
    
    public BlockType type;

    [Header("Blocks")]
    public int blockID;

    [Header("Weapons")]
    public int weaponID;

    public void Refresh()
    {
        if(type == BlockType.Holdable)
        {
            amountVisual.text = "";
            return;
        }

        amountVisual.text = amount.ToString();
    }
}
