using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SingleShotGun : Gun
{
    [SerializeField] private float impactForce = 10f;
    [SerializeField] Camera cam;
    PhotonView PV;

    private Recoil recoilScript;
    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        recoilScript = GameObject.Find("Camera").GetComponent<Recoil>();
    }

    public override void UsePrimary()
    {
        Shoot();
    }

    public override void UseSecondary()
    {
        Debug.Log("No secondary action was implemented!");
    }


    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("R KEY PRESSED");
            StartCoroutine(ReloadGun());
        }

        if (Input.GetMouseButtonUp(0))
        {
            ShootingCooldownFinished();
        }
    }

    private void Shoot()
    {
        if(((GunInfo)itemInfo).automatic == true)//CHANGE FIREMODE INSTRUCTIONS
        {
            if(Input.GetKeyDown(KeyCode.V))
            {
                Debug.Log("V KEY PRESSED");
                ((GunInfo)itemInfo).automatic = false;
                Debug.Log("FIRE MODE: SINGLE SHOT");
            }
        }
        else if(((GunInfo)itemInfo).automatic == false)
        {
            if(Input.GetKeyDown(KeyCode.V))
            {
                Debug.Log("V KEY PRESSED");
                ((GunInfo)itemInfo).automatic = true;
                Debug.Log("FIRE MODE: AUTOMATIC");
            }
        }
        if (((GunInfo)itemInfo).automatic == true)
        {
            
            if (!((GunInfo)itemInfo).canShoot)
                return;

            if (((GunInfo)itemInfo).magCapacity > 0 && cam != null)
            {
                
                Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
                RaycastHit hit;
                ray.origin = cam.transform.position;

                if (Physics.Raycast(ray, out hit, 100, 1 << LayerMask.NameToLayer("Destructible"), QueryTriggerInteraction.Ignore))
                {
                    hit.collider.GetComponent<PhotonView>().RequestOwnership();
                    hit.collider.GetComponent<DestroyedPieceController>().CauseDamage(ray.direction * impactForce);
                    PV.RPC("RPC_Shoot", RpcTarget.All, hit.point, hit.normal);
                }
                else if (Physics.Raycast(ray, out hit))
                {
                    hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(Random.Range(((GunInfo)itemInfo).minDamage, ((GunInfo)itemInfo).maxDamage));
                    PV.RPC(nameof(RPC_Shoot), RpcTarget.All, hit.point, hit.normal);
                }

                ((GunInfo)itemInfo).magCapacity -= 1;
                Debug.Log(((GunInfo)itemInfo).magCapacity);

                if (((GunInfo)itemInfo).magCapacity == 0)
                {
                    Debug.Log("Out of ammo");
                    StartCoroutine(ReloadGun());
                }

                ((GunInfo)itemInfo).canShoot = false;
                Invoke(nameof(ShootingCooldownFinished), ((GunInfo)itemInfo).bulletDelay);
                recoilScript.RecoilFire();
            }
            
        }
        else
        {
            if (!((GunInfo)itemInfo).canShoot)
                return;

            if (((GunInfo)itemInfo).magCapacity > 0 && cam != null)
            {
                Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
                RaycastHit hit;
                ray.origin = cam.transform.position;

                if (Physics.Raycast(ray, out hit, 100, 1 << LayerMask.NameToLayer("Destructible"), QueryTriggerInteraction.Ignore))
                {
                    hit.collider.GetComponent<PhotonView>().RequestOwnership();
                    hit.collider.GetComponent<DestroyedPieceController>().CauseDamage(ray.direction * impactForce);
                    PV.RPC("RPC_Shoot", RpcTarget.All, hit.point, hit.normal);
                }
                else if (Physics.Raycast(ray, out hit))
                {
                    hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(Random.Range(((GunInfo)itemInfo).minDamage, ((GunInfo)itemInfo).maxDamage));
                    PV.RPC(nameof(RPC_Shoot), RpcTarget.All, hit.point, hit.normal);
                }

                ((GunInfo)itemInfo).magCapacity -= 1;
                Debug.Log(((GunInfo)itemInfo).magCapacity);

                if (((GunInfo)itemInfo).magCapacity == 0)
                {
                    Debug.Log("Out of ammo");
                    StartCoroutine(ReloadGun());
                }

                ((GunInfo)itemInfo).canShoot = false;
            }
        }
    }

    private void ShootingCooldownFinished()
    {
        ((GunInfo)itemInfo).canShoot = true;
    }

    IEnumerator ReloadGun()
    {
        Debug.Log("Reloading gun");
        yield return new WaitForSeconds(reloadTime);
        ((GunInfo)itemInfo).magCapacity = ((GunInfo)itemInfo).bulletsToReload;
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
