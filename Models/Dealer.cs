namespace BlackjackWPFGame.Models
{
    public class Dealer : Player
    {
        public Dealer() : base("Dealer", 0) { }

        public bool ShouldHit()
        {
            return GetHandValue() < 17;
        }
    }
}
