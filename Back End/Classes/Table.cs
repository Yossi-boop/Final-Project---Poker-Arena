﻿using System;
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

        private readonly object sitOutLock = new object();

        private readonly object startRoundLock = new object();

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
            try
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
            catch (Exception e)
            {
     throw e;
            }
        }

        public bool IsRoundLive { get; set; }

        public bool NewRound { get; set; }

        public Table()
        {

        }

        public Table(int i_NumberOfSits, Setting setting)
        {
            try
            {
                Id = "table" + SystemTools.RandomString(10);
                Chat = new Chat();
                ReBuyQueue = new List<AddOn>();
                GetOutQueue = new List<int>();
                IsRoundLive = false;
                NumberOfSits = i_NumberOfSits;
                Dealer = i_NumberOfSits - 1;
                initiateUserSits();
                NewRound = true;

                GameSetting = setting; 
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        private void initiateUserSits()
        {
            try
            {
                Players = new List<PokerPlayer>();
                PlayersInTable = new List<PokerPlayer>();
                for (int i = 0; i < NumberOfSits; i++)
                {
                    Players.Add(null);
                    PlayersInTable.Add(null);
                }
            }
            catch (Exception e)
            {
                
                throw e;
            }
        }

        public void AddUser(string i_Email, string i_Name, int i_Money, int i_Index, Stats i_Stats, int i_Figure)
        {
            try
            {
                if(i_Stats.Money>= GameSetting.MaxBalance)
                {
                    i_Money = GameSetting.MaxBalance;
                }
                else
                { 
                    i_Money = i_Stats.Money;
                }

                if(i_Money < GameSetting.MinBalance)
                {
                    throw new Exception("User not have enough balance");
                }
                lock(sitOutLock){
                    for (int i = 0; i < NumberOfSits; i++)
                    {
                        if (PlayersInTable[i] != null && PlayersInTable[i].Email.Equals(i_Email))
                        {
                            throw new Exception("User already in table");
                        }
                    }
                    if (PlayersInTable[i_Index] == null)
                {
                    PlayersInTable[i_Index] = new PokerPlayer(i_Money, i_Index, i_Name, i_Email, i_Stats, i_Figure);
                }
                }
            }
            catch (Exception e)
            {     
                throw e;
            }
        }

        public void StartRound()
        {
            try
            {
                
                    if (IsRoundLive)
                    {

                        if (CurrentRound != null && !CurrentRound.FinishResult())
                        {
                            return;
                        }
                        else
                        {

                            reBuyAddOn();
                            ReBuyQueue.Clear();
                            checkIfPlayerOnline();
                            // GetOutAux();
                            // GetOutQueue.Clear();
                            // updatePlayerListAfterRound();
                            IsRoundLive = false;
                        NewRound = true;
                        CurrentRound = null;

                    }

                }
                if (NewRound)
                {
                    if (checkIfEnough())
                    {
                        NewRound = false;

                        IsRoundLive = true;
                        updatePlayerListAfterRound();
                      
                            Dealer = nextPlayer(Dealer);
                            if (CurrentRound == null)
                            {
                                CurrentRound = new Round(Players, NumberOfSits, Dealer, GameSetting.SmallBlind);
                            }
                        
                    }
                    else
                    {
                        CurrentRound = null;
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private void updatePlayerListAfterRound()
        {
            try
            {
                for (int i = 0; i < 9; i++)
                {
                    // if ((PlayersInTable[i] == null))
                    // {
                    //     PlayersInTable[i] = Players[i];
                    // }
                    // else 
                    if (Players[i] == null || !Players[i].Email.Equals(PlayersInTable[i].Email))
                    {
                        Players[i] = PlayersInTable[i];
                    }
                    // else
                    // {
                    //     PlayersInTable[i] = Players[i];
                    // }

                }
            }
            catch (Exception e)
            {
              
                throw e;
            }
        }

        private void checkIfPlayerOnline()
        {
            try
            {
                for (int i = 0; i < 9; i++)
                {
                    if (Players[i] != null && (DateTime.Now.Subtract(Players[i].LastActionTime).TotalSeconds > 20
                        || !Players[i].ReadyToPlay || Players[i].Money <= 0))
                    {
                        Players[i] = null;
                        PlayersInTable[i] = null;
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        // private void GetOutAux()
        // {
        //     try
        //     {
        //         foreach (var index in GetOutQueue)
        //         {
        //             Players[index] = null;
        //             PlayersInTable[index] = null;
        //         }
        //     }
        //     catch (Exception e)
        //     {
        //         using (System.IO.StreamWriter file = new System.IO.StreamWriter(Logger.Path, true))
        //         {
        //             file.WriteLine("Table.GetOutAux/" + e.Message);
        //         }
        //         throw e;
        //     }
        //}

        private int nextPlayer(int i_LastIndex)
        {
            try
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
            catch (Exception e)
            {
                throw e;
            }
        }

        private bool checkIfEnough()
        {
            try
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
            catch (Exception e)
            {
                 throw e;
            }
        }

        public void ReBuyAddOnAddToQueue(int i_AmountToAdd, string i_Email)
        {
            try
            {
                PokerPlayer player = GetPlayer(i_Email);
                if (player.Stat.Money >= i_AmountToAdd &&
                    player.Money + i_AmountToAdd <= GameSetting.MaxBalance)
                {
                    ReBuyQueue.Add(new AddOn(i_AmountToAdd, i_Email));
                }
            }
            catch (Exception e)
            {
               throw e;
            }
        }

        private void reBuyAddOn()
        {
            try
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
            catch (Exception e)
            {
               throw e;
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
            try
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
            catch (Exception e)
            {
               throw e;
            }
        }

        public void GetOut(string i_Email, bool i_Now)
        {
            try
            {
                PokerPlayer player = GetPlayer(i_Email);
                if (i_Now && CurrentRound != null &&  CurrentRound.GetPlayer(i_Email) != null)
                {

                    CurrentRound.GetOut(player.Position);
                }
                PlayersInTable[player.Position] = null;
                if (CurrentRound == null)
                {
                    Players[player.Position] = null;

                }
                //GetOutQueue.Add(player.Position);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}