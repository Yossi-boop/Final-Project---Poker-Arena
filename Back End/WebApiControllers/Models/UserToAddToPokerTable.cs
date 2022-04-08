using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApiControllers.Models
{
    public class UserToAddToPokerTable
    {
        public string TableId { get; set; }
        public string CasinoId { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public int Money { get; set; }
        public int Index { get; set; }

        public UserToAddToPokerTable()
        {
            
        }

        public UserToAddToPokerTable(string i_TableId, string i_CasinoId, string i_Email, string i_Name, int i_Money, int i_Index)
        {
            TableId = i_TableId;
            CasinoId = i_CasinoId;
            Email = i_Email;
            Name = i_Name;
            Money = i_Money;
            Index = i_Index;
        }
    }
}