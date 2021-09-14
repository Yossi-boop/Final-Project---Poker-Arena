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
    public class PokerTablePlayerController : ApiController
    {
        // GET: api/PokerTablePlayer

        // GET: api/PokerTablePlayer/5
        public IHttpActionResult Get(string CasinoId, string TableId)
        {
            try
            {
                Table table = DataStorage.GetTable(TableId, CasinoId);
                if (table == null)
                {
                    return BadRequest("There Is No Table");
                }
                var values = new JArray();
                values = JArray.FromObject(table.Players);

                return Ok(values);
            }
            catch (Exception e)
            {
                return BadRequest("NullReference");
            }
        }

        public IHttpActionResult Get(string CasinoId, string TableId, string email)
        {
            try
            {
                Table table = DataStorage.GetTable(TableId, CasinoId);
                if (table == null)
                {
                    return BadRequest("There Is No Table");
                }

                PokerPlayer player = table.GetPlayer(email);
                if (player == null)
                {
                    return BadRequest("There Is No Player like this in this table");
                }

                var values = new JObject();
                values = JObject.FromObject(player);

                return Ok(values);
            }
            catch (Exception e)
            {
                return BadRequest("NullReference");
            }
        }

        public void Get(string CasinoId, string TableId, string Email, string Signature, bool Now)
        {
            try
            {
                
                Table table = DataStorage.GetTable(TableId, CasinoId);
                table.GetOut(Email, Now);
                
            }
            catch (Exception e)
            {

            }
        }

        // POST: api/PokerTablePlayer
        public IHttpActionResult Post([FromBody]UserToAddToPokerTable i_User)
        {
            try
            {
                Table table = DataStorage.GetTable(i_User.TableId, i_User.CasinoId);
                if (table == null)
                {
                    return BadRequest("There Is No Table");
                }
                Stats stats = DataStorage.GetStatsByMail(i_User.Email);
                User user = DataStorage.GetUserByMail(i_User.Email);
                table.AddUser(i_User.Email, i_User.Name, i_User.Money, i_User.Index, stats, (int)user.Figure);
                PokerPlayer player = table.GetPlayer(i_User.Email);
                return Ok(player.Signature);
            }
            catch (Exception e)
            {
                return BadRequest("NullReference");
            }
        }

        // PUT: api/PokerTablePlayer/5 // ReBuy
        public void Put([FromBody]PokerPlayerReBuyDetails i_Details)
        {
            try
            {
                if (Security.CheckIfSignatureValid(i_Details.TableId, i_Details.CasinoId, i_Details.Email, i_Details.Signature))
                {
                    Table table = DataStorage.GetTable(i_Details.TableId, i_Details.CasinoId);
                    table.ReBuyAddOnAddToQueue(i_Details.Amount, i_Details.Email);
                }
            }
            catch (Exception e)
            {
                
            }
        }

        // DELETE: api/PokerTablePlayer/5 //GoOut
        public void Delete(string CasinoId, string TableId, string Email, string Signature, bool Now)
        {
            try
            {
                if (Security.CheckIfSignatureValid(TableId, CasinoId, Email, Signature))
                {
                    Table table = DataStorage.GetTable(TableId, CasinoId);
                    table.GetOut(Email,Now);
                }
            }
            catch (Exception e)
            {
         
            }
        }
    }
}
