using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LTWEB.Areas.Admin.Controllers
{
    public class AdminsController : Controller
    {
        // GET: Admin/Admins
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Test()
        {
            return View();
        }
    }
}