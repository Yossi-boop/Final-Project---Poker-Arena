using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApiControllers.Models
{
    public class FinishRoundObject
    {
        public string TableId { get; set; }

        public string CasinoId { get; set; }

        public string Email { get; set; }
    }
}