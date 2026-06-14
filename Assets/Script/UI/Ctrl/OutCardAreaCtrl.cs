using System.Collections.Generic;
using UnityEngine;

public class OutCardAreaCtrl : MonoBehaviour, IOutCardAreaView
{
    private List<Card> cards;
    public GameObject cardPrefab;
    private void Awake()
    {
        cards = new List<Card>();
        Refresh(cards);
    }
    public void RefreshOutCards(List<Card> newCards)
    {
        cards = newCards ?? new List<Card>();
        Refresh(cards);
    }
    public void Refresh(List<Card> cards)
    {
        CardRowViewHelper.Rebuild(transform, cardPrefab, cards, true);
    }

}
