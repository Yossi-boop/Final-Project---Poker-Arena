using Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Validations
    {
        public static string CheckIfUserFiledsAreValid(User i_NewUser)
        {
            string stringToReturn;
            if (i_NewUser.Name != null && i_NewUser.Name != " ")
            {
                stringToReturn = "200";
            }
            else
            {
                return "Name field is empty";
            }
            if (stringToReturn != "200" || i_NewUser.Password != null && i_NewUser.Password != " ")
            {
                stringToReturn = "200";
            }
            else
            {
                return "Password field is empty";
            }
            if (stringToReturn != "200" || i_NewUser.Email != null && i_NewUser.Email != " ")
            {
                stringToReturn = "200";
            }
            else
            {
                stringToReturn = "Email field is empty";
            }
            return stringToReturn;

        }
        //public bool CheckIfCardUsedInDualMatch(Card i_Card, DualMatch i_DualMatch)
        //{
        //    foreach (Card card in i_DualMatch.CardHistoryList)
        //    {
        //        if (card.CardID.Equals(card.CardID))
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}



    }

}
