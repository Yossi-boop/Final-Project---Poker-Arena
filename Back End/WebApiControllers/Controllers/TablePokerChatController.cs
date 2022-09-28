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
    public class TablePokerChatController : ApiController
    {


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
                    values = JArray.FromObject(table.Chat.Archive);

                    return Ok(values);
                }
                catch (Exception e)
                {                    
                    return BadRequest("There Is No Table");
                }
            
        }

        public IHttpActionResult Post([FromBody] ChatMessageForPokerTable i_Message)
        {
          
                try
                {

                    Table table = DataStorage.GetTable(i_Message.TableId, i_Message.CasinoId);
                    if (table == null)
                    {
                        return BadRequest("There Is No Table");
                    }
                    if(table.Chat == null)
                {
                    return BadRequest("There Is No Chat");

                }
                table.Chat.Archive.Add(new Message(i_Message.UserName, i_Message.Body));
                    return Ok("MessageSent");


                }
                catch (Exception e)
                {                    
                    return BadRequest(e.Message);
                }
            
        }
    }
}

