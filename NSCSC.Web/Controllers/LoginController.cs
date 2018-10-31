using NSLog;
using log4net;
using NSCSC.Shared.Models;
using NSCSC.Shared.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing;
using NSCSC.Shared.Utilities;
using System.Configuration;
using System.Web.Routing;
using NSCSC.Shared;
using NSCSC.Shared.Factory.Settings.Currency;

namespace NSCSC.Web.Controllers
{
    public class LoginController : Controller
    {
        private static string mController = "";
        //private string _IntegrationLink = "";
        //private string _integrationPozzwebLink = "";

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
        }
        // GET: Login
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Index(bool isAjax = false, string returnUrl = null)
        {
            if (Session["User"] != null)
                return RedirectToAction("Index", "Home", new { area = "" });

            // Tuấn edit
            if (returnUrl != null)
            {
                //var fullUrl = Request.UrlReferrer.ToString();
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
                var areaName = values["area"];
            }

            

            LoginRequestModel model = new LoginRequestModel();
            if (isAjax)
                //return PartialView("_Login", model);
                return RedirectToAction("Index", "Login", new { area = "" });
            else
                return View(model);
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Index(LoginRequestModel model, string returnUrl = null)
        {
            try
            {
                if (Session["User"] != null)
                    return RedirectToAction("Index", "Home", new { area = "" });

                if (ModelState.IsValid)
                {
                    UserFactory factoy = new UserFactory();
                    CurrencyFactory _CurrencyFactory = new CurrencyFactory();

                    LoginResponseModel User = factoy.Login(model.Email, model.Password);
                    bool isValid = (User != null && !string.IsNullOrEmpty(User.EmployeeID));
                    if (isValid)
                    {
                        UserSession userSession = new UserSession();
                        userSession.Email = User.EmployeeEmail;
                        userSession.UserName = User.EmployeeName;
                        userSession.UserId = User.EmployeeID;
                        userSession.IsAuthenticated = true;
                        userSession.ImageUrl = User.EmployeeImageURL;
                        userSession.ListPermission = User.ListPermission;
                        userSession.PinCode = User.EMployeePincode;
                        //userSession.IsSuperAdmin = User.IsSuperAdmin;
                        //userSession.OrganizationId = User.MerchantID;
                        // userSession.OrganizationName = User.MerchantName;
                        userSession.RoleID = User.RoleID;
                        userSession.RoleName = User.RoleName;
                        userSession.RoleLevel = User.RoleLevel;

                        userSession.FTPPassword = User.FTPPassword;
                        userSession.FTPUser = User.FTPUser;
                        userSession.FTPWebImage = User.FTPWebImage;
                        userSession.PublicImages = User.PublicImages;
                        //get currency
                       
                        var obj = _CurrencyFactory.GetCurrencyCurrent();
                        if (obj != null)
                            userSession.CurrencySymbol = obj.Symbol;

                        Session.Add("User", userSession);
                        //Tuấn edit
                        if (!string.IsNullOrEmpty(mController))
                            return RedirectToAction("Index", mController, new { area = "" });
                        if (returnUrl == null)
                            return RedirectToAction("Index", "Home", new { area = "" });
                        else
                            return Redirect(returnUrl);


                    }
                    else
                    {
                        ModelState.AddModelError("", "Email/Password is incorrect!");
                        return View(model);
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

        public void SaveQRCodeImage(string filename)
        {
            try
            {
                var tmp = filename.Split('|');
                string imageUrl = string.Format("https://chart.apis.google.com/chart?cht=qr&chs={0}&chl={1}", CommonHelper.ReturnQRCodeSize(tmp[1]), tmp[0]);

                Stream stream = null;

                int bytesToRead = 10000;

                byte[] buffer = new Byte[bytesToRead];

                try
                {
                    HttpWebRequest fileReq = (HttpWebRequest)HttpWebRequest.Create(imageUrl);
                    HttpWebResponse fileResp = (HttpWebResponse)fileReq.GetResponse();

                    if (fileReq.ContentLength > 0)
                        fileResp.ContentLength = fileReq.ContentLength;
                    stream = fileResp.GetResponseStream();

                    var resp = System.Web.HttpContext.Current.Response;

                    resp.ContentType = "application/octet-stream";

                    resp.AddHeader("Content-Disposition", "attachment; filename=\"" + tmp[0] + ".png" + "\"");
                    resp.AddHeader("Content-Length", fileResp.ContentLength.ToString());

                    int length;
                    do
                    {
                        if (resp.IsClientConnected)
                        {
                            length = stream.Read(buffer, 0, bytesToRead);
                            resp.OutputStream.Write(buffer, 0, length);
                            resp.Flush();

                            //Clear the buffer
                            buffer = new Byte[bytesToRead];
                        }
                        else
                        {
                            // cancel the download if client has disconnected
                            length = -1;
                        }
                    } while (length > 0); //Repeat until no data is read
                }
                finally
                {
                    if (stream != null)
                    {
                        //Close the input stream
                        stream.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("Index : ", ex);
            }
        }
    }
}