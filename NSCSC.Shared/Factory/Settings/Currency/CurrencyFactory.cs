using Newtonsoft.Json;
using NSCSC.Shared.Models;
using NSCSC.Shared.Models.Settings.Currency;
using NSCSC.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSCSC.Shared.Factory.Settings.Currency
{
    public class CurrencyFactory
    {
        public List<CurrencyModels> GetListData()
        {
            List<CurrencyModels> listData = new List<CurrencyModels>();
            try
            {
                GetCurrencyRequest paraBody = new GetCurrencyRequest();

                NSLog.Logger.Info("Currency Get ListData Request", paraBody);
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.CurrencyAPIGetList, null, paraBody);
                NSLog.Logger.Info("Currency Get List Data Response", paraBody);
                dynamic data = result.Data;
                var lstDataRaw = data["ListCurrency"];
                var lstObject = JsonConvert.SerializeObject(lstDataRaw);
                listData = JsonConvert.DeserializeObject<List<CurrencyModels>>(lstObject);

                NSLog.Logger.Info("CurrencyGetListData", listData);

                return listData;
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("CurrencyGetListData_Fail", e);
                return listData;
            }
        }

        public CurrencyModels GetCurrencyCurrent()
        {
            CurrencyModels obj = new CurrencyModels();
            try
            {
                GetCurrencyRequest paraBody = new GetCurrencyRequest();
                paraBody.IsActive = true;

                NSLog.Logger.Info("GetCurrencyCurrent Request", paraBody);
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.CurrencyAPIGetList, null, paraBody);
                NSLog.Logger.Info("GetCurrencyCurrent Response", paraBody);
                dynamic data = result.Data;
                var lstDataRaw = data["ListCurrency"];
                var lstObject = JsonConvert.SerializeObject(lstDataRaw);
                List<CurrencyModels> listData = JsonConvert.DeserializeObject<List<CurrencyModels>>(lstObject);
                if (listData != null && listData.Any())
                {
                    obj = listData.FirstOrDefault();
                }
                return obj;
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("GetCurrencyCurrent_Fail", e);
                return obj;
            }
        }


        public CurrencyDTO GetDetail(string ID)
        {
            CurrencyDTO model = new CurrencyDTO();
            try
            {
                GetCurrencyRequest paraBody = new GetCurrencyRequest();
                paraBody.ID = ID;
                NSLog.Logger.Info("Currency Get Detail Request", paraBody);
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.CurrencyAPIGetList, null, paraBody);
                NSLog.Logger.Info("Currency Get Detail Response", paraBody);
                dynamic data = result.Data;
                var lstDataRaw = data["ListCurrency"];
                var lstObject = JsonConvert.SerializeObject(lstDataRaw);
                model = JsonConvert.DeserializeObject<List<CurrencyDTO>>(lstObject)[0];

                NSLog.Logger.Info("Currency_GetDetail", model);

                return model;
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("Currency_GetDetail_Fail", e);
                return model;
            }
        }

        public bool CreateOrEdit(CurrencyDTO model, string useid, ref string msg)
        {
            try
            {
                CreateOrEditCurrencyRequest paraBody = new CreateOrEditCurrencyRequest();
                paraBody.Currency = model;
                paraBody.CreatedUser = useid;
                NSLog.Logger.Info("Currency Create Or Edit Request", paraBody);
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.CurrencyAPICreateOrEdit, null, paraBody);
                NSLog.Logger.Info("Currency Create Or Edit Response", paraBody);
                if (result != null)
                {
                    if (result.Success)
                        return true;
                    else
                    {
                        msg = result.Message;
                        //msg = Commons.ErrorMsg;
                        NSLog.Logger.Info("Currency_CreateOrEdit", result.Message);
                        return false;
                    }
                }
                else
                {
                    NSLog.Logger.Info("Currency_CreateOrEdit", result);
                    return false;
                }
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("Currency_CreateOrEdit_Fail", e);
                return false;
            }
        }

        public bool Delete(string ID, string CreateUser, ref string msg, string ReasonDelete = null)
        {
            try
            {
                DeleteCurrencyRequest paraBody = new DeleteCurrencyRequest();
                paraBody.CreatedUser = CreateUser;
                paraBody.ReasonDelete = ReasonDelete;
                paraBody.ID = ID;
                NSLog.Logger.Info("Currency Delete Request", paraBody);
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.CurrencyAPIDelete, null, paraBody);
                NSLog.Logger.Info("Currency Delete Response", paraBody);
                if (result != null)
                {
                    if (result.Success)
                        return true;
                    else
                    {
                        //msg = result.Message;
                        msg = Commons.ErrorMsg;
                        NSLog.Logger.Info("Currency_Delete", result.Message);
                        return false;
                    }
                }
                else
                {
                    NSLog.Logger.Info("Currency_Delete", result);
                    return false;
                }
            }
            catch (Exception e)
            {
                msg = e.ToString();
                NSLog.Logger.Error("Currency_Delete_Fail", e);
                return false;
            }
        }
    }
}
