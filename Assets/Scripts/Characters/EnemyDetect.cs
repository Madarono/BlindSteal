using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetect : MonoBehaviour
{
    public bool detectedPlayer;
    public bool canCheck;
    public float lingerDuration = 1f;
    [Header("Find Player")]
    public int rayCount;
    public float lookRange = 3f;
    public LayerMask detectLayers;

    void Start()
    {
        canCheck = true;
    }

    void Update()
    {
        if(!canCheck)
        {
            return;
        }

        FindPlayer();
    }
    void FindPlayer()
    {
        bool foundPlayer = false;
        for (int i = 0; i < rayCount; i++)
        {
            float angle = i * Mathf.PI * 2 / rayCount;
            Vector3 direction = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, lookRange, detectLayers);
            Debug.DrawRay(transform.position, direction * lookRange, Color.green);

            if (hit.collider != null)
            {
                if(hit.collider.gameObject.CompareTag("Player"))
                {
                    foundPlayer = true;
                    break;
                }
            }
        }

        detectedPlayer = foundPlayer;

        if(detectedPlayer)
        {
            StartCoroutine(CheckCooldown());
        }
    }

    IEnumerator CheckCooldown()
    {
        canCheck = false;
        yield return new WaitForSeconds(lingerDuration);
        canCheck = true;
    }
}
