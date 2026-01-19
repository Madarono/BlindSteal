using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] Transform player;
    NavMeshAgent agent;

    [Header("Attributes")]
    public float health;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    void Update()
    {
        agent.SetDestination(player.position);
    }

    public void Refresh()
    {
        if(health <= 0)
        {
            BloodManager.instance.DeathSplash(transform);
            Destroy(gameObject);
        }
    }
}
