using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FPS/New Gun")]
public class GunInfo : ItemInfo
{
    public float minDamage;
    public float maxDamage;

    public bool automatic;

    public bool canShoot = true;

    public float bulletDelay;

    public float magCapacity;
    public float bulletsToReload;

}
