using Newtonsoft.Json;
using NSCSC.Shared.Models;
using NSCSC.Shared.Models.Settings.PrefixSettings;
using NSCSC.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSCSC.Shared.Factory.Settings
{
    public class PrefixSettingsFactory
    {
        public PrefixSettingsFactory()
        {

        }
        // Get List Prefix Settings
        public List<PrefixSettingsModels> GetListPrefixSettings(string userid)
        {
            List<PrefixSettingsModels> listData = new List<PrefixSettingsModels>();
            try
            {
                GetSettingRequest paraBody = new GetSettingRequest();
                paraBody.CreatedUser = userid;
                //paraBody.SearchString = "";
                NSLog.Logger.Info("GetList Prefix Settings Request", paraBody);
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.PrefixSettingsAPIGetList, null, paraBody);
                NSLog.Logger.Info("GetList Prefix Settings Request", result);
                dynamic data = result.Data;
                var lstC = data["ListSetting"];
                var lstContent = JsonConvert.SerializeObject(lstC);
                listData = JsonConvert.DeserializeObject<List<PrefixSettingsModels>>(lstContent);
                NSLog.Logger.Info("PrefixSettingsGetList", listData);
                return listData;
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("PrefixSettings_Fail: ", e);
                return listData;
            }
        }
        //
        // Edit Prefix Sttings
        public bool UpdatePrefixSettings(List<PrefixSettingsModels> model, string userid, ref string msg)
        {
            try
            {
                GetSettingRequest paraBody = new GetSettingRequest();
                paraBody.CreatedUser = userid;               
                paraBody.ListSetting = model;
                NSLog.Logger.Info("GetList Prefix Settings Request", paraBody);
                //====================

                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.PrefixSettingsAPIEdit, null, paraBody);
                NSLog.Logger.Info("GetList Prefix Settings Request", result);
                if (result != null)
                {
                    if (result.Success)
                        return true;
                    else
                    {
                        msg = result.Message;
                        NSLog.Logger.Info("PrefixSettingsEdit", result.Message);
                        return false;
                    }
                }
                else
                {
                    NSLog.Logger.Info("PrefixSettingsEdit", result);
                    return false;
                }
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("PrefixSettingsUpdate_Fail", e);
                return false;
            }
        }
    }
}
