using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Classes;

namespace WebApiControllers.Models
{
    public static class Security
    {
        public static bool CheckIfSignatureValid(string i_TableId,string i_CasinoId, string i_Email, string i_Signature)
        {
            Table table = DataStorage.GetTable(i_TableId, i_CasinoId);
            PokerPlayer player = table.GetPlayer(i_Email);
            return player.Signature.Equals(i_Signature);
        }
    }
}