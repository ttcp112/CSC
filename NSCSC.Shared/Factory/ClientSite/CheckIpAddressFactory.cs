using NSCSC.Shared.Models;
using NSCSC.Shared.Utilities;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSCSC.Shared.Factory.ClientSite
{
   public class CheckIpAddressFactory : BaseFactory
    {
        private BaseFactory _baseFactory = null;
        public CheckIpAddressFactory()
        {
            _baseFactory = new BaseFactory();
        }

        public string GetCountryCode(string Ip="")
        {
            string CoutryCode = "";
            try
            {
                string APIConutry = ConfigurationManager.AppSettings["RestCountriesCodeApi"] ?? "";
                string listData = "";
                //listData = (string)ApiResponse.GetWithoutHostConfigString<string>(APIConutry + Ip, null, null);
                listData = (string)ApiResponse.GetWithoutHostConfigString<string>(APIConutry, null, null);
                if(!string.IsNullOrEmpty(listData))
                {
                     CoutryCode = listData.Replace("<br>", "/").Split('/')[1];
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("GetCountryCode : ", ex);
            }
            return CoutryCode;
        }

        public string GetIPAddress()
        {
            // Gets the current context
            System.Web.HttpContext context = System.Web.HttpContext.Current;

            // Checks the HTTP_X_FORWARDED_FOR Header (which can be multiple IPs)
            string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            //If that is not empty
            if (!string.IsNullOrEmpty(ipAddress))
            {
                // Grab the first address
                string[] addresses = ipAddress.Split(',');
                if (addresses.Length != 0)
                {
                    return addresses[0];
                }
            }

            // Otherwise use the REMOTE_ADDR Header
            return context.Request.ServerVariables["REMOTE_ADDR"];
        }
    }
}
