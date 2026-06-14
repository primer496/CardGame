using System.Collections.Generic;
using UnityEngine;

public class PlayPresenter
{
    private readonly GameModel model;
    private readonly INetworkService networkService;
    private readonly IPlayerHandView playerHandView;
    private readonly IOpponentHandView leftOpponentView;
    private readonly IOpponentHandView rightOpponentView;
    private readonly IGamePanelView gameView;
    private readonly IOutCardAreaView outCardAreaView;
    private readonly ILordCardsView lordCardsView;

    public PlayPresenter(
        GameModel model,
        INetworkService networkService,
        IPlayerHandView playerHandView,
        IOpponentHandView leftOpponentView,
        IOpponentHandView rightOpponentView,
        IGamePanelView gameView,
        IOutCardAreaView outCardAreaView,
        ILordCardsView lordCardsView)
    {
        this.model = model;
        this.networkService = networkService;
        this.playerHandView = playerHandView;
        this.leftOpponentView = leftOpponentView;
        this.rightOpponentView = rightOpponentView;
        this.gameView = gameView;
        this.outCardAreaView = outCardAreaView;
        this.lordCardsView = lordCardsView;
    }

    public void OnCardsReceived(string cards1Json, string cards2Json, string cards3Json, string lordJson)
    {
        var wrapper1 = JsonUtility.FromJson<CardListWrapper>(cards1Json);
        var wrapper2 = JsonUtility.FromJson<CardListWrapper>(cards2Json);
        var wrapper3 = JsonUtility.FromJson<CardListWrapper>(cards3Json);
        var wrapperLord = JsonUtility.FromJson<CardListWrapper>(lordJson);

        model.ReplaceCards(wrapper1.cards, wrapper2.cards, wrapper3.cards, wrapperLord.cards);
        playerHandView?.RefreshHand(model.GetPlayerCards(model.LocalPlayerIndex));
        leftOpponentView?.Refresh(model.GetPlayerCards(model.LeftPlayerIndex));
        rightOpponentView?.Refresh(model.GetPlayerCards(model.RightPlayerIndex));
        lordCardsView?.RefreshLordCards(model.LordCards);
        UpdateEnemyCardCount();
    }

    public void OnTurnStart(int playerIndex)
    {
        model.StartTurn(playerIndex);
        gameView?.SetTurnUI(playerIndex == model.LocalPlayerIndex);
        UpdateEnemyCardCount();
    }

    public void OnLocalPlayCards()
    {
        if (playerHandView == null) return;

        List<Card> selectedCards = playerHandView.GetSelectedCards();
        string cardsJson = JsonUtility.ToJson(new CardListWrapper { cards = selectedCards });
        networkService.SendPlayCards(cardsJson);
    }

    public void OnLocalPass()
    {
        networkService.SendPass();
        networkService.RequestNextTurn(model.LocalPlayerIndex);
    }

    public void OnOutCardsReceived(string cardsJson)
    {
        if (string.IsNullOrEmpty(cardsJson))
        {
            outCardAreaView?.RefreshOutCards(new List<Card>());
            return;
        }

        var wrapper = JsonUtility.FromJson<CardListWrapper>(cardsJson);
        outCardAreaView?.RefreshOutCards(wrapper.cards);
    }

    public void OnPreCardsUpdated(int preWeight, CardPattern prePattern)
    {
        model.PreWeight = preWeight;
        model.PrePattern = prePattern;
    }

    public void OnPreCardsReset()
    {
        model.ResetPreCards();
        outCardAreaView?.RefreshOutCards(new List<Card>());
    }

    public void OnPlayValidation(int result)
    {
        switch (result)
        {
            case 0:
                Debug.Log("[Client] 出牌被服务器拒绝");
                break;
            case 1:
                Debug.Log("[Client] 出牌被服务器接受");
                break;
            case 2:
                Debug.Log("[Client] 王炸，被服务器接受");
                break;
        }
    }

    private void UpdateEnemyCardCount()
    {
        int leftCount = model.GetPlayerCards(model.LeftPlayerIndex).Count;
        int rightCount = model.GetPlayerCards(model.RightPlayerIndex).Count;
        gameView?.RefreshPlayerCardCount(leftCount, rightCount);
    }
}
