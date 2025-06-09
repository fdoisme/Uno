using Uno.Enum;

namespace Uno.Interfaces;
public interface ICard
{
    CardColor Color { get; }
    CardValue Value { get; }
}