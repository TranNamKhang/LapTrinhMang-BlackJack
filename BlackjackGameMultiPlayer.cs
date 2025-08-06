using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.IO;

public class BlackjackGame
{
    public List<Player> Players { get; private set; }
    public Dealer Dealer { get; private set; }
    public Deck Deck { get; private set; }
    public int CurrentTurnIndex { get; private set; }

    public BlackjackGame(List<string> playerNames)
    {
        Players = playerNames.Select(name => new Player(name)).ToList();
        Dealer = new Dealer();
        Deck = new Deck();
        CurrentTurnIndex = 0;
        StartNewRound();
    }

    public void StartNewRound()
    {
        Deck.Shuffle();
        foreach (var player in Players)
        {
            player.ResetHand();
            player.PlaceBet(100); // default bet, can be customized
            player.Hit(Deck.DrawCard());
            player.Hit(Deck.DrawCard());
        }
        Dealer.ResetHand();
        Dealer.Hit(Deck.DrawCard());
        Dealer.Hit(Deck.DrawCard());
        CurrentTurnIndex = 0;
    }

    public Player GetCurrentPlayer() => Players[CurrentTurnIndex];

    public void NextTurn()
    {
        CurrentTurnIndex++;
        if (CurrentTurnIndex >= Players.Count)
        {
            DealerPlay();
            EvaluateResults();
        }
    }

    public void DealerPlay()
    {
        while (Dealer.HandValue < 17)
        {
            Dealer.Hit(Deck.DrawCard());
        }
    }

    public void EvaluateResults()
    {
        foreach (var player in Players)
        {
            if (player.IsBusted)
                player.Lose();
            else if (Dealer.IsBusted || player.HandValue > Dealer.HandValue)
                player.Win();
            else if (player.HandValue == Dealer.HandValue)
                player.Draw();
            else
                player.Lose();
        }
        SaveStats();
    }

    public void SaveStats()
    {
        var stats = LoadStats();
        foreach (var player in Players)
        {
            var existing = stats.FirstOrDefault(s => s.Name == player.Name);
            if (existing == null)
            {
                stats.Add(new PlayerStats
                {
                    Name = player.Name,
                    Wins = player.Wins,
                    Losses = player.Losses
                });
            }
            else
            {
                existing.Wins = player.Wins;
                existing.Losses = player.Losses;
            }
        }
        File.WriteAllText("stats.json", JsonSerializer.Serialize(stats));
    }

    public List<PlayerStats> LoadStats()
    {
        if (!File.Exists("stats.json")) return new List<PlayerStats>();
        return JsonSerializer.Deserialize<List<PlayerStats>>(File.ReadAllText("stats.json"));
    }
}

public class PlayerStats
{
    public string Name { get; set; }
    public int Wins { get; set; }
    public int Losses { get; set; }
}

public class Player
{
    public string Name { get; set; }
    public List<Card> Hand { get; private set; }
    public int Chips { get; set; } = 1000;
    public int Bet { get; set; }
    public int Wins { get; set; }
    public int Losses { get; set; }

    public Player(string name)
    {
        Name = name;
        Hand = new List<Card>();
    }

    public void ResetHand()
    {
        Hand.Clear();
    }

    public void Hit(Card card)
    {
        Hand.Add(card);
    }

    public void PlaceBet(int amount)
    {
        if (amount <= Chips)
        {
            Bet = amount;
            Chips -= amount;
        }
    }

    public void Win()
    {
        Chips += Bet * 2;
        Wins++;
        Bet = 0;
    }

    public void Lose()
    {
        Losses++;
        Bet = 0;
    }

    public void Draw()
    {
        Chips += Bet;
        Bet = 0;
    }

    public int HandValue => CalculateHandValue();
    public bool IsBusted => HandValue > 21;

    private int CalculateHandValue()
    {
        int value = Hand.Sum(c => c.Value);
        int aceCount = Hand.Count(c => c.Rank == "A");
        while (value > 21 && aceCount > 0)
        {
            value -= 10;
            aceCount--;
        }
        return value;
    }
}

public class AIPlayer : Player
{
    public AIPlayer(string name) : base(name) { }

    public string DecideMove(int dealerUpCard)
    {
        int score = this.HandValue;

        if (score <= 11) return "Hit";
        if (score >= 17) return "Stand";
        if (score >= 12 && score <= 16 && dealerUpCard >= 7) return "Hit";
        return "Stand";
    }
}


