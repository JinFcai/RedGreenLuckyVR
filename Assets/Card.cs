using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;

public enum CardSuit { Red, Green };
public class Card
{

    public CardSuit cardSuit;
    public Image cardImg;
    public Card(CardSuit _cardSuit, Image _cardImg, bool reveal)
    {
        cardSuit = _cardSuit;
        cardImg = _cardImg;

        if (reveal)
            UpdateCardColour();
        else
            cardImg.color = Color.gray;

    }

    public void UpdateCardColour()
    {
        cardImg.color = cardSuit == CardSuit.Red ? Color.red : Color.green;
    }
    public void RefreshCard(bool reveal = true)
    {
        cardSuit = (CardSuit)Random.Range(0, 2);
        if (reveal)
            UpdateCardColour();
        else
            cardImg.color = Color.gray;
    }
}