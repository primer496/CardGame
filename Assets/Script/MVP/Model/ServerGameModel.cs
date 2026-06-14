using System.Collections.Generic;
using System.Linq;

public class ServerGameModel
{
    public GameEnums CurrentState { get; set; } = GameEnums.NotStarted;

    public List<Card> Player1Cards { get; } = new List<Card>();
    public List<Card> Player2Cards { get; } = new List<Card>();
    public List<Card> Player3Cards { get; } = new List<Card>();
    public List<Card> LordCards { get; } = new List<Card>();

    public int CurrentPlayerIndex { get; set; } = 1;
    public int LordIndex { get; set; } = 0;
    public int RefuseCount { get; set; } = 0;
    public int LastPassPlayerIndex { get; set; } = -1;

    public CardPattern ServerPrePattern { get; set; } = CardPattern.None;
    public int ServerPreWeight { get; set; } = -1;

    // 使用 1..3 下标
    public bool[] PlayerPrepared { get; } = new bool[4];

    public List<Card> GetPlayerCards(int index)
    {
        return index switch
        {
            1 => Player1Cards,
            2 => Player2Cards,
            3 => Player3Cards,
            _ => new List<Card>(),
        };
    }

    public void RemovePlayerCards(int index, List<Card> toRemove)
    {
        if (toRemove == null || toRemove.Count == 0) return;
        var hand = GetPlayerCards(index);
        hand.RemoveAll(c => toRemove.Any(oc => oc.id == c.id));
    }

    public void DealCards(IDeckService deckService)
    {
        var allCards = deckService.CreateStandardDeck();
        deckService.ShuffleDeck(allCards);

        Player1Cards.Clear();
        Player2Cards.Clear();
        Player3Cards.Clear();
        LordCards.Clear();

        Player1Cards.AddRange(allCards.GetRange(0, 17));
        Player2Cards.AddRange(allCards.GetRange(17, 17));
        Player3Cards.AddRange(allCards.GetRange(34, 17));
        LordCards.AddRange(allCards.GetRange(51, 3));
    }

    public void AssignLordCards(int lordIndex)
    {
        LordIndex = lordIndex;
        GetPlayerCards(lordIndex).AddRange(LordCards);
    }
}
