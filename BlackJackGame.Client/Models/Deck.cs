using System;
using System.Collections.Generic;

namespace BlackJackGame.Client.Models
{
    public class Deck
    {
        private List<Card> cards;
        private Random random = new Random();

        public Deck()
        {
            cards = new List<Card>();

            foreach (CardSuit suit in Enum.GetValues(typeof(CardSuit)))
            {
                foreach (CardRank rank in Enum.GetValues(typeof(CardRank)))
                {
                    string suitSymbol = GetSuitSymbol(suit);
                    string rankText = GetRankText(rank);
                    int value = GetCardValue(rank);
                    string imagePath = "/Images/" + rankText + "_of_" + suit + ".png";

                    cards.Add(new Card(suitSymbol, rankText, value, imagePath));
                }
            }
        }

        private string GetSuitSymbol(CardSuit suit)
        {
            switch (suit)
            {
                case CardSuit.Hearts: return "♥";
                case CardSuit.Diamonds: return "♦";
                case CardSuit.Clubs: return "♣";
                case CardSuit.Spades: return "♠";
                default: return "?";
            }
        }

        private string GetRankText(CardRank rank)
        {
            switch (rank)
            {
                case CardRank.A: return "A";
                case CardRank._2: return "2";
                case CardRank._3: return "3";
                case CardRank._4: return "4";
                case CardRank._5: return "5";
                case CardRank._6: return "6";
                case CardRank._7: return "7";
                case CardRank._8: return "8";
                case CardRank._9: return "9";
                case CardRank._10: return "10";
                case CardRank.J: return "J";
                case CardRank.Q: return "Q";
                case CardRank.K: return "K";
                default: return "?";
            }
        }

        private int GetCardValue(CardRank rank)
        {
            switch (rank)
            {
                case CardRank.A: return 11;
                case CardRank.J:
                case CardRank.Q:
                case CardRank.K:
                case CardRank._10: return 10;
                default: return (int)rank;
            }
        }

        public Card DrawCard()
        {
            if (cards.Count == 0) return null;
            int index = random.Next(cards.Count);
            Card card = cards[index];
            cards.RemoveAt(index);
            return card;
        }

        public int Count
        {
            get { return cards.Count; }
        }
    }
}
