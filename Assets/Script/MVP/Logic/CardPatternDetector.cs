using System.Collections.Generic;
using UnityEngine;

public enum CardPattern
{
    None,
    Single,
    pair,
    Triple,
    TripleWithSingle,
    TripleWithPair,
    Straight,
    StraightPair,
    Bomb,
    JokerBomb,
    Plane,
}

public class CardPatternDetector
{
    private static CardGameRuleSO _rule;
    private static CardGameRuleSO Rule
    {
        get
        {
            if (_rule == null)
                _rule = Resources.Load<CardGameRuleSO>("Config/CardGameRule");
            return _rule;
        }
    }

    private static int SpecialThreshold => Rule?.specialCardWeightThreshold ?? 15;
    private static int LKWeight         => Rule?.littleJokerWeight          ?? 16;
    private static int BKWeight         => Rule?.bigJokerWeight             ?? 17;
    private static int StraightMin      => Rule?.straightMinCount           ?? 5;
    private static int StraightMax      => Rule?.straightMaxCount           ?? 12;
    private static int StraightPairMin  => Rule?.straightPairMinGroups      ?? 3;
    private static int PlaneMin         => Rule?.planeMinGroups             ?? 2;

    public static CardPattern GetPattern(List<Card> cards)
    {
        if (cards == null || cards.Count == 0)
            return CardPattern.None;

        cards.Sort((a, b) => b.weight.CompareTo(a.weight));

        if (IsJokerBomb(cards))       return CardPattern.JokerBomb;
        if (IsBomb(cards))            return CardPattern.Bomb;
        if (IsStraightPair(cards))    return CardPattern.StraightPair;
        if (IsStraight(cards))        return CardPattern.Straight;
        if (IsPlane(cards))           return CardPattern.Plane;
        if (IsTripleWithPair(cards))  return CardPattern.TripleWithPair;
        if (IsTripleWithSingle(cards))return CardPattern.TripleWithSingle;
        if (IsTriple(cards))          return CardPattern.Triple;
        if (IsPair(cards))            return CardPattern.pair;
        if (IsSingle(cards))          return CardPattern.Single;
        return CardPattern.None;
    }

    private static bool IsSingle(List<Card> cards)
        => cards.Count == 1;

    private static bool IsPair(List<Card> cards)
        => cards.Count == 2
        && cards[0].weight == cards[1].weight
        && cards[1].weight != LKWeight
        && cards[1].weight != BKWeight;

    private static bool IsTriple(List<Card> cards)
        => cards.Count == 3
        && cards[0].weight == cards[1].weight
        && cards[1].weight == cards[2].weight;

    private static bool IsTripleWithSingle(List<Card> cards)
    {
        if (cards.Count != 4) return false;
        bool leftTriple  = cards[0].weight == cards[1].weight && cards[1].weight == cards[2].weight;
        bool rightTriple = cards[1].weight == cards[2].weight && cards[2].weight == cards[3].weight;
        return leftTriple || rightTriple;
    }

    private static bool IsTripleWithPair(List<Card> cards)
    {
        if (cards.Count != 5) return false;
        bool leftTriple  = cards[0].weight == cards[1].weight && cards[1].weight == cards[2].weight && cards[3].weight == cards[4].weight;
        bool rightTriple = cards[0].weight == cards[1].weight && cards[2].weight == cards[3].weight && cards[3].weight == cards[4].weight;
        return leftTriple || rightTriple;
    }

    private static bool IsStraight(List<Card> cards)
    {
        if (cards.Count < StraightMin || cards.Count > StraightMax)
            return false;
        for (int i = 0; i < cards.Count; i++)
        {
            if (cards[i].weight >= SpecialThreshold)
                return false;
        }
        for (int i = 1; i < cards.Count; i++)
        {
            if (cards[i].weight != cards[i - 1].weight - 1)
                return false;
        }
        return true;
    }

    private static bool IsStraightPair(List<Card> cards)
    {
        if (cards.Count < StraightPairMin * 2 || cards.Count % 2 != 0)
            return false;
        for (int i = 0; i < cards.Count; i++)
        {
            if (cards[i].weight >= SpecialThreshold)
                return false;
        }
        for (int i = 0; i < cards.Count; i += 2)
        {
            if (cards[i].weight != cards[i + 1].weight)
                return false;
            if (i > 2 && cards[i].weight != cards[i - 2].weight - 1)
                return false;
        }
        return true;
    }

    private static bool IsBomb(List<Card> cards)
        => cards.Count == 4
        && cards[0].weight == cards[1].weight
        && cards[1].weight == cards[2].weight
        && cards[2].weight == cards[3].weight;

    private static bool IsJokerBomb(List<Card> cards)
    {
        if (cards.Count != 2) return false;
        int w0 = cards[0].weight;
        int w1 = cards[1].weight;
        return (w0 == LKWeight && w1 == BKWeight) || (w0 == BKWeight && w1 == LKWeight);
    }

    private static bool IsPlane(List<Card> cards)
    {
        if (cards.Count < PlaneMin * 3 || cards.Count % 3 != 0)
            return false;
        int groupCount = cards.Count / 3;
        for (int i = 0; i < cards.Count; i++)
        {
            if (cards[i].weight >= SpecialThreshold)
                return false;
        }
        for (int i = 0; i < groupCount; i++)
        {
            int baseIdx = i * 3;
            if (!(cards[baseIdx].weight == cards[baseIdx + 1].weight && cards[baseIdx + 1].weight == cards[baseIdx + 2].weight))
                return false;
            if (i > 0 && cards[baseIdx].weight != cards[(i - 1) * 3].weight - 1)
                return false;
        }
        return true;
    }
}
