using System.Collections.Generic;

/// <summary>左右对家手牌背牌展示，由 Presenter 在同步手牌后刷新。</summary>
public interface IOpponentHandView
{
    void Refresh(List<Card> cards);
}
