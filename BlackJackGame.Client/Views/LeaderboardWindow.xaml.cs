using System.Collections.Generic;
using System.Windows;

namespace BlackJackGame.Client
{
    public partial class LeaderboardWindow : Window
    {
        public LeaderboardWindow(List<LeaderboardEntry> entries)
        {
            InitializeComponent();
            LoadLeaderboard();
        }

        private void LoadLeaderboard()
        {
            List<LeaderboardEntry> leaderboard = LeaderboardService.Load();
            LeaderboardGrid.ItemsSource = leaderboard;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
