using Newtonsoft.Json;
using NSCSC.Shared.Models;
using NSCSC.Shared.Models.CRM.OrderReceiptManagement;
using NSCSC.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSCSC.Shared.Factory.CRM
{
    public class OrderReceiptManagementFactory
    {
        public List<OrderReceiptManagementModels> GetListData()
        {
            List<OrderReceiptManagementModels> listData = new List<OrderReceiptManagementModels>();
            try
            {
                GetListReceiptRequest paraBody = new GetListReceiptRequest();

                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.CustomerReceiptGetList, null, paraBody);
                dynamic data = result.Data;
                var lstDataRaw = data["ListReceipt"];
                var lstObject = JsonConvert.SerializeObject(lstDataRaw);
                listData = JsonConvert.DeserializeObject<List<OrderReceiptManagementModels>>(lstObject);
                if (listData != null && listData.Any())
                {
                    listData.ForEach(x =>
                    {
                        x.Date = CommonHelper.ConvertToLocalTime(x.Date);
                    });
                }

                NSLog.Logger.Info("OrderReceiptManagementGetListReceipt", listData);

                return listData;
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("OrderReceiptManagementGetListReceipt_Fail", e);
                return listData;
            }
        }

        public ReceiptDetailDTO GetDetail(string ID)
        {
            ReceiptDetailDTO model = new ReceiptDetailDTO();
            try
            {
                GetReceiptDetailRequest paraBody = new GetReceiptDetailRequest();
                paraBody.ID = ID;
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.CustomerReceiptGetDetail, null, paraBody);
                dynamic data = result.Data;
                var lstDataRaw = data["ReceiptDetail"];
                var lstObject = JsonConvert.SerializeObject(lstDataRaw);
                model = JsonConvert.DeserializeObject<ReceiptDetailDTO>(lstObject);
                if (model != null)
                {
                    model.Date = CommonHelper.ConvertToLocalTime(model.Date);
                }
                NSLog.Logger.Info("OrderReceiptManagement_GetDetail", model);

                return model;
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("OrderReceiptManagement_GetDetail_Fail", e);
                return model;
            }
        }

        public bool CreateOrEdit(ReceiptDetailDTO model, string useid, ref string msg)
        {
            try
            {
                AddOrRemoveDeviceSerialNoRequest paraBody = new AddOrRemoveDeviceSerialNoRequest();
                //paraBody.Currency = model;
                paraBody.CreatedUser = useid;
                paraBody.ListSerialNo = model.ListSerialNo;
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.CustomerAssetAddserialnumber, null, paraBody);
                if (result != null)
                {
                    if (result.Success)
                        return true;
                    else
                    {
                        msg = result.Message;
                        //msg = Commons.ErrorMsg;
                        NSLog.Logger.Info("OrderReceiptManagement_CreateOrEdit", result.Message);
                        return false;
                    }
                }
                else
                {
                    NSLog.Logger.Info("OrderReceiptManagement_CreateOrEdit", result);
                    return false;
                }
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("OrderReceiptManagement_CreateOrEdit_Fail", e);
                return false;
            }
        }

        //public bool CheckSerialNumber(string serialNumber, string assetID, ref string msg)
        //{
        //    try
        //    {
        //        AddOrRemoveDeviceSerialNoRequest paraBody = new AddOrRemoveDeviceSerialNoRequest();
        //        paraBody.ID = assetID;
        //        paraBody.SerialNo = serialNumber;
        //        var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.CustomerAssetAddserialnumber, null, paraBody);
        //        if (result != null)
        //        {
        //            if (result.Success)
        //                return true;
        //            else
        //            {
        //                //msg = result.Message;
        //                msg = Commons.ErrorMsg;
        //                NSLog.Logger.Info("OrderReceiptManagement_CheckSerialNumber", result.Message);
        //                return false;
        //            }
        //        }
        //        else
        //        {
        //            NSLog.Logger.Info("OrderReceiptManagement_CheckSerialNumber", result);
        //            return false;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        NSLog.Logger.Error("OrderReceiptManagement_CheckSerialNumber_Fail", e);
        //        return false;
        //    }
        //}
    }
}
