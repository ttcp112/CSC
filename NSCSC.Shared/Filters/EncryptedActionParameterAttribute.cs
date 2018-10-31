using NSCSC.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NSCSC.Shared.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class EncryptedActionParameterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Dictionary<string, object> decryptedParameters = new Dictionary<string, object>();
            if (HttpContext.Current.Request.QueryString.Get("q") != null)
            {
                string encryptedQueryString = HttpContext.Current.Request.QueryString.Get("q");
                string decrptedString = CommonHelper.Decrypt(encryptedQueryString.ToString());
                string[] paramsArrs = decrptedString.Split('?');

                for (int i = 0; i < paramsArrs.Length; i++)
                {
                    if (paramsArrs[i].Split('=').Length > 1)
                    {
                        string[] paramArr = paramsArrs[i].Split('=');
                        decryptedParameters.Add(paramArr[0], paramArr[1]);
                    }
                }
            }
            for (int i = 0; i < decryptedParameters.Count; i++)
            {
                filterContext.ActionParameters[decryptedParameters.Keys.ElementAt(i)] = decryptedParameters.Values.ElementAt(i);
            }
            base.OnActionExecuting(filterContext);
        }
    }
}
