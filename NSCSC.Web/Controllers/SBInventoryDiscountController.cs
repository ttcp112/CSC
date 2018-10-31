using NSCSC.Shared;
using NSCSC.Shared.Factory.Sandbox.Inventory;
using NSCSC.Shared.Factory.SandBox.Inventory;
using NSCSC.Shared.Models.SandBox.Inventory.Discount;
using NSCSC.Web.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace NSCSC.Web.Controllers
{
    [NuAuth]
    public class SBInventoryDiscountController : HQController
    {

        private DiscountFactory _factory = null;
        private CategoriesFactory _facCate = null;

        public SBInventoryDiscountController()
        {
            _factory = new DiscountFactory();
            _facCate = new CategoriesFactory();
        }

        // GET: SBInventoryDiscount
        public ActionResult Index()
        {
            try
            {
                DiscountViewModels model = new DiscountViewModels();
                return View(model);
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("DiscountIndex: ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }

        public ActionResult Search(DiscountViewModels model)
        {
            try
            {
                var listData = _factory.GetListData(model.SearchString);
                model.ListItem = listData;
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("DiscountSearch: ", e);
                return new HttpStatusCodeResult(400, e.Message);
            }
            return PartialView("_ListData", model);
        }

        public ActionResult Create()
        {
            DiscountDetailModels model = new DiscountDetailModels();
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(DiscountDetailModels model)
        {
            try
            {
                //if (model.ApplyFrom > model.ApplyTo)
                //{
                //    ModelState.AddModelError("ApplyFrom", "From Date must be less than To Date.");
                //}
                model.ApplyType = model.IsTotalOrder ? (byte)Commons.EDiscountApplyType.Total : (byte)Commons.EDiscountApplyType.Item;

                //if (model.ValueType == (byte)Commons.EValueType.Percent)
                //    if (model.Value < 0 || model.Value > 100)
                //        ModelState.AddModelError("Value", "Value must between 0 and 100");

                //if (model.PeriodType == (byte)Commons.EDiscountPeriodType.Time && model.PeriodTime <= 0)
                //    ModelState.AddModelError("PeriodTime", "Period of time (months) is larger 0");

                if (model.IsTotalOrder)
                {
                    model.ListDiscountCategory = null;
                }
                //else
                //{
                //    model.ListDiscountCategory = model.ListDiscountCategory.Where(x => x.Status != (byte)Commons.EStatus.Deleted).ToList();
                //    if (model.ListDiscountCategory == null || !model.ListDiscountCategory.Any())
                //        ModelState.AddModelError("IsTotalOrder", "Please choose items to apply this discount");
                //}

                if (!ModelState.IsValid)
                {
                    if ((ModelState["PictureUpload"]) != null && (ModelState["PictureUpload"]).Errors.Count > 0)
                        model.ImageURL = "";
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return View(model);
                }

                //===========
                string msg = string.Empty;
                model.CreateUser = CurrentUser.UserId;
                bool result = _factory.CreateOrEdit(model, ref msg);
                if (result)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    string errorString = msg;
                    if (!string.IsNullOrEmpty(msg))
                    {
                        ModelState.AddModelError("Name", msg);
                    }
                    else
                    {
                        errorString = Commons.ErrorMsg;
                        ModelState.AddModelError("Name", Commons.ErrorMsg /*msg*/);
                    }
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return Json(new { errorString });
                    //return View(model);
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("DiscountCreate: ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }

        public DiscountDetailModels GetDetail(string id)
        {
            try
            {
                DiscountDetailModels model = _factory.GetDetail(id);
                if (model.ListDiscountCategory == null)
                    model.ListDiscountCategory = new List<DiscountCategoryModels>();
                else
                    if (model.ListDiscountCategory.Count > 0)
                    model.IsAllCategory = true;

                if (model.IsAllAddition)
                {
                    model.ListDiscountCategory.Add(new DiscountCategoryModels
                    {
                        CategoryID = "",
                        CategoryName = "All",
                        Status = (byte)Commons.EStatus.Actived,
                        Type = "Additions"
                    });
                }
                if (model.IsAllPackage)
                {
                    model.ListDiscountCategory.Add(new DiscountCategoryModels
                    {
                        CategoryID = "",
                        CategoryName = "All",
                        Status = (byte)Commons.EStatus.Actived,
                        Type = "Packages"
                    });
                }
                int OffSet = 0;
                model.ListDiscountCategory.ForEach(x =>
                {
                    x.OffSet = OffSet++;
                    if (string.IsNullOrEmpty(x.Type))
                    //    x.Type = "Category";
                        x.Type = GetCategoryType(x.CateType);

                    if (x.Status == 0)
                        x.Status = (byte)Commons.EStatus.Actived;
                });
                model.ListDiscountCategory = model.ListDiscountCategory.OrderBy(x => x.CategoryName).ToList();
                //=======
                OffSet = 0;
                model.ListDiscountCode.ForEach(x =>
                {
                    x.OffSet = OffSet++;
                    x.Type = (x.State == (byte)Commons.EDiscountCodeState.Active ? Commons.EDiscountCodeState.Active.ToString() :
                                x.State == (byte)Commons.EDiscountCodeState.Used ? Commons.EDiscountCodeState.Used.ToString() :
                                    Commons.EDiscountCodeState.Block.ToString());
                    if (x.Status == 0)
                        x.Status = (byte)Commons.EStatus.Actived;
                });
                model.ListDiscountCode = model.ListDiscountCode.OrderBy(x => x.Type).ToList();

                model.IsTotalOrder = model.ApplyType == (byte)Commons.EDiscountApplyType.Total ? true : false;

                return model;
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("DiscountDetail: ", ex);
                return null;
            }
        }

        [HttpGet]
        public new PartialViewResult View(string id)
        {
            DiscountDetailModels model = GetDetail(id);
            return PartialView("_View", model);
        }

        public PartialViewResult Edit(string id)
        {
            DiscountDetailModels model = GetDetail(id);
            return PartialView("_Edit", model);
        }

        [HttpPost]
        public ActionResult Edit(DiscountDetailModels model)
        {
            try
            {
                model.ApplyType = model.IsTotalOrder ? (byte)Commons.EDiscountApplyType.Total : (byte)Commons.EDiscountApplyType.Item;

                if (!ModelState.IsValid)
                {
                    if ((ModelState["PictureUpload"]) != null && (ModelState["PictureUpload"]).Errors.Count > 0)
                        model.ImageURL = "";
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return PartialView("_Edit", model);
                }

                //====================
                string msg = "";
                model.CreateUser = CurrentUser.UserId;
                var result = _factory.CreateOrEdit(model, ref msg);
                if (result)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    if (msg.Equals("The discount name is existed.Please try again."))
                        ModelState.AddModelError("Name", msg);
                    else
                        ModelState.AddModelError("Name", Commons.ErrorMsg /*msg*/);
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return PartialView("_Edit", model);
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("DiscountEdit: ", ex);
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
                NSLog.Logger.Error("DiscountDelete: ", ex);
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }


        //[HttpGet]
        //public PartialViewResult Delete(string id)
        //{
        //    DiscountDetailModels model = GetDetail(id);
        //    return PartialView("_Delete", model);
        //}
        //[HttpPost]
        //public ActionResult Delete(DiscountModels model)
        //{
        //    try
        //    {
        //        string msg = "";
        //        var result = _factory.Delete(model.ID, CurrentUser.UserId, ref msg);
        //        if (!result)
        //        {
        //            ModelState.AddModelError("Name", msg);
        //            Response.StatusCode = (int)HttpStatusCode.BadRequest;
        //            return PartialView("_Delete", model);
        //        }
        //        return new HttpStatusCodeResult(HttpStatusCode.OK);
        //    }
        //    catch (Exception ex)
        //    {
        //        NSLog.Logger.Error("DiscountDelete: ", ex);
        //        ModelState.AddModelError("Name", "Have an error when you delete a Discount");
        //        Response.StatusCode = (int)HttpStatusCode.BadRequest;
        //        return PartialView("_Delete", model);
        //    }
        //}

        public ActionResult LoadCategories()
        {
            DiscountDetailModels model = new DiscountDetailModels();
            var DataCate = _facCate.GetListCategory(CurrentUser.UserId);
            int OffSet = 0;
            foreach (var item in DataCate)
            {
                model.ClientListDisCate.Add(new ItemDisCate
                {
                    Name = item.Name,
                    Id = item.Id,
                    IsSelect = false,
                    Status = item.IsActive ? "Active" : "Inactive",
                    OffSet = OffSet++,
                    Type = GetCategoryType(item.Type)

                });
            }
            if (model.ClientListDisCate != null)
            {
                model.ClientListDisCate = model.ClientListDisCate.OrderBy(x => x.Name).ToList();
            }
            return PartialView("_ListCategoy", model);
        }

        private string GetCategoryType(int type)
        {
            if (type == (int)Commons.EType.NuDisplay)
            {
                return Commons.EType.NuDisplay.ToString();
            }
            else if (type == (int)Commons.EType.NuKiosk)
            {
                return Commons.EType.NuKiosk.ToString();
            }
            else if (type == (int)Commons.EType.NuPOS)
            {
                return Commons.EType.NuPOS.ToString();
            }
            else if (type == (int)Commons.EType.POinS_Link_App)
            {
                return Commons.EType.POinS_Link_App.ToString();
            }
            else if (type == (int)Commons.EType.POZZ)
            {
                return Commons.EType.POZZ.ToString();
            }
            else if (type == (int)Commons.EType.POZZ_Display)
            {
                return Commons.EType.POZZ_Display.ToString();
            }
            else
                return Commons.EType.POZZ_Kiosk.ToString();
        }

        public ActionResult AddCategories(POSTDisCate data)
        {
            DiscountDetailModels model = new DiscountDetailModels();
            int OffSet = 0;
            if (data.IsAllAddition)
            {
                model.ListDiscountCategory.Add(new DiscountCategoryModels
                {
                    CategoryID = "",
                    CategoryName = "All",
                    Status = (byte)Commons.EStatus.Actived,
                    OffSet = OffSet++,
                    Type = "Additions"
                });
            }
            if (data.IsAllPackage)
            {
                model.ListDiscountCategory.Add(new DiscountCategoryModels
                {
                    CategoryID = "",
                    CategoryName = "All",
                    Status = (byte)Commons.EStatus.Actived,
                    OffSet = OffSet++,
                    Type = "Packages"
                });
            }

            if (data.ListItem != null && data.ListItem.Count > 0)
            {
                foreach (var item in data.ListItem)
                {
                    model.ListDiscountCategory.Add(new DiscountCategoryModels
                    {
                        CategoryID = item.Id,
                        CategoryName = item.Name,
                        OffSet = OffSet++,
                        Status = (byte)Commons.EStatus.Actived,
                        Type = item.Type
                    });
                }
            }
            if (model.ListDiscountCategory != null)
                model.ListDiscountCategory = model.ListDiscountCategory.OrderBy(x => x.CategoryName).ToList();

            return PartialView("_ListDisCate", model);
        }

        public ActionResult GenerateCodes(string ID, int NumberCode)
        {
            DiscountDetailModels model = new DiscountDetailModels();
            try
            {
                string msg = "";
                bool isSuccess = _factory.GenerateCode(ID, NumberCode, CurrentUser.UserId, ref msg);
                if (isSuccess)
                {
                    //DiscountDetailModels obj = GetDetail(ID);
                    //if (obj != null)
                    //{
                    //    model.ListDiscountCode = obj.ListDiscountCode;
                    //    if (model.ListDiscountCode != null)
                    //    {
                    //        int OffSet = 0;
                    //        model.ListDiscountCode.ForEach(x =>
                    //        {
                    //            x.OffSet = OffSet++;
                    //            x.Type = (x.State == (byte)Commons.EDiscountCodeState.Active ? Commons.EDiscountCodeState.Active.ToString() :
                    //                        x.State == (byte)Commons.EDiscountCodeState.Used ? Commons.EDiscountCodeState.Used.ToString() :
                    //                            Commons.EDiscountCodeState.Block.ToString());
                    //        });
                    //    }
                    //}
                }
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("GenerateCodes: ", e);
                return new HttpStatusCodeResult(400, e.Message);
            }
            //return PartialView("_ListCode", model);
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        public ActionResult BlockOrUnBlock(string ID, List<string> ListCodeID)
        {
            DiscountDetailModels model = new DiscountDetailModels();
            try
            {
                string msg = "";
                bool isSuccess = _factory.BlockOrUnblock(ListCodeID, CurrentUser.UserId, ref msg);
                if (isSuccess)
                {
                    //DiscountDetailModels obj = GetDetail(ID);
                    //if (obj != null)
                    //{
                    //    model.ListDiscountCode = obj.ListDiscountCode;
                    //    if (model.ListDiscountCode != null)
                    //    {
                    //        int OffSet = 0;
                    //        model.ListDiscountCode.ForEach(x =>
                    //        {
                    //            x.OffSet = OffSet++;
                    //            x.Type = (x.State == (byte)Commons.EDiscountCodeState.Active ? Commons.EDiscountCodeState.Active.ToString() :
                    //                        x.State == (byte)Commons.EDiscountCodeState.Used ? Commons.EDiscountCodeState.Used.ToString() :
                    //                            Commons.EDiscountCodeState.Block.ToString());
                    //        });
                    //    }
                    //}
                }
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("GenerateBlockOrUnblock: ", e);
                return new HttpStatusCodeResult(400, e.Message);
            }
            //return PartialView("_ListCode", model);
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        public ActionResult GetListDiscountCodes(string ID, int State)
        {
            DiscountDetailModels model = new DiscountDetailModels();
            try
            {
                DiscountDetailModels obj = GetDetail(ID);
                if (obj != null)
                {
                    model.PeriodType = obj.PeriodType;
                    model.ListDiscountCode = obj.ListDiscountCode;
                    model.ListDiscountCode = model.ListDiscountCode.Where(x => x.State == State).ToList();
                    if (model.ListDiscountCode != null)
                    {
                        int OffSet = 0;
                        model.ListDiscountCode.ForEach(x =>
                        {
                            x.OffSet = OffSet++;
                            x.Type = (x.State == (byte)Commons.EDiscountCodeState.Active ? Commons.EDiscountCodeState.Active.ToString() :
                                        x.State == (byte)Commons.EDiscountCodeState.Used ? Commons.EDiscountCodeState.Used.ToString() :
                                        x.State == (byte)Commons.EDiscountCodeState.Block ? Commons.EDiscountCodeState.Block.ToString() :
                                            Commons.EDiscountCodeState.Expired.ToString());
                        });
                    }
                }
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("GetListDiscountCodes: ", e);
                return new HttpStatusCodeResult(400, e.Message);
            }
            return PartialView("_ListCode", model);
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