
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class move : MonoBehaviour
{
    private Transform player;

    private NavMeshAgent agent;
   // public BoxCollider head;
    public float enemyDistance = 0.7f;
    public Transform player1;
    private void Start()
    {
        
        player = GameObject.FindWithTag("PlayerController").transform;
        agent = GetComponent<NavMeshAgent>();
        if (player==null)
        {
            StartCoroutine(EnemyCheckPlayer());
            Transform player2 = GameObject.FindWithTag("PlayerController").transform;
            player1 = player2;
        }
       // agent = GetComponent<NavMeshAgent>();
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
    private IEnumerator EnemyCheckPlayer()
    {
       
            yield return new WaitForSeconds(2.0f);
            
        
    }


}