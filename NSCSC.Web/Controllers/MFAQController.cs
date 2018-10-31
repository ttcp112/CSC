using NSCSC.Shared.Factory.ManagementTool;
using NSCSC.Shared.Models.ManagementTool.FAQ;
using NSCSC.Shared.Models.ManagementTool.Topics;
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
    public class MFAQController : HQController
    {

        private FAQFactory _factory = null;

        public MFAQController()
        {
            _factory = new FAQFactory();
        }

        // GET: SBInventoryDiscount
        public ActionResult Index()
        {
            try
            {
                FAQViewModels model = new FAQViewModels();
                return View(model);
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("FAQIndex: ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }

        public ActionResult Search(FAQViewModels model)
        {
            try
            {
                var listFAQData = _factory.GetListFAQData();
                var listTopicData = _factory.GetListTopicData();
                model.ListIFAQtem = listFAQData;
                model.ListTopicItem = listTopicData;
                if (model.ListTopicItem != null && model.ListTopicItem.Count > 0)
                {
                    for (int i = 0; i < model.ListTopicItem.Count; i++)
                    {
                        SelectListItem newSelectListItem = new SelectListItem()
                        {
                            Value = model.ListTopicItem[i].ID,
                            Text = model.ListTopicItem[i].Name,
                            Selected = model.ListTopicItem[i].IsActive
                        };
                        model.ListTopic.Add(newSelectListItem);
                    }
                    model.ListTopic = model.ListTopic.Where(o => o.Selected == true).ToList();
                }
               
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("FAQSearch: ", e);
                return new HttpStatusCodeResult(400, e.Message);
            }
            return PartialView("_ListData", model);
        }
        public TopicDTO GetTopicDetail(string id)
        {
            try
            {
                TopicDTO model = _factory.GetTopicDetail(id);
                return model;
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("TopicDetail: ", ex);
                return null;
            }
        }
        public FAQDTO GetFAQDetail(string id)
        {
            try
            {
                FAQDTO model = _factory.GetFAQDetail(id);
                return model;
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("FAQDetail: ", ex);
                return null;
            }
        }


        [HttpGet]
        public new PartialViewResult ViewTopic(string id)
        {
            TopicDTO model = GetTopicDetail(id);
            return PartialView("_ViewTopic", model);
        }

        [HttpGet]
        public new PartialViewResult ViewFAQ(string id)
        {
            FAQDTO model = GetFAQDetail(id);
            return PartialView("_ViewFAQ", model);
        }

        [HttpGet]
        public PartialViewResult DeleteTopic(string id)
        {
            TopicDTO model = GetTopicDetail(id);
            return PartialView("_DeleteTopic", model);
        }

        [HttpPost]
        public ActionResult DeleteTopic(TopicDTO model)
        {
            try
            {
                string msg = "";
                var result = _factory.DeleteTopic(model.ID, CurrentUser.UserId, ref msg);
                if (!result)
                {
                    ModelState.AddModelError("Name", msg);
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return PartialView("_DeleteTopic", model);
                }
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("TopicDelete: ", ex);
                ModelState.AddModelError("Name", "Have an error when you delete a Topic");
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return PartialView("_DeleteTopic", model);
            }
        }

        [HttpGet]
        public PartialViewResult DeleteFAQ(string id)
        {
            FAQDTO model = GetFAQDetail(id);
            return PartialView("_DeleteFAQ", model);
        }

        [HttpPost]
        public ActionResult DeleteFAQ(FAQDTO model)
        {
            try
            {
                string msg = "";
                var result = _factory.DeleteFAQ(model.ID, CurrentUser.UserId, ref msg);
                if (!result)
                {
                    ModelState.AddModelError("TopicName", msg);
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return PartialView("_DeleteFAQ", model);
                }
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("FAQDelete: ", ex);
                ModelState.AddModelError("Name", "Have an error when you delete a FAQ");
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return PartialView("_DeleteFAQ", model);
            }
        }

        [HttpPost]
        public ActionResult SaveTopic(TopicDTO model)
        {
            try
            {               
                string msg = "";
                bool result = _factory.CreateOrEditTopic(model, CurrentUser.UserId, ref msg);
                if (result)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("Name", msg);
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("TopicCreateOrEdit: ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }

        [HttpPost]
        public ActionResult SaveFAQ(FAQDTO model)
        {
            try
            {
                string msg = "";
                bool result = _factory.CreateOrEditFAQ(model, CurrentUser.UserId, ref msg);
                if (result)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("Name", msg);
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("FAQCreateOrEdit: ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
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