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
    public class PokerRoundController : ApiController
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

                    var values = new JObject();
                    values = JObject.FromObject(table.CurrentRound);

                    return Ok(values);
                }
                catch (Exception e)
                {
                                        
                    return BadRequest(e.Message);
                }
            
        }

        public IHttpActionResult Post([FromBody]FinishRoundObject i_FinishRound)
        {
           
                try
                {
                    Table table = DataStorage.GetTable(i_FinishRound.TableId, i_FinishRound.CasinoId);
                    if (table == null)
                    {
                        return BadRequest("There Is No Table");
                    }
                    PokerPlayer player = table.GetPlayer(i_FinishRound.Email);
                    if(player != null)
                     {
                        player.UpdateResult = true;
                    }

                return Ok("Updated");
                }
                catch (Exception e)
                {
                                        
                    return BadRequest("Bad");
                }
            
        }

    }
}
