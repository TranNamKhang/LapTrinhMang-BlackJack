public class Card
{
    public string Suit { get; set; }     // "Hearts", "Diamonds", etc.
    public string Rank { get; set; }     // "2" to "10", "J", "Q", "K", "A"

    public int Value
    {
        get
        {
            if (int.TryParse(Rank, out int number)) return number;
            if (Rank == "A") return 11;
            return 10; // J, Q, K
        }
    }

    public Card(string suit, string rank)
    {
        Suit = suit;
        Rank = rank;
    }

    public override string ToString() => $"{Rank} of {Suit}";
}
