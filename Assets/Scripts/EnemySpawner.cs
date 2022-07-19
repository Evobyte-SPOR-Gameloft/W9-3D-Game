using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    GameObject enemy;

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
        
    }

    private void CreateEnemy()
    {
        Vector3 spawnpoint = new(transform.position.x + Random.Range(-3, 3), transform.position.y, transform.position.z + Random.Range(-3, 3));
        enemy = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "EnemyZombie"), spawnpoint, new Quaternion(transform.rotation.x, Random.rotation.y, transform.rotation.z, Quaternion.identity.w), 0, new object[] { PV.ViewID });
    }

    private IEnumerator EnemySpawnDelay()
    {
        while (true)
        {
            yield return new WaitForSeconds(2.0f);
            CreateEnemy();
        }
    }
}
