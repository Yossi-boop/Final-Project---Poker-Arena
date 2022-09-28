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


        public IHttpActionResult Get(string i_CasinoId, string i_Email)
        {
            
                try
                {

                    Casino casino = DataStorage.GetCasino(i_CasinoId);
                    if(casino == null)
                {
                    throw new Exception("Casino not found");
                }

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

   

        // POST: api/UserLocation
        public IHttpActionResult Post([FromBody]CharacterInstance i_Character)
        {  
            
                try
                {
                    Casino casino = DataStorage.GetCasino(i_Character.CasinoId);
                if (casino == null)
                {
                    throw new Exception("Casino not found");
                }
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
                    return BadRequest(e.Message);
                }
            
        }
    }
}
