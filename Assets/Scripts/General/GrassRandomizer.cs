using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GrassRandomizer : MonoBehaviour
{
    public Tilemap ground;
    public TilemapBaker baker;

    [Header("Dimensions")]
    public int height = 64;
    public int width = 64;

    public int offsetHeight;
    public int offsetWidth;
    
    [Header("TileBases")]
    public TileBase[] flowerTiles;
    public float flowerChance = 20f;
    public TileBase[] groundTiles;
    public float groundChance = 40f;
    public TileBase clearTile;

    void Start()
    {
        GenerateGround(); 
        baker.Bake();  
        ground.gameObject.SetActive(false);
    }

    void GenerateGround()
    {
        for(int x = offsetWidth; x < width - offsetWidth; x++)
        {
            for(int y = offsetHeight; y < height - offsetHeight; y++)
            {
                float chance = Random.Range(0, 100);
                TileBase tileBase = clearTile;
                Vector3Int dimension = new Vector3Int(x, y, 0);
                
                if(chance <= flowerChance)
                {
                    tileBase = flowerTiles[Random.Range(0, flowerTiles.Length)];
                }
                else if(chance <= groundChance)
                {
                    tileBase = groundTiles[Random.Range(0, groundTiles.Length)];
                }
                else
                {
                    tileBase = clearTile;
                }

                ground.SetTile(dimension, tileBase);
            }
        }
    }
}
