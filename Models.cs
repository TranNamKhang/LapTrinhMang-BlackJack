using System;
using System.Collections.Generic;

namespace BlackJackGame.Shared
{
    public enum Suit
    {
        Hearts,    // ♥
        Diamonds,  // ♦
        Clubs,     // ♣
        Spades     // ♠
    }

    public enum Rank
    {
        Two = 2,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Ten,
        Jack,
        Queen,
        King,
        Ace
    }

    public class Card
    {
        public Suit Suit { get; set; }
        public Rank Rank { get; set; }
        public string ImagePath { get; set; } // Đường dẫn hình ảnh thẻ bài

        public int GetValue()
        {
            if (Rank >= Rank.Two && Rank <= Rank.Ten)
                return (int)Rank;
            if (Rank == Rank.Ace)
                return 11; // Ace có thể 1 hoặc 11, tuỳ logic sau này
            return 10; // J, Q, K
        }

        public override string ToString()
        {
            return $"{Rank} of {Suit}";
        }
    }

    public class Player
    {
        public string Name { get; set; }
        public List<Card> Hand { get; set; } = new List<Card>();
        public int Chips { get; set; } = 1000;
        public int CurrentBet { get; set; }
        public bool IsDealer { get; set; }

        public int GetHandValue()
        {
            int value = 0;
            int aceCount = 0;

            foreach (var card in Hand)
            {
                value += card.GetValue();
                if (card.Rank == Rank.Ace)
                    aceCount++;
            }

            // Giảm Ace từ 11 → 1 nếu quá 21
            while (value > 21 && aceCount > 0)
            {
                value -= 10;
                aceCount--;
            }

            return value;
        }
    }

    public class GameState
    {
        public List<Player> Players { get; set; } = new List<Player>();
        public List<Card> Deck { get; set; } = new List<Card>();
        public bool IsGameOver { get; set; }
    }

    public class LeaderboardEntry
    {
        public string PlayerName { get; set; }
        public int Score { get; set; }
        public DateTime Date { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int Ties { get; set; }
    }
}
