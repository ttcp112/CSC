using NSCSC.Shared.Factory.Settings.Permissions;
using NSCSC.Shared.Models.SandBox.Inventory.Discount;
using NSCSC.Shared.Models.Settings.Permissions;
using NSCSC.Web.App_Start;
using System;
using System.Net;
using System.Web.Mvc;

namespace NSCSC.Web.Controllers
{
    [NuAuth]
    public class SPermissionsController : HQController
    {

        private PermissionsFactory _factory = null;

        public SPermissionsController()
        {
            _factory = new PermissionsFactory();
        }

        // GET: SBInventoryDiscount
        public ActionResult Index()
        {
            try
            {
                PermissionsViewModels model = new PermissionsViewModels();
                return View(model);
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("PermissionsIndex: ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }

        public ActionResult Search(PermissionsViewModels model)
        {
            try
            {
                model = new PermissionsViewModels();
                int level = Convert.ToInt32(CurrentUser.RoleLevel);
                var listData = _factory.GetListData(level);
                model.ListItem = listData;
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("DiscountSearch: ", e);
                return new HttpStatusCodeResult(400, e.Message);
            }
            return PartialView("_ListData", model);
        }

        public void GetListModule(RoleDetailDTO model)
        {
            model.ListPermission = _factory.GetListModule();
        }

        public ActionResult Create()
        {
            RoleDetailDTO model = new RoleDetailDTO();
            GetListModule(model);
            model.Level = CurrentUser.RoleLevel + 1;
            model.IsActive = true;
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(RoleDetailDTO model)
        {
            try
            {
                //if (!model.BType)
                //{
                //    if (model.Value < 0 || model.Value > 100)
                //    {
                //        ModelState.AddModelError("Value","Value must between 0 and 100");
                //    }
                //}

                if (model.Level <= CurrentUser.RoleLevel)
                {
                    ModelState.AddModelError("Level", "The level of role must be > " + CurrentUser.RoleLevel);
                }
                if (!ModelState.IsValid)
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;                    
                    return View("Create", model);
                }

                string msg = "";
                bool result = _factory.CreateOrEdit(model, CurrentUser.UserId, ref msg);
                if (result)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    //return RedirectToAction("Create");
                    ModelState.AddModelError("Name", msg);
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("PermissionCreate: ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }

        public RoleDetailDTO GetDetail(string id)
        {
            try
            {
                RoleDetailDTO model = _factory.GetDetail(id);
                return model;
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("PermissionsDetail: ", ex);
                return null;
            }
        }

        [HttpGet]
        public new PartialViewResult View(string id)
        {
            RoleDetailDTO model = GetDetail(id);
            return PartialView("_View", model);
        }

        public PartialViewResult Edit(string id)
        {
            RoleDetailDTO model = GetDetail(id);
            return PartialView("_Edit", model);
        }

        [HttpPost]
        public ActionResult Edit(RoleDetailDTO model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Name))
                    ModelState.AddModelError("Name", "Permission Name is required");
                if (model.Level <= CurrentUser.RoleLevel)
                {
                    ModelState.AddModelError("Level", "The level of role must be > " + CurrentUser.RoleLevel);
                }
                if (!ModelState.IsValid)
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return PartialView("_Edit", model);
                }

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
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return PartialView("_Edit", model);
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("PermissionEdit: ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
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
                NSLog.Logger.Error("PermissionDelete: ", ex);
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        [HttpGet]
        public PartialViewResult Delete(string id)
        {
            RoleDetailDTO model = GetDetail(id);
            return PartialView("_Delete", model);
        }

        [HttpPost]
        public ActionResult Delete(RoleDetailDTO model)
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
                NSLog.Logger.Error("PermissionDelete: ", ex);
                ModelState.AddModelError("Name", "Have an error when you delete a Permission");
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return PartialView("_Delete", model);
            }
        }

        public ActionResult Export()
        {
            DiscountDetailModels model = new DiscountDetailModels();
            return View(model);
        }


        //[HttpPost]
        //public ActionResult Export(DiscountModels model)
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