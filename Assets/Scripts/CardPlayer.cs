using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;


public class CardPlayer : CardHandler
{
    int chipValue;
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
    [SerializeField] Transform cardHolderTrans;
    #endregion
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
    }

    #region game setup
    private void ResetChips()
    {
        chipValue = GameManager.Instance.startChipCount;
        UpdateCoinText();
    }
    protected override void GameReset()
    {
        ToggleButtons(true);
        DrawCards(GameManager.Instance.playerDrawCount);
        //   tablePlayer.ClearBet();
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
            tablePlayer.photonView.RPC(nameof(TablePlayer.RPCBetChange), RpcTarget.All, chipValue, totalBetAmount);
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
        CardSuit winCardSuit = GetWinningSuit(GameManager.Instance.dealer.redCount, GameManager.Instance.dealer.greenCount);
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
        tablePlayer.photonView.RPC(nameof(TablePlayer.RPCClearBet), RpcTarget.All, chipValue);
        return isWin;
    }
    #endregion
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
    }
    public void ToggleButtons(bool isOn)
    {
        isButtonsActive = isOn;
        // Color buttonColour = isButtonsActive ? onButtonColour : offButtonColour;
        subButton.interactable = isButtonsActive;
        addButton.interactable = isButtonsActive;
        confirmButton.interactable = isButtonsActive;
        // addButton.image.color = subButton.image.color = confirmButton.image.color = buttonColour;
    }
    #endregion
    #region UI
    public void SetBGColour(Color c)
    {
        c.a = 0.5f;
        bgImg.color = c;
    }
    public void SetPlayerName(string name)
    {
        playerNameTxt.text = name;
    }
    void UpdateCoinText()
    {
        chipValueTxt.text = "Coin: " + chipValue;
        betValueTxt.text = "Bet: " + totalBetAmount;
    }
    #endregion
}
