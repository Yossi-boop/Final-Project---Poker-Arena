﻿using System;
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
                        throw new Exception("Email not exist");
                    }

                    var values = new JObject();
                    values = JObject.FromObject(user);
                    return Ok(values);
                }
                catch (Exception e)
                {
                                    
                    return BadRequest(e.Message);
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
                        throw new Exception("Email already exist");
                    }
                    user = new User(i_UserDetails.Name, i_UserDetails.Email, i_UserDetails.Password);
                    DataStorage.AddUser(user);

                    return Ok("Successed");
                }
                catch (Exception e)
                {                    
                    return BadRequest(e.Message);
                }
            
        }

        // PUT: api/User/5
        public IHttpActionResult Put(string i_Email, [FromBody]User user)
        {
                try
                {
                    DataStorage.UpdateUserDetails(i_Email, user);
                return Ok("Updated");

                }
                catch (Exception e)
                {
                return BadRequest(e.Message);
                }
            
        }
    }
}
