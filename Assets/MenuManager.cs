using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class MenuManager : MonoBehaviourPunCallbacks
{
    [Header("Menu Screens")]
    [SerializeField] GameObject mainScreen;
    [SerializeField] GameObject lobbyScreen;
    [Header("Mean")]
    [SerializeField] Button createLobbyBotton;
    [SerializeField] Button joinLobbyButton;
    [Header("Lobby")]
    [SerializeField] Button leaveLobbyButton;
    [SerializeField] Button startGameButton;
    [SerializeField] TextMeshProUGUI playerNameList;

    string playerName;

    private void Start()
    {
        createLobbyBotton.interactable = false;
        joinLobbyButton.interactable = false;
        SetMenuScreen(mainScreen);
    }
    #region Buttons & Inputs
    public void OnCreateLobbyButton(TMP_InputField lobbyName) {
        NetworkManager.instance.CreateRoom(lobbyName.text);
    }

    public void OnJoinLobbyButton(TMP_InputField lobbyName) {
        NetworkManager.instance.JoinRoom(lobbyName.text);
    }

    public void OnPlayerNameInputUpdate(TMP_InputField _playerName)
    {
       
        playerName = _playerName.text;
        PhotonNetwork.NickName = playerName;
    }

    public void OnLeaveLobbyButton() {
        PhotonNetwork.LeaveRoom();
        SetMenuScreen(mainScreen);
    }
    public void OnStartGameButton() {
        NetworkManager.instance.photonView.RPC("ChangeScene", RpcTarget.All, "Game");
    }
    #endregion

    #region Photon
    public override void OnConnectedToMaster()
    {
        // prevent buttons being pressed before connecting to server
        createLobbyBotton.interactable = true;
        joinLobbyButton.interactable = true;
    }
    public override void OnJoinedRoom()// only called for client who just joined
    {
        SetMenuScreen(lobbyScreen);

        photonView.RPC(nameof(UpdateLobbyText), RpcTarget.All);
    }
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)//called automaticlly when a player leaves room
    {
        UpdateLobbyText();
    }
    #endregion

    [PunRPC] // update lobby for all when player enter and leaves lobby
    public void UpdateLobbyText() {

        playerNameList.text = "";
        Debug.Log("UpdateLobbyText called: " + PhotonNetwork.PlayerList.Length);
        foreach (Photon.Realtime.Player p in PhotonNetwork.PlayerList) {
            Debug.Log("UpdateLobbyText name:" + p.NickName);

           playerNameList.text += p.NickName + "\n";
        }
        startGameButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
    }

    void SetMenuScreen(GameObject activeScreen) {
        mainScreen.gameObject.SetActive(false);
        lobbyScreen.gameObject.SetActive(false);
        activeScreen.SetActive(true);


    
    }

}
