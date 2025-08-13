using System.Collections.Generic;
using System.Linq;

namespace BlackjackWPFGame.Models
{
    public class Player
    {
        public string Name { get; set; }
        public List<Card> Hand { get; private set; }
        public int Balance { get; set; }
        public int BetAmount { get; set; }
        public bool IsBusted => GetHandValue() > 21;

        public Player(string name, int balance)
        {
            Name = name;
            Balance = balance;
            Hand = new List<Card>();
        }

        public void AddCard(Card card)
        {
            Hand.Add(card);
        }

        public void ClearHand()
        {
            Hand.Clear();
        }

        public int GetHandValue()
        {
            int value = Hand.Sum(card => card.Value);
            int aceCount = Hand.Count(c => c.Rank == "Ace");

            while (value > 21 && aceCount > 0)
            {
                value -= 10;
                aceCount--;
            }
            return value;
        }
    }
}
