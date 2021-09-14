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

        // GET: api/Chest

        // GET: api/Chest/5
        public IHttpActionResult Get(string CasinoId)
        {
            Casino casino = DataStorage.GetCasino(CasinoId);
            var values = new JObject();
            values = JObject.FromObject(casino.TreasureChest);

            return Ok(values);
        }

        // POST: api/Chest
        public void Post([FromBody]CollectChest Details)
        {
            try
            {
                Casino casino = DataStorage.GetCasino(Details.CasinoId);
                Stats stats = DataStorage.GetStatsByMail(Details.Email);
                casino.CollectMoney(stats);
            }
            catch (Exception e)
            {

            }
        }
    }
}
