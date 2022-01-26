using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class TestConnect : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.GameVersion = "0.0.1";
        PhotonNetwork.ConnectUsingSettings();
        
    }

    public override void OnConnectedToMaster() {
        Debug.Log("connected to server");
    
    }
    public override void OnDisconnected(DisconnectCause cause)
    {

        Debug.Log("disconnected from server: " +  cause.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
