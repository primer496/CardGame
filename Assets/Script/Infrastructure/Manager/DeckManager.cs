using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour, IDeckService
{
    private List<Card> currentDeck = new List<Card>();
    private Stack<List<Card>> deckHistory = new Stack<List<Card>>();

    private static DeckConfigSO _deckConfig;
    private static DeckConfigSO DeckConfig
    {
        get
        {
            if (_deckConfig == null)
                _deckConfig = Resources.Load<DeckConfigSO>("Config/DeckConfig");
            return _deckConfig;
        }
    }
    private static DeckManager _instance;
    public static DeckManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<DeckManager>();
                if (_instance == null)
                {
                    GameObject gmObj = new GameObject("DeckManager");
                    _instance = gmObj.AddComponent<DeckManager>();
                }
            }
            return _instance;
        }
    }
    private void Awake()
    {
        // 单例初始化
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
    /// <summary>
    /// 创建一副标准扑克牌（默认 54 张，包含大小王）。
    /// </summary>
    public List<Card> CreateStandardDeck()
    {
        int total   = DeckConfig?.totalCardCount  ?? 54;
        int normal  = DeckConfig?.normalCardCount ?? 52;
        int wLK     = DeckConfig?.weightLittleJoker ?? 16;
        int wBK     = DeckConfig?.weightBigJoker    ?? 17;
        int wAce    = DeckConfig?.weightAce          ?? 14;
        int wTwo    = DeckConfig?.weightTwo          ?? 15;

        for (int i = 0; i < total; i++)
        {
            CardSuit suit;
            CardRank rank;
            int weight;
            if (i == normal)      // 小王
            {
                suit   = CardSuit.Joker;
                rank   = CardRank.LittleJoker;
                weight = wLK;
            }
            else if (i == normal + 1) // 大王
            {
                suit   = CardSuit.Joker;
                rank   = CardRank.BigJoker;
                weight = wBK;
            }
            else
            {
                suit   = (CardSuit)(i / 13);
                rank   = (CardRank)(i % 13 + 1);
                weight = (i % 13) + 1;
                if (weight == 1) weight = wAce; // A
                else if (weight == 2) weight = wTwo; // 2
            }
            Card card = new Card(i, suit, rank, weight);
            currentDeck.Add(card);
        }
        return currentDeck;
    }
    /// <summary>
    /// 洗牌（原地打乱牌组顺序）。
    /// </summary>
    public void ShuffleDeck(List<Card> deck)
    {
        System.Random rand = new System.Random();
        int n = deck.Count;
        for (int i = n-1; i >= 0; i--)
        {
            int j = rand.Next(i);
            // 交换 deck[i] 与 deck[j]
            Card temp = deck[i];
            deck[i] = deck[j];
            deck[j] = temp;
        }
    }
    /// <summary>
    /// 对牌组排序：先按权重从大到小，再按花色优先级排序。
    /// 花色优先级为 Spade > Heart > Club > Square > Joker；若仍相同，则按 id 升序稳定排序。
    /// </summary>
    /// <param name="deck"></param>
    public void SortCardDeck(List<Card> deck)
    {
        if (deck == null || deck.Count <= 1) return;

        // 花色优先级，从大到小
        var suitPriority = new Dictionary<CardSuit, int>
        {
            { CardSuit.Spade, 4 },
            { CardSuit.Heart, 3 },
            { CardSuit.Club, 2 },
            { CardSuit.Square, 1 },
            { CardSuit.Joker, 0 },
        };

        deck.Sort((a, b) =>
        {
            // 先比较权重
            int cmp = b.weight.CompareTo(a.weight);
            if (cmp != 0) return cmp;

            // 权重相同时比较花色优先级
            int pa = suitPriority.ContainsKey(a.suit) ? suitPriority[a.suit] : 0;
            int pb = suitPriority.ContainsKey(b.suit) ? suitPriority[b.suit] : 0;
            cmp = pb.CompareTo(pa);
            if (cmp != 0) return cmp;

            // 最后按 id 升序，保证顺序稳定
            return a.id.CompareTo(b.id);
        });
    }
    ///<summary>
    /// 从牌堆顶部发出一张牌，并将其从当前牌堆中移除。
    /// </summary>
    public Card DealCardFromTop()
    {
        Card topCard = currentDeck[0];
        currentDeck.RemoveAt(0);
        return topCard;
    }
}
