using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swim : MonoBehaviour
{
    [SerializeField] string name;
    [SerializeField] GameObject player;
    PlayerGroundCheck GCH;
    // Start is called before the first frame update
    void Start()
    {
        player = gameObject;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (player.transform.position.y < 5)
        {
            GameObject.FindGameObjectWithTag("GroundCheck").SetActive(false);
            player.GetComponent<Rigidbody>().useGravity = false;
        }
        else if (player.transform.position.y> 5)
        {
            GameObject.FindGameObjectWithTag("GroundCheck").SetActive(true);
            player.GetComponent<Rigidbody>().useGravity = true;

        }
        
    }

   /* private GameObject FindChild(GameObject topParentObj, string GameObjectName)
    {

        for(int i=0;i<topParentObj.transform.childCount;i++)
        {
            if(topParentObj.transform.GetChild(i).name==gameObject.name)
            {
                return topParentObj.transform.GetChild(i).gameObject;
            }
        }

        return null;
    }*/

    
        
   
   
}
