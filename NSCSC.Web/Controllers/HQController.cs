using NSCSC.Shared.Models;
using System.Web.Mvc;
using System;
using System.Text.RegularExpressions;
using System.Configuration;
using NSCSC.Shared.Utilities;

namespace NSCSC.Web.Controllers
{
    public class HQController : Controller
    {
        public HQController()
        {
            ViewBag.CurrencySymbol = CurrentUser.CurrencySymbol;
        }
        public string List { get; set; }
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
        
        // Mark * email in view
        public string MarkEmailView(string Email)
        {
            if (!string.IsNullOrEmpty(Email))
            {
                int length = Email.Length;
                if (length > 4)
                {
                    string strMark = new String('*', length - 4);
                    strMark = "<span style='position:relative; top:3px;'>" + strMark + "</span>";
                    Email = Email.Substring(0, 4) + strMark;
                } 
            }
            return Email;
        }

        // Remove some special character: ";", ",", ".", "\r", "\t", "\n", " "
        public string RemoveSpecChar(string Text)
        {
            if (!string.IsNullOrEmpty(Text))
            {
                Regex pattern = new Regex("[;,.\t\r\n ]|[\n]{2}");
                Text = pattern.Replace(Text, "");
            }
            return Text;
        }

        public string CurrentCountryCode
        {
            get
            {
                string result = string.Empty;
                if (System.Web.HttpContext.Current.Session["_CurrentCountryCode"] == null)
                {
                    try
                    {
                        string APICountry = ConfigurationManager.AppSettings["RestCountriesCodeApi"] ?? "";
                        string listData = (string)ApiResponse.GetWithoutHostConfigString<string>(APICountry, null, null);
                        if (!string.IsNullOrEmpty(listData))
                        {
                            result = listData.Replace("<br>", "/").Split('/')[1];
                            Session.Add("_CurrentCountryCode", result);
                        }
                    }
                    catch (Exception ex)
                    {
                        NSLog.Logger.Error("GetCurrentCountryCode Error : ", ex);
                    }
                }
                else
                {
                    result = (string)System.Web.HttpContext.Current.Session["_CurrentCountryCode"];
                }
                return result;
            }
        }
    }
}