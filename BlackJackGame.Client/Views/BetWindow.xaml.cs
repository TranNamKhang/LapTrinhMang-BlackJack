using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace BlackJackGame.Client
{
    public partial class BetWindow : Window
    {
        public int SelectedBet { get; private set; } = 0;

        public BetWindow()
        {
            InitializeComponent();
            Loaded += (s, e) => FadeIn();
        }

        private void FadeIn()
        {
            var anim = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(180));
            BeginAnimation(OpacityProperty, anim);
            Pulse(this);
        }

        private void Pulse(FrameworkElement fe)
        {
            fe.RenderTransformOrigin = new Point(0.5, 0.5);
            var st = new ScaleTransform(1, 1);
            fe.RenderTransform = st;

            var up = new DoubleAnimation(0.96, 1, TimeSpan.FromMilliseconds(220)) { EasingFunction = new BackEase() };
            st.BeginAnimation(ScaleTransform.ScaleXProperty, up);
            st.BeginAnimation(ScaleTransform.ScaleYProperty, up);
        }

        private void Chip_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement fe && int.TryParse(fe.Tag?.ToString(), out int bet))
            {
                SelectedBet = bet;
            }
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedBet > 0) { DialogResult = true; Close(); }
            else MessageBox.Show("Vui lòng chọn số tiền cược!");
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false; Close();
        }
    }
}
