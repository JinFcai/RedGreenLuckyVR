using UnityEngine;
using System.Collections.Generic;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;


public class RoomListItem : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_Text roomName;
    [SerializeField] TMP_Text roomCount;
    [SerializeField] Button joinButton;
    public RoomInfo info;

    public void SetUp(RoomInfo _info)
    {
        info = _info;
        roomName.text = _info.Name;
    }
    public void OnClick()
    {
        MenuManager.Instance.InputJoinRoom(info);
    }

    public void UpdateServerPlayerCount() {
        if (info == null)
            return;
        
            roomCount.text = info.PlayerCount.ToString() + " | " + info.MaxPlayers;
            if (info.PlayerCount >= info.MaxPlayers)
                joinButton.interactable = false;
        

    }
}
