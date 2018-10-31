using NSCSC.Shared.Factory.ManagementTool;
using NSCSC.Shared.Models.ManagementTool.Feedbacks;
using NSCSC.Web.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace NSCSC.Web.Controllers
{   
    [NuAuth]
    public class MFeesbacksController : HQController
    {

        private FeedbacksFactory _factory = null;

        public MFeesbacksController()
        {
            _factory = new FeedbacksFactory();
        }

        // GET: SBInventoryDiscount
        public ActionResult Index()
        {
            try
            {
                FeedbacksViewModels model = new FeedbacksViewModels();
                return View(model);
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("FeedbacksIndex: ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }

        public ActionResult Search(FeedbacksViewModels model)
        {
            try
            {
                var listData = _factory.GetListData();
                model.ListItem = listData;
                model.ListItem.ForEach(x => x.CustomerEmail = MarkEmailView(x.CustomerEmail));
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("FeedbacksSearch: ", e);
                return new HttpStatusCodeResult(400, e.Message);
            }
            return PartialView("_ListData", model);
        }       
        public FeedbackDTO GetDetail(string id)
        {
            try
            {
                FeedbackDTO model = _factory.GetDetail(id);
                return model;
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("FeedbacksDetail: ", ex);
                return null;
            }
        }

        [HttpGet]
        public new PartialViewResult View(string id)
        {
            FeedbackDTO model = GetDetail(id);
            return PartialView("_View", model);
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
                NSLog.Logger.Error("FeedbackDelete: ", ex);
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }


        [HttpGet]
        public PartialViewResult Delete(string id)
        {
            FeedbackDTO model = GetDetail(id);
            return PartialView("_Delete", model);
        }

        [HttpPost]
        public ActionResult Delete(FeedbackDTO model)
        {
            try
            {
                string msg = "";
                var result = _factory.Delete(model.ID, CurrentUser.UserId, ref msg);
                if (!result)
                {
                    ModelState.AddModelError("Name", msg);
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return PartialView("_Delete", model);
                }
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("FeedbacksDelete: ", ex);
                ModelState.AddModelError("Name", "Have an error when you delete a Feedback");
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return PartialView("_Delete", model);
            }
        }

        //public ActionResult Export()
        //{
        //    FeedbackDTO model = new FeedbackDTO();
        //    return View(model);
        //}


        //[HttpPost]
        //public ActionResult Export(FeedbackDTO model)
        //{
        //    try
        //    {
        //        //if (model.ListStores == null)
        //        //{
        //        //    ModelState.AddModelError("ListStores", "Please choose store.");
        //        //    return View(model);
        //        //}
        //        XLWorkbook wb = new XLWorkbook();
        //        var ws = wb.Worksheets.Add("Sheet1");
        //        StatusResponse response = _factory.Export(ref ws, model.ListStores);
        //        if (!response.Status)
        //        {
        //            ModelState.AddModelError("", response.MsgError);
        //            return View(model);
        //        }
        //        ViewBag.wb = wb;
        //        Response.Clear();
        //        Response.ClearContent();
        //        Response.ClearHeaders();
        //        Response.Charset = System.Text.UTF8Encoding.UTF8.WebName;
        //        Response.ContentEncoding = System.Text.UTF8Encoding.UTF8;
        //        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //        Response.AddHeader("content-disposition", String.Format(@"attachment;filename={0}.xlsx", CommonHelper.GetExportFileName("Discount").Replace(" ", "_")));

        //        using (var memoryStream = new System.IO.MemoryStream())
        //        {
        //            wb.SaveAs(memoryStream);
        //            memoryStream.WriteTo(HttpContext.Response.OutputStream);
        //            memoryStream.Close();
        //        }
        //        HttpContext.Response.End();
        //        return RedirectToAction("Export");
        //    }
        //    catch (Exception e)
        //    {
        //        NSLog.Logger.Error("DiscountExport: ", e);
        //        ModelState.AddModelError("ListStores", "Import file have error.");
        //        return View(model);
        //    }
        //}
    }
}