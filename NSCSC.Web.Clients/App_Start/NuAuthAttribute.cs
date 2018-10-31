using Newtonsoft.Json;
using NSCSC.Shared.Models;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Linq;

namespace NSCSC.Web.Clients.App_Start
{
    public class NuAuthAttribute : AuthorizeAttribute
    {
        private UserSession _CurrentUser;
        private string Permission;

        private List<String> _ViewTimeoutSession = new List<string>
        {
            "LoadDetailDemo" /*test view*/
        };

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (HttpContext.Current.Session["UserClientSite"] == null)
                _CurrentUser = new UserSession();
            else
                _CurrentUser = (UserSession)HttpContext.Current.Session["UserClientSite"];
            
            // set permission
            string alias = httpContext.Request.RequestContext.RouteData.Values["controller"].ToString();
            Permission = alias;
            // set actionType
            string action = httpContext.Request.RequestContext.RouteData.Values["action"].ToString().ToLower();
            // Edit get cookie
            var Cookie = HttpContext.Current.Request.Cookies["csc-order-v2"];
            if(Cookie != null)
            {
                var Order = Cookie.Value;
                var strOrder = HttpContext.Current.Server.UrlDecode(Order);
                var ListOrder = JsonConvert.DeserializeObject<List<CookieOrder>>(strOrder);
                if(ListOrder != null && ListOrder.Any())
                {
                    
                    var temp = ListOrder.FirstOrDefault(x => !string.IsNullOrEmpty(x.CusId) && x.CusId.Equals(_CurrentUser.UserId));
                    if(temp != null)
                    {
                        var _OrderId = temp.OrderId;
                        HttpContext.Current.Session["ORDERID"] = _OrderId;
                        //if (HttpContext.Current.Session["ORDERID"] != null && string.IsNullOrEmpty(HttpContext.Current.Session["ORDERID"].ToString()))
                        //{
                        //    HttpContext.Current.Session["ORDERID"] = _OrderId;
                        //}
                        //else
                        //{

                        //}
                    }
                }

            }

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
                    return true;
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("IsPermission: ", ex);
                return false;
            }
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            string controller = filterContext.RouteData.Values["controller"].ToString();
            string action = filterContext.RouteData.Values["action"].ToString();
            if(controller.Equals("YourCart") && action.Equals("Index"))
            {
            }
            else
            {
                if (!_CurrentUser.IsAuthenticated)
                {

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
                                action = "Index",
                                controller = "Home",
                                //area = string.Empty,
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
                                //area = string.Empty,
                                isAjax = filterContext.HttpContext.Request.IsAjaxRequest(),
                                returnUrl = filterContext.HttpContext.Request.Url.ToString()
                            })
                        );
                    }
                }
                else
                {
                    filterContext.Result = new RedirectToRouteResult(
                       new RouteValueDictionary(
                           new
                           {
                               controller = "Home",
                               action = "Index",
                               //area = string.Empty,
                           })
                       );
                }
            }
            
        }
    }

    public class CookieOrder
    {
        public string CusId { get; set; }
        public string OrderId { get; set; }
        public int Qty { get; set; }
    }
}