using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    GameObject enemy;
    public MyEnum EnemySelection = new MyEnum();
    [SerializeField] Transform parent;
    PhotonView PV;
    int counter=5;
    bool cond;
    private IEnumerator coroutineSpawn, coroutineStart;
    bool CR;
    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    private void Start()
    {
        coroutineSpawn = EnemySpawnDelay();
        coroutineStart = StartTheCoroutine();
        StartCoroutine(coroutineSpawn);
        
    }

    void Update()
    {
        if (GameObject.Find("EnemySpawner").transform.childCount >= counter)
        { StopCoroutine(coroutineSpawn);
            Debug.Log("S-a oprit corutina de Spawn");
            cond = true;
        }
        Debug.Log("Numarul de inamici este :" + GameObject.Find("EnemySpawner").transform.childCount);
        if (GameObject.Find("EnemySpawner").transform.childCount == 0)
        {
            StartCoroutine(coroutineStart);
            Debug.Log("A inceput corutina de Start");
            CR = true;
            counter += 5;
        }

        if (CR == true)
        { 
                StartCoroutine(coroutineSpawn);
                Debug.Log("A inceput corutina de Spawn");
                CR = false;
        }

    }
    private void CreateEnemy()
    {
        Vector3 spawnpoint = new(transform.position.x + Random.Range(-3, 3), transform.position.y, transform.position.z + Random.Range(-3, 3));
        enemy = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", EnemySelection.ToString()), spawnpoint, new Quaternion(transform.rotation.x, Random.rotation.y, transform.rotation.z, Quaternion.identity.w), 0, new object[] { PV.ViewID });
        enemy.transform.parent=gameObject.transform;
    }

    public enum MyEnum
    {
        EnemyZombie,
        Rhino_PBR,
    };

    private IEnumerator EnemySpawnDelay()
    {
        cond = true;
        while (cond==true)
        {
            yield return new WaitForSeconds(2.0f);
            CreateEnemy();
            
        }
        

    }
    private IEnumerator StartTheCoroutine()
    {
            yield return new WaitForSeconds(5.0f);
            

    }


}
