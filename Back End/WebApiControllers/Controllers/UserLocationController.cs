using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Classes;
using Newtonsoft.Json.Linq;
using WebApiControllers.Models;

namespace WebApiControllers.Controllers
{
    public class UserLocationController : ApiController
    {

        // GET: api/UserLocation
        //public IHttpActionResult Get(string i_CasinoId)
        //{
        //    Casino casino = DataStorage.GetCasino(i_CasinoId);
        //    List<CharacterInstance> users = casino.Users;
        //    var values = new JArray();
        //    values = JArray.FromObject(users);

        //    return Ok(values);
        //}

        // GET: api/UserLocation/5
        public IHttpActionResult Get(string i_CasinoId, string i_Email)
        {
            
                try
                {

                    Casino casino = DataStorage.GetCasino(i_CasinoId);

                    DataStorage.LogedInUser user = DataStorage.GetActiveUserByMail(i_Email);
                    if (user != null)
                    {
                        user.LastActionTime = DateTime.Now;
                    }

                    List<CharacterInstance> usersPosition = casino.GetUsersPositions(i_Email);
                    casino.CheckIfAllPlayerOnline();
                    casino.CheckIfTimeForChest();
                    var values = new JArray();
                    values = JArray.FromObject(usersPosition);
                    return Ok(values);
                }
                catch (Exception e)
                {                    
                    return BadRequest("Bad");
                }
            
        }

        //public IHttpActionResult Get(string i_CasinoId, string i_Email, int i_Radios)
        //{
        //    Casino casino = DataStorage.GetCasino(i_CasinoId);
        //    string usersPosition = casino.GetUsersPositions(i_Email,i_Radios);
        //    return Ok(usersPosition);
        //}

        // POST: api/UserLocation
        public IHttpActionResult Post([FromBody]CharacterInstance i_Character)
        {  
            
                try
                {
                    Casino casino = DataStorage.GetCasino(i_Character.CasinoId);
                    CharacterInstance character;
                    if ((character = casino.UserInCasino(i_Character.Email)) != null)
                    {
                        if (!character.UpdatePosition(i_Character.CurrentXPos, i_Character.CurrentYPos, i_Character.Direction, i_Character.Skin))
                        {
                            return BadRequest("Invalid movement");
                        }
                    }
                    else
                    {
                    i_Character.LastXPos = i_Character.CurrentXPos;
                    i_Character.LastYPos = i_Character.CurrentYPos;
                    casino.Users.Add(i_Character);
                    }

                    return Ok("Updated");
                }
                catch(Exception e) {                    
                    return BadRequest("Bad");
                }
            
        }
    }
}
