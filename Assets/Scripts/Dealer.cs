using UnityEngine;
using Photon.Pun;

public class Dealer : CardHandler
{
    [HideInInspector] public int[] drawnRedGreen;
    public override void Init()
    {
        base.Init();
        DrawCardsSetup(GameManager.Instance.dealerDrawCount);
    }
    protected override void Start()
    {
        base.Start();
    }
    protected override void GameReset()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            drawnRedGreen = DrawCards(GameManager.Instance.dealerDrawCount, false);
            photonView.RPC(nameof(ClientDealerDrawCards), RpcTarget.Others, drawnRedGreen);
        }
    }
    /// <summary>
    /// Tells client to draw cards in specific order
    /// </summary>
    /// <param name="_drawnRedGreen"></param>
    [PunRPC]
    public void ClientDealerDrawCards(int[] _drawnRedGreen)
    {
        redCount = 0;
        greenCount = 0;
        for (int i = 0; i < cardList.Count; i++)
        {
            cardList[i].DrawNewCard(false, _drawnRedGreen[i]);
            if (cardList[i].cardSuit == CardSuit.Red)
                redCount++;
            else
                greenCount++;
        }
    }
}
