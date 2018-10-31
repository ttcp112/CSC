using NSCSC.Shared;
using NSCSC.Shared.Factory;
using NSCSC.Shared.Factory.SandBox.Inventory;
using NSCSC.Shared.Models;
using NSCSC.Shared.Models.SandBox.Inventory.Discount;
using NSCSC.Web.App_Start;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using System.Linq;
using NSCSC.Shared.Models.Sandbox.Inventory.Product;
using System.IO;
using NSCSC.Shared.Utilities;
using NuWebNCloud.Shared.Utilities;

namespace NSCSC.Web.Controllers
{
    [NuAuth]
    public class SBInventoryPromotionController : HQController
    {

        private PromotionsFactory _promotionsFactory = null;
        private ProductFactory _productFactory = null;

        public SBInventoryPromotionController()
        {
            _promotionsFactory = new PromotionsFactory();
            _productFactory = new ProductFactory();
        }

        public ActionResult Index()
        {
            try
            {
                var models = new List<PromotionModels>();

                return View(models);
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("PromotionIndex: ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }

        public ActionResult Search()
        {
            var models = new List<PromotionModels>();
            try
            {
                models = _promotionsFactory.GetData();
               
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("PromotionSearch: ", e);
                return new HttpStatusCodeResult(400, e.Message);
            }
            return PartialView("_ListData", models);
        }

        public ActionResult Create()
        {
            PromotionModels model = new PromotionModels();
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(PromotionModels model)
        {
            try
            {
                //model.ApplyFrom = DateTime.SpecifyKind(model.ApplyFrom, DateTimeKind.Utc);
                //model.ApplyTo = DateTime.SpecifyKind(model.ApplyTo, DateTimeKind.Utc);

                if (model.ApplyFrom > model.ApplyTo)
                {
                    ModelState.AddModelError("ApplyFrom", "The Sale To date must not be sooner than the Sale From date");
                }

                if (model.IsUnlimited)
                {
                    model.MaximumAmount = -1;
                }
                else
                {
                    if (model.MaximumAmount < 0)
                        ModelState.AddModelError("MaximumAmount", "Please enter a value greater than or equal to 0");
                }

                if (model.ListSpendRule != null && model.ListSpendRule.Count() == 0)
                {
                    ModelState.AddModelError("Spending", "Spending Type is required");
                }

                if(model.ListEarnRule != null && model.ListEarnRule.Count() == 0)
                {
                    ModelState.AddModelError("Earning", "Earning Type is required ");
                }

                if ((model.ListSpendRule != null && model.ListSpendRule.Count() > 0) || (model.ListEarnRule != null && model.ListEarnRule.Count() > 0))
                {
                    model.ListSpendRule = model.ListSpendRule.Where(x => x.Status != (byte)Commons.EStatus.Deleted).ToList();
                    model.ListEarnRule = model.ListEarnRule.Where(x => x.Status != (byte)Commons.EStatus.Deleted).ToList();
                    if (model.ListSpendRule.Count == 0 && model.ListEarnRule.Count == 0)
                    {
                        ModelState.AddModelError("Spending", "Spending Type is required, Earning Type is required ");
                    }
                }
                //# Spending
                if (model.ListSpendRule != null)
                {
                    model.ListSpendRule = model.ListSpendRule.Where(x => x.Status != (byte)Commons.EStatus.Deleted).ToList();
                    if (model.ListSpendRule.Count > 1)
                        model.OperatorSpend = model.ListSpendRule.LastOrDefault().Condition == (int)Commons.EOperatorType.And ?(byte)Commons.EOperatorType.And : (byte)Commons.EOperatorType.Or;// 1 = AND ;2 = OR

                    int index = 1;
                    foreach (var item in model.ListSpendRule)
                    {
                        if (item.SpendOnType == (byte)Commons.ESpendOnType.SpecificItem)
                        {
                            item.ListProduct = item.ListProduct.Where(x => x.Status != (byte)Commons.EStatus.Deleted).ToList();
                            if (item.ListProduct.Count == 0)
                            {
                                ModelState.AddModelError(string.Format("ListSpendRule[{0}].ItemDetail",item.OffSet), "Please select specific items in item detail for spending rule no " + index);
                                break;
                            }
                        }
                        if (item.Amount < 0)
                        {
                            ModelState.AddModelError(string.Format("ListSpendRule[{0}].Amount",item.OffSet), "Please enter value of Quantity/Amount for spending rule no. " + index + "");
                            break;
                        }
                        else
                        {
                            item.Amount = Math.Round(item.Amount, 2);
                        }
                        index++;
                    }
                }
                else
                    ModelState.AddModelError("Spending", "The Promotion must have at least one spending rule");


                //# Earning
                if (model.ListEarnRule != null)
                {
                    model.ListEarnRule = model.ListEarnRule.Where(x => x.Status != (byte)Commons.EStatus.Deleted).ToList();
                    if (model.ListEarnRule.Count > 1)
                        model.OperatorEarn = model.ListEarnRule.LastOrDefault().Condition == (int)Commons.EOperatorType.And ? (byte) Commons.EOperatorType.And : (byte)Commons.EOperatorType.Or;
                    int index = 1;
                    foreach (var item in model.ListEarnRule)
                    {
                        if (item.EarnType == (byte)Commons.EEarnType.SpecificItem)
                        {
                            item.ListProduct = item.ListProduct.Where(x => x.Status != (byte)Commons.EStatus.Deleted).ToList();
                            if (item.ListProduct.Count == 0)
                            {
                                ModelState.AddModelError(string.Format("ListEarnRule[{0}].ItemDetail",item.OffSet), "Please select specific items in item detail for earning rule no." + index);
                                break;
                            }
                        }
                        //=======
                        if (item.DiscountType == (byte)Commons.EValueType.Percent)
                        {
                            if (item.DiscountValue < 0 || item.DiscountValue > 100)
                            {
                                ModelState.AddModelError(string.Format("ListEarnRule[{0}].DiscountValue",item.OffSet), "Discount Percent could not larger than 100%");
                                break;
                            }
                        }
                        else
                        {
                            if (item.DiscountValue < 0)
                            {
                                ModelState.AddModelError(string.Format("ListEarnRule[{0}].DiscountValue", item.OffSet), "Please enter value of Percent/Value for earning rule no." + index + "");
                                break;
                            }
                            else
                            {
                                item.DiscountValue = Math.Round(item.DiscountValue, 2);
                            }
                        }
                        if (item.Quantity <= 0)
                        {
                            ModelState.AddModelError(string.Format("ListEarnRule[{0}].Quantity",item.OffSet), "Please enter value of Quantity for earning rule no." + index + "");
                            break;
                        }
                        //=====
                        item.DiscountType = item.DiscountType == (byte)Commons.EValueType.Percent ? (int)Commons.EValueType.Percent : (int)Commons.EValueType.Currency;
                        index++;
                    }
                }
                else
                    ModelState.AddModelError("Earning", "The Promotion must have at least one earnin rule");

                byte[] photoByte = null;
                //# Image
                if (model.PictureUpload != null && model.PictureUpload.ContentLength > 0)
                {
                    Byte[] imgByte = new Byte[model.PictureUpload.ContentLength];
                    model.PictureUpload.InputStream.Read(imgByte, 0, model.PictureUpload.ContentLength);
                    model.PictureByte = imgByte;
                    model.ImageURL = Guid.NewGuid() + Path.GetExtension(model.PictureUpload.FileName);
                    model.PictureUpload = null;
                    photoByte = imgByte;
                }

                if (!ModelState.IsValid)
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    if ((ModelState["PictureUpload"]) != null && (ModelState["PictureUpload"]).Errors.Count > 0)
                        model.ImageURL = "";
                    return View("Create", model);
                }
                var tmp = model.PictureByte;
                model.PictureByte = null;
                string msg = "";
                bool result = _promotionsFactory.CreateOrEdit(model, ref msg);
                if (result)
                {
                    model.PictureByte = tmp;
                    //Save Image on Server
                    if (!string.IsNullOrEmpty(model.ImageURL) && model.PictureByte != null)
                    {
                        var originalDirectory = new DirectoryInfo(string.Format("{0}Uploads\\", Server.MapPath(@"\")));
                        var path = string.Format("{0}{1}", originalDirectory, model.ImageURL);
                        MemoryStream ms = new MemoryStream(photoByte, 0, photoByte.Length);
                        ms.Write(photoByte, 0, photoByte.Length);
                        System.Drawing.Image imageTmp = System.Drawing.Image.FromStream(ms, true);

                        ImageHelper.Me.SaveCroppedImage(imageTmp, path, model.ImageURL, ref photoByte);
                        model.PictureByte = photoByte;

                        FTP.Upload(model.ImageURL, model.PictureByte);

                        ImageHelper.Me.TryDeleteImageUpdated(path);
                    }
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("PromotionCode", msg);
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("PromotionCreate: ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }

        [HttpGet]
        public new PartialViewResult View(string id)
        {
            PromotionModels model = _promotionsFactory.GetDetail(id);
            //===========
            model.ImageURL = string.IsNullOrEmpty(model.ImageURL) ? Commons.Image100_100 : model.ImageURL;
            //=========Spending
            if (model.ListSpendRule != null)
            {
                for (int i = 0; i < model.ListSpendRule.Count; i++)
                {
                    var item = model.ListSpendRule[i];
                    item.OffSet = i;
                    if (item.ListProduct != null)
                    {
                        string ItemDetail = "";
                        for (int j = 0; j < item.ListProduct.Count; j++)
                        {
                            item.ListProduct[j].OffSet = j;
                            ItemDetail += item.ListProduct[j].Name + ",";
                        }
                        item.ItemDetail = ItemDetail;
                    }
                    if (i > 0)
                    {
                        item.Condition = model.OperatorSpend == (int)Commons.EOperatorType.And ? (int)Commons.EOperatorType.And : (int)Commons.EOperatorType.Or;// 1 : AND , 2 : OR
                    }
                }
            }

            if (model.ListEarnRule != null)
            {
                for (int i = 0; i < model.ListEarnRule.Count; i++)
                {
                    var item = model.ListEarnRule[i];
                    item.OffSet = i;
                    if (item.ListProduct != null)
                    {
                        string ItemDetail = "";
                        for (int j = 0; j < item.ListProduct.Count; j++)
                        {
                            item.ListProduct[j].OffSet = j;
                            ItemDetail += item.ListProduct[j].Name + ",";
                        }
                        item.ItemDetail = ItemDetail;
                    }
                    //=======
                    if (i > 0)
                    {
                        item.Condition = model.OperatorEarn == (int)Commons.EOperatorType.And ? (int)Commons.EOperatorType.And : (int)Commons.EOperatorType.Or;// 1 : AND , 2 : OR
                    }
                }
            }
            return PartialView("_View", model);
        }

        public PartialViewResult Edit(string id)
        {
            PromotionModels model = _promotionsFactory.GetDetail(id);
            //===========
            model.ImageURL = string.IsNullOrEmpty(model.ImageURL) ? Commons.Image100_100 : model.ImageURL;
            //=========Spending
            if (model.ListSpendRule != null)
            {
                for (int i = 0; i < model.ListSpendRule.Count; i++)
                {
                    var item = model.ListSpendRule[i];
                    item.OffSet = i;
                    if (item.ListProduct != null)
                    {
                        string ItemDetail = "";
                        for (int j = 0; j < item.ListProduct.Count; j++)
                        {
                            item.ListProduct[j].OffSet = j;
                            ItemDetail += item.ListProduct[j].Name + ",";
                        }
                        item.ItemDetail = ItemDetail;
                    }
                    if (i > 0)
                    {
                        item.Condition = model.OperatorSpend == (int)Commons.EOperatorType.And ? (int)Commons.EOperatorType.And : (int)Commons.EOperatorType.Or;// 1 : AND , 2 : OR
                    }
                }
            }

            if (model.ListEarnRule != null)
            {
                for (int i = 0; i < model.ListEarnRule.Count; i++)
                {
                    var item = model.ListEarnRule[i];
                    item.OffSet = i;
                    if (item.ListProduct != null)
                    {
                        string ItemDetail = "";
                        for (int j = 0; j < item.ListProduct.Count; j++)
                        {
                            item.ListProduct[j].OffSet = j;
                            ItemDetail += item.ListProduct[j].Name + ",";
                        }
                        item.ItemDetail = ItemDetail;
                    }
                    //=======
                    if (i > 0)
                    {
                        item.Condition = model.OperatorEarn == (int)Commons.EOperatorType.And ? (int)Commons.EOperatorType.And : (int)Commons.EOperatorType.Or; // 1 : AND , 2 : OR
                    }
                }
            }
            
            return PartialView("_Edit", model);
        }

        [HttpPost]
        public ActionResult Edit(PromotionModels model)
        {
            var _CurrentImage = model.ImageURL;
            try
            {
                byte[] photoByte = null;
                //# Image
                if (string.IsNullOrEmpty(model.ImageURL))
                    model.ImageURL = model.ImageURL.Replace(Commons.PublicImages, "").Replace(Commons.Image100_100, "");
                if (model.PictureUpload != null && model.PictureUpload.ContentLength > 0)
                {
                    Byte[] imgByte = new Byte[model.PictureUpload.ContentLength];
                    model.PictureUpload.InputStream.Read(imgByte, 0, model.PictureUpload.ContentLength);
                    model.PictureByte = imgByte;
                    model.ImageURL = Guid.NewGuid() + Path.GetExtension(model.PictureUpload.FileName);
                    model.PictureUpload = null;
                    photoByte = imgByte;
                }
                else
                    model.ImageURL = model.ImageURL.Replace(Commons.PublicImages, "").Replace(Commons.Image100_100, "");

                //# Apply From
                //model.ApplyFrom = DateTime.SpecifyKind(model.ApplyFrom, DateTimeKind.Utc);
                //model.ApplyTo = DateTime.SpecifyKind(model.ApplyTo, DateTimeKind.Utc);

                if (model.ApplyFrom > model.ApplyTo)
                    ModelState.AddModelError("FromDate", "The Sale To date must not be sooner than the Sale From date.");

                if (model.IsUnlimited)
                {
                    model.MaximumAmount = -1;
                }
                else
                {
                    if (model.MaximumAmount < 0)
                        ModelState.AddModelError("MaximumAmount", "Please enter a value greater than or equal to 0");
                }

                if (model.ListSpendRule != null && model.ListSpendRule.Count() == 0)
                {
                    ModelState.AddModelError("Spending", "Spending Type is required");
                }

                if(model.ListEarnRule != null && model.ListEarnRule.Count() == 0)
                {
                    ModelState.AddModelError("Earning", "Earning Type is required ");
                }

                if ((model.ListSpendRule != null && model.ListSpendRule.Count() > 0) || (model.ListEarnRule != null && model.ListEarnRule.Count() > 0))
                {
                    model.ListSpendRule = model.ListSpendRule.Where(x => x.Status != (byte)Commons.EStatus.Deleted).ToList();
                    model.ListEarnRule = model.ListEarnRule.Where(x => x.Status != (byte)Commons.EStatus.Deleted).ToList();
                    if (model.ListSpendRule.Count == 0 && model.ListEarnRule.Count ==0)
                    {
                        ModelState.AddModelError("Spending", "Spending Type is required, Earning Type is required ");
                    }
                }
                //# Spending
                if (model.ListSpendRule != null)
                {
                    model.ListSpendRule = model.ListSpendRule.Where(x => x.Status != (byte)Commons.EStatus.Deleted).ToList();
                    if (model.ListSpendRule.Count > 1)
                        model.OperatorSpend = model.ListSpendRule.LastOrDefault().Condition == (int)Commons.EOperatorType.And ? (byte)Commons.EOperatorType.And : (byte)Commons.EOperatorType.Or;

                    int index = 1;
                    foreach (var item in model.ListSpendRule)
                    {
                        if (item.SpendOnType == (byte)Commons.ESpendOnType.SpecificItem)
                        {
                            item.ListProduct = item.ListProduct.Where(x => x.Status != (byte)Commons.EStatus.Deleted).ToList();
                            if (item.ListProduct.Count == 0)
                            {
                                ModelState.AddModelError(string.Format("ListSpendRule[{0}].ItemDetail", item.OffSet), "Please select specific items in item detail for spending rule no " + index);
                                break;
                            }
                        }
                        if (item.Amount < 0)
                        {
                            ModelState.AddModelError(string.Format("ListSpendRule[{0}].Amount", item.OffSet), "Please enter value of Quantity/Amount for spending rule no. " + index + "");
                            break;
                        }
                        else
                        {
                            item.Amount = Math.Round(item.Amount, 2);
                        }
                        index++;
                    }
                }
                else
                    ModelState.AddModelError("Spending", "The Promotion must have at least one spending rule");


                //# Earning
                if (model.ListEarnRule != null)
                {
                    model.ListEarnRule = model.ListEarnRule.Where(x => x.Status != (byte)Commons.EStatus.Deleted).ToList();
                    if (model.ListEarnRule.Count > 1)
                        model.OperatorEarn = model.ListEarnRule.LastOrDefault().Condition == (int)Commons.EOperatorType.And ? (byte)Commons.EOperatorType.And : (byte)Commons.EOperatorType.Or;
                    int index = 1;
                    foreach (var item in model.ListEarnRule)
                    {
                        if (item.EarnType == (byte)Commons.EEarnType.SpecificItem)
                        {
                            item.ListProduct = item.ListProduct.Where(x => x.Status != (byte)Commons.EStatus.Deleted).ToList();
                            if (item.ListProduct.Count == 0)
                            {
                                ModelState.AddModelError(string.Format("ListEarnRule[{0}].ItemDetail", item.OffSet), "Please select specific items in item detail for earning rule no." + index);
                                break;
                            }
                        }
                        //=======
                        if (item.DiscountType == (byte)Commons.EValueType.Percent)
                        {
                            if (item.DiscountValue < 0 || item.DiscountValue > 100)
                            {
                                ModelState.AddModelError(string.Format("ListEarnRule[{0}].DiscountValue", item.OffSet), "Discount Percent could not larger than 100%");
                                break;
                            }
                        }
                        else
                        {
                            if (item.DiscountValue < 0)
                            {
                                ModelState.AddModelError(string.Format("ListEarnRule[{0}].DiscountValue", item.OffSet), "Please enter value of Percent/Value for earning rule no." + index + "");
                                break;
                            }
                            else
                            {
                                item.DiscountValue = Math.Round(item.DiscountValue, 2);
                            }
                        }
                        if (item.Quantity <= 0)
                        {
                            ModelState.AddModelError(string.Format("ListEarnRule[{0}].Quantity", item.OffSet), "Please enter value of Quantity for earning rule no." + index + "");
                            break;
                        }
                        //=====
                        item.DiscountType = item.DiscountType == (byte)Commons.EValueType.Percent ? (int)Commons.EValueType.Percent : (int)Commons.EValueType.Currency;
                        index++;
                    }
                }
                else
                    ModelState.AddModelError("Earning", "The Promotion must have at least one earnin rule");

                if (!ModelState.IsValid)
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    model.ImageURL = _CurrentImage;
                    return PartialView("_Edit", model);
                }
                var tmp = model.PictureByte;
                model.PictureByte = null;
                //====================
                string msg = "";
                bool result = _promotionsFactory.CreateOrEdit(model, ref msg);
                if (result)
                {
                    model.PictureByte = tmp;
                    //Save Image on Server
                    if (!string.IsNullOrEmpty(model.ImageURL) && model.PictureByte != null)
                    {
                        var originalDirectory = new DirectoryInfo(string.Format("{0}Uploads\\", Server.MapPath(@"\")));
                        var path = string.Format("{0}{1}", originalDirectory, model.ImageURL);
                        MemoryStream ms = new MemoryStream(photoByte, 0, photoByte.Length);
                        ms.Write(photoByte, 0, photoByte.Length);
                        System.Drawing.Image imageTmp = System.Drawing.Image.FromStream(ms, true);

                        ImageHelper.Me.SaveCroppedImage(imageTmp, path, model.ImageURL, ref photoByte);
                        model.PictureByte = photoByte;

                        FTP.Upload(model.ImageURL, model.PictureByte);

                        ImageHelper.Me.TryDeleteImageUpdated(path);
                    }
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("PromotionCode", msg);
                    model.ImageURL = _CurrentImage;
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return PartialView("_Edit", model);
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("PromotionEdit: ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }

        public bool CheckPinCode(string PinCode)
        {
            var Result = false;
            try
            {
                if (CurrentUser.PinCode.Equals(PinCode))
                    Result = true;
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("CheckPinCode", ex);
            }
            return Result;
        }

        [HttpPost]
        public ActionResult DeletePromotion(string PinCode, string Reason, string Id)
        {
            try
            {
                var IsCheckPinCode = CheckPinCode(PinCode);
                if (!IsCheckPinCode)
                {
                    return Json(new { Status = "400", Message = "Pin Code Is invalid", IsPinCode = true });
                }
                else
                {
                    PromotionsFactory _factory = new PromotionsFactory();
                    var result = _factory.Delete(Id, Reason);
                    if (!result)
                    {
                        return Json(new { Status = "400", Message = "This promotion already has transaction. Cannot delete this one.", IsPinCode = false });
                    }
                    return Json(new { Status = "200", Message = "Success." });
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("DeleteProduct : ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }

        public ActionResult DeletePopup(string ID, string PinCode, string Reason)
        {
            try
            {
                PromotionsFactory _factory = new PromotionsFactory();
                if (!PinCode.Equals(CurrentUser.PinCode))
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                var result = _factory.Delete(ID, Reason);
                if (!result)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("Promotions Delete: ", ex);
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        [HttpGet]
        public PartialViewResult Delete(string id)
        {
            PromotionModels model = _promotionsFactory.GetDetail(id);
            return PartialView("_Delete", model);
        }

        [HttpPost]
        public ActionResult Delete(PromotionModels model)
        {
            try
            {
                var result = _promotionsFactory.Delete(model.ID);
                if (!result)
                {
                    ModelState.AddModelError("Name", Commons.ErrorMsg);
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return PartialView("_Delete", model);
                }
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("PromotionDelete: ", ex);
                ModelState.AddModelError("Name", Commons.ErrorMsg);
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return PartialView("_Delete", model);
            }
        }

        public ActionResult Export()
        {
            DiscountDetailModels model = new DiscountDetailModels();
            return View(model);
        }

        public ActionResult AddSpending(int currentOffset, string Condition)
        {
            try
            {

                SpendRuleModels group = new SpendRuleModels();
                int _Condition = Condition.Equals("AND") ?  (int)Commons.EOperatorType.And: Condition.Equals("OR") ? (int)Commons.EOperatorType.Or : (int)Commons.EOperatorType.All;
                group.OffSet = currentOffset;
                group.Condition = _Condition;
                return PartialView("_TabSpending", group);
            }
            catch(Exception ex)
            {
                NSLog.Logger.Error("AddSpending", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
            
        }

        public ActionResult LoadItems(int ItemType)
        {
            try
            {
                var ListProduct = _productFactory.GetListData("", ItemType);
                SpendRuleModels model = new SpendRuleModels();
                if(ListProduct != null)
                {
                    model.ListProduct = ListProduct.Select(x => new ProductModels
                    {
                        ID = x.ID,
                        Name = x.Name,
                        Code  = x.Code,
                        ProductType = x.ProductType
                    }).ToList();
                }
                return PartialView("_TableChooseItems", model);
            }
            catch(Exception ex)
            {
                NSLog.Logger.Error("LoadItems : ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
            
        }

        public ActionResult AddItems(/*string[] productIDs, int currentGroupOffSet, int currentItemOffset*/ SpendRuleModels data)
        {
            SpendRuleModels model = new SpendRuleModels();
            if (data.ListProduct != null && data.ListProduct.Count() > 0)
                model.ListProduct = new List<ProductModels>();

            for (int i = 0; i < data.ListProduct.Count(); i++)
            {
                ProductModels item = new ProductModels();

                item.OffSet = data.currentItemOffset;

                item.ID = data.ListProduct[i].ID;
                item.Code = data.ListProduct[i].Code;
                item.Name = data.ListProduct[i].Name;
                item.ProductType = data.ListProduct[i].ProductType;
                model.ListProduct.Add(item);

                data.currentItemOffset++;
            }
            model.OffSet = data.currentgroupOffSet;

            return PartialView("_ItemModal", model);
        }

        public ActionResult AddEarning(int currentOffset, string Condition)
        {
            try
            {
                EarnRuleModels group = new EarnRuleModels();
                int _Condition = Condition.Equals("AND") ?(int)Commons.EOperatorType.And : Condition.Equals("OR") ?(int)Commons.EOperatorType.Or : (int)Commons.EOperatorType.All;
                group.OffSet = currentOffset;
                group.Condition = _Condition;
                return PartialView("_TabEarning", group);
            }
            catch(Exception ex)
            {
                NSLog.Logger.Error("AddEarning : ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }

        public ActionResult LoadItemsEarn(int ItemType)
        {
            try
            {
                var lstDish = _productFactory.GetListData("", ItemType);
                EarnRuleModels model = new EarnRuleModels();
                if (lstDish != null)
                {
                    model.ListProduct = new List<ProductModels>();
                    foreach (var item in lstDish)
                    {
                        ProductModels product = new ProductModels()
                        {
                            ID = item.ID,
                            Name = item.Name,
                            Code = item.Code,
                            ProductType = item.ProductType
                        };
                        model.ListProduct.Add(product);
                    }
                }
                return PartialView("_TableChooseItemsEarn", model);
            }
            catch(Exception ex)
            {
                NSLog.Logger.Error("LoadItemsEarn : ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }
        public ActionResult AddItemsEarn(/*string[] productIDs, int currentGroupOffSet, int currentItemOffset*/ EarnRuleModels data)
        {
            try
            {
                EarnRuleModels model = new EarnRuleModels();
                if (data.ListProduct != null && data.ListProduct.Count() > 0)
                    model.ListProduct = new List<ProductModels>();

                for (int i = 0; i < data.ListProduct.Count(); i++)
                {
                    ProductModels item = new ProductModels();

                    item.OffSet = data.currentItemOffset;

                    item.ID = data.ListProduct[i].ID;
                    item.Code = data.ListProduct[i].Code;
                    item.Name = data.ListProduct[i].Name;
                    item.ProductType = data.ListProduct[i].ProductType;
                    model.ListProduct.Add(item);

                    data.currentItemOffset++;
                }
                model.OffSet = data.currentgroupOffSet;
                return PartialView("_ItemModalEarn", model);
            }
            catch(Exception ex)
            {
                NSLog.Logger.Error("AddItemsEarn : ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }

    }
}