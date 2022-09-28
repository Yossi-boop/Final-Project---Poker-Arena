using Classes;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApiControllers.data;
using WebApiControllers.Models;

namespace WebApiControllers.Controllers
{
    public class ChestController : ApiController
    {

        
        public IHttpActionResult Get(string CasinoId)
        {
            
                try
                {
                    Casino casino = DataStorage.GetCasino(CasinoId);
                if (casino == null)
                {
                    throw new Exception("Casino not found");
                }
                var values = new JObject();
                    values = JObject.FromObject(casino.TreasureChest);

                    return Ok(values);
                }
                catch (Exception e)
                {
                                      
                    return BadRequest(e.Message);
                }
            
        }

        public IHttpActionResult Post([FromBody]CollectChest Details)
        {
            
                try
                {
                    Casino casino = DataStorage.GetCasino(Details.CasinoId);
                if (casino == null)
                {
                    throw new Exception("Casino not found");
                }
                Stats stats = DataStorage.GetStatsByMail(Details.Email);
                if (stats == null)
                {
                    throw new Exception("stats not found");
                }
                casino.CollectMoney(stats);
                return Ok("Updated");
                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);        
                }
            
        }
    }
}
