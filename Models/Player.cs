namespace Uno.models;
using Uno.Enum;
using Uno.Interfaces;
public class Player : IPlayer
{
    public string Name { get; set; }
    public List<ICard> Hand { get; set; }

    public Player(string name, List<ICard>? hand = null)
    {
        Name = name;
        Hand = hand ?? new List<ICard>();
    }
}