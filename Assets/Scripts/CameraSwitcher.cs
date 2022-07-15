using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class CameraSwitcher : MonoBehaviour
{
    // Start is called before the first frame update
    
    public GameObject FirstPersonCam;

    public GameObject ThirdPersonCam;
    public int CamMode;

    PhotonView PV;

    [SerializeField] private GameObject playerModel;

    private RawImage crosshair;
    
    void Awake()
    {
        PV = GetComponent<PhotonView>();
        crosshair = GameObject.FindWithTag("Crosshair").GetComponent<RawImage>();

    }

    void Update()
    {
        if(!PV.IsMine)
            return;

        if(Input.GetButtonDown("CamSwitch")){
            if(CamMode == 1){
                CamMode = 0;
            }
            else CamMode = 1;

            StartCoroutine(CameraChange());
        }

    }

    IEnumerator CameraChange(){
        yield return new WaitForSeconds(0.01f);

        if (FirstPersonCam != null && ThirdPersonCam != null)
        {
            if (CamMode == 0)
            {
                FirstPersonCam.SetActive(true);
                ThirdPersonCam.SetActive(false);
                //playerModel.SetActive(false);
                crosshair.enabled = true;
            }
            if (CamMode == 1)
            {
                FirstPersonCam.SetActive(false);
                ThirdPersonCam.SetActive(true);
                //playerModel.SetActive(true);
                crosshair.enabled = false;
            }
        }
    }
}
