using System.Collections.Generic;
using UnityEngine;

/// <summary>服务端纯逻辑，不直接依赖 NetworkBehaviour。</summary>
public class ServerPresenter
{
    private readonly ServerGameModel model;
    private readonly IDeckService deckService;
    private readonly IServerRpcSender sender;

    public ServerPresenter(ServerGameModel model, IDeckService deckService, IServerRpcSender sender)
    {
        this.model = model;
        this.deckService = deckService;
        this.sender = sender;
    }

    public void OnPlayerPrepared(int playerId)
    {
        if (playerId < 1 || playerId > 3) return;
        model.PlayerPrepared[playerId] = true;

        if (!model.PlayerPrepared[1] || !model.PlayerPrepared[2] || !model.PlayerPrepared[3]) return;

        model.DealCards(deckService);
        model.CurrentState = GameEnums.Bidding;
        sender.DeliverCardsForAllPlayers(model);
        sender.BroadcastBiddingTurn(model.CurrentPlayerIndex);
    }

    public bool OnRefuseLord(int currentPlayerIndex)
    {
        if (model.CurrentPlayerIndex != currentPlayerIndex) return false;

        model.CurrentPlayerIndex = currentPlayerIndex % 3 + 1;
        sender.BroadcastNotCallLandlord();
        sender.BroadcastBiddingTurn(model.CurrentPlayerIndex);
        return true;
    }

    public void OnChooseLord(int playerIndex)
    {
        model.AssignLordCards(playerIndex);
        model.CurrentPlayerIndex = playerIndex;
        model.CurrentState = GameEnums.Playing;

        sender.DeliverCardsForAllPlayers(model);
        sender.BroadcastLordConfirmed(model.CurrentPlayerIndex);
        sender.BroadcastTurnStart(model.CurrentPlayerIndex);
    }

    public void OnUpdateTurn(int callerPlayerIndex)
    {
        model.CurrentPlayerIndex = callerPlayerIndex % 3 + 1;
        sender.BroadcastTurnStart(model.CurrentPlayerIndex);
    }

    public void OnPlayCards(int playerIndex, List<Card> outCards, string cardsJson, ulong senderClientId)
    {
        if (playerIndex != model.CurrentPlayerIndex)
        {
            sender.SendValidationResult(0, senderClientId);
            return;
        }

        var validation = CardRuleValidator.Validate(outCards, model.ServerPrePattern, model.ServerPreWeight);
        model.ServerPrePattern = validation.NewPrePattern;
        model.ServerPreWeight = validation.NewPreWeight;
        sender.SendValidationResult(validation.Result, senderClientId);

        if (validation.Result != 1 && validation.Result != 2) return;

        sender.BroadcastSoundForPattern(model.ServerPrePattern, model.ServerPreWeight);
        model.RefuseCount = 0;
        sender.BroadcastOutCards(cardsJson);
        sender.BroadcastPreCardsUpdated(model.ServerPreWeight, model.ServerPrePattern);

        model.RemovePlayerCards(playerIndex, outCards);
        int remaining = model.GetPlayerCards(playerIndex).Count;
        sender.BroadcastCardCondition(playerIndex, playerIndex == model.LordIndex, remaining);
        sender.DeliverCardsForAllPlayers(model);

        if (remaining == 0)
        {
            model.CurrentState = GameEnums.GameEnd;
            Debug.Log($"玩家 {playerIndex} 牌已出完，游戏结束");
            return;
        }

        if (validation.Result == 1)
        {
            model.CurrentPlayerIndex = playerIndex % 3 + 1;
            sender.BroadcastTurnStart(model.CurrentPlayerIndex);
        }
        else
        {
            sender.BroadcastTurnStart(playerIndex);
        }
    }

    public bool OnPass(int playerIndex)
    {
        var passStep = PassSequenceCoordinator.ApplyPass(playerIndex, model.RefuseCount, model.LastPassPlayerIndex);
        if (passStep.IgnoreDuplicatePass) return false;

        sender.BroadcastPassSound();
        model.RefuseCount = passStep.RefuseCountAfter;
        model.LastPassPlayerIndex = passStep.LastPassPlayerAfter;

        if (!passStep.ShouldResetPlayArea) return true;

        model.ServerPrePattern = CardPattern.None;
        model.ServerPreWeight = -1;
        sender.BroadcastPreCardsReset();
        sender.BroadcastOutCards(string.Empty);
        sender.BroadcastPreCardsUpdated(-1, CardPattern.None);
        return true;
    }

    public void OnRequestResetPreCards()
    {
        sender.BroadcastPreCardsReset();
    }
}
