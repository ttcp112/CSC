using Newtonsoft.Json;
using NSCSC.Shared.Models;
using NSCSC.Shared.Models.SandBox.Inventory.Product;
using NSCSC.Shared.Utilities;
using System;
using System.Collections.Generic;

namespace NSCSC.Shared.Factory
{
    public class UserFactory
    {
        public LoginResponseModel Login(string email, string password)
        {

            LoginResponseModel user = null;
            try
            {
                //string sysFormat = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;

                password = CommonHelper.GetSHA512(password);
                LoginRequestModel paraBody = new LoginRequestModel();
                paraBody.Email = email;
                paraBody.Password = password;
                NSLog.Logger.Info("Promotion Login Request", paraBody);
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.Login, null, paraBody);
                NSLog.Logger.Info("Promotion Login Response", result);
                if (result.Success)
                {
                    user = new LoginResponseModel();

                    dynamic data = result.Data;
                    dynamic dataInfo = data["Info"];
                    //user.EmployeeID = dataInfo["EmployeeID"];
                    //user.EmployeeName = dataInfo["EmployeeName"];
                    //user.EmployeeEmail = dataInfo["EmployeeEmail"];
                    //user.EmployeeImageURL = dataInfo["EmployeeImageURL"];

                    ////user.MerchantID = dataInfo["MerchantID"];
                    ////user.MerchantName = dataInfo["MerchantName"];
                    ////user.CurrencySymbol = dataInfo["CurrencySymbol"];
                    ////user.IsIntergrate = dataInfo["IsIntergrate"];

                    //if (string.IsNullOrEmpty(user.EmployeeImageURL))
                    //    user.EmployeeImageURL = null;
                    ////foreach (var item in dataInfo["ListStore"])
                    ////{
                    ////user.ListStore.Add(new StoreWebDTO() { ID = item["ID"], Name = item["Name"], IndustryCode = item["IndustryCode"] });
                    ////}

                    //foreach (var item in dataInfo["ListPermission"])
                    //{
                    //    user.ListPermission.Add(
                    //        new PermissionDTO() {
                    //            ID = item["ID"],
                    //            ParentID = item["ParentID"],
                    //            Name = item["Name"],
                    //            Code = item["Code"],
                    //            IsView = item["IsView"],
                    //            IsAction = item["IsAction"]
                    //        });
                    //}

                    var lstDataInfo = JsonConvert.SerializeObject(dataInfo);
                    user = JsonConvert.DeserializeObject<LoginResponseModel>(lstDataInfo);

                }

                return user;
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("Index : ", e);
                return user;
            }
        }
        public ResponseAccountLogin LoginClient(string email, string password, bool isFromExtend = false)
        {

            ResponseAccountLogin user = null;
            try
            {              
                if(!isFromExtend)
                    password = CommonHelper.GetSHA512(password);
                AccountLoginRequest paraBody = new AccountLoginRequest();
                paraBody.Email = email;
                paraBody.Password = password;
                NSLog.Logger.Info("Promotion Login Request", paraBody);
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.LoginClient, null, paraBody);
                NSLog.Logger.Info("Promotion Login Response", result);
                if (result.Success)
                {
                    user = new ResponseAccountLogin();

                    dynamic data = result.Data;
                    dynamic dataInfo = data["Info"];
                    var lstDataInfo = JsonConvert.SerializeObject(dataInfo);
                    user = JsonConvert.DeserializeObject<ResponseAccountLogin>(lstDataInfo);
                    var dataProduct = data["ListProduct"];
                    if(dataProduct != null)
                    {
                        var lstDataProduct = JsonConvert.SerializeObject(dataProduct);
                        user.ListProduct = JsonConvert.DeserializeObject<List<ProductDetailModels>>(lstDataProduct);
                    }
                    if(!string.IsNullOrEmpty(data["OrderID"]))
                    {
                        user.OrderID = data["OrderID"];
                    }
                    if (!string.IsNullOrEmpty(data["CurrencySymbol"]))
                    {
                        user.CurrencySymbol = data["CurrencySymbol"];
                    }
                    user.NumOfItems = data["NumOfItems"];

                }
                else
                {               
                    dynamic data = result.Data;
                    if (data.Count >3)
                    {
                        user = new ResponseAccountLogin();
                        dynamic dataInfo = data["Info"];
                        user.IsVerify = dataInfo["IsVerify"];
                    }
                }

                return user;
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("Index : ", e);
                return user;
            }
        }
        public bool ForgotPassword(string email, ref string msg)
        {
            try
            {
                AccountForgotPasswordRequest paraBody = new AccountForgotPasswordRequest();
                paraBody.Email = email;

                NSLog.Logger.Info("ForgotPassword Request", paraBody);

                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.ForgotClient, null, paraBody);
                if (result != null)
                {
                    if (result.Success)
                        return true;
                    else
                    {
                        msg = result.Message;
                        NSLog.Logger.Info("ForgotPassword", result.Message);
                        return false;
                    }
                }
                else
                {
                    NSLog.Logger.Info("ForgotPassword", result);
                    return false;
                }
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("Index : ", e);
                return false;
            }
        }
        
    }
}
