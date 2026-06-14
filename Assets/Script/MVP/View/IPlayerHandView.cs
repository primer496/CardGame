using System.Collections.Generic;

public interface IPlayerHandView
{
    List<Card> GetSelectedCards();
    void RefreshHand(List<Card> cards);
}
