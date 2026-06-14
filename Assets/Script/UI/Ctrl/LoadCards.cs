using System.Collections.Generic;
using UnityEngine;

public class LoadCards : MonoBehaviour, ILordCardsView
{
    private List<Card> cards;
    public GameObject cardPrefab;

    /// <summary>地主牌默认背面显示，调用 ShowLordCards 后切换为正面显示。</summary>
    private bool _lordCardsFaceUp;

    private void Awake()
    {
        cards = new List<Card>();
        Refresh(cards);
    }

    public void RefreshLordCards(List<Card> lordCards)
    {
        cards = lordCards ?? new List<Card>();
        Refresh(cards);
    }

    public void Refresh(List<Card> list)
    {
        CardRowViewHelper.Rebuild(transform, cardPrefab, list, _lordCardsFaceUp);
    }

    public void ShowLordCards()
    {
        _lordCardsFaceUp = true;
        Refresh(cards);
    }
}
