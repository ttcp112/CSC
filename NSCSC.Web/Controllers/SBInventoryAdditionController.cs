using NSCSC.Shared;
using NSCSC.Shared.Factory.Sandbox.Inventory;
using NSCSC.Shared.Factory.SandBox.Inventory;
using NSCSC.Shared.Models.Sandbox.Inventory.Product;
using NSCSC.Shared.Models.SandBox.Inventory.Discount;
using NSCSC.Shared.Models.SandBox.Inventory.Product;
using NSCSC.Shared.Utilities;
using NSCSC.Web.App_Start;
using NuWebNCloud.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace NSCSC.Web.Controllers
{
    [NuAuth]
    public class SBInventoryAdditionController : HQController
    {
        private ProductFactory _factory = null;
        private CategoriesFactory _facCate = null;

        public SBInventoryAdditionController()
        {
            _factory = new ProductFactory();
            _facCate = new CategoriesFactory();
        }

        // GET: SBInventoryAddition
        public ActionResult Index()
        {
            try
            {
                ProductViewModels model = new ProductViewModels();
                return View(model);
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("ProductAdditionIndex: ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }

        public ActionResult Search(ProductViewModels model)
        {
            try
            {
                var listData = _factory.GetListData(model.SearchString, (byte)Commons.EProductType.Addition);
                listData.ForEach(x =>
                {
                    if (x.AdditionType == (byte)Commons.EAdditionType.Hardware)
                        x.CategoryName = Commons.EAdditionType.Hardware.ToString();
                    else if (x.AdditionType == (byte)Commons.EAdditionType.Location)
                        x.CategoryName = Commons.EAdditionType.Location.ToString();
                    else if (x.AdditionType == (byte)Commons.EAdditionType.Service)
                        x.CategoryName = Commons.EAdditionType.Service.ToString();
                    else if (x.AdditionType == (byte)Commons.EAdditionType.Software)
                        x.CategoryName = Commons.EAdditionType.Software.ToString();
                    else if (x.AdditionType == (byte)Commons.EAdditionType.Account)
                        x.CategoryName = Commons.EAdditionType.Account.ToString();
                    else if (x.AdditionType == (byte)Commons.EAdditionType.Function)
                        x.CategoryName = Commons.EAdditionType.Function.ToString();
                });
                model.ListItem = listData;
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("ProducAdditiontSearch: ", e);
                return new HttpStatusCodeResult(400, e.Message);
            }
            return PartialView("_ListData", model);
        }

        public ActionResult Create()
        {
            ProductDetailModels model = new ProductDetailModels();
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(ProductDetailModels model)
        {
            try
            {
                if (model.AdditionType == (int)Commons.EAdditionType.Function)
                {
                    var lstseletedFunction = model.ListFunction.Where(x => x.IsSelected == true).ToList();
                    if (lstseletedFunction.Count < 1)
                    {
                        ModelState.AddModelError("Error", "Unable to save this additional function. At least one function must be selected.");

                    }
                }
                if (model.SaleFrom > model.SaleTo)
                {
                    ModelState.AddModelError("SaleFrom", "From Date must be less than To Date.");
                }
                model.ListCategory = model.ListCategory.Where(x => x.Status != (byte)Commons.EStatus.Deleted).ToList();
                model.ListPrice = model.ListPrice.Where(x => x.Status != (byte)Commons.EStatus.Deleted).ToList();
                if (model.PictureUpload != null && !Commons.ImageExtendFiler.Contains(model.PictureUpload.ContentType))
                {
                    ModelState.AddModelError("PictureUpload", Commons.ErrorMsgFilterImage);
                }
                if (!ModelState.IsValid)
                {
                    //if ((ModelState["PictureUpload"]) != null && (ModelState["PictureUpload"]).Errors.Count > 0)
                    //    model.ImageURL = "";
                    //model.ImageURL = Commons.Image100_100;
                    return View(model);
                }

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

                //============
                model.CreateUser = CurrentUser.UserId;
                model.ProductType = (byte)Commons.EProductType.Addition;
                string msg = "";
                bool result = _factory.CreateOrEdit(model, ref msg);
                if (result)
                {
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
                    //return RedirectToAction("Create");
                    //ModelState.AddModelError("Name", Commons.ErrorMsg /*msg*/);
                    ModelState.AddModelError("Error", msg);
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("ProductAdditionCreate: ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }

        public ProductDetailModels GetDetail(string id)
        {
            try
            {
                ProductDetailModels model = _factory.GetDetail(id);
                int OffSet = 0;
                if (model.ListCategory != null)
                {
                    model.ListCategory.ForEach(x =>
                    {
                        x.OffSet = OffSet++;
                        if (x.Status == 0)
                            x.Status = (byte)Commons.EStatus.Actived;
                    });
                    model.ListCategory = model.ListCategory.OrderBy(x => x.CategoryName).ToList();
                }
                //=================
                OffSet = 0;
                if (model.ListPrice != null)
                {
                    model.ListPrice.ForEach(x =>
                    {
                        x.OffSet = OffSet++;
                        if (x.Status == 0)
                            x.Status = (byte)Commons.EStatus.Actived;
                        //=============
                        if (x.PeriodType == (byte)Commons.EPeriodType.Day)
                            x.NamePeriodType = Commons.EPeriodType.Day.ToString();
                        else if (x.PeriodType == (byte)Commons.EPeriodType.Month)
                            x.NamePeriodType = Commons.EPeriodType.Month.ToString();
                        else if (x.PeriodType == (byte)Commons.EPeriodType.None)
                            x.NamePeriodType = Commons.EPeriodType.None.ToString();
                        else if (x.PeriodType == (byte)Commons.EPeriodType.OneTime)
                            x.NamePeriodType = Commons.EPeriodType.OneTime.ToString().Insert(3, " ");
                        else if (x.PeriodType == (byte)Commons.EPeriodType.Year)
                            x.NamePeriodType = Commons.EPeriodType.Year.ToString();
                    });
                    model.ListPrice = model.ListPrice.OrderBy(x => x.Period).ToList();
                }

                //Get CategoryName from AdditionType
                if (model.AdditionType == (byte)Commons.EAdditionType.Hardware)
                    model.CategoryName = Commons.EAdditionType.Hardware.ToString();
                else if (model.AdditionType == (byte)Commons.EAdditionType.Location)
                    model.CategoryName = Commons.EAdditionType.Location.ToString();
                else if (model.AdditionType == (byte)Commons.EAdditionType.Service)
                    model.CategoryName = Commons.EAdditionType.Service.ToString();
                else if (model.AdditionType == (byte)Commons.EAdditionType.Software)
                    model.CategoryName = Commons.EAdditionType.Software.ToString();
                else if (model.AdditionType == (byte)Commons.EAdditionType.Account)
                    model.CategoryName = Commons.EAdditionType.Account.ToString();
                else if (model.AdditionType == (byte)Commons.EAdditionType.Function)
                    model.CategoryName = Commons.EAdditionType.Function.ToString();

                return model;
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("ProductAdditionDetail: ", ex);
                return null;
            }
        }

        [HttpGet]
        public new PartialViewResult View(string id)
        {
            ProductDetailModels model = GetDetail(id);
            // Don't show default function for addition
            if (model.ListFunction != null && model.ListFunction.Any())
            {
                model.ListFunction = model.ListFunction.Where(w => w.IsDefault == false).ToList();
            }
            return PartialView("_View", model);
        }

        public PartialViewResult Edit(string id)
        {
            ProductDetailModels model = GetDetail(id);
            return PartialView("_Edit", model);
        }

        [HttpPost]
        public ActionResult Edit(ProductDetailModels model)
        {
            try
            {
                if (model.AdditionType == (int)Commons.EAdditionType.Function)
                {
                    var lstseletedFunction = model.ListFunction.Where(x => x.IsSelected == true).ToList();
                    if (lstseletedFunction.Count < 1)
                    {
                        ModelState.AddModelError("Error", "Unable to save this additional function. At least one function must be selected.");

                    }
                }
                if (model.SaleFrom > model.SaleTo)
                    ModelState.AddModelError("SaleFrom", "From Date must be less than To Date.");

                model.ListCategory = model.ListCategory.Where(x => x.Status != (byte)Commons.EStatus.Deleted).ToList();
                model.ListPrice = model.ListPrice.Where(x => x.Status != (byte)Commons.EStatus.Deleted).ToList();
                if (model.PictureUpload != null && !Commons.ImageExtendFiler.Contains(model.PictureUpload.ContentType))
                {
                    ModelState.AddModelError("PictureUpload", Commons.ErrorMsgFilterImage);
                }
                if (!ModelState.IsValid)
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return PartialView("_Edit", model);
                }
                //====================
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
                    if (!string.IsNullOrEmpty(model.ImageURL))
                        model.ImageURL = model.ImageURL.Replace(Commons.PublicImages, "").Replace(Commons.Image100_50, "");
                    //else
                    //    model.ImageURL = model.ImageURL.Replace(Commons.PublicImages, "").Replace(Commons.Image100_50, "");
                }

                model.CreateUser = CurrentUser.UserId;
                model.ProductType = (byte)Commons.EProductType.Addition;
                string msg = "";
                var result = _factory.CreateOrEdit(model, ref msg);
                if (result)
                {
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
                    //ModelState.AddModelError("Name", Commons.ErrorMsg /*msg*/);
                    ModelState.AddModelError("Error", msg);
                    return PartialView("_Edit", model);
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("ProductAdditionEdit: ", ex);
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
                NSLog.Logger.Error("AdditionDelete: ", ex);
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        //[HttpGet]
        //public PartialViewResult Delete(string id)
        //{
        //    ProductDetailModels model = GetDetail(id);
        //    return PartialView("_Delete", model);
        //}
        //[HttpPost]
        //public ActionResult Delete(ProductDetailModels model)
        //{
        //    try
        //    {
        //        string msg = "";
        //        var result = _factory.Delete(model.ID, CurrentUser.UserId, ref msg);
        //        if (!result)
        //        {
        //            ModelState.AddModelError("Name", Commons.ErrorMsg /*msg*/);
        //            Response.StatusCode = (int)HttpStatusCode.BadRequest;
        //            return PartialView("_Delete", model);
        //        }
        //        return new HttpStatusCodeResult(HttpStatusCode.OK);
        //    }
        //    catch (Exception ex)
        //    {
        //        NSLog.Logger.Error("ProductDelete: ", ex);
        //        ModelState.AddModelError("Name", "Have an error when you delete a addition");
        //        Response.StatusCode = (int)HttpStatusCode.BadRequest;
        //        return PartialView("_Delete", model);
        //    }
        //}

        public ActionResult LoadCategories()
        {
            ProductDetailModels model = new ProductDetailModels();
            var DataCate = _facCate.GetListCategory(CurrentUser.UserId);
            int OffSet = 0;
            foreach (var item in DataCate.Where(x => !x.IsFreeTrial))
            {
                model.ClientListDisCate.Add(new ItemDisCate
                {
                    Name = item.Name,
                    Id = item.Id,
                    IsSelect = false,
                    Status = item.IsActive ? "Active" : "Inactive",
                    OffSet = OffSet++,
                });
            }
            if (model.ClientListDisCate != null)
            {
                model.ClientListDisCate = model.ClientListDisCate.OrderBy(x => x.Name).ToList();
            }
            return PartialView("_ListCategoy", model);
        }

        public ActionResult AddCategories(POSTDisCate data)
        {
            ProductDetailModels model = new ProductDetailModels();
            int OffSet = 0;
            if (data.ListItem != null && data.ListItem.Count > 0)
            {
                foreach (var item in data.ListItem)
                {
                    model.ListCategory.Add(new ProductCategoryModels
                    {
                        CategoryID = item.Id,
                        CategoryName = item.Name,
                        OffSet = OffSet++,
                        IsActive = item.Status.ToLower().Equals("active") ? true : false,
                        Status = (byte)Commons.EStatus.Actived,
                    });
                }
                if (model.ListCategory != null)
                    model.ListCategory = model.ListCategory.OrderBy(x => x.CategoryName).ToList();
            }
            return PartialView("_ListAddCate", model);
        }

        public ActionResult AddItems(ProductPriceModels data)
        {
            ProductPriceModels model = new ProductPriceModels();
            model.OffSet = data.currentItemOffset;
            model.Period = data.Period;
            model.Price = data.Price;
            model.IsActive = data.IsActive;
            model.NamePeriodType = data.NamePeriodType + (data.Period > 1 ? "s" : "");
            model.PeriodType = data.PeriodType;
            data.currentItemOffset++;
            return PartialView("_ItemPrice", model);
        }

        public ActionResult GetListFunctionProductType(int Type, List<string> ListFuncItem)
        {
            ProductDetailModels model = new ProductDetailModels();
            var data = _factory.GetProductFunctions(Type);
            data = data.Where(o => o.IsDefault == false).ToList(); /* don't get default function for addition */
            foreach (var item in data)
            {
                model.ListFunction.Add(new ProductFunctionModels
                {
                    FunctionID = item.ID,
                    FunctionName = item.Name,
                    IsSelected = item.IsDefault,
                });
            }
            model.ListFunction.ForEach(x =>
            {
                if (!x.IsSelected)
                    x.IsSelected = ListFuncItem == null ? false : ListFuncItem.Contains(x.FunctionID);
            });
            if (model.ListFunction != null)
                model.ListFunction = model.ListFunction.OrderBy(x => x.FunctionName).ToList();
            return PartialView("_ListFunctionItem", model);
        }

        //public ActionResult Import()
        //{
        //    SandboxImportModel model = new SandboxImportModel();
        //    return View(model);
        //}

        //[HttpPost]
        //public ActionResult Import(SandboxImportModel model)
        //{
        //    try
        //    {
        //        if (model.ListStores == null)
        //        {
        //            ModelState.AddModelError("ListStores", "Please choose store."));
        //            return View(model);
        //        }
        //        //if (model.ImageZipUpload == null || model.ImageZipUpload.ContentLength <= 0)
        //        //{
        //        //    ModelState.AddModelError("ImageZipUpload", "Image Folder (.zip) cannot be null");
        //        //    return View(model);
        //        //}
        //        if (model.ImageZipUpload != null)
        //        {
        //            if (!Path.GetExtension(model.ImageZipUpload.FileName).ToLower().Equals(".zip"))
        //            {
        //                ModelState.AddModelError("ImageZipUpload", "");
        //                return View(model);
        //            }
        //        }
        //        if (model.ExcelUpload == null || model.ExcelUpload.ContentLength <= 0)
        //        {
        //            ModelState.AddModelError("ImageZipUpload", "Excel filename cannot be null"));
        //            return View(model);
        //        }

        //        FileInfo[] listFiles = new FileInfo[] { };
        //        string serverZipExtractPath = System.Web.HttpContext.Current.Server.MapPath("~/Uploads") + "/ExtractFolder";
        //        if (model.ImageZipUpload != null && model.ImageZipUpload.ContentLength > 0)
        //        {
        //            bool isFolderEmpty;
        //            string fileName = Path.GetFileName(model.ImageZipUpload.FileName);
        //            string filePath = string.Format("{0}/{1}", System.Web.HttpContext.Current.Server.MapPath("~/Uploads"), fileName);

        //            //upload file to server
        //            if (System.IO.File.Exists(filePath))
        //                System.IO.File.Delete(filePath);
        //            model.ImageZipUpload.SaveAs(filePath);

        //            //extract file
        //            CommonHelper.ExtractZipFile(filePath, serverZipExtractPath);

        //            //delete zip file after extract
        //            if (System.IO.File.Exists(filePath))
        //                System.IO.File.Delete(filePath);
        //            isFolderEmpty = CommonHelper.IsDirectoryEmpty(serverZipExtractPath);

        //            if (!isFolderEmpty)
        //            {
        //                string[] extensions = new[] { ".jpg", ".png", ".jpeg" };
        //                DirectoryInfo dInfo = new DirectoryInfo(serverZipExtractPath);
        //                //Getting Text files
        //                listFiles = dInfo.EnumerateFiles()
        //                         .Where(f => extensions.Contains(f.Extension.ToLower()))
        //                         .ToArray();
        //            }
        //        }

        //        DishImportResultModels importModel = new DishImportResultModels();
        //        string msg = "";
        //        // read excel file
        //        if (model.ExcelUpload != null && model.ExcelUpload.ContentLength > 0)
        //        {
        //            string fileName = Path.GetFileName(model.ExcelUpload.FileName);
        //            string filePath = string.Format("{0}/{1}", System.Web.HttpContext.Current.Server.MapPath("~/Uploads"), fileName);

        //            //upload file to server
        //            if (System.IO.File.Exists(filePath))
        //                System.IO.File.Delete(filePath);
        //            model.ExcelUpload.SaveAs(filePath);

        //            int totalRowExcel;
        //            importModel.ListImport = _factory.ImportDish(filePath, listFiles, out totalRowExcel, model.ListStores, ref msg);
        //            importModel.TotalRowExcel = totalRowExcel;

        //            //delete folder extract after get file.
        //            if (System.IO.Directory.Exists(serverZipExtractPath))
        //                System.IO.Directory.Delete(serverZipExtractPath, true);
        //            //delete file excel after insert to database
        //            if (System.IO.File.Exists(filePath))
        //                System.IO.File.Delete(filePath);
        //        }

        //        if (msg.Equals(""))
        //        {
        //            return View("ImportDetail", importModel);
        //        }
        //        else
        //        {
        //            //NSLog.Logger.Error("ProductImport_Fail: ", msg);
        //            ModelState.AddModelError("ImageZipUpload", msg);
        //            return View(model);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        NSLog.Logger.Error("ProductImport_Fail: ", e);
        //        ModelState.AddModelError("ImageZipUpload", "Import file have error");
        //        return View(model);
        //    }
        //}

        //public ActionResult Export()
        //{
        //    ProductModels model = new ProductModels();
        //    return View(model);
        //}

        //[HttpPost]
        //public ActionResult Export(ProductModels model)
        //{
        //    try
        //    {
        //        if (model.ListStores == null)
        //        {
        //            ModelState.AddModelError("ListStores", "Please choose store.");
        //            return View(model);
        //        }
        //        XLWorkbook wb = new XLWorkbook();
        //        var wsSetMenu = wb.Worksheets.Add("Dishes");
        //        var wsTab = wb.Worksheets.Add("Tabs");
        //        var wsDish = wb.Worksheets.Add("Modifiers");
        //        StatusResponse response = _factory.ExportDish(ref wsSetMenu, ref wsTab, ref wsDish, model.ListStores);
        //        if (!response.Status)
        //        {
        //            ModelState.AddModelError("", response.MsgError);
        //            return View(model);
        //        }
        //        ViewBag.wb = wb;
        //        Response.Clear();
        //        Response.ClearContent();
        //        Response.ClearHeaders();
        //        Response.Charset = UTF8Encoding.UTF8.WebName;
        //        Response.ContentEncoding = UTF8Encoding.UTF8;
        //        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //        Response.AddHeader("content-disposition", String.Format(@"attachment;filename={0}.xlsx", CommonHelper.GetExportFileName("Package").Replace(" ", "_")));

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
        //        NSLog.Logger.Error("ProductExport_Fail: ", e);
        //        return new HttpStatusCodeResult(400, e.Message);
        //    }
        //}
    }
}