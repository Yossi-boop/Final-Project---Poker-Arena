using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApiControllers.Models
{
    public class PokerPlayerReBuyDetails
    {
        public string TableId { get; set; }

        public string CasinoId { get; set; }

        public string Email { get; set; }

        public string Signature { get; set; }

        public int Amount { get; set; }
    }
}