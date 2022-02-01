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
    /// <summary>
    /// Display card colour
    /// </summary>
    public void UpdateCardColour()
    {
        cardImg.color = cardSuit == CardSuit.Red ? Color.red : Color.green;
    }/// <summary>
    /// Draw a new card randomizing or set color of red or green,
    /// </summary>
    /// <param name="reveal">show gray if not revealed</param>
    /// <param name="cardColour">prevent randomization and set the card directly to set colour</param>
    public void DrawNewCard(bool reveal = true, int cardColour = -1)
    {
        if (cardColour == -1)
            cardSuit = (CardSuit)Random.Range(0, 2);
        else
            cardSuit = (CardSuit)cardColour;
        if (reveal)
            UpdateCardColour();
        else
            cardImg.color = Color.gray;
    }
}