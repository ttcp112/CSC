using RestSharp;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Script.Serialization;

namespace NSCSC.Shared.Utilities
{
    public class ApiResponse
    {
        private static string _hostApi = ConfigurationManager.AppSettings["HostPosAPI"];

        public static object Post<T>(string url, Dictionary<string, string> paramInfos, object paramBodys)
        {
            var client = new RestClient(_hostApi+"/" + url);
            var req = new RestRequest("");
            req.Method = Method.POST;
            if (paramInfos != null)
            {
                foreach (var param in paramInfos)
                {
                    req.AddParameter(param.Key, param.Value);
                }
            }
            if (paramBodys != null)
            {
                req.AddJsonBody(paramBodys);
            }
            var response = client.Execute(req);
            if (response.ResponseStatus == ResponseStatus.Completed)
            {
                JavaScriptSerializer jss = new JavaScriptSerializer();
                jss.MaxJsonLength = int.MaxValue;
                T responseObj = jss.Deserialize<T>(response.Content);
                return responseObj;
            }
            return null;
        }

        public static object Put<T>(string url, Dictionary<string, string> paramInfos, object paramBodys)
        {
            var client = new RestClient(_hostApi +"/"+ url);
            var req = new RestRequest("");
            req.Method = Method.PUT;
            if (paramInfos != null)
            {
                foreach (var param in paramInfos)
                {
                    req.AddParameter(param.Key, param.Value);
                }
            }
            if (paramBodys != null)
            {
                req.AddJsonBody(paramBodys);
            }
            var response = client.Execute(req);
            if (response.ResponseStatus == ResponseStatus.Completed)
            {
                JavaScriptSerializer jss = new JavaScriptSerializer();
                T responseObj = jss.Deserialize<T>(response.Content);
                return responseObj;
            }
            return null;
        }

        public static object Get<T>(string url, Dictionary<string, string> paramInfos, object paramBodys)
        {
            var client = new RestClient(_hostApi + url);
            var req = new RestRequest("");
            req.Method = Method.GET;
            if (paramInfos != null)
            {
                foreach (var param in paramInfos)
                {
                    req.AddParameter(param.Key, param.Value);
                }
            }
            if (paramBodys != null)
            {
                req.AddJsonBody(paramBodys);
            }
            var response = client.Execute(req);
            if (response.ResponseStatus == ResponseStatus.Completed)
            {
                JavaScriptSerializer jss = new JavaScriptSerializer();
                T responseObj = jss.Deserialize<T>(response.Content);
                return responseObj;
            }
            return null;
        }
        public static object GetWithoutHostConfig<T>(string url, Dictionary<string, string> paramInfos, object paramBodys)
        {
            var client = new RestClient(url);
            var req = new RestRequest("");
            req.Method = Method.GET;
            if (paramInfos != null)
            {
                foreach (var param in paramInfos)
                {
                    req.AddParameter(param.Key, param.Value);
                }
            }
            if (paramBodys != null)
            {
                req.AddJsonBody(paramBodys);
            }
            var response = client.Execute(req);
            if (response.ResponseStatus == ResponseStatus.Completed)
            {
                JavaScriptSerializer jss = new JavaScriptSerializer();
                T responseObj = jss.Deserialize<T>(response.Content);
                return responseObj;
            }
            return null;
        }


        public static object GetWithoutHostConfigString<T>(string url, Dictionary<string, string> paramInfos, object paramBodys)
        {
            var client = new RestClient(url);
            var req = new RestRequest("");
            req.Method = Method.GET;
            if (paramInfos != null)
            {
                foreach (var param in paramInfos)
                {
                    req.AddParameter(param.Key, param.Value);
                }
            }
            if (paramBodys != null)
            {
                req.AddJsonBody(paramBodys);
            }
            var response = client.Execute(req);
            if (response.ResponseStatus == ResponseStatus.Completed)
            {
                //JavaScriptSerializer jss = new JavaScriptSerializer();
                //T responseObj = response.Content;
                return response.Content;
            }
            return null;
        }
    }
}
