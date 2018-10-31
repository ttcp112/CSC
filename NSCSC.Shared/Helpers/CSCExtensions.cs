using NSCSC.Shared.Utilities;
using System;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace NSCSC.Shared.Helpers
{
    public static class CSCExtensions
    {
        /// <summary>
        /// Function get Path for Area
        /// </summary>
        /// <param name="url"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ContentArea(this UrlHelper url, string path)
        {
            var area = url.RequestContext.RouteData.DataTokens["area"];

            if (area != null)
            {
                if (!string.IsNullOrEmpty(area.ToString()))
                    area = "Areas/" + area;
                // Simple checks for '~/' and '/' at the
                // beginning of the path.
                if (path.StartsWith("~/"))
                    path = path.Remove(0, 2);

                if (path.StartsWith("/"))
                    path = path.Remove(0, 1);

                path = path.Replace("../", string.Empty);

                return VirtualPathUtility.ToAbsolute("~/" + area + "/" + path);
            }
            return string.Empty;
        }

        /// <summary>
        /// Function return string for SubMenuCategory
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string SetStringEncrypt(string url)
        {
            string result = string.Empty;
            try
            {
                string[] listData = url.Split('?');
                string ControllerAction = listData[0];
                string queryString = listData[1];
                result = ControllerAction + "?q=" + CommonHelper.Encrypt(queryString.Replace("&","?"));
            }
            catch (Exception)
            {
            }
            return result;
        }

        /// <summary>
        /// Function ActionEncoded gen link URL
        /// </summary>
        /// <param name="url"></param>
        /// <param name="actionName"></param>
        /// <param name="controllerName"></param>
        /// <param name="routeValues"></param>
        /// <returns></returns>
        public static string ActionEncoded(this UrlHelper url, string actionName, string controllerName, object routeValues = null)
        {
            string result = string.Empty;
            try
            {
                string queryString = string.Empty;
                string htmlAttributesString = string.Empty;
                string AreaName = string.Empty;
                if (routeValues != null)
                {
                    RouteValueDictionary d = new RouteValueDictionary(routeValues);
                    for (int i = 0; i < d.Keys.Count; i++)
                    {
                        string elementName = d.Keys.ElementAt(i).ToLower();
                        if (elementName == "area")
                        {
                            AreaName = Convert.ToString(d.Values.ElementAt(i));
                            continue;
                        }
                        if (i > 0)
                        {
                            queryString += "?";
                        }
                        queryString += d.Keys.ElementAt(i) + "=" + d.Values.ElementAt(i);
                    }
                }
                //What is Entity Framework??
                StringBuilder ancor = new StringBuilder();
                NSLog.Logger.Info("ActionEncoded log"
                    , string.Format("AreaName: {0}, controllerName: {1}, actionName: {2}, queryString: {3}", AreaName, controllerName, actionName, queryString));
                if (AreaName != string.Empty)
                {
                    ancor.Append("/" + AreaName);
                }
                if (controllerName != string.Empty)
                {
                    ancor.Append("/" + controllerName);
                }

                if (actionName != "Index")
                {
                    ancor.Append("/" + actionName);
                }
                if (queryString != string.Empty)
                {
                    ancor.Append("?q=" + CommonHelper.Encrypt(queryString));
                }
                result = ancor.ToString();
                NSLog.Logger.Info("ActionEncoded result log" , result);
                return result;
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("ActionEncoded Error", ex);
                return result;
            }
        }

        /// <summary>
        /// Function ActionEncoded gen link URL - Default Controller
        /// </summary>
        /// <param name="url"></param>
        /// <param name="actionName"></param>
        /// <param name="routeValues"></param>
        /// <returns></returns>
        public static string ActionEncoded(this UrlHelper url, string actionName, object routeValues = null)
        {
            string result = string.Empty;
            try
            {
                string queryString = string.Empty;
                string htmlAttributesString = string.Empty;
                string AreaName = string.Empty;
                if (routeValues != null)
                {
                    RouteValueDictionary d = new RouteValueDictionary(routeValues);
                    for (int i = 0; i < d.Keys.Count; i++)
                    {
                        string elementName = d.Keys.ElementAt(i).ToLower();
                        if (elementName == "area")
                        {
                            AreaName = Convert.ToString(d.Values.ElementAt(i));
                            continue;
                        }
                        if (i > 0)
                        {
                            queryString += "?";
                        }
                        queryString += d.Keys.ElementAt(i) + "=" + d.Values.ElementAt(i);
                    }
                }
                //What is Entity Framework??
                StringBuilder ancor = new StringBuilder();
                if (AreaName != string.Empty)
                {
                    ancor.Append("/" + AreaName);
                }
                //string alias = Request.RequestContext.RouteData.Values["controller"].ToString();
                string controllerName = HttpContext.Current.Request.RequestContext.RouteData.Values["controller"].ToString();
                if (controllerName != string.Empty)
                {
                    ancor.Append("/" + controllerName);
                }
                if (actionName != "Index")
                {
                    ancor.Append("/" + actionName);
                }
                if (queryString != string.Empty)
                {
                    ancor.Append("?q=" + CommonHelper.Encrypt(queryString));
                }
                result = ancor.ToString();
                return result;
            }
            catch (Exception)
            {
                return result;
            }
        }

        /// <summary>
        /// Function ActionEncoded gen Html
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="linkText"></param>
        /// <param name="actionName"></param>
        /// <param name="controllerName"></param>
        /// <param name="routeValues"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static MvcHtmlString ActionEncoded(this HtmlHelper htmlHelper, string linkText, string actionName, string controllerName, object routeValues, object htmlAttributes)
        {
            string queryString = string.Empty;
            string htmlAttributesString = string.Empty;
            string AreaName = string.Empty;
            if (routeValues != null)
            {
                RouteValueDictionary d = new RouteValueDictionary(routeValues);
                for (int i = 0; i < d.Keys.Count; i++)
                {
                    string elementName = d.Keys.ElementAt(i).ToLower();
                    if (elementName == "area")
                    {
                        AreaName = Convert.ToString(d.Values.ElementAt(i));
                        continue;
                    }
                    if (i > 0)
                    {
                        queryString += "?";
                    }
                    queryString += d.Keys.ElementAt(i) + "=" + d.Values.ElementAt(i);
                }
            }

            if (htmlAttributes != null)
            {
                RouteValueDictionary d = new RouteValueDictionary(htmlAttributes);
                for (int i = 0; i < d.Keys.Count; i++)
                {
                    htmlAttributesString += " " + d.Keys.ElementAt(i) + "=" + d.Values.ElementAt(i);
                }
            }

            //What is Entity Framework??
            StringBuilder ancor = new StringBuilder();
            ancor.Append("<a ");
            if (htmlAttributesString != string.Empty)
            {
                ancor.Append(htmlAttributesString);
            }
            ancor.Append(" href='");
            if (AreaName != string.Empty)
            {
                ancor.Append("/" + AreaName);
            }
            if (controllerName != string.Empty)
            {
                ancor.Append("/" + controllerName);
            }

            if (actionName != "Index")
            {
                ancor.Append("/" + actionName);
            }
            if (queryString != string.Empty)
            {
                ancor.Append("?q=" + CommonHelper.Encrypt(queryString));
            }
            ancor.Append("'");
            ancor.Append(">");
            ancor.Append(linkText);
            ancor.Append("");
            return new MvcHtmlString(ancor.ToString());
        }
    }
}
