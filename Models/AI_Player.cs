namespace BlackjackWPFGame.Models
{
    public class AIPlayer : Player
    {
        public AIPlayer(string name, int balance) : base(name, balance) { }

        public bool ShouldHit()
        {
            return GetHandValue() < 16;
        }
    }
}
