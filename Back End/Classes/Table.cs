using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsefullMethods;

namespace Classes
{
    public class Table
    {
        public string Id { get; set; }
    
        public List<PokerPlayer> Players { get; set; }

        public List<PokerPlayer> PlayersInTable { get; set; }

        public int NumberOfSits { get; set; }

        public int Dealer { get; set; }

        public Chat Chat { get; set; }

        public Setting GameSetting { get; set; }
        
        public Round CurrentRound { get; set; }

        public class AddOn
        {
            public int Amount { get; set; }

            public string Email { get; set; }

            public AddOn(int i_Amount, string i_Email)
            {
                Amount = i_Amount;
                Email = i_Email;
            }
        }

        public List<AddOn> ReBuyQueue { get; set; }

        public List<int> GetOutQueue { get; set; }

        public void updateAction(string email)
        {
            PokerPlayer player = GetPlayer(email);
            if (player != null)
            {
                player.LastActionTime = DateTime.Now;
            }

            if (CurrentRound != null)
            {
                player = CurrentRound.GetPlayer(email);
                if (player != null)
                {
                    player.LastActionTime = DateTime.Now;
                }
            }
        }

        public bool IsRoundLive { get; set; }

        public Table()
        {
            
        }

        public Table(int i_NumberOfSits)
        {
            Id = "table" + SystemTools.RandomString(10);
            Chat = new Chat();
            ReBuyQueue = new List<AddOn>();
            GetOutQueue = new List<int>();
            IsRoundLive = false;
            NumberOfSits = i_NumberOfSits;
            Dealer = i_NumberOfSits -1;
            initiateUserSits();
            
            GameSetting = new Setting(200,10000,20,10,30);
        }

        private void initiateUserSits()
        {
            Players = new List<PokerPlayer>(NumberOfSits);
            PlayersInTable = new List<PokerPlayer>();
            for (int i = 0; i < NumberOfSits; i++)
            {
                Players.Add(null);
                PlayersInTable.Add(null);
            }
        }

        public void AddUser(string i_Email,string i_Name, int i_Money, int i_Index, Stats i_Stats, int i_Figure)
        {
            PlayersInTable[i_Index] = new PokerPlayer(i_Money,i_Index,i_Name,i_Email,i_Stats, i_Figure);
        }

        public void StartRound()
        {
            if (IsRoundLive)
            {
              
                    if (!CurrentRound.FinishResult())
                    {
                        return;
                    }
                    else
                    {

                        reBuyAddOn();
                        ReBuyQueue.Clear();
                        checkIfPlayerOnline();
                        GetOutAux();
                        GetOutQueue.Clear();
                        updatePlayerListAfterRound();
                        IsRoundLive = false;
                    }
                
            }

            if (checkIfEnough())
            {
                updatePlayerListAfterRound();
                Dealer = nextPlayer(Dealer);
                CurrentRound = new Round(Players,NumberOfSits,Dealer,GameSetting.SmallBlind);
                IsRoundLive = true;
            }
            else
            {
                CurrentRound = null;
            }
        }

        private void updatePlayerListAfterRound()
        {
            for (int i = 0; i < 9; i++)
            {
                if((PlayersInTable[i] == null))
                {
                    PlayersInTable[i] = Players[i];
                }
                else if (Players[i] == null || !Players[i].Email.Equals(PlayersInTable[i].Email))
                {
                    Players[i] = PlayersInTable[i];
                }
                else
                {
                    PlayersInTable[i] = Players[i];
                }
                
            }
        }

        private void checkIfPlayerOnline()
        {
            for(int i = 0; i < 9; i++)
            {
                if(Players[i] != null && (DateTime.Now.Subtract(Players[i].LastActionTime).TotalSeconds > 20
                    ||!Players[i].ReadyToPlay|| Players[i].Money <= 0))
                {
                    Players[i] = null;
                }
            }
        }

        private void GetOutAux()
        {
            foreach (var index in GetOutQueue)
            {
                Players[index] = null;
            }
        }

        private int nextPlayer(int i_LastIndex)
        {
            i_LastIndex++;
            for (int i = 0; i < NumberOfSits; i++)
            {
                if (Players[(i_LastIndex + i) % NumberOfSits] != null)
                {
                    return (i_LastIndex + i) % NumberOfSits;
                }
            }

            return -1;
        }

        private bool checkIfEnough()
        {
            int count = 0;
            foreach (var player in PlayersInTable)
            {
                if (player != null)
                {
                    if (player.ReadyToPlay && player.Money > 0)
                    {
                        count++;
                        if (count > 1)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public void ReBuyAddOnAddToQueue(int i_AmountToAdd, string i_Email)
        {
            PokerPlayer player = GetPlayer(i_Email);
            if (player.Stat.Money >= i_AmountToAdd &&
                player.Money + i_AmountToAdd <= GameSetting.MaxBalance)
            {
                ReBuyQueue.Add(new AddOn(i_AmountToAdd,i_Email));
            }
        }

        private void reBuyAddOn()
        {
            foreach (var addOn in ReBuyQueue)
            {
                PokerPlayer player = GetPlayer(addOn.Email);
                if (player.Stat.Money >= addOn.Amount &&
                    player.Money + addOn.Amount <= GameSetting.MaxBalance)
                {
                    player.Rebuy(addOn.Amount);
                }
            }
        }

        //private void checkIfNeedsRebuy()
        //{
        //    int count = 0;
        //    foreach (var player in Players)
        //    {
        //        if (player != null)
        //        {
        //            if (player.ReadyToPlay && player.Money <= 0)
        //            {
        //                player.Rebuy();
        //            }
        //        }
        //    }
        //}

        public PokerPlayer GetPlayer(string i_Email)
        {
            foreach (var player in PlayersInTable)
            {
                if (player != null)
                {
                    if (player.Email.Equals(i_Email))
                    {
                        return player;
                    }
                }
            }

            return null;
        }

        public void GetOut(string i_Email, bool i_Now)
        {
            PokerPlayer player = GetPlayer(i_Email);
            if (i_Now && CurrentRound != null) {
                CurrentRound.GetOut(player.Position);
            }
            PlayersInTable[player.Position] = null;
            GetOutQueue.Add(player.Position);
        }
    }
}
