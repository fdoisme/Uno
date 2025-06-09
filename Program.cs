using Uno.controllers;
using Uno.Interfaces;
using Uno.models;
using Uno.utils;
using Uno.views;

Display.DrawWelcome();
Display.DrawLoading();
List<IPlayer> players = new List<IPlayer>();
Console.Clear();
int totalPlayer = 0;
while (true)
{
    Console.WriteLine("Welcome to the Uno Game!");
    Console.WriteLine("1. 2-Player Game");
    Console.WriteLine("2. 3-Player Game");
    Console.WriteLine("3. 4-Player Game");
    Console.Write("Pilih Jumlah Pemain (1-3): ");
    string? manyPlayer = Console.ReadLine();
    if (int.TryParse(manyPlayer, out totalPlayer) && totalPlayer <= 3 && totalPlayer > 0)
    {
        break;
    }
    Console.WriteLine("Invalid Input");
}
for (int i = 0; i <= totalPlayer; i++)
{
    Console.Write($"Masukkan nama Player{i + 1}: ");
    string? playerName = Console.ReadLine();
    playerName = string.IsNullOrWhiteSpace(playerName) ? $"Player {i + 1}" : playerName;
    players.Add(new Player(playerName));
}
GameController gameController = new GameController(players);
gameController.StartGame();
Console.Clear();
Console.WriteLine(Frames.gameOver);