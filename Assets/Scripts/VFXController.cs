using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXController : MonoBehaviour
{
    public GameObject dispersion;
    public static VFXController Instance
    {
        get
        {
            if (!_instance)
                _instance = FindObjectOfType<VFXController>();
            if (!_instance)
                Debug.LogError("No VFXController in scene");

            return _instance;
        }
    }
    private static VFXController _instance;
    
    
    public void spawn_dispersion(Vector3 position)
    {
        Instantiate(dispersion, position, Quaternion.identity);
    }
}