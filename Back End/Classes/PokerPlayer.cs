using System;
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

        public int Position { get; set; }

        public Stats Stat { get; set; }

        public DateTime LastActionTime { get; set; }

        public PokerPlayer()
        {
            
        }

        public PokerPlayer(int i_StartMoney, int i_Index, string i_Name, string i_Email, Stats i_Stats, int i_Figure)
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
            
            this.NewHand();
            this.NewRound();
        }

        public void UpdateStats(bool i_Winner, PokerHand i_UsersHand)
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

            if (Stat.Card1 != null)
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

            writeInDataBase();
        }

        private void writeInDataBase()
        {
            var values = new JObject();
            values = JObject.FromObject(Stat);
            HttpContent content = new StringContent(values.ToString(), Encoding.UTF8, "application/json");
            putRequestAsync("https://pokerarenaapi.azurewebsites.net/api/Stats?i_Email=" + Email, content);
        }

        private async void putRequestAsync(string i_Url, HttpContent i_Content)
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

        public void Rebuy(int i_Amount)
        {
            Money += i_Amount;
            this.ReadyToPlay = true;
        }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
            result.AppendLine(Name + ":");
            result.AppendLine("Money" + ":" + Money);
            result.AppendLine("CurrentlyInPot" + ":" + CurrentlyInPot);
            result.AppendLine("CurrentRoundBet" + ":" + CurrentRoundBet);
            result.AppendLine("ShouldPlayInRound" + ":" + ShouldPlayInRound);

            return result.ToString();
        }
    }
}
