namespace Uno.models;
using Uno.Enum;
using Uno.Interfaces;
public class Deck : IDeck
{
    public List<ICard> Cards { get; set; }

    public Deck(List<ICard> cards)
    {
        Cards = cards;
    }
}