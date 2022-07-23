using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    GameObject enemy;
    public MyEnum myDropDown = new MyEnum();


   
    PhotonView PV;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    private void Start()
    {
        StartCoroutine(EnemySpawnDelay());
        
    }
    void Update()
    {
        if(myDropDown.ToString()== "EnemyZombie")
        {
            Debug.Log("EnemyZombie");
        }
        else
            Debug.Log("Rhino");

        /*var dropDown1 = MyEnum.EnemyZombie;
        var dropDown2 = MyEnum.Rhino_PBR;*/
    }

    private void CreateEnemy()
    {
        
        Vector3 spawnpoint = new(transform.position.x + Random.Range(-3, 3), transform.position.y, transform.position.z + Random.Range(-3, 3));
        //var dropDown1 = MyEnum.EnemyZombie;
        enemy = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", myDropDown.ToString()), spawnpoint, new Quaternion(transform.rotation.x, Random.rotation.y, transform.rotation.z, Quaternion.identity.w), 0, new object[] { PV.ViewID });
    }
    public enum MyEnum
    {
        EnemyZombie,
        Rhino_PBR,
    };
    private IEnumerator EnemySpawnDelay()
    {
        while (true)
        {
            yield return new WaitForSeconds(2.0f);
            CreateEnemy();
        }
    }
}
