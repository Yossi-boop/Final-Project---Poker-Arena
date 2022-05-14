using Classes;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.UI.WebControls;
using WebApiControllers.Models;

namespace WebApiControllers.Controllers
{
    public class CasinoChatController : ApiController
    {        private readonly object postLock = new object();
        // GET: api/CasinoChat/5
        public IHttpActionResult Get(string i_CasinoId)
        {
                try
                {
                    Casino casino = DataStorage.GetCasino(i_CasinoId);


                    var values = new JArray();
                    values = JArray.FromObject(casino.GetMessages());
                    return Ok(values);
                }
                catch (Exception e)
                {

                Logger.WriteToLogger("CasinoChatController.get/" + e.Message);
                    
                    return BadRequest("Bad");
                }
            
        }

        // POST: api/CasinoChat
        public IHttpActionResult Post([FromBody] ChatMessageForPokerTable i_Message)
        {
            
                try
                {
                    Casino casino = DataStorage.GetCasino(i_Message.CasinoId);
                    if (casino == null)
                    {
                        return BadRequest("There Is No Casino");
                    }
                    lock(postLock)
                    {
                    casino.CasinoChat.Archive.Add(new Message(i_Message.UserName, i_Message.Body));
                    }
                    CharacterInstance user = casino.UserInCasino(i_Message.Email);
                    user.AddMessage(i_Message.Body);

                    return Ok("MessageSent");
                }
                catch (Exception e)
                {

                Logger.WriteToLogger("CasinoChatController.post/" + e.Message);
                    
                    return BadRequest("Bad");
                }
            
        }
       
    }
}
