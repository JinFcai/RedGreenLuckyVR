using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public class Card
    {
        public CardSuit cardSuit;
        public Image cardImg;
        public Card(CardSuit _cardSuit, Image _cardImg)
        {
            cardSuit = _cardSuit;
            cardImg = _cardImg;
            UpdateCardColour();
        }

        void UpdateCardColour()
        {
            cardImg.color = cardSuit == CardSuit.Red ? Color.red : Color.green;
        }
        public void RefreshCard()
        {
            cardSuit = (GameManager.CardSuit)Random.Range(0, 2);
            UpdateCardColour();
        }
    }
    public enum CardSuit {Red, Green};
    public Image cardIMG;



    [Header("dealer")]
    public List<Card> dealerCardList = new List<Card>();
    public int dealerDrawCount;
    public Transform dealerCardContainerTrans;
    public int redCount;
    public int greenCount;
    public UnityEvent NewGameRound;


    [Header("Player")]
    public int maxChipCount = 100;
    public int betInterval = 10;
    public int playerDrawCount = 3;


    [Header("Game Turn")]
    public int betPool = 0;
    public int maxTurnBet = 0;


    [SerializeField] List<Player> playerList = new List<Player>();
    string winnerNames;
    int playersRdy = 0;

    #region UI
    [SerializeField] TextMeshProUGUI resultTxt;

    #endregion


    private void Awake()
    {
        playerList = new List<Player>(FindObjectsOfType<Player>());
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
        DrawCards(dealerCardList, dealerDrawCount, dealerCardContainerTrans, ref redCount, ref greenCount);
        CardSuit winSuit = (redCount > greenCount) ? CardSuit.Red : CardSuit.Green;
        Debug.Log("CheckResultPhase : " + winSuit);
        foreach (Player player in playerList)
        {
            if (player.CheckWinLose(winSuit))
            {
                winnerNames += player.playerName + "\n";
            }
        }
        resultTxt.text = "Winner! \n" + winnerNames;
        Invoke(nameof(GameReset), 3);
    }
    void ClearCards() {
        foreach (Card c in dealerCardList) {
            c.cardImg.color = Color.clear;
        }
    }

    void GameReset() {
        NewGameRound.Invoke();
        ClearCards();
        resultTxt.text = "";
    }
    public void PlayerDrawCards(List<Card> cardList, Transform cardTransformParent, ref int red, ref int green)
    {
        DrawCards(cardList, playerDrawCount, cardTransformParent, ref red, ref green);
    }
    public void DrawCards(List<Card> cardList, int drawCount, Transform cardTransformParent, ref int red, ref int green)
    {
        red = 0;
        green = 0;
        if (cardList.Count != drawCount)
        {
            for (int i = 0; i < drawCount; i++)
            {
                GameManager.CardSuit cardSuit = (GameManager.CardSuit)Random.Range(0, 2);
                Image cardImg = Instantiate(cardIMG, transform.position, Quaternion.identity);
                GameManager.Card card = new GameManager.Card(cardSuit, cardImg);
                card.cardImg.transform.parent = cardTransformParent;
                if (card.cardSuit == GameManager.CardSuit.Red)
                    red++;
                else
                    green++;
                cardList.Add(card);
            }
        }
        else
        {
            for (int i = 0; i < cardList.Count; i++)
            {
                cardList[i].RefreshCard();
                if (cardList[i].cardSuit == GameManager.CardSuit.Red)
                    red++;
                else
                    green++;
            }
        }
    }
}
