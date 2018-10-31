using Newtonsoft.Json;
using NSCSC.Shared.Models;
using NSCSC.Shared.Models.SandBox.Inventory.Discount;
using NSCSC.Shared.Utilities;
using System;
using System.Collections.Generic;

namespace NSCSC.Shared.Factory.SandBox.Inventory
{
    public class DiscountFactory : BaseFactory
    {
        private BaseFactory _baseFactory = null;
        public DiscountFactory()
        {
            _baseFactory = new BaseFactory();
        }

        /// <summary>
        /// Result will return list Discount
        /// </summary>
        /// <param name="SearchString"></param>
        /// <returns></returns>
        public List<DiscountModels> GetListData(string SearchString = null)
        {
            List<DiscountModels> listData = new List<DiscountModels>();
            try
            {
                DiscountRequest paraBody = new DiscountRequest();
                paraBody.SearchString = SearchString;
                NSLog.Logger.Info("DiscountGetListData Request: ", paraBody);

                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.DiscountAPIGetList, null, paraBody);
                dynamic data = result.Data;
                var lstDataRaw = data["ListDiscount"];
                var lstObject = JsonConvert.SerializeObject(lstDataRaw);
                listData = JsonConvert.DeserializeObject<List<DiscountModels>>(lstObject);

                NSLog.Logger.Info("DiscountGetListData", listData);

                return listData;
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("DiscountGetListData_Fail", e);
                return listData;
            }
        }

