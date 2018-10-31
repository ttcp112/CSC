using NSCSC.Shared.Factory.Settings;
using NSCSC.Shared.Models.Settings.PrefixSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NSCSC.Shared;
using System.Net;

namespace NSCSC.Web.Controllers
{
    public class SPrefixSettingsController : HQController
    {
        private PrefixSettingsFactory _PrefixSettingsFactory;
        public SPrefixSettingsController()
        {
            _PrefixSettingsFactory = new PrefixSettingsFactory();
        }
        // GET: SPrefixSettings
        [HttpGet]
        public ActionResult Index()
        {
            try
            {
                List<PrefixSettingsModels> model = new List<PrefixSettingsModels>();
                PrefixSettingsTextModels ListName = new PrefixSettingsTextModels();
                model = _PrefixSettingsFactory.GetListPrefixSettings(CurrentUser.UserId);
                model.ForEach(x =>
                {
                    if (x.Code == (int)Commons.EGeneralSetting.OrderPrefix)
                    {
                        ListName.IDReceiptPrefix = x.ID;
                        ListName.ReceiptPrefix = x.Value;
                        ListName.CodeOrderPrefix = x.Code;
                    }
                    else if (x.Code == (int)Commons.EGeneralSetting.ReceiptPrefix)
                    {
                        ListName.IDOrderPrefix = x.ID;
                        ListName.OrderPrefix = x.Value;
                        ListName.CodeReceiptPrefix = x.Code;
                    }
                    else if (x.Code == (int)Commons.EGeneralSetting.StartNumber)
                    {
                        ListName.IDStartNumber = x.ID;
                        ListName.StartNumber = x.Value;
                        ListName.CodeStartNumber = x.Code;
                    }
                });
                if (ListName != null)
                {
                    ListName.ID = "1";
                }

                return View(ListName);
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("PrefixSettings_Index:", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }
        [HttpPost]
        public ActionResult Create(PrefixSettingsTextModels model)
        {
            try
            {
                List<PrefixSettingsModels> ListPrefixSettings = new List<PrefixSettingsModels>();
                // Remove white space
                model.OrderPrefix = RemoveSpecChar(model.OrderPrefix);
                if (string.IsNullOrEmpty(model.OrderPrefix))
                {
                    ModelState.AddModelError("OrderPrefix", "Order Prefix field is required");
                }
                // Remove white space
                model.ReceiptPrefix = RemoveSpecChar(model.ReceiptPrefix);
                if (string.IsNullOrEmpty(model.ReceiptPrefix))
                {
                    ModelState.AddModelError("ReceiptPrefix", "Receipt Prefix field is required");
                }
                if (string.IsNullOrEmpty(model.StartNumber))
                {
                    ModelState.AddModelError("StartNumber", "Start Number field must Number and is required");
                }
                else if (!string.IsNullOrEmpty(model.StartNumber))
                {                    
                    int num = 0;
                    bool isDouble = false;
                    isDouble = int.TryParse(model.StartNumber, out num);
                    if(!isDouble)
                    {
                        ModelState.AddModelError("StartNumber", "Start Number field not an odd number");
                    }
                    else if (int.Parse(model.StartNumber) < 0)
                    {
                        ModelState.AddModelError("StartNumber", "Start Number field must greater 0");
                    }

                }
                //else if(int.Parse(model.StartNumber) < 0)
                //{
                //    ModelState.AddModelError("StartNumber", "Start Number field must greater 0");
                //}

                ListPrefixSettings = new List<PrefixSettingsModels>
                {
                    new PrefixSettingsModels(){ ID = model.IDOrderPrefix, Value = model.OrderPrefix},
                    new PrefixSettingsModels(){ ID = model.IDReceiptPrefix, Value = model.ReceiptPrefix},
                    new PrefixSettingsModels(){ ID = model.IDStartNumber, Value = model.StartNumber}
                };



                //=========================
                if (!ModelState.IsValid)
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return View("Index", model);
                }
                string msg = "";
                bool result = _PrefixSettingsFactory.UpdatePrefixSettings(ListPrefixSettings, CurrentUser.UserId, ref msg);
                if (result)
                {

                    return RedirectToAction("Index");
                }
                else
                {

                    ModelState.AddModelError("OrderPrefix", Commons.ErrorMsg /*msg*/);
                    return View("Index", model);
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("PrefixSettings_Update : ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }


    }
}