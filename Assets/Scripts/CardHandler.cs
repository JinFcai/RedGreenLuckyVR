using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;


public class CardHandler : MonoBehaviourPunCallbacks
{
    [SerializeField] protected List<Card> cardList = new List<Card>();
    [SerializeField] protected Transform cardContainer;

    [HideInInspector]public int redCount;
    [HideInInspector] public int greenCount;

    public virtual void Init() {
        GameManager.Instance.OnGameReset.AddListener(GameReset);
    }
    protected virtual void Awake() {
    }
    protected virtual void Start() {
       
    }
    protected virtual void GameReset() { }
    public CardSuit GetWinningSuit(int red, int green) {
        return (red > green) ? CardSuit.Red : CardSuit.Green;
    }
    public void ClearCards() {
        foreach (Card c in cardList)
        {
            c.cardImg.color = Color.clear;
        }
    }
    public void RevealAllCards()
    {
        foreach (Card c in cardList)
        {
            c.UpdateCardColour();
        }
    }
    public void DrawCardsSetup(int drawCount) {

        for (int i = 0; i < drawCount; i++)
        {
            CardSuit cardSuit = (CardSuit)Random.Range(0, 2);
            Image cardImg = Instantiate(GameManager.Instance.cardIMG, transform.position, Quaternion.identity);
            Card card = new Card(cardSuit, cardImg, false);
            card.cardImg.transform.parent = cardContainer;
            cardList.Add(card);
        }
    }

    protected int[] DrawCards(int drawCount, bool revealCards = true)
    {
        int[] redGreen = new int[drawCount];

        redCount = 0;
        greenCount = 0;
        for (int i = 0; i < cardList.Count; i++)
        {
            cardList[i].DrawNewCard(revealCards);
            redGreen[i] = (int)cardList[i].cardSuit;
            if (cardList[i].cardSuit == CardSuit.Red)
                redCount++;
            else
                greenCount++;
        }

        return redGreen;
    }
    
}
