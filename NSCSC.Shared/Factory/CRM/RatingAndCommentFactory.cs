using Newtonsoft.Json;
using NSCSC.Shared.Models;
using NSCSC.Shared.Models.CRM;
using NSCSC.Shared.Models.CRM.RatingAndComment;
using NSCSC.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSCSC.Shared.Factory.CRM
{
    public class RatingAndCommentFactory
    {
        public RatingAndCommentFactory()
        {

        }
        // get Ratting and Comments
        public List<RatingAndCommentModels> GetListofProductFeatures(string userid)
        {
            List<RatingAndCommentModels> listData = new List<RatingAndCommentModels>();
            try
            {
                RatingAndCommentRequestModels paraBody = new RatingAndCommentRequestModels();
                paraBody.CreatedUser = userid;
                paraBody.SearchString = "";
                NSLog.Logger.Info("Ratting and Comments GetData Request", paraBody);
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.CRMAPIGetList, null, paraBody);
                NSLog.Logger.Info("Ratting and Comments GetData Response", result);
                dynamic data = result.Data;
                var lstC = data["ListRatingAndComment"];
                var lstContent = JsonConvert.SerializeObject(lstC);
                listData = JsonConvert.DeserializeObject<List<RatingAndCommentModels>>(lstContent);
                NSLog.Logger.Info("RatingAndCommentGetListData", listData);
                return listData;
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("RatingAndComment_Fail: ", e);
                return listData;
            }
        }
        //Delete
        public bool Delete(string ID, string UserId, ref string msg, string ReasonDelete = null)
        {
            try
            {
                RatingAndCommentRequestModels Para = new RatingAndCommentRequestModels();
                Para.ID = ID;
                Para.CreatedUser = UserId;
                Para.ReasonDelete = ReasonDelete;
                NSLog.Logger.Info("RatingAndComment Delete Request", Para);
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.CRMAPIDelete, null, Para);
                NSLog.Logger.Info("RatingAndComment Delete Response", result);
                if (result != null)
                {
                    if (result.Success)
                        return true;
                    else
                    {
                        msg = result.Message;
                        NSLog.Logger.Info("RatingAndCommentDelete", result.Message);
                        return false;
                    }
                }
                else
                {
                    NSLog.Logger.Info("RatingAndCommentDelete", result);
                    return false;
                }
            }
            catch (Exception e)
            {
                msg = e.ToString();
                NSLog.Logger.Error("RatingAndComment_Delete: ", e);
                return false;
            }
        }
        // Show or Hide Comments
        public bool ShowHideComment(string ID, string UserId, ref string msg)
        {
            try
            {
                RatingAndCommentRequestModels Para = new RatingAndCommentRequestModels();
                Para.ID = ID;
                Para.CreatedUser = UserId;
                NSLog.Logger.Info("RatingAndComment ShowHideComment Request", Para);
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.CRMAPIShowHide, null, Para);
                NSLog.Logger.Info("RatingAndComment ShowHideComment Request", result);
                if (result != null)
                {
                    if (result.Success)
                        return true;
                    else
                    {
                        msg = result.Message;
                        NSLog.Logger.Info("RatingAndCommentShow/Hide", result.Message);
                        return false;
                    }
                }
                else
                {
                    NSLog.Logger.Info("RatingAndCommentShow/Hide", result);
                    return false;
                }
            }
            catch (Exception e)
            {
                msg = e.ToString();
                NSLog.Logger.Error("RatingAndCommentShow/HIde: ", e);
                return false;
            }
        }
    }
}
