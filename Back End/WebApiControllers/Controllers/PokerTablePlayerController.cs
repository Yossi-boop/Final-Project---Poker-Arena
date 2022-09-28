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
      
    
        public IHttpActionResult Get(string CasinoId, string TableId)
        {
           
                try
                {
                    Table table = DataStorage.GetTable(TableId, CasinoId);
                    if (table == null)
                    {
                        return BadRequest("Table not found");
                    }
                    var values = new JArray();
                    values = JArray.FromObject(table.Players);

                    return Ok(values);
                }
                catch (Exception e)
                {                    
                    return BadRequest(e.Message);
                }
            
        }

        public IHttpActionResult Get(string CasinoId, string TableId, string email)
        {
            
                try
                {
                    Table table = DataStorage.GetTable(TableId, CasinoId);
                    if (table == null)
                    {
                        return BadRequest("Table not found");
                    }

                    PokerPlayer player = table.GetPlayer(email);
                    if (player == null)
                    {
                        return BadRequest("Player not found");
                    }

                    var values = new JObject();
                    values = JObject.FromObject(player);

                    return Ok(values);
                }
                catch (Exception e)
                {
                                       
                    return BadRequest(e.Message);

                }
            
        }

        public IHttpActionResult Get(string CasinoId, string TableId, string Email, string Signature)
        {
            
                try
                {

                    Table table = DataStorage.GetTable(TableId, CasinoId);
                    table.GetOut(Email, true);
                    return Ok("Deleted");
                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);

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
                    throw new Exception("Table not found");
                    }
                    Stats stats = DataStorage.GetStatsByMail(i_User.Email);
                if (stats == null)
                {
                    throw new Exception("Stats not found");
                }
                User user = DataStorage.GetUserByMail(i_User.Email);
                if (user == null)
                {
                    throw new Exception("User not found");
                }
                table.AddUser(i_User.Email, i_User.Name, i_User.Money, i_User.Index, stats, (int)user.Figure);
                    PokerPlayer player = table.GetPlayer(i_User.Email);
                if (player == null)
                {
                    throw new Exception("player not found");
                }
                return Ok(player.Signature);
                }
                catch (Exception e)
                {
                                        
                    return BadRequest(e.Message);
                }
            
        }

        // PUT: api/PokerTablePlayer/5 // ReBuy
        public IHttpActionResult Put([FromBody]PokerPlayerReBuyDetails i_Details)
        {
           
                try
                {
                    if (Security.CheckIfSignatureValid(i_Details.TableId, i_Details.CasinoId, i_Details.Email, i_Details.Signature))
                    {
                        Table table = DataStorage.GetTable(i_Details.TableId, i_Details.CasinoId);
                    if (table == null)
                    {
                        throw new Exception("Table not found");
                    }
                    table.ReBuyAddOnAddToQueue(i_Details.Amount, i_Details.Email);
                    return Ok("Updated");
                    }
                throw new Exception("Invalid signature");
                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);
                }
            
        }

        // DELETE: api/PokerTablePlayer/5 //GoOut
        public IHttpActionResult Delete(string CasinoId, string TableId, string Email, string Signature)
        {
            
                try
                {
                    if (Security.CheckIfSignatureValid(TableId, CasinoId, Email, Signature))
                    {
                        Table table = DataStorage.GetTable(TableId, CasinoId);
                    if (table == null)
                    {
                        throw new Exception("Table not found");
                    }
                    table.GetOut(Email, true);
                    return Ok("Deleted");
                }
                throw new Exception("Invalid signature");
            }
                catch (Exception e)
                {
                return BadRequest(e.Message);

            }

        }
    }
}
