using Newtonsoft.Json;
using NSCSC.Shared.Models;
using NSCSC.Shared.Models.Settings.PaymentMethods;
using NSCSC.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSCSC.Shared.Factory.Settings.PaymentMethods
{
    public class PaymentMethodFactory
    {
        public List<PaymentMethodModels> GetData()
        {
            List<PaymentMethodModels> listData = new List<PaymentMethodModels>();
            try
            {
                NSApiResponseBase paraBody = new NSApiResponseBase();
                NSLog.Logger.Info("Payment Method GetData Request", paraBody);
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.paymentMethodAPIGet, null, paraBody);
                NSLog.Logger.Info("Payment Method GetData Response", result);
                if(result != null)
                {
                    dynamic data = result.Data;
                    var lstC = data["ListPaymentMethod"];
                    var lstContent = JsonConvert.SerializeObject(lstC);
                    listData = JsonConvert.DeserializeObject<List<PaymentMethodModels>>(lstContent);
                }
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("Payment Method GetData Error", e);
            }
            return listData;
        }

        public bool CreateOrEdit(PaymentMethodModels paymentMethod, ref string msg)
        {
            bool resultReturn = false;
            try
            {
                CreateOrEditPaymentMethodRequest paraBody = new CreateOrEditPaymentMethodRequest();
                paraBody.PaymentMethod = paymentMethod;
                NSLog.Logger.Info("Payment method CreateOrEdit Request", paraBody);
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.paymentMethodAPICreateOrEdit, null, paraBody);
                NSLog.Logger.Info("Payment method CreateOrEdit Response", result);
                if (result != null)
                {
                    if (result.Success)
                        resultReturn = result.Success;
                    else
                    {
                        msg = result.Message;
                        //msg = Commons.ErrorMsg;
                        NSLog.Logger.Info("Payment method CreateOrEdit Error", result.Message);
                    }
                }
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("Payment method CreateOrEdit Error", e);
            }
            return resultReturn;
        }

        public bool Delete(string id, string Reason = "")
        {
            bool resultReturn = false;
            try
            {
                NSApiResponseBase paraBody = new NSApiResponseBase();
                paraBody.ID = id;
                paraBody.ReasonDelete = Reason;
                NSLog.Logger.Info("Payment Method Delete Request", paraBody);
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.paymentMethodAPIDelete, null, paraBody);
                NSLog.Logger.Info("Payment Method Delete Response", result);
                resultReturn = result.Success;

            }
            catch (Exception e)
            {
                NSLog.Logger.Error("Promotion Delete Error", e);
            }
            return resultReturn;
        }
    }
}
