using Newtonsoft.Json;
using NSCSC.Shared.Models;
using NSCSC.Shared.Models.CRM.Support;
using NSCSC.Shared.Utilities;
using System;
using System.Collections.Generic;

namespace NSCSC.Shared.Factory.CRM
{
    public class SupportFactory : BaseFactory
    {
        private BaseFactory _baseFactory = null;
        public SupportFactory()
        {
            _baseFactory = new BaseFactory();
        }

        public List<MessageModels> GetListMessage()
        {
            List<MessageModels> listData = new List<MessageModels>();
            try
            {
                //SupportRequest paraBody = new SupportRequest();
                //NSLog.Logger.Info("GetListMessage Request: ", paraBody);

                //var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.DiscountAPIGetList, null, paraBody);
                //dynamic data = result.Data;
                //var lstDataRaw = data["ListMessage"];
                //var lstObject = JsonConvert.SerializeObject(lstDataRaw);
                //listData = JsonConvert.DeserializeObject<List<MessageModels>>(lstObject);

                //NSLog.Logger.Info("Support_GetListMessage", listData);

                listData.Add(new MessageModels
                {
                    CreatedTime = DateTime.Now,
                    Account = "Elysa",
                    Email = "elysa@gmail.com",
                    ID = "112233445566",
                    Subject = "xyz",
                    Status = "Replied",
                    Location = "Tokyo - Japan"
                });
                listData.Add(new MessageModels
                {
                    CreatedTime = DateTime.Now,
                    Account = "Anonymous",
                    Email = "anonymous@gmail.com",
                    ID = "112233445577",
                    Subject = "abc",
                    Status = "New",
                    Location = "NYDC"
                });

                return listData;
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("Support_GetListMessage_Fail", e);
                return listData;
            }
        }
    }
}
