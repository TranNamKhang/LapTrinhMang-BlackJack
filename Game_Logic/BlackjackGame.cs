using System.Collections.Generic;
using System.Linq;
using BlackjackWPFGame.Models;

namespace BlackjackWPFGame.GameLogic
{
    public class BlackjackGame
    {
        public Deck Deck { get; private set; }
        public Dealer Dealer { get; private set; }
        public List<Player> Players { get; private set; }

        public BlackjackGame(List<Player> players)
        {
            Deck = new Deck();
            Dealer = new Dealer();
            Players = players;
        }

        public void StartRound()
        {
            Dealer.ClearHand();
            foreach (var player in Players)
            {
                player.ClearHand();
            }

            Deck = new Deck();

            for (int i = 0; i < 2; i++)
            {
                foreach (var player in Players)
                {
                    player.AddCard(Deck.DrawCard());
                }
                Dealer.AddCard(Deck.DrawCard());
            }
        }

        public void DealerPlay()
        {
            while (Dealer.ShouldHit())
            {
                Dealer.AddCard(Deck.DrawCard());
            }
        }

        public Player GetWinner()
        {
            int dealerValue = Dealer.GetHandValue();
            var winners = Players
                .Where(p => !p.IsBusted && (p.GetHandValue() > dealerValue || dealerValue > 21))
                .ToList();

            return winners.FirstOrDefault();
        }
    }
}
