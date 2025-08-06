using System;
using System.Collections.Generic;

public class Deck
{
    private Stack<Card> cards;

    public Deck()
    {
        cards = new Stack<Card>();
        Initialize();
        Shuffle();
    }

    private void Initialize()
    {
        string[] suits = { "Hearts", "Diamonds", "Clubs", "Spades" };
        string[] ranks = { "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A" };

        foreach (var suit in suits)
        {
            foreach (var rank in ranks)
            {
                cards.Push(new Card(suit, rank));
            }
        }
    }

    public void Shuffle()
    {
        var cardList = new List<Card>(cards);
        var rng = new Random();
        for (int i = cardList.Count - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (cardList[i], cardList[j]) = (cardList[j], cardList[i]);
        }
        cards = new Stack<Card>(cardList);
    }

    public Card DrawCard()
    {
        if (cards.Count == 0)
        {
            Initialize();
            Shuffle();
        }
        return cards.Pop();
    }
}
