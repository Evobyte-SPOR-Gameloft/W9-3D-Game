using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SingleShotGun : Gun
{

    [SerializeField] Camera cam;
    PhotonView PV;

    private int magCapacity = 15;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    public override void Use()
    {
        Shoot();
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
        }
        else
        {
            StartCoroutine(ReloadGun());
        }
    }

    IEnumerator ReloadGun()
    {
        
            Debug.Log("Reloading gun");
            yield return new WaitForSeconds(2.5f);
            magCapacity = 15;
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
