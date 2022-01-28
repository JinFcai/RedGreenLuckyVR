using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;


public class GameManager : MonoBehaviourPunCallbacks
{
    //public class Card
    //{
    //    public CardSuit cardSuit;
    //    public Image cardImg;
    //    public Card(CardSuit _cardSuit, Image _cardImg)
    //    {
    //        cardSuit = _cardSuit;
    //        cardImg = _cardImg;
    //        UpdateCardColour();
    //    }

    //    void UpdateCardColour()
    //    {
    //        cardImg.color = cardSuit == CardSuit.Red ? Color.red : Color.green;
    //    }
    //    public void RefreshCard()
    //    {
    //        cardSuit = (GameManager.CardSuit)Random.Range(0, 2);
    //        UpdateCardColour();
    //    }
    //}
    // public enum CardSuit {Red, Green};
    public static GameManager Instance;

    public Image cardIMG;

    [Header("dealer")]
    public int dealerDrawCount;
    public UnityEvent NewGameRound;
    public Dealer dealer;

    [Header("Player")]
    public int maxChipCount = 100;
    public int betInterval = 10;
    public int playerDrawCount = 3;

    [Header("Photon")]
    public string playerPrefabLocation;
    int playersInGame;
    [SerializeField] List<CardPlayer> playerList = new List<CardPlayer>();


    string winnerNames;
    int playersRdy = 0;

    #region UI
    [SerializeField] TextMeshProUGUI resultTxt;
    #endregion
    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
        playerList = new List<CardPlayer>(FindObjectsOfType<CardPlayer>());
        foreach (CardPlayer p in playerList)
            p.Init();
        dealer.Init();
    }
    private void Start()
    {
        GameReset();
    }
    public void CheckAllBetsIn() {
        playersRdy++;
        if (playersRdy >= playerList.Count) {
            CheckBetResultPhase();
            playersRdy = 0;
        }
    }
    void CheckBetResultPhase()
    {
        winnerNames = "";
        dealer.RevealCards();
        CardSuit winSuit = dealer.GetWinningSuit();
       // DrawCards(dealerCardList, dealerDrawCount, dealerCardContainerTrans, ref redCount, ref greenCount);
       // CardSuit winSuit = (redCount > greenCount) ? CardSuit.Red : CardSuit.Green;

        Debug.Log("CheckResultPhase : " + winSuit);
        foreach (CardPlayer player in playerList)
        {
            if (player.CheckWinLose(winSuit))
            {
                winnerNames += player.playerName + "\n";
            }
        }
        if (winnerNames != "")
            resultTxt.text = "Winner! \n" + winnerNames;
        else
            resultTxt.text = "No Winner!"; 


        Invoke(nameof(GameReset), 3);
    }
    void GameReset() {

        Debug.Log("game reset");
        NewGameRound.Invoke();
        resultTxt.text = "Place your bets!";
    }
}
