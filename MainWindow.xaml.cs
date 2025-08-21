using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BlackJackGame.Client
{
    public partial class MainWindow : Window
    {
        // Base pack URI for embedded card resources
        private readonly string _packCardsBase = "pack://application:,,,/BlackJackGame.Client;component/Controls/Images/Cards/";

        // Game state
        private List<string> _deck = new List<string>();
        private List<string> _playerHand = new List<string>();
        private List<string> _dealerHand = new List<string>();
        private Random _rnd = new Random();

        private int accountBalance = 4000;
        private int currentBet = 0;
        private int _wins = 0, _losses = 0, _ties = 0;

        public object PlayerAvatarCenter { get; private set; }

        public MainWindow()
        {
            InitializeComponent();

            // An toàn: gỡ rồi gắn lại handlers để chắc chắn chúng được register
            try
            {
                if (HitButton != null) { HitButton.Click -= HitButton_Click; HitButton.Click += HitButton_Click; }
                if (StandButton != null) { StandButton.Click -= StandButton_Click; StandButton.Click += StandButton_Click; }
                if (DealButton != null) { DealButton.Click -= DealButton_Click; DealButton.Click += DealButton_Click; }
                if (LeaderboardButton != null) { LeaderboardButton.Click -= LeaderboardButton_Click; LeaderboardButton.Click += LeaderboardButton_Click; }

                // Chip buttons in XAML use Click="ChipButton_Click" — no Name; we rely on XAML hookup.
                // Clear button uses Click="ClearBet_Click" in XAML, handler implemented below.
            }
            catch
            {
                // ignore
            }

            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // prepare deck and UI
            BuildDeck();
            UpdateUI();
            TryLoadAvatar();
            TryLoadCardBacks();
        }

        #region Assets
        private void TryLoadAvatar()
        {
            try
            {
                // Optional avatar embedded or external not provided; ignore if missing
                // If you want a default avatar, add Controls/Images/Cards/player.png as Resource
                var p = _packCardsBase + "player.png";
                PlayerAvatar.Source = new BitmapImage(new Uri(p, UriKind.Absolute));
            }
            catch { /* ignore */ }
        }

        private void TryLoadCardBacks()
        {
            try
            {
                var cb = _packCardsBase + "back.png";
                var bmp = new BitmapImage(new Uri(cb, UriKind.Absolute));
                DeckBack1.Source = bmp;
                DeckBack2.Source = bmp;
                DeckBack3.Source = bmp;
            }
            catch { }
        }
        #endregion

        #region Deck & helpers
        private void BuildDeck()
        {
            _deck.Clear();
            // Build deck directly using embedded resource names
            string[] suits = { "hearts", "diamonds", "clubs", "spades" };
            string[] ranks = { "ace", "2", "3", "4", "5", "6", "7", "8", "9", "10", "jack", "queen", "king" };
            foreach (var s in suits)
                foreach (var r in ranks)
                    _deck.Add($"{r}_of_{s}.png");

            // shuffle
            _deck = _deck.OrderBy(x => _rnd.Next()).ToList();
        }

        private string DrawCardFromDeck()
        {
            if (_deck.Count == 0) BuildDeck();
            if (_deck.Count == 0) return null;
            var c = _deck[0];
            _deck.RemoveAt(0);
            return c;
        }

        private BitmapImage LoadImageByName(string fileName)
        {
            try
            {
                // Map to pack URI for embedded card assets
                string uri = fileName;
                if (!Uri.IsWellFormedUriString(fileName, UriKind.Absolute))
                {
                    uri = _packCardsBase + fileName;
                }
                var bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.UriSource = new Uri(uri, UriKind.Absolute);
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.EndInit();
                return bmp;
            }
            catch { return null; }
        }
        #endregion

        #region UI update & helpers
        private void UpdateUI()
        {
            AccountBalanceText.Text = $"${accountBalance}";
            MyBetText.Text = $"${currentBet}";
            WinsText.Text = _wins.ToString();
            LossesText.Text = _losses.ToString();
            TiesText.Text = _ties.ToString();
        }

        private void ClearCardsOnTable()
        {
            CardA.Source = null;
            CardB.Source = null;
            CardC.Source = null;
            CardImage.Source = null;
            // deck backs left as visual deck
        }
        #endregion

        #region Event Handlers (names exactly match XAML)
        // Chip buttons in XAML have Click="ChipButton_Click"
        private void ChipButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!(sender is Button b)) return;
                var tagOrContent = (b.Tag ?? b.Content)?.ToString();
                if (!int.TryParse(tagOrContent, out int value)) return;

                if (accountBalance >= value)
                {
                    currentBet += value;
                    accountBalance -= value;
                    Pulse(b);
                    UpdateUI();
                }
                else
                {
                    MessageBox.Show("Không đủ tiền để đặt cược.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ChipButton error: " + ex.Message);
            }
        }

        // Clear button in XAML uses Click="ClearBet_Click"
        private void ClearBet_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                accountBalance += currentBet;
                currentBet = 0;
                UpdateUI();
            }
            catch (Exception ex)
            {
                MessageBox.Show("ClearBet error: " + ex.Message);
            }
        }

        // Deal button (XAML: Click="DealButton_Click")
        private void DealButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (currentBet <= 0)
                {
                    MessageBox.Show("Vui lòng đặt cược trước khi Deal.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // prepare
                BuildDeck();
                _playerHand.Clear();
                _dealerHand.Clear();
                ClearCardsOnTable();

                // deal: player two cards, dealer two cards (one face down)
                _playerHand.Add(DrawCardFromDeck());
                _playerHand.Add(DrawCardFromDeck());

                _dealerHand.Add(DrawCardFromDeck()); // face up
                _dealerHand.Add(DrawCardFromDeck()); // face down

                // show player cards
                CardA.Source = LoadImageByName(_playerHand[0]) ?? LoadImageByName("back.png");
                CardB.Source = LoadImageByName(_playerHand[1]) ?? LoadImageByName("back.png");

                // dealer: show first, second as back
                DeckBack1.Source = LoadImageByName(_dealerHand[0]) ?? LoadImageByName("back.png");
                DeckBack2.Source = LoadImageByName("back.png");

                // reset possible other visuals
                DeckBack3.Source = null;
                CardC.Source = null;
                CardImage.Source = null;

                // small pulse
                Pulse(CardA);
                Pulse(CardB);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Deal error: " + ex.Message);
            }
        }

        // Hit button (XAML: Click="HitButton_Click")
        private void HitButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (currentBet <= 0)
                {
                    MessageBox.Show("Vui lòng đặt cược trước khi Hit.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var card = DrawCardFromDeck();
                if (card == null) return;

                _playerHand.Add(card);

                // show on next available player slot (CardC then CardImage)
                if (CardC.Source == null)
                    CardC.Source = LoadImageByName(card);
                else if (CardImage.Source == null)
                    CardImage.Source = LoadImageByName(card);

                // check bust
                int pval = CalculateHandValue(_playerHand);
                if (pval > 21)
                {
                    _losses++;
                    MessageBox.Show($"BUST! Bạn có {pval}. Bạn thua.", "Kết quả", MessageBoxButton.OK, MessageBoxImage.Information);
                    currentBet = 0;
                    UpdateUI();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hit error: " + ex.Message);
            }
        }

        // Stand button (XAML: Click="StandButton_Click")
        private void StandButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (currentBet <= 0)
                {
                    MessageBox.Show("Vui lòng đặt cược trước khi Stand.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // reveal dealer face-down card
                DeckBack2.Source = LoadImageByName(_dealerHand[1]) ?? LoadImageByName("back.png");

                // dealer draws until >=17
                while (CalculateHandValue(_dealerHand) < 17)
                {
                    var c = DrawCardFromDeck();
                    if (c == null) break;
                    _dealerHand.Add(c);

                    // show on DeckBack3 if available
                    if (DeckBack3.Source == null)
                    {
                        DeckBack3.Source = LoadImageByName(c);
                    }
                }

                // resolve
                int p = CalculateHandValue(_playerHand);
                int d = CalculateHandValue(_dealerHand);

                string msg;
                if (p > 21)
                {
                    msg = $"Bạn BUST ({p}) — Dealer thắng ({d}).";
                    _losses++;
                }
                else if (d > 21)
                {
                    msg = $"Dealer BUST ({d}) — Bạn thắng ({p})!";
                    _wins++;
                    accountBalance += currentBet * 2;
                }
                else if (p > d)
                {
                    msg = $"Bạn thắng ({p} vs {d})!";
                    _wins++;
                    accountBalance += currentBet * 2;
                }
                else if (p < d)
                {
                    msg = $"Dealer thắng ({d} vs {p}).";
                    _losses++;
                }
                else
                {
                    msg = $"Hòa ({p} vs {d}).";
                    _ties++;
                    accountBalance += currentBet; // return bet
                }

                MessageBox.Show(msg, "Kết quả", MessageBoxButton.OK, MessageBoxImage.Information);
                currentBet = 0;
                UpdateUI();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Stand error: " + ex.Message);
            }
        }

        // Leaderboard button
        private void LeaderboardButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var entries = LeaderboardService.Load();
                var w = new LeaderboardWindow(entries) { Owner = this };
                w.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể mở Leaderboard: " + ex.Message);
            }
        }
        #endregion

        #region Utilities: calculate hand value, pulse animation
        private int CalculateHandValue(List<string> hand)
        {
            int total = 0;
            int aces = 0;
            foreach (var f in hand)
            {
                string name = Path.GetFileNameWithoutExtension(f).ToUpper();
                // handle common patterns: A, 2..10, J,Q,K or words "ace", "jack" etc.
                if (name.StartsWith("A") || name.Contains("ACE")) { total += 11; aces++; }
                else if (name.StartsWith("J") || name.StartsWith("Q") || name.StartsWith("K") || name.Contains("KING") || name.Contains("QUEEN") || name.Contains("JACK")) { total += 10; }
                else
                {
                    // extract number 2..10
                    int num = 0;
                    foreach (var part in name.Split(new[] { '_', '-', ' ' }, StringSplitOptions.RemoveEmptyEntries))
                        if (int.TryParse(part, out num)) break;
                    total += (num == 0 ? 10 : num);
                }
            }

            while (total > 21 && aces > 0)
            {
                total -= 10;
                aces--;
            }
            return total;
        }

        // small pulse animation helper
        private void Pulse(UIElement el)
        {
            try
            {
                if (el == null) return;
                var st = el.RenderTransform as ScaleTransform;
                if (st == null)
                {
                    st = new ScaleTransform(1, 1);
                    el.RenderTransform = st;
                    el.RenderTransformOrigin = new Point(0.5, 0.5);
                }

                var a = new System.Windows.Media.Animation.DoubleAnimation(1, 1.12, TimeSpan.FromMilliseconds(140)) { AutoReverse = true };
                st.BeginAnimation(ScaleTransform.ScaleXProperty, a);
                st.BeginAnimation(ScaleTransform.ScaleYProperty, a);
            }
            catch { }
        }
        #endregion
    }
}
