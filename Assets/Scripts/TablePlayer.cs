using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using TMPro;


public class TablePlayer : MonoBehaviourPunCallbacks
{
    [HideInInspector] public bool isActiveTable = false;
    [HideInInspector] public int[] playerChipCount;
    [HideInInspector] public int[] betChipCount;
    [HideInInspector] public Player photonPlayer;

    [Header("Chip Visual")]
    //public List<Chip> betChipList = new List<Chip>();
    [SerializeField] List<ChipStack> playerChipStackList = new List<ChipStack>();
    [SerializeField] List<ChipStack> betChipStackList = new List<ChipStack>();



    [Header("Table Visual")]
    [SerializeField] TextMeshProUGUI playerNameTxt;
    [SerializeField] TextMeshProUGUI chipCountTxt;
    [SerializeField] TextMeshProUGUI betCountTxt;
    [SerializeField] GameObject tableTopObj;
    [SerializeField] Color ConfirmColour;
    [SerializeField] Color defaultColour;
    [SerializeField] Color emptyColor;
    Material tableMaterial;


    //   [Header("Photon")]
    // public Player photonPlayer;

    private void Awake()
    {
        RPCClearAll();
        Renderer tableTopRenderer = tableTopObj.GetComponent<Renderer>();
        tableMaterial = tableTopRenderer.material = new Material(tableTopRenderer.material);
        tableMaterial.color = emptyColor;
    }

    public void ResetTableColour() {
        tableMaterial.color = defaultColour;
    }

    [PunRPC]
    public void RPCConfirmTableColour()
    {
        tableMaterial.color = ConfirmColour;
    }
    [PunRPC]
    public void RPCResetTableColour()
    {
        ResetTableColour();
    }
    [PunRPC]
    public void RPCJoinTable(Player player)
    {
        if (player == null)
            return;

        // Debug.Log("JoinTable : " + player.ActorNumber + " " + gameObject.name);
        photonPlayer = player;
        isActiveTable = true;
        if (player == PhotonNetwork.LocalPlayer)
        {
            GameManager.Instance.myCardPlayer.tablePlayer = this;
            GameManager.Instance.myCardPlayer.setBGColour(ConfirmColour);
            GameManager.Instance.myCardPlayer.SetPlayerName(player.NickName);
        }
        if (player == null)
 
          playerNameTxt.text = player.NickName;
          chipCountTxt.text = "Chip: " + GameManager.Instance.startChipCount;
          betCountTxt.text = "Bet: 0";
        ResetTableColour();
        playerChipCount = new int[GameManager.Instance.chipList.Count];
        betChipCount = new int[GameManager.Instance.chipList.Count];
        UpdateChipAmountAndStack(playerChipCount, playerChipStackList, GameManager.Instance.startChipCount);
        GameManager.Instance.OnBetRoundEnd.AddListener(BetRoundEnd);
    }

    public void LeaveTable()
    {
        playerNameTxt.text = chipCountTxt.text = betCountTxt.text = "";
        isActiveTable = false;
        photonPlayer = null;
        //GameManager.Instance.myCardPlayer.tablePlayer = null;
        photonView.RPC(nameof(RPCClearAll), RpcTarget.All);
        GameManager.Instance.OnBetRoundEnd.RemoveListener(BetRoundEnd);
        tableMaterial.color = emptyColor;
    }

