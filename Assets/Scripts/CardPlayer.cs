using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;
using System;
using Photon.Pun;
using Photon.Realtime;


public class CardPlayer : CardHandler
{
    [HideInInspector] public int id;
    Player photonPlayer;

    public string playerName;
    public UnityEvent BetCompleteEvent;
    int chipValue;

    [HideInInspector] public bool isFold = false;
    int roundBetAmount;
    int totalBetAmount;
    bool isButtonsActive = false;
    [HideInInspector] public TablePlayer tablePlayer;

    #region UI
    [Space(10)]
    [Header("UI Elements")]
    [SerializeField] Image bgImg;
    [SerializeField] TextMeshProUGUI playerNameTxt;
    [SerializeField] TextMeshProUGUI chipValueTxt;
    [SerializeField] TextMeshProUGUI betValueTxt;
    [SerializeField] Button subButton;
    [SerializeField] Button addButton;
    [SerializeField] Button confirmButton;
    [SerializeField] Button foldButton;
    [SerializeField] Transform cardHolderTrans;


    #endregion
    // Start is called before the first frame update

    protected override void Awake()
    {
        ToggleButtons(false);
    }
    public override void Init()
    {
        base.Init();
        DrawCardsSetup(GameManager.Instance.playerDrawCount);
    }

    protected override void Start()
    {
        base.Start();

        ResetChips();
        //  tablePlayer.Setup(coinCount);

    }
    private void ResetChips()
    {
        chipValue = GameManager.Instance.startChipCount;
        UpdateCoinText();
    }
    protected override void GameReset()
    {
        ToggleButtons(true);
        DrawCards(GameManager.Instance.playerDrawCount);
        isFold = false;
        //   tablePlayer.ClearBet();
    }
    protected override void BetRoundEnd()
    {
        // Debug.Log("BetRoundEnd");
        if (isFold || chipValue == 0)
            GameManager.Instance.photonView.RPC(nameof(GameManager.Instance.RPCCheckAllBetsIn), RpcTarget.MasterClient, true);
        else
            ToggleButtons(true);
        roundBetAmount = 0;
    }


    #region Buttons
    public void ConfirmBet()
    {
        if (roundBetAmount == 0)
            return;
        ToggleButtons(false);
        GameManager.Instance.photonView.RPC(nameof(GameManager.Instance.RPCCheckAllBetsIn), RpcTarget.MasterClient, true);
        tablePlayer.photonView.RPC(nameof(TablePlayer.RPCConfirmTableColour), RpcTarget.All);
        //  GameManager.Instance.CheckAllBetsIn();
    }
    public void FoldBet()
    {
        isFold = true;
        ResetRoundBet();
        ToggleButtons(false);
        Lose();
        GameManager.Instance.photonView.RPC(nameof(GameManager.Instance.RPCCheckAllBetsIn), RpcTarget.MasterClient, true);
        tablePlayer.photonView.RPC(nameof(TablePlayer.RPCConfirmTableColour), RpcTarget.All);
    }

    public void AddBet()
    {
        Bet(GameManager.Instance.betIncrement);
    }
    public void SubtractBet()
    {
        Bet(-GameManager.Instance.betIncrement);
    }

    public void ToggleButtons()
    {
        isButtonsActive = !isButtonsActive;
        subButton.enabled = isButtonsActive;
        addButton.enabled = isButtonsActive;
        confirmButton.enabled = isButtonsActive;
        foldButton.interactable = isButtonsActive;
    }
    public void ToggleButtons(bool isOn)
    {
        isButtonsActive = isOn;
        // Color buttonColour = isButtonsActive ? onButtonColour : offButtonColour;
        subButton.interactable = isButtonsActive;
        addButton.interactable = isButtonsActive;
        confirmButton.interactable = isButtonsActive;
        foldButton.interactable = isButtonsActive;
        // addButton.image.color = subButton.image.color = confirmButton.image.color = buttonColour;
    }
    #endregion

    void UpdateCoinText()
    {
        chipValueTxt.text = "Coin: " + chipValue;
        betValueTxt.text = "Bet: " + totalBetAmount;
    }
    void ResetRoundBet()
    {
        Bet(-roundBetAmount);
    }
    void Bet(int betValue)
    {
        if ((Mathf.Sign(betValue) == 1 && chipValue >= betValue) ||
           (Mathf.Sign(betValue) == -1 && totalBetAmount >= Mathf.Abs(betValue)))
        {
            roundBetAmount += betValue;
            chipValue -= betValue;
            totalBetAmount += betValue;
            tablePlayer.Bet(chipValue, totalBetAmount);
        }
        UpdateCoinText();
    }

    void Win()
    {
        totalBetAmount *= 2;
        chipValue += totalBetAmount;
        totalBetAmount = 0;
    }
    void Lose()
    {
        if (chipValue <= 0)
        {
            ResetChips();
        }
        totalBetAmount = 0;
    }
    public bool CheckWinLose()
    {

        CardSuit cardSuit = GetWinningSuit(redCount, greenCount);
        CardSuit winCardSuit = GetWinningSuit(redCount + GameManager.Instance.dealer.redCount, greenCount + GameManager.Instance.dealer.greenCount);
        bool isWin = false;
        if (cardSuit == winCardSuit)
        {
            Win();
            isWin = true;
        }
        else
        {
            Lose();
            isWin = false;
        }
        UpdateCoinText();
        // Debug.Log("CheckWinLose " + PhotonNetwork.LocalPlayer.ActorNumber);
        tablePlayer.ClearBet(chipValue);
        return isWin;
    }
    public void setBGColour(Color c)
    {
        c.a = 0.5f;
        bgImg.color = c;
    }
    public void SetPlayerName(string name)
    {
        playerNameTxt.text = name;
        playerName = name;
    }
}
