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
            try 
            {
                Table table = DataStorage.GetTable(i_TableId, i_CasinoId);
                PokerPlayer player = table.GetPlayer(i_Email);
                if (player == null)
                {
                    return false;
                }
                return player.Signature.Equals(i_Signature);
            } 
            catch(Exception)
            {
                return false;
            }
            
        }
    }
}