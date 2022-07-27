using Photon.Pun;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;



public class RhinoEnemy : MonoBehaviourPunCallbacks, IDamageable
{

    [SerializeField] private float currentHealth;
    [SerializeField] private float maxHealth;
    [SerializeField] private float minAttackDamage;
    [SerializeField] private float maxAttackDamage;



    PhotonView PV;

    private bool isWalking = false;
    private bool isChasing = false;


    private bool isAttacking = false;

    private bool isDead = false;

    private bool canChase = true;

    [HideInInspector] public GameObject target;

    [SerializeField] float knockStr;


    private Transform player;

    private NavMeshAgent agent;

    public float enemyDistance = 0.7f;

    private void Start()
    {
        StartCoroutine(EnemyCheckPlayer());
        agent = GetComponent<NavMeshAgent>();
    }

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }
    void Update()
    {
        target = GameObject.FindGameObjectWithTag("PlayerController");
        if (target == true)
        {
            Debug.Log("player found");
            transform.LookAt(player);
            agent.SetDestination(player.transform.position);

            if (Vector3.Distance(transform.position, player.position) < enemyDistance)
            {
                gameObject.GetComponent<NavMeshAgent>().velocity = Vector3.zero;
                gameObject.GetComponent<Animator>().Play("Atac");


            }
            else
            {
                gameObject.GetComponent<Animator>().Play("Run");
                //gameObject.GetComponent<impact>().enabled = false;
            }

        }
        else if (target == null)
            Debug.Log("player dead");
            StartCoroutine(EnemyCheckPlayer());
    }

    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody rb = collision.collider.GetComponent<Rigidbody>();
        if (rb != null && rb.tag == "PlayerController")
        {
            Debug.Log("hit player");
            Vector3 direction = collision.transform.position - transform.position;
            direction.y = 0;
            rb.AddForce(direction.normalized * knockStr, ForceMode.Impulse);
            collision.gameObject.GetComponent<IDamageable>()?.TakeDamageFromMonster(Random.Range(minAttackDamage, maxAttackDamage));
        }
    }

    public void TakeDamageFromMonster(float damage)
    {
        Debug.Log($"Nothing");
    }
    public void TakeDamage(float damage)
    {
        PV.RPC(nameof(RPC_TakeDamage), PV.Owner, damage);
    }

    [PunRPC]
    private void RPC_TakeDamage(float damage, PhotonMessageInfo info)
    {
        currentHealth -= damage;

       

        if (currentHealth <= 0)
        {
            if (isDead == true)
                return;

            this.tag = "DeadZombie";

            isDead = true;

            PlayerManager.Find(info.Sender).GetMonsterKill();

            StartCoroutine(DelayedDestroy());
        }

    }

    private IEnumerator DelayedDestroy()
    {
        yield return new WaitForSeconds(1.0f);
        PhotonNetwork.Destroy(gameObject);
    }
    private IEnumerator EnemyCheckPlayer()
    {

        yield return new WaitForSeconds(2.0f);
        player = GameObject.FindGameObjectWithTag("PlayerController").transform;
        target = GameObject.FindGameObjectWithTag("PlayerController");


    }



}
