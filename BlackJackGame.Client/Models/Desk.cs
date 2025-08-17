using System;
using System.Collections.Generic;
using System.Linq;

namespace BlackJackGame.Client.Models
{
    public class Desk
    {
        private readonly List<Card> _cards;
        private readonly Random _rng;

        public Desk()
        {
            _cards = new List<Card>();
            _rng = new Random();
            InitializeDeck();
            Shuffle();
        }

        private void InitializeDeck()
        {
            _cards.Clear();
            foreach (CardSuit suit in Enum.GetValues(typeof(CardSuit)))
            {
                // iterate ranks we defined
                foreach (CardRank rank in Enum.GetValues(typeof(CardRank)))
                {
                    var card = new Card(suit, rank);
                    // optional: set ImagePath if you have naming scheme
                    // card.ImagePath = $"/Images/Cards/{GetFileNameFor(rank, suit)}.png";
                    _cards.Add(card);
                }
            }
        }

        // Fisher–Yates shuffle (efficient, doesn't reassign readonly field)
        public void Shuffle()
        {
            int n = _cards.Count;
            for (int i = n - 1; i > 0; i--)
            {
                int j = _rng.Next(i + 1);
                var tmp = _cards[i];
                _cards[i] = _cards[j];
                _cards[j] = tmp;
            }
        }

        public Card DrawCard()
        {
            if (_cards.Count == 0) return null;
            var c = _cards[0];
            _cards.RemoveAt(0);
            return c;
        }

        // optional utility
        public IReadOnlyList<Card> Cards => _cards.AsReadOnly();
    }
}
