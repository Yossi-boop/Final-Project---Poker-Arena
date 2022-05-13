using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Classes;
using Newtonsoft.Json.Linq;
using WebApiControllers.data;
using WebApiControllers.Models;

namespace WebApiControllers.Controllers
{
    public class UserController : ApiController
    {
        // GET: api/User

        // GET: api/User/5

        public IHttpActionResult Get(string i_Email)
        {
            
                try
                {
                    User user = DataStorage.GetUserByMail(i_Email);
                    if (user == null)
                    {
                        return BadRequest("Email not exist");
                    }

                    var values = new JObject();
                    values = JObject.FromObject(user);
                    return Ok(values);
                }
                catch (Exception e)
                {
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(Logger.Path, true))
                    {
                        file.WriteLine("UserController.get/" + e.Message);
                    }
                    return BadRequest("");
                }
            
        }

        // POST: api/User
        public IHttpActionResult Post([FromBody]UserDetailsForCreation i_UserDetails)
        {
          

                try
                {
                    User user = DataStorage.GetUserByMail(i_UserDetails.Email);
                    if (user != null)
                    {
                        return BadRequest("Email already exist");
                    }
                    user = new User(i_UserDetails.Name, i_UserDetails.Email, i_UserDetails.Password);
                    DataStorage.AddUser(user);

                    return Ok("Successed");
                }
                catch (Exception e)
                {
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(Logger.Path, true))
                    {
                        file.WriteLine("UserController.post/" + e.Message);
                    }
                    return BadRequest("There Is No Table");
                }
            
        }

        // PUT: api/User/5
        public void Put(string i_Email, [FromBody]User user)
        {
            

                try
                {
                    DataStorage.UpdateUserDetails(i_Email, user);


                }
                catch (Exception e)
                {

                }
            
        }
    }
}
