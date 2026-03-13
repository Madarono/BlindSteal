using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using NavMeshPlus.Components;
using UnityEngine.Tilemaps;

[System.Serializable]
public class ReferenceBlocks
{
    public TileBase block;
    public int block_health;
    public int amountGiven;
    public int resourceID;
}

[System.Serializable]
public class BlockData
{
    public int health;
    public int amountGiven = 1;
    public int resourceID = 0;
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
    public int selectedhealth;
    public int selectedID;
    public int selectedAmount;
    public ReferenceBlocks[] blocks;
    public Sprite[] blocks_sprite;

    public Dictionary<Vector3Int, BlockData> blocksPlaced = new Dictionary<Vector3Int, BlockData>();

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        SaveNewPath();
        selection.SetActive(false);
    }

    void Update()
    {
        isBuilding = selectedObj ? true : false;

        if(!isBuilding)
        {
            selection.SetActive(false);
            return;
        }

        Vector3Int? pos = GetTilePosition();
        selection.SetActive(pos.HasValue);
        if(pos.HasValue)
        {
            Vector3 newPos = new Vector3(pos.Value.x + 0.5f, pos.Value.y + 0.5f, pos.Value.z);
            selection.transform.position = newPos;
            if(Input.GetMouseButtonDown(0) && selectedObj != null)
            {
                StartCoroutine(PlaceBlock(pos.Value));
                Inventory.instance.DecreaseFromInventory(Inventory.instance.selectionID, 1);
            }
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
        blocksPlaced.Add(pos, new BlockData {health = selectedhealth, amountGiven = selectedAmount, resourceID = selectedID});

        yield return null;       
        shadow.DestroyOldShadowCasters();
        shadow.Create();
        SaveNewPath();
    }
}