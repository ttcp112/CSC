using NSCSC.Shared;
using NSCSC.Shared.Factory.SandBox.Inventory;
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
using System.Web.Mvc;

namespace NSCSC.Web.Controllers
{
    [NuAuth]
    public class SBInventoryPackageController : HQController
    {
        // GET: SBInventoryPackage
        private ProductFactory _factory = null;

        public SBInventoryPackageController()
        {
            _factory = new ProductFactory();
        }

        // GET: SBInventoryProduct
        public ActionResult Index()
        {
            try
            {
                ProductViewModels model = new ProductViewModels();
                return View(model);
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("ProductPackageIndex: ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }

        public ActionResult Search(ProductViewModels model)
        {
            try
            {
                var listData = _factory.GetListData(model.SearchString, (byte)Commons.EProductType.Package);
                listData.ForEach(x =>
                {
                    //x.CategoryName = x.ListProductName == null ? "" : string.Join(", ", x.ListProductName);
                    string sItem = "";
                    int countItem = x.ListProductName.ToList().Count;
                    for (int s = 1; s <= countItem; s++)
                    {
                        sItem += x.ListProductName[s - 1] + (s == countItem ? "" : ", ");
                        if (s % 8 == 0)
                            sItem += "<br/>";
                    }
                    x.CategoryName = sItem;

                });
                model.ListItem = listData;
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("ProductPackageSearch: ", e);
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
                if (model.SaleFrom > model.SaleTo)
                    ModelState.AddModelError("DateSaleValidate", "From Date must be less than To Date.");
                if (model.PictureUpload != null && !Commons.ImageExtendFiler.Contains(model.PictureUpload.ContentType))
                {
                    ModelState.AddModelError("PictureUpload", Commons.ErrorMsgFilterImage);
                }
                //==========Composite
                model.ListComposite = model.ListComposite.Where(x => x.Status != (byte)Commons.EStatus.Deleted).ToList();
                model.ListComposite.ForEach(x =>
                {
                    if (x.IsUnlimited)
                        x.Quantity = -1;
                });
                //==========Price
                model.ListPrice = model.ListPrice.Where(x => x.Status != (byte)Commons.EStatus.Deleted).ToList();
                model.ListPriceExtend = model.ListPriceExtend.Where(x => x.Status != (byte)Commons.EStatus.Deleted).ToList();
                model.ListPrice.AddRange(model.ListPriceExtend);

                if (!ModelState.IsValid)
                {
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
                //==============
                model.CreateUser = CurrentUser.UserId;
                model.ProductType = (byte)Commons.EProductType.Package;
                string msg = "";
                var tmp = model.PictureByte;
                model.PictureByte = null;
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
                    //return RedirectToAction("Create");
                    //ModelState.AddModelError("Name", Commons.ErrorMsg /*msg*/);
                    ModelState.AddModelError("Error", msg);
                    model.ListPrice = model.ListPrice.Where(w => w.IsExtend == false).ToList();
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("ProductPackageCreate: ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }

        public ProductDetailModels GetDetail(string id)
        {
            try
            {
                ProductDetailModels model = _factory.GetDetail(id);
                //==========Price
                List<ProductPriceModels> listPrice = model.ListPrice;
                listPrice.ForEach(x =>
                {
                    if (x.Status == 0)
                        x.Status = (byte)Commons.EStatus.Actived;
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

                //======ListPrice
                int OffSet = 0;
                model.ListPrice = listPrice.Where(x => !x.IsExtend).OrderBy(x => x.Period).ToList();
                model.ListPrice.ForEach(x =>
                {
                    x.OffSet = OffSet++;
                });
                //=====ListPriceExtend
                OffSet = 0;
                model.ListPriceExtend = listPrice.Where(x => x.IsExtend).OrderBy(x => x.Period).ToList();
                model.ListPriceExtend.ForEach(x =>
                {
                    x.OffSet = OffSet++;
                });
                //==================Composite
                OffSet = 0;
                List<ProductCompositeModels> ListComposite = model.ListComposite;
                ListComposite.ForEach(x =>
                {
                    x.OffSet = OffSet++;

                    if (x.Status == 0)
                        x.Status = (byte)Commons.EStatus.Actived;

                    //if (x.ProductType == (byte)Commons.EProductType.Addition)
                    //{
                    //    if (x.AdditionType == (byte)Commons.EAdditionType.Hardware)
                    //        x.CategoryName = Commons.EAdditionType.Hardware.ToString();
                    //    else if (x.AdditionType == (byte)Commons.EAdditionType.Service)
                    //        x.CategoryName = Commons.EAdditionType.Service.ToString();
                    //}
                    if (x.ProductType == (byte)Commons.EProductType.Product)
                    {
                        x.CategoryName = x.ListCategory.Select(s => s.CategoryName).FirstOrDefault().ToString();
                        int categoryType = x.ListCategory.Select(s => s.Type).FirstOrDefault();
                        x.TypeName = ((Commons.EType)Enum.ToObject(typeof(Commons.EType), categoryType)).ToString().Replace("_", " ");
                    } else if (x.ProductType == (byte)Commons.EProductType.Addition)
                    {
                        x.TypeName = ((Commons.EAdditionType)Enum.ToObject(typeof(Commons.EAdditionType), x.AdditionType)).ToString();
                    }

                    if (x.Quantity == -1)
                        x.IsUnlimited = true;
                });
                model.ListComposite = ListComposite.OrderBy(x => x.ProductName).ToList();

                return model;
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("ProductPackageDetail: ", ex);
                return null;
            }
        }

        [HttpGet]
        public new PartialViewResult View(string id)
        {
            ProductDetailModels model = GetDetail(id);
            return PartialView("_View", model);
        }

        [HttpGet]
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
                if (model.SaleFrom > model.SaleTo)
                    ModelState.AddModelError("DateSaleValidate", "From Date must be less than To Date.");
                if (model.PictureUpload != null && !Commons.ImageExtendFiler.Contains(model.PictureUpload.ContentType))
                {
                    ModelState.AddModelError("PictureUpload", Commons.ErrorMsgFilterImage);
                }
                //==========Composite
                model.ListComposite = model.ListComposite.Where(x => x.Status != (byte)Commons.EStatus.Deleted).ToList();
                model.ListComposite.ForEach(x =>
                {
                    if (x.IsUnlimited)
                        x.Quantity = -1;
                });
                //==========Price
                model.ListPrice = model.ListPrice.Where(x => x.Status != (byte)Commons.EStatus.Deleted).ToList();
                model.ListPriceExtend = model.ListPriceExtend.Where(x => x.Status != (byte)Commons.EStatus.Deleted).ToList();
                model.ListPrice.AddRange(model.ListPriceExtend);
                if (!ModelState.IsValid)
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return PartialView("_Edit", model);
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
                else
                {
                    if (!string.IsNullOrEmpty(model.ImageURL))
                        model.ImageURL = model.ImageURL.Replace(Commons.PublicImages, "").Replace(Commons.Image100_50, "");
                }

                model.CreateUser = CurrentUser.UserId;
                model.ProductType = (byte)Commons.EProductType.Package;
                //====================
                string msg = "";
                var tmp = model.PictureByte;
                model.PictureByte = null;
                var result = _factory.CreateOrEdit(model, ref msg);
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
                    //ModelState.AddModelError("Name", Commons.ErrorMsg  /*msg*/);
                    ModelState.AddModelError("Error", msg);
                    model.ListPrice = model.ListPrice.Where(w => w.IsExtend == false).ToList();
                    return PartialView("_Edit", model);
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("ProductPackageEdit: ", ex);
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
                NSLog.Logger.Error("PackageDelete: ", ex);
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
        //public ActionResult Delete(ProductModels model)
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
        //        NSLog.Logger.Error("ProductPackageDelete: ", ex);
        //        ModelState.AddModelError("Name", "Have an error when you delete a package");
        //        Response.StatusCode = (int)HttpStatusCode.BadRequest;
        //        return PartialView("_Delete", model);
        //    }
        //}

        public ActionResult LoadItems(byte Eproducttype, byte EAdditionType)
        {
            ProductDetailModels model = new ProductDetailModels();
            var Data = _factory.GetListData(null, Eproducttype);
            if (Eproducttype == (byte)Commons.EProductType.Addition)
            {
                Data = Data.Where(x => x.AdditionType == (byte)Commons.EAdditionType.Hardware || x.AdditionType == (byte)Commons.EAdditionType.Service).ToList();
                //Data.ForEach(x =>
                //{
                //    x.CategoryName = x.AdditionType == (byte)Commons.EAdditionType.Hardware ? Commons.EAdditionType.Hardware.ToString() : Commons.EAdditionType.Service.ToString();
                //});
            }
            int OffSet = 0;
            foreach (var item in Data)
            {
                string typeName = "";
                if (item.ProductType == (byte)Commons.EProductType.Product)
                {
                    typeName = ((Commons.EType)Enum.ToObject(typeof(Commons.EType), item.Type)).ToString().Replace("_", " ");
                } else if (item.ProductType == (byte)Commons.EProductType.Addition)
                {
                    typeName = ((Commons.EAdditionType)Enum.ToObject(typeof(Commons.EAdditionType), item.AdditionType)).ToString();
                }
                model.ClientListProduct.Add(new ProductCompositeModels
                {
                    CategoryName = item.CategoryName,
                    TypeName = typeName,
                    ProductName = item.Name,
                    ProductID = item.ID,
                    AdditionType = item.AdditionType,
                    ProductType = Eproducttype,
                    Sequence = 0,
                    Quantity = 1,
                    Status = item.IsActive ? 1 : 0,
                    IsSelect = false,
                    IsUnlimited = false,
                    OffSet = OffSet++,
                });
            }
            if (model.ClientListProduct != null)
                model.ClientListProduct = model.ClientListProduct.OrderBy(x => x.ProductName).ToList();
            return PartialView("_ListProduct", model);
        }

        public ActionResult AddItems(POSTItem data)
        {
            ProductDetailModels model = new ProductDetailModels();
            int OffSet = 0;
            if (data.ListItem != null && data.ListItem.Count > 0)
            {
                List<ProductCompositeModels> listData = new List<ProductCompositeModels>();
                foreach (var item in data.ListItem)
                {
                    listData.Add(new ProductCompositeModels
                    {
                        ProductID = item.ProductID,
                        CategoryName = item.CategoryName,
                        TypeName = item.TypeName,
                        ProductName = item.ProductName,
                        ProductType = item.ProductType,
                        Quantity = item.IsUnlimited ? -1 : item.Quantity,
                        Sequence = item.Sequence,
                        AdditionType = item.AdditionType,
                        IsSelect = item.IsSelect,
                        IsUnlimited = item.IsUnlimited,
                        OffSet = OffSet++,
                        //IsSelect = item.Status.Equals("active") ? true : false,
                        Status = (byte)Commons.EStatus.Actived,
                    });
                }
                if (listData != null)
                    listData = listData.OrderBy(x => x.ProductName).ToList();
                model.ListComposite = listData;
            }
            return PartialView("_ListItem", model);
        }

        /*====*/
        public ActionResult AddItemsPrice(ProductPriceModels data)
        {
            ProductPriceModels model = new ProductPriceModels();
            model.OffSet = data.currentItemOffset;
            model.Period = data.Period;
            model.Price = data.Price;
            model.IsActive = data.IsActive;
            model.NamePeriodType = data.NamePeriodType + (data.Period > 1 ? "s" : "");
            model.PeriodType = data.PeriodType;
            model.IsExtend = data.IsExtend;
            data.currentItemOffset++;
            return PartialView(data.IsExtend ? "_ItemPriceExtend" : "_ItemPrice", model);
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