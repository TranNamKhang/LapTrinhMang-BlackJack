using System;

namespace BlackjackWPFGame.Models
{
    public class Card
    {
        public string Suit { get; private set; }
        public string Rank { get; private set; }
        public int Value { get; private set; }

        public Card(string suit, string rank, int value)
        {
            Suit = suit;
            Rank = rank;
            Value = value;
        }

        public override string ToString()
        {
            return $"{Rank} of {Suit}";
        }
    }
}
