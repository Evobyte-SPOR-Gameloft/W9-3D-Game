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

        if (grabbedRB)
        {
            //grabbedRB.MovePosition(Vector3.Lerp(grabbedRB.position, objectHolder.transform.position, Time.deltaTime * lerpSpeed));

            PV.RPC(nameof(RPC_Movement), RpcTarget.All);

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
            ray.origin = rayOrigin.transform.position;

            if (Physics.Raycast(ray, out hit, maxGrabbingDistance))
            {

                if (hit.collider.gameObject.CompareTag("PlayerController"))
                {
                    if (hit.collider.gameObject.GetComponent<PhotonView>().IsMine)
                        return;

                    //Debug.Log(hit.collider.gameObject.GetComponent<Rigidbody>());

                    hit.collider.gameObject.GetComponent<PlayerController>().isPickedUp = true;
                    grabbedRB = hit.collider.gameObject.GetComponent<Rigidbody>();
                }
                else if (Physics.Raycast(ray, out hit, maxGrabbingDistance, 1 << LayerMask.NameToLayer("Destructible"), QueryTriggerInteraction.Ignore))
                {
                    hit.collider.GetComponent<PhotonView>().RequestOwnership();

                    grabbedRB = hit.collider.gameObject.GetComponent<Rigidbody>();

                    hit.collider.GetComponent<DestroyedPieceController>().CauseDamageByGravityGun();
                }
                else if (hit.collider != null)
                {
                    hit.collider.GetComponent<PhotonView>().RequestOwnership();

                    grabbedRB = hit.collider.gameObject.GetComponent<Rigidbody>();
                }

                //grabbedRB = hit.collider.gameObject.GetComponent<Rigidbody>();

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

    [PunRPC]
    private void RPC_Movement()
    {
        if(grabbedRB != null)
        {
            grabbedRB.MovePosition(Vector3.Lerp(grabbedRB.position, objectHolder.transform.position, Time.deltaTime * lerpSpeed));
        }
    }
}
