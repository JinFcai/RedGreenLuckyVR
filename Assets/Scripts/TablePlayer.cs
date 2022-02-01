using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.Collections;

public class TablePlayer : MonoBehaviourPunCallbacks
{
    [HideInInspector] public bool isActiveTable = false;
    [HideInInspector] public Player photonPlayer;

    [Header("Chip Visual")]
    [SerializeField] bool generateChipStack;
    [SerializeField] ChipStack chipStackObj;
    [SerializeField] int gridSize = 10;
    [SerializeField] Transform playerStackContainer;
    [SerializeField] Transform betStackContainer;
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

    /// <summary>
    /// Generate chipstack within the editor to avoid extra processing time during runtime
    /// </summary>
#if (UNITY_EDITOR)
    private void OnValidate()
    {
        if (chipStackObj == null || gridSize == 0 || !generateChipStack)
            return;

        GenerateChipStack(playerChipStackList, playerStackContainer);
        GenerateChipStack(betChipStackList, betStackContainer);
        generateChipStack = false;
    }
    public IEnumerator Destroy(GameObject go)
    {
        yield return new WaitForEndOfFrame();
        DestroyImmediate(go);
        yield return null;
    }
    public void GenerateChipStack(List<ChipStack> _chipStack, Transform _container)
    {
        _chipStack.Clear();
        foreach (Transform obj in _container)
            StartCoroutine(Destroy(obj.gameObject));

        float objSize = chipStackObj.transform.GetChild(0).transform.localScale.x;
        float startPt = objSize * (gridSize) / 2.0f - objSize / 2;
        Vector3 startPos = new Vector3(-startPt, 0, -startPt);
        float xIncrement = 0;
        float yIncrement = 0;
        for (int y = 0; y < gridSize; y++)
        {
            yIncrement = y * objSize;
            for (int x = 0; x < gridSize; x++)
            {
                xIncrement = x * objSize;
                ChipStack newStack = Instantiate(chipStackObj, Vector3.zero, Quaternion.identity);
                newStack.transform.parent = _container;
                newStack.transform.localPosition = new Vector3(startPos.x + xIncrement, 1, startPos.z + yIncrement);
                newStack.objRenderer.material = ChipMatarialGenerator.Instance.GetChipMaterial();
                _chipStack.Add(newStack);
            }
        }
    }
#endif
    private void Awake()
    {
        Renderer tableTopRenderer = tableTopObj.GetComponent<Renderer>();
        tableMaterial = tableTopRenderer.material = new Material(tableTopRenderer.material);
        tableMaterial.color = emptyColor;
    }
    public void ResetTableColour()
    {
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
            GameManager.Instance.myCardPlayer.SetBGColour(ConfirmColour);
            GameManager.Instance.myCardPlayer.SetPlayerName(player.NickName);
        }
        if (player != null)
            playerNameTxt.text = player.NickName;
        chipCountTxt.text = "Chip: " + GameManager.Instance.startChipCount;
        betCountTxt.text = "Bet: 0";
        ResetTableColour();
        ReorderChipStack(GameManager.Instance.startChipCount, playerChipStackList);
    }
    public void LeaveTable()
    {
        playerNameTxt.text = chipCountTxt.text = betCountTxt.text = "";
        isActiveTable = false;
        photonPlayer = null;
        //GameManager.Instance.myCardPlayer.tablePlayer = null;
        photonView.RPC(nameof(RPCClearAll), RpcTarget.All);
        tableMaterial.color = emptyColor;
    }

    [PunRPC]
    public void RPCBetChange(int _clipValue, int _betValue)
    {
        chipCountTxt.text = "Chip: " + _clipValue;
        betCountTxt.text = "Bet: " + _betValue;
        ReorderChipStack(_clipValue, playerChipStackList);
        ReorderChipStack(_betValue, betChipStackList);
    }
    [PunRPC]
    public void RPCClearBet(int _clipValue)
    {
        chipCountTxt.text = "Chip: " + _clipValue;
        betCountTxt.text = "Bet: 0";
        ReorderChipStack(_clipValue, playerChipStackList);
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

        for (int i = 0; i < playerChipStackList.Count; i++)
        {
            if (playerChipStackList[i] == null) {
                Debug.LogError("wtf: " + i);
            }
            playerChipStackList[i].objRenderer.gameObject.SetActive(false);
        }
    }
    /// <summary>
    /// Reorder chip stack visually
    /// </summary>
    /// <param name="_chipValue">player chip value</param>
    /// <param name="_chipStackList">players chip stack on the table</param>
    void ReorderChipStack(int _chipValue, List<ChipStack> _chipStackList)
    {
        int chipStackCount = _chipValue / 10;
        if (chipStackCount > _chipStackList.Count)
            return;
      //  Debug.LogError("ReorderChipStack: " + highestIndex);
        for (int i = 0; i < _chipStackList.Count; i++)
        {
            if (i < chipStackCount)
            {
                _chipStackList[i].objRenderer.gameObject.SetActive(true);
            }
            else
            {
                _chipStackList[i].objRenderer.gameObject.SetActive(false);
            }
        }
    }
}
