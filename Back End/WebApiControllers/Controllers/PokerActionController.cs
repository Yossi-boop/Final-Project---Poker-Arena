using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Classes;
using WebApiControllers.data;
using WebApiControllers.Models;

namespace WebApiControllers.Controllers
{
    public class PokerActionController : ApiController
    {
        // GET: api/PokerAction
       
        public IEnumerable<string> Get()
        {
            Casino m = DataStorage.Casinos[0];
            return new string[] { m.Id, m.Furnitures[0].Type.ToString() };
        }

        // GET: api/PokerAction/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/PokerAction
        public IHttpActionResult Post([FromBody] ActionPoker Action)
        {
           
            try
            {
                if (Security.CheckIfSignatureValid(Action.TableId, Action.CasinoId, Action.Email, Action.PlayerSignature))
                {
                    Table table = DataStorage.GetTable(Action.TableId, Action.CasinoId);
                    if (table == null)
                    {
                        return BadRequest("There Is No Table");
                    }

                    Round round = table.CurrentRound;
                       
                    round.MakeAnAction(Action.PlayerSignature, Action.Action, Action.RaiseAmount);
                   
                    return Ok("OK");
                }

                return BadRequest("Invalid Signature");
            }
            catch (Exception e)
            {
                return BadRequest("NullReference");
            }
        }

        // PUT: api/PokerAction/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/PokerAction/5
        public void Delete(int id)
        {
        }
    }
}
