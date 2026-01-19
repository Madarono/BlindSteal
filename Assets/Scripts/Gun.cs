using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public Animator anim;
    public float shootCooldown = 0.25f;
    public float shootRange = 5f;
    public float damage = 5f;
    public bool canShoot = true;
    public bool fullAuto = false;

    [Header("Visuals")]
    public LineFade lineFade;
    public GameObject particle;
    public Transform shootPosition;
    public GameObject light;
    public float lightDuration = 0.05f;
    
    void Start()
    {
        canShoot = true;
        light.SetActive(false);
    }
    void Update()
    {
        if(!gameObject.activeInHierarchy)
        {
            return;
        }

        if(((fullAuto && Input.GetMouseButton(0)) || (!fullAuto && Input.GetMouseButtonDown(0))) && canShoot)
        {
            Shoot();
        }
    }

    void Shoot()
    {
        //Mechanic
        RaycastHit2D hit = Physics2D.Raycast(shootPosition.position, shootPosition.right, shootRange);

        Vector2 endPoint;
        if (hit.collider != null)
        {
            endPoint = hit.point;
            GameObject obj = hit.collider.gameObject;

            if(obj.TryGetComponent(out Enemy enemy))
            {
                BloodManager.instance.MakeBloodParticle(obj.transform);
                BloodManager.instance.MakeBloodSplash(obj.transform, damage);
                enemy.health -= damage;
                enemy.Refresh();
            }
        }
        else
        {
            endPoint = shootPosition.position + shootPosition.right.normalized * shootRange;
        }
        
        LineRenderer lr = GetComponent<LineRenderer>();

        lineFade.ShowLine(shootPosition.position, endPoint);

        //Visual
        anim.SetTrigger("Shoot");
        GameObject go = Instantiate(particle, shootPosition.position, Quaternion.Euler(0, 0, transform.eulerAngles.z - 90));
        if(go.TryGetComponent(out ParticleSystem goScript))
        {
            goScript.Play();
        }
        Destroy(go, 0.7f);
        StartCoroutine(ShowLight());
        StartCoroutine(ShootCooldown());
    }

    IEnumerator ShowLight()
    {
        light.SetActive(true);
        yield return new WaitForSeconds(lightDuration);
        light.SetActive(false);
    }

    IEnumerator ShootCooldown()
    {
        canShoot = false;
        yield return new WaitForSeconds(shootCooldown);
        canShoot = true;
    }
}
