using System.Collections.Generic;

public interface IServerRpcSender
{
    /// <summary>定向发送：每个客户端只收到自己的手牌；全员收到底牌和各家牌数。</summary>
    void DeliverCardsForAllPlayers(ServerGameModel model);
    void BroadcastBiddingTurn(int playerIndex);
    void BroadcastLordConfirmed(int lordIndex);
    void BroadcastTurnStart(int playerIndex);
    void BroadcastOutCards(string cardsJson);
    void BroadcastPreCardsUpdated(int weight, CardPattern pattern);
    void BroadcastPreCardsReset();
    void BroadcastNotCallLandlord();
    void BroadcastPassSound();
    void BroadcastSoundForPattern(CardPattern pattern, int weight);
    void BroadcastCardCondition(int playerIndex, bool isLord, int remaining);
    void SendValidationResult(int result, ulong clientId);
}
