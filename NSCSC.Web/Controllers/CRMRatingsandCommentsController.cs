using NSCSC.Shared;
using NSCSC.Shared.Factory.CRM;
using NSCSC.Shared.Models.CRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace NSCSC.Web.Controllers
{
    public class CRMRatingsandCommentsController : HQController
    {
        public RatingAndCommentFactory _factory = null;
        public CRMRatingsandCommentsController()
        {
            _factory = new RatingAndCommentFactory();
        }
        // GET: CRMRatingsandComments
        public ActionResult Index()
        {
            try
            {
                RatingAndCommentViewModels model = new RatingAndCommentViewModels();
                return View(model);

            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("RatingAndComment_Index:", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }

        public ActionResult Search(RatingAndCommentViewModels model)
        {
            try
            {
                var data = _factory.GetListofProductFeatures(CurrentUser.UserId);

                model.ListRatingAndComment = data;
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("RatingAndComment_Search: ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
            return PartialView("_ListData", model);
        }

        [HttpGet]
        public PartialViewResult Delete(string id)
        {
            RatingAndCommentModels model = new RatingAndCommentModels();
            model.ID = id;
            return PartialView("_Delete",model);
        }

        [HttpPost]
        public ActionResult Delete(RatingAndCommentModels model)
        {
            try
            {
                string msg = "";
                var result = _factory.Delete(model.ID, CurrentUser.UserId, ref msg);
                if (!result)
                {
                    ModelState.AddModelError("Name", /*msg*/ Commons.ErrorMsg);
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return PartialView("_Delete", model);
                }
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("RatingAndComment_Delete: ", ex);
                ModelState.AddModelError("Name", Commons.ErrorMsg);
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return PartialView("_Delete", model);
            }
        }
     
        public ActionResult ShowHideComment(string id)
        {
            try
            {
                string msg = "";
                var result = _factory.ShowHideComment(id, CurrentUser.UserId, ref msg);
                if (!result)
                {
                    ModelState.AddModelError("Name", Commons.ErrorMsg);
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return PartialView("_ListData");
                }
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("RatingAndComment_Delete: ", ex);
                ModelState.AddModelError("Name", Commons.ErrorMsg);
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return PartialView("_ListData");
            }
        }

        public ActionResult DeletePopup(string ID, string PinCode, string Reason)
        {
            try
            {
                if (!PinCode.Equals(CurrentUser.PinCode))
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                string msg = "";
                var result = _factory.Delete(ID, CurrentUser.UserId, ref msg, Reason);
                if (!result)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("Rating And Comment Delete: ", ex);
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

    }
}