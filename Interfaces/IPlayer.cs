namespace Uno.Interfaces;
public interface IPlayer
{
    string Name { get; set; }
    List<ICard> Hand { get; set; }
}