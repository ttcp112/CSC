using Newtonsoft.Json;
using NSCSC.Shared.Models;
using NSCSC.Shared.Models.ClientSite;
using NSCSC.Shared.Models.ClientSite.MyProfile;
using NSCSC.Shared.Utilities;
using System;
using System.Collections.Generic;

namespace NSCSC.Shared.Factory.ClientSite
{
    public class MyProfileFactory : BaseFactory
    {
        public MyProfileFactory()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public CustomerDetailModels GetInfo(string id)
        {
            CustomerDetailModels model = new CustomerDetailModels();
            Customermodel customerdetail = new Customermodel();
            List<PurchaseReceiptModels> listoder = new List<PurchaseReceiptModels>();
            MerchantDTO merchentdetail = new MerchantDTO();
            try
            {
                GetInfoRequestModels paraBody = new GetInfoRequestModels();
                paraBody.ID = id;
                NSLog.Logger.Info("GetInfoPersonalDetail_Request: ", paraBody);

                // Reseller => merchantDetail = null | listReceipt > 0
                // Customer => listReceipt == 1

                //====================
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.ClientSideMyProfileGetInfo, null, paraBody);
                dynamic data = result.Data;
                //====customer detail
                var customer = data["CustomerDetail"];
                var lstObject = JsonConvert.SerializeObject(customer);
                customerdetail = JsonConvert.DeserializeObject<Customermodel>(lstObject);
                NSLog.Logger.Info("GetInfoCustomerDetail_Response: ", customerdetail);
                //======merchant detail
                var merchant = data["MerchantDetail"];
                var merchants = JsonConvert.SerializeObject(merchant);
                merchentdetail = JsonConvert.DeserializeObject<MerchantDTO>(merchants);
                NSLog.Logger.Info("GetInfoMerchantDetail_Response: ", merchentdetail);
                //====list order
                var order = data["ListReceipt"];
                var lstorders = JsonConvert.SerializeObject(order);
                listoder = JsonConvert.DeserializeObject<List<PurchaseReceiptModels>>(lstorders);
                NSLog.Logger.Info("GetInfoListOder_Response: ", listoder);
                //--------------------
                model.CustomerDetail = customerdetail;
                model.MerchantDetail = merchentdetail;
                model.ListReceipt = listoder;
                NSLog.Logger.Info("GetInfoPersonalDetail_Response: ", model);
                return model;
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("GetInfoPersonalDetail_Fail: ", e);
                return model;
            }
        }

        public OrderDetailModels GetDetailReceipt(string id, ref string msg)
        {
            OrderDetailModels model = new OrderDetailModels();
            try
            {
                GetOrderDetailRequest paraBody = new GetOrderDetailRequest();
                paraBody.ID = id;
                NSLog.Logger.Info("GetReceiptDetail_Request: ", paraBody);
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.ClientSiteYourCartGetOrderDetail, null, paraBody);
                NSLog.Logger.Info("GetReceiptDetail_Response: ", result);
                if (result != null)
                {
                    dynamic data = result.Data;
                    //====Receipt detail
                    var receipt = data["OrderDetail"];
                    var lstObject = JsonConvert.SerializeObject(receipt);
                    model = JsonConvert.DeserializeObject<OrderDetailModels>(lstObject);
                    NSLog.Logger.Info("GetReceiptDetail_Response: ", model);
                    return model;
                }
                else
                {
                    msg = result.Message;
                    NSLog.Logger.Info("GetReceiptDetail_Response: ", result.Message);
                    return model;
                }
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("GetReceiptDetail_Fail: ", e);
                return model;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="userid"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool UpdatePersonalInfo(Customermodel model, string userid, ref string msg)
        {
            try
            {
                AccountEditInfoRequest paraBody = new AccountEditInfoRequest();
                paraBody.CustomerDetail = model;
                NSLog.Logger.Info("PersonalInfoUpdate_Request: ", paraBody);
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.ClientSideMyProfileEdit, null, paraBody);
                NSLog.Logger.Info("PersonalInfoUpdate_Response: ", result);
                if (result != null)
                {
                    if (result.Success)
                        return true;
                    else
                    {
                        msg = result.Message;
                        NSLog.Logger.Info("PersonalInfoUpdate_Response: ", result.Message);
                        return false;
                    }
                }
                else
                {
                    NSLog.Logger.Info("PersonalInfoUpdate_Response: ", result);
                    return false;
                }
            }
            catch (Exception e)
            {
                msg = e.Message;
                NSLog.Logger.Error("PersonalInfoUpdate_Fail: ", e);
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="userId"></param>
        /// <param name="msg"></param>
        /// <param name="CurrentPassword"></param>
        /// <param name="NewPassword"></param>
        /// <returns></returns>
        public bool ChangePassword(string ID, string userId, ref string msg, string CurrentPassword, string NewPassword)
        {
            try
            {
                AccountChangePasswordRequest Para = new AccountChangePasswordRequest();
                Para.ID = ID;
                Para.CreatedUser = userId;
                Para.OldPassword = CommonHelper.GetSHA512(CurrentPassword);
                Para.NewPassword = CommonHelper.GetSHA512(NewPassword);
                NSLog.Logger.Info("ChangePassword_Request: ", Para);
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.ClientSideMyProfileChangePassword, null, Para);
                NSLog.Logger.Info("ChangePassword_Response: ", result);
                if (result != null)
                {
                    if (result.Success)
                        return true;
                    else
                    {
                        msg = result.Message;
                        NSLog.Logger.Info("ChangePassword_Response: ", result.Message);
                        return false;
                    }
                }
                else
                {
                    NSLog.Logger.Info("ChangePassword_Response: ", result);
                    return false;
                }
            }
            catch (Exception e)
            {
                msg = e.Message.ToString();
                NSLog.Logger.Error("ChangePassword_Fail: ", e);
                return false;
            }
        }

        /*Trongntn 10-03-2017*/
        /// <summary>
        /// 
        /// </summary>
        /// <param name="CustomerID"></param>
        /// <returns></returns>
        public CustomerDetailModels GetInfoLite(string CustomerID)
        {
            CustomerDetailModels model = new CustomerDetailModels();
            Customermodel customerdetail = new Customermodel();
            MerchantDTO merchentdetail = new MerchantDTO();
            try
            {
                GetInfoRequestModels paraBody = new GetInfoRequestModels();
                paraBody.ID = CustomerID;
                NSLog.Logger.Info("GetInfoLite_Request: ", paraBody);
                //====================
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.ClientSideMyProfileGetInfo, null, paraBody);
                dynamic data = result.Data;
                //====customer detail
                var customer = data["CustomerDetail"];
                var lstObject = JsonConvert.SerializeObject(customer);
                customerdetail = JsonConvert.DeserializeObject<Customermodel>(lstObject);
                NSLog.Logger.Info("GetInfoLite_Response: ", customerdetail);
                //======merchant detail
                var merchant = data["MerchantDetail"];
                var merchants = JsonConvert.SerializeObject(merchant);
                merchentdetail = JsonConvert.DeserializeObject<MerchantDTO>(merchants);
                NSLog.Logger.Info("GetInfoLite_Response: ", merchentdetail);
                //--------------
                model.CustomerDetail = customerdetail;
                model.MerchantDetail = merchentdetail;
                return model;
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("GetInfoLite_Fail: ", e);
                return model;
            }
        }
    }
}
