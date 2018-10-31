using NSCSC.Shared;
using NSCSC.Shared.Factory.Sandbox.Inventory;
using NSCSC.Shared.Factory.SandBox.Inventory;
using NSCSC.Shared.Models;
using NSCSC.Shared.Models.Sandbox.Inventory.Product;
using NSCSC.Shared.Models.SandBox.Inventory.Product;
using NSCSC.Shared.Utilities;
using NSCSC.Web.App_Start;
using NuWebNCloud.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace NSCSC.Web.Controllers
{
    [NuAuth]
    public class SBInventoryProductsController : HQController
    {
        private ProductFactory _factory = null;

        // GET: SBInventoryProducts
        public SBInventoryProductsController()
        {
            _factory = new ProductFactory();
        }

        public ActionResult Index()
        {
            ProductDetailModels model = new ProductDetailModels();
            
            return View(model);
        }

        [HttpGet]
        public ActionResult LoadFunction(string CategoryId)
        {
            try
            {
                CategoriesFactory _cateFactory = new CategoriesFactory();
                var Type = _cateFactory.GetDetail(CategoryId).Type;
                var lstFunction = _factory.GetProductFunctions(Type).Select(x => new ProductFunctionModels
                {
                    FunctionID = x.ID,
                    FunctionName = x.Name,
                    IsDefault = x.IsDefault,
                    IsSelected = x.IsDefault ? true : false
                }).ToList();
                lstFunction = lstFunction.OrderBy(x => x.FunctionName).ToList();
                return PartialView("_ItemFunctions", lstFunction);
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("LoadFunction Error", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }

        public ActionResult Create()
        {
            ProductDetailModels model = new ProductDetailModels();
            try
            {
                var _user = Session["User"] as UserSession;
                if(_user == null)
                    return RedirectToAction("Index", "Login", new { area = "" });
                else
                {
                    CategoriesFactory _cateFactory = new CategoriesFactory();
                    var lstCategory = _cateFactory.GetListCategory(_user.UserId);
                    lstCategory = lstCategory.OrderBy(o => o.Name).ThenBy(o => o.Id).ToList();
                    var _categories = lstCategory
                                                    .Where(x=>x.IsActive)
                                                    .Select(x=> new SelectListItem
                                                    {
                                                        Value = x.Id,
                                                        Text = x.Name
                                                    })
                                                    .ToList();
                    model.Categories = _categories;

                    var Type = lstCategory != null ? lstCategory.Where(x=>x.IsActive).FirstOrDefault().Type : 0;
                    var lstFunction = _factory.GetProductFunctions(Type).OrderBy(o => o.Name).Select(x=>new ProductFunctionModels
                                                                                {
                                                                                    FunctionID = x.ID,
                                                                                    FunctionName = x.Name,
                                                                                    IsDefault = x.IsDefault,
                                                                                    IsSelected = x.IsDefault ? true : false
                                                                                })
                                                                                .ToList();
                    model.ListFunction = lstFunction;
                }
               
            }
            catch(Exception ex)
            {
                NSLog.Logger.Error("Create: " , ex);
            }
           
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(ProductDetailModels model)
        {
            try
            {
                CategoriesFactory _cateFactory = new CategoriesFactory();
                var _categories = _cateFactory.GetListCategory(CurrentUser.UserId)
                                                .Where(x => x.IsActive)
                                                .Select(x => new SelectListItem
                                                {
                                                    Value = x.Id,
                                                    Text = x.Name
                                                })
                                                .ToList();
                model.Categories = _categories;
                if (model.ListFunction != null && model.ListFunction.Count > 0)
                {
                    model.ListFunction.Where(x => x.IsDefault).ToList().ForEach(x =>  x.IsSelected = true);
                }

                if (string.IsNullOrEmpty(model.Name))
                {
                    ModelState.AddModelError("Name", "The Name field is required");
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return View(model);
                }

                if (string.IsNullOrEmpty(model.Code))
                {
                    ModelState.AddModelError("Code", "The Code field is required");
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return View(model);
                }

                if (string.IsNullOrEmpty(model.CategoryId))
                {
                    ModelState.AddModelError("CategoryId", "The Category field is required");
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return View(model);
                }
                if (model.PictureUpload != null && !Commons.ImageExtendFiler.Contains(model.PictureUpload.ContentType))
                {
                    ModelState.AddModelError("PictureUpload", Commons.ErrorMsgFilterImage);
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return View(model);
                }
                if (model.SaleFrom > model.SaleTo)
                {
                    ModelState.AddModelError("SaleFrom", "From Date must be less than To Date.");
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return View(model);
                }

                if (model.ListPrice == null || (model.ListPrice != null && model.ListPrice.Count == 0))
                {
                    ModelState.AddModelError("ListPrice", "The Price Items is required");
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return View(model);
                }



                //if (model.ListPrice != null && model.ListPrice.Count > 0)
                //{
                //    var isExistActive = model.ListPrice.Any(x => x.IsActive);
                //    if (!isExistActive)
                //    {
                //        ModelState.AddModelError("ListPrice", "Please enter a price item greater than or equal to 1 is active");
                //        Response.StatusCode = (int)HttpStatusCode.BadRequest;
                //        return View(model);
                //    }
                //}


                //if (model.IsExtend && (model.ListPriceExtend == null ||( model.ListPriceExtend != null && model.ListPriceExtend.Count == 0)))
                //{
                //    ModelState.AddModelError("ListPriceExtend", "The Extend Price Items is requered");
                //    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                //    return View(model);
                //}

                if (!ModelState.IsValid)
                {
                    if ((ModelState["PictureUpload"]) != null && (ModelState["PictureUpload"]).Errors.Count > 0)
                        model.ImageURL = "";
                    return View(model);
                }
                /*** Processing Image *****/
                byte[] photoByte = null;
                if (model.PictureUpload != null && model.PictureUpload.ContentLength > 0)
                {
                    Byte[] imgByte = new Byte[model.PictureUpload.ContentLength];
                    model.PictureUpload.InputStream.Read(imgByte, 0, model.PictureUpload.ContentLength);
                    model.PictureByte = imgByte;
                    model.ImageURL = Guid.NewGuid() + Path.GetExtension(model.PictureUpload.FileName);
                    model.PictureUpload = null;
                    photoByte = imgByte;
                }
                /*** End Processing Image ****/
                string msg = "";
                model.CreateUser = CurrentUser.UserId;
                model.ProductType = (int)Commons.EProductType.Product;
                if (model.ListCategory == null)
                    model.ListCategory = new List<ProductCategoryModels>();

                model.ListCategory.Add(new ProductCategoryModels {
                    CategoryID = model.CategoryId
                });
                model.ListPrice.AddRange(model.ListPriceExtend);
                model.ListPrice.Where(x => x.IsNew).ToList().ForEach(x => x.ID = null);
                model.ListFunction = model.ListFunction.Where(x => x.IsSelected).ToList();
                var tmp = model.PictureByte;
                model.PictureByte = null;
                bool result = _factory.CreateOrEdit(model, ref msg);
                if(result)
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
                    model.ListPrice = model.ListPrice.Where(w => w.IsExtend == false).ToList();
                    model.ListPrice.Where(x => x.IsNew).ToList().ForEach(x => x.ID = Guid.NewGuid().ToString());
                    //ModelState.AddModelError("Name", Commons.ErrorMsg);
                    ModelState.AddModelError("Error", msg);
                    return View(model);
                }

            }
            catch(Exception ex)
            {
                NSLog.Logger.Error("Create :", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }

        public ProductDetailModels GetDetail(string Id)
        {
            try
            {
                var model = _factory.GetDetail(Id);
                model.CategoryId = model.ListCategory.Select(x => x.CategoryID).FirstOrDefault();
                model.ImageURL = string.IsNullOrEmpty(model.ImageURL) ? Commons.Image100_100 : model.ImageURL;
                model.CategoryName = model.ListCategory.Select(x => x.CategoryName).FirstOrDefault();
                if (model.ListComposite == null)
                    model.ListComposite = new List<ProductCompositeModels>();
                if (model.ListPrice == null)
                {
                    model.ListPrice = new List<ProductPriceModels>();
                    model.ListPriceExtend = new List<ProductPriceModels>();
                }
                else
                {
                    var _ListPrice = model.ListPrice.Where(x => !x.IsExtend).ToList();
                    var _ListPriceExtend = model.ListPrice.Where(x => x.IsExtend).ToList();

                    model.ListPrice = _ListPrice;
                    model.ListPriceExtend = _ListPriceExtend;
                }

                if (model.ListFunction != null)
                    model.ListFunction.Where(x => x.IsDefault).ToList().ForEach(x => x.IsSelected = true);
                return model;
            }catch(Exception ex)
            {
                NSLog.Logger.Error("GetDetail :", ex);
                return null;
            }
        }

        [HttpGet]
        public ActionResult ViewProduct(string Id)
        {
            try
            {
                var model = GetDetail(Id);
                return PartialView("_View", model);
            }catch(Exception ex)
            {
                NSLog.Logger.Error("View :", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }

        [HttpGet]
        public PartialViewResult Edit(string Id)
        {
            var model = GetDetail(Id);

            CategoriesFactory _cateFactory = new CategoriesFactory();

            var _categories = _cateFactory.GetListCategory(CurrentUser.UserId);
            //Can not change category has different type with the old category.
            int curCateType = _categories.Where(w => w.Id == model.CategoryId).Select(s => s.Type).FirstOrDefault();
            var lstCategories = new List<SelectListItem>();
            lstCategories = _categories.Where(x => x.IsActive && x.Type == curCateType && x.Id != model.CategoryId)
                                            .Select(x => new SelectListItem
                                            {
                                                Value = x.Id,
                                                Text = x.Name
                                            })
                                            .ToList();
            lstCategories.Add(new SelectListItem
            {
                Value = model.CategoryId,
                Text = model.CategoryName
            });

            model.Categories = lstCategories;
            return PartialView("_Edit", model);
        }

        public ActionResult Edit(ProductDetailModels model)
        {
            string _CurrentURL = model.ImageURL;
            try
            {
                CategoriesFactory _cateFactory = new CategoriesFactory();
                //var _categories = _cateFactory.GetListCategory(CurrentUser.UserId)
                //                                .Where(x => x.IsActive)
                //                                .Select(x => new SelectListItem
                //                                {
                //                                    Value = x.Id,
                //                                    Text = x.Name
                //                                })
                //                                .ToList();
                //model.Categories = _categories;
                var _categories = _cateFactory.GetListCategory(CurrentUser.UserId);
                //Can not change category has different type with the old category.
                int curCateType = _categories.Where(w => w.Id == model.CategoryId).Select(s => s.Type).FirstOrDefault();
                var lstCategories = new List<SelectListItem>();
                lstCategories = _categories.Where(x => x.IsActive && x.Type == curCateType && x.Id != model.CategoryId)
                                                .Select(x => new SelectListItem
                                                {
                                                    Value = x.Id,
                                                    Text = x.Name
                                                })
                                                .ToList();
                lstCategories.Add(new SelectListItem
                {
                    Value = model.CategoryId,
                    Text = model.CategoryName
                });
                model.Categories = lstCategories;
                if (string.IsNullOrEmpty(model.Name))
                {
                    ModelState.AddModelError("Name", "The Name field is required");
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return PartialView("_Edit", model);
                }

                if (string.IsNullOrEmpty(model.Code))
                {
                    ModelState.AddModelError("Code", "The Code field is required");
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return PartialView("_Edit", model);
                }

                if (string.IsNullOrEmpty(model.CategoryId))
                {
                    ModelState.AddModelError("CategoryId", "The Category field is required");
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return PartialView("_Edit", model);
                }
                if (model.PictureUpload != null && !Commons.ImageExtendFiler.Contains(model.PictureUpload.ContentType))
                {
                    ModelState.AddModelError("PictureUpload", Commons.ErrorMsgFilterImage);
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return PartialView("_Edit", model);
                }
                if (model.SaleFrom > model.SaleTo)
                {
                    ModelState.AddModelError("SaleFrom", "From Date must be less than To Date.");
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return PartialView("_Edit", model);
                }

                if (model.ListPrice == null || (model.ListPrice != null && model.ListPrice.Count == 0))
                {
                    ModelState.AddModelError("ListPrice", "The Price Items is required");
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return PartialView("_Edit", model);
                }

               

                //if (model.ListPrice != null && model.ListPrice.Count > 0)
                //{
                //    var isExistActive = model.ListPrice.Any(x => x.IsActive);
                //    if (!isExistActive)
                //    {
                //        ModelState.AddModelError("ListPrice", "Please enter a price item greater than or equal to 1 is active");
                //        Response.StatusCode = (int)HttpStatusCode.BadRequest;
                //        return PartialView("_Edit", model);
                //    }
                //}

                //if (model.IsExtend && (model.ListPriceExtend == null ||(model.ListPriceExtend != null && model.ListPriceExtend.Count == 0)))
                //{
                //    ModelState.AddModelError("ListPriceExtend", "The Extend Price Items is requered");
                //    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                //    return PartialView("_Edit", model);
                //}

                if (!ModelState.IsValid)
                {
                    if ((ModelState["PictureUpload"]) != null && (ModelState["PictureUpload"]).Errors.Count > 0)
                        model.ImageURL = "";
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return PartialView("_Edit", model);
                }

                if (string.IsNullOrEmpty(model.ImageURL))
                {
                    model.ImageURL = model.ImageURL.Replace(Commons.PublicImages, "").Replace(Commons.Image100_100, "");
                }
                /*** Processing Image *****/
                byte[] photoByte = null;
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
                {
                    model.ImageURL = model.ImageURL.Replace(Commons.PublicImages, "").Replace(Commons.Image100_100, "");
                }
                /*** End Processing Image ****/
                string msg = "";
                model.CreateUser = CurrentUser.UserId;
                model.ProductType = (int)Commons.EProductType.Product;
                if (model.ListCategory == null)
                    model.ListCategory = new List<ProductCategoryModels>();

                model.ListCategory.Add(new ProductCategoryModels
                {
                    CategoryID = model.CategoryId
                });
                model.ListPrice.AddRange(model.ListPriceExtend);
                model.ListPrice.Where(x => x.IsNew).ToList().ForEach(x => x.ID = null);
                var tmp = model.PictureByte;
                model.PictureByte = null;
                if (model.ListFunction != null && model.ListFunction.Count > 0)
                {
                    model.ListFunction.Where(x => x.IsDefault).ToList().ForEach(x => x.IsSelected = true);
                }
                bool result = _factory.CreateOrEdit(model, ref msg);
                if (result)
                {
                    model.PictureByte = tmp;
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
                    //ModelState.AddModelError("Name", Commons.ErrorMsg);
                    ModelState.AddModelError("Error", msg);
                    model.ImageURL = _CurrentURL;
                    model.ListPrice = model.ListPrice.Where(w => w.IsExtend == false).ToList();
                    model.ListPrice.Where(x => x.IsNew).ToList().ForEach(x => x.ID = Guid.NewGuid().ToString());
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return PartialView("_Edit", model);
                }
            }
            catch(Exception ex)
            {
                NSLog.Logger.Error("Edit :", ex);
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
            }catch(Exception ex)
            {
                NSLog.Logger.Error("CheckPinCode", ex);
            }
            return Result;
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
                NSLog.Logger.Error("Products Delete: ", ex);
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        [HttpPost]
        public ActionResult DeleteProduct(string PinCode, string Reason, string Id)
        {
            try
            {
                var IsCheckPinCode = CheckPinCode(PinCode);
                if(!IsCheckPinCode)
                {
                    return Json(new { Status = "400", Message = "Pin Code Is invalid" , IsPinCode=true});
                }
                else
                {
                    string msg = "";
                    var result = _factory.Delete(Id, CurrentUser.UserId, ref msg,Reason);
                    if (!result)
                    {
                        return Json(new { Status = "400", Message = "This product already has transaction. Cannot delete this one.", IsPinCode = false });
                    }
                    return Json(new { Status = "200", Message = "Success."});
                }
            }catch(Exception ex)
            {
                NSLog.Logger.Error("DeleteProduct : ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }

        [HttpGet]
        public ActionResult Delete(string id)
        {
            try
            {
                var model = GetDetail(id);
                return PartialView("_Delete", model);
            }
            catch(Exception ex)
            {
                NSLog.Logger.Error("Delete :", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
           
        }

        [HttpPost]
        public ActionResult Delete(ProductDetailModels model)
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
                NSLog.Logger.Error("ProductDelete: ", ex);
                ModelState.AddModelError("Name", "Have an error when you delete a Products");
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return PartialView("_Delete", model);
            }
        }

        public ActionResult Search()
        {
            try
            {
                string SearchString = "";
                ProductDetailModels model = new ProductDetailModels();
                var _ProductType =(int)Commons.EProductType.Product;
                model.ListProducts = _factory.GetListData(SearchString, _ProductType);
                return PartialView("_ListData", model);
            }
            catch(Exception e)
            {
                NSLog.Logger.Error("Search :" , e);
                return new HttpStatusCodeResult(400, e.Message);
            }
           
        }

        #region "Price Items"

        [HttpPost]
        public ActionResult AddEditPriceItem(ProductPriceModels ProductPrice, List<ProductPriceModels> ProductDetailModels)
        {
            try
            {
                if (ProductDetailModels == null)
                    ProductDetailModels = new List<ProductPriceModels>();

                var IsEdit = false;
                if (ProductPrice != null && !string.IsNullOrEmpty(ProductPrice.ID) 
                    && ProductDetailModels != null  && ProductDetailModels.Count > 0)
                {
                    IsEdit = ProductDetailModels.Any(x => x.ID == ProductPrice.ID && !x.IsExtend);

                }

                if(IsEdit)
                {
                    ProductDetailModels.Where(x => x.ID == ProductPrice.ID && !x.IsExtend)
                                                    .Select(x => x).ToList()
                                                    .ForEach(x =>
                                                                {
                                                                    x.Price = ProductPrice.Price;
                                                                    x.Period = ProductPrice.Period;
                                                                    x.PeriodType = ProductPrice.PeriodType;
                                                                });
                }
                else
                {
                    //ProductPrice.OffSet = model.ListPrice.Max(x => x.OffSet) + 1;
                    ProductPrice.ID = Guid.NewGuid().ToString();
                    ProductPrice.IsExtend = false;
                    ProductPrice.IsActive = true;
                    ProductPrice.IsNew = true;
                    ProductDetailModels.Add(ProductPrice);
                }
                ProductDetailModels model = new ProductDetailModels();
                model.ListPrice.AddRange(ProductDetailModels);
                return PartialView("_ItemModalPriceItems", model);
            }catch(Exception e)
            {
                NSLog.Logger.Error("AddPriceItem :", e);
                return new HttpStatusCodeResult(400, e.Message);
            }
        }

        [HttpPost]
        public ActionResult DeletePriceItem(string Id , ProductDetailModels model)
        {
            try
            {
                if(model != null && model.ListPrice != null && model.ListPrice.Count > 0)
                {
                    var _Product = model.ListPrice.FirstOrDefault(x => x.ID == Id && !x.IsExtend);
                    if (_Product != null)
                        model.ListPrice.Remove(_Product);
                }
                return PartialView("_ItemModalPriceItems", model);
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("DeletePriceItem :", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }

        [HttpPost]
        public JsonResult GetDetailPriceItem(string Id, ProductDetailModels model)
        {
            var _Product = new ProductPriceModels();
            try
            {
                if(model != null && model.ListPrice != null && model.ListPrice.Count > 0)
                {
                    _Product = model.ListPrice.FirstOrDefault(x => x.ID == Id && !x.IsExtend);
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("GetDetailPriceItem : ", ex);
            }
            return Json(_Product);
        }

        [HttpPost]
        public ActionResult SetActiveUnActivePriceItem(string Id, ProductDetailModels model, bool IsActive)
        {
            try
            {
                model.ListPrice.Where(x => x.ID == Id).ToList().ForEach(x => x.IsActive = IsActive);
                return PartialView("_ItemModalPriceItems", model);
            }catch(Exception ex)
            {
                NSLog.Logger.Error("SetActiveUnActivePriceItem : ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }

        #endregion

        #region "Extend Price Items"

        [HttpPost]
        public ActionResult AddEditExtendPriceItem(ProductPriceModels ProductPrice, ProductDetailModels model)
        {
            try
            {
                if (model == null)
                    model = new ProductDetailModels();

                var IsEdit = false;
                if (ProductPrice != null && !string.IsNullOrEmpty(ProductPrice.ID)
                    && model != null && model.ListPriceExtend != null && model.ListPriceExtend.Count > 0)
                {
                    IsEdit = model.ListPriceExtend.Any(x => x.ID == ProductPrice.ID && x.IsExtend);
                }
                if(IsEdit)
                {
                    model.ListPriceExtend.Where(x => x.ID == ProductPrice.ID && x.IsExtend)
                                 .Select(x => x).ToList()
                                 .ForEach(x =>
                                 {
                                     x.Price = ProductPrice.Price;
                                     x.Period = ProductPrice.Period;
                                     x.PeriodType = ProductPrice.PeriodType;
                                 });
                }
                else
                {
                    ProductPrice.ID = Guid.NewGuid().ToString();
                    ProductPrice.IsExtend = true;
                    ProductPrice.IsActive = true;
                    ProductPrice.IsNew = true;
                    model.ListPriceExtend.Add(ProductPrice);
                    //ProductPrice.OffSet = model.ListPrice.Max(x => x.OffSet) + 1;
                }
                return PartialView("_ItemModalExtendPriceItems", model);
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("AddPriceItem :", e);
                return new HttpStatusCodeResult(400, e.Message);
            }
        }

        [HttpPost]
        public ActionResult DeleteExtendPriceItem(string Id, ProductDetailModels model)
        {
            try
            {
                if (model != null && model.ListPriceExtend != null && model.ListPriceExtend.Count > 0)
                {
                    var _Product = model.ListPriceExtend.FirstOrDefault(x => x.ID == Id && x.IsExtend);
                    if (_Product != null)
                        model.ListPriceExtend.Remove(_Product);
                }
                return PartialView("_ItemModalExtendPriceItems", model);
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("DeleteExtendPriceItem :", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }

        [HttpPost]
        public JsonResult GetDetailExtendPriceItem(string Id, ProductDetailModels model)
        {
            var _Product = new ProductPriceModels();
            try
            {
                if (model != null && model.ListPriceExtend != null && model.ListPriceExtend.Count > 0)
                {
                    _Product = model.ListPriceExtend.FirstOrDefault(x => x.ID == Id && x.IsExtend);
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("GetDetailExtendPriceItem : ", ex);
            }
            return Json(_Product);
        }

        [HttpPost]
        public ActionResult SetActiveUnActiveExtendPriceItem(string Id, ProductDetailModels model, bool IsActive)
        {
            try
            {
                model.ListPriceExtend.Where(x => x.ID == Id).ToList().ForEach(x => x.IsActive = IsActive);
                return PartialView("_ItemModalExtendPriceItems", model);
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("SetActiveUnActivePriceItem : ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }

        #endregion

        #region "Hardware"
        [HttpPost]
        public ActionResult GetListHardware(ProductDetailModels model, bool IsCreate = false)
        {
            try
            {
                string SearchString = "";
                var _ProductType =(int)Commons.EProductType.Addition;
                var _AdditionType = (int)Commons.EAdditionType.Hardware;
                var _Products = _factory.GetListData(SearchString, _ProductType)
                                            .Where(x => x.AdditionType == _AdditionType 
                                                        || x.AdditionType == (int)Commons.EAdditionType.Service)
                                            .OrderBy(x => x.AdditionType)
                                            .Select(x=> new ProductModels
                                            {
                                                ProductType = x.ProductType,
                                                Name = x.Name,
                                                ID = x.ID,
                                                Quantity = x.Quantity.HasValue ? x.Quantity.Value : 1,
                                                Sequence = x.Sequence.HasValue ? x.Sequence.Value : 0,
                                                AdditionType = x.AdditionType,
                                                //CategoryName = Commons.EAdditionType.Hardware.ToString()
                                            })
                                            
                                            .ToList();

                /**** List Id Composite ****/
                var CompositeIds = model.ListComposite
                                        .Select(x => x.ProductID).ToList();

                /***** Comparse exits Product and Composite *****/
                _Products.Where(x => CompositeIds.Contains(x.ID)).Select(x => x).ToList()
                                               .ForEach(x =>
                                               {
                                                   x.IsSelect = true;
                                                   x.Quantity = model.ListComposite.FirstOrDefault(y => y.ProductID == x.ID) != null ? model.ListComposite.FirstOrDefault(y => y.ProductID == x.ID).Quantity : 1;
                                                   x.Sequence = model.ListComposite.FirstOrDefault(y => y.ProductID == x.ID) != null ? model.ListComposite.FirstOrDefault(y => y.ProductID == x.ID).Sequence : 0;
                                               });
                if (model == null)
                    model = new ProductDetailModels();
                model.ListProducts = _Products;
                
                return PartialView("_ItemModalCompositeHardware", model);
            }catch(Exception e)
            {
                NSLog.Logger.Error("GetListHardware : ", e);
                return new HttpStatusCodeResult(400, e.Message);
            }
        }

        [HttpPost]
        public ActionResult GetListServices(ProductDetailModels model)
        {
            try
            {
                string SearchString = "";
                var _ProductType = (int)Commons.EProductType.Addition;
                var _AdditionType = (int)Commons.EAdditionType.Service;
                var _Products = _factory.GetListData(SearchString, _ProductType)
                                            .Where(x => x.AdditionType == _AdditionType)
                                            .Select(x => new ProductModels
                                            {
                                                ProductType = x.ProductType,
                                                Name = x.Name,
                                                ID = x.ID,
                                                Quantity = x.Quantity.HasValue ? x.Quantity.Value : 1,
                                                Sequence = x.Sequence.HasValue ? x.Sequence.Value : 0,
                                                AdditionType = x.AdditionType,
                                                CategoryName = Commons.EAdditionType.Service.ToString()
                                            })
                                            .ToList();
                /****** Add Code Products *****/
                //_Products = new List<ProductModels>()
                //{
                //    new ProductModels { ID = "40b849b9-5f50-48f8-9546-c7161f465a68", ProductType = 2, AdditionType = 3, Name = "Ipad Mini" , IsSelect = false, Quantity = 0 , Sequence = 0, CategoryName="Hardware"},
                //    new ProductModels { ID = "28734388-5b92-4ddc-b583-5a304df8954a", ProductType = 2, AdditionType = 3, Name = "Printer HP", IsSelect = false, Quantity = 0 , Sequence = 0,CategoryName="Hardware" },
                //};

                /**** List Id Composite ****/
                var CompositeIds = model.ListComposite
                                        .Where(x=>x.AdditionType == _AdditionType)
                                        .Select(x => x.ProductID).ToList();

                /***** Comparse exits Product and Composite *****/
                _Products.Where(x => CompositeIds.Contains(x.ID)).Select(x => x).ToList()
                                                .ForEach(x =>
                                                {
                                                    x.IsSelect = true;
                                                    x.Quantity = model.ListComposite.FirstOrDefault(y => y.ProductID == x.ID) != null ? model.ListComposite.FirstOrDefault(y => y.ProductID == x.ID).Quantity : 1;
                                                    x.Sequence = model.ListComposite.FirstOrDefault(y => y.ProductID == x.ID) != null ? model.ListComposite.FirstOrDefault(y => y.ProductID == x.ID).Sequence : 0;
                                                });


                if (model == null)
                    model = new ProductDetailModels();
                model.ListProducts = _Products;

                return PartialView("_ItemModalCompositeServices", model);
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("GetListHardware : ", e);
                return new HttpStatusCodeResult(400, e.Message);
            }
        }

        [HttpPost]
        public ActionResult SaveComposite(ProductDetailModels model)
        {
            try
            {
                model.ListComposite.OrderBy(x => x.Sequence).ToList();
                return PartialView("_ItemModalComposite", model);
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("SaveComposite : ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }

        [HttpPost]
        public ActionResult DeleteComposite(string Id ,ProductDetailModels model)
        {
            try
            {
                if(model != null && model.ListComposite != null && model.ListComposite.Count > 0)
                {
                    var _Composite = model.ListComposite.FirstOrDefault(x => x.ProductID == Id);
                    model.ListComposite.Remove(_Composite);
                }
                return PartialView("_ItemModalComposite", model);
            }catch(Exception ex)
            {
                NSLog.Logger.Error("DeleteComposite : ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }
        #endregion
    }
}