using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance;
    public Image cardIMG;
    [SerializeField] TextMeshProUGUI resultTxt;
    public UnityEvent OnGameReset;


    [Header("dealer")]
    public int dealerDrawCount;
    public Dealer dealer;

    [Header("Player")]
    public int startChipCount = 100;
    public int betIncrement = 10;
    public int playerDrawCount = 3;
    public CardPlayer myCardPlayer;
    public List<TablePlayer> tableList;
    int playersRdy = 0;

    List<Player> joinedPlayer = new List<Player>();
    List<Player> newJoinedPlayer = new List<Player>();

    bool isGameStarted = false;
    bool isMatchStarted = false;


    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
        dealer.Init();

        myCardPlayer.Init();
    }
    private void Start()
    {
        photonView.RPC(nameof(RPCPlayerJoinedGame), RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer);
    }
    /// <summary>
    /// When player leaves the room, remove player from list, reset the table, and check if bets are ready.
    /// </summary>
    /// <param name="player"></param> photon player
    public override void OnPlayerLeftRoom(Player player)
    {
        base.OnPlayerLeftRoom(player);
        // Debug.Log("player " + player.ActorNumber + " left the room");
        if (joinedPlayer.Contains(player))
        {
            joinedPlayer.Remove(player);
            for (int i = 0; i < tableList.Count; i++)
            {
                if (tableList[i].photonPlayer == player)
                {
                    tableList[i].LeaveTable();
                }
            }
            if (PhotonNetwork.IsMasterClient)
            {
                RPCCheckAllBetsIn(false);
            }
        }
    }
    /// <summary>
    /// When player joins a room, if game already started, put the player in queue  for next match. If not wait for host to setup all joined players
    /// </summary>
    /// <param name="newPlayer"></param>
    [PunRPC]
    void RPCPlayerJoinedGame(Player newPlayer)
    {
        if (!isGameStarted)
        {
            joinedPlayer.Add(newPlayer);
            //Debug.Log("add Player | " + newPlayer.ActorNumber);
            if (joinedPlayer.Count == PhotonNetwork.PlayerList.Length) // When all players join the room
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    HostSetupPlayers();
                    photonView.RPC(nameof(RPCGameReset), RpcTarget.All);
                }
            }
        }
        else if (PhotonNetwork.IsMasterClient)
        {
            newJoinedPlayer.Add(newPlayer);
            foreach (var table in tableList)
            {
                if (table.isActiveTable)
                {
                    table.photonView.RPC(nameof(TablePlayer.RPCJoinTable), newPlayer, table.photonPlayer);
                }
            }
        }
    }
    /// <summary>
    /// host setup table for all players
    /// </summary>
    void HostSetupPlayers()
    {
        for (int i = 0; i < joinedPlayer.Count; i++)
        {
            for (int j = 0; j < tableList.Count; j++)
            {
                if (!tableList[j].isActiveTable)
                {
                    // Debug.Log("" + "Assign " + joinedPlayer[i].ActorNumber + " to " + tableList[j].gameObject.name);
                    tableList[j].photonView.RPC(nameof(TablePlayer.RPCJoinTable), RpcTarget.AllBuffered, joinedPlayer[i]);
                    break;
                }
            }
        }
    }
    /// <summary>
    /// Host setup up table for specific player
    /// </summary>
    /// <param name="p"></param>
    void HostSetupPlayers(Player p)
    {
        for (int j = 0; j < tableList.Count; j++)
        {
            if (!tableList[j].isActiveTable)
            {
                // Debug.log("Assign " + p.ActorNumber + " to " + tableList[j].gameObject.name);
                tableList[j].photonView.RPC(nameof(TablePlayer.RPCJoinTable), RpcTarget.AllBuffered, p);
                break;
            }
        }
    }

    /// <summary>
    /// Check if all player bets are in, move to next betting round, or reset the game
    /// </summary>
    /// <param name="addPlayerRdy"></param>
    [PunRPC]
    public void RPCCheckAllBetsIn(bool addPlayerRdy)
    {
        if (addPlayerRdy)
            playersRdy++;
        if (playersRdy >= joinedPlayer.Count)
        {
            photonView.RPC(nameof(RPCCheckBetResultPhase), RpcTarget.All);
            playersRdy = 0;
        }
        //CheckBetResultPhase();
    }
    /// <summary>
    /// Check win lose condition, reset the game in 3 seconds after check
    /// </summary>
    [PunRPC]
    void RPCCheckBetResultPhase()
    {
        if (myCardPlayer.tablePlayer == null)
            return;
        dealer.RevealAllCards();
        if (myCardPlayer.CheckWinLose())
            resultTxt.text = "You Win";
        else
            resultTxt.text = "You Lose!";

        isMatchStarted = false;
        Invoke(nameof(ClientGameReset), 3);
    }
    void ClientGameReset()
    {
        if (PhotonNetwork.IsMasterClient)
            photonView.RPC(nameof(RPCGameReset), RpcTarget.All);
    }

    /// <summary>
    /// All clients reset their game. Host client add new joined player if they exist.
    /// </summary>
    [PunRPC]
    void RPCGameReset()
    {
        //isGameStarted = true;
        if (PhotonNetwork.IsMasterClient)
        {
            if (newJoinedPlayer.Count > 0)
            {
                foreach (Player p in newJoinedPlayer)
                {
                    HostSetupPlayers(p);
                    joinedPlayer.Add(p);
                }
                newJoinedPlayer.Clear();
            }
        }
        foreach (TablePlayer t in tableList)
        {
            if (t.isActiveTable)
                t.ResetTableColour();
        }
        isGameStarted = true;
        isMatchStarted = true;
        OnGameReset.Invoke();
        resultTxt.text = "Place your bets!";
    }
}
