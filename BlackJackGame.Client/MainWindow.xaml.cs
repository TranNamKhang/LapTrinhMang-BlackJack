using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using BlackJackGame.Client.Views;

namespace BlackJackGame.Client
{
    public partial class MainWindow : Window
    {
        private readonly string[] _deck = { "AS", "KH", "7D", "3C", "10S", "QD", "5H" };
        private readonly Random _random = new Random();
        private string[] _cardFiles;
        private int currentBet = 0;

        public MainWindow()
        {
            InitializeComponent();

            string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "Cards");

            if (Directory.Exists(folderPath))
            {
                _cardFiles = Directory.GetFiles(folderPath, "*.*")
                                      .Where(f => f.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                                                  f.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase))
                                      .ToArray();
            }
            else
            {
                MessageBox.Show("Không tìm thấy thư mục: " + folderPath);
                _cardFiles = Array.Empty<string>();
            }
        }

        private void FlipCards_Click(object sender, RoutedEventArgs e)
        {
            if (_cardFiles.Length > 0)
            {
                CardA.FlipCard(GetRandomCardSymbol(), GetRandomCardImage());
                CardB.FlipCard(GetRandomCardSymbol(), GetRandomCardImage());
                CardC.FlipCard(GetRandomCardSymbol(), GetRandomCardImage());
            }
            else
            {
                MessageBox.Show("Không có ảnh lá bài để lật!");
            }
        }

        private void PlaceBet_Click(object sender, RoutedEventArgs e)
        {
            var betWindow = new BetWindow { Owner = this };
            if (betWindow.ShowDialog() == true)
            {
                currentBet = betWindow.SelectedBet;
                MessageBox.Show($"Bạn đã đặt cược: {currentBet}$");
            }
        }

        private void DrawRandomCard_Click(object sender, RoutedEventArgs e)
        {
            if (_cardFiles.Length > 0)
            {
                string randomCard = _cardFiles[_random.Next(_cardFiles.Length)];
                CardImage.Source = new BitmapImage(new Uri(randomCard, UriKind.Absolute));
            }
            else
            {
                MessageBox.Show("Không có ảnh lá bài để hiển thị!");
            }
        }

        private string GetRandomCardSymbol() => _deck[_random.Next(_deck.Length)];
        private string GetRandomCardImage() => _cardFiles[_random.Next(_cardFiles.Length)];
    }
}
