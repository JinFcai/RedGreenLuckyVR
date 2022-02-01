using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NetworkManager : MonoBehaviourPunCallbacks
{

    public static NetworkManager instance;

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(gameObject);
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    private void Start()
    {
    
    }
    //public void CreateRoom(string roomName) {
    //    PhotonNetwork.CreateRoom(roomName);
    //}
    //public void JoinRoom(string roomName) {
    //    PhotonNetwork.JoinRoom(roomName);
    //}

    //[PunRPC] // all player call changeScene when game starts
    //public void ChangeScene(string sceneName) {
    //    PhotonNetwork.LoadLevel(sceneName);
    //}

    ////public override void OnConnectedToMaster()
    ////{
    ////    Debug.Log("connected to server");
    ////    CreateRoom("testRoom");
    ////}
    //public override void OnCreatedRoom()
    //{
    //    base.OnCreatedRoom();
    //    Debug.Log("server create room: " + PhotonNetwork.CurrentRoom.Name);
    //}

    //public override void OnJoinedRoom()
    //{
    //    base.OnJoinedRoom();
    //}
}
