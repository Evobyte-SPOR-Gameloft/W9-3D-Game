using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Gun : Item
{
    public abstract override void UsePrimary();
    public abstract override void UseSecondary();

    public GameObject bulletImpactPrefab;

    public float reloadTime;
}
