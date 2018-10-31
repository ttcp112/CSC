using Newtonsoft.Json;
using NSCSC.Shared.Models;
using NSCSC.Shared.Models.ManagementTool.FAQ;
using NSCSC.Shared.Models.ManagementTool.Topics;
using NSCSC.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSCSC.Shared.Factory.ManagementTool
{
    public class FAQFactory : BaseFactory
    {
        public List<FAQModels> GetListFAQData(string SearchString = null)
        {
            List<FAQModels> listData = new List<FAQModels>();
            try
            {
                GetFAQRequest paraBody = new GetFAQRequest();

                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.FAQAPIGetList, null, paraBody);
                dynamic data = result.Data;
                var lstDataRaw = data["ListFAQ"];
                var lstObject = JsonConvert.SerializeObject(lstDataRaw);
                listData = JsonConvert.DeserializeObject<List<FAQModels>>(lstObject);

                NSLog.Logger.Info("FAQGetListData", listData);

                return listData;
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("FAQGetListData_Fail", e);
                return listData;
            }
        }

        public List<TopicModel> GetListTopicData(string SearchString = null)
        {
            List<TopicModel> listData = new List<TopicModel>();
            try
            {
                GetTopicRequest paraBody = new GetTopicRequest();

                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.TopicAPIGetList, null, paraBody);
                dynamic data = result.Data;
                var lstDataRaw = data["ListTopic"];
                var lstObject = JsonConvert.SerializeObject(lstDataRaw);
                listData = JsonConvert.DeserializeObject<List<TopicModel>>(lstObject);

                NSLog.Logger.Info("TopicGetListData", listData);

                return listData;
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("TopicGetListData_Fail", e);
                return listData;
            }
        }

        public TopicDTO GetTopicDetail(string ID)
        {
            TopicDTO item = new TopicDTO();
            try
            {
                GetTopicRequest paraBody = new GetTopicRequest();
                paraBody.ID = ID;

                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.TopicAPIGetList, null, paraBody);
                dynamic data = result.Data;
                var lstDataRaw = data["ListTopic"];
                var lstObject = JsonConvert.SerializeObject(lstDataRaw);
                item = JsonConvert.DeserializeObject<List<TopicDTO>>(lstObject)[0];

                NSLog.Logger.Info("TopicGetDetail", item);

                return item;
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("TopicGetDetail_Fail", e);
                return item;
            }
        }

        public FAQDTO GetFAQDetail(string ID)
        {
            FAQDTO item = new FAQDTO();
            try
            {
                GetFAQRequest paraBody = new GetFAQRequest();
                paraBody.ID = ID;

                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.FAQAPIGetList, null, paraBody);
                dynamic data = result.Data;
                var lstDataRaw = data["ListFAQ"];
                var lstObject = JsonConvert.SerializeObject(lstDataRaw);
                item = JsonConvert.DeserializeObject<List<FAQDTO>>(lstObject)[0];

                NSLog.Logger.Info("FAQGetDetail", item);

                return item;
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("FAQGetDetail_Fail", e);
                return item;
            }
        }

        public bool CreateOrEditTopic(TopicDTO model, string user, ref string msg)
        {
            try
            {
                CreateOrEditTopicRequest paraBody = new CreateOrEditTopicRequest();
                paraBody.TopicDetail = model;
                paraBody.CreatedUser = user;

                //====================
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.TopicAPICreateOrEdit, null, paraBody);
                if (result != null)
                {
                    if (result.Success)
                        return true;
                    else
                    {
                        msg = result.Message;
                        NSLog.Logger.Info("TopicCreateOrEdit", result.Message);
                        return false;
                    }
                }
                else
                {
                    NSLog.Logger.Info("TopicCreateOrEdit", result);
                    return false;
                }
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("TopicCreateOrEdit_Fail", e);
                return false;
            }
        }

        public bool CreateOrEditFAQ(FAQDTO model, string user, ref string msg)
        {
            try
            {
                CreateOrEditFAQRequest paraBody = new CreateOrEditFAQRequest();
                paraBody.FAQ = model;
                paraBody.CreatedUser = user;

                //====================
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.FAQAPICreateOrEdit, null, paraBody);
                if (result != null)
                {
                    if (result.Success)
                        return true;
                    else
                    {
                        msg = result.Message;
                        NSLog.Logger.Info("FAQCreateOrEdit", result.Message);
                        return false;
                    }
                }
                else
                {
                    NSLog.Logger.Info("FAQCreateOrEdit", result);
                    return false;
                }
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("FAQCreateOrEdit_Fail", e);
                return false;
            }
        }

        public bool DeleteTopic(string ID, string CreateUser, ref string msg)
        {
            try
            {
                DeleteTopicRequest paraBody = new DeleteTopicRequest();
                paraBody.CreatedUser = CreateUser;
                paraBody.ID = ID;
                //====================
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.TopicAPIDelete, null, paraBody);
                if (result != null)
                {
                    if (result.Success)
                        return true;
                    else
                    {
                        msg = result.Message;
                        NSLog.Logger.Info("TopicDelete", result.Message);
                        return false;
                    }
                }
                else
                {
                    NSLog.Logger.Info("TopicDelete", result);
                    return false;
                }
            }
            catch (Exception e)
            {
                msg = e.ToString();
                NSLog.Logger.Error("TopicDelete_Fail", e);
                return false;
            }
        }

        public bool DeleteFAQ(string ID, string CreateUser, ref string msg)
        {
            try
            {
                DeleteFAQRequest paraBody = new DeleteFAQRequest();
                paraBody.CreatedUser = CreateUser;
                paraBody.ID = ID;
                //====================
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.FAQAPIDelete, null, paraBody);
                if (result != null)
                {
                    if (result.Success)
                        return true;
                    else
                    {
                        msg = result.Message;
                        NSLog.Logger.Info("FAQDelete", result.Message);
                        return false;
                    }
                }
                else
                {
                    NSLog.Logger.Info("FAQDelete", result);
                    return false;
                }
            }
            catch (Exception e)
            {
                msg = e.ToString();
                NSLog.Logger.Error("FAQDelete_Fail", e);
                return false;
            }
        }
    }
}
