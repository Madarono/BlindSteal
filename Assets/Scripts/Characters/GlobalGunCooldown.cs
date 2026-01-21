using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalGunCooldown : MonoBehaviour
{
    public static GlobalGunCooldown instance {get; private set;}
    public Gun gun;

    void Awake()
    {
        instance = this;
    }

    IEnumerator ShootCooldown()
    {
        gun.canShoot = false;
        yield return new WaitForSeconds(gun.shootCooldown);
        gun.canShoot = true;
    }
}
