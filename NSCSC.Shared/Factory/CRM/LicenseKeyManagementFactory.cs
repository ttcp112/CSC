using Newtonsoft.Json;
using NSCSC.Shared.Models;
using NSCSC.Shared.Models.CRM.LicenseKeyManagement;
using NSCSC.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSCSC.Shared.Factory.CRM
{
    public class LicenseKeyManagementFactory
    {
        public List<LicenseKeyManagementModels> GetListData()
        {
            List<LicenseKeyManagementModels> listData = new List<LicenseKeyManagementModels>();
            try
            {
                GetListLicenseRequest paraBody = new GetListLicenseRequest();

                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.CustomerLicenseGetList, null, paraBody);
                dynamic data = result.Data;
                var lstDataRaw = data["ListLicense"];
                var lstObject = JsonConvert.SerializeObject(lstDataRaw);
                listData = JsonConvert.DeserializeObject<List<LicenseKeyManagementModels>>(lstObject);
                if(listData != null && listData.Any())
                {
                    listData.ForEach(x =>
                    {
                        if (x.ExpiredTime.HasValue)
                        {
                            x.ExpiredTime = CommonHelper.ConvertToLocalTime(x.ExpiredTime.Value);
                        }
                    });
                }
                NSLog.Logger.Info("LicenseKeyManagementGetListData", listData);

                return listData;
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("LicenseKeyManagementGetListData_Fail", e);
                return listData;
            }
        }

        public LicenseDetailDTO GetDetail(string ID)
        {
            LicenseDetailDTO model = new LicenseDetailDTO();
            try
            {
                GetLicenseDetailRequest paraBody = new GetLicenseDetailRequest();
                paraBody.ID = ID;
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.CustomerLicenseGetDetail, null, paraBody);
                dynamic data = result.Data;
                var lstDataRaw = data["LicenseDetail"];
                var lstObject = JsonConvert.SerializeObject(lstDataRaw);
                model = JsonConvert.DeserializeObject<LicenseDetailDTO>(lstObject);
                if (model != null)
                {
                    model.DateCreated = CommonHelper.ConvertToLocalTime(model.DateCreated);
                    if (model.ExpiredTime.HasValue)
                    {
                        model.ExpiredTime = CommonHelper.ConvertToLocalTime(model.ExpiredTime.Value);
                    }
                    if (model.ListItem != null && model.ListItem.Any())
                    {
                        model.ListItem.ForEach(x =>
                        {
                            if (x.ActiveTime.HasValue)
                            {
                                x.ActiveTime = CommonHelper.ConvertToLocalTime(x.ActiveTime.Value);
                            }
                        });
                    }
                }
                NSLog.Logger.Info("LicenseKeyManagement_GetDetail", model);

                return model;
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("LicenseKeyManagement_GetDetail_Fail", e);
                return model;
            }
        }

        public bool CreateOrEdit(LicenseDetailDTO model, string useid, ref string msg)
        {
            try
            {
                UpdateLicenseKeyRequest paraBody = new UpdateLicenseKeyRequest();
                paraBody.ID = model.ID;
                paraBody.IsActive = model.IsActive;
                paraBody.ListItem = model.ListItem;
                paraBody.CreatedUser = useid;
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.CustomerLicenseUpdate, null, paraBody);
                if (result != null)
                {
                    if (result.Success)
                        return true;
                    else
                    {
                        msg = result.Message;
                        msg = Commons.ErrorMsg;
                        NSLog.Logger.Info("LicenseKeyManagement_CreateOrEdit", result.Message);
                        return false;
                    }
                }
                else
                {
                    NSLog.Logger.Info("LicenseKeyManagement_CreateOrEdit", result);
                    return false;
                }
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("LicenseKeyManagement_CreateOrEdit_Fail", e);
                return false;
            }
        }
        public bool ChangeStatusLicenseKey(string ID, string useid, ref string msg)
        {
            try
            {
                ChangeStatusLicenseKeyRequest paraBody = new ChangeStatusLicenseKeyRequest();
                paraBody.ID = ID;
                paraBody.CreatedUser = useid;
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.CustomerLicenseChangeStatus, null, paraBody);
                if (result != null)
                {
                    if (result.Success)
                        return true;
                    else
                    {
                        msg = result.Message;
                        msg = Commons.ErrorMsg;
                        NSLog.Logger.Info("LicenseKeyManagement_ChangeStatus", result.Message);
                        return false;
                    }
                }
                else
                {
                    NSLog.Logger.Info("LicenseKeyManagement_ChangeStatus", result);
                    return false;
                }
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("LicenseKeyManagement_ChangeStatus_Fail", e);
                return false;
            }
        }
    }
}
