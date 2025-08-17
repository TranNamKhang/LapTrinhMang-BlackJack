namespace BlackJackGame.Client.Models
{
    // Use numeric values for ranks 2..10, A=1 (we treat A as 11 in Card.GetValue)
    public enum CardRank
    {
        A = 1,
        _2 = 2,
        _3 = 3,
        _4 = 4,
        _5 = 5,
        _6 = 6,
        _7 = 7,
        _8 = 8,
        _9 = 9,
        _10 = 10,
        J = 11,
        Q = 12,
        K = 13
    }
}
