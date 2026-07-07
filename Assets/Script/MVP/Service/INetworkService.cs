using System;

public interface INetworkService
{
    /// <summary>定向：仅本客户端收到自己的手牌 JSON。</summary>
    event Action<string> OnMyCardsReceived;
    /// <summary>广播：全员收到底牌 JSON（背面显示）。</summary>
    event Action<string> OnLordCardsReceived;
    /// <summary>广播：全员收到各玩家手牌数量 (p1,p2,p3,lord)。</summary>
    event Action<int, int, int, int> OnCardCountsUpdated;
    event Action<int> OnBiddingTurn;
    event Action<int> OnPlayerIdAssigned;
    event Action<int> OnLordConfirmed;
    event Action<int> OnTurnStarted;
    event Action<string> OnOutCardsReceived;
    event Action<int> OnPlayValidation;
    event Action<int, bool> OnGameOver;
    event Action<int, CardPattern> OnPreCardsUpdated;
    event Action OnPreCardsReset;

    int CachedAssignedPlayerIndex { get; }
    void ApplyCachedPlayerIdToModel(GameModel model);

    void PreparePlayer(int localId);
    void SendPlayCards(string cardsJson);
    void SendPass();
    void SendAcceptLord(int localId);
    void SendRefuseLord(int currentId);
    void RequestNextTurn(int localId);
}
