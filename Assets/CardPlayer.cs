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
    int coinCount;
    int betAmount;
    bool isButtonsActive = false;

    [SerializeField] TablePlayer tablePlayer;

    #region UI
    [Space(10)]
    [Header("UI Elements")]
    [SerializeField] Image bgImg;
    [SerializeField] TextMeshProUGUI coinTxt;
    [SerializeField] TextMeshProUGUI betTxt;
    [SerializeField] Button subButton;
    [SerializeField] Button addButton;
    [SerializeField] Button confirmButton;
    [SerializeField] Transform cardHolderTrans;
    [SerializeField] Color offButtonColour;
    [SerializeField] Color onButtonColour;

    #endregion
    // Start is called before the first frame update

    protected override void Start()
    {
        base.Start();
        ResetChips();

        tablePlayer.Setup(coinCount);

    }
    private void ResetChips()
    {
        coinCount = GameManager.Instance.maxChipCount;
        UpdateCoinText();
    }
    protected override void GameReset()
    {
        ToggleButtons(true);
        DrawCards(GameManager.Instance.playerDrawCount);
     //   tablePlayer.ClearBet();
    }
    #region Buttons
    public void ConfirmBet() {
        if (betAmount <= 0)
            return;
        ToggleButtons(false);
        GameManager.Instance.CheckAllBetsIn();
    }
    public void AddBet() {
        Bet(GameManager.Instance.betInterval);
    }
    public void SubtractBet()
    {
        Bet(-GameManager.Instance.betInterval);
    }

    public void ToggleButtons()
    {
        isButtonsActive = !isButtonsActive;
        Color buttonColour = isButtonsActive ? onButtonColour : offButtonColour;
        subButton.enabled = isButtonsActive;
        addButton.enabled = isButtonsActive;
        confirmButton.enabled = isButtonsActive;
        addButton.image.color = subButton.image.color = confirmButton.image.color = buttonColour;
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

    void UpdateCoinText()
    {
        coinTxt.text = "Coin: " + coinCount;
        betTxt.text = "Bet: " + betAmount;
    }
    void Bet(int betValue)
    {
        if ((Mathf.Sign(betValue) == 1 && coinCount >= betValue) ||
           (Mathf.Sign(betValue) == -1 && betAmount >= Mathf.Abs(betValue)))
        {
          
            coinCount -= betValue;
            betAmount += betValue;
            tablePlayer.Bet(coinCount, betAmount);
        }
        UpdateCoinText();
    }

    void Win() {
        Debug.Log(playerName + " Win!");
        betAmount *= 2;
        coinCount += betAmount;
        betAmount = 0;
    }
    void Lose() {
        Debug.Log(playerName + " Lose!");
        if (coinCount <= 0)
        {
            ResetChips();
        }
        betAmount = 0;
    }
    public bool CheckWinLose(CardSuit winCardSuit) {

        CardSuit cardSuit = GetWinningSuit();
        Debug.Log(playerName + " | " + cardSuit);
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
        tablePlayer.ClearBet(coinCount);
        return isWin;
    }
}
