using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BlockType
{
    Buildable,
    Holdable,
}

public enum ResourceType
{
    Wood,
    Rock,
    Steel
}

[System.Serializable]
public class HotbarItem
{
    public Sprite icon;
    public int amount;
    public BlockType type;
    
    [Header("Block")]
    public int blockID;
    
    [Header("Weapons")]
    public int weaponID;
}

[System.Serializable]
public class HotBar
{
    public Transform parent;
    public bool isFull;
    public InventoryItems item;
}

[System.Serializable]
public class Resource 
{
    public ResourceType type;
    public int amount;
}

public class Inventory : MonoBehaviour
{
    public static Inventory instance {get; private set;}

    public int moneyCount;
    public Resource[] resources;
    public Player player;
    
    [Header("HotBar")]
    public List<HotbarItem> items = new List<HotbarItem>();

    [Header("Hotbar")]
    public GameObject hotBarPrefab;
    public HotBar[] hotbar;
    public GameObject hotbarSelection;
    public int selectionID = 0;
    
    [Header("Debug")]
    public bool isDebug = false;
    public int debug_blockID;
    public int debug_amount;
    public Sprite debug_icon;
    public BlockType debug_type;
    public KeyCode debug_keycode;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        FillHotbar();
        hotbarSelection.SetActive(true);
        CheckHotbarItem();
    }

    void Update()
    {

        if(Input.GetKeyDown(debug_keycode) && isDebug)
        {
            AddToInventory(debug_blockID, debug_amount, debug_icon, debug_type);
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        hotbarSelection.SetActive(true);

        if (scroll < 0f)
        {
            selectionID++;
            if(selectionID >= hotbar.Length)
            {
                selectionID = 0;
            }
            CheckHotbarItem();
        }
        else if (scroll > 0f)
        {
            selectionID--;
            if(selectionID < 0)
            {
                selectionID = hotbar.Length - 1;
            }
            CheckHotbarItem();
        }
    }

    public void AddToInventory(int blockID, int amount, Sprite icon, BlockType type)
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
            items.Add(new HotbarItem{icon = icon, amount = amount, blockID = blockID, type = type});
        }

        ClearHotBar();
        FillHotbar();
        CheckHotbarItem();
    }

    public void DecreaseFromInventory(int ID, int amount)
    {
        items[ID].amount -= amount;
        UpdateInventory();
        UpdateHotBar();
    }

    public void CheckHotbarItem()
    {   
        hotbarSelection.transform.position = hotbar[selectionID].parent.position;

        if(hotbar[selectionID].item == null)
        {
            BuildingSystem.instance.selectedObj = null;
            player.RemoveWeapon();
            return;
        }

        switch(hotbar[selectionID].item.type)
        {
            case BlockType.Buildable:
                BuildingSystem.instance.selectedObj = BuildingSystem.instance.blocks[hotbar[selectionID].item.blockID].block;
                BuildingSystem.instance.selectedhealth = BuildingSystem.instance.blocks[hotbar[selectionID].item.blockID].block_health;
                BuildingSystem.instance.selectedID = BuildingSystem.instance.blocks[hotbar[selectionID].item.blockID].resourceID;
                BuildingSystem.instance.selectedAmount = BuildingSystem.instance.blocks[hotbar[selectionID].item.blockID].amountGiven;
                player.RemoveWeapon();
                break;
            case BlockType.Holdable:
                player.SwitchWeapon(hotbar[selectionID].item.weaponID);
                BuildingSystem.instance.selectedObj = null;
                break;
        }
    }

    public void FillHotbar()
    {
        int placement = 0;
        foreach(var item in items)
        {
            if(placement >= hotbar.Length)
            {
                break;
            }

            GameObject go = Instantiate(hotBarPrefab, hotbar[placement].parent.position, Quaternion.identity);
            go.transform.SetParent(hotbar[placement].parent);
            go.transform.localScale = Vector3.one;
            if(go.TryGetComponent(out InventoryItems goScript))
            {
                goScript.itself.sprite = item.icon;
                goScript.amount = item.amount;
                goScript.blockID = item.blockID;
                goScript.weaponID = item.weaponID;
                goScript.type = item.type;
                goScript.Refresh();
                hotbar[placement].item = goScript;
                hotbar[placement].isFull = true;
            }
            placement++;
        }
    }

    public void ClearHotBar()
    {
        foreach(var bar in hotbar)
        {
            bar.isFull = false;
            if(bar.item != null)
            {
                Destroy(bar.item.gameObject);   
            }
            bar.item = null;   
        }
    }

    public void UpdateInventory()
    {
        for(int i = items.Count - 1; i >= 0; i--)
        {
            if(items[i].amount <= 0)
            {
                items.RemoveAt(i);
            }
        }

        
    }

    public void UpdateHotBar()
    {
        ClearHotBar();
        FillHotbar();
        CheckHotbarItem();
    }
    

}
