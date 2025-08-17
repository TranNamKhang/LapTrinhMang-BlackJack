namespace BlackJackGame.Client.Models
{
    public class Card
    {
        private CardSuit suit;
        private CardRank rank;

        public string Suit { get; set; }    // ♠, ♥, ♦, ♣
        public string Rank { get; set; }    // A, 2..10, J, Q, K
        public int Value { get; set; }      // Giá trị dùng tính điểm
        public string ImagePath { get; set; } // Đường dẫn ảnh lá bài

        public Card(string suit, string rank, int value, string imagePath)
        {
            Suit = suit;
            Rank = rank;
            Value = value;
            ImagePath = imagePath;
        }

        public Card(CardSuit suit, CardRank rank)
        {
            this.suit = suit;
            this.rank = rank;
        }
    }
}
