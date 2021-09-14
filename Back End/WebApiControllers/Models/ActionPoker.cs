using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Classes;

namespace WebApiControllers.Models
{
    public class ActionPoker
    {
        public string TableId { get; set; }

        public string CasinoId { get; set; }

        public string Email { get; set; }

        public string PlayerSignature { get; set; }

        public eAction Action { get; set; }

        public int RaiseAmount { get; set; }
    }
}