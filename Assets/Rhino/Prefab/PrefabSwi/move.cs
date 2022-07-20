
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class move : MonoBehaviour
{
    public Transform player;

    private NavMeshAgent agent;

    public float enemyDistance = 0.7f;


    private void Start()
    {
        StartCoroutine(Agro());
        

        agent = GetComponent<NavMeshAgent>();
    }

    //Call every frame
    void Update()
    {
        //Look at the player
        transform.LookAt(player);
        //gameObject.GetComponent<Animator>().Play("Run");
        agent.SetDestination(player.transform.position);

        if (Vector3.Distance(transform.position, player.position) < enemyDistance)
        {
            gameObject.GetComponent<NavMeshAgent>().velocity = Vector3.zero;
            gameObject.GetComponent<Animator>().Play("Atac");
        }
        else
            gameObject.GetComponent<Animator>().Play("Run");

    }
    private IEnumerator Agro()
    {
        while (true)
        {
            yield return new WaitForSeconds(2.0f);
            player = GameObject.FindWithTag("PlayerController").transform;
        }
    }
}