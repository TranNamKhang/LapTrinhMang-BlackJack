using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace BlackJackGame.Client
{
    public partial class CardControl : UserControl
    {
        public CardControl()
        {
            InitializeComponent();
        }

        public void SetSource(string path)
        {
            try { CardImage.Source = new BitmapImage(new Uri(path, UriKind.RelativeOrAbsolute)); } catch { }
        }

        public void FlipTo(string newImagePath, int durationMs = 420)
        {
            var easeIn = new SineEase { EasingMode = EasingMode.EaseIn };
            var easeOut = new SineEase { EasingMode = EasingMode.EaseOut };

            var toHalf = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(durationMs / 2)) { EasingFunction = easeIn };
            toHalf.Completed += (s, e) =>
            {
                try { CardImage.Source = new BitmapImage(new Uri(newImagePath, UriKind.RelativeOrAbsolute)); } catch { }
                Skew.BeginAnimation(SkewTransform.AngleYProperty, new DoubleAnimation(-6, 0, TimeSpan.FromMilliseconds(durationMs / 2)) { EasingFunction = easeOut });
                Scale.BeginAnimation(ScaleTransform.ScaleXProperty, new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(durationMs / 2)) { EasingFunction = easeOut });
            };
            Skew.BeginAnimation(SkewTransform.AngleYProperty, new DoubleAnimation(0, 6, TimeSpan.FromMilliseconds(durationMs / 2)) { EasingFunction = easeIn });
            Scale.BeginAnimation(ScaleTransform.ScaleXProperty, toHalf);
        }
    }
}
