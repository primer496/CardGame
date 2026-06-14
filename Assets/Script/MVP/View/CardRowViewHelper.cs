using System.Collections.Generic;
using UnityEngine;

/// <summary>手牌/出牌区等“一排 PCardCell”的通用重建，避免多个 View 重复 Destroy+Instantiate。</summary>
public static class CardRowViewHelper
{
    public static void Rebuild(Transform parent, GameObject cardPrefab, IReadOnlyList<Card> cards, bool faceUp)
    {
        if (parent == null || cardPrefab == null) return;

        for (int i = 0; i < parent.childCount; i++)
            Object.Destroy(parent.GetChild(i).gameObject);

        if (cards == null) return;

        foreach (Card card in cards)
        {
            GameObject go = Object.Instantiate(cardPrefab, parent);
            go.GetComponent<PCardCell>().Refresh(card, faceUp);
        }
    }
}
