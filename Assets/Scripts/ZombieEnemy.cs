using Photon.Pun;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;



public class ZombieEnemy : MonoBehaviourPunCallbacks, IDamageable
{
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float currentHealth;
    [SerializeField] private float maxHealth;
    [SerializeField] private float minAttackDamage;
    [SerializeField] private float maxAttackDamage;

    [SerializeField] Image healthbarImage;

    [SerializeField] private float animationMultiplier = 0.2f;

    private readonly string walkAnimation = "isWalking";
    private readonly string deathAnimation = "isDead";
    private readonly string attackAnimation = "isAttacking";
    private readonly string runAnimation = "isChasing";

    private readonly string moveMultiplier = "moveMultiplier";

    PhotonView PV;

    [SerializeField] Animator animator;
    [SerializeField] Rigidbody rb;

    private bool isWalking = false;
    private bool isChasing = false;

    private bool isRotatingLeft = false;
    private bool isRotatingRight = false;

    //private bool isStrolling = false;

    private bool isAttacking = false;

    private bool isDead = false;

    private bool canChase = true;

    private GameObject[] targets;

    public GameObject target;





    private Vector3 moveDirection;

    [SerializeField] private CapsuleCollider triggerCollider;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }
    void Update()
    {
        if (!PV.IsMine)
            return;

        ZombieAnimation();

        if (isDead)
            return;
        
        /*if(isStrolling == false)
        {
            StartCoroutine(Strolling());
        }

        StrollingRotationLogic();*/

        ZombieAnimation();

        if(target == null)
        {
            targets = GameObject.FindGameObjectsWithTag("PlayerController");

            target = targets[Random.Range(0, targets.Length)];
        }

        ChasePlayer();
    }
    private void ChasePlayer()
    {
        if (target == null)
            return;

        if (!canChase)
            return;

        Vector3 targetTransform = target.transform.position;

        targetTransform.y = transform.position.y;
        transform.LookAt(targetTransform);                                  

        isChasing = true;

        rb.MovePosition(transform.position + runSpeed * Time.deltaTime * Vector3.forward);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerController"))
        {
            canChase = false;

            isAttacking = true;

            other.gameObject.GetComponent<IDamageable>()?.TakeDamageFromMonster(Random.Range(minAttackDamage, maxAttackDamage));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        isAttacking = false;
        canChase = true;
    }


    /*private void StrollingRotationLogic()
    {
        if (isRotatingRight == true)
        {
            rb.transform.Rotate(rotationSpeed * Time.deltaTime * transform.up);
        }

        if (isRotatingLeft == true)
        {
            rb.transform.Rotate(-rotationSpeed * Time.deltaTime * transform.up);
        }

        if (isWalking == true)
        {
            rb.AddForce(transform.forward * walkSpeed);
        }
    }/*

    /*private IEnumerator Strolling()
    {
        float walkWait = Random.Range(1.0f, 2.0f);
        float walkTime = Random.Range(1.5f, 3.0f);

        float rotateWait = Random.Range(1.0f, 3.0f);
        float rotationTime = Random.Range(0.1f, 0.7f);

        int rotateDirection = Random.Range(1, 2);

        isStrolling = true;

        yield return new WaitForSeconds(walkWait);

        isWalking = true;

        yield return new WaitForSeconds(walkTime);

        isWalking = false;

        yield return new WaitForSeconds(rotateWait);

        //Left
        if(rotateDirection == 1)
        {
            isRotatingLeft = true;
            yield return new WaitForSeconds(rotationTime);
            isRotatingLeft = false;
        }

        //Right
        if (rotateDirection == 2)
        {
            isRotatingRight = true;
            yield return new WaitForSeconds(rotationTime);
            isRotatingRight = false;
        }

        isStrolling = false;
    }
    */

    private void ZombieAnimation()
    {
        //Walking
        if(isWalking == true)
        {
            animator.SetBool(walkAnimation, true);
            animator.SetFloat(moveMultiplier, walkSpeed * animationMultiplier);
        }
        else
        {
            animator.SetBool(walkAnimation, false);
        }

        //Chasing
        if (isChasing == true)
        {
            animator.SetBool(runAnimation, true);
            animator.SetFloat(moveMultiplier, runSpeed * animationMultiplier);
        }
        else
        {
            animator.SetBool(runAnimation, false);
        }

        //Attacking
        if(isAttacking == true)
        {
            animator.SetBool(attackAnimation, true);
        }
        else
        {
            animator.SetBool(attackAnimation, false);
        }

        //Death
        if(isDead == true)
        {
            animator.SetBool(deathAnimation, true);
        }
        else
        {
            animator.SetBool(deathAnimation, false);
        }
    }

    public void TakeDamageFromMonster(float damage)
    {
        Debug.Log($"Nothing here...");
    }
    public void TakeDamage(float damage)
    {
        PV.RPC(nameof(RPC_TakeDamage), RpcTarget.Others, damage);
    }

    [PunRPC]
    private void RPC_TakeDamage(float damage, PhotonMessageInfo info)
    {
        currentHealth -= damage;

        healthbarImage.fillAmount = currentHealth / maxHealth;

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




}
