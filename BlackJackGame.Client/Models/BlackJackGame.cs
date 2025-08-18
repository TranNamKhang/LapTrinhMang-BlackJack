using System;

namespace BlackJackGame.Client.Models
{
    public class BlackjackGame
    {
        public Deck Deck { get; private set; }
        public Player Player { get; private set; }
        public Dealer Dealer { get; private set; }

        public BlackjackGame(string playerName)
        {
            Deck = new Deck();
            Player = new Player { Name = playerName, Balance = 1000 }; // Use object initializer
            Dealer = new Dealer { Name = "Dealer" };
        }

        public void StartRound(int bet)
        {
            Player.ResetHand();
            Dealer.ResetHand();
            Deck = new Deck();

            Player.Bet = bet;
            Player.Balance -= bet;

            Player.AddCard(Deck.DrawCard());
            Player.AddCard(Deck.DrawCard());
            Dealer.AddCard(Deck.DrawCard());
            Dealer.AddCard(Deck.DrawCard());
        }

        public string DetermineWinner()
        {
            int playerScore = Player.GetScore();
            int dealerScore = Dealer.GetScore();

            if (playerScore > 21) return "Dealer wins!";
            if (dealerScore > 21) { Player.Balance += Player.Bet * 2; return "Player wins!"; }
            if (playerScore > dealerScore) { Player.Balance += Player.Bet * 2; return "Player wins!"; }
            if (playerScore == dealerScore) { Player.Balance += Player.Bet; return "Draw!"; }
            return "Dealer wins!";
        }
    }
}
