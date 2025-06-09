namespace Uno.models;
using Uno.Enum;
using Uno.Interfaces;
public class Card : ICard
{
    public CardColor Color { get; }
    public CardValue Value { get; }
    public Card(CardColor color, CardValue value)
    {
        Color = color;
        Value = value;
    }
    public override string ToString()
    {
        return $"{Color} {Value}";
    }
}