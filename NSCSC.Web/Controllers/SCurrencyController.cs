using NSCSC.Shared.Factory.Settings.Currency;
using NSCSC.Shared.Models.Settings.Currency;
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
    public class SCurrencyController : HQController
    {
        private CurrencyFactory _factory = null;
        public SCurrencyController()
        {
            _factory = new CurrencyFactory();
        }
        // GET: SCurrency
        public ActionResult Index()
        {
            try
            {
                CurrencyViewModels model = new CurrencyViewModels();
                return View(model);
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("CurrencyIndex: ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }

        public ActionResult Search(CurrencyViewModels model)
        {
            try
            {
                model = new CurrencyViewModels();
                var listData = _factory.GetListData();
                model.ListItem = listData;
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("CurrencySearch: ", e);
                return new HttpStatusCodeResult(400, e.Message);
            }
            return PartialView("_ListData", model);
        }

        public CurrencyDTO GetDetail(string id)
        {
            try
            {
                CurrencyDTO model = _factory.GetDetail(id);
                return model;
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("CurrencyDetail: ", ex);
                return null;
            }
        }

        [HttpGet]
        public new PartialViewResult View(string id)
        {
            CurrencyDTO model = GetDetail(id);
            return PartialView("_View", model);
        }

        public ActionResult Create()
        {
            CurrencyDTO model = new CurrencyDTO();
            model.IsActive = true;
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(CurrencyDTO model)
        {
            try
            {
                // Remove white space
                model.Symbol = RemoveSpecChar(model.Symbol);

                //=============================                

                string msg = "";
                bool result = _factory.CreateOrEdit(model, CurrentUser.UserId, ref msg);
                if (result)
                {                    
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("Name", msg);
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("Currency_Create: ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }

        public PartialViewResult Edit(string id)
        {
            CurrencyDTO model = GetDetail(id);
            return PartialView("_Edit", model);
        }

        [HttpPost]
        public ActionResult Edit(CurrencyDTO model)
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
                //    //model.GetListRole(CurrentUser.RoleLevel);
                //    //model.Birthday = DateTime.Now;
                //    //model.HiredDate = DateTime.Now;
                //    //model.ExpiredDate = DateTime.Now;
                //    return PartialView("_Edit", model);
                //}

                // Remove white space
                model.Symbol = RemoveSpecChar(model.Symbol);

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
                NSLog.Logger.Error("Currency_Edit: ", ex);
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
                NSLog.Logger.Error("CurrencyDelete: ", ex);
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        [HttpGet]
        public PartialViewResult Delete(string id)
        {
            CurrencyDTO model = GetDetail(id);
            return PartialView("_Delete", model);
        }


        [HttpPost]
        public ActionResult Delete(CurrencyDTO model)
        {
            try
            {
                string msg = "";
                var result = _factory.Delete(model.ID, CurrentUser.UserId, ref msg);
                if (!result)
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    CurrencyViewModels tmp = new CurrencyViewModels();
                    var listData = _factory.GetListData();
                    tmp.ListItem = listData;
                    return PartialView("_ListData", tmp);
                }
                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("Currency_Delete: ", ex);
                ModelState.AddModelError("Name", "Have an error when you delete a Currency");
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                CurrencyViewModels tmp = new CurrencyViewModels();
                var listData = _factory.GetListData();
                tmp.ListItem = listData;
                return PartialView("_ListData", tmp);
            }
        }
    }
}
