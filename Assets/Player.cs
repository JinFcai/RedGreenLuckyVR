using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;
using System;


public class Player : MonoBehaviour
{
   
    public UnityEvent BetCompleteEvent;
    public string playerName;
    GameManager gManager;


    int coinCount;
    int betAmount;

    List<GameManager.Card> cardList = new List<GameManager.Card>();
    int redCount = 0;
    int greenCount = 0;
    bool isButtonsActive = false;

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
    private void Awake()
    {
        gManager = FindObjectOfType<GameManager>();
        gManager.NewGameRound.AddListener(GameReset);
        
    }
    private void Start()
    {
        gManager.PlayerDrawCards(cardList, cardHolderTrans, ref redCount, ref greenCount);
        ResetChips();
        ToggleButtons(true);
    }
    private void ResetChips()
    {
        coinCount = gManager.maxChipCount;
        UpdateCoinText();
    }

    void GameReset() {
        ToggleButtons(true);
        gManager.PlayerDrawCards(cardList, cardHolderTrans, ref redCount, ref greenCount);
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
    public void ToggleButtons(bool isOn) {
        isButtonsActive = isOn;
        Color buttonColour = isButtonsActive ? onButtonColour : offButtonColour;
        subButton.enabled = isButtonsActive;
        addButton.enabled = isButtonsActive;
        confirmButton.enabled = isButtonsActive;
        addButton.image.color = subButton.image.color = confirmButton.image.color = buttonColour;
    }

    void UpdateCoinText() {
        coinTxt.text = "Coin: " + coinCount;
        betTxt.text = "Bet: " + betAmount;
    }
    void Bet(int betValue) {
        if ((Mathf.Sign(betValue) == 1 && coinCount >= betValue)||
           (Mathf.Sign(betValue) == -1 && betAmount >= Mathf.Abs(betValue)))
        {
            coinCount -= betValue;
            betAmount += betValue;
        }
        UpdateCoinText();
    }
    public void ConfirmBet() {
        if (betAmount <= 0)
            return;

        ToggleButtons(false);
        gManager.CheckAllBetsIn();
    }

    public void AddBet() {
        Bet(gManager.betInterval);
    }
    public void SubtractBet()
    {
        Bet(-gManager.betInterval);
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
    public bool CheckWinLose(GameManager.CardSuit winCardSuit) {

        GameManager.CardSuit cardSuit = redCount > greenCount ? GameManager.CardSuit.Red : GameManager.CardSuit.Green;
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
        return isWin;
    }

}
