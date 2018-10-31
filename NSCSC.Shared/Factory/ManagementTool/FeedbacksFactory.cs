using Newtonsoft.Json;
using NSCSC.Shared.Models;
using NSCSC.Shared.Models.ManagementTool.Feedbacks;
using NSCSC.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSCSC.Shared.Factory.ManagementTool
{
    public class FeedbacksFactory : BaseFactory
    {
        private BaseFactory _baseFactory = null;
        public FeedbacksFactory()
        {
            _baseFactory = new BaseFactory();
        }

        public bool CreateOrEdit(FeedbackDTO model, ref string msg)
        {
            try
            {
                CreateOrEditFeedbackRequest paraBody = new CreateOrEditFeedbackRequest();
                paraBody.Feedback = model;
                paraBody.CreatedUser = model.CreatedUser;
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.FeedbackAPICreateOrEdit, null, paraBody);
                if (result != null)
                {
                    if (result.Success)
                        return true;
                    else
                    {
                        msg = result.Message;
                        //msg = Commons.ErrorMsg;
                        NSLog.Logger.Info("Feedback_CreateOrEdit", result.Message);
                        return false;
                    }
                }
                else
                {
                    NSLog.Logger.Info("Feedback_CreateOrEdit", result);
                    return false;
                }
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("Employee_CreateOrEdit_Fail", e);
                return false;
            }
        }

        public List<FeedbacksModels> GetListData()
        {
            List<FeedbacksModels> listData = new List<FeedbacksModels>();
            try
            {
                GetFeedbackRequest paraBody = new GetFeedbackRequest();
                NSLog.Logger.Info("Feedbacks Get List Data Request", paraBody);
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.FeedbackAPIGetList, null, paraBody);
                NSLog.Logger.Info("Feedbacks Get List Data Response", paraBody);
                dynamic data = result.Data;
                var lstDataRaw = data["ListFeedback"];
                var lstObject = JsonConvert.SerializeObject(lstDataRaw);
                listData = JsonConvert.DeserializeObject<List<FeedbacksModels>>(lstObject);
                if (listData != null)
                {
                    listData.ForEach(xy =>
                    {
                        xy.Time = CommonHelper.ConvertToLocalTime(xy.Time);
                    });
                }
                NSLog.Logger.Info("FeedbackGetListData", listData);

                return listData;
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("FeedbackGetListData_Fail", e);
                return listData;
            }
        }

        public FeedbackDTO GetDetail(string ID)
        {
            FeedbackDTO item = new FeedbackDTO();
            try
            {
                GetFeedbackRequest paraBody = new GetFeedbackRequest();
                paraBody.ID = ID;
                paraBody.PageIndex = 1;
                paraBody.PageSize = 5000;
                NSLog.Logger.Info("Feedbacks Get Detail Request", paraBody);
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.FeedbackAPIGetList, null, paraBody);
                NSLog.Logger.Info("Feedbacks Get Detail Response", paraBody);
                dynamic data = result.Data;
                var lstDataRaw = data["ListFeedback"];
                var lstObject = JsonConvert.SerializeObject(lstDataRaw);
                item = JsonConvert.DeserializeObject<List<FeedbackDTO>>(lstObject)[0];

                NSLog.Logger.Info("FeedbackGetDetail", item);

                return item;
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("FeedbackGetDetail_Fail", e);
                return item;
            }
        }        

        public bool Delete(string ID, string CreateUser, ref string msg, string ReasonDelete = null)
        {
            try
            {
                DeleteFeedbackRequest paraBody = new DeleteFeedbackRequest();
                paraBody.CreatedUser = CreateUser;
                paraBody.ReasonDelete = ReasonDelete;
                paraBody.ID = ID;
                //====================
                NSLog.Logger.Info("Feedbacks Delete Request", paraBody);
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.FeedbackAPIDelete, null, paraBody);
                NSLog.Logger.Info("Feedbacks Delete Response", paraBody);
                if (result != null)
                {
                    if (result.Success)
                        return true;
                    else
                    {
                        msg = result.Message;
                        NSLog.Logger.Info("FeedbackDelete", result.Message);
                        return false;
                    }
                }
                else
                {
                    NSLog.Logger.Info("FeedbackDelete", result);
                    return false;
                }
            }
            catch (Exception e)
            {
                msg = e.ToString();
                NSLog.Logger.Error("FeedbackDelete_Fail", e);
                return false;
            }
        }
    }
}
