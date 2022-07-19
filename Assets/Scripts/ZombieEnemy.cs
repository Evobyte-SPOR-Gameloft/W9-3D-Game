using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieEnemy : MonoBehaviour
{
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float maxHealth;
    [SerializeField] private float minAttackDamage;
    [SerializeField] private float maxAttackDamage;

    [SerializeField] private float animationMultiplier = 0.2f;

    private readonly string walkAnimation = "isWalking";
    private readonly string deathAnimation = "isDead";
    private readonly string attackAnimation = "isAttacking";
    private readonly string runAnimation = "isChasing";

    private readonly string moveMultiplier = "moveMultiplier";


    [SerializeField] Animator animator;
    [SerializeField] Rigidbody rb;

    private bool isWalking = false;
    private bool isChasing = false;

    private bool isRotatingLeft = false;
    private bool isRotatingRight = false;

    private bool isStrolling = false;

    private bool isAttacking = false;

    private bool isDead = false;

    void Update()
    {
        if(isStrolling == false)
        {
            StartCoroutine(Strolling());
        }

        StrollingRotationLogic();

        ZombieAnimation();
    }

    private void StrollingRotationLogic()
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
    }
    private IEnumerator Strolling()
    {
        float walkWait = Random.Range(2.0f, 4.0f);
        float walkTime = Random.Range(2.5f, 5.0f);

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
}
