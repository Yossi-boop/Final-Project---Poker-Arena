using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Classes;
using Newtonsoft.Json.Linq;
using Server;
using WebApiControllers.data;
using WebApiControllers.Models;


namespace WebApiControllers.Controllers
{
    public class CasinoController : ApiController
    {
        
        public IHttpActionResult Get(string i_CasinoId)
        {
            
                try
                {
                    Casino casino = DataStorage.GetCasino(i_CasinoId);
                if (casino == null)
                {
                    throw new Exception("Casino not found");
                }

                var values = new JArray();
                    values = JArray.FromObject(casino.Furnitures);

                    return Ok(values);
                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);
                }
            
        }

    
    }
}
