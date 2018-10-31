using Newtonsoft.Json;
using NSCSC.Shared.Models;
using NSCSC.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSCSC.Shared.Factory
{ 
    public class PromotionsFactory
    {

        public List<PromotionModels> GetData()
        {
            List<PromotionModels> listData = new List<PromotionModels>();
            try
            {
                NSApiResponseBase paraBody = new NSApiResponseBase();
                NSLog.Logger.Info("Promotion GetData Request", paraBody);
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.PromotionAPIGetList, null, paraBody);
                NSLog.Logger.Info("Promotion GetData Response", result);
                dynamic data = result.Data;
                var lstC = data["ListPromo"];
                var lstContent = JsonConvert.SerializeObject(lstC);
                listData = JsonConvert.DeserializeObject<List<PromotionModels>>(lstContent);
                
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("Promotion GetData Error", e);
            }
            return listData;
        }

        public PromotionModels GetDetail(string id)
        {
            PromotionModels promotion = new PromotionModels();
            try
            {
                NSApiResponseBase paraBody = new NSApiResponseBase();
                paraBody.ID = id;
                NSLog.Logger.Info("Promotion GetDetail Request", paraBody);
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.PromotionAPIGetDetail, null, paraBody);
                NSLog.Logger.Info("Promotion GetDetail Response", result);
                dynamic data = result.Data;
                var lstC = data["PromoData"];
                var lstContent = JsonConvert.SerializeObject(lstC);
                promotion = JsonConvert.DeserializeObject<PromotionModels>(lstContent);

                // Convert timezone
                if (promotion != null)
                {
                    promotion.ApplyFrom = CommonHelper.ConvertToLocalTime(promotion.ApplyFrom);
                    promotion.ApplyTo = CommonHelper.ConvertToLocalTime(promotion.ApplyTo);
                }

            }
            catch (Exception e)
            {
                NSLog.Logger.Error("Promotion GetDetail Error", e);
            }
            return promotion;
        }
        public bool CreateOrEdit(PromotionModels promotion, ref string msg)
        {
            bool resultReturn = false;
            try
            {
                CreateOrEditPromotionRequest paraBody = new CreateOrEditPromotionRequest();

                // Convert timezone
                promotion.ApplyFrom = CommonHelper.ConvertToUTCTime(promotion.ApplyFrom);
                promotion.ApplyTo = CommonHelper.ConvertToUTCTime(promotion.ApplyTo);

                paraBody.PromoData = promotion;
                NSLog.Logger.Info("Promotion CreateOrEdit Request", paraBody);
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.PromotionAPICreateOrEdit, null, paraBody);
                NSLog.Logger.Info("Promotion CreateOrEdit Response", result);
                if (result != null)
                {
                    if (result.Success)
                        resultReturn = result.Success;
                    else
                    {
                        msg = result.Message;
                        //msg = Commons.ErrorMsg;
                        NSLog.Logger.Info("Promotion CreateOrEdit", result.Message);
                    }
                }
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("Promotion CreateOrEdit Error", e);
            }
            return resultReturn;
        }
        public bool Delete(string id,string Reason="")
        {
            bool resultReturn = false;
            try
            {
                NSApiResponseBase paraBody = new NSApiResponseBase();
                paraBody.ID = id;
                paraBody.ReasonDelete = Reason;
                NSLog.Logger.Info("Promotion Delete Request", paraBody);
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.PromotionAPIDelete, null, paraBody);
                NSLog.Logger.Info("Promotion Delete Response", result);
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
