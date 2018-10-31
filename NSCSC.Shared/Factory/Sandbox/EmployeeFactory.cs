using Newtonsoft.Json;
using NSCSC.Shared.Models;
using NSCSC.Shared.Models.Sandbox.Employee;
using NSCSC.Shared.Utilities;
using System;
using System.Collections.Generic;
using static NSCSC.Shared.Models.Sandbox.Employee.EmployeeRequest;

namespace NSCSC.Shared.Factory.Sandbox
{
    public class EmployeeFactory : BaseFactory
    {
        private BaseFactory _baseFactory = null;
        public EmployeeFactory()
        {
            _baseFactory = new BaseFactory();
        }

        public List<EmployeeModels> GetListEmployee(string UserId, int Level, string SearchString = null)
        {
            List<EmployeeModels> listData = new List<EmployeeModels>();
            try
            {
                EmployeeRequest paraBody = new EmployeeRequest();
                paraBody.SearchString = SearchString;
                paraBody.Level = Level;
                paraBody.CreatedUser = UserId;
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.EmployeeAPIGetList, null, paraBody);
                dynamic data = result.Data;
                var lstDataRaw = data["ListEmployee"];
                var lstContent = JsonConvert.SerializeObject(lstDataRaw);
                listData = JsonConvert.DeserializeObject<List<EmployeeModels>>(lstContent);

                NSLog.Logger.Info("Employee_GetList", listData);

                return listData;
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("Employee GetListEmployee error", e);
                return listData;
            }
        }

        public EmployeeDetailModels GetDetail(string ID)
        {
            EmployeeDetailModels item = new EmployeeDetailModels();
            try
            {
                EmployeeRequest paraBody = new EmployeeRequest();
                paraBody.ID = ID;
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.EmployeeAPIGetDetail, null, paraBody);
                dynamic data = result.Data;
                var lstDataRaw = data["EmployeeDetail"];
                var lstObject = JsonConvert.SerializeObject(lstDataRaw);
                item = JsonConvert.DeserializeObject<EmployeeDetailModels>(lstObject);
                if (item != null)
                {
                    item.Birthday = CommonHelper.ConvertToLocalTime(item.Birthday);
                    item.HiredDate = CommonHelper.ConvertToLocalTime(item.HiredDate);
                    item.ExpiredDate = CommonHelper.ConvertToLocalTime(item.ExpiredDate);
                }
                NSLog.Logger.Info("Employee_GetDetail", item);

                return item;
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("Employee_GetDetail_Fail", e);
                return item;
            }
        }

        public bool CreateOrEdit(EmployeeDetailModels model, ref string msg)
        {
            try
            {
                EmployeeRequest paraBody = new EmployeeRequest();

                model.Birthday = CommonHelper.ConvertToUTCTime(model.Birthday);
                model.HiredDate = CommonHelper.ConvertToUTCTime(model.HiredDate);
                model.ExpiredDate = CommonHelper.ConvertToUTCTime(model.ExpiredDate);

                paraBody.EmployeeDetail = model;
                paraBody.CreatedUser = model.CreateUser;
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.EmployeeAPICreateOrEdit, null, paraBody);
                if (result != null)
                {
                    if (result.Success)
                        return true;
                    else
                    {
                        msg = result.Message;
                        //msg = Commons.ErrorMsg;
                        NSLog.Logger.Info("Employee_CreateOrEdit", result.Message);
                        return false;
                    }
                }
                else
                {
                    NSLog.Logger.Info("Employee_CreateOrEdit", result);
                    return false;
                }
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("Employee_CreateOrEdit_Fail", e);
                return false;
            }
        }

        public bool Delete(string ID, string CreateUser, ref string msg, string ReasonDelete = null)
        {
            try
            {
                EmployeeRequest paraBody = new EmployeeRequest();
                paraBody.CreatedUser = CreateUser;
                paraBody.ID = ID;
                paraBody.ReasonDelete = ReasonDelete;
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.EmployeeAPIDelete, null, paraBody);
                if (result != null)
                {
                    if (result.Success)
                        return true;
                    else
                    {
                        msg = result.Message;
                        //msg = Commons.ErrorMsg;
                        NSLog.Logger.Info("Employee_Delete", result.Message);
                        return false;
                    }
                }
                else
                {
                    NSLog.Logger.Info("Employee_Delete", result);
                    return false;
                }
            }
            catch (Exception e)
            {
                msg = e.ToString();
                NSLog.Logger.Error("Employee_Delete_Fail", e);
                return false;
            }
        }
        public bool ChangePassword(EmployeeDetailModels model, ref string msg, string mec, string userid)
        {
            try
            {
                EmployeeChangePasswordRequest paraBody = new EmployeeChangePasswordRequest();
                paraBody.CreatedUser = userid;
                paraBody.ID = model.ID;
                paraBody.OldPassword = CommonHelper.GetSHA512(model.OldPassword);
                paraBody.NewPassword = CommonHelper.GetSHA512(model.NewPassword);
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.ChangePassword, null, paraBody);
                if (result != null)
                {
                    if (result.Success)
                        return true;
                    else
                    {
                        msg = result.Message;
                        //msg = Commons.ErrorMsg;
                        NSLog.Logger.Info("Employee_ChangePassword", result.Message);
                        return false;
                    }
                }
                else
                {
                    NSLog.Logger.Info("Employee_ChangePassword", result);
                    return false;
                }                
            }
            catch (Exception e)
            {
                msg = e.ToString();
                NSLog.Logger.Error("Employee_ChangePassword error: ", e);
                return false;
            }
        }

    }
}
