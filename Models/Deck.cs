using System;
using System.Collections.Generic;

namespace BlackjackWPFGame.Models
{
    public class Deck
    {
        private List<Card> cards;
        private Random random;

        public Deck()
        {
            random = new Random();
            InitializeDeck();
            Shuffle();
        }

        private void InitializeDeck()
        {
            cards = new List<Card>();
            string[] suits = { "Hearts", "Diamonds", "Clubs", "Spades" };
            string[] ranks = { "2", "3", "4", "5", "6", "7", "8", "9", "10", "Jack", "Queen", "King", "Ace" };
            int[] values = { 2, 3, 4, 5, 6, 7, 8, 9, 10, 10, 10, 10, 11 };

            foreach (var suit in suits)
            {
                for (int i = 0; i < ranks.Length; i++)
                {
                    cards.Add(new Card(suit, ranks[i], values[i]));
                }
            }
        }

        public void Shuffle()
        {
            for (int i = cards.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                var temp = cards[i];
                cards[i] = cards[j];
                cards[j] = temp;
            }
        }

        public Card DrawCard()
        {
            if (cards.Count == 0)
            {
                InitializeDeck();
                Shuffle();
            }
            Card card = cards[0];
            cards.RemoveAt(0);
            return card;
        }
    }
}
