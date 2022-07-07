using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyedPieceController : MonoBehaviour
{
    [HideInInspector] public bool IsConnected = true;
    [HideInInspector] public bool WasVisited = false;
    public List<DestroyedPieceController> listOfConnections;

    public static bool IsDirty = false;

    private Rigidbody rb;
    private Vector3 startingPosition;
    private Quaternion startingOrientation;
    private Vector3 startingScale;

    private bool IsConfigured = false;

    void Start()
    {
        listOfConnections = new List<DestroyedPieceController>();
        startingPosition = transform.position;
        startingOrientation = transform.rotation;
        startingScale = transform.localScale;

        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
       
        if (!IsConfigured)
        {
            var neighbour = collision.gameObject.GetComponent<DestroyedPieceController>();
            if (neighbour)
            {
                if(!listOfConnections.Contains(neighbour))
                    listOfConnections.Add(neighbour);
            }
        }
        else if (collision.gameObject.CompareTag("Floor"))
        {
            //Destroy(gameObject, 10f);
        }
    }

    public void MakeStatic()
    {
        IsConfigured = true;
        rb.isKinematic = true;
        rb.useGravity = true;

        transform.localScale = startingScale;
        transform.position = startingPosition;
        transform.rotation = startingOrientation;
    }

    public void CauseDamage(Vector3 force)
    {
        IsConnected = false;
        rb.isKinematic = false;
        IsDirty = true;
        rb.AddForce(force, ForceMode.Impulse);
    }

    public void CauseDamageByGravityGun()
    {
        IsConnected = false;
        IsDirty = true;
    }

    public void DropPiece()
    {
        IsConnected = false;
        if(rb == SpecialityGun.grabbedRB)
        {
            return;
        }
        else
        {
            rb.isKinematic = false;
        }
    }
}
