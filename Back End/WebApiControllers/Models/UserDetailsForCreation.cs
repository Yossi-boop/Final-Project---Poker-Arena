using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApiControllers.Models
{
    public class UserDetailsForCreation
    {
        public string Name { get; set; }  //need to change the accessability level
        public string Email { get; set; } //need to change the accessability level
        public string Password { get; set; }
    }
}