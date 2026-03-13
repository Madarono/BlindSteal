using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DayCycle : MonoBehaviour
{
    public Light2D globalLight;
    public float maxIntensity = 2f;
    public float minIntensity = 0.1f;
    public float transitionInMinutes = 5f;
    public float nightDurationInMinutes = 2.5f;
    public float dayDurationInMinutes = 2.5f;

    [Header("Enemy Spawning")]
    public float enemySpawnCooldown = 5f;
    Coroutine enemySpawn;

    void Start()
    {
        StartCoroutine(TransitionDay());
    }

    IEnumerator TransitionDay()
    {
        float t = 0;
        float seconds = transitionInMinutes * 60f;
        bool isNight = false;

        while(true)
        {   
            globalLight.intensity = maxIntensity;
            if(enemySpawn != null)
            {
                StopCoroutine(enemySpawn);
            }
            yield return new WaitForSeconds(dayDurationInMinutes * 60f);

            while (t < seconds && !isNight)
            {
                t += Time.deltaTime;
                globalLight.intensity = Mathf.Lerp(maxIntensity, minIntensity, t / seconds);
                if(t >= seconds)
                {
                    isNight = true;
                    t = 0;
                    // Debug.Log("Night");
                }
                yield return null;
            }
            
            if(enemySpawn != null)
            {
                StopCoroutine(enemySpawn);
            }
            enemySpawn = StartCoroutine(SpawnEnemyCycle());
            yield return new WaitForSeconds(nightDurationInMinutes * 60f);

            while(t < seconds && isNight)
            {
                t += Time.deltaTime;

                globalLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, t / seconds);
                if(t >= seconds)
                {
                    isNight = false;
                    t = 0;
                }
                yield return null;
            }
        }
    }

    IEnumerator SpawnEnemyCycle()
    {
        float enemy_t = 0;
        
        while(true)
        {
            enemy_t += Time.deltaTime;

            if(enemy_t >= enemySpawnCooldown)
            {
                enemy_t = 0;
                EnemySystem.instance.SpawnEnemy();
            }
            yield return null;
        }
    }
}
