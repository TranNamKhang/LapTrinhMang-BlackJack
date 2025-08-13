using System.Collections.Generic;

public class Dealer : Player
{
    public Dealer() : base("Dealer") { }

    public void PlayTurn(Deck deck)
    {
        while (HandValue < 17)
        {
            Hit(deck.DrawCard());
        }
    }
}
