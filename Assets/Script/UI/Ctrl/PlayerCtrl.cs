using System.Collections.Generic;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour, IPlayerHandView
{
    private List<Card> cards;
    public bool isLandlord = false;
    public GameObject cardPrefab;

    private void Awake()
    {
        cards = new List<Card>();
        Refresh(cards);
    }

    public List<Card> GetSelectedCards()
    {
        List<Card> selected = new List<Card>();
        foreach (Transform t in transform)
        {
            var cell = t.GetComponent<PCardCell>();
            if (cell != null && cell.IsSelect)
            {
                selected.Add(cell.card);
            }
        }

        return selected;
    }

    public void RefreshHand(List<Card> handCards)
    {
        cards = handCards ?? new List<Card>();
        DeckManager.Instance.SortCardDeck(cards);
        Refresh(cards);
    }

    public void Refresh(List<Card> cards)
    {
        CardRowViewHelper.Rebuild(transform, cardPrefab, cards, true);
    }
}
