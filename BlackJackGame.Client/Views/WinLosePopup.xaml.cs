using System.Windows;

namespace BlackJackGame.Client.Views
{
    public partial class WinLosePopup : Window
    {
        public WinLosePopup(string message, bool isWin = true)
        {
            InitializeComponent();
            ResultText.Text = message;
            ResultText.Foreground = isWin ? System.Windows.Media.Brushes.Green : System.Windows.Media.Brushes.Red;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