    /// <summary>
    /// Calulated chip amount based on different chip values
    /// </summary>
    /// <param name="_chipCount">Array to keep track of each individual chip count</param>
    /// <param name="_totalChipValue">Total sum of all chip value</param>
    public void CacluateClipAmount(int[] _chipCount, int _totalChipValue)
    {
        int chipValue = _totalChipValue;
        for (int i = 0; i < GameManager.Instance.chipList.Count; i++)
        {
            bool highestCoinReached = false;
            _chipCount[i] = 0;
            // total value greater than 9 of current coin and not last index
            if (chipValue - (GameManager.Instance.chipList[i].chipValue * 9) > 0 && i < GameManager.Instance.chipList.Count - 1)
            {
                chipValue -= GameManager.Instance.chipList[i].chipValue * 9;
                _chipCount[i] += 9;
            }
            else if (chipValue / GameManager.Instance.chipList[i].chipValue > 0)
            {
                int chipIncrement = chipValue / GameManager.Instance.chipList[i].chipValue;
                chipValue -= chipIncrement * GameManager.Instance.chipList[i].chipValue;
                _chipCount[i] += chipIncrement;
                highestCoinReached = true;
            }
            else
            {
                highestCoinReached = true;
            }
            if (highestCoinReached && chipValue > 0)
            {
                for (int j = i - 1; j > -1; j--)
                {
                    if (chipValue > 0 && chipValue >= GameManager.Instance.chipList[j].chipValue)
                    {
                        int chipIncrement = chipValue / GameManager.Instance.chipList[j].chipValue;
                        _chipCount[j] += chipIncrement;
                        chipValue -= chipIncrement * GameManager.Instance.chipList[j].chipValue;
                    }
                    if (chipValue < 0)
                        break;
                }
            }
            if (chipValue < 0)
                break;
        }
    }
    /// <summary>
    /// Reorder the chip stacks visual by colour and value
    /// </summary>
    /// <param name="_chipCount">Array to keep track of each individual chip count</param>
    /// <param name="_chipStackList">List of chipStack Obj that belongs to the table</param>
    void ReorderChipStack(int[] _chipCount, List<ChipStack> _chipStackList)
    {
        int coinStackIndex = 0;
        for (int i = 0; i < GameManager.Instance.chipList.Count; i++)
        {
            _chipStackList[coinStackIndex].objRenderer.gameObject.SetActive(false);
            int chipStackSize = _chipCount[i];

            float stackSize = 1;
            while (chipStackSize >= 1)
            {
                if (chipStackSize < 10)
                    stackSize = chipStackSize / 10.0f;
                if (stackSize > 0)
                {
                    _chipStackList[coinStackIndex].objRenderer.gameObject.SetActive(true);
                }
                chipStackSize -= 10;
                _chipStackList[coinStackIndex].transform.localScale = new Vector3(1, stackSize, 1);
                _chipStackList[coinStackIndex].SetMaterial(GameManager.Instance.chipList[i].chipMaterial);

                chipStackSize -= 10;
                coinStackIndex++;
            }
        }
    }
    public void Bet(int _clipValue, int _betValue)
    {
        int dir = (int)Mathf.Sign(_betValue);
        playerChipCount[0] -= 1 * dir;
        betChipCount[0] += 1 * dir;
        photonView.RPC(nameof(RPCBetChange), RpcTarget.All, _clipValue, _betValue);
    }

    void UpdateChipAmountAndStack(int[] _chipCount, List<ChipStack> _chipStackList, int _chipValue)
    {
        CacluateClipAmount(_chipCount, _chipValue);
        ReorderChipStack(_chipCount, _chipStackList);
    }

    [PunRPC]
    public void RPCBetChange(int _clipValue, int _betValue)
    {
        chipCountTxt.text = "Chip: " + _clipValue;
        betCountTxt.text = "Bet: " + _betValue;
        UpdateChipAmountAndStack(playerChipCount, playerChipStackList, _clipValue);
        UpdateChipAmountAndStack(betChipCount, betChipStackList, _betValue);
    }
    [PunRPC]
    public void RPCClearBet(int _clipValue)
    {
        chipCountTxt.text = "Chip: " + _clipValue;
        betCountTxt.text = "Bet: 0";
        UpdateChipAmountAndStack(playerChipCount, playerChipStackList, _clipValue);
        foreach (ChipStack stack in betChipStackList)
            stack.objRenderer.gameObject.SetActive(false);
    }
    [PunRPC]
    public void RPCClearAll()
    {
        foreach (ChipStack stack in betChipStackList)
        {
            stack.objRenderer.gameObject.SetActive(false);
        }
        foreach (ChipStack stack in playerChipStackList)
        {
            stack.objRenderer.gameObject.SetActive(false);
        }
    }
    public void ClearBet(int _clipValue)
    {
        for (int i = 0; i < betChipCount.Length; i++)
            betChipCount[i] = 0;
        photonView.RPC(nameof(RPCClearBet), RpcTarget.All, _clipValue);
    }
    public void BetRoundEnd()
    {
        photonView.RPC(nameof(RPCResetTableColour), RpcTarget.All);
    }
}
