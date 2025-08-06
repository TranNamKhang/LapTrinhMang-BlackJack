// Partial class structure for BlackjackGame and supporting classes
public class BlackjackGame
{
    public Deck Deck { get; private set; }
    public Player Player { get; private set; }
    public Player Dealer { get; private set; }

    public BlackjackGame(string playerName)
    {
        Deck = new Deck();
        Player = new Player(playerName);
        Dealer = new Player("Dealer");
    }

    public void Start()
    {
        Player.Reset();
        Dealer.Reset();
        Deck.Shuffle();
        Player.Hit(Deck.Draw());
        Player.Hit(Deck.Draw());
        Dealer.Hit(Deck.Draw());
        Dealer.Hit(Deck.Draw());
    }

    public string GetWinner()
    {
        int playerScore = Player.GetScore();
        int dealerScore = Dealer.GetScore();

        if (playerScore > 21) return "Dealer";
        if (dealerScore > 21) return "Player";
        if (playerScore > dealerScore) return "Player";
        if (dealerScore > playerScore) return "Dealer";
        return "Draw";
    }
}

// Add Player, Deck, Card, BlackjackAI class definitions here.