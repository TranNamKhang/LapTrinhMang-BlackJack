using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace BlackJackGame.Client.Controls
{
    public partial class CardControl : UserControl
    {
        private bool isFlipped = false;

        public CardControl()
        {
            InitializeComponent();
        }

        public void FlipCard(string symbol, string imagePath)
        {
            CardSymbol.Text = symbol;
            FrontBrush.Visual = new Image
            {
                Source = new BitmapImage(new Uri(imagePath, UriKind.Absolute)),
                Stretch = System.Windows.Media.Stretch.UniformToFill
            };

            double from = isFlipped ? 180 : 0;
            double to = isFlipped ? 0 : 180;

            var animation = new DoubleAnimation
            {
                From = from,
                To = to,
                Duration = new Duration(TimeSpan.FromSeconds(0.5)),
                AutoReverse = false
            };

            cardRotation.BeginAnimation(AxisAngleRotation3D.AngleProperty, animation);
            isFlipped = !isFlipped;
        }
    }
}
