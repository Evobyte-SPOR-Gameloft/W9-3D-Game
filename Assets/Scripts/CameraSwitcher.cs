using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    // Start is called before the first frame update
    
    public GameObject FirstPersonCam;

    public GameObject ThirdPersonCam;
    public int CamMode;
    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("CamSwitch")){
            if(CamMode == 1){
                CamMode = 0;
            }
            else CamMode = 1;
        }
        StartCoroutine(CameraChange());
    }

    IEnumerator CameraChange(){
        yield return new WaitForSeconds(0.01f);
        if(CamMode == 0)
        {
            FirstPersonCam.SetActive(true);
            ThirdPersonCam.SetActive(false);
        }
        if(CamMode == 1)
        {
            FirstPersonCam.SetActive(false);
            ThirdPersonCam.SetActive(true);
        }
    }
}
