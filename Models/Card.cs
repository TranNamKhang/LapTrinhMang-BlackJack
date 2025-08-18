using System;
using System.Collections.Generic;
using System.Linq;

namespace BlackJackGame
{
    public class Player
    {
        // ===== Thuộc tính cơ bản =====
        public string Name { get; set; }         // Tên người chơi
        public int Chips { get; set; }           // Số chip hiện có
        public int CurrentBet { get; set; }      // Tiền cược ván hiện tại
        public List<Card> Hand { get; set; }     // Bộ bài đang có
        public bool IsDealer { get; set; }       // Có phải Dealer không
        public bool IsAI { get; set; }           // Có phải AI không

        // ===== Thuộc tính phục vụ UI (binding WPF) =====
        public string Avatar { get; set; }       // Ảnh đại diện
        public bool IsBusted => GetHandValue() > 21;
        public bool HasBlackjack => GetHandValue() == 21 && Hand.Count == 2;

        // ===== Constructor =====
        public Player(string name, int chips = 1000, bool isDealer = false, bool isAI = false)
        {
            Name = name;
            Chips = chips;
            IsDealer = isDealer;
            IsAI = isAI;
            Hand = new List<Card>();
        }

        // ===== Các hành động game =====
        public void PlaceBet(int amount)
        {
            if (amount > Chips)
                throw new Exception($"{Name} không đủ chip để cược!");
            
            CurrentBet = amount;
            Chips -= amount;
        }

        public void Hit(Deck deck)
        {
            Hand.Add(deck.DrawCard());
        }

        public void Stand()
        {
            // Dừng rút bài (có thể xử lý thêm trong Game.cs)
        }

        public void ClearHand()
        {
            Hand.Clear();
            CurrentBet = 0;
        }

        public int GetHandValue()
        {
            int value = Hand.Sum(c => c.Value);
            int aceCount = Hand.Count(c => c.Rank == "A");

            while (value > 21 && aceCount > 0)
            {
                value -= 10; // Chuyển 1 lá A từ 11 thành 1
                aceCount--;
            }

            return value;
        }

        public void WinBet()
        {
            Chips += CurrentBet * 2;
            CurrentBet = 0;
        }

        public void PushBet()
        {
            Chips += CurrentBet;
            CurrentBet = 0;
        }

        public void LoseBet()
        {
            CurrentBet = 0;
        }

        // ===== AI Strategy cơ bản =====
        public string GetAIMove(Deck deck)
        {
            int value = GetHandValue();
            if (value < 17)
            {
                Hit(deck);
                return "Hit";
            }
            else
            {
                Stand();
                return "Stand";
            }
        }
    }
}
