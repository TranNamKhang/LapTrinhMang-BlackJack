using System.Collections.Generic;
using System.Linq;

namespace BlackJackGame.Client.Models
{
    public class Player
    {
        public string Name { get; set; }
        public List<Card> Hand { get; set; } = new List<Card>();
        public int Balance { get; set; } = 1000; // mặc định có 1000$
        public int Bet { get; set; }

        public void AddCard(Card card) => Hand.Add(card);

        public int GetScore()
        {
            int score = 0, aceCount = 0;
            foreach (var card in Hand)
            {
                if (int.TryParse(card.Rank, out int val)) score += val;
                else if (card.Rank == "A") { score += 11; aceCount++; }
                else score += 10;
            }

            // nếu quá 21 thì A = 1
            while (score > 21 && aceCount > 0)
            {
                score -= 10;
                aceCount--;
            }

            return score;
        }

        public void ResetHand() => Hand.Clear();
    }
}
