using System.Collections.Generic;

public interface IDeckService
{
    List<Card> CreateStandardDeck();
    void ShuffleDeck(List<Card> deck);
    void SortCardDeck(List<Card> deck);
}
