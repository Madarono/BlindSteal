using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HittingWeapon : MonoBehaviour
{
    public int damage = 1;
    public float cooldown = 1f;
    public float hittingRange = 1.5f;
    public bool canHit = true;
    public ParticleSystem hitting;

    void Update()
    {
        if(!gameObject.activeInHierarchy || BuildingSystem.instance.isBuilding)
        {
            return;
        }

        if(Input.GetMouseButtonDown(0) && canHit)
        {
            CheckForBlock();
        }
    }

    public void CheckForBlock()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10f; 
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

        if(Vector2.Distance(Settings.instance.player.position, worldPos) > hittingRange)
        {
            return;
        }

        Vector3Int tilePos = BuildingSystem.instance.buildings.WorldToCell(worldPos);
        if(BuildingSystem.instance.blocksPlaced.ContainsKey(tilePos))
        {
            BuildingSystem.instance.blocksPlaced[tilePos].health -= damage;
            hitting.transform.position = worldPos;
            hitting.gameObject.SetActive(true);
            hitting.Play();
            if(BuildingSystem.instance.blocksPlaced[tilePos].health <= 0)
            {
                BuildingSystem.instance.builtObjects.SetTile(tilePos, null);
                Inventory.instance.resources[BuildingSystem.instance.blocksPlaced[tilePos].resourceID].amount += BuildingSystem.instance.blocksPlaced[tilePos].amountGiven; 
                BuildingSystem.instance.SaveNewPath();
                BuildingSystem.instance.blocksPlaced.Remove(tilePos);
                StartCoroutine(RebuildShadow());
            }
            StartCoroutine(CooldownWeapon());
        }
        //else if, checking the sprites instead
    }

    IEnumerator RebuildShadow()
    {
        yield return null;
        BuildingSystem.instance.shadow.DestroyOldShadowCasters();
        BuildingSystem.instance.shadow.Create();
    }

    IEnumerator CooldownWeapon()
    {
        canHit = false;
        yield return new WaitForSeconds(cooldown);
        canHit = true;
    }
}
