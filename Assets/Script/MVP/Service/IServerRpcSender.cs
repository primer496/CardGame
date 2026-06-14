using System.Collections.Generic;

public interface IServerRpcSender
{
    void BroadcastCards(ServerGameModel model);
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
