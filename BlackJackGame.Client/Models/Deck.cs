using System;
using System.Collections.Generic;

namespace BlackJackGame.Client.GameLogic
{
    public class Deck
    {
        private List<Card> cards;
        private Random random = new Random();

        public Deck()
        {
            cards = new List<Card>();
            string[] suits = { "Hearts", "Diamonds", "Clubs", "Spades" };
            string[] ranks = { "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A" };

            foreach (var suit in suits)
                foreach (var rank in ranks)
                    cards.Add(new Card(rank, suit));
        }

        public Card DrawCard()
        {
            if (cards.Count == 0) return null;
            int index = random.Next(cards.Count);
            Card card = cards[index];
            cards.RemoveAt(index);
            return card;
        }
    }
}
