using NSCSC.Shared;
using NSCSC.Shared.Factory.ManagementTool;
using NSCSC.Shared.Factory.Settings.Currency;
using NSCSC.Shared.Models;
using NSCSC.Shared.Models.ManagementTool.ClientModule;
using NSCSC.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace NSCSC.Web.Clients.Controllers
{
    public class ClientController : Controller
    {
        /// <summary>
        /// Variable for Order
        /// </summary>
        public string ORDERID
        {
            get
            {
                if (System.Web.HttpContext.Current.Session["ORDERID"] != null)
                    return System.Web.HttpContext.Current.Session["ORDERID"].ToString();
                else
                    return "";
            }
            set
            {
                Session.Add("ORDERID", value);
            }
        }

        public ActionResult GetORDERID(string _ORDERID)
        {
            ORDERID = _ORDERID;
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        public UserSession CurrentUser
        {
            get
            {
                if (System.Web.HttpContext.Current.Session["UserClientSite"] != null)
                    return (UserSession)System.Web.HttpContext.Current.Session["UserClientSite"];
                else
                    return new UserSession();
            }
        }

        public ClientPackageSession ClientPackageSession
        {
            get
            {
                if (System.Web.HttpContext.Current.Session["ClientPackageSessionObj"] != null)
                    return (ClientPackageSession)System.Web.HttpContext.Current.Session["ClientPackageSessionObj"];
                else
                {
                    var obj = new ClientPackageSession();
                    ClientModuleFactory _ClientModuleFactory = new ClientModuleFactory();
                    var result = _ClientModuleFactory.GetClientModule();
                    if (result != null && result.Any())
                    {
                        var package = result.Where(ww => ww.Code == (int)Commons.EClientModule.Package).FirstOrDefault();
                        if (package != null)
                        {
                            obj.ID = package.ID;
                            obj.Name = package.Name;
                            obj.Description = package.Description;
                            obj.ImageUrl = package.ImageUrl;
                            obj.VideoUrl = package.VideoUrl;

                            Session.Add("ClientPackageSessionObj", obj);
                        }
                    }
                    return obj;
                }
            }

        }

        public List<ClientModuleModel> ClientStartBusinessSession
        {
            get
            {
                if (System.Web.HttpContext.Current.Session["ClientStartBusinessSessionObj"] != null)
                    return (List<ClientModuleModel>)System.Web.HttpContext.Current.Session["ClientStartBusinessSessionObj"];
                else
                {
                    var lstResult = new List<ClientModuleModel>();
                    ClientModuleFactory _ClientModuleFactory = new ClientModuleFactory();
                    var result = _ClientModuleFactory.GetClientModule();
                    if (result != null && result.Any())
                    {
                        lstResult = result.Where(ww => ww.Code == (int)Commons.EClientModule.StartBusiness).ToList();
                        if (lstResult != null && lstResult.Any())
                        {
                            Session.Add("ClientStartBusinessSessionObj", lstResult);
                        }
                    }
                    return lstResult;
                }
            }

        }

        public string CurrencySymbol
        {
            get
            {
                string currency = string.Empty;
                if (System.Web.HttpContext.Current.Session["_NSCurrencySymbol"] == null)
                {
                    CurrencyFactory _CurrencyFactory = new CurrencyFactory();
                    var obj = _CurrencyFactory.GetCurrencyCurrent();
                    if (obj != null)
                        currency = obj.Symbol;

                    Session.Add("_NSCurrencySymbol", currency);
                }
                else
                {
                    currency = (string)System.Web.HttpContext.Current.Session["_NSCurrencySymbol"];
                }
                return currency;
            }
        }

        #region /*Default Image Dummy for Page*/
        public string _ImgDummyCustomer
        {
            get
            {
                return Url.Content("~/images/dummy/customer.jpg");
            }
        }
        public string _ImgDummyPackage
        {
            get
            {
                return Url.Content("~/images/dummy/package.jpg");
            }
        }
        public string _ImgDummyProduct
        {
            get
            {
                return Url.Content("~/images/dummy/product.jpg");
            }
        }
        public string _ImgDummyStore
        {
            get
            {
                return Url.Content("~/images/dummy/store.jpg");
            }
        }
        #endregion

        public string CountryCode
        {
            get
            {
                string result = string.Empty;
                if (System.Web.HttpContext.Current.Session["_CountryCode"] == null) {
                    try
                    {
                        string APIConutry = ConfigurationManager.AppSettings["RestCountriesCodeApi"] ?? "";
                        string listData = (string)ApiResponse.GetWithoutHostConfigString<string>(APIConutry, null, null);
                        if (!string.IsNullOrEmpty(listData))
                        {
                            result = listData.Replace("<br>", "/").Split('/')[1];
                            Session.Add("_CountryCode", result);
                        }
                    }
                    catch (Exception ex)
                    {
                        NSLog.Logger.Error("GetCountryCode : ", ex);
                    }
                }
                else
                {
                    result = (string)System.Web.HttpContext.Current.Session["_CountryCode"];
                }
                return result;
            }
        }
    }
}