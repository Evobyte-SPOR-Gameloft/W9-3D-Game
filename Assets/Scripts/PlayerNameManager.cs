using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerNameManager : MonoBehaviour
{
    [SerializeField] TMP_InputField usernameInput;

    private void Start()
    {
        if (PlayerPrefs.HasKey("username"))
        {
            usernameInput.text = PlayerPrefs.GetString("username");
            PhotonNetwork.NickName = PlayerPrefs.GetString("username");
        }
        else
        {
            usernameInput.text = $"Guest{Random.Range(0,999):000}";
            OnUsernameInpuValueChanged();
        }
    }

    public void OnUsernameInpuValueChanged()
    {
        PhotonNetwork.NickName = usernameInput.text;
        PlayerPrefs.SetString("username", usernameInput.text);
    }
}