        /// <summary>
        /// Result will return Discount
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public DiscountDetailModels GetDetail(string ID)
        {
            DiscountDetailModels item = new DiscountDetailModels();
            try
            {
                DiscountRequest paraBody = new DiscountRequest();
                paraBody.ID = ID;
                NSLog.Logger.Info("DiscountDetail Request: ", paraBody);
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.DiscountAPIGetDetail, null, paraBody);
                dynamic data = result.Data;
                var lstDataRaw = data["DiscountDetail"];
                var lstObject = JsonConvert.SerializeObject(lstDataRaw);
                item = JsonConvert.DeserializeObject<DiscountDetailModels>(lstObject);
                if (item != null)
                {
                    if (item.PeriodDate == null)
                    {
                        item.PeriodDate = DateTime.Now;
                    }
                    item.PeriodDate = CommonHelper.ConvertToLocalTime(item.PeriodDate);
                    //item.ApplyFrom = CommonHelper.ConvertToLocalTime(item.ApplyFrom);
                    //item.ApplyTo = CommonHelper.ConvertToLocalTime(item.ApplyTo);
                }
                NSLog.Logger.Info("DiscountGetDetail", item);

                return item;
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("DiscountGetDetail_Fail", e);
                return item;
            }
        }

        /// <summary>
        /// Create Or Edit Discount
        /// </summary>
        /// <param name="model"></param>
        /// <param name="msg"></param>
        /// <returns> return value type bool </returns>
        public bool CreateOrEdit(DiscountDetailModels model, ref string msg)
        {
            try
            {
                DiscountRequest paraBody = new DiscountRequest();
                if (model.PeriodDate != null)
                {
                    model.PeriodDate = CommonHelper.ConvertToUTCTime(model.PeriodDate);
                }

                //model.ApplyFrom = CommonHelper.ConvertToUTCTime(model.ApplyFrom);
                //model.ApplyTo = CommonHelper.ConvertToUTCTime(model.ApplyTo);
                paraBody.DiscountDetail = model;
                paraBody.CreatedUser = model.CreateUser;

                NSLog.Logger.Info("DiscountCreateOrEdit Request: ", paraBody);
                //====================
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.DiscountAPICreateOrEdit, null, paraBody);
                if (result != null)
                {
                    if (result.Success)
                        return true;
                    else
                    {
                        msg = result.Message;
                        NSLog.Logger.Info("DiscountCreateOrEdit", result.Message);
                        return false;
                    }
                }
                else
                {
                    NSLog.Logger.Info("DiscountCreateOrEdit", result);
                    return false;
                }
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("DiscountCreateOrEdit_Fail", e);
                return false;
            }
        }

        /// <summary>
        /// Delete Discount
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="CreateUser"></param>
        /// <param name="msg"></param>
        /// <returns> return value type bool </returns>
        public bool Delete(string ID, string CreateUser, ref string msg, string ReasonDelete = null)
        {
            try
            {
                DiscountRequest paraBody = new DiscountRequest();
                paraBody.CreatedUser = CreateUser;
                paraBody.ReasonDelete = ReasonDelete;
                paraBody.ID = ID;

                NSLog.Logger.Info("DiscountDelete Request: ", paraBody);
                //====================
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.DiscountAPIDelete, null, paraBody);
                if (result != null)
                {
                    if (result.Success)
                        return true;
                    else
                    {
                        msg = result.Message;
                        NSLog.Logger.Info("DiscountDelete", result.Message);
                        return false;
                    }
                }
                else
                {
                    NSLog.Logger.Info("DiscountDelete", result);
                    return false;
                }
            }
            catch (Exception e)
            {
                msg = e.ToString();
                NSLog.Logger.Error("DiscountDelete_Fail", e);
                return false;
            }
        }

        /// <summary>
        /// Generate Code for Discount
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="NumberCode"></param>
        /// <param name="CreatedUser"></param>
        /// <param name="msg"></param>
        /// <returns> return value type bool </returns>
        public bool GenerateCode(string ID, int NumberCode, string CreatedUser, ref string msg)
        {
            try
            {
                DiscountRequest paraBody = new DiscountRequest();
                paraBody.ID = ID;
                paraBody.NumberCode = NumberCode;
                paraBody.CreatedUser = CreatedUser;

                NSLog.Logger.Info("DiscountGenerateCode Request: ", paraBody);
                //====================
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.DiscountAPIGenerate, null, paraBody);
                if (result != null)
                {
                    if (result.Success)
                        return true;
                    else
                    {
                        msg = result.Message;
                        NSLog.Logger.Info("DiscountGenerateCode", result.Message);
                        return false;
                    }
                }
                else
                {
                    NSLog.Logger.Info("DiscountGenerateCode", result);
                    return false;
                }
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("DiscountGenerateCode_Fail", e);
                return false;
            }
        }

        /// <summary>
        /// Block Or Unblock Discount from ListCodeID
        /// </summary>
        /// <param name="ListCodeID"></param>
        /// <param name="CreateUser"></param>
        /// <param name="msg"></param>
        /// <returns> return value type bool </returns>
        public bool BlockOrUnblock(List<string> ListCodeID, string CreateUser, ref string msg)
        {
            try
            {
                DiscountRequest paraBody = new DiscountRequest();
                paraBody.ListID = ListCodeID;
                paraBody.CreatedUser = CreateUser;

                NSLog.Logger.Info("DiscountBlockOrUnblock Request: ", paraBody);
                //====================
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.DiscountAPIBlockOrUnblock, null, paraBody);
                if (result != null)
                {
                    if (result.Success)
                        return true;
                    else
                    {
                        msg = result.Message;
                        NSLog.Logger.Info("DiscountBlockOrUnblock", result.Message);
                        return false;
                    }
                }
                else
                {
                    NSLog.Logger.Info("DiscountBlockOrUnblock", result);
                    return false;
                }
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("DiscountBlockOrUnblock_Fail", e);
                return false;
            }
        }

        //public StatusResponse Export(ref IXLWorksheet ws, List<string> lstStore)
        //{
        //    StatusResponse Response = new StatusResponse();
        //    try
        //    {
        //        List<DiscountDetailModels> listData = new List<DiscountDetailModels>();
        //        DiscountRequest paraBody = new DiscountRequest();
        //        paraBody.CreatedUser = Commons.CreateUser;
        //        var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.ExportDiscount, null, paraBody);
        //        dynamic data = result.Data;
        //        var lstDataRaw = data["ListDiscount"];
        //        var lstObject = JsonConvert.SerializeObject(lstDataRaw);
        //        listData = JsonConvert.DeserializeObject<List<DiscountDetailModels>>(lstObject);
        //        int cols = 8;
        //        //Header
        //        int row = 1;
        //        ws.Cell("A" + row).Value = "Index";
        //        ws.Cell("B" + row).Value = "Discount Name";
        //        ws.Cell("C" + row).Value = "Status";
        //        ws.Cell("D" + row).Value = "Discount Type";
        //        ws.Cell("E" + row).Value = "Amount";
        //        ws.Cell("F" + row).Value = "Allow Open Discount";
        //        ws.Cell("G" + row).Value = "Description";
        //        //Item
        //        row = 2;
        //        int countIndex = 1;
        //        foreach (var item in listData)
        //        {
        //            ws.Cell("A" + row).Value = countIndex;
        //            ws.Cell("B" + row).Value = item.Name;
        //            ws.Cell("C" + row).Value = item.IsActive ? "Active" : "InActive";
        //            //ws.Cell("D" + row).Value = item.Type == (byte)Commons.EValueType.Currency ? "Currency" : "Percent";
        //            ws.Cell("E" + row).Value = item.Value;
        //            //ws.Cell("F" + row).Value = item.IsAllowOpenValue ? "Yes" : "No";
        //            ws.Cell("G" + row).Value = item.Description;
        //            row++;
        //            countIndex++;
        //        }
        //        FormatExcelExport(ws, row, cols);
        //        Response.Status = true;
        //    }
        //    catch (Exception e)
        //    {
        //        Response.Status = false;
        //        Response.MsgError = e.Message;
        //    }
        //    finally
        //    {
        //    }
        //    return Response;
        //}
    }
}
