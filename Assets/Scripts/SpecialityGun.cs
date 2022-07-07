using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialityGun : Gun
{
    [SerializeField] private float maxGrabbingDistance = 10f, pushForce = 10f, lerpSpeed = 10f;

    [SerializeField] Camera cam;

    [SerializeField] Transform ObjectHolder;

    [HideInInspector] public static Rigidbody grabbedRB;

    PhotonView PV;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    public override void UsePrimary()
    {
        Push();
    }
    public override void UseSecondary()
    {
        PickUp();
    }

    private void Update()
    {
        if (grabbedRB)
        {
            grabbedRB.MovePosition(Vector3.Lerp(grabbedRB.position, ObjectHolder.transform.position, Time.deltaTime * lerpSpeed));
        }
    }

    private void PickUp()
    {
        if (grabbedRB)
        {
            grabbedRB.isKinematic = false;
            grabbedRB = null;
        }
        else
        {
            RaycastHit hit;
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
            if (Physics.Raycast(ray, out hit, maxGrabbingDistance))
            {
                grabbedRB = hit.collider.gameObject.GetComponent<Rigidbody>();
                if (grabbedRB)
                {
                    grabbedRB.isKinematic = true;
                }
            }
        }
    }

    private void Push()
    {
        if (grabbedRB)
        {
            grabbedRB.isKinematic = false;
            grabbedRB.AddForce(cam.transform.forward * pushForce, ForceMode.VelocityChange);
            grabbedRB = null;
        }
    }
}