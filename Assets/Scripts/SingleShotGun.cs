using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SingleShotGun : Gun
{

    [SerializeField] Camera cam;
    PhotonView PV;

    public float fireRate;

    private float lastFired;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    public override void Use()
    {
        Shoot();
    }
    

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("R KEY PRESSED");
            StartCoroutine(ReloadGun());
        }

        if(Input.GetKey(KeyCode.Mouse0))
        {
            if(fireRate > 0)
            {
                 if (Time.time - lastFired > 1 / fireRate)
                 {
                    Shoot();
                    lastFired = Time.time;
                 }
            }
        }
    }

    private void Shoot()
    {
        if(magCapacity > 0)
        {
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
            ray.origin = cam.transform.position;
            if(Physics.Raycast(ray, out RaycastHit hit))
            {
                hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(Random.Range(((GunInfo)itemInfo).minDamage, ((GunInfo)itemInfo).maxDamage));
                PV.RPC("RPC_Shoot", RpcTarget.All, hit.point, hit.normal);
            }
            magCapacity -= 1;
            Debug.Log(magCapacity);
            if(magCapacity == 0)
            {
                Debug.Log("Out of ammo");
                StartCoroutine(ReloadGun());
            }
        }
        
    }

    IEnumerator ReloadGun()
    {
        Debug.Log("Reloading gun");
        yield return new WaitForSeconds(reloadTime);
        magCapacity = bulletsToReload;
        Debug.Log("Reloaded new magazine");
    }

    [PunRPC]
    private void RPC_Shoot(Vector3 hitPosition, Vector3 hitNormal)
    {
        Collider[] colliders = Physics.OverlapSphere(hitPosition, 0.3f);
        if(colliders.Length != 0)
        {
            GameObject bulletImpactObj = Instantiate(bulletImpactPrefab, hitPosition + hitNormal * 0.001f, Quaternion.LookRotation(hitNormal, Vector3.up) * bulletImpactPrefab.transform.rotation);
            Destroy(bulletImpactObj, 10f);
            bulletImpactObj.transform.SetParent(colliders[0].transform);
        }
    }

}
