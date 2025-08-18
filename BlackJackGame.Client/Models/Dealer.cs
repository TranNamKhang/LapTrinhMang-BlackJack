namespace BlackJackGame.Client.Models
{
    public class Dealer : Player
    {
        public void Play(Deck deck)
        {
            while (GetScore() < 17)
                AddCard(deck.DrawCard());
        }
    }
}
