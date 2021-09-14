using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Proxies;
using System.Text;
using System.Threading.Tasks;
using Classes;
using Server;

namespace BackEndMTAProject
{
    public static class Program
    {
     

        static void Main(string[] args)
        {
            Proxy proxy = new Proxy();
//            Console.WriteLine(proxy.SitOut("1234", "1234", "meir@gmail.com", null, true));
            proxy.getCasinoMessages("1234");
            proxy.getCasinoMessages("1234");
            proxy.getCasinoMessages("1234");
            proxy.getCasinoMessages("1234");
        }
    }
}