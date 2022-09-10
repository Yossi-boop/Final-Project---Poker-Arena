using Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    interface Interface
    {
        string Login(string i_Email, string i_Password);
        string CreateUser(User i_NewUser);
        string AddFriend(string i_Email, string i_FriendEmail);


        User OpenDualMode(string i_Email);
       



        string CreateRoom(string i_ManagerEmail);
        string JoinRoom(string i_PinNumber, string i_UserEmail);
      

    }
}
