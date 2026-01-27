using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] Transform player;
    NavMeshAgent agent;
    public EnemyDetect detect;

    [Header("Inaccuracy")]
    public float minInaccuracy = -10f;
    public float maxInaccuracy = 10f;
    public float speedOfLooking = 4f;
    private float inaccuracy;

    [Header("Attributes")]
    public Gun gun;
    public float health = 10f;
    public float speed = 2f;
    public float reloadTime = 0.75f;
    public float damage = 5f;
    public float shootRange = 5f;
    public float minResponseTime = 0.8f;
    public float maxResponseTime = 1.2f;

    [Header("Check Player")]
    public float distanceFromPlayer = 3f;
    public Transform checkPosition;
    public LayerMask playerLayer;

    [Header("Death")]
    public GameObject deathPrefab;

    private Coroutine responseTimeCoroutine;
    bool inLineOfSight = false;
    bool firstTime = true;
    bool canShoot = false;

    void Start()
    {
        player = Settings.instance.player;
        inaccuracy = Random.Range(minInaccuracy, maxInaccuracy); 
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed = speed;
        agent.stoppingDistance = distanceFromPlayer;
        gun.shootCooldown = reloadTime;
        gun.damage = damage;
        gun.shootRange = shootRange;
        inLineOfSight = false;
        firstTime = true;
        canShoot = false;
    }

    void Update()
    {
        bool foundPlayer = detect.detectedPlayer;
        agent.isStopped = !foundPlayer;
        if(!foundPlayer)
        {
            return;
        }
        agent.SetDestination(player.position);
        inLineOfSight = DetectPlayer();

        if(Vector2.Distance(player.position, transform.position) <= distanceFromPlayer && gun.canShoot && inLineOfSight)
        {
            if(firstTime)
            {
                firstTime = false;
                StartCoroutine(DelayBeforeShot());
            }
            else if(canShoot)
            {
                gun.Shoot();
            }
        }

        //LookAt Player 
        Vector2 direction = (player.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        Quaternion rot = Quaternion.Euler(0, 0, angle + inaccuracy);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, speedOfLooking * Time.deltaTime);
    }

    bool DetectPlayer()
    {
        RaycastHit2D hit = Physics2D.Raycast(checkPosition.position, transform.right, shootRange, playerLayer);
        RaycastHit2D hit2 = Physics2D.Raycast(checkPosition.position, Quaternion.Euler(0, 0, -45) * transform.right, shootRange, playerLayer);
        RaycastHit2D hit3 = Physics2D.Raycast(checkPosition.position, Quaternion.Euler(0, 0, 45) * transform.right, shootRange, playerLayer);
        
        List<RaycastHit2D> hits = new List<RaycastHit2D>();
        hits.Add(hit);
        hits.Add(hit2);
        hits.Add(hit3);

        Debug.DrawRay(checkPosition.position, transform.right * shootRange, Color.red);

        foreach(var _hit in hits)
        {    
            if(_hit.collider != null)
            {
                // Debug.Log(_hit.collider);
                if(_hit.collider.gameObject.TryGetComponent(out Player player))
                {
                    return true;
                }
            }
        }

        canShoot = false;
        firstTime = true;
        return false;
    }

    
    IEnumerator DelayBeforeShot()
    {
        float responseTime = Random.Range(minResponseTime, maxResponseTime);
        yield return new WaitForSeconds(responseTime);
        if(DetectPlayer())
        {
            canShoot = true;
            gun.Shoot();
        }
    }
    public void Refresh()
    {
        if(health <= 0)
        {
            BloodManager.instance.DeathSplash(transform);
            GameObject go = Instantiate(deathPrefab, transform.position, transform.rotation);
            if(go.TryGetComponent(out DeadBody goScript))
            {
                goScript.Fade();
            }
            Destroy(gameObject);
        }
    }
}
