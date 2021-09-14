using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace WebApiControllers
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "Duel",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "Casino",
                routeTemplate: "api/{controller}/{i_Email}",
                defaults: new { i_Email = RouteParameter.Optional }
            );
        }
    }
}
