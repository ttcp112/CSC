using Newtonsoft.Json;
using NSCSC.Shared;
using NSCSC.Shared.Factory;
using NSCSC.Shared.Factory.ClientSite;
using NSCSC.Shared.Factory.ClientSite.OurProducts;
using NSCSC.Shared.Factory.CRM;
using NSCSC.Shared.Filters;
using NSCSC.Shared.Models.ClientSite.MyProfile;
using NSCSC.Shared.Models.ClientSite.MyStoreAndBusiness;
using NSCSC.Shared.Models.ClientSite.OurProDucts;
using NSCSC.Shared.Models.OurProducts.Package;
using NSCSC.Shared.Models.OurProducts.Product;
using NSCSC.Shared.Utilities;
using NSCSC.Web.Clients.App_Start;
using NuWebNCloud.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace NSCSC.Web.Clients.Controllers
{
    [NuAuth]
    public class MyStoreAndBusinessController : ClientController
    {
        private MyStoreAndBusinessFactory _factory = null;
        private YourCartFactory _facCart = null;
        private BaseFactory _baseFactory = null;
        private MyProfileFactory _myProfileFactory = null;

        public MyStoreAndBusinessController()
        {
            _factory = new MyStoreAndBusinessFactory();
            _facCart = new YourCartFactory();
            _baseFactory = new BaseFactory();
            _myProfileFactory = new MyProfileFactory();
        }

        public void GetListCountry()
        {
            var data = _myProfileFactory.GetListCountry();
            ViewBag.ListCountry = data;
        }

        public ActionResult ListMerchant()
        {
            if (!CurrentUser.IsReseller)
                return RedirectToAction("Index");

            MyStoreAndBusinessMerchantViewModels model = new MyStoreAndBusinessMerchantViewModels();
            model.ListMerchant = _factory.GetListMerchant(CurrentUser.UserId);
            model.ListMerchant.ForEach(x =>
            {
                x.MaskPhone = CommonHelper.GetMaskString(x.Phone, "left", 4);
                x.MaskEmail = CommonHelper.GetMaskString(x.Email, "right", 4);
            });
            return View("_ListMerchant", model);
        }

        [EncryptedActionParameter]
        public ActionResult Index(string id)
        {
            if (CurrentUser.IsReseller && string.IsNullOrEmpty(id))
            {
                return RedirectToAction("ListMerchant");
            }

            MyStoreAndBusinessViewModels model = new MyStoreAndBusinessViewModels();
            model.IsReseller = CurrentUser.IsReseller;
            //--------------------------
            model.MerchantInfo = _factory.GetMerchantInfo(string.IsNullOrEmpty(id) ? CurrentUser.UserId : id);
            if (model.MerchantInfo != null)
            {
                model.MerchantInfo.CusMerchantID = id;
                model.MerchantInfo.ListCounTry = _factory.GetListCountry();
                model.MerchantInfo.ImageURL = string.IsNullOrEmpty(model.MerchantInfo.ImageURL) ? _ImgDummyStore : model.MerchantInfo.ImageURL;
                //=================
                List<StoreModels> GetListStore = _factory.GetListStore(CurrentUser.UserId);
                model.MerchantInfo.ListStore = GetListStore;//.Where(x => !x.IsTemp).ToList();
                if (model.MerchantInfo.ListStore != null && model.MerchantInfo.ListStore.Count > 0)
                {
                    model.MerchantInfo.ListStore.ForEach(x =>
                    {
                        if (x.ExpiredDate.HasValue)
                            x.ExpiredDate = CommonHelper.ConvertToLocalTime(x.ExpiredDate.Value);
                        if (x.ExpiredDate == null)
                            x.sExpiredDate = "Not yet activated";
                        else
                            x.sExpiredDate = x.ExpiredDate.Value.ToString(Commons.FormatDateTime);
                        //=================
                        if (x.IsActive)
                            x.sStatus = "Active";
                        else if (!x.IsActive)
                            x.sStatus = "Inactive";
                    });
                }
            }
            //--------------------------
            UserGetListProductManagementModels dataHardwareServiceAccountAddition = _factory.GetListHardwareServiceAccountAddition(CurrentUser.UserId);
            //---Hardware
            model.ListHardware = dataHardwareServiceAccountAddition.ListHardware;
            if (model.ListHardware != null && model.ListHardware.Count > 0)
            {
                model.ListHardware.ForEach(x =>
                {
                    x.Status = x.State == (byte)Commons.EServiceAssetStatus.New ? Commons.EServiceAssetStatus.New.ToString() :
                                x.State == (byte)Commons.EServiceAssetStatus.Done ? Commons.EServiceAssetStatus.Done.ToString() :
                                 Commons.EServiceAssetStatus.Recalled.ToString();

                    if (x.BoughtTime.HasValue)
                        x.sBoughtTime = CommonHelper.ConvertToLocalTime(x.BoughtTime.Value).ToString(Commons.FormatDateTime);
                });
            }
            //---Service
            model.ListService = dataHardwareServiceAccountAddition.ListService;
            if (model.ListService != null && model.ListService.Count > 0)
            {
                model.ListService.ForEach(x =>
                {
                    x.Status = x.State == (byte)Commons.EServiceAssetStatus.New ? Commons.EServiceAssetStatus.New.ToString() :
                                x.State == (byte)Commons.EServiceAssetStatus.Done ? Commons.EServiceAssetStatus.Done.ToString() :
                                 Commons.EServiceAssetStatus.Recalled.ToString();

                    if (x.BoughtTime.HasValue)
                        x.sBoughtTime = CommonHelper.ConvertToLocalTime(x.BoughtTime.Value).ToString(Commons.FormatDateTime);
                });
            }

            //test
            GetListCountry();

            return View(model);
        }

        #region /*For Merchant*/
        [HttpPost]
        public ActionResult UpdateMerchant(MyStoreAndBusinessViewModels model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    model.MerchantInfo.ListCounTry = _factory.GetListCountry();
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return PartialView(/*"_FormMerchant"*/"_MerchantDetail", model);
                }
                //====================
                byte[] photoByte = null;
                if (model.MerchantInfo.PictureUpload != null && model.MerchantInfo.PictureUpload.ContentLength > 0)
                {
                    Byte[] imgByte = new Byte[model.MerchantInfo.PictureUpload.ContentLength];
                    model.MerchantInfo.PictureUpload.InputStream.Read(imgByte, 0, model.MerchantInfo.PictureUpload.ContentLength);
                    model.MerchantInfo.PictureByte = imgByte;
                    model.MerchantInfo.ImageURL = Guid.NewGuid() + Path.GetExtension(model.MerchantInfo.PictureUpload.FileName);
                    model.MerchantInfo.PictureUpload = null;
                    model.MerchantInfo.PictureByte = null;
                    photoByte = imgByte;
                }
                else
                {
                    if (!string.IsNullOrEmpty(model.MerchantInfo.ImageURL))
                        model.MerchantInfo.ImageURL = model.MerchantInfo.ImageURL.Replace(Commons.PublicImagesClient, "").Replace(_ImgDummyStore, "");
                }

                string msg = "";
                string _CustomerID = !CurrentUser.IsReseller ? CurrentUser.UserId : model.MerchantInfo.CusMerchantID;
                model.MerchantInfo.CustomerID = _CustomerID;
                var result = _factory.UpdateMerchantInfo(model.MerchantInfo, _CustomerID, ref msg);
                if (result)
                {
                    model.MerchantInfo.PictureByte = photoByte;
                    if (!string.IsNullOrEmpty(model.MerchantInfo.ImageURL) && model.MerchantInfo.PictureByte != null)
                    {
                        var originalDirectory = new DirectoryInfo(string.Format("{0}Uploads\\", Server.MapPath(@"\")));
                        var path = string.Format("{0}{1}", originalDirectory, model.MerchantInfo.ImageURL);
                        MemoryStream ms = new MemoryStream(photoByte, 0, photoByte.Length);
                        ms.Write(photoByte, 0, photoByte.Length);
                        System.Drawing.Image imageTmp = System.Drawing.Image.FromStream(ms, true);
                        ImageHelper.Me.SaveCroppedImage(imageTmp, path, model.MerchantInfo.ImageURL, ref photoByte);
                        model.MerchantInfo.PictureByte = photoByte;
                        FTP.UploadClient(model.MerchantInfo.ImageURL, model.MerchantInfo.PictureByte);
                        ImageHelper.Me.TryDeleteImageUpdated(path);
                    }
                    return new HttpStatusCodeResult(HttpStatusCode.OK);
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    model.MerchantInfo.ListCounTry = _factory.GetListCountry();
                    ModelState.AddModelError("MerchantInfo.Name", msg);
                    return PartialView(/*"_FormMerchant"*/"_MerchantDetail", model);
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("UpdateMerchant: ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }
        #endregion /*End For Merchant*/

        #region /*For Store*/

        #region =======> Update Code [Reseller]
        public ActionResult CallPopupStoreInfo(string StoreID, List<int> ListIndustry)
        {
            StoreDetailModels model = new StoreDetailModels();
            model = _factory.GetStoreInfo(StoreID);
            var listAppliedProduct = _factory.GetListProductForStore(CurrentUser.UserId, StoreID);
            if (listAppliedProduct != null && listAppliedProduct.Count > 0)
            {
                // For create store filter to industry
                if (string.IsNullOrEmpty(StoreID) && ListIndustry != null && ListIndustry.Any())
                {
                    listAppliedProduct = listAppliedProduct.Where(w => ListIndustry.Contains(w.Type)).ToList();
                }
                //------------------------------------

                listAppliedProduct = listAppliedProduct.Where(ww => ww.ProductType != (int)Commons.EProductType.Addition).ToList();
                int OffSet = 0;
                if (string.IsNullOrEmpty(StoreID) && ListIndustry != null && ListIndustry.Any())
                {
                    foreach (var item in listAppliedProduct)
                    {
                        if (item.ProductType == (int)Commons.EProductType.Product)
                        {
                            if ((item.Type == (int)Commons.EType.POinS_Link_App)
                                || (item.Type == (int)Commons.EType.NuPOS
                                        || item.Type == (int)Commons.EType.NuKiosk || item.Type == (int)Commons.EType.NuDisplay)
                                 || (item.Type == (int)Commons.EType.POZZ
                                        || item.Type == (int)Commons.EType.POZZ_Display || item.Type == (int)Commons.EType.POZZ_Kiosk))
                            {
                                model.ListProductApply.Add(new ProductApply()
                                {
                                    AssetID = item.AssetID,
                                    ProductName = item.ProductName,
                                    IsApply = item.IsAppliedStore,
                                    ExpiredTime = item.ExpiredTime,
                                    ExpiredTimeDisplay = ((item.ExpiredTime.HasValue && item.ExpiredTime.Value.Date == Commons.MaxDate.Date) || !item.ExpiredTime.HasValue) ? Commons.NoExpiryDate : item.ExpiredTime.Value.ToString(Commons.FormatDateTime),
                                    ActiveTime = item.ActiveTime,
                                    OffSet = OffSet++
                                });
                            }
                        }
                        else if (item.ProductType == (int)Commons.EProductType.Package)
                        {
                            if (item.ListProduct != null && item.ListProduct.Count > 0)
                            {
                                foreach (var _item in item.ListProduct)
                                {
                                    if ((_item.Type == (int)Commons.EType.POinS_Link_App)
                                        || (_item.Type == (int)Commons.EType.NuPOS
                                                || _item.Type == (int)Commons.EType.NuKiosk || _item.Type == (int)Commons.EType.NuDisplay)
                                         || (_item.Type == (int)Commons.EType.POZZ
                                                || _item.Type == (int)Commons.EType.POZZ_Display || _item.Type == (int)Commons.EType.POZZ_Kiosk))
                                    {
                                        model.ListProductApply.Add(new ProductApply()
                                        {
                                            AssetID = _item.AssetID,
                                            ProductName = _item.ProductName + " (in " + item.ProductName + ")",
                                            IsApply = _item.IsAppliedStore,
                                            ExpiredTime = _item.ExpiredTime,
                                            ExpiredTimeDisplay = ((item.ExpiredTime.HasValue && item.ExpiredTime.Value.Date == Commons.MaxDate.Date) || !item.ExpiredTime.HasValue) ? Commons.NoExpiryDate : item.ExpiredTime.Value.ToString(Commons.FormatDateTime),
                                            ActiveTime = item.ActiveTime,
                                            OffSet = OffSet++
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    foreach (var item in listAppliedProduct)
                    {
                        if (item.ProductType == (int)Commons.EProductType.Product)
                        {
                            if ((item.Type == (int)Commons.EType.POinS_Link_App)
                                || (item.Type == (int)Commons.EType.NuPOS
                                        || item.Type == (int)Commons.EType.NuKiosk || item.Type == (int)Commons.EType.NuDisplay)
                                 || (item.Type == (int)Commons.EType.POZZ
                                        || item.Type == (int)Commons.EType.POZZ_Display || item.Type == (int)Commons.EType.POZZ_Kiosk))
                            {
                                model.ListProductApply.Add(new ProductApply()
                                {
                                    AssetID = item.AssetID,
                                    ProductName = item.ProductName,
                                    IsApply = item.IsAppliedStore,
                                    ExpiredTime = item.ExpiredTime,
                                    ExpiredTimeDisplay = ((item.ExpiredTime.HasValue && item.ExpiredTime.Value.Date == Commons.MaxDate.Date) || !item.ExpiredTime.HasValue) ? Commons.NoExpiryDate : item.ExpiredTime.Value.ToString(Commons.FormatDateTime),
                                    ActiveTime = item.ActiveTime,
                                    OffSet = OffSet++
                                });
                            }
                        }
                        else if (item.ProductType == (int)Commons.EProductType.Package)
                        {
                            if (item.ListProduct != null && item.ListProduct.Count > 0)
                            {
                                foreach (var _item in item.ListProduct)
                                {
                                    if ((_item.Type == (int)Commons.EType.POinS_Link_App)
                                        || (_item.Type == (int)Commons.EType.NuPOS
                                                || _item.Type == (int)Commons.EType.NuKiosk || _item.Type == (int)Commons.EType.NuDisplay)
                                         || (_item.Type == (int)Commons.EType.POZZ
                                                || _item.Type == (int)Commons.EType.POZZ_Display || _item.Type == (int)Commons.EType.POZZ_Kiosk))
                                    {
                                        model.ListProductApply.Add(new ProductApply()
                                        {
                                            AssetID = _item.AssetID,
                                            ProductName = _item.ProductName + " (in " + item.ProductName + ")",
                                            IsApply = _item.IsAppliedStore,
                                            ExpiredTime = _item.ExpiredTime,
                                            ExpiredTimeDisplay = ((item.ExpiredTime.HasValue && item.ExpiredTime.Value.Date == Commons.MaxDate.Date) || !item.ExpiredTime.HasValue) ? Commons.NoExpiryDate : item.ExpiredTime.Value.ToString(Commons.FormatDateTime),
                                            ActiveTime = item.ActiveTime,
                                            OffSet = OffSet++
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (string.IsNullOrEmpty(StoreID))
            {
                model.ListProductApply.ForEach(o =>
                {
                    o.IsApply = true;
                });
            }
            model.ApplyProductCount = model.ListProductApply.Count(x => x.IsApply) + " Product(s)";
            //---------------
            if (!string.IsNullOrEmpty(StoreID))
                model._action = "update";
            model.ImageURL = string.IsNullOrEmpty(model.ImageURL) ? _ImgDummyStore : model.ImageURL;
            model.Industry = model.StoreType.ToString();
            //-------
            var listCountry = _baseFactory.GetListCountry();
            ViewBag.ListCounTry = listCountry;
            var Country = listCountry.Where(x => x.Name == model.Country).FirstOrDefault();
            ViewBag.TimeZones = "";
            if (Country != null)
                ViewBag.TimeZones = Country.TimeZones;
            //------
            return PartialView("_PopupStore", model);
        }

        public ActionResult LoadTimeZone(string countryName)
        {
            var listCountry = _baseFactory.GetListCountry();
            var Country = listCountry.Where(x => x.Name == countryName).FirstOrDefault();
            List<string> lstData = new List<string>();
            if (Country != null)
                lstData = Country.TimeZones;
            return Json(lstData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CallPopupAppliedProducts(string StoreID)
        {
            StoreDetailModels model = new StoreDetailModels();
            model = _factory.GetStoreInfo(StoreID);
            var listAppliedProduct = _factory.GetListProductForStore(CurrentUser.UserId, StoreID);
            if (listAppliedProduct != null && listAppliedProduct.Count > 0)
            {
                listAppliedProduct = listAppliedProduct.Where(x => x.ProductType != (int)Commons.EProductType.Addition).ToList();
                int OffSet = 0;
                foreach (var item in listAppliedProduct)
                {
                    if (item.ProductType == (int)Commons.EProductType.Product)
                    {
                        if ((item.Type == (int)Commons.EType.POinS_Link_App)
                            || (item.Type == (int)Commons.EType.NuPOS
                                    || item.Type == (int)Commons.EType.NuKiosk || item.Type == (int)Commons.EType.NuDisplay)
                             || (item.Type == (int)Commons.EType.POZZ
                                    || item.Type == (int)Commons.EType.POZZ_Display || item.Type == (int)Commons.EType.POZZ_Kiosk))
                        {
                            model.ListProductApply.Add(new ProductApply()
                            {
                                AssetID = item.AssetID,
                                ProductName = item.ProductName,
                                IsApply = item.IsAppliedStore,
                                ExpiredTime = item.ExpiredTime,
                                sExpiredTime = ((item.ExpiredTime.HasValue && item.ExpiredTime.Value.Date == Commons.MaxDate.Date)
                                                    || !item.ExpiredTime.HasValue) ? Commons.NoExpiryDate
                                                    : item.ExpiredTime.Value.ToString(Commons.FormatDateTime),
                                ActiveTime = item.ActiveTime,
                                OffSet = OffSet++
                            });
                        }
                    }
                    else if (item.ProductType == (int)Commons.EProductType.Package)
                    {
                        if (item.ListProduct != null && item.ListProduct.Count > 0)
                        {
                            foreach (var _item in item.ListProduct)
                            {
                                if ((_item.Type == (int)Commons.EType.POinS_Link_App)
                                    || (_item.Type == (int)Commons.EType.NuPOS
                                            || _item.Type == (int)Commons.EType.NuKiosk || _item.Type == (int)Commons.EType.NuDisplay)
                                     || (_item.Type == (int)Commons.EType.POZZ
                                            || _item.Type == (int)Commons.EType.POZZ_Display || _item.Type == (int)Commons.EType.POZZ_Kiosk))
                                {
                                    model.ListProductApply.Add(new ProductApply()
                                    {
                                        AssetID = _item.AssetID,
                                        ProductName = _item.ProductName + " (in " + item.ProductName + ")",
                                        IsApply = _item.IsAppliedStore,
                                        ExpiredTime = _item.ExpiredTime,
                                        sExpiredTime = ((item.ExpiredTime.HasValue && item.ExpiredTime.Value.Date == Commons.MaxDate.Date)
                                                        || !item.ExpiredTime.HasValue) ? Commons.NoExpiryDate
                                                        : item.ExpiredTime.Value.ToString(Commons.FormatDateTime),
                                        ActiveTime = item.ActiveTime,
                                        OffSet = OffSet++
                                    });
                                }
                            }
                        }
                    }

                }
                //-----------
                model.ListProductApply.ForEach(x =>
                {
                    x.sExpiredTime = x.ExpiredTime.HasValue ? CommonHelper.ConvertToLocalTime(x.ExpiredTime.Value).ToString(Commons.FormatDateTime) : "";
                    x.sActiveTime = x.ActiveTime.HasValue ? CommonHelper.ConvertToLocalTime(x.ActiveTime.Value).ToString(Commons.FormatDateTime) : "";
                });
            }
            if (string.IsNullOrEmpty(StoreID))
            {
                model.ListProductApply.ForEach(o =>
                {
                    o.IsApply = true;
                });
            }
            model.ApplyProductCount = model.ListProductApply.Count(x => x.IsApply) + " Product(s)";
            //---------------
            model.ImageURL = string.IsNullOrEmpty(model.ImageURL) ? _ImgDummyStore : model.ImageURL;
            return PartialView("_PopupAppliedProducts", model);
        }

        public ActionResult CallPopupExtend(string Id = "", string AssetID = "")
        {
            var model = new ProductPackageUserModels();
            try
            {
                var temp = _factory.GetProductPackageDetail(AssetID);
                var ListPrice = _factory.GetPriceProduct(Id);
                var ArrPeriod = new Dictionary<int, string>();
                ArrPeriod.Add((int)Commons.EPeriodType.Day, Commons.EPeriodType.Day.ToString());
                ArrPeriod.Add((int)Commons.EPeriodType.Month, Commons.EPeriodType.Month.ToString());
                ArrPeriod.Add((int)Commons.EPeriodType.Year, Commons.EPeriodType.Year.ToString());
                ArrPeriod.Add((int)Commons.EPeriodType.OneTime, Commons.PeriodTypeOneTime);
                if (ListPrice != null)
                {
                    ListPrice = ListPrice.Where(x => x.IsExtend)
                                            .Select(x => new ProductPriceModels()
                                            {
                                                ID = x.ID,
                                                IsExtend = x.IsExtend,
                                                Period = x.Period,
                                                PeriodType = x.PeriodType,
                                                Price = x.Price,
                                                PeriodName = ArrPeriod.ContainsKey(x.PeriodType) ? ArrPeriod.FirstOrDefault(y => y.Key == x.PeriodType).Value : "",
                                                CurrencySymbol = CurrencySymbol,
                                                sPeriodName = CommonHelper.PeriodOfTheProduct(x.Period, x.PeriodType)
                                            })
                                            .OrderBy(x => x.PeriodType).ThenBy(x => x.Period)
                                            .ToList();
                    if (ListPrice != null && ListPrice.Any())
                    {
                        ListPrice.First().IsSelected = true;
                    }
                }

                ViewBag.AssetIdExtendAppliedOn = "";
                if (temp != null)
                {
                    if (temp.ProductType == (int)Commons.EProductType.Package)
                    {
                        var _tempProducts = temp.ListProduct.Select(x => new ProductUserModels
                        {
                            AssetID = x.AssetID,
                            ItemID = x.ItemID,
                            ItemName = x.ItemName,
                            ProductType = x.ProductType,
                            Image = x.Image,
                            PeriodType = x.PeriodType,
                            ExpiryDate = x.ExpiryDate,
                            NumberOfUnit = x.NumberOfUnit,
                            NumberOfAccount = x.NumberOfAccount,
                            IsActive = x.IsActive,
                            IsUnlimitedAccount = x.IsUnlimitedAccount,
                            IsUnlimitedUnit = x.IsUnlimitedUnit,
                            ListDevices = temp.ListDevice.Where(y => y.ProductID == x.ItemID).Select(z => new DeviceUserModels
                            {
                                ID = z.ID,
                                UUID = z.UUID,
                                ActiveTime = z.ActiveTime,
                                LicenseKey = z.LicenseKey,
                                ProductType = z.ProductType,
                                IsActive = z.IsActive,
                                ProductID = z.ProductID,
                                AssetID = temp.AssetID
                            }).ToList()
                        }).ToList();
                        var _product = _tempProducts
                                                   .Where(x => x.IsActive)
                                                   .Select(x => new SelectListItem
                                                   {
                                                       Value = x.ItemID + ',' + x.AssetID, // ProductID + AssetID
                                                       Text = x.ItemName
                                                   })
                                                   .ToList();
                        temp.ListItemProduct = _product;
                        temp.ListProduct = _tempProducts;
                        temp.IsProduct = false;
                        // Set default Extend Product
                        ViewBag.AssetIdExtendAppliedOn = temp.ListItemProduct.Select(ss => ss.Value).FirstOrDefault();
                    }
                    else
                    {
                        var _tempDevices = temp.ListDevice.Select(z => new DeviceUserModels
                        {
                            ID = z.ID,
                            UUID = z.UUID,
                            ActiveTime = z.ActiveTime,
                            LicenseKey = z.LicenseKey,
                            ProductType = z.ProductType,
                            IsActive = z.IsActive,
                            ProductID = z.ProductID,
                            AssetID = temp.AssetID
                        }).ToList();
                        var _product = new List<SelectListItem>()
                        {
                            new SelectListItem
                            {
                                Value = temp.ItemID + ',' + temp.AssetID, // ProductID + AssetID
                                Text = temp.ItemName
                            }
                        };
                        temp.ListItemProduct = _product;
                        temp.ListDevice = _tempDevices;
                        temp.IsProduct = true;
                        // Set default Extend Product
                        ViewBag.AssetIdExtendAppliedOn = temp.AssetID;
                    }

                    temp.ListPrices = ListPrice;

                    temp.ProductAppliOnExtendID = temp.ListItemProduct.Any() ? temp.ListItemProduct.FirstOrDefault().Value : "";
                    model = temp;
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("LoadPopupProductExtend|ProductsTab|: ", ex);
            }
            return PartialView("_PopupExtend", model);
        }

        public ActionResult CallPopupBuyAdditions(string AssetID = "")
        {
            PackageDetailViewModels model = new PackageDetailViewModels();
            try
            {
                model.PackageDetail = new OurProDuctsModel();
                OurProductFactory _factory = new OurProductFactory();
                var dataOrder = _factory.GetProductApplyAdditionsStoreBusiness(AssetID, CurrentUser.UserId);
                if (dataOrder != null && dataOrder.Any())
                {
                    var itemDetail = dataOrder.Where(ww => ww.AssetID == AssetID && (ww.ProductType == (byte)Commons.EProductType.Product || ww.ProductType == (byte)Commons.EProductType.Package)).FirstOrDefault();
                    if (itemDetail != null)
                    {
                        model.PackageDetail.ItemID = itemDetail.AssetID;
                        model.PackageDetail.ID = itemDetail.ProductID;
                        model.PackageDetail.Name = itemDetail.ProductName;
                        model.PackageDetail.ProductType = itemDetail.ProductType;
                        // If item is package
                        if (model.PackageDetail.ProductType == (byte)Commons.EProductType.Package)
                        {
                            //=======Get List Product Child
                            if (itemDetail.ListProduct != null && itemDetail.ListProduct.Any())
                            {
                                // Get list cate product of package
                                var lstProducts = itemDetail.ListProduct.Where(ww => ww.ProductType == (byte)Commons.EProductType.Product).ToList();
                                lstProducts.ForEach(xx =>
                                {
                                    model.ListApplyAdditionProduct.Add(new ProductApplyAdditionPackage
                                    {
                                        ProductName = xx.ProductName,
                                        ProductId = xx.AssetID,
                                    });
                                });
                            }
                        }
                        // If item is product
                        else if (model.PackageDetail.ProductType == (byte)Commons.EProductType.Product)
                        {
                            model.ListApplyAdditionProduct.Add(new ProductApplyAdditionPackage
                            {
                                ProductName = model.PackageDetail.Name,
                                ProductId = model.PackageDetail.ItemID,
                            });
                        }
                    }
                }
                //==========Get List Addition Type
                List<SelectListItem> ListAdditionType = new List<SelectListItem>();
                ListAdditionType = GetListAdditionType();
                ViewBag.ListAdditionType = ListAdditionType;
                model.CurrencySymbol = CurrencySymbol;
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("LoadPopupBuyAdditions|ProductsTab|: ", ex);
            }
            return PartialView("_PopupBuyAdditions", model);
        }

        public List<SelectListItem> GetListAdditionType()
        {
            List<SelectListItem> ListData = new List<SelectListItem>()
                {
                    new SelectListItem() { Text = "All", Value = "0"},
                    new SelectListItem() { Text = Commons.EAdditionType.Account.ToString(), Value = Commons.EAdditionType.Account.ToString("d")},
                    new SelectListItem() { Text = Commons.EAdditionType.Function.ToString(), Value = Commons.EAdditionType.Function.ToString("d")},
                    new SelectListItem() { Text = Commons.EAdditionType.Hardware.ToString(), Value = Commons.EAdditionType.Hardware.ToString("d")},
                    new SelectListItem() { Text = Commons.EAdditionType.Location.ToString(), Value = Commons.EAdditionType.Location.ToString("d")},
                    new SelectListItem() { Text = Commons.EAdditionType.Service.ToString(), Value = Commons.EAdditionType.Service.ToString("d")},
                    new SelectListItem() { Text = Commons.EAdditionType.Software.ToString(), Value = Commons.EAdditionType.Software.ToString("d")}
                };
            return ListData;
        }

        public ActionResult GetListAdditionsForProduct(int AdditionType, List<ProductApplyAdditionPackage> ListApplyAdditionProduct)
        {
            try
            {
                PackageDetailViewModels model = new PackageDetailViewModels();
                model.PackageDetail = new OurProDuctsModel();
                model.PackageDetail.ListProducts = new List<OurProDuctsModel>();
                //PackageViewModels model = new PackageViewModels();
                OurProductFactory _factory = new OurProductFactory();
                model.PackageDetail.ListProducts = _factory.GetListData(CurrentUser.UserId, "", (byte)Commons.EProductType.Addition);
                if (AdditionType > 0)
                    model.PackageDetail.ListProducts = model.PackageDetail.ListProducts.Where(x => x.AdditionType == AdditionType).ToList();

                var gData = model.PackageDetail.ListProducts.OrderBy(o => o.AdditionType).GroupBy(x => x.AdditionType).Select(x => new BuyingAdditionViewModels
                {
                    ID = x.Key,
                    Name = ((Commons.EAdditionType)Enum.ToObject(typeof(Commons.EAdditionType), x.Key)).ToString(),
                    ListItem = x.ToList()
                }).ToList();

                int OffSet = 0;
                gData.ForEach(x =>
                {
                    x.ListItem = x.ListItem.OrderBy(o => o.Sequence).ThenBy(oo => oo.Name).ToList();
                    x.ListItem.ForEach(z =>
                    {
                        z.Index = OffSet++;
                        z.AdditionTypeName = x.Name;
                        z.AdditionType = x.ID;
                        z.Quantity = 1;
                        //------
                        if (z.ListPrice != null && z.ListPrice.Count > 0)
                        {
                            z.ListPrice.ForEach(xx =>
                            {
                                xx.NamePeriodType = CommonHelper.PeriodOfTheProduct(xx.Period, xx.PeriodType);
                            });

                            var PriceMin = z.ListPrice.OrderBy(o => o.PeriodType).ThenBy(w => w.Period).ThenBy(w => w.Price).FirstOrDefault();
                            z.Price = PriceMin.Price;
                            z.Period = (int)PriceMin.Period;
                            z.PeriodType = PriceMin.PeriodType;
                            z.sPeriodType = PriceMin.ID;
                            // Period Time != One Time
                            if (z.PeriodType != (byte)Commons.EPeriodType.OneTime)
                                z.PeriodTime = PriceMin.Period + " " + PriceMin.NamePeriodType;
                            else
                                z.PeriodTime = PriceMin.NamePeriodType;
                        }
                        // Get list product apply addition
                        if (ListApplyAdditionProduct != null && ListApplyAdditionProduct.Any())
                        {
                            if (z.AdditionType != (byte)Commons.EAdditionType.Hardware && z.AdditionType != (byte)Commons.EAdditionType.Service && z.AdditionType != (byte)Commons.EAdditionType.Function)
                            {
                                z.ListProductApplicable = new List<SelectListItem>();
                                ListApplyAdditionProduct.ForEach(y =>
                                {
                                    z.ListProductApplicable.Add(new SelectListItem
                                    {
                                        Text = y.ProductName,
                                        Value = y.ProductId
                                    });
                                });
                                z.ListProductApplicable = z.ListProductApplicable.OrderBy(o => o.Text).ToList();
                            }
                        }
                        z.CurrencySymbol = CurrencySymbol;

                        if (z.IsUnlimitedAmountOfUnit)
                            z.AmountOfUnit = -1;
                    });

                    model.ListAdditionBuy.AddRange(x.ListItem);
                });
                gData = gData.OrderBy(x => x.Name).ToList();
                model.PackageDetail.ListBuyAdditionPopup = gData;
                return PartialView("_PopupBuyAdditionsItem", model/*gData*/);
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("LoadPopupBuyAdditionsItem|ProductsTab|: ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }

        public ActionResult LoadPeriod(int AdditionType, string PeriodId, string Id)
        {
            PackageDetailViewModels model = new PackageDetailViewModels();
            model.PackageDetail = new OurProDuctsModel();
            model.PackageDetail.ListProducts = new List<OurProDuctsModel>();
            OurProductFactory _factory = new OurProductFactory();
            model.PackageDetail.ListProducts = _factory.GetListData(CurrentUser.UserId, "", (byte)Commons.EProductType.Addition);
            var obj = model.PackageDetail.ListProducts.Where(x => x.AdditionType == AdditionType && x.ID.Equals(Id)).FirstOrDefault();
            Shared.Models.SandBox.Inventory.Product.ProductPriceModels _price = new Shared.Models.SandBox.Inventory.Product.ProductPriceModels();
            if (obj != null)
            {
                _price = obj.ListPrice.Where(x => x.ID.Equals(PeriodId)).FirstOrDefault();
            }
            return Json(_price, JsonRequestBehavior.AllowGet);
        }

        #endregion 

        public ActionResult GetListStore(string type)
        {
            MyStoreAndBusinessViewModels model = new MyStoreAndBusinessViewModels();
            List<StoreModels> GetListStore = _factory.GetListStore(CurrentUser.UserId);
            model.ListStore = GetListStore.Where(x => !x.IsTemp).ToList();
            model.IsTabStore = type;
            DateTime _currentDate = DateTime.Now;
            if (type.Equals("ongoing"))
            {
                model.ListStore = model.ListStore.Where(x => x.ExpiredDate == null || CommonHelper.ConvertToLocalTime(x.ExpiredDate.Value) >= _currentDate).ToList();
            }
            else if (type.Equals("expired"))
            {
                model.ListStore = model.ListStore.Where(x => x.ExpiredDate.HasValue && CommonHelper.ConvertToLocalTime(x.ExpiredDate.Value) < _currentDate).ToList();
            }

            if (model.ListStore != null && model.ListStore.Count > 0)
            {
                model.ListStore.ForEach(x =>
                {
                    if (x.ExpiredDate.HasValue)
                    {
                        x.ExpiredDate = CommonHelper.ConvertToLocalTime(x.ExpiredDate.Value);
                    }
                    if (x.ExpiredDate == null)
                        x.sExpiredDate = "Not yet activated";
                    else
                        x.sExpiredDate = x.ExpiredDate.Value.ToString(Commons.FormatDateTime);
                    //=================
                    if (x.ExpiredDate == null)
                        x.sStatus = "Not yet activated";
                    else if (x.IsActive)
                        x.sStatus = "Active";
                    else if (!x.IsActive)
                        x.sStatus = "Inactive";

                    x.ImageURL = string.IsNullOrEmpty(x.ImageURL) ? _ImgDummyStore : x.ImageURL;
                });
            }
            return PartialView("_FormStore", model);
        }

        public ActionResult ViewStoreDetail(string StoreID)
        {
            StoreDetailModels model = new StoreDetailModels();
            model = _factory.GetStoreInfo(StoreID);
            var listproductapplied = _factory.GetListProductForStore(CurrentUser.UserId, StoreID);
            if (listproductapplied != null && listproductapplied.Count > 0)
            {
                listproductapplied = listproductapplied.Where(ww => ww.ProductType != (int)Commons.EProductType.Addition).ToList();
                int i = 0;
                foreach (var item in listproductapplied)
                {
                    if (item.ProductType == (int)Commons.EProductType.Product)
                    {
                        if ((item.Type == (int)Commons.EType.POinS_Link_App)
                            || (model.StoreType == (int)Commons.EStoreType.FnB && (item.Type == (int)Commons.EType.NuPOS
                                    || item.Type == (int)Commons.EType.NuKiosk || item.Type == (int)Commons.EType.NuDisplay))
                             || (model.StoreType == (int)Commons.EStoreType.Beauty && (item.Type == (int)Commons.EType.POZZ
                                    || item.Type == (int)Commons.EType.POZZ_Display || item.Type == (int)Commons.EType.POZZ_Kiosk)))
                        {
                            model.ListProductApply.Add(new ProductApply()
                            {
                                AssetID = item.AssetID,
                                ProductName = item.ProductName,
                                IsApply = item.IsAppliedStore,
                                ExpiredTime = item.ExpiredTime,
                                ExpiredTimeDisplay = ((item.ExpiredTime.HasValue && item.ExpiredTime.Value.Date == Commons.MaxDate.Date) || !item.ExpiredTime.HasValue) ? Commons.NoExpiryDate : item.ExpiredTime.Value.ToString(Commons.FormatDateTime),
                                ActiveTime = item.ActiveTime,
                                OffSet = i
                            });
                            i++;
                        }

                    }//end item.ProductType == (int)Commons.EProductType.Product
                    else
                    {
                        if (item.ProductType == (int)Commons.EProductType.Package)
                        {
                            if (item.ListProduct != null && item.ListProduct.Count > 0)
                            {
                                foreach (var _item in item.ListProduct)
                                {
                                    if ((_item.Type == (int)Commons.EType.POinS_Link_App)
                                        || (model.StoreType == (int)Commons.EStoreType.FnB && (_item.Type == (int)Commons.EType.NuPOS
                                                || _item.Type == (int)Commons.EType.NuKiosk || _item.Type == (int)Commons.EType.NuDisplay))
                                         || (model.StoreType == (int)Commons.EStoreType.Beauty && (_item.Type == (int)Commons.EType.POZZ
                                                || _item.Type == (int)Commons.EType.POZZ_Display || _item.Type == (int)Commons.EType.POZZ_Kiosk)))
                                    {
                                        model.ListProductApply.Add(new ProductApply()
                                        {
                                            AssetID = _item.AssetID,
                                            ProductName = _item.ProductName + " (in " + item.ProductName + ")",
                                            IsApply = _item.IsAppliedStore,
                                            ExpiredTime = _item.ExpiredTime,
                                            ExpiredTimeDisplay = ((item.ExpiredTime.HasValue && item.ExpiredTime.Value.Date == Commons.MaxDate.Date) || !item.ExpiredTime.HasValue) ? Commons.NoExpiryDate : item.ExpiredTime.Value.ToString(Commons.FormatDateTime),
                                            ActiveTime = item.ActiveTime,
                                            OffSet = i
                                        });
                                        i++;
                                    }

                                }
                            }
                        }//end (item.ProductType == (int)Commons.EProductType.Package)

                    }

                }
                model.ListProductApply.ForEach(x =>
                {
                    if (x.ExpiredTime.HasValue)
                        x.ExpiredTime = CommonHelper.ConvertToLocalTime(x.ExpiredTime.Value);
                    if (x.ActiveTime.HasValue)
                        x.ActiveTime = CommonHelper.ConvertToLocalTime(x.ActiveTime.Value);
                });
                //model.ListApplyProduct.ForEach(x => x. = CommonHelper.ConvertToLocalTime(x.ExpiredTime.Value));
            }
            model.ImageURL = string.IsNullOrEmpty(model.ImageURL) ? _ImgDummyStore : model.ImageURL;
            model.Industry = model.StoreType.ToString();
            var _listCounTry = _baseFactory.GetListCountry();
            ViewBag.ListCounTry = _listCounTry;
            ViewBag.ListCounTry = ViewBag.ListCounTry ?? "";
            ViewBag.TimeZones = "";
            foreach (var item in _listCounTry)
            {
                if (item.Name == model.Country)
                    ViewBag.TimeZones = item.TimeZones;
            }
            var name = model.ListProductApply.Where(x => x.IsApply).ToList();
            return PartialView("_StoreInfo", model);
        }

        public ActionResult ViewStoreDetail_Old(string StoreID)
        {
            StoreDetailModels model = new StoreDetailModels();
            model = _factory.GetStoreInfo(StoreID);
            var listproductapplied = _factory.GetListProductForStore(CurrentUser.UserId, StoreID);
            if (listproductapplied != null && listproductapplied.Count > 0)
            {
                int i = 0;
                foreach (var item in listproductapplied)
                {
                    if (item.ProductType == (int)Commons.EProductType.Product)
                    {
                        if (item.Type == (int)Commons.EType.POinS_Link_App)
                        {
                            model.ListProductApply.Add(new ProductApply()
                            {
                                AssetID = item.AssetID,
                                ProductName = item.ProductName,
                                IsApply = item.IsAppliedStore,
                                ExpiredTime = item.ExpiredTime,
                                ExpiredTimeDisplay = ((item.ExpiredTime.HasValue && item.ExpiredTime.Value.Date == Commons.MaxDate.Date) || !item.ExpiredTime.HasValue) ? Commons.NoExpiryDate : item.ExpiredTime.Value.ToString(Commons.FormatDateTime),
                                ActiveTime = item.ActiveTime,
                                OffSet = i
                            });
                            i++;
                        }
                        else
                        {
                            if (model.StoreType == (int)Commons.EStoreType.FnB && (item.Type == (int)Commons.EType.NuPOS
                                    || item.Type == (int)Commons.EType.NuKiosk || item.Type == (int)Commons.EType.NuDisplay))
                            {
                                model.ListProductApply.Add(new ProductApply()
                                {
                                    AssetID = item.AssetID,
                                    ProductName = item.ProductName,
                                    IsApply = item.IsAppliedStore,
                                    ExpiredTime = item.ExpiredTime,
                                    ExpiredTimeDisplay = ((item.ExpiredTime.HasValue && item.ExpiredTime.Value.Date == Commons.MaxDate.Date) || !item.ExpiredTime.HasValue) ? Commons.NoExpiryDate : item.ExpiredTime.Value.ToString(Commons.FormatDateTime),
                                    ActiveTime = item.ActiveTime,
                                    OffSet = i
                                });
                                i++;
                            }
                            else
                            {
                                if (model.StoreType == (int)Commons.EStoreType.Beauty && (item.Type == (int)Commons.EType.POZZ
                                    || item.Type == (int)Commons.EType.POZZ_Display || item.Type == (int)Commons.EType.POZZ_Kiosk))
                                {
                                    model.ListProductApply.Add(new ProductApply()
                                    {
                                        AssetID = item.AssetID,
                                        ProductName = item.ProductName,
                                        IsApply = item.IsAppliedStore,
                                        ExpiredTime = item.ExpiredTime,
                                        ExpiredTimeDisplay = ((item.ExpiredTime.HasValue && item.ExpiredTime.Value.Date == Commons.MaxDate.Date) || !item.ExpiredTime.HasValue) ? Commons.NoExpiryDate : item.ExpiredTime.Value.ToString(Commons.FormatDateTime),
                                        ActiveTime = item.ActiveTime,
                                        OffSet = i
                                    });
                                    i++;
                                }
                            }
                        }
                    }//end item.ProductType == (int)Commons.EProductType.Product
                    else
                    {
                        if (item.ProductType == (int)Commons.EProductType.Package)
                        {
                            if (item.ListProduct != null && item.ListProduct.Count > 0)
                            {
                                foreach (var _item in item.ListProduct)
                                {
                                    if (_item.Type == (int)Commons.EType.POinS_Link_App)
                                    {
                                        model.ListProductApply.Add(new ProductApply()
                                        {
                                            AssetID = _item.AssetID,
                                            ProductName = _item.ProductName + " (in " + item.ProductName + ")",
                                            IsApply = _item.IsAppliedStore,
                                            ExpiredTime = _item.ExpiredTime,
                                            ExpiredTimeDisplay = ((item.ExpiredTime.HasValue && item.ExpiredTime.Value.Date == Commons.MaxDate.Date) || !item.ExpiredTime.HasValue) ? Commons.NoExpiryDate : item.ExpiredTime.Value.ToString(Commons.FormatDateTime),
                                            ActiveTime = item.ActiveTime,
                                            OffSet = i
                                        });
                                        i++;
                                    }
                                    else
                                    {
                                        if (model.StoreType == (int)Commons.EStoreType.FnB && (_item.Type == (int)Commons.EType.NuPOS
                                                || _item.Type == (int)Commons.EType.NuKiosk || _item.Type == (int)Commons.EType.NuDisplay))
                                        {
                                            model.ListProductApply.Add(new ProductApply()
                                            {
                                                AssetID = _item.AssetID,
                                                ProductName = _item.ProductName + " (in " + item.ProductName + ")",
                                                IsApply = _item.IsAppliedStore,
                                                ExpiredTime = _item.ExpiredTime,
                                                ExpiredTimeDisplay = ((item.ExpiredTime.HasValue && item.ExpiredTime.Value.Date == Commons.MaxDate.Date) || !item.ExpiredTime.HasValue) ? Commons.NoExpiryDate : item.ExpiredTime.Value.ToString(Commons.FormatDateTime),
                                                ActiveTime = item.ActiveTime,
                                                OffSet = i
                                            });
                                            i++;
                                        }
                                        else
                                        {
                                            if (model.StoreType == (int)Commons.EStoreType.Beauty && (_item.Type == (int)Commons.EType.POZZ
                                                || _item.Type == (int)Commons.EType.POZZ_Display || _item.Type == (int)Commons.EType.POZZ_Kiosk))
                                            {
                                                model.ListProductApply.Add(new ProductApply()
                                                {
                                                    AssetID = _item.AssetID,
                                                    ProductName = _item.ProductName + " (in " + item.ProductName + ")",
                                                    IsApply = _item.IsAppliedStore,
                                                    ExpiredTime = _item.ExpiredTime,
                                                    ExpiredTimeDisplay = ((item.ExpiredTime.HasValue && item.ExpiredTime.Value.Date == Commons.MaxDate.Date) || !item.ExpiredTime.HasValue) ? Commons.NoExpiryDate : item.ExpiredTime.Value.ToString(Commons.FormatDateTime),
                                                    ActiveTime = item.ActiveTime,
                                                    OffSet = i
                                                });
                                                i++;
                                            }
                                        }
                                    }

                                }
                            }
                        }
                        else
                        {
                            if (item.ProductType == (int)Commons.EProductType.Addition && item.AdditionApply != null)
                            {
                                if (item.AdditionApply.Type == (int)Commons.EType.POinS_Link_App)
                                {
                                    model.ListProductApply.Add(new ProductApply()
                                    {
                                        AssetID = item.AssetID,
                                        ProductName = item.AdditionApply.Name,
                                        IsApply = item.IsAppliedStore,
                                        ExpiredTime = item.ExpiredTime,
                                        ExpiredTimeDisplay = ((item.ExpiredTime.HasValue && item.ExpiredTime.Value.Date == Commons.MaxDate.Date) || !item.ExpiredTime.HasValue) ? Commons.NoExpiryDate : item.ExpiredTime.Value.ToString(Commons.FormatDateTime),
                                        ActiveTime = item.ActiveTime,
                                        OffSet = i
                                    });
                                    i++;
                                }
                                else
                                {
                                    if (model.StoreType == (int)Commons.EStoreType.FnB && (item.AdditionApply.Type == (int)Commons.EType.NuPOS
                                        || item.AdditionApply.Type == (int)Commons.EType.NuKiosk || item.AdditionApply.Type == (int)Commons.EType.NuDisplay))
                                    {
                                        model.ListProductApply.Add(new ProductApply()
                                        {
                                            AssetID = item.AssetID,
                                            ProductName = item.AdditionApply.Name,
                                            IsApply = item.IsAppliedStore,
                                            ExpiredTime = item.ExpiredTime,
                                            ExpiredTimeDisplay = ((item.ExpiredTime.HasValue && item.ExpiredTime.Value.Date == Commons.MaxDate.Date) || !item.ExpiredTime.HasValue) ? Commons.NoExpiryDate : item.ExpiredTime.Value.ToString(Commons.FormatDateTime),
                                            ActiveTime = item.ActiveTime,
                                            OffSet = i
                                        });
                                        i++;
                                    }
                                    else
                                    {
                                        if (model.StoreType == (int)Commons.EStoreType.Beauty && (item.AdditionApply.Type == (int)Commons.EType.POZZ
                                        || item.AdditionApply.Type == (int)Commons.EType.POZZ_Kiosk || item.AdditionApply.Type == (int)Commons.EType.POZZ_Display))
                                        {
                                            model.ListProductApply.Add(new ProductApply()
                                            {
                                                AssetID = item.AssetID,
                                                ProductName = item.AdditionApply.Name,
                                                IsApply = item.IsAppliedStore,
                                                ExpiredTime = item.ExpiredTime,
                                                ExpiredTimeDisplay = ((item.ExpiredTime.HasValue && item.ExpiredTime.Value.Date == Commons.MaxDate.Date) || !item.ExpiredTime.HasValue) ? Commons.NoExpiryDate : item.ExpiredTime.Value.ToString(Commons.FormatDateTime),
                                                ActiveTime = item.ActiveTime,
                                                OffSet = i
                                            });
                                            i++;
                                        }
                                    }
                                }

                            }
                        }
                    }

                }
                model.ListProductApply.ForEach(x =>
                {
                    if (x.ExpiredTime.HasValue)
                        x.ExpiredTime = CommonHelper.ConvertToLocalTime(x.ExpiredTime.Value);
                    if (x.ActiveTime.HasValue)
                        x.ActiveTime = CommonHelper.ConvertToLocalTime(x.ActiveTime.Value);
                });
                //model.ListApplyProduct.ForEach(x => x. = CommonHelper.ConvertToLocalTime(x.ExpiredTime.Value));
            }
            model.ImageURL = string.IsNullOrEmpty(model.ImageURL) ? _ImgDummyStore : model.ImageURL;
            model.Industry = model.StoreType.ToString();
            var _listCounTry = _baseFactory.GetListCountry();
            ViewBag.ListCounTry = _listCounTry;
            ViewBag.ListCounTry = ViewBag.ListCounTry ?? "";
            ViewBag.TimeZones = "";
            foreach (var item in _listCounTry)
            {
                if (item.Name == model.Country)
                    ViewBag.TimeZones = item.TimeZones;
            }
            var name = model.ListProductApply.Where(x => x.IsApply).ToList();
            return PartialView("_StoreInfo", model);
        }
        [HttpPost]
        public ActionResult UpdateStoreInfo(StoreDetailModels model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    ViewBag.ListCounTry = _baseFactory.GetListCountry();
                    ViewBag.ListCounTry = ViewBag.ListCounTry ?? "";
                    List<string> _timezone = new List<string>();
                    _timezone.Add(model.TimeZone);
                    ViewBag.TimeZones = _timezone;
                    ViewBag.TimeZones = ViewBag.TimeZones ?? "";
                    return PartialView(/*"_StoreInfo"*/"_PopupStore", model);
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
                        model.ImageURL = model.ImageURL.Replace(Commons.PublicImagesClient, "").Replace(_ImgDummyStore, "");
                }
                string msg = "";
                var temp = model.PictureByte;
                model.PictureByte = null;
                var result = _factory.UpdateStoreInfo(model, CurrentUser.UserId, ref msg);
                if (result)
                {
                    model.PictureByte = temp;
                    if (!string.IsNullOrEmpty(model.ImageURL) && model.PictureByte != null)
                    {
                        var originalDirectory = new DirectoryInfo(string.Format("{0}Uploads\\", Server.MapPath(@"\")));
                        var path = string.Format("{0}{1}", originalDirectory, model.ImageURL);
                        MemoryStream ms = new MemoryStream(photoByte, 0, photoByte.Length);
                        ms.Write(photoByte, 0, photoByte.Length);
                        System.Drawing.Image imageTmp = System.Drawing.Image.FromStream(ms, true);
                        ImageHelper.Me.SaveCroppedImage(imageTmp, path, model.ImageURL, ref photoByte);
                        model.PictureByte = photoByte;
                        FTP.UploadClient(model.ImageURL, model.PictureByte);
                        ImageHelper.Me.TryDeleteImageUpdated(path);
                    }
                    return new HttpStatusCodeResult(HttpStatusCode.OK);
                }
                else
                {
                    ModelState.AddModelError("Name", msg);
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    ViewBag.ListCounTry = _baseFactory.GetListCountry();
                    ViewBag.ListCounTry = ViewBag.ListCounTry ?? "";
                    List<string> _timezone = new List<string>();
                    _timezone.Add(model.TimeZone);
                    ViewBag.TimeZones = _timezone;
                    ViewBag.TimeZones = ViewBag.TimeZones ?? "";
                    return PartialView(/*"_StoreInfo"*/"_PopupStore", model);
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("UpdateStoreInfo: ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }

        public ActionResult AssignProductToStore(StoreDetailModels models)
        {
            try
            {
                if (models.ListProductApply != null && models.ListProductApply.Count > 0)
                {
                    List<StoreAssignProduct> _StoreAssignProduct = new List<StoreAssignProduct>();
                    foreach (var item in models.ListProductApply.Where(x => x.IsApply))
                    {
                        _StoreAssignProduct.Add(new StoreAssignProduct
                        {
                            AssetID = item.AssetID,
                            StoreID = models.ID
                        });
                    }
                    string mess = "";
                    bool IsStoreAssignProduct = true;
                    var IsOk = _factory.StoreAssignProduct(_StoreAssignProduct, models.ID, IsStoreAssignProduct, CurrentUser.UserId, ref mess);
                    return Json(IsOk, JsonRequestBehavior.AllowGet);
                }
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("Assign Product to Store", ex);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetListProductApplyStore()
        {
            MyStoreAndBusinessViewModels model = new MyStoreAndBusinessViewModels();
            model.ListProductApply = _factory.GetListProductApplyStore(CurrentUser.UserId)
                                             .Where(x => x.IsUnlimitedLocation ||
                                                     //(!x.IsUnlimitedLocation && x.RemainingLocation > 0 && CommonHelper.ConvertToLocalTime(x.ExpiredTime.Value) > DateTime.Now)).ToList();
                                                     (!x.IsUnlimitedLocation && x.RemainingLocation > 0 && x.ExpiredTime > DateTime.Now)
                                                     || !x.IsUnlimitedLocation && x.RemainingLocation > 0 && x.ExpiredTime == null
                                                     || !x.IsUnlimitedLocation && x.RemainingLocation > 0 && x.ExpiredTime == Commons.MinDate).ToList();
            model.ListProductApply.ForEach(x =>
            {
                string Period = x.PeriodType == (byte)Commons.EPeriodType.Day ? Commons.PeriodTypeDay :
                                x.PeriodType == (byte)Commons.EPeriodType.Month ? Commons.PeriodTypeMonth :
                                x.PeriodType == (byte)Commons.EPeriodType.Year ? Commons.PeriodTypeYear :
                                x.PeriodType == (byte)Commons.EPeriodType.OneTime ? Commons.PeriodTypeOneTime :
                                Commons.PeriodTypeNone;
                x.sPeriod = (x.PeriodType != (byte)Commons.EPeriodType.OneTime) ? ((x.Period > 1) ? (x.Period + " " + Period + "s") : (x.Period + " " + Period)) : Period;
                if (x.PeriodType == (byte)Commons.EPeriodType.OneTime)
                    x.sExpiredTime = "No expiry date";
                else
                {
                    if (x.ExpiredTime.HasValue)
                        x.sExpiredTime = CommonHelper.ConvertToLocalTime(x.ExpiredTime.Value).ToString(Commons.FormatDateTime);
                    else
                        x.sExpiredTime = "Not yet activated";
                }

                //=====
                if (x.IsUnlimitedLocation)
                    x.sRemainingLocation = "This item has unlimited locations remaining";
                else
                    x.sRemainingLocation = "This item still has " + x.RemainingLocation + " location(s) remaining";

                x.ProductImageURL = string.IsNullOrEmpty(x.ProductImageURL) ? _ImgDummyProduct : x.ProductImageURL;
            });
            model.isShow = model.ListProductApply != null && model.ListProductApply.Count > 0 ? true : false;
            return PartialView("_StoreProduct", model);
        }

        public ActionResult ProductSelect(string AssetID)
        {
            var ListProductApply = _factory.GetListProductApplyStore(CurrentUser.UserId);
            var product = ListProductApply.Where(x => x.AssetID.Equals(AssetID)).FirstOrDefault();
            var ListProductType = new List<SelectListItem>();
            CheckIpAddressFactory _checkIpFactory = new CheckIpAddressFactory();
            StoreDetailModels model = new StoreDetailModels();
            var ListCounTry = _baseFactory.GetListCountry();
            //var CountryCode = _checkIpFactory.GetCountryCode();
            if (ListCounTry != null)
            {
                var objCountry = ListCounTry.Where(x => x.Alpha2Code.Equals(CountryCode)).FirstOrDefault();
                model.Country = objCountry != null ? objCountry.Name : "";
                ViewBag.TimeZones = objCountry.TimeZones;
            }
            ViewBag.ListCounTry = ListCounTry;
            ViewBag.ListCounTry = ViewBag.ListCounTry ?? "";
            ViewBag.TimeZones = ViewBag.TimeZones ?? "";
            model.IsNew = true;
            model.AssetID = AssetID;
            model.sID = AssetID;
            model.ImageURL = string.IsNullOrEmpty(model.ImageURL) ? _ImgDummyStore : model.ImageURL;
            if (product != null)
            {
                if (product.ProductType == (byte)Commons.EProductType.Package)
                {
                    if (product.ListProduct != null && product.ListProduct.Count > 0)
                    {
                        foreach (var item in product.ListProduct)
                        {
                            model.ListApplyProductClient.Add(new SelectListItem
                            {
                                Value = item.AssetID,
                                Text = item.ProductName,

                            });
                            model.sAssetID = item.AssetID;
                            ListProductType.Add(new SelectListItem
                            {
                                Value = item.Type.ToString(),
                                Text = item.AssetID
                            });
                        }
                    }
                    else
                    {
                        model.ListApplyProductClient.Add(new SelectListItem
                        {
                            Value = product.AssetID,
                            Text = product.ProductName
                        });
                        model.sAssetID = product.AssetID;

                        if (product.Type == (int)Commons.EType.NuPOS || product.Type == (int)Commons.EType.NuKiosk || product.Type == (int)Commons.EType.NuDisplay)
                            model.Industry = Commons.EStoreType.FnB.ToString("d");
                        else
                        {
                            if (product.Type == (int)Commons.EType.POinS_Link_App)
                            {
                                model.Industry = Commons.EStoreType.FnB.ToString("d");
                            }
                            else
                                model.Industry = Commons.EStoreType.Beauty.ToString("d");
                        }
                    }
                }
                else if (product.ProductType == (byte)Commons.EProductType.Addition)
                {
                    if (product.AdditionApply != null)
                    {
                        model.ListApplyProductClient.Add(new SelectListItem
                        {
                            Value = product.AdditionApply.ID,
                            Text = product.AdditionApply.Name + (product.AdditionApply.ParentName == null ? "" : " (In " + product.AdditionApply.ParentName + ") ")
                        });
                        model.sAssetID = product.AdditionApply.ID;

                        if (product.AdditionApply.Type == (int)Commons.EType.NuPOS || product.AdditionApply.Type == (int)Commons.EType.NuKiosk || product.AdditionApply.Type == (int)Commons.EType.NuDisplay)
                        {
                            model.Industry = Commons.EStoreType.FnB.ToString("d");
                        }
                        else
                        {
                            if (product.AdditionApply.Type == (int)Commons.EType.POZZ || product.AdditionApply.Type == (int)Commons.EType.POZZ_Display || product.AdditionApply.Type == (int)Commons.EType.POZZ_Kiosk)
                            {
                                model.Industry = Commons.EStoreType.Beauty.ToString("d");
                            }
                            else
                            {
                                model.Industry = Commons.EStoreType.FnB.ToString("d");
                            }
                        }
                        model.Type = product.AdditionApply.Type;
                    }
                    else
                    {
                        model.ListApplyProductClient.Add(new SelectListItem
                        {
                            Value = product.AssetID,
                            Text = product.ProductName
                        });
                        model.sAssetID = product.AssetID;

                        if (product.Type == (int)Commons.EType.NuPOS || product.Type == (int)Commons.EType.NuKiosk || product.Type == (int)Commons.EType.NuDisplay)
                            model.Industry = Commons.EStoreType.FnB.ToString("d");
                        else
                        {
                            if (product.Type == (int)Commons.EType.POinS_Link_App)
                            {
                                model.Industry = Commons.EStoreType.FnB.ToString("d");
                                ViewBag.IsDisabledIndustry = false;
                            }
                            else
                                model.Industry = Commons.EStoreType.Beauty.ToString("d");
                        }
                    }
                }
                else
                {
                    if (product.ProductType == (byte)Commons.EProductType.Product)
                    {
                        model.ListApplyProductClient.Add(new SelectListItem
                        {
                            Value = product.AssetID,
                            Text = product.ProductName
                        });
                        if (product.Type == (int)Commons.EType.NuPOS || product.Type == (int)Commons.EType.NuKiosk || product.Type == (int)Commons.EType.NuDisplay)
                            model.Industry = Commons.EStoreType.FnB.ToString("d");
                        else
                        {
                            if (product.Type == (int)Commons.EType.POinS_Link_App)
                            {
                                model.Industry = Commons.EStoreType.FnB.ToString("d");
                                ViewBag.IsDisabledIndustry = false;
                            }
                            else
                                model.Industry = Commons.EStoreType.Beauty.ToString("d");
                        }
                    }
                    model.sAssetID = product.AssetID;
                    model.Type = product.Type;
                }
                model.ProductType = product.ProductType;
            }
            ViewBag.IsDisable = model.ListApplyProductClient.Count == 1 ? true : false;
            ViewBag.ListProduct = ListProductType;
            return PartialView("_StoreSelect", model);
        }
        public ActionResult GetListProductAssignToStore(int Storetype, string AssetID)
        {
            StoreDetailModels model = new StoreDetailModels();
            var _ProductApplyStore = _factory.GetListProductApplyStore(CurrentUser.UserId);
            var _listproduct = _ProductApplyStore.Where(x => x.AssetID != AssetID).ToList();
            if (_listproduct != null && _listproduct.Count > 0)
            {
                int i = 0;
                foreach (var item in _listproduct)
                {
                    if (item.ProductType == (int)Commons.EProductType.Product)
                    {

                        if (item.Type == (int)Commons.EType.POinS_Link_App)
                        {
                            model.ListProductApply.Add(new ProductApply()
                            {
                                AssetID = item.AssetID,
                                ProductName = item.ProductName,
                                IsApply = true,
                                ExpiredTime = item.ExpiredTime,
                                ActiveTime = item.ActiveTime,
                                OffSet = i
                            });
                            i++;
                        }
                        else
                        {
                            if (Storetype == 1 && (item.Type == (int)Commons.EType.NuPOS
                                    || item.Type == (int)Commons.EType.NuKiosk || item.Type == (int)Commons.EType.NuDisplay))
                            {
                                model.ListProductApply.Add(new ProductApply()
                                {
                                    AssetID = item.AssetID,
                                    ProductName = item.ProductName,
                                    IsApply = true,
                                    ExpiredTime = item.ExpiredTime,
                                    ActiveTime = item.ActiveTime,
                                    OffSet = i
                                });
                                i++;
                            }
                            else
                            {
                                if (Storetype == 2 && (item.Type == (int)Commons.EType.POZZ
                                    || item.Type == (int)Commons.EType.POZZ_Display || item.Type == (int)Commons.EType.POZZ_Kiosk))
                                {
                                    model.ListProductApply.Add(new ProductApply()
                                    {
                                        AssetID = item.AssetID,
                                        ProductName = item.ProductName,
                                        IsApply = true,
                                        ExpiredTime = item.ExpiredTime,
                                        ActiveTime = item.ActiveTime,
                                        OffSet = i
                                    });
                                    i++;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (item.ProductType == (int)Commons.EProductType.Package)
                        {
                            if (item.ListProduct != null && item.ListProduct.Count > 0)
                            {
                                foreach (var _item in item.ListProduct.Where(x => x.AssetID != AssetID))
                                {
                                    if (_item.Type == (int)Commons.EType.POinS_Link_App)
                                    {
                                        model.ListProductApply.Add(new ProductApply()
                                        {
                                            AssetID = _item.AssetID,
                                            ProductName = _item.ProductName + " (in " + item.ProductName + ")",
                                            IsApply = true,
                                            ExpiredTime = _item.ExpiredTime,
                                            ActiveTime = item.ActiveTime,
                                            OffSet = i
                                        });
                                        i++;
                                    }
                                    else
                                    {
                                        if (Storetype == 1 && (_item.Type == (int)Commons.EType.NuPOS
                                                || _item.Type == (int)Commons.EType.NuKiosk || _item.Type == (int)Commons.EType.NuDisplay))
                                        {
                                            model.ListProductApply.Add(new ProductApply()
                                            {
                                                AssetID = _item.AssetID,
                                                ProductName = _item.ProductName + " (in " + item.ProductName + ")",
                                                IsApply = true,
                                                ExpiredTime = _item.ExpiredTime,
                                                ActiveTime = item.ActiveTime,
                                                OffSet = i
                                            });
                                            i++;
                                        }
                                        else
                                        {
                                            if (Storetype == 2 && (_item.Type == (int)Commons.EType.POZZ
                                                || _item.Type == (int)Commons.EType.POZZ_Display || _item.Type == (int)Commons.EType.POZZ_Kiosk))
                                            {
                                                model.ListProductApply.Add(new ProductApply()
                                                {
                                                    AssetID = _item.AssetID,
                                                    ProductName = _item.ProductName + " (in " + item.ProductName + ")",
                                                    IsApply = true,
                                                    ExpiredTime = _item.ExpiredTime,
                                                    ActiveTime = item.ActiveTime,
                                                    OffSet = i
                                                });
                                                i++;
                                            }
                                        }
                                    }

                                }
                            }
                        }
                        else
                        {
                            if (item.ProductType == (int)Commons.EProductType.Addition && item.AdditionApply != null)
                            {
                                if (item.AdditionApply.Type == (int)Commons.EType.POinS_Link_App)
                                {
                                    model.ListProductApply.Add(new ProductApply()
                                    {
                                        AssetID = item.AssetID,
                                        ProductName = item.AdditionApply.Name,
                                        IsApply = true,
                                        ExpiredTime = item.ExpiredTime,
                                        ActiveTime = item.ActiveTime,
                                        OffSet = i
                                    });
                                    i++;
                                }
                                else
                                {
                                    if (Storetype == 1 && (item.AdditionApply.Type == (int)Commons.EType.NuPOS
                                        || item.AdditionApply.Type == (int)Commons.EType.NuKiosk || item.AdditionApply.Type == (int)Commons.EType.NuDisplay))
                                    {
                                        model.ListProductApply.Add(new ProductApply()
                                        {
                                            AssetID = item.AssetID,
                                            ProductName = item.AdditionApply.Name,
                                            IsApply = true,
                                            ExpiredTime = item.ExpiredTime,
                                            ActiveTime = item.ActiveTime,
                                            OffSet = i
                                        });
                                        i++;
                                    }
                                    else
                                    {
                                        if (Storetype == 2 && (item.AdditionApply.Type == (int)Commons.EType.POZZ
                                        || item.AdditionApply.Type == (int)Commons.EType.POZZ_Kiosk || item.AdditionApply.Type == (int)Commons.EType.POZZ_Display))
                                        {
                                            model.ListProductApply.Add(new ProductApply()
                                            {
                                                AssetID = item.AssetID,
                                                ProductName = item.AdditionApply.Name,
                                                IsApply = true,
                                                ExpiredTime = item.ExpiredTime,
                                                ActiveTime = item.ActiveTime,
                                                OffSet = i
                                            });
                                            i++;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            model.ListProductApply.ForEach(x =>
            {
                if (x.ExpiredTime.HasValue)
                    x.ExpiredTime = CommonHelper.ConvertToLocalTime(x.ExpiredTime.Value);
                if (x.ActiveTime.HasValue)
                    x.ActiveTime = CommonHelper.ConvertToLocalTime(x.ActiveTime.Value);
            });
            return PartialView("_ItemStoreAssignToProduct", model);
        }

        public ActionResult GetDetailStoreToJSon(string StoreId)
        {
            StoreDetailModels store = new StoreDetailModels();
            store = _factory.GetStoreInfo(StoreId);
            return Json(store, JsonRequestBehavior.AllowGet);
        }

        List<string> listStorePropertiesReject = null;
        public void PropertyReject()
        {
            foreach (var item in listStorePropertiesReject)
            {
                if (ModelState.ContainsKey(item))
                    ModelState[item].Errors.Clear();
            }
        }

        public ActionResult GetListStoreSelect(int type)
        {
            MyStoreAndBusinessViewModels model = new MyStoreAndBusinessViewModels();
            var _storeType = (type == (int)Commons.EType.NuPOS
                                                                       || type == (int)Commons.EType.NuKiosk
                                                                       || type == (int)Commons.EType.NuDisplay) ? (int)Commons.EStoreType.FnB :
                                (type == (int)Commons.EType.POZZ
                                                                       || type == (int)Commons.EType.POZZ_Kiosk
                                                                       || type == (int)Commons.EType.POZZ_Display) ? (int)Commons.EStoreType.Beauty :
                                                                       0;


            List<StoreModels> GetListStore = _factory.GetListStore(CurrentUser.UserId)
                                    .Where(x => (_storeType != 0 ? (x.StoreType == _storeType || x.StoreType == 0) : true))
                                    .ToList();
            model.ListStore = GetListStore.Where(x => !x.IsTemp).ToList();
            if (model.ListStore != null && model.ListStore.Count > 0)
            {
                model.ListStore.ForEach(x =>
                {
                    string status = "";
                    if (x.StoreStatus == (byte)Commons.EStoreStatus.Expired)
                        status = Commons.EStoreStatus.Expired.ToString();
                    else if (x.StoreStatus == (byte)Commons.EStoreStatus.InUse)
                        status = "In Use";
                    else if (x.StoreStatus == (byte)Commons.EStoreStatus.Temporary)
                        status = Commons.EStoreStatus.Temporary.ToString();
                    x.sStatus = status;
                });
            }
            return PartialView("_ListItemStoreSelect", model);
        }

        [HttpPost]
        public ActionResult CreateStoreInfo(StoreDetailModels model)
        {
            try
            {
                byte[] photoByte = null;
                var ListProductType = new List<SelectListItem>();
                //Create New Store
                //if (string.IsNullOrEmpty(model.AssetID))
                //    model.AssetID = model.sAssetID;

                if (model.IsNew)
                {
                    if (string.IsNullOrEmpty(model.Name))
                        ModelState.AddModelError("Name", "The name is required");
                    if (string.IsNullOrEmpty(model.Phone))
                        ModelState.AddModelError("Phone", "The phone is required");
                    if (string.IsNullOrEmpty(model.Email))
                        ModelState.AddModelError("Email", "The phone is required");
                    if (!ModelState.IsValid)
                    {
                        var ListProductApply = _factory.GetListProductApplyStore(CurrentUser.UserId);
                        var product = ListProductApply.Where(x => x.AssetID.Equals(model.sID)).FirstOrDefault();
                        if (product != null)
                        {
                            if (product.ProductType == (byte)Commons.EProductType.Package)
                            {
                                if (product.ListProduct != null && product.ListProduct.Count > 0)
                                {
                                    foreach (var item in product.ListProduct)
                                    {
                                        model.ListApplyProductClient.Add(new SelectListItem
                                        {
                                            Value = item.AssetID,
                                            Text = item.ProductName,

                                        });
                                        model.sAssetID = item.AssetID;
                                        ListProductType.Add(new SelectListItem
                                        {
                                            Value = item.Type.ToString(),
                                            Text = item.AssetID
                                        });
                                    }
                                    var temp = ListProductType.FirstOrDefault();
                                    if (temp != null)
                                    {
                                        model.Industry = temp.Value;
                                    }
                                }
                                else
                                {
                                    model.ListApplyProductClient.Add(new SelectListItem
                                    {
                                        Value = product.AssetID,
                                        Text = product.ProductName
                                    });
                                    model.sAssetID = product.AssetID;

                                    if (product.Type == (int)Commons.EType.NuPOS || product.Type == (int)Commons.EType.NuKiosk || product.Type == (int)Commons.EType.NuDisplay)
                                        model.Industry = Commons.EStoreType.FnB.ToString("d");
                                    else
                                    {
                                        if (product.Type == (int)Commons.EType.POinS_Link_App)
                                        {
                                            model.Industry = Commons.EStoreType.FnB.ToString("d");
                                        }
                                        else
                                            model.Industry = Commons.EStoreType.Beauty.ToString("d");
                                    }
                                }
                            }
                            else if (product.ProductType == (byte)Commons.EProductType.Addition)
                            {
                                if (product.AdditionApply != null)
                                {
                                    model.ListApplyProductClient.Add(new SelectListItem
                                    {
                                        Value = product.AdditionApply.ID,
                                        Text = product.AdditionApply.Name + "( In " + product.AdditionApply.ParentName + ")"
                                    });
                                    model.sAssetID = product.AdditionApply.ID;

                                    if (product.AdditionApply.Type == (int)Commons.EType.NuPOS || product.AdditionApply.Type == (int)Commons.EType.NuKiosk || product.AdditionApply.Type == (int)Commons.EType.NuDisplay)
                                    {
                                        model.Industry = Commons.EStoreType.FnB.ToString("d");
                                    }
                                    else
                                    {
                                        if (product.AdditionApply.Type == (int)Commons.EType.POZZ || product.AdditionApply.Type == (int)Commons.EType.POZZ_Display || product.AdditionApply.Type == (int)Commons.EType.POZZ_Kiosk)
                                        {
                                            model.Industry = Commons.EStoreType.Beauty.ToString("d");
                                        }
                                        else
                                        {
                                            model.Industry = Commons.EStoreType.FnB.ToString("d");
                                        }
                                    }
                                }
                                else
                                {
                                    model.ListApplyProductClient.Add(new SelectListItem
                                    {
                                        Value = product.AssetID,
                                        Text = product.ProductName
                                    });
                                    model.sAssetID = product.AssetID;

                                    if (product.Type == (int)Commons.EType.NuPOS || product.Type == (int)Commons.EType.NuKiosk || product.Type == (int)Commons.EType.NuDisplay)
                                        model.Industry = Commons.EStoreType.FnB.ToString("d");
                                    else
                                    {
                                        if (product.Type == (int)Commons.EType.POinS_Link_App)
                                        {
                                            model.Industry = Commons.EStoreType.FnB.ToString("d");
                                        }
                                        else
                                            model.Industry = Commons.EStoreType.Beauty.ToString("d");
                                    }
                                }
                            }
                            else
                            {
                                if (product.ProductType == (byte)Commons.EProductType.Product)
                                {
                                    model.ListApplyProductClient.Add(new SelectListItem
                                    {
                                        Value = product.AssetID,
                                        Text = product.ProductName
                                    });
                                    if (product.Type == (int)Commons.EType.NuPOS || product.Type == (int)Commons.EType.NuKiosk || product.Type == (int)Commons.EType.NuDisplay)
                                        model.Industry = Commons.EStoreType.FnB.ToString("d");
                                    else
                                    {
                                        if (product.Type == (int)Commons.EType.POinS_Link_App)
                                        {
                                            model.Industry = Commons.EStoreType.FnB.ToString("d");
                                        }
                                        else
                                            model.Industry = Commons.EStoreType.Beauty.ToString("d");
                                    }
                                }
                                model.sAssetID = product.AssetID;
                            }
                        }
                        ViewBag.IsDisable = model.ListApplyProductClient.Count == 1 ? true : false;

                        var ListCountry = _baseFactory.GetListCountry();
                        ViewBag.ListCounTry = ListCountry;
                        ViewBag.ListCounTry = ViewBag.ListCounTry ?? "";
                        var CounTry = ListCountry.Where(w => w.Name.Equals(model.Country)).FirstOrDefault();
                        //ViewBag.TimeZones = CounTry.TimeZones;
                        ViewBag.TimeZones = ViewBag.TimeZones ?? "";
                        ViewBag.IsBack = true;
                        Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        ViewBag.ListProduct = ListProductType;
                        return PartialView("_StoreSelect", model);
                    }
                    //====================

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
                            model.ImageURL = model.ImageURL.Replace(Commons.PublicImagesClient, "").Replace(_ImgDummyStore, "");
                    }
                }
                else //Existing Store 
                {
                    model.IsActive = true;
                    listStorePropertiesReject = new List<string>();
                    listStorePropertiesReject.Add("Name");
                    listStorePropertiesReject.Add("Email");
                    listStorePropertiesReject.Add("Phone");
                    listStorePropertiesReject.Add("Industry");
                    listStorePropertiesReject.Add("TimeZone");
                    PropertyReject();

                }
                List<string> LstAssetID = new List<string>();
                LstAssetID = model.ListProductApply.Where(w => w.IsApply).Select(o => o.AssetID).ToList();
                model.ListAssetID.AddRange(LstAssetID);
                string msg = "";
                model.ProductID = model.AssetID;
                string StoreIDReturn = "";
                var tempPB = model.PictureByte;
                model.PictureByte = null;
                var result = true;
                if (model.ListAssetID.Count == 0)
                {
                    result = _facCart.CreateStoreInfoTemp(model, CurrentUser.UserId, ref msg, ref StoreIDReturn);
                }
                else
                {
                    result = _factory.CreateStoreInfo(model, CurrentUser.UserId, ref StoreIDReturn, ref msg);
                }
                if (result)
                {
                    model.PictureByte = tempPB;
                    if (model.IsNew)
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
                            FTP.UploadClient(model.ImageURL, model.PictureByte);
                            ImageHelper.Me.TryDeleteImageUpdated(path);
                        }
                    }
                    return new HttpStatusCodeResult(HttpStatusCode.OK);
                }
                else
                {
                    var ListProductApply = _factory.GetListProductApplyStore(CurrentUser.UserId);
                    var product = ListProductApply.Where(x => x.AssetID.Equals(model.sID)).FirstOrDefault();
                    if (product != null)
                    {
                        if (product.ProductType == (byte)Commons.EProductType.Package)
                        {
                            if (product.ListProduct != null && product.ListProduct.Count > 0)
                            {
                                foreach (var item in product.ListProduct)
                                {
                                    model.ListApplyProductClient.Add(new SelectListItem
                                    {
                                        Value = item.AssetID,
                                        Text = item.ProductName,

                                    });
                                    model.sAssetID = item.AssetID;
                                    ListProductType.Add(new SelectListItem
                                    {
                                        Value = item.Type.ToString(),
                                        Text = item.AssetID
                                    });
                                }
                                var temp = ListProductType.FirstOrDefault();
                                if (temp != null)
                                {
                                    model.Industry = temp.Value;
                                }
                            }
                            else
                            {
                                model.ListApplyProductClient.Add(new SelectListItem
                                {
                                    Value = product.AssetID,
                                    Text = product.ProductName
                                });
                                model.sAssetID = product.AssetID;

                                if (product.Type == (int)Commons.EType.NuPOS || product.Type == (int)Commons.EType.NuKiosk || product.Type == (int)Commons.EType.NuDisplay)
                                    model.Industry = Commons.EStoreType.FnB.ToString("d");
                                else
                                {
                                    if (product.Type == (int)Commons.EType.POinS_Link_App)
                                    {
                                        model.Industry = Commons.EStoreType.FnB.ToString("d");
                                    }
                                    else
                                        model.Industry = Commons.EStoreType.Beauty.ToString("d");
                                }
                            }
                        }
                        else if (product.ProductType == (byte)Commons.EProductType.Addition)
                        {
                            if (product.AdditionApply != null)
                            {
                                model.ListApplyProductClient.Add(new SelectListItem
                                {
                                    Value = product.AdditionApply.ID,
                                    Text = product.AdditionApply.Name + "( In " + product.AdditionApply.ParentName + ")"
                                });
                                model.sAssetID = product.AdditionApply.ID;

                                if (product.AdditionApply.Type == (int)Commons.EType.NuPOS || product.AdditionApply.Type == (int)Commons.EType.NuKiosk || product.AdditionApply.Type == (int)Commons.EType.NuDisplay)
                                {
                                    model.Industry = Commons.EStoreType.FnB.ToString("d");
                                }
                                else
                                {
                                    if (product.AdditionApply.Type == (int)Commons.EType.POZZ || product.AdditionApply.Type == (int)Commons.EType.POZZ_Display || product.AdditionApply.Type == (int)Commons.EType.POZZ_Kiosk)
                                    {
                                        model.Industry = Commons.EStoreType.Beauty.ToString("d");
                                    }
                                    else
                                    {
                                        model.Industry = Commons.EStoreType.FnB.ToString("d");
                                    }
                                }
                            }
                            else
                            {
                                model.ListApplyProductClient.Add(new SelectListItem
                                {
                                    Value = product.AssetID,
                                    Text = product.ProductName
                                });
                                model.sAssetID = product.AssetID;

                                if (product.Type == (int)Commons.EType.NuPOS || product.Type == (int)Commons.EType.NuKiosk || product.Type == (int)Commons.EType.NuDisplay)
                                    model.Industry = Commons.EStoreType.FnB.ToString("d");
                                else
                                {
                                    if (product.Type == (int)Commons.EType.POinS_Link_App)
                                    {
                                        model.Industry = Commons.EStoreType.FnB.ToString("d");
                                    }
                                    else
                                        model.Industry = Commons.EStoreType.Beauty.ToString("d");
                                }
                            }
                        }
                        else
                        {
                            if (product.ProductType == (byte)Commons.EProductType.Product)
                            {
                                model.ListApplyProductClient.Add(new SelectListItem
                                {
                                    Value = product.AssetID,
                                    Text = product.ProductName
                                });
                                if (product.Type == (int)Commons.EType.NuPOS || product.Type == (int)Commons.EType.NuKiosk || product.Type == (int)Commons.EType.NuDisplay)
                                    model.Industry = Commons.EStoreType.FnB.ToString("d");
                                else
                                {
                                    if (product.Type == (int)Commons.EType.POinS_Link_App)
                                    {
                                        model.Industry = Commons.EStoreType.FnB.ToString("d");
                                        ViewBag.IsDisabledIndustry = false;
                                    }
                                    else
                                        model.Industry = Commons.EStoreType.Beauty.ToString("d");
                                }
                            }
                            model.sAssetID = product.AssetID;
                        }
                    }
                    ViewBag.IsDisable = model.ListApplyProductClient.Count == 1 ? true : false;
                    ModelState.AddModelError("Name", msg);
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    var ListCountry = _baseFactory.GetListCountry();
                    ViewBag.ListCounTry = ListCountry;
                    var Country = ListCountry.Where(w => w.Name.Equals(model.Country)).FirstOrDefault();
                    ViewBag.TimeZones = Country.TimeZones;
                    ViewBag.TimeZones = ViewBag.TimeZones ?? "";
                    ViewBag.IsBack = true;
                    ViewBag.ListProduct = ListProductType;
                    return PartialView("_StoreSelect", model);
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("CreateStoreInfo: ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }

        #endregion/*End For Store*/

        #region  /* For Products & Packages */
        public ActionResult MATERIALS()
        {
            return PartialView("_ItemMaterials");
        }

        public ActionResult ONGOING()
        {
            var model = GetProductOrPackageOnGoing();
            return PartialView("_ItemOnGoing", model);
        }

        public ActionResult EXPIRED()
        {
            var model = GetProductOrPackageExpired();
            return PartialView("_ItemExpired", model);
        }

        public ActionResult ViewDetail(string AssetID = "", int TabId = 1)
        {
            var model = new ProductPackageUserModels();
            try
            {
                var temp = _factory.GetProductPackageDetail(AssetID);
                var _ListStore = _factory.GetListStore(CurrentUser.UserId);
                _ListStore = _ListStore.Where(x => x.StoreType != 0).ToList();
                if (temp != null)
                {
                    if (temp.ProductType == (int)Commons.EProductType.Package)
                    {
                        var _tempProducts = temp.ListProduct.Select(x => new ProductUserModels
                        {
                            AssetID = x.AssetID,
                            ItemID = x.ItemID,
                            ItemName = x.ItemName,
                            ProductType = x.ProductType,
                            Image = string.IsNullOrEmpty(x.Image) ? _ImgDummyProduct : x.Image,
                            PeriodType = x.PeriodType,
                            //ExpiryDate = x.ExpiryDate.HasValue? CommonHelper.ConvertToLocalTime(x.ExpiryDate.Value): x.ExpiryDate,
                            ExpiryDate = x.ExpiryDate,
                            NumberOfUnit = x.NumberOfUnit,
                            NumberOfAccount = x.NumberOfAccount,
                            IsActive = x.IsActive,
                            IsUnlimitedAccount = x.IsUnlimitedAccount,
                            IsUnlimitedUnit = x.IsUnlimitedUnit,
                            ListDevices = temp.ListDevice.Where(y => y.ProductID == x.ItemID).Select(z => new DeviceUserModels
                            {
                                ID = z.ID,
                                UUID = z.UUID,
                                ActiveTime = z.ActiveTime,
                                LicenseKey = z.LicenseKey,
                                ProductType = z.ProductType,
                                IsActive = z.IsActive,
                                ProductID = z.ProductID,
                                AssetID = temp.AssetID
                            }).ToList(),
                            ListFunction = x.ListFunction,
                            Category = x.Category,
                            ListStoreApply = x.ListStoreApply

                        }).ToList();
                        temp.ListProduct = _tempProducts;

                        model.Image = string.IsNullOrEmpty(model.Image) ? _ImgDummyPackage : model.Image;
                        var _liststores = _ListStore;
                        foreach (var _Product in temp.ListProduct)
                        {
                            if (_Product.Category.Type == (int)Commons.EType.NuPOS || _Product.Category.Type == (int)Commons.EType.NuKiosk || _Product.Category.Type == (int)Commons.EType.NuDisplay)
                                _liststores = _ListStore.Where(x => x.StoreType == (int)Commons.EStoreType.FnB).ToList();
                            else
                                if (_Product.Category.Type == (int)Commons.EType.POZZ || _Product.Category.Type == (int)Commons.EType.POZZ_Display || _Product.Category.Type == (int)Commons.EType.POZZ_Kiosk)
                                _liststores = _ListStore.Where(x => x.StoreType == (int)Commons.EStoreType.Beauty).ToList();
                            if (_liststores != null && _liststores.Count > 0)
                            {
                                AssignToStore _AssignToStore = new AssignToStore();
                                _AssignToStore.tempAssetID = _Product.AssetID;

                                int i = 0;
                                foreach (var item in _liststores)
                                {
                                    string _industry = "";
                                    _industry = item.StoreType == 1 ? Commons.StoreTypeFnB : item.StoreType == 2 ? Commons.StoreTypeBeauty : "";
                                    bool _isApply = false;
                                    foreach (var _item in _Product.ListStoreApply)
                                    {
                                        if (_item.StoreID == item.ID)
                                            _isApply = true;
                                    }
                                    _AssignToStore.ListAssignStore.Add(new AssignStore()
                                    {
                                        Offset = i,
                                        StoreName = item.Name,
                                        StoreID = item.ID,
                                        Address = item.Address,
                                        IsApply = _isApply,
                                        ExpiredDate = item.ExpiredDate,
                                        StoreType = item.StoreType,
                                        Industry = _industry
                                    });
                                    i++;
                                }
                                _AssignToStore.ListAssignStore.ForEach(x =>
                                {
                                    if (x.ActivatedDate.HasValue)
                                        x.ActivatedDate = CommonHelper.ConvertToLocalTime(x.ActivatedDate.Value);
                                    if (x.ExpiredDate.HasValue)
                                        x.ExpiredDate = CommonHelper.ConvertToLocalTime(x.ExpiredDate.Value);
                                });
                                _Product.AssignStore = _AssignToStore;
                            }
                        }
                    }
                    else
                    {
                        if (temp.ProductType == (int)Commons.EProductType.Product)
                        {
                            var _tempDevices = temp.ListDevice.Select(z => new DeviceUserModels
                            {
                                ID = z.ID,
                                UUID = z.UUID,
                                ActiveTime = z.ActiveTime,
                                LicenseKey = z.LicenseKey,
                                ProductType = z.ProductType,
                                IsActive = z.IsActive,
                                ProductID = z.ProductID,
                                AssetID = temp.AssetID
                            }).ToList();

                            temp.ListDevice = _tempDevices;
                            model.Image = string.IsNullOrEmpty(model.Image) ? _ImgDummyProduct : model.Image;
                            if (temp.Category.Type == (int)Commons.EType.NuPOS || temp.Category.Type == (int)Commons.EType.NuKiosk || temp.Category.Type == (int)Commons.EType.NuDisplay)
                                _ListStore = _ListStore.Where(x => x.StoreType == (int)Commons.EStoreType.FnB).ToList();
                            else
                        if (temp.Category.Type == (int)Commons.EType.POZZ || temp.Category.Type == (int)Commons.EType.POZZ_Display || temp.Category.Type == (int)Commons.EType.POZZ_Kiosk)
                                _ListStore = _ListStore.Where(x => x.StoreType == (int)Commons.EStoreType.Beauty).ToList();
                            if (_ListStore != null && _ListStore.Count > 0)
                            {
                                AssignToStore _AssignToStore = new AssignToStore();
                                _AssignToStore.tempAssetID = AssetID;
                                int i = 0;
                                foreach (var item in _ListStore)
                                {
                                    bool _isApply = false;
                                    string _industry = "";
                                    _industry = item.StoreType == 1 ? Commons.StoreTypeFnB : item.StoreType == 2 ? Commons.StoreTypeBeauty : "";
                                    foreach (var _item in temp.ListStoreApply)
                                    {
                                        if (_item.StoreID == item.ID)
                                            _isApply = true;
                                    }
                                    _AssignToStore.ListAssignStore.Add(new AssignStore()
                                    {
                                        Offset = i,
                                        StoreName = item.Name,
                                        StoreID = item.ID,
                                        Address = item.Address,
                                        IsApply = _isApply,
                                        ExpiredDate = item.ExpiredDate,
                                        StoreType = item.StoreType,
                                        Industry = _industry,
                                        ActivatedDate = item.ActivatedDate
                                    });
                                    i++;
                                }
                                _AssignToStore.ListAssignStore.ForEach(x =>
                                {
                                    if (x.ActivatedDate.HasValue)
                                        x.ActivatedDate = CommonHelper.ConvertToLocalTime(x.ActivatedDate.Value);
                                    if (x.ExpiredDate.HasValue)
                                        x.ExpiredDate = CommonHelper.ConvertToLocalTime(x.ExpiredDate.Value);
                                });
                                temp.AssignStore = _AssignToStore;
                            }
                        }
                    }
                    model = temp;
                    model.TabId = TabId;
                    model.IsFree = temp.Category != null ? temp.Category.IsFreeTrial : false;
                    // Set Extend Product
                    model.ListPrices = model.ListPrices != null ? model.ListPrices.Where(ww => ww.IsExtend).ToList() : model.ListPrices;
                    // If Item has already activated && PeriodType != OneTime && List Extend Prices != null => can extend
                    if (model.ActiveTime.HasValue && model.ExpiryDate.HasValue && model.ExpiryDate.Value <= DateTime.Now && model.ExpiryDate.Value.Date != Commons.MinDate.Date
                        && model.PeriodType != (byte)Commons.EPeriodType.OneTime && model.ListPrices != null && model.ListPrices.Any())
                    {
                        model.ApplyExtend = true;
                    }
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("ViewDetail :", ex);
            }
            if (model.ProductType == (int)Commons.EProductType.Product)
            {
                return PartialView("_ItemProductDetail", model);
            }
            else
            {
                return PartialView("_ItemPackageDetail", model);
            }

        }
        public ActionResult AssignStoreToProduct(AssignToStore model)
        {
            try
            {
                if (model.ListAssignStore != null && model.ListAssignStore.Count > 0)
                {
                    List<StoreAssignProduct> _StoreAssignProduct = new List<StoreAssignProduct>();
                    foreach (var item in model.ListAssignStore.Where(x => x.IsApply))
                    {
                        _StoreAssignProduct.Add(new StoreAssignProduct
                        {
                            AssetID = model.tempAssetID,
                            StoreID = item.StoreID
                        });
                    }
                    string mess = "";
                    bool IsStoreAssignProduct = false;
                    var results = _factory.StoreAssignProduct(_StoreAssignProduct, model.tempAssetID, IsStoreAssignProduct, CurrentUser.UserId, ref mess);
                    return Json(results, JsonRequestBehavior.AllowGet);
                }
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("Assign store to Product", ex);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ChangeStatusDevice(string DeviceID = "", string AssetID = "", string ProductID = "")
        {
            var model = new ProductPackageUserModels();
            try
            {
                string msg = "";
                var result = _factory.ChangeStatusDevice(DeviceID, ref msg);
                var temp = _factory.GetProductPackageDetail(AssetID);
                if (temp != null)
                {
                    if (temp.ProductType == (int)Commons.EProductType.Package)
                    {
                        var _tempProducts = temp.ListDevice.Where(x => x.ProductID == ProductID).Select(z => new DeviceUserModels
                        {
                            ID = z.ID,
                            UUID = z.UUID,
                            //ActiveTime = CommonHelper.ConvertToLocalTime(z.ActiveTime.Value),
                            ActiveTime = z.ActiveTime,
                            LicenseKey = z.LicenseKey,
                            ProductType = z.ProductType,
                            IsActive = z.IsActive,
                            ProductID = z.ProductID,
                            AssetID = temp.AssetID
                        }).ToList();
                    }
                    else
                    {
                        var _tempDevices = temp.ListDevice.Select(z => new DeviceUserModels
                        {
                            ID = z.ID,
                            UUID = z.UUID,
                            ActiveTime = z.ActiveTime,
                            LicenseKey = z.LicenseKey,
                            ProductType = z.ProductType,
                            IsActive = z.IsActive,
                            ProductID = z.ProductID,
                            AssetID = temp.AssetID
                        }).ToList();

                        temp.ListDevice = _tempDevices;
                    }
                    model = temp;
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("ChangeStatusDevice : ", ex);
            }
            return PartialView("_ItemDevice", model.ListDevice);
        }

        public ActionResult Back(int TabID)
        {
            //MyStoreAndBusinessViewModels model = new MyStoreAndBusinessViewModels();
            //model.TabProductAndPackage = TabID;
            //return PartialView("_ItemMainProductPackage", model);
            if (TabID == 2)//going
            {
                var model = GetProductOrPackageOnGoing();
                return PartialView("_ItemOnGoing", model);
            }
            else if (TabID == 3)
            {
                var model = GetProductOrPackageExpired();
                return PartialView("_ItemExpired", model);
            }
            else
            {
                MyStoreAndBusinessViewModels model = new MyStoreAndBusinessViewModels();
                model.TabProductAndPackage = TabID;
                return PartialView("_ItemMainProductPackage", model);
            }

        }

        [EncryptedActionParameter]
        public ActionResult Extend(string Id = "", string AssetID = "")
        {
            var model = new ProductPackageUserModels();
            try
            {
                var temp = _factory.GetProductPackageDetail(AssetID);
                var ListPrice = _factory.GetPriceProduct(Id);
                var ArrPeriod = new Dictionary<int, string>();
                ArrPeriod.Add((int)Commons.EPeriodType.Day, Commons.EPeriodType.Day.ToString());
                ArrPeriod.Add((int)Commons.EPeriodType.Month, Commons.EPeriodType.Month.ToString());
                ArrPeriod.Add((int)Commons.EPeriodType.Year, Commons.EPeriodType.Year.ToString());
                ArrPeriod.Add((int)Commons.EPeriodType.OneTime, "One Time");
                if (ListPrice != null)
                {
                    ListPrice = ListPrice.Where(x => x.IsExtend)
                                            .Select(x => new ProductPriceModels()
                                            {
                                                ID = x.ID,
                                                IsExtend = x.IsExtend,
                                                Period = x.Period,
                                                PeriodType = x.PeriodType,
                                                Price = x.Price,
                                                PeriodName = ArrPeriod.ContainsKey(x.PeriodType) ? ArrPeriod.FirstOrDefault(y => y.Key == x.PeriodType).Value : "",
                                                CurrencySymbol = CurrencySymbol
                                            })
                                            .OrderBy(x => x.PeriodType).ThenBy(x => x.Period)
                                            .ToList();
                    if (ListPrice != null && ListPrice.Any())
                    {
                        ListPrice.First().IsSelected = true;
                    }
                }
                ViewBag.AssetIdExtendAppliedOn = "";
                if (temp != null)
                {
                    if (temp.ProductType == (int)Commons.EProductType.Package)
                    {
                        var _tempProducts = temp.ListProduct.Select(x => new ProductUserModels
                        {
                            AssetID = x.AssetID,
                            ItemID = x.ItemID,
                            ItemName = x.ItemName,
                            ProductType = x.ProductType,
                            Image = x.Image,
                            PeriodType = x.PeriodType,
                            //ExpiryDate = x.ExpiryDate.HasValue ? CommonHelper.ConvertToLocalTime(x.ExpiryDate.Value) : x.ExpiryDate,
                            ExpiryDate = x.ExpiryDate,
                            NumberOfUnit = x.NumberOfUnit,
                            NumberOfAccount = x.NumberOfAccount,
                            IsActive = x.IsActive,
                            IsUnlimitedAccount = x.IsUnlimitedAccount,
                            IsUnlimitedUnit = x.IsUnlimitedUnit,
                            ListDevices = temp.ListDevice.Where(y => y.ProductID == x.ItemID).Select(z => new DeviceUserModels
                            {
                                ID = z.ID,
                                UUID = z.UUID,
                                ActiveTime = z.ActiveTime,
                                LicenseKey = z.LicenseKey,
                                ProductType = z.ProductType,
                                IsActive = z.IsActive,
                                ProductID = z.ProductID,
                                AssetID = temp.AssetID
                            }).ToList()
                        }).ToList();
                        var _product = _tempProducts
                                                   .Where(x => x.IsActive)
                                                   .Select(x => new SelectListItem
                                                   {
                                                       Value = x.ItemID + ',' + x.AssetID, // ProductID + AssetID
                                                       Text = x.ItemName
                                                   })
                                                   .ToList();
                        temp.ListItemProduct = _product;
                        temp.ListProduct = _tempProducts;
                        temp.IsProduct = false;

                        temp.Image = string.IsNullOrEmpty(temp.Image) ? _ImgDummyPackage : temp.Image;
                        // Set default Extend Product
                        ViewBag.AssetIdExtendAppliedOn = temp.ListItemProduct.Select(ss => ss.Value).FirstOrDefault();
                    }
                    else
                    {
                        var _tempDevices = temp.ListDevice.Select(z => new DeviceUserModels
                        {
                            ID = z.ID,
                            UUID = z.UUID,
                            ActiveTime = z.ActiveTime,
                            LicenseKey = z.LicenseKey,
                            ProductType = z.ProductType,
                            IsActive = z.IsActive,
                            ProductID = z.ProductID,
                            AssetID = temp.AssetID
                        }).ToList();

                        var _product = new List<SelectListItem>()
                        {
                            new SelectListItem
                            {
                                Value = temp.ItemID + ',' + temp.AssetID, // ProductID + AssetID
                                Text = temp.ItemName
                            }
                        };
                        temp.ListItemProduct = _product;
                        temp.ListDevice = _tempDevices;
                        temp.IsProduct = true;

                        temp.Image = string.IsNullOrEmpty(temp.Image) ? _ImgDummyProduct : temp.Image;
                        // Set default Extend Product
                        ViewBag.AssetIdExtendAppliedOn = temp.AssetID;
                    }
                    temp.ListPrices = ListPrice;
                    model = temp;
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("Extend : ", ex);
            }
            return View("_FormExtend", model);
        }

        #endregion  For Products & Packages

        #region  /* For Accounts & Functions */
        public ActionResult GetListAccountFunction()
        {
            MyStoreAndBusinessViewModels model = new MyStoreAndBusinessViewModels();
            UserGetListProductManagementModels dataHardwareServiceAccountAddition = _factory.GetListHardwareServiceAccountAddition(CurrentUser.UserId);
            //Account
            model.ListAccount = dataHardwareServiceAccountAddition.ListAccount;
            if (model.ListAccount != null && model.ListAccount.Count > 0)
            {
                model.ListAccount.ForEach(x =>
                {
                    if (x.ExpiryDate.HasValue)
                    {
                        x.ExpiryDate = CommonHelper.ConvertToLocalTime(x.ExpiryDate.Value);
                        x.sExpiryDate = x.ExpiryDate.Value.ToString(Commons.FormatDateTime);
                        DateTime _curDate = DateTime.Now;
                        if (x.ExpiryDate > _curDate)
                            x.Status = Commons.EStatusAccountFunction.Expired.ToString();
                        else
                            x.Status = x.IsActive ? Commons.EStatusAccountFunction.Active.ToString() : Commons.EStatusAccountFunction.Inactive.ToString();
                    }
                    else
                        x.Status = Commons.EStatusAccountFunction.Inactive.ToString();
                    //-------------
                    if (x.ActivateTime.HasValue)
                    {
                        x.ActivateTime = CommonHelper.ConvertToLocalTime(x.ActivateTime.Value);
                        x.sActivateTime = x.ActivateTime.Value.ToString(Commons.FormatDateTime);
                    }
                    else
                        x.sActivateTime = "";

                    //if (!x.ExpiryDate.HasValue)
                    //    x.Status = Commons.EStatusAccountFunction.Inactive.ToString();
                    //else
                    //{
                    //    DateTime _curDate = DateTime.Now;
                    //    if (x.ExpiryDate > _curDate)
                    //        x.Status = Commons.EStatusAccountFunction.Expired.ToString();
                    //    else
                    //        x.Status = x.IsActive ? Commons.EStatusAccountFunction.Active.ToString() : Commons.EStatusAccountFunction.Inactive.ToString();
                    //}

                    //=====
                    //if (x.ExpiryDate.HasValue)
                    //    x.sExpiryDate = x.ExpiryDate.Value.ToString(Commons.FormatDateTime);

                    //if (x.ActivateTime.HasValue)
                    //    x.sActivateTime = x.ActivateTime.Value.ToString(Commons.FormatDateTime);
                    //else
                    //    x.sActivateTime = "";

                    if (x.Account == CurrentUser.Email)
                        x.IsSupperAdmin = true;
                    else
                        x.IsSupperAdmin = false;

                    //if (x.ProductType == (int)Commons.EProductType.Addition)
                    //    x.sProductType = "Addition";
                    //else if (x.ProductType == (int)Commons.EProductType.Discount)
                    //    x.sProductType = "Discount";
                    //else if (x.ProductType == (int)Commons.EProductType.Package)
                    //    x.sProductType = "Package";
                    //else if (x.ProductType == (int)Commons.EProductType.Product)
                    //    x.sProductType = "Product";
                    //else if (x.ProductType == (int)Commons.EProductType.Promotion)
                    //    x.sProductType = "Promotion";

                    //update by Trongntn 06-09-2018
                    x.sProductType = ((Commons.EProductType)Enum.ToObject(typeof(Commons.EProductType), x.ProductType)).ToString();
                    //if (x.IsSupperAdmin)
                    //{
                    //    x.sExpiryDate = "Never";
                    //    x.Status = Commons.EStatusAccountFunction.Active.ToString();
                    //}

                });
            }
            //Function
            model.ListFunction = dataHardwareServiceAccountAddition.ListFunction;
            if (model.ListFunction != null && model.ListFunction.Count > 0)
            {
                model.ListFunction.ForEach(x =>
                {
                    if (x.ExpiryDate.HasValue)
                    {
                        x.ExpiryDate = CommonHelper.ConvertToLocalTime(x.ExpiryDate.Value);
                        DateTime _curDate = DateTime.Now;
                        if (x.ExpiryDate < _curDate)
                            x.Status = Commons.EStatusAccountFunction.Expired.ToString();
                        else
                            x.Status = x.IsActive ? Commons.EStatusAccountFunction.Active.ToString() : Commons.EStatusAccountFunction.Inactive.ToString();

                        if (x.PeriodType == (byte)Commons.EPeriodType.OneTime)
                            x.sExpiryDate = "Never";
                        else
                        {
                            if (x.ExpiryDate.HasValue)
                                x.sExpiryDate = x.ExpiryDate.Value.ToString(Commons.FormatDateTime);
                        }
                    }
                    else
                    {
                        x.Status = Commons.EStatusAccountFunction.Inactive.ToString();
                    }
                    if (x.ActivateTime.HasValue)
                    {
                        x.ActivateTime = CommonHelper.ConvertToLocalTime(x.ActivateTime.Value);
                        x.sActivateTime = x.ActivateTime.Value.ToString(Commons.FormatDateTime);
                    }
                    else
                        x.sActivateTime = "";
                    if (x.IsBlock)
                        x.Status = Commons.EStatusAccountFunction.Blocked.ToString();
                    //else if (!x.ExpiryDate.HasValue)
                    //    x.Status = Commons.EStatusAccountFunction.Inactive.ToString();
                    //else
                    //{
                    //    DateTime _curDate = DateTime.Now;
                    //    if (x.ExpiryDate > _curDate)
                    //        x.Status = Commons.EStatusAccountFunction.Expired.ToString();
                    //    else
                    //        x.Status = x.IsActive ? Commons.EStatusAccountFunction.Active.ToString() : Commons.EStatusAccountFunction.Inactive.ToString();

                    //    if (x.PeriodType == (byte)Commons.EPeriodType.OneTime)
                    //        x.sExpiryDate = "Never";
                    //    else
                    //    {
                    //        if (x.ExpiryDate.HasValue)
                    //            x.sExpiryDate = x.ExpiryDate.Value.ToString(Commons.FormatDateTime);
                    //    }
                    //}

                    //=====
                    //if (x.ActivateTime.HasValue)
                    //    x.sActivateTime = x.ActivateTime.Value.ToString(Commons.FormatDateTime);
                    //else
                    //    x.sActivateTime = "";
                });
            }
            return PartialView(/*"_FormAccountFuction"*/"_ListAccountsFunctions", model);
        }

        public ActionResult ChangeStatusAccountFuction(string type, string ID)
        {
            bool success = false;
            string msg = "";
            try
            {
                success = _factory.ChangeStatusFunctionAccount(type, ID, ref msg);
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("ChangeStatusAccountFuction : ", ex);
            }
            //=========
            if (success)
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            else
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, msg);
        }

        #endregion  /* For Accounts & Functions */

        public ActionResult GetTimeZones(string _Country)
        {

            ViewBag.TimeZones = "";
            if (_Country != "")
            {
                var Country = _baseFactory.GetListCountry();
                for (int i = 0; i < Country.Count; i++)
                {
                    if (Country[i].Name == _Country)
                    {
                        ViewBag.TimeZones = Country[i].TimeZones;
                    }
                }
            }         //=======
            return PartialView("_TimeZones");
        }

        private List<ProductPackageUserModels> GetProductOrPackageOnGoing()
        {
            var model = new List<ProductPackageUserModels>();
            try
            {
                UserGetListProductManagementModels dataHardwareServiceAccountAddition = _factory.GetListHardwareServiceAccountAddition(CurrentUser.UserId);
                if (dataHardwareServiceAccountAddition != null)
                {
                    dataHardwareServiceAccountAddition.ListProductPackage.ForEach(x => x.ExpiryDate = CommonHelper.ConvertToLocalTime(x.ExpiryDate.Value));
                    model = dataHardwareServiceAccountAddition.ListProductPackage.Where(x => x.ExpiryDate.HasValue && x.ExpiryDate.Value > DateTime.Now
                    || x.ExpiryDate.Value.Date == Commons.MinDate.Date).Select(x => x).ToList();
                    model.ForEach(x =>
                    {
                        x.TabId = 2;
                        x.Image = string.IsNullOrEmpty(x.Image) ? ((x.ProductType == (int)Commons.EProductType.Product) ? _ImgDummyProduct : _ImgDummyPackage) : x.Image;
                        x.ListPrices = x.ListPrices.Where(ww => ww.IsExtend).ToList();

                        // If Item has already activated && PeriodType != OneTime && List Extend Prices != null => can extend
                        if (x.ActiveTime.HasValue && x.ExpiryDate.HasValue && x.ExpiryDate.Value.Date != Commons.MinDate.Date
                            && x.PeriodType != (byte)Commons.EPeriodType.OneTime && x.ListPrices != null && x.ListPrices.Any())
                            x.ApplyExtend = true;

                        if (x.Category != null)
                            x.IsFree = x.Category.IsFreeTrial;
                        //|----updated code
                        x.PeriodName = CommonHelper.PeriodOfTheProduct(x.Period, x.PeriodType);
                        //-----ExpiryDate
                        if (x.ActiveTime.HasValue)
                        {
                            if (x.ExpiryDate.Value.ToString(Commons.FormatDate).Equals(Commons.MaxDate.ToString(Commons.FormatDate)))
                                x.sExpiryDate = "No expiry date";
                            else
                                x.sExpiryDate = "Expired on: " + (string.Format("{0}", x.ExpiryDate.HasValue ? x.ExpiryDate.Value.ToString(Commons.FormatDateTime) : ""));
                        }
                        else
                            x.sExpiryDate = "Not yet activated";
                        //-----IsUnlimitedUnit
                        if (x.IsUnlimitedUnit)
                            x.sIsUnlimitedUnit = "Product key can be applied on unlimited devices";
                        else
                            x.sIsUnlimitedUnit = "Product key can be applied on " + x.NumberOfUnit + " user device(s).";
                        //-----IsUnlimitedAccount
                        string description = "";
                        if (x.ProductType == (int)Commons.EProductType.Package)
                            description = "each product in this package";
                        else if (x.ProductType == (int)Commons.EProductType.Product)
                            description = "the product";
                        if (x.IsUnlimitedAccount)
                            x.sIsUnlimitedAccount = "License key of " + description + " can be used to activate unlimited user accounts.";
                        else
                            x.sIsUnlimitedAccount = "License key of " + description + " can be used to activate " + x.NumberOfAccount + " user account(s).";
                        //-----Functions
                        if (x.IsAllFunction)
                            x.sFunctions = "All functions included";
                        else
                            x.sFunctions = string.Join(", ", x.ListFunction.Select(z => z.FunctionName));
                        //-----List Item(Product)
                        if (x.ProductType == (int)Commons.EProductType.Package)
                        {
                            x.ListProduct.ForEach(z =>
                            {
                                z.Image = string.IsNullOrEmpty(z.Image) ? _ImgDummyProduct : z.Image;
                                //-----ExpiryDate
                                if (z.ActiveTime.HasValue)
                                {
                                    if (z.ExpiryDate.Value.ToString(Commons.FormatDate).Equals(Commons.MaxDate.ToString(Commons.FormatDate)))
                                        z.sExpiryDate = "No expiry date";
                                    else
                                        z.sExpiryDate = "Expired on: " + (string.Format("<span style='color:#e54609'>{0}</span>",
                                                                    z.ExpiryDate.HasValue ? z.ExpiryDate.Value.ToString(Commons.FormatDateTime) : ""));
                                }
                                else
                                    z.sExpiryDate = "Not yet activated";
                            });
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("Load ON GOING Error", ex);
            }
            return model;
        }

        private List<ProductPackageUserModels> GetProductOrPackageExpired()
        {
            var model = new List<ProductPackageUserModels>();
            try
            {
                UserGetListProductManagementModels dataHardwareServiceAccountAddition = _factory.GetListHardwareServiceAccountAddition(CurrentUser.UserId);
                if (dataHardwareServiceAccountAddition != null)
                {
                    dataHardwareServiceAccountAddition.ListProductPackage.ForEach(x => x.ExpiryDate = CommonHelper.ConvertToLocalTime(x.ExpiryDate.Value));
                    model = dataHardwareServiceAccountAddition.ListProductPackage.Where(x => x.ActiveTime.HasValue && x.ExpiryDate.HasValue && x.ExpiryDate.Value <= DateTime.Now && x.ExpiryDate.Value.Date != Commons.MinDate.Date).Select(x => x).ToList();
                    model.ForEach(x =>
                    {
                        x.TabId = 3;
                        x.Image = string.IsNullOrEmpty(x.Image) ? ((x.ProductType == (int)Commons.EProductType.Product) ? _ImgDummyProduct : _ImgDummyPackage) : x.Image;
                        x.ListPrices = x.ListPrices.Where(ww => ww.IsExtend).ToList();

                        // PeriodType != OneTime && List Extend Prices != null => can extend
                        if (x.PeriodType != (byte)Commons.EPeriodType.OneTime && x.ListPrices != null && x.ListPrices.Any())
                            x.ApplyExtend = true;

                        //----updated code
                        x.PeriodName = CommonHelper.PeriodOfTheProduct(x.Period, x.PeriodType);
                        //-----ExpiryDate
                        if (x.IsActive)
                            x.sExpiryDate = string.Format("{0}", x.ExpiryDate.HasValue ? x.ExpiryDate.Value.ToString(Commons.FormatDateTime) : "");
                        else
                            x.sExpiryDate = "Not yet activated";
                        //-----IsUnlimitedUnit
                        if (x.IsUnlimitedUnit)
                            x.sIsUnlimitedUnit = "Product key can be applied on unlimited devices";
                        else
                            x.sIsUnlimitedUnit = "Product key can be applied on " + x.NumberOfUnit + " user device(s).";
                        //-----IsUnlimitedAccount
                        string description = "";
                        if (x.ProductType == (int)Commons.EProductType.Package)
                            description = "each product in this package";
                        else if (x.ProductType == (int)Commons.EProductType.Product)
                            description = "the product";
                        if (x.IsUnlimitedAccount)
                            x.sIsUnlimitedAccount = "License key of " + description + " can be used to activate unlimited user accounts.";
                        else
                            x.sIsUnlimitedAccount = "License key of " + description + " can be used to activate " + x.NumberOfAccount + " user account(s).";
                        //-----Functions
                        if (x.IsAllFunction)
                            x.sFunctions = "All functions included";
                        else
                            x.sFunctions = string.Join(", ", x.ListFunction.Select(z => z.FunctionName));
                        //-----List Item(Product)
                        if (x.ProductType == (int)Commons.EProductType.Package)
                        {
                            x.ListProduct.ForEach(z =>
                            {
                                z.Image = string.IsNullOrEmpty(z.Image) ? _ImgDummyProduct : z.Image;
                                //-----ExpiryDate
                                if (z.IsActive)
                                    z.sExpiryDate = string.Format("<span style='color:#e54609'>{0}</span>",
                                                    z.ExpiryDate.HasValue ? z.ExpiryDate.Value.ToString(Commons.FormatDateTime) : "");
                                else
                                    z.sExpiryDate = "Not yet activated";
                            });
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("Load EXPIRED", ex);
            }
            return model;
        }
        public ActionResult CheckOutItem(string AssetId, string Period, string PeriodType, string Price, string ProductAppliOn)
        {
            var models = new OrderDetailModels();
            try
            {
                CustomerFactory _factory = new CustomerFactory();
                string CustomerID = CurrentUser.UserId;
                double quan = 0;
                bool IsExit = false;
                var Cookie = Request.Cookies["csc-order-v2"];
                if (Cookie != null)
                {
                    var Order = Cookie.Value;
                    var strOrder = Server.UrlDecode(Order);
                    var ListOrder = JsonConvert.DeserializeObject<List<CookieOrder>>(strOrder);
                    if (ListOrder != null)
                    {
                        var temp = ListOrder.FirstOrDefault(x => x.OrderId.Equals(ORDERID));
                        if (temp == null)
                        {
                            ORDERID = null;
                        }
                        models = _factory.GetOrderDataProducts(CustomerID, AssetId, Period, PeriodType, Price, ProductAppliOn, ORDERID);
                        quan = models.ListItems[0].Quantity;
                        ORDERID = models.ID;
                    }
                }
                string str = "{'IsExit':'" + IsExit + "', 'CusId':'" + CurrentUser.UserId + "', 'OrderId':'" + ORDERID + "', 'Qty':'" + quan + "'}";
                JavaScriptSerializer jsSer = new JavaScriptSerializer();
                object obj = jsSer.Deserialize(str, typeof(object));
                return Json(obj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("CheckoutProduct", ex);
                return new HttpStatusCodeResult(400, "");
            }
            //return PartialView("_ItemProductCheckOut", models);
        }
    }
}