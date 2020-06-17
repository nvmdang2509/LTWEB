using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Mvc;

namespace LTWEB.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Admins";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Admins_default",
                "Admins/{controller}/{action}/{id}",
                new { controller = "Admins", action = "Index", id = UrlParameter.Optional }
                );
        }
    }
}