using System.Web;
using System.Web.Mvc;

namespace NSCSC.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());

            //Use MVC's ValidateInput attribute in global filter collection
            filters.Add(new ValidateInputAttribute(false)); //application wide
        }
    }
}
