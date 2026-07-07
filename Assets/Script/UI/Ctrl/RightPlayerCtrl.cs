using System.Collections.Generic;
using UnityEngine;

public class RightPlayerCtrl : MonoBehaviour
{
    private List<Card> cards;
    public GameObject cardPrefab;

    private void Awake()
    {
        cards = new List<Card>();
        Refresh(cards);
    }

    private void OnEnable()
    {
        var gp = GamePresenter.Instance;
        if (gp != null) gp.OnLocalCardsUpdated += OnLocalCardsUpdated;
    }

    private void OnDisable()
    {
        var gp = GamePresenter.Instance;
        if (gp != null) gp.OnLocalCardsUpdated -= OnLocalCardsUpdated;
    }

    private void OnLocalCardsUpdated()
    {
        var model = GamePresenter.Instance?.Model;
        if (model == null) return;
        Refresh(model.GetPlayerCards(model.RightPlayerIndex));
    }

    public void Refresh(List<Card> cards)
    {
        this.cards = cards ?? new List<Card>();
        DeckManager.Instance.SortCardDeck(this.cards);
        CardRowViewHelper.Rebuild(transform, cardPrefab, this.cards, false);
    }
}
