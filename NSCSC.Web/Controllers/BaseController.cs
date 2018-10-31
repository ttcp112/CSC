using NSCSC.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NSCSC.Shared.Factory;


namespace NSCSC.Web.Controllers
{
    public class BaseController : Controller
    {
        //public string List { get; set; }
        // GET: Base
        public UserSession CurrentUser
        {
            get
            {
                if (System.Web.HttpContext.Current.Session["User"] != null)
                    return (UserSession)System.Web.HttpContext.Current.Session["User"];
                else
                    return new UserSession();
            }
        }
    
    }
}