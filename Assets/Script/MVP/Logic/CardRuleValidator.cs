using System.Collections.Generic;
using System.Linq;

/// <summary>服务端出牌规则校验（原 GameServer.CheckOutCardServerRpc 内联逻辑）。会调用 CardPatternDetector.GetPattern，可能就地排序 outCards。</summary>
public static class CardRuleValidator
{
    public struct Outcome
    {
        public int Result;
        public CardPattern NewPrePattern;
        public int NewPreWeight;
    }

    /// <returns>Result: 0 不合法, 1 合法, 2 王炸；同时给出更新后的上轮牌型/权重。</returns>
    public static Outcome Validate(List<Card> outCards, CardPattern serverPrePattern, int serverPreWeight)
    {
        CardPattern pattern = CardPatternDetector.GetPattern(outCards);
        int result = 0;
        CardPattern newPrePattern = serverPrePattern;
        int newPreWeight = serverPreWeight;

        if (pattern == CardPattern.JokerBomb)
        {
            result = 2;
            newPrePattern = CardPattern.None;
            newPreWeight = -1;
        }
        else if (pattern == CardPattern.None)
        {
            result = 0;
        }
        else if (pattern == CardPattern.Bomb && serverPrePattern != CardPattern.Bomb && serverPrePattern != CardPattern.JokerBomb)
        {
            result = 1;
            newPrePattern = CardPattern.Bomb;
            newPreWeight = outCards.Max(c => c.weight);
        }
        else if (pattern != serverPrePattern && serverPrePattern != CardPattern.None)
        {
            result = 0;
        }
        else
        {
            int maxWeight = outCards.Count > 0 ? outCards.Max(c => c.weight) : -1;
            if (maxWeight <= serverPreWeight)
            {
                result = 0;
            }
            else
            {
                result = 1;
                newPreWeight = maxWeight;
                newPrePattern = pattern;
            }
        }

        return new Outcome
        {
            Result = result,
            NewPrePattern = newPrePattern,
            NewPreWeight = newPreWeight
        };
    }
}
