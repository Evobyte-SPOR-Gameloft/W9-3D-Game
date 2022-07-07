using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableObjectController : MonoBehaviour
{
    public GameObject root;
    [HideInInspector] public DestroyedPieceController destroyableRootPiece;

    private List<DestroyedPieceController> listOfDestroyablePieces = new List<DestroyedPieceController>();
    
    private void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            var _dpc = child.gameObject.AddComponent<DestroyedPieceController>();

            var _rigidbody = child.gameObject.AddComponent<Rigidbody>();
            _rigidbody.isKinematic = false;
            _rigidbody.useGravity = false;

            var _mc = child.gameObject.AddComponent<MeshCollider>();
            _mc.convex = true;
            listOfDestroyablePieces.Add(_dpc);
        }
        destroyableRootPiece = root.GetComponent<DestroyedPieceController>();
        StartCoroutine(RunPhysicsSteps(10));
    }

    private void Update()
    {
        
        if(DestroyedPieceController.IsDirty)
        {

            foreach (var destroyedPiece in listOfDestroyablePieces)
            {
                destroyedPiece.WasVisited = false;
            }


            //Breadth first search to find all connected pieces
            findAllConnectedPieces(destroyableRootPiece);

            // Drop all pieces not reachable from root
            foreach (var piece in listOfDestroyablePieces)
            {
                if (piece && !piece.WasVisited)
                {
                    piece.DropPiece();
                }
            }
        }
    }

    private void findAllConnectedPieces(DestroyedPieceController destroyedPiece)
    {
        if (!destroyedPiece.WasVisited)
        {
            if (!destroyedPiece.IsConnected)
                return;
            destroyedPiece.WasVisited = true;

            foreach (var connection in destroyedPiece.listOfConnections)
            {
                findAllConnectedPieces(connection);
            }
        }
        else
            return;
    }

    private IEnumerator RunPhysicsSteps(int stepsCount)
    {
        for (int i = 0; i < stepsCount; i++)
            yield return new WaitForFixedUpdate();
        
        foreach( var piece in listOfDestroyablePieces)
        {
            piece.MakeStatic();
        }
    }
}
