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
    public class PokerRoundController : ApiController
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

                    var values = new JObject();
                    values = JObject.FromObject(table.CurrentRound);

                    return Ok(values);
                }
                catch (Exception e)
                {
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(Logger.Path, true))
                    {
                        file.WriteLine("PokerRoundController.get/" + e.Message);
                    }
                    return BadRequest("Bad");
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

                    table.GetPlayer(i_FinishRound.Email).UpdateResult = true;

                    return Ok("Updated");
                }
                catch (Exception e)
                {
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(Logger.Path, true))
                    {
                        file.WriteLine("PokerRoundController.post/" + e.Message);
                    }
                    return BadRequest("Bad");
                }
            
        }

    }
}
