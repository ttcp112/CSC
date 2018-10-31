using NSCSC.Shared.Factory.CRM;
using NSCSC.Shared.Models.CRM.LicenseKeyManagement;
using NSCSC.Web.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using NSCSC.Shared;
using NSCSC.Shared.Utilities;

namespace NSCSC.Web.Controllers
{
    [NuAuth]
    public class CRMLicenseKeyManagementController : HQController
    {
        private LicenseKeyManagementFactory _factory = null;
        public CRMLicenseKeyManagementController()
        {
            _factory = new LicenseKeyManagementFactory();
        }

        // GET: CRMLicenseKeyManagement
        public ActionResult Index()
        {
            try
            {
                LicenseKeyManagementViewModels model = new LicenseKeyManagementViewModels();
                return View(model);
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("LicenseKeyManagementIndex: ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
            //return View();
        }

        public ActionResult Search(LicenseKeyManagementViewModels model)
        {
            try
            {
                var listData = _factory.GetListData();
                model.ListItem = listData;
                model.ListItem.ForEach(x => x.CustomerEmail = MarkEmailView(x.CustomerEmail));
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("LicenseKeyManagementSearch: ", e);
                return new HttpStatusCodeResult(400, e.Message);
            }
            return PartialView("_ListData", model);
        }

        public LicenseDetailDTO GetDetail(string id)
        {
            try
            {
                LicenseDetailDTO model = _factory.GetDetail(id);
                model.PaidTime = CommonHelper.ConvertToLocalTime(model.PaidTime);
                // Check Status License Key
                ViewBag.LicenseStatusName = "";
                ViewBag.ExpiryTimeStr = "";

                if (model.IsActive)
                {
                    ViewBag.LicenseStatusName = "Active";
                }
                else
                {
                    ViewBag.LicenseStatusName = "Blocked";
                }

                if (!model.ExpiredTime.HasValue)
                {
                    ViewBag.ExpiryTimeStr = "Not yet Activated";
                } else {
                    ViewBag.ExpiryTimeStr = model.ExpiredTime.Value.ToString(Commons.FormatDateTime);
                    if (DateTime.Compare(model.ExpiredTime.Value.Date, Commons.MaxDate.Date) >= 0)
                    {
                        ViewBag.ExpiryTimeStr = "Never";
                    } else if (model.ExpiredTime.Value < DateTime.Now)
                    {
                        ViewBag.LicenseStatusName = "Expired";
                    } 
                }

                if (model.ListItem != null && model.ListItem.Count > 0)
                {
                    int index = 0;
                    model.ListItem.ForEach(o => o.Offset = index++);
                    foreach (var item in model.ListItem)
                    {
                        if (item.IsActive)
                        {
                            item.StatusName = "Yes";
                        }
                        else
                            item.StatusName = "No";
                    }
                }
                if (model.ListFunctionName != null && model.ListFunctionName.Count > 0)
                {
                    //foreach (var item in model.ListFunctionName)
                    //{
                    //    model.NameFunction += item + ", ";
                    //}
                    model.NameFunction = String.Join(", ", model.ListFunctionName.OrderBy(oo => oo).ToList());
                }
                return model;
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("LicenseKeyManagementDetail: ", ex);
                return null;
            }
        }

        [HttpGet]
        public new PartialViewResult View(string id)
        {
            LicenseDetailDTO model = GetDetail(id);
            return PartialView("_View", model);
        }


        public PartialViewResult Edit(string id)
        {
            LicenseDetailDTO model = GetDetail(id);
            return PartialView("_Edit", model);
        }

        [HttpPost]
        public ActionResult Edit(LicenseDetailDTO model)
        {
            try
            {
                //if (string.IsNullOrEmpty(model.Name))
                //    ModelState.AddModelError("Name", "Employee Name is required");

                //if (!ModelState.IsValid)
                //{
                //    if ((ModelState["PictureUpload"]) != null && (ModelState["PictureUpload"]).Errors.Count > 0)
                //        model.ImageURL = "";

                //    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                //    return PartialView("_Edit", model);
                //}

                //====================

                string msg = "";
                var result = _factory.CreateOrEdit(model, CurrentUser.UserId, ref msg);
                if (result)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("Name", msg);
                    return PartialView("_Edit", model);
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("LicenseKeyManagement_Edit: ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }
        [HttpPost]
        public ActionResult ChangeStatus(string ID)
        {
            try
            {
                string msg = "";
                bool result = _factory.ChangeStatusLicenseKey(ID, CurrentUser.UserId, ref msg);
                if (result)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("LicenseKeyManagementChangeStatus: ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }
    }
}