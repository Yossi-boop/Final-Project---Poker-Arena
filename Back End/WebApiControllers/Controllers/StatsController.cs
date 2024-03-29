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
    public class StatsController : ApiController
    {
        
        public IHttpActionResult Get(string i_Email)
        {
            
                try
                {
                    Stats stats = DataStorage.GetStatsByMail(i_Email);
                    if (stats == null)
                    {
                        return BadRequest("Email not exist");
                    }

                    var values = new JObject();
                    values = JObject.FromObject(stats);
                    return Ok(values);
                }
                catch (Exception e)
                {
                                        
                    return BadRequest(e.Message);

                }
            
        }

        
        public IHttpActionResult Post(string i_Email)
        {
            
                try
                {
                    DataStorage.AddStats(i_Email);
                return Ok("Stats created");
                }
                catch (Exception e)
                {
                return BadRequest(e.Message);
                }
            
        }

        public IHttpActionResult Put(string i_Email, [FromBody]Stats i_Stats)
        {
            
                try
                {
                    DataStorage.UpdataStats(i_Email, i_Stats);
                return Ok("Stats updated");

            }
            catch (Exception e)
                {
                return BadRequest(e.Message);

            }

        }
    }
}
