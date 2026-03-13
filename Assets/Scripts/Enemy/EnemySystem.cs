using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySystem : MonoBehaviour
{
    public static EnemySystem instance {get; private set;}
    public Transform player;
    public float maxRadius = 20f;
    public float minRadius = 5f;
    public List<GameObject> enemies = new List<GameObject>();
    public GameObject enemyPrefab;
    public float enemySpawnChance = 20f;

    private float randomAngle;
    private float radianOfAngle;

    void Awake()
    {
        instance = this;
    }

    public void SpawnEnemy()
    {
        float chance = Random.Range(0f, 100f);
        if(chance <= enemySpawnChance)
        {
            RandomizePoint();
        }
    }
    void RandomizePoint()
    {
        randomAngle = Random.Range(0f, 360f);
        radianOfAngle = randomAngle * Mathf.Deg2Rad;
        Vector2 randomPoint = GetPoint();
        GameObject enemy = Instantiate(enemyPrefab, randomPoint, Quaternion.identity);
        enemies.Add(enemy);
        // Debug.Log(randomPoint);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Y))
        {
            RandomizePoint();
        }
    }

    Vector2 GetPoint()
    {
        float radius = Random.Range(minRadius, maxRadius);
        float x = player.position.x + radius * Mathf.Cos(radianOfAngle);
        float y = player.position.y + radius * Mathf.Sin(radianOfAngle);

        return new Vector2(x,y);
    }
}
