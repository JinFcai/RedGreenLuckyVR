using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class MenuManager : MonoBehaviourPunCallbacks
{
    public static MenuManager Instance;

    [Header("Menu Screens")]
    [SerializeField] GameObject loadingMenu;
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject createRoomMenu;
    [SerializeField] GameObject findRoomMenu;
    [SerializeField] GameObject roomMenu;

    GameObject currentActiveMenu;
    [SerializeField] TextMeshProUGUI failedConnectionTxt;

    [Header("Main")]
    [SerializeField] TMP_InputField playerNameInput;

    [Header("CreateRoom")]
    [SerializeField] TMP_InputField roomNameInput;

    [Header("FindRoom")]
    [SerializeField] Transform roomListContainer;
    [SerializeField] RoomListItem roomListItemPrefab;
    [SerializeField] List<RoomListItem> roomItemList;

    [Header("LobbyMenu")]
    [SerializeField] Button startGameButton;
    [SerializeField] Transform playerListContainer;
    [SerializeField] PlayerListItem PlayerListItemPrefab;
    [SerializeField] TextMeshProUGUI RoomNameTxt;

    public int playerRoomSize = 1;

    string playerName;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
        {
            Instance = this;
          // DontDestroyOnLoad(gameObject);
        }
    }
    private void Start()
    {
        InitMenuScenes();
        PhotonNetwork.ConnectUsingSettings();
    }
    #region menu buttons & input
    public void InputCreateRoom()
    {
        if (string.IsNullOrEmpty(roomNameInput.text))
            return;
        
        if (playerNameInput.text != "")
            PhotonNetwork.NickName = playerNameInput.text;

        RoomOptions roomOption = new RoomOptions();
        roomOption.MaxPlayers = (byte)playerRoomSize;
        PhotonNetwork.CreateRoom(roomNameInput.text, roomOption);
        SetActiveMenu(loadingMenu);
    }
    public void InputJoinRoom(RoomInfo info)
    {
        //if (PhotonNetwork.IsConnected && PhotonNetwork.CountOfPlayersInRooms >= playerRoomSize)
        //{
        //    // UpdateRoomPlayerCount();
        ////    Debug.Log("FullRoom");
        //    return;
        //}

        if (playerNameInput.text != "")
            PhotonNetwork.NickName = playerNameInput.text;

       if( PhotonNetwork.JoinRoom(info.Name))
        SetActiveMenu(loadingMenu);
    }

    public void InputLeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        SetActiveMenu(loadingMenu);
    }
    public void InputStartGame()
    {
        PhotonNetwork.LoadLevel(1);
    }
    #endregion


    //#region Buttons & Inputs
    //public void OnCreateLobbyButton(TMP_InputField lobbyName) {
    //    NetworkManager.instance.CreateRoom(lobbyName.text);
    //}

    //public void OnJoinLobbyButton(TMP_InputField lobbyName) {
    //    PhotonNetwork.JoinRoom(info.Name);
    //    NetworkManager.instance.JoinRoom(lobbyName.text);
    //}

    //public void OnPlayerNameInputUpdate(TMP_InputField _playerName)
    //{

    //    playerName = _playerName.text;
    //    PhotonNetwork.NickName = playerName;
    //}

    //public void OnLeaveLobbyButton() {
    //    PhotonNetwork.LeaveRoom();
    //    SetMenuScreen(mainMenu);
    //}
    //public void OnStartGameButton() {
       
    //}
    //#endregion

    #region Photon
    public override void OnConnectedToMaster()
    {
       PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;

    }
    public override void OnJoinedLobby()
    {

        SetActiveMenu(mainMenu);
        Debug.Log("Joined Lobby");
        PhotonNetwork.NickName = "Player " + Random.Range(0, 100).ToString();
    }
    public override void OnJoinedRoom()
    {
        //UpdateRoomPlayerCount();

        Debug.Log("Joined room");
        SetActiveMenu(roomMenu);
        RoomNameTxt.text = PhotonNetwork.CurrentRoom.Name;
        Player[] players = PhotonNetwork.PlayerList;
        foreach (Transform child in playerListContainer)
        {
            Destroy(child.gameObject);
        }
        for (int i = 0; i < players.Length; i++)
        {
            Instantiate(PlayerListItemPrefab, playerListContainer).SetUp(players[i]);
        }
        startGameButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        PlayerListItem newPlayerItem = Instantiate(PlayerListItemPrefab, playerListContainer);
        newPlayerItem.SetUp(newPlayer);
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (Transform trans in roomListContainer)
        {
            Destroy(trans.gameObject);
        }
        Debug.Log("roomList: " + roomList.Count);
        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].RemovedFromList)
                continue;
            RoomListItem newRoom = Instantiate(roomListItemPrefab, roomListContainer);
            newRoom.SetUp(roomList[i]);
            roomItemList.Add(newRoom);
        }
        UpdateRoomPlayerCount();
    }
    public override void OnLeftRoom()
    {
        SetActiveMenu(mainMenu);
    }
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startGameButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
    }
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)//called automaticlly when a player leaves room
    {
        //UpdateLobbyText();
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        StartCoroutine(FailedConnectError("Failed To Create Lobby"));
        SetActiveMenu(mainMenu);
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        StartCoroutine(FailedConnectError("Failed To Join Lobby"));
        SetActiveMenu(mainMenu);
    }

    IEnumerator FailedConnectError(string errorMsg) {
        failedConnectionTxt.text = errorMsg;
        failedConnectionTxt.gameObject.SetActive(true);
        yield return new WaitForSeconds(3);
        failedConnectionTxt.gameObject.SetActive(false);
    }

    #endregion

    //[PunRPC] // update lobby for all when player enter and leaves lobby
    //public void UpdateLobbyText() {

    //    playerNameList.text = "";
    //    Debug.Log("UpdateLobbyText called: " + PhotonNetwork.PlayerList.Length);
    //    foreach (Photon.Realtime.Player p in PhotonNetwork.PlayerList) {
    //        Debug.Log("UpdateLobbyText name:" + p.NickName);

    //       playerNameList.text += p.NickName + "\n";
    //    }
    //    startGameButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
    //}

    public void SetActiveMenu(GameObject newActiveMenu) {
        if (currentActiveMenu)
            currentActiveMenu.gameObject.SetActive(false);

        newActiveMenu.gameObject.SetActive(true);
        currentActiveMenu = newActiveMenu;
    }

    public void InitMenuScenes() {
        loadingMenu.SetActive(true);
        currentActiveMenu = loadingMenu;

        mainMenu.SetActive(false);
        createRoomMenu.SetActive(false);
        findRoomMenu.SetActive(false);
        roomMenu.SetActive(false);
    }

    public void UpdateRoomPlayerCount() {
        foreach (Transform child in roomListContainer.transform) {
            RoomListItem roomItem = new RoomListItem();
            if ((roomItem = child.GetComponent<RoomListItem>())!= null)
                roomItem.UpdateServerPlayerCount();
        }
    }
}
