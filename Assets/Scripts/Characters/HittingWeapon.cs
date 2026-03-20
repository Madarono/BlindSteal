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

        if(Input.GetMouseButton(0) && canHit)
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
        else if(ResourceSystem.instance.resourcesData.ContainsKey(tilePos))
        {
            ResourceSystem.instance.resourcesData[tilePos].health -= damage;
            hitting.transform.position = worldPos;
            hitting.gameObject.SetActive(true);
            hitting.Play();
            if(ResourceSystem.instance.resourcesData[tilePos].health <= 0)
            {
                Destroy(ResourceSystem.instance.resourcesData[tilePos].obj);
                Inventory.instance.resources[ResourceSystem.instance.resourcesData[tilePos].resourceID].amount += ResourceSystem.instance.resourcesData[tilePos].amountGiven; 
                ResourceSystem.instance.resourcesData.Remove(tilePos);
            }
            StartCoroutine(CooldownWeapon());
        }
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
