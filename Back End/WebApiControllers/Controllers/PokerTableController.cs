﻿using System;
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
    public class PokerTableController : ApiController
    {

        public IHttpActionResult Get()
        {
            
                try
                {
                    var values = new JArray();
                    values = JArray.FromObject(DataStorage.OpenTables);

                    return Ok(values);
                }
                catch (Exception e)
                {

                    
                    return BadRequest(e.Message);
                }
            
        }

        public IHttpActionResult Get(string CasinoId, string TableId, string Email)
        {
           
                try
                {
                    Table table = DataStorage.GetTable(TableId, CasinoId);
                if (table == null)
                {
                    return BadRequest("Table not found");
                }
                table.updateAction(Email);


                    DataStorage.LogedInUser user = DataStorage.GetActiveUserByMail(Email);
                    if (user != null)
                    {
                        user.LastActionTime = DateTime.Now;
                    }

                    if (table == null)
                    {
                        return BadRequest("There Is No Table");
                    }
                table.StartRound();

                if (table.CurrentRound != null && table.CurrentRound.Part != RoundPart.Result)
                    {
                        table.CurrentRound.CheckIfTimeout();
                    }

                    var values = new JObject();
                    values = JObject.FromObject(table);

                    return Ok(values);
                }
                catch (Exception e)
                {

                    
                    return BadRequest(e.Message);
                }
            
        }


        public IHttpActionResult Get(string CasinoId, string TableId)
        {

            try
            {
                Table table = DataStorage.GetTable(TableId, CasinoId);

                table.StartRound();
               

                return Ok("");
            }
            catch (Exception e)
            {


                return BadRequest("Bad");
            }

        }
    }
}
