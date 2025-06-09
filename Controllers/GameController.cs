namespace Uno.controllers;

using System.Security.Cryptography;
using Uno.Enum;
using Uno.Interfaces;
using Uno.models;
using Uno.views;


public class GameController
{
    List<IPlayer> _players = new List<IPlayer>();
    Deck _drawPile;
    List<ICard> _discardPile = new List<ICard>();
    IPlayer _currentPlayer;
    Direction _currentDirection = Direction.Clockwise;
    CardColor _currentColor;
    Action _onCardPlayed;
    int _cardToDraw;
    bool isGameOver;

    public GameController(List<IPlayer> players)
    {
        _players = players;
    }

    public void DrawCardToPlayer(IPlayer player, int count)
    {
        for (int i = 0; i < count; i++)
        {
            player.Hand.Add(_drawPile.Cards[0]);
            _drawPile.Cards.RemoveAt(0);
        }
    }
    public void RemoveCardFromPlayer(Card card)
    {
        foreach (ICard item in _currentPlayer.Hand)
        {
            if (item.Color == card.Color && item.Value == card.Value)
            {
                _currentPlayer.Hand.Remove(item);
                break;
            }
        }
    }
    public void ReverseDirection()
    {
        _currentDirection = _currentDirection == Direction.Clockwise ? Direction.CounterClockwise : Direction.Clockwise;
    }
    public void SkipPlayer()
    {
        GetNextPlayer();
    }
    public CardColor PromptForColorChoice()
    {
        Display.RenderText("Choose a color(Red, Green, Blue, Yellow) : ");
        string? input = Console.ReadLine();
        if (System.Enum.TryParse<CardColor>(input, true, out CardColor color))
        {
            return color;
        }
        else
        {
            Display.RenderText("Invalid color choice");
            return PromptForColorChoice();
        }
    }
    public void SetCurrentColor(CardColor color)
    {
        _currentColor = color;
    }
    public bool CanPlayerPlay(IPlayer player)
    {
        foreach (Card card in player.Hand)
        {
            if (CheckValidMove(card)) return true;
        }
        return false;
    }
    public void ApplyEffect()
    {
        switch (_discardPile[0].Value)
        {
            case CardValue.Skip:
                SkipPlayer();
                break;
            case CardValue.Reverse:
                ReverseDirection();
                break;
            case CardValue.DrawTwo:
                _cardToDraw += 2;
                break;
            case CardValue.WildDrawFour:
                _cardToDraw += 4;
                break;
        }
    }
    public bool CheckValidMove(ICard cardToPlay)
    {
        if (cardToPlay.Value == CardValue.WildDrawFour) return true;
        if (_cardToDraw == 0 && (_currentColor == cardToPlay.Color || cardToPlay.Color == CardColor.Wild || _discardPile[0].Value == cardToPlay.Value)) return true;
        if (_cardToDraw != 0 && _discardPile[0].Value == cardToPlay.Value) return true;
        return false;
    }
    public List<int> GetPlayableCards()
    {
        List<int> playableCards = new List<int>();
        for (int i = 0; i < _currentPlayer.Hand.Count; i++)
        {
            Card card = (Card)_currentPlayer.Hand[i];
            if (CheckValidMove(card))
            {
                playableCards.Add(i);
            }
        }
        return playableCards;
    }
    public void GetNextPlayer()
    {
        int currentIndex = _players.IndexOf(_currentPlayer);
        if (_currentDirection == Direction.Clockwise)
        {
            currentIndex = (currentIndex + 1) % _players.Count;
        }
        else
        {
            currentIndex = (currentIndex - 1 + _players.Count) % _players.Count;
        }
        _currentPlayer = _players[currentIndex];
    }
    public void HandleTurn()
    {
        Console.Clear();
        _onCardPlayed.Invoke();
        Display.RenderText("", true);
        Display.DrawPileCard((Card)_discardPile[0]);
        Display.RenderText($"{(_discardPile[0].Color == CardColor.Wild ? $"Color : {_currentColor}    " : "")}{(_cardToDraw != 0 ? $"Card to drawn : {_cardToDraw}" : "")}\n", true);
        Display.RenderText($"Hand Card Player {_currentPlayer.Name}", true);
        Display.DrawHandCard(_currentPlayer.Hand, GetPlayableCards());
        bool canPlay = CanPlayerPlay(_currentPlayer);
        if (!canPlay)
        {
            Display.RenderText("No card can be played, press any key to continue (will draw card)");
            Console.ReadKey();
            DrawCardToPlayer(_currentPlayer, _cardToDraw != 0 ? _cardToDraw : 1);
            _cardToDraw = 0;
            GetNextPlayer();
            return;
        }
        Display.RenderText("Choose a card (1, 2, ..., n) or type 'draw' : ");
        string? input = Console.ReadLine();
        string[] inputWwithUno = input.Split(" ");
        if (inputWwithUno[0]?.ToLower() == "draw")
        {
            DrawCardToPlayer(_currentPlayer, _cardToDraw != 0 ? _cardToDraw : 1);
            _cardToDraw = 0;
            GetNextPlayer();
            return;
        }
        int cardIndex;
        if (int.TryParse(inputWwithUno[0], out cardIndex))
        {
            cardIndex -= 1;
            if (cardIndex >= 0 && cardIndex < _currentPlayer.Hand.Count)
            {
                Card selectedCard = (Card)_currentPlayer.Hand[cardIndex];
                if (CheckValidMove(selectedCard))
                {
                    RemoveCardFromPlayer(selectedCard);
                    InsertDiscardPile(selectedCard);
                    ApplyEffect();
                }
                else
                {
                    Display.RenderText("Invalid move. Try again. Press any key..");
                    Console.ReadKey();
                    return;
                }
            }
            else
            {
                Display.RenderText("Invalid input. Out Range. Press any key..");
                Console.ReadKey();
                return;
            }
        }
        else
        {
            Display.RenderText("Invalid input. Try again. Press any key..");
            Console.ReadKey();
            return;
        }
        HandleUnoCall(inputWwithUno);

        if (_currentPlayer.Hand.Count == 0)
        {
            isGameOver = true;
            return;
        }
        GetNextPlayer();
    }
    internal void StartGame()
    {
        Display.RenderText($"Masukkan jumlah pack kartu: ");
        string? inputPackCard = Console.ReadLine();
        int packCard;
        if (!int.TryParse(inputPackCard, out packCard))
        {
            Display.RenderText($"Invalid input, card pack will become 1");
            packCard = 1;
        }
        _drawPile = InitializeDeck(packCard);
        ShuffleDeck(_drawPile.Cards);
        foreach (IPlayer player in _players)
        {
            DrawCardToPlayer(player, 1);
        }
        SetupInitialDiscard();
        IntializeTurn();
        _onCardPlayed = Dashboard;
        while (!isGameOver)
        {
            HandleTurn();
            RestockDrawPile();
        }
    }
    public void IntializeTurn()
    {
        _currentPlayer = _players[RandomNumberGenerator.GetInt32(0, _players.Count)];
    }
    public Deck InitializeDeck(int packCard)
    {
        Deck deck = new Deck(new List<ICard>());
        foreach (CardValue item in System.Enum.GetValues(typeof(CardValue)))
        {
            switch (item)
            {
                case CardValue.Wild:
                case CardValue.WildDrawFour:
                    for (int i = 0; i < (4 * packCard); i++)
                    {
                        deck.Cards.Add(new Card(CardColor.Wild, item));
                    }
                    break;
                case CardValue.Zero:
                    for (int i = 0; i < (1 * packCard); i++)
                    {
                        deck.Cards.Add(new Card(CardColor.Red, item));
                        deck.Cards.Add(new Card(CardColor.Green, item));
                        deck.Cards.Add(new Card(CardColor.Blue, item));
                        deck.Cards.Add(new Card(CardColor.Yellow, item));
                    }
                    break;
                default:
                    for (int i = 0; i < (2 * packCard); i++)
                    {
                        deck.Cards.Add(new Card(CardColor.Red, item));
                        deck.Cards.Add(new Card(CardColor.Green, item));
                        deck.Cards.Add(new Card(CardColor.Blue, item));
                        deck.Cards.Add(new Card(CardColor.Yellow, item));
                    }
                    break;
            }
        }
        return deck;
    }
    public void ShuffleDeck(List<ICard> deck)
    {
        for (int i = 0; i < deck.Count
    ; i++)
        {
            int n = RandomNumberGenerator.GetInt32(0, deck.Count - 1);
            (deck[i], deck[n]) = (deck[n], deck[i]);
        }
    }
    public void SetupInitialDiscard()
    {
        InsertDiscardPile(_drawPile.Cards[0]);
    }
    public void InsertDiscardPile(ICard card)
    {
        _discardPile.Insert(0, card);
        if (_discardPile[0].Value == CardValue.Wild || _discardPile[0].Value == CardValue.WildDrawFour)
        {
            SetCurrentColor(PromptForColorChoice());
            return;
        }
        SetCurrentColor(_discardPile[0].Color);
    }
    public void RestockDrawPile()
    {
        if (_drawPile.Cards.Count <= 30)
        {
            List<ICard> tempDiscard = _discardPile.Skip(1).ToList();
            ShuffleDeck(tempDiscard);
            _drawPile.Cards.AddRange(tempDiscard);
            _discardPile.RemoveRange(1, tempDiscard.Count);
        }
    }
    public void Dashboard()
    {
        string[] names = _players.Select(player => player.Name).ToArray();
        int[] handCards = _players.Select(player => player.Hand.Count).ToArray();
        string currentName = _currentPlayer.Name;
        Display.DrawDashboard(names, handCards, currentName, _currentDirection, _discardPile.Count, _drawPile.Cards.Count);
    }
    public bool CanPlayerCallUno()
    {
        return _currentPlayer.Hand.Count == 1 ? true : false;
    }
    public void HandleUnoCall(string[] input)
    {
        bool isUno = CanPlayerCallUno();
        if (input.Length <= 1 && isUno)
        {
            DrawCardToPlayer(_currentPlayer, 2);
            return;
        }
        if (input.Length <= 1) return;
        if ((isUno && input[1].ToLower() != "uno") || (!isUno && input[1].ToLower() == "uno"))
        {
            DrawCardToPlayer(_currentPlayer, 2);
            return;
        }
    }
}