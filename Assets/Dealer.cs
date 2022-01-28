using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dealer : CardHandler
{
    protected override void GameReset()
    {
        DrawCards(GameManager.Instance.dealerDrawCount, false);
    }
}
