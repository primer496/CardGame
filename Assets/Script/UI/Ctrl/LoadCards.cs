using System.Collections.Generic;
using UnityEngine;

public class LoadCards : MonoBehaviour
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

    private void OnEnable()
    {
        var gp = GamePresenter.Instance;
        if (gp != null) gp.OnLocalCardsUpdated += OnLocalCardsUpdated;

        var ns = NetworkService.Instance;
        if (ns != null) ns.OnLordConfirmed += OnLordConfirmed;
    }

    private void OnDisable()
    {
        var gp = GamePresenter.Instance;
        if (gp != null) gp.OnLocalCardsUpdated -= OnLocalCardsUpdated;

        var ns = NetworkService.Instance;
        if (ns != null) ns.OnLordConfirmed -= OnLordConfirmed;
    }

    private void OnLocalCardsUpdated()
    {
        var model = GamePresenter.Instance?.Model;
        if (model == null) return;
        RefreshLordCards(model.LordCards);
    }

    private void OnLordConfirmed(int lordIndex)
    {
        ShowLordCards();
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
