using System.Collections.Concurrent;
using System.Linq;

namespace Blackjack.Shared;

public class Room
{
    private readonly List<Player> players = new();
    private readonly object syncRoot = new object();
    public int MaxPlayers { get; set; } = 4;
    public int MinPlayers { get; set; } = 2;
    public string GameStatus { get; set; } = "waiting"; // waiting, playing, ended
    public int CurrentPlayerIndex { get; set; } = 0;
    private readonly List<string> dealerHand = new();
    private readonly Stack<string> deck = new();

    // Thêm player vào phòng, trả về true nếu thành công
    public bool AddPlayer(Player p)
    {
        lock (syncRoot)
        {
            if (players.Count >= MaxPlayers) return false;
            players.Add(p);
            return true;
        }
    }

    public Player? GetCurrentPlayer()
    {
        lock (syncRoot)
        {
            if (players.Count == 0) return null;
            return players[CurrentPlayerIndex % players.Count];
        }
    }

    public void NextTurn()
    {
        lock (syncRoot)
        {
            if (players.Count == 0) return;
            CurrentPlayerIndex = (CurrentPlayerIndex + 1) % players.Count;
        }
    }

    public RoomState ToRoomState()
    {
        lock (syncRoot)
        {
            var current = GetCurrentPlayer();
            return new RoomState
            {
                Players = players.Select(x => new PlayerInfo
                {
                    Id = x.Id,
                    Name = x.Name,
                    Chips = x.Chips,
                    IsTurn = (x == current),
                    Hand = new List<string>(x.Hand),
                    Total = CalculateBestTotal(x.Hand),
                    HasStood = x.HasStood,
                    IsBusted = CalculateBestTotal(x.Hand) > 21
                }).ToList(),
                CurrentTurnId = current?.Id,
                GameStatus = GameStatus,
                DealerHand = new List<string>(dealerHand),
                DealerTotal = CalculateBestTotal(dealerHand),
                DeckCount = deck.Count
            };
        }
    }

    public int GetPlayerCount()
    {
        lock (syncRoot)
        {
            return players.Count;
        }
    }

    public List<Player> GetConnectedPlayersSnapshot()
    {
        lock (syncRoot)
        {
            return players.Where(p => p.Connected).ToList();
        }
    }

    public void StartGame()
    {
        lock (syncRoot)
        {
            if (players.Count >= MinPlayers)
            {
                GameStatus = "playing";
                CurrentPlayerIndex = 0;
                dealerHand.Clear();
                foreach (var p in players)
                {
                    p.Hand.Clear();
                    p.HasStood = false;
                }
                BuildAndShuffleDeck();
                // initial deal: two cards each, dealer 2 cards
                foreach (var p in players)
                {
                    p.Hand.Add(DrawCardInternal());
                }
                dealerHand.Add(DrawCardInternal());
                foreach (var p in players)
                {
                    p.Hand.Add(DrawCardInternal());
                }
                dealerHand.Add(DrawCardInternal());
            }
        }
    }

    public void PlayerHit(Player player)
    {
        lock (syncRoot)
        {
            if (GameStatus != "playing") return;
            if (!players.Contains(player)) return;
            if (player.HasStood) return;
            player.Hand.Add(DrawCardInternal());
        }
    }

    public void PlayerStand(Player player)
    {
        lock (syncRoot)
        {
            if (GameStatus != "playing") return;
            player.HasStood = true;
        }
    }

    public void ResolveIfNeeded()
    {
        lock (syncRoot)
        {
            if (GameStatus != "playing") return;
            bool allDone = players.All(p => p.HasStood || CalculateBestTotal(p.Hand) > 21);
            if (!allDone) return;

            // Dealer plays to 17+
            while (CalculateBestTotal(dealerHand) < 17)
            {
                dealerHand.Add(DrawCardInternal());
            }

            int dealerTotal = CalculateBestTotal(dealerHand);
            foreach (var p in players)
            {
                int total = CalculateBestTotal(p.Hand);
                if (total > 21)
                {
                    // bust, lose
                }
                else if (dealerTotal > 21 || total > dealerTotal)
                {
                    p.Chips += 100;
                }
                else if (total < dealerTotal)
                {
                    p.Chips -= 100;
                }
                else
                {
                    // push
                }
            }
            GameStatus = "ended";
        }
    }

    private void BuildAndShuffleDeck()
    {
        var cards = new List<string>();
        string[] ranks = new[] { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };
        string[] suits = new[] { "S", "H", "D", "C" };
        foreach (var s in suits)
        {
            foreach (var r in ranks)
            {
                cards.Add(r + s);
            }
        }
        // simple Fisher-Yates shuffle
        var rng = new Random();
        for (int i = cards.Count - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (cards[i], cards[j]) = (cards[j], cards[i]);
        }
        deck.Clear();
        for (int i = 0; i < cards.Count; i++) deck.Push(cards[i]);
    }

    private string DrawCardInternal()
    {
        if (deck.Count == 0) BuildAndShuffleDeck();
        return deck.Pop();
    }

    private static int CalculateBestTotal(IEnumerable<string> hand)
    {
        int total = 0;
        int aces = 0;
        foreach (var card in hand)
        {
            var rank = card.Trim().Length >= 2 ? new string(card.TakeWhile(char.IsLetterOrDigit).ToArray()) : card;
            switch (rank)
            {
                case "A":
                    aces++;
                    total += 11; // treat as 11 initially
                    break;
                case "K":
                case "Q":
                case "J":
                case "10":
                    total += 10;
                    break;
                default:
                    if (int.TryParse(rank, out int v)) total += v;
                    break;
            }
        }
        while (total > 21 && aces > 0)
        {
            total -= 10; // convert an Ace from 11 to 1
            aces--;
        }
        return total;
    }
}