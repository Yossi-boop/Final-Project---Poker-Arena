using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Classes;
using UsefullMethods;
using WebApiControllers.data;
using WebApiControllers.Models;

namespace WebApiControllers.Controllers
{
    public class LogInController : ApiController
    {
     
        public IHttpActionResult Post([FromBody]UserDetailsForCreation i_UserDetails)
        {
           
                try
                {
                    User user = DataStorage.GetUserByMail(i_UserDetails.Email);
                    if (user == null)
                    {
                        return BadRequest("Email doesn't exist");
                    }

                    if (DataStorage.checkIfUserOnline(i_UserDetails.Email))
                    {
                        return BadRequest("User Allready playing");
                    }

                    if (user.Password.Equals(i_UserDetails.Password))
                    {
                        DataStorage.ActiveUsers.Add(new DataStorage.LogedInUser(user));
                        return Ok("loggedIn complete");
                    }

                    return BadRequest("Incorrect password");
                }
                catch (Exception e)
                {
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(Logger.Path, true))
                    {
                        file.WriteLine("LogInController.post/" + e.Message);
                    }
                    return BadRequest("Bad");
                }
            
        }
    }
}
