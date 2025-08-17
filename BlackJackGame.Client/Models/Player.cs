using System.Collections.Generic;

namespace BlackJackGame.Client.Models
{
    public class Player
    {
        public string Name { get; set; }
        public int Chips { get; set; }
        public int Score { get; set; }
        public bool IsDealer { get; set; }
        public List<Card> Hand { get; set; }

        public Player()
        {
            Hand = new List<Card>();
        }

        public void ResetHand()
        {
            Hand.Clear();
            Score = 0;
        }
    }
}
