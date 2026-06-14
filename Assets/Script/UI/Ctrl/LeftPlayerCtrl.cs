using System.Collections.Generic;
using UnityEngine;

public class LeftPlayerCtrl : MonoBehaviour, IOpponentHandView
{
    private List<Card> cards;
    public GameObject cardPrefab;

    private void Awake()
    {
        cards = new List<Card>();
        Refresh(cards);
    }

    public void Refresh(List<Card> cards)
    {
        this.cards = cards ?? new List<Card>();
        DeckManager.Instance.SortCardDeck(this.cards);
        CardRowViewHelper.Rebuild(transform, cardPrefab, this.cards, false);
    }
}
