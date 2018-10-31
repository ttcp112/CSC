using Newtonsoft.Json;
using NSCSC.Shared.Models;
using NSCSC.Shared.Models.Settings.Tax;
using NSCSC.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSCSC.Shared.Factory.Settings
{
    public class TaxFactory
    {
        public TaxFactory()
        {

        }
        // get list catagory
        public List<TaxModels> GetListCategory(string userid)
        {
            List<TaxModels> listData = new List<TaxModels>();
            try
            {
                TaxRequestModels paraBody = new TaxRequestModels();
                //paraBody.CreatedUser = userid;
                paraBody.SearchString = "";
                NSLog.Logger.Info("GetList Tax Request", paraBody);

                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.TaxAPIGetList, null, paraBody);
                NSLog.Logger.Info("GetList Tax Request", result);
                dynamic data = result.Data;
                var lstC = data["ListTax"];
                var lstContent = JsonConvert.SerializeObject(lstC);
                listData = JsonConvert.DeserializeObject<List<TaxModels>>(lstContent);
                NSLog.Logger.Info("TaxGetListData", listData);
                return listData;
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("TaxGetList_Fail: ", e);
                return listData;
            }
        }
        // Create Catagory
        public bool InsertOrUpdateTax(TaxModels model, string userid, ref string msg)
        {
            try
            {
                TaxRequestModels paraBody = new TaxRequestModels();
                paraBody.CreatedUser = userid;
                paraBody.ID = model.Id;
                paraBody.TaxData = model;
                NSLog.Logger.Info("Tax Update Request", paraBody);
                //====================

                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.TaxAPICreateOrEdit, null, paraBody);
                NSLog.Logger.Info("Tax Update Request", result);
                if (result != null)
                {
                    if (result.Success)
                        return true;
                    else
                    {
                        msg = result.Message;
                        NSLog.Logger.Info("TaxCreateorEdit", result.Message);
                        return false;
                    }
                }
                else
                {
                    NSLog.Logger.Info("TaxCreateorEdit", result);
                    return false;
                }
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("TaxInsertOrUpdate_Fail", e);
                return false;
            }
        }

        // Get Detail
        public TaxModels GetDetail(string ID)
        {
            List<TaxModels> LstTaxDTO = new List<TaxModels>();
            TaxModels TaxDTO = new TaxModels();
            try
            {
                TaxRequestModels paraBody = new TaxRequestModels();
                paraBody.ID = ID;
                NSLog.Logger.Info("Tax GetDetail Request", paraBody);
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.TaxAPIGetList, null, paraBody);
                NSLog.Logger.Info("Tax GetDetail Request", result);
                dynamic data = result.Data;
                var lstDataRaw = data["ListTax"];
                var lstContent = JsonConvert.SerializeObject(lstDataRaw);
                LstTaxDTO = JsonConvert.DeserializeObject<List<TaxModels>>(lstContent);
                NSLog.Logger.Info("TaxGetDetail", LstTaxDTO);
                TaxDTO = LstTaxDTO.FirstOrDefault();
                if (TaxDTO.ListProduct != null && TaxDTO.ListProduct.Count > 0)
                {
                    for (int i = 0; i < TaxDTO.ListProduct.Count; i++)
                    {
                        var item = TaxDTO.ListProduct[i];
                        item.OffSet = i;
                        TaxDTO.ItemDetail += item.Name + ",";
                    }
                }
                return TaxDTO;

            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("TaxGetDetail_Fail", ex);
                return null;
            }

        }

        //Delete
        public bool Delete(string ID, string UserId, ref string msg, string ReasonDelete = null)
        {
            try
            {
                TaxRequestModels Para = new TaxRequestModels();
                Para.ID = ID;
                Para.CreatedUser = UserId;
                Para.ReasonDelete = ReasonDelete;
                NSLog.Logger.Info("Tax Delete Request", Para);
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.TaxAPIDelete, null, Para);
                NSLog.Logger.Info("Tax Delete Request", result);
                if (result != null)
                {
                    if (result.Success)
                        return true;
                    else
                    {
                        msg = result.Message;
                        NSLog.Logger.Info("TaxDelete", result.Message);
                        return false;
                    }
                }
                else
                {
                    NSLog.Logger.Info("TaxDelete", result);
                    return false;
                }
            }
            catch (Exception e)
            {
                msg = e.ToString();
                NSLog.Logger.Error("Tax_Delete: ", e);
                return false;
            }
        }
    }
}
