using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialityGun : Gun
{
    [SerializeField] private float maxGrabbingDistance = 7f, pushForce = 60f, lerpSpeed = 100f;

    [SerializeField] Camera cam;

    [SerializeField] Transform objectHolder;

    [SerializeField] Transform rayOrigin;

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
        if (!PV.IsMine)
            return;

        if (grabbedRB != null)
        {
            grabbedRB.MovePosition(Vector3.Lerp(grabbedRB.position, objectHolder.transform.position, Time.deltaTime * lerpSpeed));
        }
    }

    private void PickUp()
    {
        if (grabbedRB != null)
        {
            grabbedRB.isKinematic = false;
            grabbedRB = null;

            if(grabbedRB != null && grabbedRB.GetComponent<PlayerController>() != null)
            {
                grabbedRB.gameObject.GetComponent<PlayerController>().isPickedUp = false;
            }
        }
        else
        {
            RaycastHit hit;
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
            ray.origin = rayOrigin.transform.position;

            if (Physics.Raycast(ray, out hit, maxGrabbingDistance))
            {

                if (hit.collider.gameObject.CompareTag("PlayerController"))
                {
                    if (hit.collider.gameObject.GetComponent<PhotonView>().IsMine)
                        return;

                    hit.collider.gameObject.GetComponent<PlayerController>().isPickedUp = true;
                    grabbedRB = hit.collider.gameObject.GetComponent<Rigidbody>();
                }
                else if (Physics.Raycast(ray, out hit, maxGrabbingDistance, 1 << LayerMask.NameToLayer("Destructible"), QueryTriggerInteraction.Ignore))
                {
                    //hit.collider.GetComponent<PhotonView>().RequestOwnership();

                    grabbedRB = hit.collider.gameObject.GetComponent<Rigidbody>();

                    hit.collider.GetComponent<DestroyedPieceController>().CauseDamageByGravityGun();
                }

                if (grabbedRB != null)
                {
                    grabbedRB.isKinematic = true;
                }
            }
        }
    }

    private void Push()
    {
        if (grabbedRB != null)
        {
            grabbedRB.isKinematic = false;
            grabbedRB.AddForce(cam.transform.forward * pushForce, ForceMode.VelocityChange);
            grabbedRB = null;
        }
    }
}
