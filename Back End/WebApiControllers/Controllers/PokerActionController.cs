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
                        if (round == null)
                        {
                            return BadRequest("There Is No round");
                        }
                        round.MakeAnAction(Action.PlayerSignature, Action.Action, Action.RaiseAmount);

                        return Ok("OK");
                    }

                    return BadRequest("Invalid Signature");
                }
                catch (Exception e)
                {
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(Logger.Path, true))
                    {
                        file.WriteLine("PokerActionController.post/" + e.Message);
                    }
                    return BadRequest("Bad");
                }
            
        }    
    }
}
