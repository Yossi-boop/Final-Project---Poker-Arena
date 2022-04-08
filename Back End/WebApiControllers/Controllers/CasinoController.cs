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
                var values = new JArray();
                values = JArray.FromObject(casino.Furnitures);

                return Ok(values);
            }
            catch (Exception e)
            {
                return BadRequest("NullReference");
            }
        }

        // GET: api/Casino/5
        public IHttpActionResult Get(string i_CasinoId, string i_Email)
        {
            try {
                Casino casino = DataStorage.GetCasino(i_CasinoId);
                User user = DataStorage.GetUserByMail(i_Email);

                return Ok("Updated");
            }
            catch (Exception e)
            {
                return BadRequest("NullReference");
            }
        }


        public IHttpActionResult Post([FromBody]string i_CasinoId, [FromBody]string i_Email)
        {
            try {
                Casino casino = DataStorage.GetCasino(i_CasinoId);
                User user = DataStorage.GetUserByMail(i_Email);
                return Ok("Updated");
            }
            catch (Exception e)
            {
                return BadRequest("NullReference");
            }
        }

        // PUT: api/Casino/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Casino/5
        public void Delete(int id)
        {
        }
    }
}
