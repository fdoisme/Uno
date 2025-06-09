using Uno.Enum;
using Uno.Interfaces;
using Uno.models;
using Uno.utils;

namespace Uno.views;

public class Display
{
    private static int width = Console.WindowWidth;
    private static int height = Console.WindowHeight;
    public static void SerializeHandCard(CardColor color, CardValue value, int row, int idxCard = 0, bool isPlayable = false)
    {
        Console.ForegroundColor = ConsoleColor.Black;
        switch (color)
        {
            case CardColor.Wild:
                Console.BackgroundColor = ConsoleColor.White;
                break;
            case CardColor.Green:
                Console.BackgroundColor = ConsoleColor.Green;
                break;
            case CardColor.Blue:
                Console.BackgroundColor = ConsoleColor.Blue;
                break;
            case CardColor.Yellow:
                Console.BackgroundColor = ConsoleColor.Yellow;
                break;
            case CardColor.Red:
                Console.BackgroundColor = ConsoleColor.Red;
                break;
            default:
                Console.BackgroundColor = ConsoleColor.Black;
                break;
        }
        switch (row)
        {
            case 0:

                Console.Write($"┌{string.Concat(Enumerable.Repeat("-", 4))}┐");
                Console.ResetColor();
                break;
            case 1:
                if ((int)value < 10)
                {
                    Console.Write($"|{(int)value}{string.Concat(Enumerable.Repeat(" ", 3))}|");
                }
                else
                {
                    Console.Write($"|{string.Concat(Enumerable.Repeat(" ", 4))}|");
                }
                Console.ResetColor();
                break;
            case 2:
                if ((int)value < 10)
                {
                    Console.Write($"|{string.Concat(Enumerable.Repeat(" ", 4))}|");
                }
                else
                {
                    string symbol = value switch
                    {
                        CardValue.Skip => "🛇",
                        CardValue.Reverse => "⟳",
                        CardValue.DrawTwo => "+2",
                        CardValue.Wild => "🏳️‍🌈",
                        CardValue.WildDrawFour => "+4",
                        _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Invalid card value")
                    };
                    if (value == CardValue.Wild || value == CardValue.WildDrawFour || value == CardValue.DrawTwo)
                    {
                        Console.Write($"| {symbol} |");
                    }
                    else
                    {
                        Console.Write($"| {symbol}  |");
                    }
                }
                Console.ResetColor();
                break;
            case 3:
                if ((int)value < 10)
                {
                    Console.Write($"|{string.Concat(Enumerable.Repeat(" ", 3))}{(int)value}|");
                }
                else
                {
                    Console.Write($"|{string.Concat(Enumerable.Repeat(" ", 4))}|");
                }
                Console.ResetColor();
                break;
            case 4:
                Console.Write($"└{string.Concat(Enumerable.Repeat("-", 4))}┘");
                Console.ResetColor();
                break;
            case 5:
                Console.BackgroundColor = isPlayable && idxCard != 0 ? ConsoleColor.Cyan : ConsoleColor.Black;
                Console.ForegroundColor = isPlayable && idxCard != 0 ? ConsoleColor.Black : ConsoleColor.White;
                string strIdx = idxCard.ToString();
                int padLeft = (6 - strIdx.Length) / 2;
                Console.Write($"{string.Concat(Enumerable.Repeat(" ", padLeft))}{(idxCard == 0 ? " " : idxCard)}{string.Concat(Enumerable.Repeat(" ", 6 - padLeft - strIdx.Length))}");
                Console.ResetColor();
                break;
            default:
                Console.ResetColor();
                break;
        }
    }
    public static void DrawHandCard(List<ICard> cards, List<int>? listPlayableCards = null)
    {
        Console.CursorVisible = false;
        int rowCount = 7;
        for (int row = 0; row < rowCount; row++)
        {
            for (int col = 0; col < cards.Count; col++)
            {
                Card card = (Card)cards[col];
                bool isPlayable = listPlayableCards != null && listPlayableCards.Contains(col);
                SerializeHandCard(card.Color, card.Value, row, col + 1, isPlayable);
            }
            Console.WriteLine();
        }
    }
    public static void DrawPileCard(Card card)
    {
        Console.WriteLine("\nPile Cards:");
        for (int i = 0; i < 7; i++)
        {
            SerializeHandCard(card.Color, card.Value, i);
            if (i != 6)
            {
                Console.WriteLine();
            }
        }
    }
    public static void DrawLoading()
    {
        Console.CursorVisible = false;


        string[] loadings = new string[]
        {
            "Loading",
            "Loading.",
            "Loading..",
            "Loading..."
        };
        foreach (var item in loadings)
        {
            Console.Clear();
            for (int i = 0; i < loadings.Length; i++)
            {
                Console.SetCursorPosition(width / 2, 0);
                Console.Write(loadings[i]);
                Thread.Sleep(100);
            }
            Thread.Sleep(100);
        }
    }
    public static void DrawWelcome()
    {
        Console.Clear();
        Console.CursorVisible = false;

        List<string[]> welcomeMessage = new List<string[]>
        {
            SerializeFrame(Frames.play3),
            SerializeFrame(Frames.play4),
            SerializeFrame(Frames.play5),
            SerializeFrame(Frames.play6)
        };
        List<List<Card>> unoCards = Frames.unoCards;
        int i = 0;
        int j = 0;

        while (true)
        {
            Console.Clear();
            if (i >= welcomeMessage.Count) i = 0;
            if (j >= unoCards.Count) j = 0;
            int currentY;
            Console.SetCursorPosition(0, 3);
            Console.WriteLine();
            for (int k = 0; k < welcomeMessage[i].Length; k++)
            {
                currentY = Console.CursorTop;
                Console.SetCursorPosition((width - welcomeMessage[i][k].Length) / 2, currentY);
                Console.Write(welcomeMessage[i][k] + "\n");
            }
            Console.WriteLine("\n\n");
            for (int k = 0; k < 7; k++)
            {
                currentY = Console.CursorTop;
                Console.SetCursorPosition((width - 35) / 2, currentY);
                foreach (var card in unoCards[j])
                {
                    SerializeHandCard(card.Color, card.Value, k);
                }
                Console.WriteLine();
            }
            string[] anyKey = SerializeFrame(Frames.key);
            foreach (string item in anyKey)
            {
                currentY = Console.CursorTop;
                Console.SetCursorPosition((width - 48) / 2, currentY);
                Console.WriteLine(item);
            }
            i++;
            j++;
            Thread.Sleep(300);
            if (Console.KeyAvailable)
            {
                Console.ReadKey(true);
                break;
            }
            Thread.Sleep(100);
        }
    }
    public static string[] SerializeFrame(string frames)
    {
        return frames.Split("\n");
    }
    public static void RenderText(string str, bool isWriteLn = false)
    {
        if (isWriteLn)
        {
            Console.WriteLine(str);
            return;
        }
        Console.Write(str);
    }
    public static void DrawDashboard(string[] names, int[] handCards, string currentName, Direction direction, int discardPile, int drawPile)
    {
        int width = Console.WindowWidth;
        Console.SetCursorPosition(width * 2 / 5, 0);
        for (int i = 0; i < names.Length; i++)
        {
            if (names[i] == currentName)
            {
                Console.BackgroundColor = ConsoleColor.Cyan;
                Console.ForegroundColor = ConsoleColor.Black;
            }
            string name = $"Name : {names[i]}";
            Display.RenderText($"{name}{string.Concat(Enumerable.Repeat(" ", 17 - name.Length))}");
            Console.ResetColor();
            Display.RenderText("  ");
        }
        Display.RenderText("", true);
        int currentY = Console.CursorTop;
        Console.SetCursorPosition(width * 2 / 5, currentY);
        for (int i = 0; i < names.Length; i++)
        {
            if (names[i] == currentName)
            {
                Console.BackgroundColor = ConsoleColor.Cyan;
                Console.ForegroundColor = ConsoleColor.Black;
            }
            string qty = $"Cards : {handCards[i]}";
            Console.Write($"{qty}{string.Concat(Enumerable.Repeat(" ", 17 - qty.Length))}");
            Console.ResetColor();
            Console.Write("  ");
        }
        Display.RenderText("", true);
        currentY = Console.CursorTop;
        Console.SetCursorPosition(width * 2 / 5, currentY);
        Console.Write($"Direction : {(direction == Direction.Clockwise ? ">>>>" : "<<<<")}\n");
        Console.Write($"Discard Pile : {discardPile}    Draw Pile : {drawPile}");
    }
}