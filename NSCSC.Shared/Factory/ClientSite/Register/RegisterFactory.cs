using Newtonsoft.Json;
using NSCSC.Shared.Models;
using NSCSC.Shared.Models.ClientSite;
using NSCSC.Shared.Models.ClientSite.Account.ResendMail;
using NSCSC.Shared.Models.ClientSite.Account.Verify;
using NSCSC.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSCSC.Shared.Factory.ClientSite.Register
{
    public class RegisterFactory
    {
        public bool Create(RegisterModels model, ref string msg)
        {
            try
            {
                AccountRegisterRequest paraBody = new AccountRegisterRequest();

                // Hash password
                model.CustomerDetail.Password = CommonHelper.GetSHA512(model.CustomerDetail.Password);

                paraBody.CustomerDetail = model.CustomerDetail;
                paraBody.Merchant = model.MerchantDetail;

                NSLog.Logger.Info("ClientSideToRegister Request: ", paraBody);
                //====================
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.ClientSideToRegister, null, paraBody);
                if (result != null)
                {
                    if (result.Success)
                        return true;
                    else
                    {
                        msg = result.Message;
                        return false;
                    }
                }
                else
                {
                    //NSLog.Logger.Info("AccountRegisterCreate", result);
                    return false;
                }
                NSLog.Logger.Info("ClientSideToRegister Result", result);
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("ClientSideToRegister_Fail", e);
                return false;
            }
        }        

        public bool ResendMail(string Email, ref string msg)
        {
            try
            {
                ResendEmailRequest paraBody = new ResendEmailRequest();
                paraBody.Email = Email;

                NSLog.Logger.Info("ResendMail Request: ", paraBody);
                //====================
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.ClientSideAccountToResendEmail, null, paraBody);
                if (result != null)
                {
                    if (result.Success)
                        return true;
                    else
                    {
                        msg = result.Message;
                        //NSLog.Logger.Info("AccountRegisterCreate", result.Message);
                        return false;
                    }
                }
                else
                {
                    return false;
                }
                NSLog.Logger.Info("ResendMail Result", result);
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("ResendMail_Fail", e);
                return false;
            }
        }

        public ResponseAccountLogin Verify(VerificationModels model, ref string msg)
        {
            try
            {
                AccountVerifyRequest paraBody = new AccountVerifyRequest();
                paraBody.Email = model.ReSendEmail;
                paraBody.VerifyCode = model.VerificationCode;

                NSLog.Logger.Info("Verify Request: ", paraBody);
                //====================
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.ClientSideAccountToVerify, null, paraBody);
                if (result != null)
                {
                    if (result.Success)
                    {
                        dynamic data = result.Data;
                        var lstDataRaw = data["Info"];
                        var lstContent = JsonConvert.SerializeObject(lstDataRaw);
                        var listData = JsonConvert.DeserializeObject<ResponseAccountLogin>(lstContent);
                        return listData;
                    }
                    else
                    {
                        msg = result.Message;
                        //NSLog.Logger.Info("AccountRegisterCreate", result.Message);
                        return null;
                    }
                }
                else
                {
                    return null;
                }
                NSLog.Logger.Info("Verify Result", result);
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("Verify_Fail", e);
                return null;
            }
        }

        public bool CheckEmail(string email, ref string msg)
        {
            try
            {
                AccountRegisterCheckEmailRequest paraBody = new AccountRegisterCheckEmailRequest();
                paraBody.Email = email;

                NSLog.Logger.Info("CheckEmail Request: ", paraBody);
                //====================
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.ClientSideAccountCheckEmail, null, paraBody);
                if (result != null)
                {
                    if (result.Success)
                        return true;
                    else
                    {
                        msg = result.Message;
                        //NSLog.Logger.Info("AccountRegisterCreate", result.Message);
                        return false;
                    }
                }
                else
                {
                    return false;
                }
                NSLog.Logger.Info("CheckEmail Result", result);
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("CheckEmail_Fail", e);
                return false;
            }
        }
    }
}
