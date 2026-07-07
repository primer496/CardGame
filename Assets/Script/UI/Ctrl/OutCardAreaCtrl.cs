using System.Collections.Generic;
using UnityEngine;

public class OutCardAreaCtrl : MonoBehaviour
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
        var ns = NetworkService.Instance;
        if (ns == null) return;
        ns.OnOutCardsReceived += OnOutCardsReceived;
        ns.OnPreCardsReset += OnPreCardsReset;
    }

    private void OnDisable()
    {
        var ns = NetworkService.Instance;
        if (ns == null) return;
        ns.OnOutCardsReceived -= OnOutCardsReceived;
        ns.OnPreCardsReset -= OnPreCardsReset;
    }

    private void OnOutCardsReceived(string cardsJson)
    {
        if (string.IsNullOrEmpty(cardsJson))
        {
            RefreshOutCards(new List<Card>());
            return;
        }
        var wrapper = JsonUtility.FromJson<CardListWrapper>(cardsJson);
        RefreshOutCards(wrapper.cards);
    }

    private void OnPreCardsReset()
    {
        RefreshOutCards(new List<Card>());
        var model = GamePresenter.Instance?.Model;
        if (model != null) model.ResetPreCards();
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
