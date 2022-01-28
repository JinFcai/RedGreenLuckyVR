using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;


public class CardHandler : MonoBehaviour
{
    [SerializeField] protected List<Card> cardList = new List<Card>();
    [SerializeField] protected Transform cardContainer;

    [HideInInspector]public int redCount;
    [HideInInspector] public int greenCount;

    public void Init() {
        GameManager.Instance.NewGameRound.AddListener(GameReset);
    }
    protected virtual void Awake() {
    }
    protected virtual void Start() {
    }
    protected virtual void GameReset() { 
    
    }
    public CardSuit GetWinningSuit() {
        return (redCount > greenCount) ? CardSuit.Red : CardSuit.Green;
    }
    public void ClearCards() {
        foreach (Card c in cardList)
        {
            c.cardImg.color = Color.clear;
        }
    }
    public void RevealCards()
    {
        foreach (Card c in cardList)
        {
            c.UpdateCardColour();
        }
    }
    protected virtual void DrawCards(int drawCount, bool revealCards = true)
    {
        redCount = 0;
        greenCount = 0;
        if (cardList.Count != drawCount)
        {
            for (int i = 0; i < drawCount; i++)
            {
                CardSuit cardSuit = (CardSuit)Random.Range(0, 2);
                Image cardImg = Instantiate(GameManager.Instance.cardIMG, transform.position, Quaternion.identity);
                Card card = new Card(cardSuit, cardImg, revealCards);
                card.cardImg.transform.parent = cardContainer;
                if (card.cardSuit == CardSuit.Red)
                    redCount++;
                else
                    greenCount++;
                cardList.Add(card);
            }
        }
        else
        {
            for (int i = 0; i < cardList.Count; i++)
            {
                cardList[i].RefreshCard(revealCards);
                if (cardList[i].cardSuit == CardSuit.Red)
                    redCount++;
                else
                    greenCount++;
            }
        }
    }
}
