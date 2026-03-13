using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using NavMeshPlus.Components;
using UnityEngine.Tilemaps;

[System.Serializable]
public class HotBar
{
    public Transform parent;
    public bool isFull;
    public InventoryItems item;
}

public class BuildingSystem : MonoBehaviour
{
    public static BuildingSystem instance {get; private set;} 
    public float buildingRange = 5f;   
    public bool isBuilding;
    public Tilemap buildings;
    public Tilemap builtObjects;
    public ShadowCaster2DCreator shadow;
    public GameObject selection;

    [Header("Buildable objects")]
    public NavMeshSurface meshSurface;
    public TileBase selectedObj;
    public TileBase[] blocks;
    public Sprite[] blocks_sprite;

    [Header("Hotbar")]
    public GameObject hotBarPrefab;
    public HotBar[] hotbar;
    public GameObject hotbarSelection;
    public int hotbarSelectionID = 0;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        SaveNewPath();
        selection.SetActive(false);
        hotbarSelection.SetActive(false);
        FillHotbar();
    }

    void Update()
    {
        if(Input.GetKeyDown(Settings.instance.building))
        {
            isBuilding = !isBuilding;
            CheckHotbarItem();
        }

        if(!isBuilding)
        {
            selection.SetActive(false);
            hotbarSelection.SetActive(false);
            return;
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        hotbarSelection.SetActive(true);

        if (scroll < 0f)
        {
            hotbarSelectionID++;
            if(hotbarSelectionID >= hotbar.Length)
            {
                hotbarSelectionID = 0;
            }
            CheckHotbarItem();
        }
        else if (scroll > 0f)
        {
            hotbarSelectionID--;
            if(hotbarSelectionID < 0)
            {
                hotbarSelectionID = hotbar.Length - 1;
            }
            CheckHotbarItem();
        }

        Vector3Int? pos = GetTilePosition();
        selection.SetActive(pos.HasValue);
        if(pos.HasValue)
        {
            Vector3 newPos = new Vector3(pos.Value.x + 0.5f, pos.Value.y + 0.5f, pos.Value.z);
            selection.transform.position = newPos;
            if(Input.GetMouseButtonDown(0) && selectedObj != null)
            {
                hotbar[hotbarSelectionID].item.amount--;
                hotbar[hotbarSelectionID].item.Refresh();
                StartCoroutine(PlaceBlock(pos.Value));
                if(hotbar[hotbarSelectionID].item.amount == 0)
                {
                    Destroy(hotbar[hotbarSelectionID].item.gameObject);
                    hotbar[hotbarSelectionID].isFull = false;
                    hotbar[hotbarSelectionID].item = null;
                    CheckHotbarItem();
                }

                UploadToInventory(); //Important later in the Load&Save System
            }
            Debug.Log(pos);
        }
    }

    public void CheckHotbarItem()
    {   
        hotbarSelection.transform.position = hotbar[hotbarSelectionID].parent.position;
        if(hotbar[hotbarSelectionID].item != null)
        {
            selectedObj = blocks[hotbar[hotbarSelectionID].item.blockID];
        }
        else
        {
            selectedObj = null;
        }
    }

    void UploadToInventory()
    {
        Inventory inventory = Inventory.instance;
        inventory.items.Clear();
        inventory.items = hotbar
        .Where(bar => bar.isFull) 
        .Select(bar => new HotbarItem {
            icon = blocks_sprite[bar.item.blockID],
            amount = bar.item.amount, 
            blockID = bar.item.blockID}).ToList();
    }
    public void FillHotbar()
    {
        int placement = 0;
        foreach(var item in Inventory.instance.items)
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

    Vector3Int? GetTilePosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10f; 
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

        Vector3Int buildingPosition = buildings.WorldToCell(worldPos);
        if(buildings.GetTile(buildingPosition) == null && builtObjects.GetTile(buildingPosition) == null && Vector2.Distance(worldPos, Settings.instance.player.position) <= buildingRange && Vector2.Distance(worldPos, Settings.instance.player.position) >= 0.9f)
        {
            return buildingPosition;
        }
        else
        {
            return null;
        }
    }

    public void SaveNewPath() 
    {
        meshSurface.UpdateNavMesh(meshSurface.navMeshData);
    }
    IEnumerator PlaceBlock(Vector3Int pos)
    {
        builtObjects.SetTile(pos, selectedObj);
        yield return null;       
        shadow.DestroyOldShadowCasters();
        shadow.Create();
        SaveNewPath();
    }
}