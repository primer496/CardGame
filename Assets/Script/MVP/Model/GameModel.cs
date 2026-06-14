using System.Collections.Generic;
using UnityEngine;

public class GameModel
{
    public GameEnums CurrentState { get; set; } = GameEnums.NotStarted;

    public List<Card> Player1Cards { get; } = new List<Card>();
    public List<Card> Player2Cards { get; } = new List<Card>();
    public List<Card> Player3Cards { get; } = new List<Card>();
    public List<Card> LordCards { get; } = new List<Card>();

    public int CurrentPlayerIndex { get; set; } = 1;
    public int LocalPlayerIndex { get; set; } = 0;

    public CardPattern PrePattern { get; set; } = CardPattern.None;
    public int PreWeight { get; set; } = -1;
    public int RefuseCount { get; set; } = 0;

    public int LeftPlayerIndex => (LocalPlayerIndex % 3) + 1;
    public int RightPlayerIndex => ((LocalPlayerIndex + 1) % 3) + 1;

    public void ReplaceCards(List<Card> cards1, List<Card> cards2, List<Card> cards3, List<Card> lordCards)
    {
        Player1Cards.Clear();
        Player2Cards.Clear();
        Player3Cards.Clear();
        LordCards.Clear();

        if (cards1 != null) Player1Cards.AddRange(cards1);
        if (cards2 != null) Player2Cards.AddRange(cards2);
        if (cards3 != null) Player3Cards.AddRange(cards3);
        if (lordCards != null) LordCards.AddRange(lordCards);
    }

    public List<Card> GetPlayerCards(int playerIndex)
    {
        return playerIndex switch
        {
            1 => Player1Cards,
            2 => Player2Cards,
            3 => Player3Cards,
            _ => new List<Card>(),
        };
    }

    public void StartTurn(int firstPlayer)
    {
        CurrentPlayerIndex = Mathf.Clamp(firstPlayer, 1, 3);
    }

    public void ResetPreCards()
    {
        PreWeight = -1;
        PrePattern = CardPattern.None;
        RefuseCount = 0;
    }
}
