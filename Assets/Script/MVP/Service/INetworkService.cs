using System;

public interface INetworkService
{
    event Action<string, string, string, string> OnCardsDelivered;
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
