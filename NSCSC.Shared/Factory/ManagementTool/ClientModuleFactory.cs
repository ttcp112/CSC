using Newtonsoft.Json;
using NSCSC.Shared.Models;
using NSCSC.Shared.Models.ManagementTool.ClientModule;
using NSCSC.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSCSC.Shared.Factory.ManagementTool
{
    public class ClientModuleFactory
    {
        public List<ClientModuleModel> GetClientModule()
        {
            var result = new List<ClientModuleModel>();
            try
            {
                RequestBaseModels paraRequestBaseModels = new RequestBaseModels();
                
                var apiResult = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.ClientModuleSetting, null, paraRequestBaseModels);
                dynamic data = apiResult.Data;

                NSLog.Logger.Info("Response GetClientModule", data);
                var lstObject = JsonConvert.SerializeObject(data["ListModule"]);
                result = JsonConvert.DeserializeObject<List<ClientModuleModel>>(lstObject);
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("Error GetClientModule", ex);
            }
            return result;
        }
    }
}
