using System.Windows;
using System.Windows.Controls;

namespace BlackJackGame.Client.Views
{
    public partial class BetWindow : Window
    {
        public int SelectedBet { get; private set; } = 0;

        public BetWindow()
        {
            InitializeComponent();
        }

        private void Chip_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                if (int.TryParse(btn.Content.ToString(), out int bet))
                {
                    SelectedBet = bet;
                    BetAmountText.Text = SelectedBet.ToString();
                }
            }
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedBet > 0)
            {
                DialogResult = true;
            }
            else
            {
                MessageBox.Show("Please select a bet amount before confirming.");
            }
        }
    }
}
