using System.Collections.Generic;
using UnityEngine;

/// <summary>手牌/出牌区等"一排 PCardCell"的通用重建，避免多个 View 重复 Destroy+Instantiate。
/// 自动根据容器宽度和卡牌数量计算重叠间距，适配移动端窄屏。</summary>
public static class CardRowViewHelper
{
    /// <summary>卡牌默认宽度（逻辑像素），与 MyCard/OthersCard 预制体一致</summary>
    private const float DefaultCardWidth = 120f;
    /// <summary>手牌可见部分的最小重叠后宽度</summary>
    private const float MinCardPeekWidth = 35f;
    /// <summary>左右边距</summary>
    private const float HorizontalPadding = 10f;

    public static void Rebuild(Transform parent, GameObject cardPrefab, IReadOnlyList<Card> cards, bool faceUp)
    {
        if (parent == null || cardPrefab == null) return;

        // 销毁旧子对象
        for (int i = parent.childCount - 1; i >= 0; i--)
            Object.Destroy(parent.GetChild(i).gameObject);

        if (cards == null || cards.Count == 0) return;

        // 获取卡牌宽度（从预制体 RectTransform 读取，回退默认值）
        float cardWidth = DefaultCardWidth;
        RectTransform prefabRect = cardPrefab.GetComponent<RectTransform>();
        if (prefabRect != null && prefabRect.rect.width > 0)
        {
            cardWidth = prefabRect.rect.width;
        }

        // 获取容器可用宽度
        float availableWidth = GetAvailableWidth(parent);

        // 计算间距：尽量让所有卡牌在容器内均匀分布
        float spacing = CalculateSpacing(cards.Count, cardWidth, availableWidth);

        // 生成卡牌并定位
        for (int i = 0; i < cards.Count; i++)
        {
            GameObject go = Object.Instantiate(cardPrefab, parent);
            RectTransform rt = go.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.anchoredPosition = new Vector2(HorizontalPadding + i * spacing, 0f);
            }
            go.GetComponent<PCardCell>().Refresh(cards[i], faceUp);
        }

        // 更新父容器宽度以适应所有卡牌
        RectTransform parentRect = parent.GetComponent<RectTransform>();
        if (parentRect != null)
        {
            float totalWidth = HorizontalPadding * 2f + (cards.Count - 1) * spacing + cardWidth;
            parentRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Max(totalWidth, availableWidth));
        }
    }

    /// <summary>获取容器可用宽度（逻辑像素）</summary>
    private static float GetAvailableWidth(Transform parent)
    {
        RectTransform rt = parent.GetComponent<RectTransform>();
        if (rt == null) return Screen.width;
        return rt.rect.width > 0 ? rt.rect.width : Screen.width;
    }

    /// <summary>计算卡牌间距：优先让所有卡牌填入容器，超出时使用最小重叠宽度</summary>
    private static float CalculateSpacing(int cardCount, float cardWidth, float availableWidth)
    {
        float usableWidth = availableWidth - HorizontalPadding * 2f;
        if (cardCount <= 1) return 0f;

        // 理想间距：卡牌均匀分布
        float idealSpacing = (usableWidth - cardWidth) / (cardCount - 1);

        // 如果理想间距大于卡牌宽度，限制为卡牌宽度（不露空白）
        if (idealSpacing > cardWidth)
            idealSpacing = cardWidth;

        // 确保每张卡至少露出 MinCardPeekWidth 宽度
        if (idealSpacing < MinCardPeekWidth)
            idealSpacing = MinCardPeekWidth;

        return idealSpacing;
    }
}
