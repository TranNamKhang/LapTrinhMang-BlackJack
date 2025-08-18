using System.Windows;

namespace BlackJackGame.Client
{
    public partial class ResultPopup : Window
    {
        public ResultPopup(string result)
        {
            InitializeComponent();
            SetResult(result);
        }

        private void SetResult(string result)
        {
            switch (result)
            {
                case "Win":
                    ResultText.Text = "🏆 You Win! 🏆";
                    ResultText.Foreground = System.Windows.Media.Brushes.LimeGreen;
                    break;
                case "Lose":
                    ResultText.Text = "💔 You Lose! 💔";
                    ResultText.Foreground = System.Windows.Media.Brushes.Red;
                    break;
                case "Draw":
                    ResultText.Text = "🤝 It's a Draw! 🤝";
                    ResultText.Foreground = System.Windows.Media.Brushes.Gold;
                    break;
                default:
                    ResultText.Text = "🎮 Game Over 🎮";
                    ResultText.Foreground = System.Windows.Media.Brushes.White;
                    break;
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
