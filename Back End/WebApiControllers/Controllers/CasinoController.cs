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
                    return BadRequest("Bad");
                }
            
        }

        // public IHttpActionResult Get(string i_CasinoId, string i_Email)
        // {
            
        //         try
        //         {
        //             Casino casino = DataStorage.GetCasino(i_CasinoId);
        //             User user = DataStorage.GetUserByMail(i_Email);

        //             return Ok("Updated");
        //         }
        //         catch (Exception e)
        //         {
        //             using (System.IO.StreamWriter file = new System.IO.StreamWriter(Logger.Path, true))
        //             {
        //                 file.WriteLine("CasinoController.get2/" + e.Message);
        //             }
        //             return BadRequest("Bad");
        //         }
            
        // }


        // public IHttpActionResult Post([FromBody]string i_CasinoId, [FromBody]string i_Email)
        // {
           
        //         try
        //         {
        //             Casino casino = DataStorage.GetCasino(i_CasinoId);
        //             User user = DataStorage.GetUserByMail(i_Email);
        //             return Ok("Updated");
        //         }
        //         catch (Exception e)
        //         {
        //             using (System.IO.StreamWriter file = new System.IO.StreamWriter(Logger.Path, true))
        //             {
        //                 file.WriteLine("CasinoController.post/" + e.Message);
        //             }
        //             return BadRequest("Bad");
        //         }
        // }
    }
}
