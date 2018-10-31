using NSCSC.Web.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace NSCSC.Web.Controllers
{
    [NuAuth]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
           
            return View();
        }

        [HttpGet]
        public ActionResult Logout()
        {
            try
            {
                NuAuthAttribute.listUserModule.Clear();
                if (Session["User"] == null)
                    return RedirectToAction("Login", new { area = "" });

                FormsAuthentication.SignOut();
                Session.Remove("User");

                return RedirectToAction("Index", "Home", new { area = "" });
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("Logout Error: ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }
    }
}