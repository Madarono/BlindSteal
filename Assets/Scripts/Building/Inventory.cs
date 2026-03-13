using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HotbarItem
{
    public Sprite icon;
    public int amount;
    public int blockID;
}

public class Inventory : MonoBehaviour
{
    public static Inventory instance {get; private set;}

    public int moneyCount;
    public int woodCount;
    public int rockCount;
    public int ironCount;

    [Header("HotBar")]
    public List<HotbarItem> items = new List<HotbarItem>();

    [Header("Debug")]
    public bool isDebug = false;
    public int debug_blockID;
    public int debug_amount;
    public Sprite debug_icon;
    public KeyCode debug_keycode;

    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        if(Input.GetKeyDown(debug_keycode) && isDebug)
        {
            AddToBuildingInventory(debug_blockID, debug_amount, debug_icon);
        }
    }

    public void AddToBuildingInventory(int blockID, int amount, Sprite icon)
    {
        bool hasFound = false;
        foreach(var item in items)
        {
            if(item.blockID == blockID)
            {
                item.amount += amount;
                hasFound = true; 
                break;
            }
        }

        if(!hasFound)
        {
            items.Add(new HotbarItem{icon = icon, amount = amount, blockID = blockID});
        }

        BuildingSystem.instance.ClearHotBar();
        BuildingSystem.instance.FillHotbar();
        BuildingSystem.instance.CheckHotbarItem();
    }
}
