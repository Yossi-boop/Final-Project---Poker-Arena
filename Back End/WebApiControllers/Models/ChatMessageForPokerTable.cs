using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApiControllers.Models
{
    public class ChatMessageForPokerTable
    {
        public string TableId { get; set; }

        public string CasinoId { get; set; }

        public string Email { get; set; }

        public string Signature { get; set; }

        public string UserName { get; set; }

        public string Body { get; set; }

        public ChatMessageForPokerTable()
        {
            
        }

        public ChatMessageForPokerTable(string i_TableId, string i_UserName, string i_Body)
        {
            TableId = i_TableId;
            UserName = i_UserName;
            Body = i_Body;
        }
    }
}