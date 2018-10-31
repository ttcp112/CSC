using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace NSCSC.Shared.Utilities
{
    public class DateTimeModelHelper : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var displayFormat = bindingContext.ModelMetadata.DisplayFormatString;
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (!string.IsNullOrEmpty(displayFormat) && value != null)
            {
                DateTime date;
                displayFormat = displayFormat.Replace("{0:", string.Empty).Replace("}", string.Empty);

                try
                {

                    date = DateTime.ParseExact(value.AttemptedValue, displayFormat, null);

                    return date;
                }
                catch (Exception ex)
                {
                    bindingContext.ModelState.AddModelError(
                       bindingContext.ModelName,
                       string.Format("{0} is an invalid date format", value.AttemptedValue)
                   );

                }
            }

            return base.BindModel(controllerContext, bindingContext);
        }

        /*editor by trongntn 01/0182017 */
        public static DateTime GetDateImport(string sDateTime, ref string msg)
        {
            DateTime date = DateTime.Now;
            try
            {
                string[] arrDate = sDateTime.Split(' ')[0].Split('/');
                int day = int.Parse(arrDate[0]);
                int month = int.Parse(arrDate[1]);
                int year = int.Parse(arrDate[2]);
                date = new DateTime(year, month, day);
            }
            catch (Exception ex)
            {
                msg = ex.Message.ToString();
            }
            return date;
        }

        public static DateTime GetTimeImport(string sTime, ref string msg)
        {
            DateTime date = Commons.MinDate;
            try
            {
                string[] arrTime = sTime.Split(':');
                int hh = int.Parse(arrTime[0]);
                int mm = int.Parse(arrTime[1]);

                date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hh, mm, 59);
            }
            catch (Exception ex)
            {
                msg = ex.Message.ToString();
            }
            return date;
        }
    }
}
