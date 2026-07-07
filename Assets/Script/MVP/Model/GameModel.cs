using System.Collections.Generic;
using UnityEngine;

public class GameModel
{
    public GameEnums CurrentState { get; set; } = GameEnums.NotStarted;

    /// <summary>仅存储本地玩家的手牌，不再持有对手的牌（防作弊）。</summary>
    public List<Card> MyCards { get; } = new List<Card>();
    /// <summary>底牌（全员可见，地主确认前背面显示）。</summary>
    public List<Card> LordCards { get; } = new List<Card>();
    /// <summary>各玩家手牌数量 [1]=玩家1, [2]=玩家2, [3]=玩家3。</summary>
    public int[] PlayerCardCounts { get; } = new int[4];

    public int CurrentPlayerIndex { get; set; } = 1;
    public int LocalPlayerIndex { get; set; } = 0;

    public CardPattern PrePattern { get; set; } = CardPattern.None;
    public int PreWeight { get; set; } = -1;
    public int RefuseCount { get; set; } = 0;

    public int LeftPlayerIndex => (LocalPlayerIndex % 3) + 1;
    public int RightPlayerIndex => ((LocalPlayerIndex + 1) % 3) + 1;

    /// <summary>更新自己的手牌和底牌。</summary>
    public void ReplaceMyCards(List<Card> myCards)
    {
        MyCards.Clear();
        if (myCards != null) MyCards.AddRange(myCards);
    }

    /// <summary>更新底牌。</summary>
    public void ReplaceLordCards(List<Card> lordCards)
    {
        LordCards.Clear();
        if (lordCards != null) LordCards.AddRange(lordCards);
    }

    /// <summary>更新各家手牌数量。</summary>
    public void UpdateCardCounts(int p1, int p2, int p3, int lord)
    {
        PlayerCardCounts[1] = p1;
        PlayerCardCounts[2] = p2;
        PlayerCardCounts[3] = p3;
    }

    /// <summary>
    /// 获取玩家手牌。本地玩家返回真实牌，对手返回占位牌（仅用于牌背渲染，防止数据泄露）。
    /// </summary>
    public List<Card> GetPlayerCards(int playerIndex)
    {
        if (playerIndex == LocalPlayerIndex) return new List<Card>(MyCards);

        int count = playerIndex >= 1 && playerIndex <= 3 ? PlayerCardCounts[playerIndex] : 0;
        var placeholders = new List<Card>(count);
        for (int i = 0; i < count; i++) placeholders.Add(new Card());
        return placeholders;
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
