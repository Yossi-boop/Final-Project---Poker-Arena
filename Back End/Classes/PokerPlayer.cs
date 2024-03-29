﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UsefullMethods;

namespace Classes
{
    public class PokerPlayer
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public string Signature { get; set; }
        // Player money in the game
        public int Money { get; set; }

        public int Figure { get; set; }

        // The amount of money the player is currently put in the pot
        public int CurrentlyInPot { get; set; }

        // The amount of money the player is currently bet
        public int CurrentRoundBet { get; set; }

        public eAction LastAction { get; set; }

        public float WinningAmount { get; set; }

        // False when player folds
        public bool InHand { get; set; }

        // Player action is expected (some other player raised)
        public bool ShouldPlayInRound { get; set; }

        public bool ReadyToPlay { get; set; }

        public bool UpdateResult { get; set; }

        public bool InitilizeGame { get; set; }


        public int Position { get; set; }

        public Stats Stat { get; set; }

        public DateTime LastActionTime { get; set; }

        public PokerPlayer()
        {
            
        }

        public PokerPlayer(int i_StartMoney, int i_Index, string i_Name, string i_Email, Stats i_Stats, int i_Figure)
        {
            try
            {
                Stat = i_Stats;
                Figure = i_Figure;
                this.Money = i_StartMoney;
                Position = i_Index;
                Name = i_Name;
                Email = i_Email;
                Signature = SystemTools.RandomString(15);
                ReadyToPlay = true;
                UpdateResult = false;
                LastActionTime = DateTime.Now;
                InitilizeGame = false;
                this.NewHand();
                this.NewRound();
            }
            catch (Exception e)
            {                
                throw e;
            }
        }

        public void UpdateStatsAfterRound(bool i_Winner, PokerHand i_UsersHand)
        {
            try
            {
                Stat.NumberOfHandsPlay += 1;

                if (i_Winner)
                {
                    if (Stat.BiggestPot < WinningAmount)
                    {
                        Stat.BiggestPot = WinningAmount;
                    }
                    Stat.NumberOfHandsWon += 1;
                }

                if (Stat.Card1 != null && Stat.Card1 != 52)
                {
                    List<Card> cards = new List<Card>();
                    cards.Add(Deck.deck[(int)Stat.Card1]);
                    cards.Add(Deck.deck[(int)Stat.Card2]);
                    cards.Add(Deck.deck[(int)Stat.Card3]);
                    cards.Add(Deck.deck[(int)Stat.Card4]);
                    cards.Add(Deck.deck[(int)Stat.Card5]);
                    PokerHand bestHand = new PokerHand(cards);
                    if (bestHand.CompareTo(i_UsersHand) >= 0)
                    {
                        i_UsersHand = bestHand;
                    }
                }

                Stat.ConvertHandToInts(i_UsersHand);
                Stat.Money += Money;
                Stat.VictoryPercentage = (float)Stat.NumberOfHandsWon / (float)Stat.NumberOfHandsPlay;

                UpdateStats(false);
            }
            catch (Exception e)
            {
              throw e;
            }
        }

        public void UpdateStats(bool startGame)
        {
            try
            {
                InitilizeGame = startGame;
                writeInDataBase();
            }
            catch (Exception e)
            { 
                throw e;
            }
        }

        private void writeInDataBase()
        {
            try {
                var values = new JObject();
                values = JObject.FromObject(Stat);
                HttpContent content = new StringContent(values.ToString(), Encoding.UTF8, "application/json");
                putRequestAsync("http://localhost:61968/api/Stats?i_Email=" + Email, content);
            }
            catch (Exception e)
            {                
                throw e;
            }
        }


        private async void putRequestAsync(string i_Url, HttpContent i_Content)
        {
            try
            {
                string result = String.Empty;
                using (HttpClient client = new HttpClient())
                {

                    using (HttpResponseMessage response = await client.PutAsync(i_Url, i_Content))
                    {
                        using (HttpContent content = response.Content)
                        {
                            result = await content.ReadAsStringAsync();
                        }
                    }
                }
            }
            catch (Exception e)
            {                
                throw e;
            }
        }

        public void SitOut()
        {
            ReadyToPlay = false;
        }

        public void NewHand()
        {
            this.CurrentlyInPot = 0;
            this.CurrentRoundBet = 0;
            this.InHand = true;
            this.ShouldPlayInRound = true;
            this.UpdateResult = false;
        }

        public void NewRound()
        {
            this.CurrentRoundBet = 0;
            this.LastAction = eAction.None;
            

            if (this.InHand && this.Money > 0)
            {
                this.ShouldPlayInRound = true;
            }
        }

        public void PlaceMoney(int money)
        {
            try
            {
                if (money <= Money)
                {
                    this.CurrentRoundBet += money;
                    this.CurrentlyInPot += money;
                    this.Money -= money;
                }
                else
                {
                    this.CurrentRoundBet += Money;
                    this.CurrentlyInPot += Money;
                    this.Money -= Money;
                }
            }
            catch (Exception e)
            {
               throw e;
            }
        }

        public void Rebuy(int i_Amount)
        {
            try
            {
                if(i_Amount <= Stat.Money)
                {
                    Money += i_Amount;
                    Stat.Money -= i_Amount;
                    this.ReadyToPlay = true;
                    UpdateStats(false);
                } 
                else
                {
                    throw  new Exception("Player Balance too Low");
                }
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        public override string ToString()
        {
            try
            {
                StringBuilder result = new StringBuilder();
                result.AppendLine(Name + ":");
                result.AppendLine("Money" + ":" + Money);
                result.AppendLine("CurrentlyInPot" + ":" + CurrentlyInPot);
                result.AppendLine("CurrentRoundBet" + ":" + CurrentRoundBet);
                result.AppendLine("ShouldPlayInRound" + ":" + ShouldPlayInRound);

                return result.ToString();
            }
            catch (Exception e)
            {
               throw e;
            }
        }
    }
}
