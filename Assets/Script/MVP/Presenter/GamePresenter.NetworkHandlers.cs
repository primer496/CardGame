using UnityEngine;

/// <summary>网络回调：更新 Model 并刷新 View（与本地按钮入口分离）。</summary>
public partial class GamePresenter
{
    public void OnCardsReceived(string cards1Json, string cards2Json, string cards3Json, string lordJson)
    {
        playPresenter?.OnCardsReceived(cards1Json, cards2Json, cards3Json, lordJson);
    }

    private void OnPlayerIdAssigned(int playerIndex)
    {
        if (model.LocalPlayerIndex != 0) return;
        model.LocalPlayerIndex = playerIndex;
        Debug.Log($"[Client] 收到玩家编号分配：{model.LocalPlayerIndex}");
    }

    public void OnBiddingTurn(int playerIndex)
    {
        biddingPresenter?.OnBiddingTurn(playerIndex);
    }

    public void OnTurnStart(int playerIndex)
    {
        playPresenter?.OnTurnStart(playerIndex);
    }

    public void OnOutCardsReceived(string cardsJson)
    {
        playPresenter?.OnOutCardsReceived(cardsJson);
    }

    public void OnPlayValidation(int result)
    {
        playPresenter?.OnPlayValidation(result);
    }

    public void OnLordConfirmed(int lordIndex)
    {
        biddingPresenter?.OnLordConfirmed(lordIndex);
    }

    public void OnGameOver(int playerIndex, bool isLord)
    {
        resultPresenter?.OnGameOver(playerIndex, isLord);
    }

    private void OnPreCardsUpdated(int preWeight, CardPattern prePattern)
    {
        playPresenter?.OnPreCardsUpdated(preWeight, prePattern);
    }

    private void OnPreCardsReset()
    {
        playPresenter?.OnPreCardsReset();
    }

}
