using NSCSC.Shared;
using NSCSC.Shared.Factory.CRM;
using NSCSC.Shared.Models.CRM.OrderReceiptManagement;
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
    public class CRMOrderReceiptManagementController : HQController
    {
        private OrderReceiptManagementFactory _factory = null;
        public CRMOrderReceiptManagementController()
        {
            _factory = new OrderReceiptManagementFactory();
        }

        // GET: CRMOrderReceiptManagement
        public ActionResult Index()
        {
            try
            {
                OrderReceiptManagementViewModels model = new OrderReceiptManagementViewModels();
                return View(model);
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("OrderReceiptManagementIndex: ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
            //return View();
        }

        public ActionResult Search(OrderReceiptManagementViewModels model)
        {
            try
            {
                var listData = _factory.GetListData();
                model.ListItem = listData;
                model.ListItem.ForEach(x => x.CustomerEmail = MarkEmailView(x.CustomerEmail));
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("OrderReceiptManagementSearch: ", e);
                return new HttpStatusCodeResult(400, e.Message);
            }
            return PartialView("_ListData", model);
        }

        public ReceiptDetailDTO GetDetail(string id)
        {
            try
            {
                ReceiptDetailDTO model = _factory.GetDetail(id);
                if (model.ListAdditionItem != null && model.ListAdditionItem.Count > 0)
                {
                    int count = 0;
                    foreach (var item in model.ListAdditionItem)
                    {
                        switch (item.AdditionType)
                        {
                            case (byte)Commons.EAdditionType.Hardware:
                                item.AdditionName = @Commons.EAdditionType.Hardware.ToString();
                                if (item.State == (byte)Commons.EServiceAssetStatus.New)
                                {
                                    item.SerialNoView = "Updating";
                                }
                                else
                                {
                                    item.SerialNoView = item.SerialNo;
                                }
                                break;
                            //case (byte)Commons.EAdditionType.Software:
                            //    item.AdditionName = @Commons.EAdditionType.Software.ToString();
                            //    break;
                            case (byte)Commons.EAdditionType.Service:
                                item.AdditionName = @Commons.EAdditionType.Service.ToString();
                                break;
                        }
                        //if (string.IsNullOrEmpty(item.SerialNo) && item.State == (byte)Commons.EServiceAssetStatus.New && item.AdditionType == (byte)Commons.EAdditionType.Hardware)
                        //{
                        //    item.SerialNoView = "Updating";
                        //}
                        //else
                        //{
                        //    item.SerialNoView = item.SerialNo;
                        //}

                        item.StateChange = item.State;

                        switch (item.State)
                        {
                            case (byte)Commons.EServiceAssetStatus.New:
                                item.StatusName = @Commons.EServiceAssetStatus.New.ToString();
                                break;
                            case (byte)Commons.EServiceAssetStatus.Done:
                                item.StatusName = @Commons.EServiceAssetStatus.Done.ToString();
                                break;
                            case (byte)Commons.EServiceAssetStatus.Recalled:
                                item.StatusName = @Commons.EServiceAssetStatus.Recalled.ToString();
                                break;
                        }
                        item.Offset = count;
                        count++;
                    }
                }
                return model;
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("OrderReceiptManagementDetail: ", ex);
                return null;
            }
        }

        [HttpGet]
        public new PartialViewResult View(string id)
        {
            ReceiptDetailDTO model = GetDetail(id);
            return PartialView("_View", model);
        }

        public ActionResult Create()
        {
            ReceiptDetailDTO model = new ReceiptDetailDTO();
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(ReceiptDetailDTO model)
        {
            try
            {
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
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("OrderReceiptManagement_Create: ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }

        public PartialViewResult Edit(string id)
        {
            ReceiptDetailDTO model = GetDetail(id);
            return PartialView("_Edit", model);
        }

        [HttpPost]
        public ActionResult Edit(ReceiptDetailDTO model)
        {
            try
            {
                //if (model.ListAdditionItem != null && model.ListAdditionItem.Count > 0)
                //{
                //    var count1 = model.ListAdditionItem.Where(o => o.Update == 9 && o.AdditionType != (byte)Commons.EAdditionType.Service).Select(s =>s.SerialNo).ToList().Count();
                //    var count2 = model.ListAdditionItem.Where(o => o.Update == 9 && o.AdditionType != (byte)Commons.EAdditionType.Service).Select(s => s.SerialNo).Distinct().ToList().Count();
                //    if (count1 != count2)
                //    {
                //        ModelState.AddModelError("errorItem", "Serial number cannot be duplicated.");
                //    }
                //}




                //====================

                List<SerialNumberDTO> ListSerialNo = new List<SerialNumberDTO>();

                var lstSerialNo = model.ListAdditionItem.Where(o => o.Update == 9).ToList();

                if (lstSerialNo != null && !lstSerialNo.Any())
                {
                    ModelState.AddModelError("errorItem", "Nothing has changed. Please try again.");
                }

                if (!ModelState.IsValid)
                {
                    model = GetDetail(model.ID);
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return PartialView("_Edit", model);
                }


                lstSerialNo.ForEach(xx =>
                   {
                       if (!string.IsNullOrEmpty(xx.SerialNo))
                       {
                           xx.SerialNo = RemoveSpecChar(xx.SerialNo);

                       }
                       ListSerialNo.Add(new SerialNumberDTO
                       {
                           AssetID = xx.AssetID,
                           SerialNo = xx.SerialNo,
                           State = xx.StateChange
                       });

                   });
                //lstSerialNo = model.ListAdditionItem.Where(o => o.Update == 9).Select(s => new SerialNumberDTO
                //{
                //    AssetID = s.AssetID,
                //    // Remove white spaces 
                //    SerialNo = s.SerialNo,
                //    State = s.StateChange
                //}).ToList();


                //if (lstSerialNo != null && lstSerialNo.Count > 0)
                //{
                //    foreach (var item in lstSerialNo)
                //    {
                //        if (item.State == (byte)Commons.EServiceAssetStatus.Recalled)
                //        {
                //            //item.SerialNo = "";
                //        }
                //    }
                //}

                model.ListSerialNo = ListSerialNo;
                string msg = "";
                var result = _factory.CreateOrEdit(model, CurrentUser.UserId, ref msg);
                if (result)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    model = GetDetail(model.ID);
                    lstSerialNo.ForEach(
                        xy =>
                        {
                            var itemChanged = model.ListAdditionItem.Where(w => w.AssetID == xy.AssetID).FirstOrDefault();
                            if (itemChanged != null)
                            {
                                itemChanged.SerialNo = xy.SerialNo;
                                itemChanged.SerialNoView = xy.SerialNo;
                                itemChanged.State = xy.StateChange;
                                itemChanged.StateChange = xy.StateChange;
                                itemChanged.StatusName = xy.StatusName;
                                itemChanged.Update = xy.Update;
                            }
                        }
                    );
                    ModelState.AddModelError("errorItem", msg);
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return PartialView("_Edit", model);
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("OrderReceiptManagement_Edit: ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }

        //[HttpPost]
        //public ActionResult CheckSerialNumber(string SeriaNumber, string AssetID)
        //{
        //    try
        //    {
        //        string msg = "";
        //        bool result = _factory.CheckSerialNumber(SeriaNumber, AssetID, ref msg);
        //        if (result)
        //        {
        //            return new HttpStatusCodeResult(HttpStatusCode.OK);
        //        }
        //        else
        //        {
        //            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        NSLog.Logger.Error("TopicCreateOrEdit: ", ex);
        //        return new HttpStatusCodeResult(400, ex.Message);
        //    }
        //}

    }
}