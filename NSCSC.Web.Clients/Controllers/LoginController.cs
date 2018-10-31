using Newtonsoft.Json;
using NSCSC.Shared.Factory;
using NSCSC.Shared.Factory.ClientSite;
using NSCSC.Shared.Factory.Settings.Currency;
using NSCSC.Shared.Models;
using NSCSC.Shared.Models.Settings.Currency;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace NSCSC.Web.Clients.Controllers
{
    public class LoginController : ClientController
    {
        private CurrencyFactory _factory = null;
        private static string mController = "";
        public LoginController()
        {
            try
            {
                var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                ViewBag.Version = string.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);
            }
            catch
            {
            }
            _factory = new CurrencyFactory();
        }

        public ActionResult Index(bool isAjax = false, string returnUrl = null)
        {
            if (Session["UserClientSite"] != null)
                return RedirectToAction("Index", "Home");
            if (returnUrl != null)
            {
                var questionMarkIndex = returnUrl.IndexOf('?');
                string queryString = null;
                string url = returnUrl;
                if (questionMarkIndex != -1) // There is a QueryString
                {
                    url = returnUrl.Substring(0, questionMarkIndex);
                    queryString = returnUrl.Substring(questionMarkIndex + 1);
                }

                // Arranges
                var request = new HttpRequest(null, url, queryString);
                var response = new HttpResponse(new StringWriter());
                var httpContext = new HttpContext(request, response);

                var routeData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(httpContext));

                // Extract the data    
                var values = routeData.Values;
                mController = values["controller"].ToString();
                var actionName = values["action"];
                //var areaName = values["area"];
            }

            AccountLoginRequest model = new AccountLoginRequest();
            if (Request.UrlReferrer != null)
                model.ReturnUrl = Request.UrlReferrer.ToString();//get url before login
            if (isAjax)
                return RedirectToAction("Index", "Login");
            else
                return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Index(AccountLoginRequest model, string returnUrl = null)
        {
            try
            {
                if (Session["UserClientSite"] != null)
                    return RedirectToAction("Index", "Home");

                if (ModelState.IsValid)
                {
                    UserFactory factoy = new UserFactory();

                    ResponseAccountLogin User = factoy.LoginClient(model.Email, model.Password);
                    bool isValid = (User != null && !string.IsNullOrEmpty(User.ID));
                    if (isValid)
                    {
                        UserSession userSession = new UserSession();
                        userSession.Email = User.Email;
                        userSession.UserName = User.Name;
                        userSession.UserId = User.ID;
                        userSession.IsAuthenticated = true;
                        userSession.ImageUrl = User.ImageURL;
                        userSession.IsReseller = User.IsReseller;
                        userSession.CurrencySymbol = User.CurrencySymbol;
                        // Update 04/09/2018
                        userSession.ListProduct = User.ListProduct;
                        //currency
                        //userSession.CurrencySymbol = GetCurrency();
                        //Commons.CurrencySymbol = userSession.CurrencySymbol;
                        if (!string.IsNullOrEmpty(User.OrderID))
                        {
                            var Cookie = Request.Cookies["csc-order-v2"];
                            if (Cookie != null)
                            {
                                var Order = Cookie.Value;
                                var strOrder = Server.UrlDecode(Order);
                                var ListOrder = JsonConvert.DeserializeObject<List<CookieOrder>>(strOrder);
                                if (ListOrder != null && ListOrder.Any())
                                {
                                    /* Check merge order anynomus */
                                    var itemMerge = ListOrder.FirstOrDefault(x => !string.IsNullOrEmpty(x.OrderId) && string.IsNullOrEmpty(x.CusId));
                                    if (itemMerge != null && !string.IsNullOrEmpty(User.OrderID))
                                    {
                                        YourCartFactory _factoryYC = null;
                                        _factoryYC = new YourCartFactory();
                                        var msg = string.Empty;
                                        var result = _factoryYC.OrderMerge(itemMerge.OrderId, User.OrderID, ref msg);
                                        ListOrder.Remove(itemMerge);
                                        Cookie.Value = null;
                                        Response.Cookies.Add(Cookie);
                                        var strListOrderMerge = JsonConvert.SerializeObject(ListOrder);
                                        Cookie.Value = strListOrderMerge;
                                        Response.Cookies.Add(Cookie);
                                    }

                                    var temp = ListOrder.FirstOrDefault(x => x.OrderId.Equals(User.OrderID));
                                    if (temp != null)
                                    {
                                        var _OrderId = temp.OrderId;
                                        if (Session["ORDERID"] != null && string.IsNullOrEmpty(Session["ORDERID"].ToString()))
                                        {
                                            Session["ORDERID"] = _OrderId;
                                        }
                                        YourCartFactory _factory = new YourCartFactory();
                                        var modelYourCart = _factory.GetOrderDetail(_OrderId);
                                        if (modelYourCart.ListItems != null && modelYourCart.ListItems.Count > 0)
                                        {
                                            modelYourCart.TotalQuantity = (int)modelYourCart.ListItems.Sum(x => x.Quantity);
                                            Session["IsFree"] = modelYourCart.IsFree;
                                        }

                                        temp.Qty = modelYourCart.TotalQuantity;
                                    }
                                    else
                                    {
                                        //check cusid
                                        var orderTmp = ListOrder.Where(ww => ww.CusId == User.ID).FirstOrDefault();
                                        if (orderTmp != null)
                                        {
                                            orderTmp.OrderId = User.OrderID;
                                            orderTmp.Qty = User.NumOfItems;
                                        }
                                        else
                                        {
                                            var _OrderId = User.OrderID;
                                            if (Session["ORDERID"] != null && string.IsNullOrEmpty(Session["ORDERID"].ToString()))
                                            {
                                                Session["ORDERID"] = _OrderId;
                                            }
                                            YourCartFactory _factory = new YourCartFactory();
                                            var modelYourCart = _factory.GetOrderDetail(_OrderId);
                                            if (modelYourCart.ListItems != null && modelYourCart.ListItems.Count > 0)
                                            {
                                                modelYourCart.TotalQuantity = (int)modelYourCart.ListItems.Sum(x => x.Quantity);
                                                Session["IsFree"] = modelYourCart.IsFree;
                                            }

                                            var newtemp = new CookieOrder
                                            {
                                                CusId = User.ID,
                                                OrderId = User.OrderID,
                                                Qty = modelYourCart.TotalQuantity
                                            };
                                            ListOrder.Add(newtemp);
                                        }
                                    }

                                    //ListOrder.ForEach(x => {
                                    //    x.Qty = x.OrderId == User.OrderID ? temp.Qty : x.Qty;
                                    //});
                                }
                                else
                                {
                                    YourCartFactory _factory = new YourCartFactory();
                                    var modelYourCart = _factory.GetOrderDetail(User.OrderID);
                                    if (modelYourCart.ListItems != null && modelYourCart.ListItems.Count > 0)
                                    {
                                        modelYourCart.TotalQuantity = (int)modelYourCart.ListItems.Sum(x => x.Quantity);
                                        Session["IsFree"] = modelYourCart.IsFree;
                                    }
                                    ListOrder = new List<CookieOrder>()
                                    {
                                        new CookieOrder
                                        {
                                            CusId = User.ID,
                                            OrderId = User.OrderID,
                                            Qty = modelYourCart.TotalQuantity
                                        }
                                    };
                                }
                                //Cookie.Expires = DateTime.Now.AddDays(-1d);
                                // Reset value cookie
                                Cookie.Value = null;
                                Response.Cookies.Add(Cookie);
                                var strListOrder = JsonConvert.SerializeObject(ListOrder);
                                Cookie.Value = strListOrder;
                                Response.Cookies.Add(Cookie);
                            }
                            else
                            {
                                YourCartFactory _factory = new YourCartFactory();
                                var modelYourCart = _factory.GetOrderDetail(User.OrderID);
                                if (modelYourCart.ListItems != null && modelYourCart.ListItems.Count > 0)
                                {
                                    modelYourCart.TotalQuantity = (int)modelYourCart.ListItems.Sum(x => x.Quantity);
                                    Session["IsFree"] = modelYourCart.IsFree;
                                }
                                var ListOrder = new List<CookieOrder>()
                                {
                                    new CookieOrder
                                    {
                                        CusId = User.ID,
                                        OrderId = User.OrderID,
                                        Qty = modelYourCart.TotalQuantity
                                    }
                                };
                                var strListOrder = JsonConvert.SerializeObject(ListOrder);
                                Cookie = new HttpCookie("csc-order-v2");
                                Cookie.Value = strListOrder;
                                Response.Cookies.Add(Cookie);
                            }
                        }
                        else
                        {
                            var Cookie = Request.Cookies["csc-order-v2"];
                            if (Cookie != null)
                            {
                                var Order = Cookie.Value;
                                var strOrder = Server.UrlDecode(Order);
                                var ListOrder = JsonConvert.DeserializeObject<List<CookieOrder>>(strOrder);
                                if (ListOrder != null && ListOrder.Any())
                                {
                                    var temp = ListOrder.FirstOrDefault(x => x.CusId.Equals(User.ID));
                                    if (temp != null)
                                    {
                                        ListOrder.Remove(temp);
                                        Cookie.Value = null;
                                        Response.Cookies.Add(Cookie);
                                        var strListOrder = JsonConvert.SerializeObject(ListOrder);
                                        Cookie.Value = strListOrder;
                                        Response.Cookies.Add(Cookie);
                                    }
                                }
                                //Cookie.Expires = DateTime.Now.AddDays(-1d);
                                // Reset value cookie
                            }
                        }

                        Session.Add("UserClientSite", userSession);
                        Session.Remove("ListItemCategory");
                        //Update 04/09/2018
                        if (User.ListProduct != null && User.ListProduct.Any() && User.IsReseller)
                        {
                            return RedirectToAction("Cart", "YourCart");
                        }
                        //else
                        //{
                        return RedirectToAction("Index", "MyStoreAndBusiness");
                        //}
                        //if (!string.IsNullOrEmpty(mController))
                        //    return RedirectToAction("Index", mController);
                        //if (returnUrl == null)
                        //    return RedirectToAction("Index", "Home");
                        //else
                        //    return Redirect(returnUrl);
                    }
                    else
                    {
                        if (User != null && User.IsVerify == false)
                        {
                            return RedirectToAction("Verification", "SignUp", new { email = model.Email });
                        }
                        else
                        {
                            ModelState.AddModelError("", "Email/Password is incorrect!");
                            return View(model);
                        }
                    }
                }
                else
                {
                    return View(model);// Return Error page
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("Index : ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult LoginExtend(string email = null, string pwd = null)
        {
            try
            {
                NSLog.Logger.Info(string.Format("LoginExtend: Email {0}", email));
                if (Session["UserClientSite"] != null)
                    return RedirectToAction("Index", "Home");

                UserFactory factoy = new UserFactory();

                ResponseAccountLogin User = factoy.LoginClient(email, pwd, true);
                bool isValid = (User != null && !string.IsNullOrEmpty(User.ID));
                if (isValid)
                {
                    UserSession userSession = new UserSession();
                    userSession.Email = User.Email;
                    userSession.UserName = User.Name;
                    userSession.UserId = User.ID;
                    userSession.IsAuthenticated = true;
                    userSession.ImageUrl = User.ImageURL;
                    userSession.IsReseller = User.IsReseller;
                    // Update 04/09/2018
                    userSession.ListProduct = User.ListProduct;

                    if (!string.IsNullOrEmpty(User.OrderID))
                    {
                        var Cookie = Request.Cookies["csc-order-v2"];
                        if (Cookie != null)
                        {
                            var Order = Cookie.Value;
                            var strOrder = Server.UrlDecode(Order);
                            var ListOrder = JsonConvert.DeserializeObject<List<CookieOrder>>(strOrder);
                            if (ListOrder != null && ListOrder.Any())
                            {
                                /* Check merge order anynomus */
                                var itemMerge = ListOrder.FirstOrDefault(x => !string.IsNullOrEmpty(x.OrderId) && string.IsNullOrEmpty(x.CusId));
                                if (itemMerge != null && !string.IsNullOrEmpty(User.OrderID))
                                {
                                    YourCartFactory _factoryYC = null;
                                    _factoryYC = new YourCartFactory();
                                    var msg = string.Empty;
                                    var result = _factoryYC.OrderMerge(itemMerge.OrderId, User.OrderID, ref msg);
                                    ListOrder.Remove(itemMerge);
                                    Cookie.Value = null;
                                    Response.Cookies.Add(Cookie);
                                    var strListOrderMerge = JsonConvert.SerializeObject(ListOrder);
                                    Cookie.Value = strListOrderMerge;
                                    Response.Cookies.Add(Cookie);
                                }

                                var temp = ListOrder.FirstOrDefault(x => x.OrderId.Equals(User.OrderID));
                                if (temp != null)
                                {
                                    var _OrderId = temp.OrderId;
                                    if (Session["ORDERID"] != null && string.IsNullOrEmpty(Session["ORDERID"].ToString()))
                                    {
                                        Session["ORDERID"] = _OrderId;
                                    }
                                    YourCartFactory _factory = new YourCartFactory();
                                    var modelYourCart = _factory.GetOrderDetail(_OrderId);
                                    if (modelYourCart.ListItems != null && modelYourCart.ListItems.Count > 0)
                                    {
                                        modelYourCart.TotalQuantity = (int)modelYourCart.ListItems.Sum(x => x.Quantity);
                                        Session["IsFree"] = modelYourCart.IsFree;
                                    }

                                    temp.Qty = modelYourCart.TotalQuantity;
                                }
                                else
                                {
                                    //check cusid
                                    var orderTmp = ListOrder.Where(ww => ww.CusId == User.ID).FirstOrDefault();
                                    if (orderTmp != null)
                                    {
                                        orderTmp.OrderId = User.OrderID;
                                        orderTmp.Qty = User.NumOfItems;
                                    }
                                    else
                                    {
                                        var _OrderId = User.OrderID;
                                        if (Session["ORDERID"] != null && string.IsNullOrEmpty(Session["ORDERID"].ToString()))
                                        {
                                            Session["ORDERID"] = _OrderId;
                                        }
                                        YourCartFactory _factory = new YourCartFactory();
                                        var modelYourCart = _factory.GetOrderDetail(_OrderId);
                                        if (modelYourCart.ListItems != null && modelYourCart.ListItems.Count > 0)
                                        {
                                            modelYourCart.TotalQuantity = (int)modelYourCart.ListItems.Sum(x => x.Quantity);
                                            Session["IsFree"] = modelYourCart.IsFree;
                                        }

                                        var newtemp = new CookieOrder
                                        {
                                            CusId = User.ID,
                                            OrderId = User.OrderID,
                                            Qty = modelYourCart.TotalQuantity
                                        };
                                        ListOrder.Add(newtemp);
                                    }
                                }

                            }
                            else
                            {
                                YourCartFactory _factory = new YourCartFactory();
                                var modelYourCart = _factory.GetOrderDetail(User.OrderID);
                                if (modelYourCart.ListItems != null && modelYourCart.ListItems.Count > 0)
                                {
                                    modelYourCart.TotalQuantity = (int)modelYourCart.ListItems.Sum(x => x.Quantity);
                                    Session["IsFree"] = modelYourCart.IsFree;
                                }
                                ListOrder = new List<CookieOrder>()
                                    {
                                        new CookieOrder
                                        {
                                            CusId = User.ID,
                                            OrderId = User.OrderID,
                                            Qty = modelYourCart.TotalQuantity
                                        }
                                    };
                            }
                            //Cookie.Expires = DateTime.Now.AddDays(-1d);
                            // Reset value cookie
                            Cookie.Value = null;
                            Response.Cookies.Add(Cookie);
                            var strListOrder = JsonConvert.SerializeObject(ListOrder);
                            Cookie.Value = strListOrder;
                            Response.Cookies.Add(Cookie);
                        }
                        else
                        {
                            YourCartFactory _factory = new YourCartFactory();
                            var modelYourCart = _factory.GetOrderDetail(User.OrderID);
                            if (modelYourCart.ListItems != null && modelYourCart.ListItems.Count > 0)
                            {
                                modelYourCart.TotalQuantity = (int)modelYourCart.ListItems.Sum(x => x.Quantity);
                                Session["IsFree"] = modelYourCart.IsFree;
                            }
                            var ListOrder = new List<CookieOrder>()
                                {
                                    new CookieOrder
                                    {
                                        CusId = User.ID,
                                        OrderId = User.OrderID,
                                        Qty = modelYourCart.TotalQuantity
                                    }
                                };
                            var strListOrder = JsonConvert.SerializeObject(ListOrder);
                            Cookie = new HttpCookie("csc-order-v2");
                            Cookie.Value = strListOrder;
                            Response.Cookies.Add(Cookie);
                        }
                    }
                    else
                    {
                        var Cookie = Request.Cookies["csc-order-v2"];
                        if (Cookie != null)
                        {
                            var Order = Cookie.Value;
                            var strOrder = Server.UrlDecode(Order);
                            var ListOrder = JsonConvert.DeserializeObject<List<CookieOrder>>(strOrder);
                            if (ListOrder != null && ListOrder.Any())
                            {
                                var temp = ListOrder.FirstOrDefault(x => x.CusId.Equals(User.ID));
                                if (temp != null)
                                {
                                    ListOrder.Remove(temp);
                                    Cookie.Value = null;
                                    Response.Cookies.Add(Cookie);
                                    var strListOrder = JsonConvert.SerializeObject(ListOrder);
                                    Cookie.Value = strListOrder;
                                    Response.Cookies.Add(Cookie);
                                }
                            }

                        }
                    }

                    Session.Add("UserClientSite", userSession);
                    Session.Remove("ListItemCategory");
                    //Update 04/09/2018
                    if (User.ListProduct != null && User.ListProduct.Any())
                    {
                        return RedirectToAction("Cart", "YourCart");
                    }

                    else
                        return RedirectToAction("Index", "MyStoreAndBusiness");
                }
                else
                {

                    return RedirectToAction("Verification", "SignUp", new { email = email });

                }

            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("Index : ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }

        [HttpGet]
        public ActionResult Logout()
        {
            try
            {
                if (Session["UserClientSite"] == null)
                    return RedirectToAction("Index", "Login");

                FormsAuthentication.SignOut();
                Session.Remove("UserClientSite");
                ORDERID = "";
                Session.Remove("ListItemCategory");
                return RedirectToAction("Index", "Login");
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("Logout Error : ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }

        public ActionResult ForgotPassword()
        {
            return View();
        }

        public ActionResult ResetPassword(string Email)
        {
            try
            {
                if (Session["UserClientSite"] != null)
                    return RedirectToAction("Index", "Home");

                UserFactory factoy = new UserFactory();
                string msg = "";
                bool isForgot = factoy.ForgotPassword(Email, ref msg);
                if (isForgot)
                    return new HttpStatusCodeResult(HttpStatusCode.OK);
                else
                {
                    //return new HttpStatusCodeResult(HttpStatusCode.BadRequest, msg);
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return Content(msg);
                }

            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("ForgotAction : ", ex);
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Content("Processing Reset Password have error from server");
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Processing Reset Password have error from server");
            }
        }
        //get currency
        public string GetCurrency()
        {
            string currency = "";
            CurrencyViewModels model = new CurrencyViewModels();
            try
            {

                var listData = _factory.GetListData();
                listData = listData.Where(x => x.IsActive == true).ToList();
                model.ListItem = listData;
                currency = model.ListItem[0].Symbol;
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("CurrencySearch: ", e);
            }
            return currency;
        }
    }

    public class CookieOrder
    {
        public string CusId { get; set; }
        public string OrderId { get; set; }
        public int Qty { get; set; }
    }
}