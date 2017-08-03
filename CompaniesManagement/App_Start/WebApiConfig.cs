using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Routing.Constraints;

namespace CompaniesManagement
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();
            /*
            config.Routes.MapHttpRoute(
                            name: "SpreadApi",
                            routeTemplate: "api/Crud/ids",
                            defaults: new { ids = RouteParameter.Optional },
                            constraints: new
                            {
                                id = new IntRouteConstraint()
                            }
                        );
*/
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{ids}",
                defaults: new { ids = RouteParameter.Optional }
            );
            

        }
    }
}
