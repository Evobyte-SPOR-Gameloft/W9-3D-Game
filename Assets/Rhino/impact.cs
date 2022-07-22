using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class impact : MonoBehaviour
{
    [SerializeField] float knockStr;
    
    private void Awake()
    {
        
    }

    private void Start()
    {
        
    }
    private void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody rb = collision.collider.GetComponent<Rigidbody>();
        if (rb != null && rb.tag =="PlayerController")
        {
            Debug.Log("hit");
            Vector3 direction = collision.transform.position - transform.position;
            direction.y = 0;
            rb.AddForce(direction.normalized * knockStr, ForceMode.Impulse);
        }
    }

    
}

