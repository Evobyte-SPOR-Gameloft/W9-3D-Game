
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class move : MonoBehaviour
{
    private Transform player;
    /*public Object ImpactSCR;*/
    private NavMeshAgent agent;
    /*public GameObject rhino;*/
    public float enemyDistance = 0.7f;
    
    private void Start()
    {
        StartCoroutine(EnemyCheckPlayer());
        agent = GetComponent<NavMeshAgent>();
        
    }

    
    void Update()
    {
        if (GameObject.FindGameObjectWithTag("PlayerController") == true)
        {
            transform.LookAt(player);
            agent.SetDestination(player.transform.position);

            if (Vector3.Distance(transform.position, player.position) < enemyDistance)
            {
                gameObject.GetComponent<NavMeshAgent>().velocity = Vector3.zero;
                gameObject.GetComponent<Animator>().Play("Atac");
                gameObject.GetComponent<impact>().enabled = true;

            }
            else
            { 
                gameObject.GetComponent<Animator>().Play("Run");
                gameObject.GetComponent<impact>().enabled=false;
            }

        }
        else
            StartCoroutine(EnemyCheckPlayer());
    }
    private IEnumerator EnemyCheckPlayer()
    {
       
            yield return new WaitForSeconds(2.0f);
            player = GameObject.FindGameObjectWithTag("PlayerController").transform;
            
        
    }


}