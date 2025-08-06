using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media;

public partial class MainWindow : Window
{
    private BlackjackGame game;

    public MainWindow()
    {
        InitializeComponent();
        game = new BlackjackGame("You");
        game.Start();
        UpdateUI();
    }

    private void BtnHit_Click(object sender, RoutedEventArgs e)
    {
        var card = game.Deck.Draw();
        game.Player.Hit(card);
        AddCardToUI(PlayerPanel, card);
        UpdateScores();
        CheckEnd();
    }

    private void BtnStand_Click(object sender, RoutedEventArgs e)
    {
        game.Player.IsStand = true;
        var ai = new BlackjackAI();
        while (ai.ShouldHit(game.Dealer.GetScore()))
        {
            var card = game.Deck.Draw();
            game.Dealer.Hit(card);
            AddCardToUI(DealerPanel, card);
        }
        UpdateScores();
        ShowResult(game.GetWinner());
    }

    private void UpdateUI()
    {
        PlayerPanel.Children.Clear();
        DealerPanel.Children.Clear();
        foreach (var c in game.Player.Hand)
            AddCardToUI(PlayerPanel, c);
        foreach (var c in game.Dealer.Hand)
            AddCardToUI(DealerPanel, c);
        UpdateScores();
    }

    private void AddCardToUI(StackPanel panel, Card card)
    {
        var img = new Image
        {
            Width = 60,
            Height = 90,
            Margin = new Thickness(5),
            Source = new BitmapImage(new Uri($"pack://application:,,,/Images/{card.Rank}_of_{card.Suit}.png"))
        };

        var sb = new Storyboard();
        var anim = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromSeconds(0.3)));
        Storyboard.SetTarget(anim, img);
        Storyboard.SetTargetProperty(anim, new PropertyPath(UIElement.OpacityProperty));
        sb.Children.Add(anim);
        panel.Children.Add(img);
        sb.Begin();
    }

    private void UpdateScores()
    {
        TxtPlayerScore.Text = $"Player: {game.Player.GetScore()}";
        TxtDealerScore.Text = $"Dealer: {game.Dealer.GetScore()}";
    }

    private void CheckEnd()
    {
        if (game.Player.GetScore() > 21)
        {
            ShowResult("Dealer");
        }
    }

    private void ShowResult(string winner)
    {
        MessageBox.Show($"{winner} wins!", "Game Over", MessageBoxButton.OK);
        BtnHit.IsEnabled = false;
        BtnStand.IsEnabled = false;
    }
}