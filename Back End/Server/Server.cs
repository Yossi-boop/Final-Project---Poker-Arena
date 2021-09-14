using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Classes;


namespace Server
{
    //public class Server
    //{
       
    //    public List<string> OnlineUserIds { get; set; }
    //    public List<Table> ActiveTables { get; set; }

    //    // Check if User Exist and if so LogIn, Return status to the Client.
    //    public string Login(string i_Email, string i_Password)
    //    {
    //        User user = Data.CheckIfUserExist(i_Email);
    //        if (user == null)
    //        {
    //            return "User not exist in our system";
    //        }

    //        if (user.Password.Equals(i_Password))
    //        {
    //            OnlineUserIds.Add(user.Email);
    //            return "200";
    //        }

    //        return "Wrong Password";
            

    //    }

    //    // Create new user and return the status of the request to client
    //    public string CreateUser(User i_NewUser)
    //    {
    //        string stringToReturn = Validations.CheckIfUserFiledsAreValid(i_NewUser);
    //        if(stringToReturn == "200")
    //        {
    //            if (Data.InsertUserToTheDataBase(i_NewUser) == "200")
    //            {
    //                return "200";
    //            }
    //            else
    //            {
    //                return "The user not enter to the database";
    //            }
    //        }
    //        else
    //        {
    //            return stringToReturn;
    //        }
    //    }

    //    public string AddFriend(string i_Email, string i_FriendEmail)
    //    {
    //        string messageToReturn = Data.CheckIfFriendIsNotInFriendsList(i_Email, i_FriendEmail);
    //        if (messageToReturn == "200")
    //        {
    //            if (Data.AddFriendToFriendList(i_Email, i_FriendEmail) == "200")
    //            {
    //                return "200";
    //            }
    //        }

    //        return messageToReturn;
    //    }

    //    // return the User object to the client.
    //    public User OpenDualMode(string i_Email)
    //    {
    //        // maybe we need to decide which part of the data we will sent here
    //        return Data.CheckIfUserExist(i_Email);
    //    }

    //    // Create new Duel match with friend and return the match object to client.
       

        

        

        



        

        

        
        

        

        

    //    public string StartStreaming(string i_PinNumber)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public char[] GetStreaming(string i_PinNumber)
    //    {
    //        throw new NotImplementedException();
    //    }

       

        




    //    // need to move this to other package

        

    //}
}
