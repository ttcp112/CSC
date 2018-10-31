using NSCSC.Shared.Models;
using NSCSC.Shared.Models.AccessControl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Xml;
using System.Xml.Linq;

namespace NSCSC.Web.App_Start
{
    public class NuAuthAttribute : AuthorizeAttribute
    {
        private UserSession _CurrentUser;
        private string ActionType;
        private string Permission;
        private string Controller;
        private List<string> _Views = new List<string> { "index", "default", "view", "detail", "get", "load", "filter", "search", "apply" };
        private Dictionary<string, string> listCtr;

        private List<String> _ViewTimeoutSession = new List<string>
        {
            "LoadIngredient", "LoadIngredientIngredient",       //Ingrident
            "AddTab", "AddDishes", "CheckDish",                 //SetMenu 
            "AddModifiers","LoadModifiers","CheckModifier",      //Dish
            "LoadDetail"
        };

        /*Factory*/        
        public static List<PermissionDTO> listUserModule = new List<PermissionDTO>();
        public static List<string> _ListMenuUserModule = new List<string>();

        public NuAuthAttribute()
        {
           
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (HttpContext.Current.Session["User"] == null)
                _CurrentUser = new UserSession();
            else
                _CurrentUser = (UserSession)HttpContext.Current.Session["User"];
            if (_CurrentUser.UserId != null)
            {
                if (listUserModule == null)
                {
                    listUserModule = new List<PermissionDTO>();
                }
                if (listUserModule.Count == 0)
                {
                    if (_CurrentUser.ListPermission != null && _CurrentUser.ListPermission.Count > 0)
                        listUserModule.AddRange(_CurrentUser.ListPermission);
                    else
                        listUserModule = new List<PermissionDTO>();
                }
            }
            if (_CurrentUser.IsSuperAdmin)
            {
                _ListMenuUserModule = new List<string>();
                listUserModule = new List<PermissionDTO>();
            }
            if (listUserModule != null && listUserModule.Count > 0)
            {
                _ListMenuUserModule = new List<string>();
                _ListMenuUserModule = listUserModule.Where(o =>o.IsView).Select(x => x.Code.ToString()).ToList();
            }
            Controller = httpContext.Request.RequestContext.RouteData.Values["controller"].ToString();
            XDocument xmldoc = new XDocument();
            string path = HttpContext.Current.Server.MapPath("~\\XMLData\\XMLRoleData.xml");
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            xmldoc = XDocument.Load(fs);
            StringBuilder result = new StringBuilder();
            var lv1s = from lv1 in xmldoc.Descendants("item")
                        select new
                        {
                            Value = lv1.Descendants("code"),
                            Text = lv1.Descendants("name")
                        };
            //Loop through results
            listCtr = new Dictionary<string, string>();
            foreach (var lv1 in lv1s)
            {
                var value = lv1.Value.FirstOrDefault().Value;
                var text = lv1.Text.FirstOrDefault().Value;
                bool ischeck = listCtr.ContainsKey(value);
                if (!ischeck)
                {
                    listCtr.Add(value, text);
                }
            }

            // set permission
            string alias = httpContext.Request.RequestContext.RouteData.Values["controller"].ToString();
            //if (alias == "ACGroupingAccount" || alias == "ACGroupingAreas")
            //    Permission = "AccessControl";
            //else if (alias.IndexOf("HQReport") >= 0)
            //    Permission = "HQReports";
            //else
                Permission = alias;

            // set actionType
            string action = httpContext.Request.RequestContext.RouteData.Values["action"].ToString().ToLower();
            bool isViewAction = _Views.Any(s => action.Contains(s));
            ActionType = isViewAction ? "View" : "Action";

            return IsPermission();
        }

        protected bool IsPermission()
        {
            try
            {
                // If user not logged in, require login
                if (!_CurrentUser.IsAuthenticated)
                    return false;
                else
                {
                    if (_CurrentUser.IsSuperAdmin)
                    {
                        return true;
                    }
                    if (_CurrentUser.IsSuperAdmin || Controller.ToLower().Equals("home"))
                    {
                        return true;
                    }
                    else
                    {
                        bool per = false;
                        List<string> _name = new List<string>();
                        _ListMenuUserModule.ForEach(s =>
                        {
                            bool ischeck = listCtr.ContainsKey(s);
                            if (ischeck)
                            {
                                _name.Add(listCtr[s]);
                            }
                        });
                        if (ActionType.ToLower().Equals("view"))//View
                        {
                            per = _name.Any(s =>s.Equals(Controller));
                        }
                        else //Action
                        {
                            per = _name.Any(s => s.Equals(Controller));
                        }
                        return per;
                    }                    
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (!_CurrentUser.IsAuthenticated)
            {
                string controller = filterContext.RouteData.Values["controller"].ToString();
                string action = filterContext.RouteData.Values["action"].ToString();
                bool isChange = false;
                if (_ViewTimeoutSession.Contains(action))
                {
                    isChange = true;
                }
                if (isChange) //TimeoutSession
                {
                    filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(
                        new
                        {
                            controller = "Error",
                            action = "TimeOutSession",
                            area = string.Empty,
                        })
                    );
                }
                else //Login
                {
                    filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(
                        new
                        {
                            controller = "Login",
                            action = "Index",
                            area = string.Empty,
                            isAjax = filterContext.HttpContext.Request.IsAjaxRequest(),
                            returnUrl = filterContext.HttpContext.Request.Url.ToString().Replace("/Report", "")
                        })
                    );
                }
            }
            else
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(
                        new
                        {
                            controller = "Error",
                            action = "Unauthorised",
                            area = string.Empty,
                        })
                    );
        }
    }
}