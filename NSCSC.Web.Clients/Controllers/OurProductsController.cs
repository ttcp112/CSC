using Newtonsoft.Json;
using NSCSC.Shared;
using NSCSC.Shared.Factory.ClientSite;
using NSCSC.Shared.Factory.ClientSite.OurProducts;
using NSCSC.Shared.Models.ClientSite.MyProfile;
using NSCSC.Shared.Models.ClientSite.OurProDucts;
using NSCSC.Shared.Models.ClientSite.YourCart;
using NSCSC.Shared.Models.OurProducts;
using NSCSC.Shared.Models.OurProducts.Addition;
using NSCSC.Shared.Models.OurProducts.Package;
using NSCSC.Shared.Models.OurProducts.Price;
using NSCSC.Shared.Models.OurProducts.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using NSCSC.Shared.Models.SandBox.Inventory.Product;
using NSCSC.Shared.Filters;
using NSCSC.Shared.Helpers;

namespace NSCSC.Web.Clients.Controllers
{
    public class OurProductsController : ClientController
    {
        private OurProductFactory _factory = null;
        private YourCartFactory _yourcartFactory = null;

        public OurProductsController()
        {
            _factory = new OurProductFactory();
            _yourcartFactory = new YourCartFactory();
            ViewBag.InPage = true;
        }

        [EncryptedActionParameter]
        // GET: OurProducts
        public ActionResult Index(string ProductType, string CategoryID = null)
        {
            if (ProductType == null) {
                return RedirectToAction("Index", "Home");
            }
            try
            {
                OurProductsCategoryModels model = new OurProductsCategoryModels();
                // Get value for category information
                // If this is a Product category 
                if (ProductType == "Products")
                {
                    //List<CategoryDetailModels> lstCategories = _factory.ProductGetCate(CategoryID);
                    //var categoryDetail = lstCategories.Where(w => w.ID == CategoryID).FirstOrDefault();

                    var categoryDetail = _factory.ProductGetCate(CategoryID).FirstOrDefault();
                    if (categoryDetail != null)
                    {
                        model.ID = CategoryID;
                        model.ProductType = ProductType;
                        model.Name = categoryDetail.Name;
                        model.ImageURL = string.IsNullOrEmpty(categoryDetail.ImageURL) ? _ImgDummyProduct : categoryDetail.ImageURL;
                        model.VideoUrl = categoryDetail.VideoUrl;
                        model.Description = categoryDetail.Description;
                        model.IsFreeTrial = categoryDetail.IsFreeTrial;
                        // Get list promotions
                        model.ListPromotions = _factory.GetPromotion((byte)categoryDetail.Type, false);
                    }
                }
                // If this is a Package
                else if (ProductType == "Packages")
                {
                    var data = ClientPackageSession;
                    // Data text
                    model.ProductType = ProductType;
                    model.Name = data.Name;
                    if (!string.IsNullOrEmpty(data.ImageUrl))
                    {
                        model.ImageURL = data.ImageUrl;
                    }
                    if (!string.IsNullOrEmpty(data.VideoUrl))
                    {
                        model.VideoUrl = data.VideoUrl;
                    }
                    model.Description = data.Description;
                    // Get list promotions
                    model.ListPromotions = _factory.GetPromotion(0, true);
                }
                //======
                model.ListPromotions.ForEach(x =>
                {
                    x.ImageURL = string.IsNullOrEmpty(x.ImageURL) ? _ImgDummyProduct : x.ImageURL;
                });
                return View(model);
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("Index OurProducts Error: ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }

        #region Product Category
        public List<OurProDuctsModel> GetListProductByCategory(List<string> ListCateID = null)
        {
            var models = new List<OurProDuctsModel>();
            try
            {
                models = _factory.GetListData(CurrentUser.UserId, null, (byte)Commons.EProductType.Product, ListCateID, null);
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("GetListProductByCategory Error: ", ex);
            }
            return models;
        }

        [EncryptedActionParameter]
        public ActionResult Products(string CategoryID = "")
        {
            OurProductListViewModels model = new OurProductListViewModels();
            model.CategoryID = CategoryID;
            try
            {
                var ProductType = (int)Commons.EProductType.Product;
                var PriceList = _factory.GetListPrice(ProductType, 0, CategoryID);
                var PeriodListTemp = new List<PeriodModels>();
                var ArrPeriod = new Dictionary<int, string>();
                ArrPeriod.Add((int)Commons.EPeriodType.Day, Commons.EPeriodType.Day.ToString());
                ArrPeriod.Add((int)Commons.EPeriodType.Month, Commons.EPeriodType.Month.ToString());
                ArrPeriod.Add((int)Commons.EPeriodType.Year, Commons.EPeriodType.Year.ToString());
                ArrPeriod.Add((int)Commons.EPeriodType.OneTime, Commons.EPeriodType.OneTime.ToString());
                if (PriceList != null)
                {
                    PeriodListTemp = PriceList.ListPeriod.Select(x => new PeriodModels()
                    {
                        Period = x.Period,
                        PeriodType = x.PeriodType,
                        Price = x.Price,
                        PeriodName = ArrPeriod.ContainsKey(x.PeriodType) ? ArrPeriod.Where(y => y.Key == x.PeriodType).FirstOrDefault().Value : "-"
                    }).ToList();

                    PriceList.ListPeriod = PeriodListTemp;
                    model.PriceList = PriceList;
                }
                var ListCateID = new List<string>();
                ListCateID.Add(CategoryID);
                model.IsReCommen = GetListProductByCategory(ListCateID).Any(x => x.ProductType == (int)Commons.EProductType.Package);
                List<CategoryDetailModels> listCate = (List<CategoryDetailModels>)Session["ListItemCategory"];
                if (listCate != null)
                {
                    var CateDetail = listCate.Where(x => x.ID.Equals(model.CategoryID)).FirstOrDefault();
                    model.Name = CateDetail == null ? "" : CateDetail.Name;
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("Products : ", ex);
            }
            return View(model);
        }

        public OurProDuctsModel GetProductByProductID(string ProductID = "")
        {
            var model = new OurProDuctsModel();
            try
            {
                var ListProductID = new List<string>();
                ListProductID.Add(ProductID);
                var models = _factory.GetListData(CurrentUser.UserId, null, 0, null, ListProductID);
                if (models != null)
                {
                    model = models.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("GetProductByProductID : ", ex);
            }
            return model;
        }

        [EncryptedActionParameter]
        public ActionResult LoadProducts(string CategoryID = "")
        {
            ViewBag.CategoryId = CategoryID;
            var models = new List<OurProDuctsModel>();
            try
            {
                var ListCateID = new List<string>();
                ListCateID.Add(CategoryID);
                models = GetListProductByCategory(ListCateID);
                models = models.Where(x => x.ProductType == (int)Commons.EProductType.Product)
                                            .Select(x => new OurProDuctsModel
                                            {
                                                Index = x.Index,
                                                ID = x.ID,
                                                Type = x.Type,
                                                ProductType = x.ProductType,
                                                AdditionType = x.AdditionType,
                                                Sequence = x.Sequence,
                                                Name = x.Name,
                                                ImageURL = string.IsNullOrEmpty(x.ImageURL) ? _ImgDummyProduct : x.ImageURL,
                                                Code = x.Code,
                                                AmountOfUnit = x.AmountOfUnit,
                                                IsUnlimitedAmountOfUnit = x.IsUnlimitedAmountOfUnit,
                                                SaleFrom = x.SaleFrom,
                                                SaleTo = x.SaleTo,
                                                IsActive = x.IsActive,
                                                IsPublic = x.IsPublic,
                                                IsExtend = x.IsExtend,
                                                IncludeStore = x.IncludeStore,
                                                IsUnlimitedIncludeStore = x.IsUnlimitedIncludeStore,
                                                NumberOfAccount = x.NumberOfAccount,
                                                IsUnlimitedNumberOfAccount = x.IsUnlimitedNumberOfAccount,
                                                IsDisplayWeb = x.IsDisplayWeb,
                                                IsIntegrate = x.IsIntegrate,
                                                IsIncludeLocalServer = x.IsIncludeLocalServer,
                                                IsIncludeCloudServer = x.IsIncludeCloudServer,
                                                Description = x.Description,
                                                ListComposite = x.ListComposite,
                                                ListFunction = x.ListFunction,
                                                ListPrice = x.ListPrice,
                                                ListProducts = x.ListProducts,
                                                // Price = x.ListPrice != null ? x.ListPrice.Min(y => y.Price) : 0,
                                                CurrencySymbol = CurrencySymbol
                                            }).ToList();
                //update by trongntn  23-11-2017
                models.ForEach(x =>
                {
                    if (x.IsUnlimitedAmountOfUnit)
                        x.sUnlimitedAmountOfUnit = "Product key can be applied on unlimited devices";
                    else
                        x.sUnlimitedAmountOfUnit = "Product key can be applied on " + x.AmountOfUnit + " device(s)";

                    if (x.IsUnlimitedNumberOfAccount)
                        x.sUnlimitedNumberOfAccount = "License key of the product can be used to activate unlimited user account(s)";
                    else
                        x.sUnlimitedNumberOfAccount = "License key of the product can be used to activate " + x.NumberOfAccount + " user account(s)";
                    if (x.ListPrice != null && x.ListPrice.Count() > 0)
                    {
                        var PriceMin = x.ListPrice.Where(w => !w.IsExtend).OrderBy(w => w.PeriodType).ThenBy(w => w.Period).ThenBy(w => w.Price).First();
                        x.Price = PriceMin.Price;
                    }
                });
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("LoadProducts : ", ex);
            }
            return PartialView("_FormProductList", models);
        }

        [EncryptedActionParameter]
        public ActionResult LoadPackages(string CategoryID = "")
        {
            ViewBag.CategoryId = CategoryID;
            var models = new List<OurProDuctsModel>();
            try
            {
                var ListCateID = new List<string>();
                ListCateID.Add(CategoryID);
                models = GetListProductByCategory(ListCateID);
                models = models.Where(x => x.ProductType == (int)Commons.EProductType.Package)
                                             .Select(x => new OurProDuctsModel
                                             {
                                                 Index = x.Index,
                                                 ID = x.ID,
                                                 Type = x.Type,
                                                 ProductType = x.ProductType,
                                                 AdditionType = x.AdditionType,
                                                 Sequence = x.Sequence,
                                                 Name = x.Name,
                                                 ImageURL = string.IsNullOrEmpty(x.ImageURL) ? _ImgDummyProduct : x.ImageURL,
                                                 Code = x.Code,
                                                 AmountOfUnit = x.AmountOfUnit,
                                                 IsUnlimitedAmountOfUnit = x.IsUnlimitedAmountOfUnit,
                                                 SaleFrom = x.SaleFrom,
                                                 SaleTo = x.SaleTo,
                                                 IsActive = x.IsActive,
                                                 IsPublic = x.IsPublic,
                                                 IsExtend = x.IsExtend,
                                                 IncludeStore = x.IncludeStore,
                                                 IsUnlimitedIncludeStore = x.IsUnlimitedIncludeStore,
                                                 NumberOfAccount = x.NumberOfAccount,
                                                 IsUnlimitedNumberOfAccount = x.IsUnlimitedNumberOfAccount,
                                                 IsDisplayWeb = x.IsDisplayWeb,
                                                 IsIntegrate = x.IsIntegrate,
                                                 IsIncludeLocalServer = x.IsIncludeLocalServer,
                                                 IsIncludeCloudServer = x.IsIncludeCloudServer,
                                                 Description = x.Description,
                                                 ListComposite = x.ListComposite,
                                                 ListFunction = x.ListFunction,
                                                 ListPrice = x.ListPrice,
                                                 ListProducts = x.ListProducts,
                                                 //Price = x.ListPrice != null ? x.ListPrice.Min(y => y.Price) : 0,

                                                 CurrencySymbol = CurrencySymbol,
                                                 //update by trongntn  23-11-2017
                                                 ListProductChild = x.ListProductChild,
                                                 ListProductOnPackage = x.ListProductOnPackage
                                             }).ToList();
                //update by trongntn  23-11-2017
                models.ForEach(x =>
                {
                    var _tempcomposite = x.ListComposite;
                    if (x.ListProductChild != null)
                    {
                        x.ListProductChild = x.ListProductChild.Where(o => o.ProductType == (byte)Commons.EProductType.Product).ToList();
                        x.ListProductChild.ForEach(z =>
                        {
                            if (z.ImageURL == null || z.ImageURL.Equals(""))
                                z.ImageURL = _ImgDummyProduct;

                            if (z.IsUnlimitedAmountOfUnit)
                                z.sUnlimitedAmountOfUnit = "Product key can be applied on unlimited devices";
                            else
                                z.sUnlimitedAmountOfUnit = "Product key can be applied on " + z.AmountOfUnit + " device(s)";

                            if (z.IsUnlimitedNumberOfAccount)
                                z.sUnlimitedNumberOfAccount = "License key of the product can be used to activate unlimited user account(s)";
                            else
                                z.sUnlimitedNumberOfAccount = "License key of the product can be used to activate " + z.NumberOfAccount + " user account(s)";

                            //==============
                            if (z.IsFullFunction)
                                z.sFullFunction = "All functions included.";
                            else
                                z.sFullFunction = string.Join(", ", z.ListFunction.OrderBy(o => o.FunctionName).Select(o => o.FunctionName).ToList());

                        });

                        if (x.ListProductOnPackage != null)
                        {
                            x.ListProductOnPackage.ForEach(z =>
                            {
                                if (z.Quantity == -1)
                                    z.ProductName = z.ProductName + " (For unlimited devices)";
                                else
                                    z.ProductName = z.ProductName + " (For " + z.Quantity + " device" + (z.Quantity > 1 ? "s" : "") + ")";
                            });
                        }
                    }
                    if (x.ListPrice != null && x.ListPrice.Count() > 0)
                    {
                        var PriceMin = x.ListPrice.Where(w => !w.IsExtend).OrderBy(w => w.PeriodType).ThenBy(w => w.Period).ThenBy(w => w.Price).First();
                        x.Price = PriceMin.Price;
                    }

                    if (_tempcomposite != null)
                    {
                        x.ListComposite = _tempcomposite.Where(z => (z.AdditionType == (int)Commons.EAdditionType.Hardware || z.AdditionType == (int)Commons.EAdditionType.Service) && z.ProductType == (int)Commons.EProductType.Addition).ToList();
                    }

                    if (x.ListProducts != null)
                    {
                        var _product = _tempcomposite.Where(z => z.ProductType == (int)Commons.EProductType.Product).ToList();
                        x.ListProductOnPackage.AddRange(_product);
                    }
                    else
                    {
                        var _product = _tempcomposite.Where(z => z.ProductType == (int)Commons.EProductType.Product).ToList();
                        x.ListProductOnPackage = _product;
                    }
                });
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("LoadPackages :", ex);
            }
            return PartialView("_ItemPackages", models);
        }
        #endregion End Product Category

        #region Product
        public ActionResult ProductDetail(string id, string period)
        {
            ProductDetailViewModels model = new ProductDetailViewModels();
            var ListProduct = _factory.GetListData(CurrentUser.UserId, null, (byte)Commons.EProductType.Product, null, null, id);
            if (ListProduct != null && ListProduct.Count > 0)
            {
                //=====Test BuyingAddition
                List<BuyingAdditionModels> listAdditionItem = new List<BuyingAdditionModels>();
                if (Session["_ListBuyingAdditionItem"] != null)
                    listAdditionItem = (List<BuyingAdditionModels>)Session["_ListBuyingAdditionItem"];
                listAdditionItem = listAdditionItem.Where(x => x.ID.Equals(id)).ToList();
                //============

                model.ProductDetail = ListProduct.FirstOrDefault();
                model.ProductDetail.Quantity = 1;
                if (model.ProductDetail.IsUnlimitedAmountOfUnit)
                    model.ProductDetail.AmountOfUnit = -1;

                if (model.ProductDetail.IsUnlimitedNumberOfAccount)
                    model.ProductDetail.NumberOfAccount = -1;

                if (model.ProductDetail.IsUnlimitedIncludeStore)
                {
                    model.ProductDetail.NumOfStore = -1;
                    model.ProductDetail.IncludeStore = -1;
                }
                else
                    model.ProductDetail.NumOfStore = model.ProductDetail.IncludeStore;

                model.ProductDetail.ListComposite = model.ProductDetail.ListComposite.OrderByDescending(x => x.Sequence).ToList();
                if (model.ProductDetail.ListProductChild != null)
                {
                    int OffSet = 0;
                    model.ProductDetail.ListProductChild.ForEach(x =>
                    {
                        x.Index = OffSet++;
                    });
                }
                if (model.ProductDetail.ImageURL == null || model.ProductDetail.ImageURL.Equals(""))
                    model.ProductDetail.ImageURL = _ImgDummyProduct;

                if (model.ProductDetail.ListPrice != null && model.ProductDetail.ListPrice.Count > 0)
                {
                    model.ProductDetail.ListPrice.ForEach(x =>
                    {
                        x.NamePeriodType = PeriodName(x.Period, x.PeriodType);
                    });

                    var PriceMin = model.ProductDetail.ListPrice.Where(w => !w.IsExtend).OrderBy(x => x.PeriodType).ThenBy(w => w.Period).ThenBy(w => w.Price).FirstOrDefault();

                    model.ProductDetail.TotalPrice = PriceMin.Price;
                    model.ProductDetail.Price = PriceMin.Price;
                    if (PriceMin.PeriodType != (int)Commons.EPeriodType.OneTime)
                        model.ProductDetail.PeriodTime = PriceMin.Period + " ";
                    model.ProductDetail.PeriodTime += PriceMin.NamePeriodType;

                    model.ProductDetail.Period = (int)PriceMin.Period;
                    model.ProductDetail.PeriodType = PriceMin.PeriodType;
                    //========
                    string PriceID = PriceMin.ID;
                    model.ProductDetail.ListPrice.ForEach(x =>
                    {
                        x.IsActive = x.ID.Equals(PriceID) ? true : false;
                    });
                }
                //BuyingAddition
                model.ProductDetail.ListBuyingAddition = listAdditionItem;
                model.ProductDetail.AdditionPrice = listAdditionItem.Sum(x => x.Total);
                model.ProductDetail.CurrencySymbol = CurrencySymbol;
                //=======
                model.ProductDetail.CusId = CurrentUser.UserId;
                model.ProductDetail.CategoryId = model.ProductDetail != null && model.ProductDetail.ListCategory != null && model.ProductDetail.ListCategory.Count > 0 ? model.ProductDetail.ListCategory.FirstOrDefault().CategoryID : "";
                model.Name = model.ProductDetail.ListCategory.FirstOrDefault().CategoryName;
            }
            return View("_ProductDetail", model);
        }

        public ActionResult BuyingAdditon(string id)
        {
            ProductDetailViewModels model = new ProductDetailViewModels();
            var ListProduct = _factory.GetListData(CurrentUser.UserId, null, (byte)Commons.EProductType.Product, null, null, id);
            if (ListProduct != null && ListProduct.Count > 0)
            {
                model.ProductDetail.ID = ListProduct.FirstOrDefault().ID;
                model.ProductDetail.Name = ListProduct.FirstOrDefault().Name;

            }
            if (model.ProductDetail != null)
            {
                List<BuyingAdditionModels> listAdditionItem = new List<BuyingAdditionModels>();
                if (Session["_ListBuyingAdditionItem"] != null)
                    listAdditionItem = (List<BuyingAdditionModels>)Session["_ListBuyingAdditionItem"];
                listAdditionItem = listAdditionItem.Where(x => x.ID.Equals(id)).ToList();
                foreach (var item in listAdditionItem)
                {
                    model.ProductDetail.ListProductChild.Add(new OurProDuctsModel
                    {
                        Index = item.OffSet,
                        Status = (!item.IsDelete ? (byte)Commons.EStatus.Actived : (byte)Commons.EStatus.Deleted),
                        AdditionTypeName = item.AdditionTypeName,
                        Name = item.ProductName,
                        Quantity = item.Quantity,
                        Code = item.AppliedOn,
                        PeriodTime = item.PeriodTime,
                        Price = item.Price,
                        Description = item.ID,
                        ID = item.ID,
                        AdditionType = item.AdditionType,
                        PeriodType = item.PeriodType,
                        Period = item.Period,
                        IsDelete = item.IsDelete,
                        IsActive = item.IsDelete,
                        CurrencySymbol = CurrencySymbol,
                        ProductType = (byte)Commons.EProductType.Addition,
                        TotalPrice = item.Price * item.Quantity,

                    });
                }
                model.ProductDetail.AdditionPrice = listAdditionItem.Sum(x => x.Total);
            }
            model.ProductDetail.CurrencySymbol = CurrencySymbol;
            model.ProductDetail.CategoryId = ListProduct.FirstOrDefault().ListCategory.Select(x => x.CategoryID).FirstOrDefault();
            model.Name = ListProduct.FirstOrDefault().ListCategory.Select(x => x.CategoryName).FirstOrDefault();

            //==========Get List Addition Type
            List<SelectListItem> ListAdditionType = new List<SelectListItem>();
            ListAdditionType = GetListAdditionType();
            ViewBag.ListAdditionType = ListAdditionType;
            return View("_BuyingAddition", model);
        }

        //public ActionResult CallModalBuyingAddition(string ProductID)
        //{
        //    ProductDetailViewModels model = new ProductDetailViewModels();

        //    var ListProduct = _factory.GetListData(null, (byte)Commons.EProductType.Product, null, null, ProductID);
        //    if (ListProduct != null && ListProduct.Count > 0)
        //    {
        //        model.ProductDetail = ListProduct.FirstOrDefault();
        //        //=======Get List Addition
        //        if (model.ProductDetail != null)
        //        {
        //            if (model.ProductDetail.ListProductChild != null)
        //                model.ProductDetail.ListProductChild = model.ProductDetail.ListProductChild.Where(x => x.ProductType == (byte)Commons.EProductType.Addition).ToList();
        //        }

        //        model.ProductDetail.CurrencySymbol = CurrencySymbol;
        //    }
        //    //==========Get List Addition Type
        //    List<SelectListItem> ListAdditionType = new List<SelectListItem>();
        //    ListAdditionType = GetListAdditionType();
        //    ViewBag.ListAdditionType = ListAdditionType;
        //    return PartialView("_ModalBuyingAdditionProduct", model);
        //}

        public ActionResult GetListAdditionForProduct(int AdditionType)
        {
            var ListProduct = _factory.GetListData(CurrentUser.UserId, null, (byte)Commons.EProductType.Addition, null, null, null);
            if (AdditionType != 0)
                ListProduct = ListProduct.Where(x => x.AdditionType == AdditionType).ToList();
            var gData = ListProduct.GroupBy(x => x.AdditionType).Select(x => new BuyingAdditionViewModels
            {
                ID = x.Key,
                ListItem = x.ToList()
            }).ToList();
            gData.ForEach(x =>
            {
                x.Name = ((Commons.EAdditionType)Enum.ToObject(typeof(Commons.EAdditionType), x.ID)).ToString();
                int OffSet = 0;
                x.ListItem.ForEach(z =>
                {
                    z.Index = OffSet++;
                    //================
                    z.ListPrice.ForEach(o =>
                    {
                        o.NamePeriodType = PeriodName(z.Period, z.PeriodType);
                    });
                    //For Period = 1
                    z.IsDisplayWeb = true;
                    if (z.ListPrice.Count == 1)
                        z.IsDisplayWeb = false;
                    //================
                    var PriceMin = z.ListPrice.OrderBy(o => o.PeriodType).ThenBy(w => w.Period).ThenBy(w => w.Price).FirstOrDefault();
                    if (PriceMin != null)
                    {
                        if (PriceMin.PeriodType != (int)Commons.EPeriodType.OneTime)
                            z.PeriodTime = PriceMin.Period + " ";
                        z.PeriodTime += PeriodName(PriceMin.Period, PriceMin.PeriodType); //PriceMin.Period + " " + PriceMin.NamePeriodType;
                        z.Price = PriceMin.Price;
                        z.Period = (int)PriceMin.Period;
                        z.PeriodType = PriceMin.PeriodType;
                        string PriceID = PriceMin.ID;
                        z.ListPrice.ForEach(o =>
                        {
                            o.IsActive = o.ID.Equals(PriceID) ? true : false;
                            if (o.PeriodType == (byte)Commons.EPeriodType.OneTime)
                                o.NamePeriodType = Commons.PeriodTypeOneTime;
                            else
                                o.NamePeriodType = o.Period + " " + ((Commons.EPeriodType)Enum.ToObject(typeof(Commons.EPeriodType), o.PeriodType)).ToString() + (o.Period > 1 ? "s" : "");
                        });
                    }

                    z.CurrencySymbol = CurrencySymbol;
                });
                x.ListItem = x.ListItem.OrderByDescending(o => o.Price).ThenBy(o => o.Price).ToList();
            });
            gData = gData.OrderBy(x => x.Name).ToList();
            return PartialView("_ProductFilterAddition", gData);
        }

        public ActionResult AddItemAddition(OurProDuctsModel data)
        {
            OurProDuctsModel model = new OurProDuctsModel();
            //product            
            if (data.AdditionType == (byte)Commons.EAdditionType.Hardware || data.AdditionType == (byte)Commons.EAdditionType.Service || data.AdditionType == (byte)Commons.EAdditionType.Function)
            {
                model.Code = "";
            }
            else
            {
                model.Code = data.Code;
            }
            //model.Code = data.Code;
            model.Description = data.Description;
            //Addition
            model.Index = data.Index;
            model.AdditionType = data.AdditionType;
            model.AdditionTypeName = data.AdditionTypeName;
            model.ID = data.ID;
            model.Name = data.Name;
            model.Quantity = data.Quantity;
            model.Period = data.Period;
            model.PeriodType = data.PeriodType;
            model.Price = data.Price;
            model.TotalPrice = data.Price * data.Quantity;
            if (model.PeriodType != (int)Commons.EPeriodType.OneTime)
                model.PeriodTime = model.Period + " ";
            model.PeriodTime += PeriodName(data.Period, data.PeriodType);
            model.CurrencySymbol = CurrencySymbol;

            return PartialView("_ProductItemAddition", model);
        }

        [HttpPost]
        public ActionResult AddAdditionForProduct(ProductDetailViewModels model)
        {
            try
            {
                string Action = "addnew";
                if (model.ProductDetail.ListProductChild != null)
                {
                    model.ProductDetail.ListProductChild = model.ProductDetail.ListProductChild.Where(x => !x.IsDelete).ToList();
                    List<BuyingAdditionModels> listAdditionItem = new List<BuyingAdditionModels>();
                    int OffSet = 0;
                    foreach (var item in model.ProductDetail.ListProductChild)
                    {
                        listAdditionItem.Add(new BuyingAdditionModels
                        {
                            OffSet = OffSet++,
                            ProductID = item.ID,
                            ProductName = item.Name,
                            AdditionTypeName = item.AdditionTypeName,
                            ProductType = (byte)Commons.EProductType.Addition,
                            AdditionType = item.AdditionType,
                            Quantity = item.Quantity,
                            Period = item.Period,
                            PeriodType = item.PeriodType,
                            Price = item.Price,
                            Total = item.Price * item.Quantity,
                            PeriodTime = item.PeriodTime,

                            AppliedOn = model.ProductDetail.Name,//item.Code,
                            ID = model.ProductDetail.ID,//item.Description,
                        });
                    }
                    if (Session["_ListBuyingAdditionItem"] == null)
                        Session.Add("_ListBuyingAdditionItem", listAdditionItem);
                    else
                    {
                        var listTemp = (List<BuyingAdditionModels>)Session["_ListBuyingAdditionItem"];
                        var isExist = listTemp.Exists(x => x.ID.Equals(model.ProductDetail.ID));
                        if (isExist)
                            listTemp.RemoveAll(x => x.ID.Equals(model.ProductDetail.ID));
                        listTemp.AddRange(listAdditionItem);
                        Session.Add("_ListBuyingAdditionItem", listTemp);
                        //============
                    }
                    var data = _yourcartFactory.GetOrderDetail(ORDERID);
                    if (data != null)
                    {
                        if (data.ListItems != null && data.ListItems.Count > 0)
                        {
                            var product = data.ListItems.Where(x => x.ProductID.Equals(model.ProductDetail.ID)).FirstOrDefault();
                            if (product != null)
                            {
                                Action = "update";
                                product.ListItem.ForEach(x => x.IsDelete = true);
                                var lstData = listAdditionItem.Where(x => x.ID.Equals(model.ProductDetail.ID)).ToList();
                                foreach (var item in lstData)
                                {
                                    product.ListItem.Add(new Item
                                    {
                                        //ID = item.ID,
                                        ProductID = item.ProductID,
                                        ProductName = item.ProductName,
                                        ProductType = (byte)Commons.EProductType.Addition,
                                        AdditionType = item.AdditionType,
                                        Quantity = item.Quantity,
                                        Period = item.Period,
                                        PeriodType = item.PeriodType,
                                        Price = item.Price,
                                        IsDelete = item.IsDelete
                                    });
                                }
                                //====================
                                AddItemToOrderModels modelAddItems = new AddItemToOrderModels();
                                modelAddItems.CusID = CurrentUser.UserId;
                                modelAddItems.OrderID = ORDERID;
                                modelAddItems.ListItems.Add(product);
                                string msg = "";
                                bool success = false;
                                //=====Update Items
                                OrderDetailModels OrderDetail = _yourcartFactory.AddItems(modelAddItems, ref success, ref msg);
                            }
                        }
                    }
                }
                string str = "{'Action':'" + Action + "'}";
                JavaScriptSerializer jsSer = new JavaScriptSerializer();
                object obj = jsSer.Deserialize(str, typeof(object));
                return Json(obj, JsonRequestBehavior.AllowGet);
                //return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("AddAdditionForProduct: ", ex);
                return new HttpStatusCodeResult(400, Commons.MsgErrorForClient);
            }
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult AddToCartOfProduct(ProductDetailViewModels model)
        {
            try
            {
                AddItemToOrderModels modelAddItems = new AddItemToOrderModels();
                modelAddItems.CusID = CurrentUser.UserId;
                modelAddItems.OrderID = ORDERID;// model.ProductDetail.OrderID;
                List<Item> ListItems = new List<Item>();
                //===============
                Item product = new Item();
                product.ID = "";// model.ProductDetail.ID;
                product.ProductID = model.ProductDetail.ID;
                product.ProductName = model.ProductDetail.Name;
                product.ProductType = model.ProductDetail.ProductType;
                product.AdditionType = model.ProductDetail.AdditionType;
                product.ImageURL = model.ProductDetail.ImageURL;
                product.Period = model.ProductDetail.Period;
                product.PeriodType = model.ProductDetail.PeriodType;
                product.Quantity = model.ProductDetail.Quantity;
                product.Price = model.ProductDetail.Price;

                product.NumOfStore = model.ProductDetail.NumOfStore;
                product.NumOfAccount = model.ProductDetail.NumberOfAccount;
                product.AmountOfUnit = model.ProductDetail.AmountOfUnit;

                product.PromotionID = model.ProductDetail.PromotionID;
                product.PromotionName = model.ProductDetail.PromotionName;
                product.PromotionAmount = model.ProductDetail.PromotionAmount;
                product.IsApplyPromotion = model.ProductDetail.IsApplyPromotion;

                product.DiscountID = model.ProductDetail.DiscountID;
                product.DiscountName = model.ProductDetail.DiscountName;
                product.DiscountAmount = model.ProductDetail.DiscountAmount;
                product.DiscountValue = model.ProductDetail.DiscountValue;
                product.DiscountType = model.ProductDetail.DiscountType;

                product.Description = model.ProductDetail.Description;
                product.ItemGroup = model.ProductDetail.ItemGroup;
                product.IsDelete = model.ProductDetail.IsDelete;
                product.ListComposite = model.ProductDetail.ListComposite;

                //Buying Addition
                product.ListItem = new List<Item>();
                if (model.ProductDetail.ListBuyingAddition != null)
                {
                    model.ProductDetail.ListBuyingAddition = model.ProductDetail.ListBuyingAddition.Where(x => !x.IsDelete).ToList();
                    foreach (var item in model.ProductDetail.ListBuyingAddition)
                    {
                        product.ListItem.Add(new Item
                        {
                            //ID = item.ID,
                            ProductID = item.ProductID,
                            ProductName = item.ProductName,
                            ProductType = (byte)Commons.EProductType.Addition,
                            AdditionType = item.AdditionType,
                            Quantity = item.Quantity,
                            Period = item.Period,
                            PeriodType = item.PeriodType,
                            Price = item.Price,
                            IsDelete = item.IsDelete,
                            ItemGroup = (byte)Commons.EItemGroup.Addition
                        });
                    }
                }
                product.ListStoreApply = new List<ApplyStore>();
                product.ListFunction = new List<ItemFunction>();
                product.ListAdditionApply = new List<AdditionApply>();
                //=========
                ListItems.Add(product);
                modelAddItems.ListItems = ListItems;
                //====================
                string msg = "";
                bool success = false;
                OrderDetailModels OrderDetail = _yourcartFactory.AddItems(modelAddItems, ref success, ref msg);
                if (success)
                {
                    ORDERID = OrderDetail.ID;
                    if (OrderDetail.ListItems != null && OrderDetail.ListItems.Count > 0)
                        OrderDetail.TotalQuantity = (int)OrderDetail.ListItems.Sum(x => x.Quantity);
                    return Json(OrderDetail, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    if (!string.IsNullOrEmpty(msg))
                    {
                        if (msg.Equals("Payment is pending!") || msg.Equals("The receipt has been paid in full!"))
                        {
                            var Cookie = Request.Cookies["csc-order-v2"];
                            if (Cookie != null)
                            {
                                var Order = Cookie.Value;
                                var strOrder = Server.UrlDecode(Order);
                                var ListOrder = JsonConvert.DeserializeObject<List<CookieOrder>>(strOrder);
                                if (ListOrder != null && ListOrder.Any())
                                {
                                    var temp = ListOrder.FirstOrDefault(x => x.OrderId.Equals(ORDERID));
                                    if (temp != null)
                                    {
                                        ListOrder.Remove(temp);
                                        Cookie.Value = null;
                                        Response.Cookies.Add(Cookie);
                                        var strListOrder = JsonConvert.SerializeObject(ListOrder);
                                        Cookie.Value = strListOrder;
                                        Response.Cookies.Add(Cookie);
                                    }
                                }
                            }
                            ORDERID = "";
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, msg);
                        }
                    }
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, Commons.MsgErrorForClient);
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("AddToCartOfProduct: ", ex);
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        #endregion

        #region Addition
        public ActionResult Addition()
        {
            AdditionViewModels model = new AdditionViewModels();
            model.ListCategory = _factory.ProductGetCate();
            model.ListCategory = model.ListCategory.Where(x=>!x.IsFreeTrial).OrderBy(o => o.Sequence).ThenBy(oo => oo.Name).ToList();
            model.ListCategory.ForEach(x =>
            {
                if (x.ImageURL == null || x.ImageURL.Equals(""))
                    x.ImageURL = _ImgDummyProduct;

            });
            return View("_Additions", model);
        }

        public ActionResult LoadProductOfAddition(int AdditionType, List<string> ListCate, List<int> ListType)
        {
            AdditionViewModels model = new AdditionViewModels();
            if (ListCate != null && ListCate.Count > 0 && ListType != null && ListType.Count > 0)
            {
                model.ListProduct = _factory.GetListData(CurrentUser.UserId, "", (byte)Commons.EProductType.Addition, ListCate, null, null, ListType);
                model.ListProduct = model.ListProduct.Where(x => x.AdditionType == AdditionType).ToList();
                model.ListProduct.ForEach(x =>
                {
                    if (x.ListPrice != null && x.ListPrice.Count > 0)
                    {
                        //x.Price = x.ListPrice.Select(z => z.Price).Min();
                        var PriceMin = x.ListPrice.OrderBy(w => w.PeriodType).ThenBy(w => w.Period).ThenBy(w => w.Price).First();
                        x.Price = PriceMin.Price;
                    }

                    if (x.IsFullCate)
                        x.ApplicableProducts = "The addition is applicable on all products.";
                    else
                    {
                        if (x.ListCategory != null && x.ListCategory.Count > 0)
                        {
                            x.ApplicableProducts = string.Join(", ", x.ListCategory.Select(z => z.CategoryName).ToList());
                        }
                    }

                    if (x.ImageURL == null || x.ImageURL.Equals(""))
                        x.ImageURL = _ImgDummyProduct;

                    x.CurrencySymbol = CurrencySymbol;
                });
                model.ListProduct = model.ListProduct.OrderByDescending(x => x.Price).ToList();
            }
            else
            {
                //model.ListProduct = _factory.GetListData("", (byte)Commons.EProductType.Addition, ListCate, null);
                //model.ListProduct = model.ListProduct.Where(x => x.AdditionType == AdditionType).ToList();
                //model.ListProduct.ForEach(x =>
                //{
                //    if (x.ListPrice != null && x.ListPrice.Count > 0)
                //    {
                //        //x.Price = x.ListPrice.Select(z => z.Price).Min();
                //        var PriceMin = x.ListPrice.OrderBy(w => w.PeriodType).ThenBy(w => w.Period).ThenBy(w => w.Price).First();
                //        x.Price = PriceMin.Price;
                //    }
                //    if (x.IsFullCate)
                //        x.ApplicableProducts = "The addition is applicable on all products.";
                //    else
                //    {
                //        if (x.ListCategory != null && x.ListCategory.Count > 0)
                //        {
                //            x.ApplicableProducts = string.Join(", ", x.ListCategory.Select(z => z.CategoryName).ToList());
                //        }
                //    }

                //    if (x.ImageURL == null || x.ImageURL.Equals(""))
                //        x.ImageURL = _ImgDummyProduct;

                //    x.CurrencySymbol = CurrencySymbol;
                //});
                //model.ListProduct = model.ListProduct.OrderByDescending(x => x.Price).ToList();
            }
            return PartialView("_AdditionsListItem", model);
        }

        [EncryptedActionParameter]
        public ActionResult AdditionDetail(string id)
        {
            AdditionDetailViewModels model = new AdditionDetailViewModels();

            var ListProduct = _factory.GetListData(CurrentUser.UserId, null, (byte)Commons.EProductType.Addition, null, null, id);
            if (ListProduct != null && ListProduct.Count > 0)
            {
                model.AdditionDetail = ListProduct.FirstOrDefault();
                model.AdditionDetail.Quantity = 1;
                if (model.AdditionDetail.IsUnlimitedAmountOfUnit)
                    model.AdditionDetail.AmountOfUnit = -1;

                if (model.AdditionDetail.IsUnlimitedNumberOfAccount)
                    model.AdditionDetail.NumberOfAccount = -1;

                if (model.AdditionDetail.IsUnlimitedIncludeStore)
                {
                    model.AdditionDetail.NumOfStore = -1;
                    model.AdditionDetail.IncludeStore = -1;
                }
                else
                    model.AdditionDetail.NumOfStore = model.AdditionDetail.IncludeStore;

                if (model.AdditionDetail.ImageURL == null || model.AdditionDetail.ImageURL.Equals(""))
                    model.AdditionDetail.ImageURL = _ImgDummyProduct;

                if (model.AdditionDetail.ListPrice != null && model.AdditionDetail.ListPrice.Count > 0)
                {
                    model.AdditionDetail.ListPrice.ForEach(x =>
                    {
                        x.NamePeriodType = PeriodName(x.Period, x.PeriodType);
                    });

                    var PriceMin = model.AdditionDetail.ListPrice.Where(w => !w.IsExtend).OrderBy(x => x.PeriodType).ThenBy(w => w.Period).ThenBy(w => w.Price).FirstOrDefault();

                    model.AdditionDetail.TotalPrice = PriceMin.Price;
                    model.AdditionDetail.Price = PriceMin.Price;
                    if (PriceMin.PeriodType != (int)Commons.EPeriodType.OneTime)
                        model.AdditionDetail.PeriodTime = PriceMin.Period + " ";
                    model.AdditionDetail.PeriodTime += PriceMin.NamePeriodType;

                    model.AdditionDetail.Period = (int)PriceMin.Period;
                    model.AdditionDetail.PeriodType = PriceMin.PeriodType;

                    //========
                    string PriceID = PriceMin.ID;
                    model.AdditionDetail.ListPrice.ForEach(x =>
                    {
                        x.IsActive = x.ID.Equals(PriceID) ? true : false;
                    });
                    //==========Check Remaining days of the Addition
                    if (model.AdditionDetail.ListPrice.Count > 1)
                    {
                        var _year = model.AdditionDetail.ListPrice.Where(x => x.PeriodType
                                            == (byte)Commons.EPeriodType.Year).OrderByDescending(x => x.Period).FirstOrDefault();

                        var _month = model.AdditionDetail.ListPrice.Where(x => x.PeriodType
                                            == (byte)Commons.EPeriodType.Month).OrderByDescending(x => x.Period).FirstOrDefault();

                        var _day = model.AdditionDetail.ListPrice.Where(x => x.PeriodType
                                            == (byte)Commons.EPeriodType.Day).OrderByDescending(x => x.Period).FirstOrDefault();

                        if (_year != null)
                        {
                            model.AdditionDetail.MaxDay = (int)_year.Period * 365;
                            model.AdditionDetail.MinDay = model.AdditionDetail.MaxDay;
                        }
                        if (_month != null)
                        {
                            model.AdditionDetail.MinDay = (int)_month.Period * 30;
                            model.AdditionDetail.MinDay = model.AdditionDetail.MinDay;
                        }
                        if (_day != null)
                        {
                            model.AdditionDetail.MinDay = (int)_day.Period;
                            model.AdditionDetail.MinDay = model.AdditionDetail.MinDay;
                        }
                    }
                }
                //=======
                model.AdditionDetail.ItemGroup = (byte)Commons.EItemGroup.Addition;
                model.AdditionDetail.CusId = CurrentUser.UserId;

                model.AdditionDetail.CurrencySymbol = CurrencySymbol;
            }
            return View("_AdditionDetail", model);
        }

        [EncryptedActionParameter]
        public ActionResult AdditionApplyProduct(int Type, string AdditionID)
        {
            AdditionDetailViewModels model = new AdditionDetailViewModels();
            var ListProductApply = _factory.ProductGetProductApplyAddition(AdditionID, CurrentUser.UserId);
            if (Type == 1)
                ListProductApply = ListProductApply.Where(x => !x.Category.IsFreeTrial).ToList();
            ListProductApply = ListProductApply.Where(x => x.ProductType == Type).OrderByDescending(x => x.BoughtTime).ThenBy(x => x.ProductName).ToList();
            ListProductApply.ForEach(x =>
            {
                if (!x.ExpiredTime.HasValue)
                    x.sExpiredTime = "Not yet activated";
                else if (x.ExpiredTime.Value.Date == Commons.MaxDate.Date)
                    x.sExpiredTime = "No expiry date";
                else
                    x.sExpiredTime = x.ExpiredTime.Value.ToString(Commons.FormatDate);
                //========
                //if (x.RecommendedPrice != null)
                //    x.Price = x.RecommendedPrice.Price;

                //x.CurrencySymbol = CurrencySymbol;

                // Recommend Price
                if (x.RecommendedPrice != null)
                {
                    x.Price = x.RecommendedPrice.Price;
                    x.Period = (int)x.RecommendedPrice.Period;
                    x.PeriodType = x.RecommendedPrice.PeriodType;
                }
                else
                {
                    x.Price = 0;
                    x.Period = 0;
                    x.PeriodType = 0;
                }

                // For Package
                if (x.ListProduct != null && x.ListProduct.Any())
                {
                    x.ListProduct.ForEach(xy =>
                    {
                        if (!xy.ExpiredTime.HasValue)
                            xy.sExpiredTime = "Not yet activated";
                        else if (x.ExpiredTime.Value.Date == Commons.MaxDate.Date)
                            x.sExpiredTime = "No expiry date";
                        else
                            xy.sExpiredTime = xy.ExpiredTime.Value.ToString(Commons.FormatDateTime);

                        // Recommend Price
                        if (xy.RecommendedPrice != null)
                        {
                            xy.Price = xy.RecommendedPrice.Price;
                            xy.Period = (int)xy.RecommendedPrice.Period;
                            xy.PeriodType = xy.RecommendedPrice.PeriodType;
                        }
                        else
                        {
                            xy.Price = 0;
                            xy.Period = 0;
                            xy.PeriodType = 0;
                        }
                    });
                }
            });
            model.TypeProductApply = Type;
            model.ListProductApply = ListProductApply;
            return PartialView("_AdditionApplyProduct", model);
        }

        public ActionResult SetProductForAddition(ProductApplyAdditionModels data)
        {
            AdditionDetailViewModels model = new AdditionDetailViewModels();
            //if (data.ExpiredTime.HasValue)
            //{
            //    DateTime curentDate = DateTime.Now;
            //    int days = (data.ExpiredTime.Value.Date - curentDate.Date).Days + 1;
            //    if (days > 0)
            //        data.Days = days + " Days";
            //    if (days > data.MinDay && days < data.MaxDay)
            //        model.ListProductApply.Add(data);

            //    model.ListProductApply.ForEach(x =>
            //    {
            //        x.CurrencySymbol = CurrencySymbol;
            //    });
            //}
            if (data.Period > 0 && data.PeriodType > 0)
            {
                data.Days = data.Period + ((Commons.EPeriodType)Enum.ToObject(typeof(Commons.EPeriodType), data.PeriodType)).ToString();
                if (data.Period > 1)
                {
                    data.Days = data.Days + "s";
                }
            }

            model.ListProductApply.Add(data);
            model.ListProductApply.ForEach(x =>
            {
                x.CurrencySymbol = CurrencySymbol;
            });
            return PartialView("_AdditionItemProduct", model);
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult AddToCartOfAddition(AdditionDetailViewModels model)
        {
            try
            {
                AddItemToOrderModels modelAddItems = new AddItemToOrderModels();
                modelAddItems.CusID = CurrentUser.UserId;
                modelAddItems.OrderID = ORDERID; //model.AdditionDetail.OrderID;
                List<Item> ListItems = new List<Item>();
                //===============
                Item addition = new Item();
                addition.ID = "";// model.AdditionDetail.ID;
                addition.ProductID = model.AdditionDetail.ID;
                addition.ProductName = model.AdditionDetail.Name;
                addition.ProductType = model.AdditionDetail.ProductType;
                addition.AdditionType = model.AdditionDetail.AdditionType;
                addition.ImageURL = model.AdditionDetail.ImageURL;
                addition.Period = model.AdditionDetail.Period;
                addition.PeriodType = model.AdditionDetail.PeriodType;
                addition.Quantity = model.AdditionDetail.Quantity;
                addition.Price = model.AdditionDetail.Price;
                addition.AmountOfUnit = model.AdditionDetail.AmountOfUnit;
                addition.NumOfAccount = model.AdditionDetail.NumberOfAccount;
                // Number of store = Amount of unit
                addition.NumOfStore = model.AdditionDetail.AmountOfUnit;

                addition.PromotionID = model.AdditionDetail.PromotionID;
                addition.PromotionName = model.AdditionDetail.PromotionName;
                addition.PromotionAmount = model.AdditionDetail.PromotionAmount;
                addition.IsApplyPromotion = model.AdditionDetail.IsApplyPromotion;

                addition.DiscountID = model.AdditionDetail.DiscountID;
                addition.DiscountName = model.AdditionDetail.DiscountName;
                addition.DiscountAmount = model.AdditionDetail.DiscountAmount;
                addition.DiscountValue = model.AdditionDetail.DiscountValue;
                addition.DiscountType = model.AdditionDetail.DiscountType;

                addition.Description = model.AdditionDetail.Description;
                addition.ItemGroup = model.AdditionDetail.ItemGroup;
                addition.IsDelete = model.AdditionDetail.IsDelete;

                addition.ListItem = new List<Item>();
                addition.ListStoreApply = new List<ApplyStore>();
                addition.ListFunction = new List<ItemFunction>();
                addition.ListAdditionApply = new List<AdditionApply>();
                if (model.ListProductApply != null && model.ListProductApply.Count > 0)
                {
                    var ProductApply = model.ListProductApply.FirstOrDefault();
                    //if (ProductApply.IsSelect)
                    //{
                    //    addition.ListAdditionApply.Add(new AdditionApply
                    //    {
                    //        ID = ProductApply.AssetID,
                    //        Name = ProductApply.ProductName,
                    //        ProductID = ProductApply.ProductID,
                    //    });
                    //}

                    addition.ListAdditionApply.Add(new AdditionApply
                    {
                        ID = ProductApply.AssetID,
                        Name = ProductApply.ProductName,
                        ProductID = ProductApply.ProductID,
                        ParentID = ProductApply.ParentID,
                        ParentName = ProductApply.ParentProductName,
                        ProductParentID = ProductApply.ParentProductID,
                    });

                    // Set Period if Recommend Price were chosen
                    if (ProductApply.IsSelect)
                    {
                        addition.Period = ProductApply.Period;
                        addition.Price = ProductApply.Price;
                        addition.PeriodType = ProductApply.PeriodType;
                    }
                }
                //=========
                ListItems.Add(addition);
                modelAddItems.ListItems = ListItems;
                modelAddItems.IsFree = Convert.ToBoolean(Session["IsFree"]);
                //====================
                string msg = "";
                bool success = false;
                OrderDetailModels OrderDetail = _yourcartFactory.AddItems(modelAddItems, ref success, ref msg);
                if (success)
                {
                    ORDERID = OrderDetail.ID;
                    if (OrderDetail.ListItems != null && OrderDetail.ListItems.Count > 0)
                        OrderDetail.TotalQuantity = (int)OrderDetail.ListItems.Sum(x => x.Quantity);
                    return Json(OrderDetail, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    if (!string.IsNullOrEmpty(msg))
                    {
                        if (msg.Equals("Payment is pending!") || msg.Equals("The receipt has been paid in full!"))
                        {
                            var Cookie = Request.Cookies["csc-order-v2"];
                            if (Cookie != null)
                            {
                                var Order = Cookie.Value;
                                var strOrder = Server.UrlDecode(Order);
                                var ListOrder = JsonConvert.DeserializeObject<List<CookieOrder>>(strOrder);
                                if (ListOrder != null && ListOrder.Any())
                                {
                                    var temp = ListOrder.FirstOrDefault(x => x.OrderId.Equals(ORDERID));
                                    if (temp != null)
                                    {
                                        ListOrder.Remove(temp);
                                        Cookie.Value = null;
                                        Response.Cookies.Add(Cookie);
                                        var strListOrder = JsonConvert.SerializeObject(ListOrder);
                                        Cookie.Value = strListOrder;
                                        Response.Cookies.Add(Cookie);
                                    }
                                }
                            }
                            ORDERID = "";
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, msg);
                        }
                        else if (msg.Equals("The curent order contains free trial product. Please checkout this order before buying other items."))
                        {
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, msg);
                        }
                    }
                    
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, Commons.MsgErrorForClient);
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("AddToCartOfAddition: ", ex);
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
            }
        }
        #endregion End Addition

        #region Package
        public ActionResult Packages()
        {
            try
            {
                PackageViewModels model = new PackageViewModels();
                model.ListCategories = _factory.ProductGetCate();
                model.ListCategories = model.ListCategories.OrderBy(o => o.Sequence).ThenBy(oo => oo.Name).ToList();
                model.ListCategories.ForEach(x =>
                {
                    if (x.ImageURL == null || x.ImageURL.Equals(""))
                        x.ImageURL = _ImgDummyProduct;
                });
                return View("_Packages", model);
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("OurProducts Packages: ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }

        public ActionResult LoadPackageByProductCategories(List<string> ListCate)
        {
            try
            {
                PackageViewModels model = new PackageViewModels();
                //if (ListCate != null && ListCate.Count > 0)
                //{
                model.ListProducts = _factory.GetListData(CurrentUser.UserId, "", (byte)Commons.EProductType.Package, ListCate, null);
                model.ListProducts.ForEach(x =>
                {
                    if (x.ListPrice != null && x.ListPrice.Count > 0)
                    {
                        // x.Price = x.ListPrice.Select(z => z.Price).Min();
                        var PriceMin = x.ListPrice.Where(w => !w.IsExtend).OrderBy(w => w.PeriodType).ThenBy(w => w.Period).ThenBy(w => w.Price).FirstOrDefault();
                        x.Price = PriceMin == null ? 0 : PriceMin.Price;
                    }

                    if (x.ImageURL == null || x.ImageURL.Equals(""))
                        x.ImageURL = _ImgDummyPackage;

                    x.CurrencySymbol = CurrencySymbol;
                });
                model.ListProducts = model.ListProducts.OrderBy(o => o.Sequence).ThenBy(oo => oo.Name).ToList();

                model.ListProducts.ForEach(x =>
                {
                    //Product
                    x.ListProductOnPackage = x.ListComposite.Where(ww => ww.ProductType == (byte)Commons.EProductType.Product).OrderBy(ww => ww.Sequence).ThenBy(ww => ww.ProductName).ToList();
                    x.ListProductOnPackage.ForEach(z =>
                    {
                        if (z.Quantity == -1)
                            z.ProductName = z.ProductName + " (For unlimited devices)";
                        else
                            z.ProductName = z.ProductName + " (For " + z.Quantity + " device" + (z.Quantity > 1 ? "s" : "") + ")";
                    });
                    //Addition
                    x.ListComposite = x.ListComposite.Where(ww => ww.ProductType != (byte)Commons.EProductType.Product).OrderBy(ww => ww.Sequence).ThenBy(ww => ww.ProductName).ToList();
                    x.ListComposite.ForEach(z =>
                    {
                        z.ProductName = z.Quantity + " x " + z.ProductName;
                    });
                    //============List Product Child
                    x.ListProductChild = x.ListProductChild.Where(o => o.ProductType == (byte)Commons.EProductType.Product).OrderBy(o => o.Sequence).ThenBy(oo => oo.Name).ToList();
                    x.ListProductChild.ForEach(o =>
                    {
                        if (o.ImageURL == null || o.ImageURL.Equals(""))
                            o.ImageURL = _ImgDummyProduct;

                        if (o.IsUnlimitedAmountOfUnit)
                            o.sUnlimitedAmountOfUnit = "Product key can be applied on unlimited devices.";
                        else
                            o.sUnlimitedAmountOfUnit = "Product key can be applied on " + o.AmountOfUnit + " user device(s).";
                        //==============
                        if (o.IsUnlimitedNumberOfAccount)
                            o.sUnlimitedNumberOfAccount = " License key of the product can be used to activate unlimited user account(s).";
                        else
                            o.sUnlimitedNumberOfAccount = "License key of the product can be used to activate " + o.NumberOfAccount + " user account(s)";
                        //==============
                        if (o.IsFullFunction)
                            o.sFullFunction = "All functions included.";
                        else
                            o.sFullFunction = string.Join(", ", o.ListFunction.OrderBy(z => z.FunctionName).Select(z => z.FunctionName).ToList());
                    });
                });
                //}
                return PartialView("_PackagesListItem", model);
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("OurProducts List Packages: ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }

        public ActionResult PackageDetail(string Id)
        {
            try
            {
                PackageDetailViewModels model = new PackageDetailViewModels();

                var ListPackages = _factory.GetListData(CurrentUser.UserId, null, (byte)Commons.EProductType.Package, null, null, Id);
                //ListPackages.ForEach(x => { x.CusId = CurrentUser.UserId; });
                if (ListPackages != null && ListPackages.Any())
                {
                    model.PackageDetail = ListPackages.FirstOrDefault();
                    model.PackageDetail.CusId = CurrentUser.UserId;
                    model.PackageDetail.Quantity = 1;
                    if (model.PackageDetail.IsUnlimitedAmountOfUnit)
                        model.PackageDetail.AmountOfUnit = -1;

                    if (model.PackageDetail.IsUnlimitedNumberOfAccount)
                        model.PackageDetail.NumberOfAccount = -1;

                    if (model.PackageDetail.IsUnlimitedIncludeStore)
                    {
                        model.PackageDetail.NumOfStore = -1;
                        model.PackageDetail.IncludeStore = -1;
                    }
                    else
                        model.PackageDetail.NumOfStore = model.PackageDetail.IncludeStore;

                    if (model.PackageDetail.ImageURL == null || model.PackageDetail.ImageURL.Equals(""))
                        model.PackageDetail.ImageURL = _ImgDummyPackage;

                    if (model.PackageDetail.ListPrice != null && model.PackageDetail.ListPrice.Any())
                    {
                        model.PackageDetail.ListPrice.ForEach(x =>
                        {
                            x.NamePeriodType = PeriodName(x.Period, x.PeriodType);
                        });

                        var PriceMin = model.PackageDetail.ListPrice.Where(x => !x.IsExtend).OrderBy(w => w.PeriodType).ThenBy(w => w.Period).ThenBy(w => w.Price).FirstOrDefault();

                        model.PackageDetail.TotalPrice = PriceMin.Price;
                        model.PackageDetail.Price = PriceMin.Price;

                        model.PackageDetail.Period = (int)PriceMin.Period;
                        model.PackageDetail.PeriodType = PriceMin.PeriodType;

                        // Period Time != One Time
                        if (model.PackageDetail.PeriodType != (byte)Commons.EPeriodType.OneTime)
                        {
                            model.PackageDetail.PeriodTime = PriceMin.Period + " " + PriceMin.NamePeriodType;
                        }
                        else
                        {
                            model.PackageDetail.PeriodTime = PriceMin.NamePeriodType;
                        }
                    }

                    //=======Get List Product Child
                    if (model.PackageDetail.ListProductChild != null && model.PackageDetail.ListProductChild.Any())
                    {
                        //update bug of Long by Trongntn 22-11-2017
                        model.PackageDetail.ListProductChild = model.PackageDetail.ListProductChild.Where(x => x.ProductType == (byte)Commons.EProductType.Product).ToList();

                        model.PackageDetail.ListProductChild.ForEach(xx =>
                        {
                            // Get list cate product of package
                            if (xx.ProductType == (byte)Commons.EProductType.Product)
                            {
                                model.ListApplyAdditionProduct.Add(new ProductApplyAdditionPackage
                                {
                                    ProductName = xx.Name,
                                    ProductId = xx.ID,
                                    ProductCateId = xx.ListCategory.Select(s => s.CategoryID).FirstOrDefault().ToString()
                                });
                            }
                        });
                    }

                    //==========Get List Addition Type
                    List<SelectListItem> ListAdditionType = new List<SelectListItem>();
                    ListAdditionType = GetListAdditionType();
                    ViewBag.ListAdditionType = ListAdditionType;

                    model.PackageDetail.CurrencySymbol = CurrencySymbol;
                    if (model.PackageDetail.ListComposite != null && model.PackageDetail.ListComposite.Any())
                    {
                        //Product
                        model.PackageDetail.ListProductOnPackage = model.PackageDetail.ListComposite
                            .Where(ww => ww.ProductType == (byte)Commons.EProductType.Product)
                            .OrderBy(ww => ww.Sequence).ThenBy(ww => ww.ProductName).ToList();
                        model.PackageDetail.ListProductOnPackage.ForEach(x =>
                        {
                            if (x.Quantity == -1)
                                x.ProductName = x.ProductName + " (For unlimited devices)";
                            else
                                x.ProductName = x.ProductName + " (For " + x.Quantity + " device" + (x.Quantity > 1 ? "s" : "") + ")";
                        });

                        //Addition
                        model.PackageDetail.ListComposite = model.PackageDetail.ListComposite
                            .Where(ww => ww.ProductType != (byte)Commons.EProductType.Product)
                            .OrderBy(ww => ww.Sequence).ThenBy(ww => ww.ProductName).ToList();
                    }

                }
                return View("_PackageDetail", model);
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("OurProducts Package Detail: ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }

        public ActionResult GetListAdditionForPackage(int AdditionType, List<ProductApplyAdditionPackage> ListApplyAdditionProduct)
        {
            try
            {
                PackageViewModels model = new PackageViewModels();

                model.ListProducts = _factory.GetListData(CurrentUser.UserId, "", (byte)Commons.EProductType.Addition);

                if (AdditionType > 0)
                {
                    model.ListProducts = model.ListProducts.Where(x => x.AdditionType == AdditionType).ToList();
                }

                //// Get Additions can apply for products in this package
                //if (ListApplyAdditionProduct != null && ListApplyAdditionProduct.Any())
                //{
                //    var ListCateApplicable = ListApplyAdditionProduct.Select(s => s.ProductCateId).Distinct().ToList();
                //    // Addition is a Hardware || Service || Function 
                //    // Addition is a Software || Location || Account has any product category of package in list category || IsFullCate == true
                //    model.ListProducts = model.ListProducts.Where(w => (w.AdditionType == (byte)Commons.EAdditionType.Hardware || w.AdditionType == (byte)Commons.EAdditionType.Service || w.AdditionType == (byte)Commons.EAdditionType.Function)
                //    || ((w.AdditionType == (byte)Commons.EAdditionType.Software || w.AdditionType == (byte)Commons.EAdditionType.Location || w.AdditionType == (byte)Commons.EAdditionType.Account) && (w.IsFullCate == true || (w.ListCategory != null && w.ListCategory.Any() && w.ListCategory.Where(ww => ListCateApplicable.Contains(ww.CategoryID)).Any())))).ToList();
                //}
                //else
                //{
                //    // Addition is a Hardware || Service || Function 
                //    model.ListProducts = model.ListProducts.Where(w => w.AdditionType == (byte)Commons.EAdditionType.Hardware || w.AdditionType == (byte)Commons.EAdditionType.Service || w.AdditionType == (byte)Commons.EAdditionType.Function).ToList();
                //}

                var gData = model.ListProducts.OrderBy(o => o.AdditionType).GroupBy(x => x.AdditionType).Select(x => new BuyingAdditionViewModels
                {
                    ID = x.Key,
                    ListItem = x.ToList()
                }).ToList();

                gData.ForEach(x =>
                {
                    x.Name = ((Commons.EAdditionType)Enum.ToObject(typeof(Commons.EAdditionType), x.ID)).ToString();
                    int OffSet = 0;
                    x.ListItem = x.ListItem.OrderBy(o => o.Sequence).ThenBy(oo => oo.Name).ToList();
                    x.ListItem.ForEach(z =>
                    {
                        z.Index = OffSet++;
                        //================
                        z.AdditionTypeName = x.Name;
                        if (z.ListPrice != null && z.ListPrice.Count > 0)
                        {
                            z.ListPrice.ForEach(xx =>
                            {
                                xx.NamePeriodType = PeriodName(xx.Period, xx.PeriodType);
                            });

                            var PriceMin = z.ListPrice.OrderBy(o => o.PeriodType).ThenBy(w => w.Period).ThenBy(w => w.Price).FirstOrDefault();
                            z.Price = PriceMin.Price;
                            z.Period = (int)PriceMin.Period;
                            z.PeriodType = PriceMin.PeriodType;

                            // Period Time != One Time
                            if (z.PeriodType != (byte)Commons.EPeriodType.OneTime)
                            {
                                z.PeriodTime = PriceMin.Period + " " + PriceMin.NamePeriodType;
                            }
                            else
                            {
                                z.PeriodTime = PriceMin.NamePeriodType;
                            }
                        }
                        // Get list product apply addition
                        if (ListApplyAdditionProduct != null && ListApplyAdditionProduct.Any())
                        {
                            if (z.AdditionType != (byte)Commons.EAdditionType.Hardware && z.AdditionType != (byte)Commons.EAdditionType.Service && z.AdditionType != (byte)Commons.EAdditionType.Function)
                            {
                                z.ListProductApplicable = new List<SelectListItem>();
                                //if (z.IsFullCate == true)
                                //{
                                //    ListApplyAdditionProduct.ForEach(y =>
                                //    {
                                //        z.ListProductApplicable.Add(new SelectListItem
                                //        {
                                //            Text = y.ProductName,
                                //            Value = y.ProductId
                                //        });
                                //    });
                                //}
                                //else
                                //{
                                //    var lstApplyAdditionProduct = ListApplyAdditionProduct.Where(w => (z.ListCategory.Select(s => s.CategoryID).ToList()).Contains(w.ProductCateId)).ToList();
                                //    lstApplyAdditionProduct.ForEach(y =>
                                //    {
                                //        z.ListProductApplicable.Add(new SelectListItem
                                //        {
                                //            Text = y.ProductName,
                                //            Value = y.ProductId
                                //        });
                                //    });
                                //}
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
                    });
                });
                gData = gData.OrderBy(x => x.Name).ToList();
                return PartialView("_AdditionsPackage", gData);
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("OurProducts GetListAdditionForPackage: ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }

        public ActionResult AddItemAdditionPackage(OurProDuctsModel data)
        {
            try
            {
                OurProDuctsModel model = new OurProDuctsModel();
                model.ID = data.ID;
                model.Code = data.Code;
                model.Index = data.Index;
                model.AdditionType = data.AdditionType;
                model.AdditionTypeName = data.AdditionTypeName;
                model.Name = data.Name;
                model.Quantity = data.Quantity;
                model.Period = data.Period;
                model.PeriodType = data.PeriodType;
                model.Price = data.Price;
                model.ApplyProductId = data.ApplyProductId;
                if (data.PeriodType != (int)Commons.EPeriodType.OneTime)
                    model.PeriodTime = data.Period + " " + PeriodName(data.Period, data.PeriodType);
                else
                    model.PeriodTime = PeriodName(data.Period, data.PeriodType);

                return PartialView("_ItemAdditionPackage", model);
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("OurProducts AddItemAdditionPackage: ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult AddToCartOfPackage(PackageDetailViewModels model)
        {
            try
            {
                AddItemToOrderModels modelAddItems = new AddItemToOrderModels();
                modelAddItems.CusID = CurrentUser.UserId;
                modelAddItems.OrderID = ORDERID;// model.PackageDetail.OrderID;
                List<Item> ListItems = new List<Item>();
                //===============
                Item package = new Item();
                package.ID = "";
                package.ProductID = model.PackageDetail.ID;
                package.ProductName = model.PackageDetail.Name;
                package.ProductType = model.PackageDetail.ProductType;
                package.AdditionType = model.PackageDetail.AdditionType;
                package.ImageURL = model.PackageDetail.ImageURL;
                package.Period = model.PackageDetail.Period;
                package.PeriodType = model.PackageDetail.PeriodType;
                package.Quantity = model.PackageDetail.Quantity;
                package.Price = model.PackageDetail.Price;

                package.NumOfStore = model.PackageDetail.NumOfStore;
                package.NumOfAccount = model.PackageDetail.NumberOfAccount;

                package.PromotionID = model.PackageDetail.PromotionID;
                package.PromotionName = model.PackageDetail.PromotionName;
                package.PromotionAmount = model.PackageDetail.PromotionAmount;
                package.IsApplyPromotion = model.PackageDetail.IsApplyPromotion;

                package.DiscountID = model.PackageDetail.DiscountID;
                package.DiscountName = model.PackageDetail.DiscountName;
                package.DiscountAmount = model.PackageDetail.DiscountAmount;
                package.DiscountValue = model.PackageDetail.DiscountValue;
                package.DiscountType = model.PackageDetail.DiscountType;

                package.Description = model.PackageDetail.Description;
                package.ItemGroup = model.PackageDetail.ItemGroup;
                package.IsDelete = model.PackageDetail.IsDelete;

                package.ListItem = new List<Item>();
                foreach (var item in model.ListAdditionBuy.Where(w => w.IsDelete != true).ToList())
                {
                    package.ListItem.Add(new Item
                    {
                        ID = "",
                        ProductID = item.ID,
                        ParentAddition = item.ApplyProductId,
                        ProductName = item.Name,
                        ProductType = (byte)Commons.EProductType.Addition,
                        AdditionType = item.AdditionType,
                        Quantity = item.Quantity,
                        Period = item.Period,
                        PeriodType = item.PeriodType,
                        Price = item.Price,
                        IsDelete = item.IsDelete,
                        ItemGroup = (byte)Commons.EItemGroup.Addition
                    });
                }
                package.ListStoreApply = new List<ApplyStore>();
                package.ListFunction = new List<ItemFunction>();
                package.ListAdditionApply = new List<AdditionApply>();

                if (model.PackageDetail.ListComposite == null)
                    model.PackageDetail.ListComposite = new List<ProductCompositeModels>();
                if (model.PackageDetail.ListProductOnPackage != null && model.PackageDetail.ListProductOnPackage.Any())
                    model.PackageDetail.ListComposite.AddRange(model.PackageDetail.ListProductOnPackage);

                package.ListComposite = model.PackageDetail.ListComposite;
                //=========
                ListItems.Add(package);
                modelAddItems.ListItems = ListItems;
                //====================
                string msg = "";
                bool success = false;
                OrderDetailModels OrderDetail = _yourcartFactory.AddItems(modelAddItems, ref success, ref msg);
                if (success)
                {
                    ORDERID = OrderDetail.ID;
                    if (OrderDetail.ListItems != null && OrderDetail.ListItems.Count > 0)
                        OrderDetail.TotalQuantity = (int)OrderDetail.ListItems.Sum(x => x.Quantity);
                    return Json(OrderDetail, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    if (!string.IsNullOrEmpty(msg))
                    {
                        if (msg.Equals("Payment is pending!") || msg.Equals("The receipt has been paid in full!"))
                        {
                            var Cookie = Request.Cookies["csc-order-v2"];
                            if (Cookie != null)
                            {
                                var Order = Cookie.Value;
                                var strOrder = Server.UrlDecode(Order);
                                var ListOrder = JsonConvert.DeserializeObject<List<CookieOrder>>(strOrder);
                                if (ListOrder != null && ListOrder.Any())
                                {
                                    var temp = ListOrder.FirstOrDefault(x => x.OrderId.Equals(ORDERID));
                                    if (temp != null)
                                    {
                                        ListOrder.Remove(temp);
                                        Cookie.Value = null;
                                        Response.Cookies.Add(Cookie);
                                        var strListOrder = JsonConvert.SerializeObject(ListOrder);
                                        Cookie.Value = strListOrder;
                                        Response.Cookies.Add(Cookie);
                                    }
                                }
                            }
                            ORDERID = "";
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, msg);
                        }
                    }
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, Commons.MsgErrorForClient);
                }

            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("OurProduct AddToCartOfPackage: ", ex);
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        public ActionResult EditAdditionsPackage(string Id)
        {
            try
            {
                PackageDetailViewModels model = new PackageDetailViewModels();
                model.PackageDetail = new OurProDuctsModel();

                if (!string.IsNullOrEmpty(ORDERID))
                {
                    YourCartFactory yourCartFactory = new YourCartFactory();
                    var dataOrder = yourCartFactory.GetOrderDetail(ORDERID);
                    if (dataOrder != null)
                    {
                        var lstItemOrder = dataOrder.ListItems.ToList();
                        var itemDetail = lstItemOrder.Where(w => w.ProductID == Id && w.IsDelete != true).FirstOrDefault();
                        if (itemDetail != null)
                        {
                            model.PackageDetail.ItemID = itemDetail.ID;
                            model.PackageDetail.ID = itemDetail.ProductID;
                            model.PackageDetail.Name = itemDetail.ProductName;

                            //=======Get List Product Child
                            if (itemDetail.ListComposite != null && itemDetail.ListComposite.Any())
                            {
                                itemDetail.ListComposite.ForEach(xx =>
                                {
                                    // Get list cate product of package
                                    if (xx.ProductType == (byte)Commons.EProductType.Product)
                                    {
                                        model.ListApplyAdditionProduct.Add(new ProductApplyAdditionPackage
                                        {
                                            ProductName = xx.ProductName,
                                            ProductId = xx.ProductID,
                                            ProductCateId = xx.ListCategory.Select(s => s.CategoryID).FirstOrDefault().ToString()
                                        });
                                    }
                                });
                            }

                            foreach (var itm in itemDetail.ListItem)
                            {

                                string ApplyProductName = null;
                                if (!string.IsNullOrEmpty(itm.ParentAddition))
                                {
                                    ApplyProductName = model.ListApplyAdditionProduct.Where(ww => ww.ProductId == itm.ParentAddition).Select(ss => ss.ProductName).FirstOrDefault();
                                }

                                model.ListAdditionBuy.Add(new OurProDuctsModel
                                {
                                    ItemID = itm.ID,
                                    ID = itm.ProductID,
                                    ApplyProductId = itm.ParentAddition,
                                    Code = ApplyProductName,
                                    Name = itm.ProductName,
                                    ProductType = (byte)Commons.EProductType.Addition,
                                    AdditionType = itm.AdditionType,
                                    Quantity = itm.Quantity,
                                    Period = itm.Period,
                                    PeriodType = itm.PeriodType,
                                    PeriodTime = GetPeriodTime(itm.PeriodType, itm.Period),
                                    Price = itm.Price,
                                    IsDelete = itm.IsDelete,
                                    AdditionTypeName = ((Commons.EAdditionType)Enum.ToObject(typeof(Commons.EAdditionType), itm.AdditionType)).ToString()
                                });
                            }

                            if (model.ListAdditionBuy != null && model.ListAdditionBuy.Any())
                            {
                                model.ListAdditionBuy = model.ListAdditionBuy.OrderBy(o => o.AdditionType).ThenBy(oo => oo.Sequence).ThenBy(oo => oo.Name).ToList();
                                int index = 0;
                                model.ListAdditionBuy.ForEach(xy =>
                                {
                                    xy.Index = index;
                                    index++;
                                });
                            }
                        }
                    }
                }
                //==========Get List Addition Type
                List<SelectListItem> ListAdditionType = new List<SelectListItem>();
                ListAdditionType = GetListAdditionType();
                ViewBag.ListAdditionType = ListAdditionType;
                model.CurrencySymbol = CurrencySymbol;

                return View("_BuyingAdditionsPackage", model);

            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("OurProducts EditAdditionsPackage: ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }

        public ActionResult AddToCartEditAdditionsPackage(PackageDetailViewModels model)
        {
            try
            {
                AddItemToOrderModels modelAddItems = new AddItemToOrderModels();
                modelAddItems.CusID = CurrentUser.UserId;
                modelAddItems.OrderID = ORDERID;
                List<Item> ListItems = new List<Item>();
                //===============
                Item package = new Item();
                package.ID = model.PackageDetail.ItemID;
                package.ProductID = model.PackageDetail.ID;
                package.ProductName = model.PackageDetail.Name;
                package.ProductType = (byte)Commons.EProductType.Package;
                package.ListItem = new List<Item>();
                foreach (var item in model.ListAdditionBuy)
                {
                    package.ListItem.Add(new Item
                    {
                        ID = item.ItemID,
                        ProductID = item.ID,
                        ParentAddition = item.ApplyProductId,
                        ProductName = item.Name,
                        ProductType = (byte)Commons.EProductType.Addition,
                        AdditionType = item.AdditionType,
                        Quantity = item.Quantity,
                        Period = item.Period,
                        PeriodType = item.PeriodType,
                        Price = item.Price,
                        IsDelete = item.IsDelete,
                        ItemGroup = (byte)Commons.EItemGroup.Addition
                    });
                }
                package.ListStoreApply = new List<ApplyStore>();
                package.ListFunction = new List<ItemFunction>();
                package.ListAdditionApply = new List<AdditionApply>();
                //=========
                ListItems.Add(package);
                modelAddItems.ListItems = ListItems;
                //====================
                string msg = "";
                bool success = false;
                OrderDetailModels OrderDetail = _yourcartFactory.AddItems(modelAddItems, ref success, ref msg);
                if (success)
                {
                    ORDERID = OrderDetail.ID;
                    return Json(OrderDetail, JsonRequestBehavior.AllowGet);
                }
                else
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, Commons.MsgErrorForClient);
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("OurProducts AddToCartEditAdditionsPackage: ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }

        public string GetPeriodTime(int PeriodType, int Period)
        {
            string PeriodTime = (PeriodType == (byte)Commons.EPeriodType.OneTime) ? Commons.PeriodTypeOneTime
                                            : Period + " " + ((Commons.EPeriodType)Enum.ToObject(typeof(Commons.EPeriodType), PeriodType)).ToString() + (Period > 1 ? "s" : "");
            return PeriodTime;
        }
        #endregion Package

        public string PeriodName(double Period, int PeriodType)
        {
            string _PeriodName = (PeriodType == (byte)Commons.EPeriodType.OneTime)
                                                ? Commons.PeriodTypeOneTime : ((Commons.EPeriodType)Enum.ToObject(typeof(Commons.EPeriodType), PeriodType)).ToString() + (Period > 1 ? "s" : "");
            return _PeriodName;
        }

        public ActionResult GetListCategory()
        {
            var _ListItemCategory = _factory.ProductGetCate();
            _ListItemCategory = _ListItemCategory.OrderBy(o => o.Sequence).ThenBy(oo => oo.Name).Where(x=>!x.IsFreeTrial).ToList();
            _ListItemCategory.ForEach(x=> {
                if (x.ShortDescription.Length > 60) {
                    x.ShortDescription = x.ShortDescription.Substring(0, 60) + "...";
                }
            });
            _ListItemCategory.ForEach(x => x.UrlEncrypt = CSCExtensions.SetStringEncrypt("/OurProducts/Index?ProductType=Products&CategoryID=" + x.ID));
            Session.Add("ListItemCategory", _ListItemCategory);
            return Json(_ListItemCategory, JsonRequestBehavior.AllowGet);
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

        #region//// Buy Item (Product/Package)
        [EncryptedActionParameter]
        public ActionResult ProductPackageDetail(string Id, string CateID)
        {
            try
            {
                PackageDetailViewModels model = new PackageDetailViewModels();

                // Set info for breadcrumb if item is product
                ViewBag.Title = "";
                ViewBag.CategoryID = "";
                ViewBag.CategoryName = "";

                var ListPackages = _factory.GetListData(CurrentUser.UserId, null, 0, null, null, Id);
                if (ListPackages != null && ListPackages.Any())
                {
                    model.PackageDetail = ListPackages.FirstOrDefault();
                    model.PackageDetail.CusId = CurrentUser.UserId;
                    model.PackageDetail.Quantity = 1;
                    if (model.PackageDetail.IsUnlimitedAmountOfUnit)
                        model.PackageDetail.AmountOfUnit = -1;

                    if (model.PackageDetail.IsUnlimitedNumberOfAccount)
                        model.PackageDetail.NumberOfAccount = -1;

                    if (model.PackageDetail.IsUnlimitedIncludeStore)
                    {
                        model.PackageDetail.NumOfStore = -1;
                        model.PackageDetail.IncludeStore = -1;
                    }
                    else
                        model.PackageDetail.NumOfStore = model.PackageDetail.IncludeStore;

                    if (model.PackageDetail.ImageURL == null || model.PackageDetail.ImageURL.Equals(""))
                        model.PackageDetail.ImageURL = _ImgDummyPackage;

                    if (model.PackageDetail.ListPrice != null && model.PackageDetail.ListPrice.Any())
                    {
                        model.PackageDetail.ListPrice.ForEach(x =>
                        {
                            x.NamePeriodType = PeriodName(x.Period, x.PeriodType);
                        });

                        var PriceMin = model.PackageDetail.ListPrice.Where(x => !x.IsExtend).OrderBy(w => w.PeriodType).ThenBy(w => w.Period).ThenBy(w => w.Price).FirstOrDefault();

                        model.PackageDetail.TotalPrice = PriceMin.Price;
                        model.PackageDetail.Price = PriceMin.Price;

                        model.PackageDetail.Period = (int)PriceMin.Period;
                        model.PackageDetail.PeriodType = PriceMin.PeriodType;

                        // Period Time != One Time
                        if (model.PackageDetail.PeriodType != (byte)Commons.EPeriodType.OneTime)
                        {
                            model.PackageDetail.PeriodTime = PriceMin.Period + " " + PriceMin.NamePeriodType;
                        }
                        else
                        {
                            model.PackageDetail.PeriodTime = PriceMin.NamePeriodType;
                        }
                    }

                    // If item is package
                    if (model.PackageDetail.ProductType == (byte)Commons.EProductType.Package)
                    {
                        ViewBag.Title = "Package - " + model.PackageDetail.Name;

                        //=======Get List Product Child
                        if (model.PackageDetail.ListProductChild != null && model.PackageDetail.ListProductChild.Any())
                        {
                            //update bug of Long by Trongntn 22-11-2017
                            model.PackageDetail.ListProductChild = model.PackageDetail.ListProductChild.Where(x => x.ProductType == (byte)Commons.EProductType.Product).ToList();

                            model.PackageDetail.ListProductChild.ForEach(xx =>
                            {
                                // Get list cate product of package
                                if (xx.ProductType == (byte)Commons.EProductType.Product)
                                {
                                    model.ListApplyAdditionProduct.Add(new ProductApplyAdditionPackage
                                    {
                                        ProductName = xx.Name,
                                        ProductId = xx.ID,
                                        ProductCateId = xx.ListCategory.Select(s => s.CategoryID).FirstOrDefault().ToString()
                                    });
                                }
                            });
                        }

                        // If item is package from list recommend in Product list
                        if (!string.IsNullOrEmpty(CateID))
                        {
                            var listCate = ((List<CategoryDetailModels>)Session["ListItemCategory"]);
                            if (listCate != null)
                            {
                                var CateDetail = listCate.Where(x => x.ID.Equals(CateID)).FirstOrDefault();
                                // Set info for breadcrumb if item is product
                                ViewBag.CategoryID = CateDetail == null ? "" : CateDetail.ID;
                                ViewBag.CategoryName = CateDetail == null ? "" : CateDetail.Name;
                            }
                        }
                    }
                    // If item is product
                    else if (model.PackageDetail.ProductType == (byte)Commons.EProductType.Product)
                    {
                        ViewBag.Title = "Product - " + model.PackageDetail.Name;

                        if (model.PackageDetail.ListCategory != null && model.PackageDetail.ListCategory.Any())
                        {
                            var infoCategory = model.PackageDetail.ListCategory.FirstOrDefault();
                            model.ListApplyAdditionProduct.Add(new ProductApplyAdditionPackage
                            {

                                ProductName = model.PackageDetail.Name,
                                ProductId = model.PackageDetail.ID,
                                ProductCateId = infoCategory.CategoryID
                            });

                            // Set info for breadcrumb if item is product
                            ViewBag.CategoryID = infoCategory.CategoryID;
                            ViewBag.CategoryName = infoCategory.CategoryName;
                        }
                    }


                    //==========Get List Addition Type
                    List<SelectListItem> ListAdditionType = new List<SelectListItem>();
                    ListAdditionType = GetListAdditionType();
                    ViewBag.ListAdditionType = ListAdditionType;

                    model.PackageDetail.CurrencySymbol = CurrencySymbol;
                    if (model.PackageDetail.ListComposite != null && model.PackageDetail.ListComposite.Any())
                    {
                        //Product
                        model.PackageDetail.ListProductOnPackage = model.PackageDetail.ListComposite
                            .Where(ww => ww.ProductType == (byte)Commons.EProductType.Product)
                            .OrderBy(ww => ww.Sequence).ThenBy(ww => ww.ProductName).ToList();
                        model.PackageDetail.ListProductOnPackage.ForEach(x =>
                        {
                            if (x.Quantity == -1)
                                x.ProductName = x.ProductName + " (For unlimited devices)";
                            else
                                x.ProductName = x.ProductName + " (For " + x.Quantity + " device" + (x.Quantity > 1 ? "s" : "") + ")";
                        });

                        //Addition
                        model.PackageDetail.ListComposite = model.PackageDetail.ListComposite
                            .Where(ww => ww.ProductType != (byte)Commons.EProductType.Product)
                            .OrderBy(ww => ww.Sequence).ThenBy(ww => ww.ProductName).ToList();
                    }

                }
                return View("ProductPackage/_ProductPackageDetail", model);
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("OurProducts ProductPackageDetail: ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }

        public ActionResult GetListAdditionForProductPackage(int AdditionType, List<ProductApplyAdditionPackage> ListApplyAdditionProduct)
        {
            try
            {
                PackageViewModels model = new PackageViewModels();

                model.ListProducts = _factory.GetListData(CurrentUser.UserId, "", (byte)Commons.EProductType.Addition);

                if (AdditionType > 0)
                {
                    model.ListProducts = model.ListProducts.Where(x => x.AdditionType == AdditionType).ToList();
                }

                //// Get Additions can apply for products in this package
                //if (ListApplyAdditionProduct != null && ListApplyAdditionProduct.Any())
                //{
                //    var ListCateApplicable = ListApplyAdditionProduct.Select(s => s.ProductCateId).Distinct().ToList();
                //    // Addition is a Hardware || Service || Function 
                //    // Addition is a Software || Location || Account has any product category of package in list category || IsFullCate == true
                //    model.ListProducts = model.ListProducts.Where(w => (w.AdditionType == (byte)Commons.EAdditionType.Hardware || w.AdditionType == (byte)Commons.EAdditionType.Service || w.AdditionType == (byte)Commons.EAdditionType.Function)
                //    || ((w.AdditionType == (byte)Commons.EAdditionType.Software || w.AdditionType == (byte)Commons.EAdditionType.Location || w.AdditionType == (byte)Commons.EAdditionType.Account) && (w.IsFullCate == true || (w.ListCategory != null && w.ListCategory.Any() && w.ListCategory.Where(ww => ListCateApplicable.Contains(ww.CategoryID)).Any())))).ToList();
                //}
                //else
                //{
                //    // Addition is a Hardware || Service || Function 
                //    model.ListProducts = model.ListProducts.Where(w => w.AdditionType == (byte)Commons.EAdditionType.Hardware || w.AdditionType == (byte)Commons.EAdditionType.Service || w.AdditionType == (byte)Commons.EAdditionType.Function).ToList();
                //}

                var gData = model.ListProducts.OrderBy(o => o.AdditionType).GroupBy(x => x.AdditionType).Select(x => new BuyingAdditionViewModels
                {
                    ID = x.Key,
                    ListItem = x.ToList()
                }).ToList();

                gData.ForEach(x =>
                {
                    x.Name = ((Commons.EAdditionType)Enum.ToObject(typeof(Commons.EAdditionType), x.ID)).ToString();
                    int OffSet = 0;
                    x.ListItem = x.ListItem.OrderBy(o => o.Sequence).ThenBy(oo => oo.Name).ToList();
                    x.ListItem.ForEach(z =>
                    {
                        z.Index = OffSet++;
                        //================
                        z.AdditionTypeName = x.Name;
                        if (z.ListPrice != null && z.ListPrice.Count > 0)
                        {
                            z.ListPrice.ForEach(xx =>
                            {
                                xx.NamePeriodType = PeriodName(xx.Period, xx.PeriodType);
                            });

                            var PriceMin = z.ListPrice.OrderBy(o => o.PeriodType).ThenBy(w => w.Period).ThenBy(w => w.Price).FirstOrDefault();
                            z.Price = PriceMin.Price;
                            z.Period = (int)PriceMin.Period;
                            z.PeriodType = PriceMin.PeriodType;

                            // Period Time != One Time
                            if (z.PeriodType != (byte)Commons.EPeriodType.OneTime)
                            {
                                z.PeriodTime = PriceMin.Period + " " + PriceMin.NamePeriodType;
                            }
                            else
                            {
                                z.PeriodTime = PriceMin.NamePeriodType;
                            }
                        }
                        // Get list product apply addition
                        if (ListApplyAdditionProduct != null && ListApplyAdditionProduct.Any())
                        {
                            if (z.AdditionType != (byte)Commons.EAdditionType.Hardware && z.AdditionType != (byte)Commons.EAdditionType.Service && z.AdditionType != (byte)Commons.EAdditionType.Function)
                            {
                                z.ListProductApplicable = new List<SelectListItem>();
                                //if (z.IsFullCate == true)
                                //{
                                //    ListApplyAdditionProduct.ForEach(y =>
                                //    {
                                //        z.ListProductApplicable.Add(new SelectListItem
                                //        {
                                //            Text = y.ProductName,
                                //            Value = y.ProductId
                                //        });
                                //    });
                                //}
                                //else
                                //{
                                //    var lstApplyAdditionProduct = ListApplyAdditionProduct.Where(w => (z.ListCategory.Select(s => s.CategoryID).ToList()).Contains(w.ProductCateId)).ToList();
                                //    lstApplyAdditionProduct.ForEach(y =>
                                //    {
                                //        z.ListProductApplicable.Add(new SelectListItem
                                //        {
                                //            Text = y.ProductName,
                                //            Value = y.ProductId
                                //        });
                                //    });
                                //}
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
                });
                gData = gData.OrderBy(x => x.Name).ToList();
                return PartialView("ProductPackage/_AdditionsProductPackage", gData);
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("OurProducts GetListAdditionForProductPackage: ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }
        public ActionResult AddItemAdditionProductPackage(OurProDuctsModel data)
        {
            try
            {
                OurProDuctsModel model = new OurProDuctsModel();
                model.ID = data.ID;
                model.Code = data.Code;
                model.Index = data.Index;
                model.AdditionType = data.AdditionType;
                model.AdditionTypeName = data.AdditionTypeName;
                model.Name = data.Name;
                model.Quantity = data.Quantity;
                model.Period = data.Period;
                model.PeriodType = data.PeriodType;
                model.Price = data.Price;
                model.ApplyProductId = data.ApplyProductId;
                model.AmountOfUnit = data.AmountOfUnit;
                // Amount Addition : quantity * price
                model.Amount = 0;
                if (model.Quantity > 0 && model.Price > 0)
                {
                    model.Amount = model.Quantity * model.Price;
                }
                if (data.PeriodType != (int)Commons.EPeriodType.OneTime)
                    model.PeriodTime = data.Period + " " + PeriodName(data.Period, data.PeriodType);
                else
                    model.PeriodTime = PeriodName(data.Period, data.PeriodType);

                return PartialView("ProductPackage/_ItemAdditionProductPackage", model);
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("OurProducts AddItemAdditionProductPackage: ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult AddToCartOfProductPackage(PackageDetailViewModels model)
        {
            try
            {
                AddItemToOrderModels modelAddItems = new AddItemToOrderModels();
                modelAddItems.CusID = CurrentUser.UserId;
                modelAddItems.OrderID = ORDERID;
                List<Item> ListItems = new List<Item>();
                //===============
                Item package = new Item();
                package.ID = "";
                package.ProductID = model.PackageDetail.ID;
                package.ProductName = model.PackageDetail.Name;
                package.ProductType = model.PackageDetail.ProductType;
                package.AdditionType = model.PackageDetail.AdditionType;
                package.ImageURL = model.PackageDetail.ImageURL;
                package.Period = model.PackageDetail.Period;
                package.PeriodType = model.PackageDetail.PeriodType;
                package.Quantity = model.PackageDetail.Quantity;
                package.Price = model.PackageDetail.Price;

                package.NumOfStore = model.PackageDetail.NumOfStore;
                package.NumOfAccount = model.PackageDetail.NumberOfAccount;
                package.AmountOfUnit = model.PackageDetail.AmountOfUnit;

                package.PromotionID = model.PackageDetail.PromotionID;
                package.PromotionName = model.PackageDetail.PromotionName;
                package.PromotionAmount = model.PackageDetail.PromotionAmount;
                package.IsApplyPromotion = model.PackageDetail.IsApplyPromotion;

                package.DiscountID = model.PackageDetail.DiscountID;
                package.DiscountName = model.PackageDetail.DiscountName;
                package.DiscountAmount = model.PackageDetail.DiscountAmount;
                package.DiscountValue = model.PackageDetail.DiscountValue;
                package.DiscountType = model.PackageDetail.DiscountType;

                package.Description = model.PackageDetail.Description;
                package.ItemGroup = model.PackageDetail.ItemGroup;
                package.IsDelete = model.PackageDetail.IsDelete;

                package.ListItem = new List<Item>();
                foreach (var item in model.ListAdditionBuy.Where(w => w.IsDelete != true).ToList())
                {
                    package.ListItem.Add(new Item
                    {
                        ID = "",
                        ProductID = item.ID,
                        ParentAddition = item.ApplyProductId,
                        ProductName = item.Name,
                        ProductType = (byte)Commons.EProductType.Addition,
                        AdditionType = item.AdditionType,
                        Quantity = item.Quantity,
                        Period = item.Period,
                        PeriodType = item.PeriodType,
                        Price = item.Price,
                        IsDelete = item.IsDelete,
                        AmountOfUnit = item.AmountOfUnit,
                        ItemGroup = (byte)Commons.EItemGroup.Addition
                    });
                }
                package.ListStoreApply = new List<ApplyStore>();
                package.ListAdditionApply = new List<AdditionApply>();

                package.ListFunction = new List<ItemFunction>();
                //// If Item is product
                //if (model.PackageDetail.ProductType == (byte)Commons.EProductType.Product && model.PackageDetail.ListFunction != null && model.PackageDetail.ListFunction.Any())
                //{
                //    model.PackageDetail.ListFunction.ForEach(xy => {
                //        package.ListFunction.Add(new ItemFunction
                //        {
                //            ID = xy.FunctionID,
                //            Name = xy.FunctionName
                //        });
                //    });
                //}

                // Get all composite (products + additions)
                if (model.PackageDetail.ListComposite == null)
                    model.PackageDetail.ListComposite = new List<ProductCompositeModels>();
                if (model.PackageDetail.ListProductOnPackage != null && model.PackageDetail.ListProductOnPackage.Any())
                    model.PackageDetail.ListComposite.AddRange(model.PackageDetail.ListProductOnPackage);

                package.ListComposite = model.PackageDetail.ListComposite;
                //=========
                ListItems.Add(package);
                modelAddItems.ListItems = ListItems;
                modelAddItems.IsFree = Convert.ToBoolean(Session["IsFree"]);
                //====================
                string msg = "";
                bool success = false;
                OrderDetailModels OrderDetail = _yourcartFactory.AddItems(modelAddItems, ref success, ref msg);
                if (success)
                {
                    ORDERID = OrderDetail.ID;
                    if (OrderDetail.ListItems != null && OrderDetail.ListItems.Count > 0)
                        OrderDetail.TotalQuantity = (int)OrderDetail.ListItems.Sum(x => x.Quantity);
                    return Json(OrderDetail, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    if (!string.IsNullOrEmpty(msg))
                    {
                        if (msg.Equals("Payment is pending!") || msg.Equals("The receipt has been paid in full!"))
                        {
                            var Cookie = Request.Cookies["csc-order-v2"];
                            if (Cookie != null)
                            {
                                var Order = Cookie.Value;
                                var strOrder = Server.UrlDecode(Order);
                                var ListOrder = JsonConvert.DeserializeObject<List<CookieOrder>>(strOrder);
                                if (ListOrder != null && ListOrder.Any())
                                {
                                    var temp = ListOrder.FirstOrDefault(x => x.OrderId.Equals(ORDERID));
                                    if (temp != null)
                                    {
                                        ListOrder.Remove(temp);
                                        Cookie.Value = null;
                                        Response.Cookies.Add(Cookie);
                                        var strListOrder = JsonConvert.SerializeObject(ListOrder);
                                        Cookie.Value = strListOrder;
                                        Response.Cookies.Add(Cookie);
                                    }
                                }
                            }
                            ORDERID = "";
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, msg);
                        }
                        else if(msg.Equals("The current order contains free trial product. Please checkout this order before buying other items."))
                        {
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, msg);
                        }
                    }
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, Commons.MsgErrorForClient);
                }

            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("OurProduct AddToCartOfProductPackage: ", ex);
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [EncryptedActionParameter]
        public ActionResult EditAdditionsProductPackage(string AssetID)
        {
            try
            {
                PackageDetailViewModels model = new PackageDetailViewModels();
                model.PackageDetail = new OurProDuctsModel();

                // Set info for breadcrumb if item is product
                ViewBag.Title = "Buying Additions";
                ViewBag.CategoryID = "";
                ViewBag.CategoryName = "";

                if (!string.IsNullOrEmpty(ORDERID))
                {
                    YourCartFactory yourCartFactory = new YourCartFactory();
                    var dataOrder = yourCartFactory.GetOrderDetail(ORDERID);
                    if (dataOrder != null)
                    {
                        var lstItemOrder = dataOrder.ListItems.ToList();
                        var itemDetail = lstItemOrder.Where(w => w.ID == AssetID && w.IsDelete != true).FirstOrDefault();
                        if (itemDetail != null)
                        {
                            model.PackageDetail.ItemID = itemDetail.ID;
                            model.PackageDetail.ID = itemDetail.ProductID;
                            model.PackageDetail.Name = itemDetail.ProductName;
                            model.PackageDetail.ProductType = itemDetail.ProductType;

                            ViewBag.Title = "Buying Additions for " + model.PackageDetail.Name;

                            // If item is package
                            if (model.PackageDetail.ProductType == (byte)Commons.EProductType.Package)
                            {
                                //=======Get List Product Child
                                if (itemDetail.ListComposite != null && itemDetail.ListComposite.Any())
                                {
                                    itemDetail.ListComposite.ForEach(xx =>
                                    {
                                        // Get list cate product of package
                                        if (xx.ProductType == (byte)Commons.EProductType.Product)
                                        {
                                            model.ListApplyAdditionProduct.Add(new ProductApplyAdditionPackage
                                            {
                                                ProductName = xx.ProductName,
                                                ProductId = xx.ProductID,
                                                //ProductCateId = xx.ListCategory.Select(s => s.CategoryID).FirstOrDefault().ToString()
                                            });
                                        }
                                    });
                                }
                            }
                            // If item is product
                            else if (model.PackageDetail.ProductType == (byte)Commons.EProductType.Product)
                            {
                                if (itemDetail.ListCategory != null && itemDetail.ListCategory.Any())
                                {
                                    var infoCategory = itemDetail.ListCategory.FirstOrDefault();
                                    ViewBag.CategoryID = infoCategory.CategoryID;
                                    ViewBag.CategoryName = infoCategory.CategoryName;
                                }
                                model.ListApplyAdditionProduct.Add(new ProductApplyAdditionPackage
                                {
                                    ProductName = model.PackageDetail.Name,
                                    ProductId = model.PackageDetail.ID,
                                    //ProductCateId = model.PackageDetail.ListCategory.Select(s => s.CategoryID).FirstOrDefault().ToString()
                                });
                            }

                            double amountAddition = 0;
                            foreach (var itm in itemDetail.ListItem)
                            {

                                string ApplyProductName = null;
                                if (!string.IsNullOrEmpty(itm.ParentAddition))
                                {
                                    ApplyProductName = model.ListApplyAdditionProduct.Where(ww => ww.ProductId == itm.ParentAddition).Select(ss => ss.ProductName).FirstOrDefault();
                                }

                                amountAddition = 0;
                                if (itm.Quantity > 0 && itm.Price > 0)
                                {
                                    amountAddition = itm.Quantity * itm.Price;
                                }
                                model.ListAdditionBuy.Add(new OurProDuctsModel
                                {
                                    ItemID = itm.ID,
                                    ID = itm.ProductID,
                                    ApplyProductId = itm.ParentAddition,
                                    Code = ApplyProductName,
                                    Name = itm.ProductName,
                                    ProductType = (byte)Commons.EProductType.Addition,
                                    AdditionType = itm.AdditionType,
                                    Quantity = itm.Quantity,
                                    Period = itm.Period,
                                    PeriodType = itm.PeriodType,
                                    PeriodTime = GetPeriodTime(itm.PeriodType, itm.Period),
                                    Price = itm.Price,
                                    IsDelete = itm.IsDelete,
                                    Amount = amountAddition,
                                    AmountOfUnit = itm.AmountOfUnit,
                                    AdditionTypeName = ((Commons.EAdditionType)Enum.ToObject(typeof(Commons.EAdditionType), itm.AdditionType)).ToString()
                                });
                            }

                            if (model.ListAdditionBuy != null && model.ListAdditionBuy.Any())
                            {
                                model.ListAdditionBuy = model.ListAdditionBuy.OrderBy(o => o.AdditionType).ThenBy(oo => oo.Sequence).ThenBy(oo => oo.Name).ToList();
                                int index = 0;
                                model.ListAdditionBuy.ForEach(xy =>
                                {
                                    xy.Index = index;
                                    index++;
                                });
                            }
                        }
                    }
                }
                //==========Get List Addition Type
                List<SelectListItem> ListAdditionType = new List<SelectListItem>();
                ListAdditionType = GetListAdditionType();
                ViewBag.ListAdditionType = ListAdditionType;
                model.CurrencySymbol = CurrencySymbol;

                return View("ProductPackage/_BuyingAdditionsProductPackage", model);

            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("OurProducts EditAdditionsProductPackage: ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }

        public ActionResult AddToCartEditAdditionsProductPackage(PackageDetailViewModels model)
        {
            try
            {
                AddItemToOrderModels modelAddItems = new AddItemToOrderModels();
                modelAddItems.CusID = CurrentUser.UserId;
                modelAddItems.OrderID = ORDERID;
                List<Item> ListItems = new List<Item>();
                //===============
                Item package = new Item();
                package.ID = model.PackageDetail.ItemID;
                package.ProductID = model.PackageDetail.ID;
                package.ProductName = model.PackageDetail.Name;
                package.ProductType = (byte)Commons.EProductType.Package;
                package.ListItem = new List<Item>();
                foreach (var item in model.ListAdditionBuy)
                {
                    package.ListItem.Add(new Item
                    {
                        ID = item.ItemID,
                        ProductID = item.ID,
                        ParentAddition = item.ApplyProductId,
                        ProductName = item.Name,
                        ProductType = (byte)Commons.EProductType.Addition,
                        AdditionType = item.AdditionType,
                        Quantity = item.Quantity,
                        Period = item.Period,
                        PeriodType = item.PeriodType,
                        Price = item.Price,
                        IsDelete = item.IsDelete,
                        AmountOfUnit = item.AmountOfUnit,
                        ItemGroup = (byte)Commons.EItemGroup.Addition
                    });
                }
                package.ListStoreApply = new List<ApplyStore>();
                package.ListFunction = new List<ItemFunction>();
                package.ListAdditionApply = new List<AdditionApply>();
                //=========
                ListItems.Add(package);
                modelAddItems.ListItems = ListItems;
                //====================
                string msg = "";
                bool success = false;
                OrderDetailModels OrderDetail = _yourcartFactory.AddItems(modelAddItems, ref success, ref msg);
                if (success)
                {
                    ORDERID = OrderDetail.ID;
                    if (OrderDetail.ListItems != null && OrderDetail.ListItems.Count > 0)
                        OrderDetail.TotalQuantity = (int)OrderDetail.ListItems.Sum(x => x.Quantity);
                    return Json(OrderDetail, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    if (!string.IsNullOrEmpty(msg))
                    {
                        if (msg.Equals("Payment is pending!") || msg.Equals("The receipt has been paid in full!"))
                        {
                            var Cookie = Request.Cookies["csc-order-v2"];
                            if (Cookie != null)
                            {
                                var Order = Cookie.Value;
                                var strOrder = Server.UrlDecode(Order);
                                var ListOrder = JsonConvert.DeserializeObject<List<CookieOrder>>(strOrder);
                                if (ListOrder != null && ListOrder.Any())
                                {
                                    var temp = ListOrder.FirstOrDefault(x => x.OrderId.Equals(ORDERID));
                                    if (temp != null)
                                    {
                                        ListOrder.Remove(temp);
                                        Cookie.Value = null;
                                        Response.Cookies.Add(Cookie);
                                        var strListOrder = JsonConvert.SerializeObject(ListOrder);
                                        Cookie.Value = strListOrder;
                                        Response.Cookies.Add(Cookie);
                                    }
                                }
                            }
                            ORDERID = "";
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, msg);
                        }
                    }
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, Commons.MsgErrorForClient);
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("OurProducts AddToCartEditAdditionsProductPackage: ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }
        #endregion

        #region//// Buy Additions from My Store & Business
        [EncryptedActionParameter]
        public ActionResult BuyingAdditionsStoreBusiness(string AssetID)
        {
            try
            {
                PackageDetailViewModels model = new PackageDetailViewModels();
                model.PackageDetail = new OurProDuctsModel();

                // Set info for breadcrumb if item is product
                ViewBag.Title = "Buying Additions";
                ViewBag.CategoryID = "";
                ViewBag.CategoryName = "";

                var dataOrder = _factory.GetProductApplyAdditionsStoreBusiness(AssetID, CurrentUser.UserId);

                if (dataOrder != null && dataOrder.Any())
                {
                    var itemDetail = dataOrder.Where(ww=>ww.AssetID == AssetID && (ww.ProductType == (byte)Commons.EProductType.Product || ww.ProductType == (byte)Commons.EProductType.Package)).FirstOrDefault();
                    if (itemDetail != null)
                    {

                        model.PackageDetail.ItemID = itemDetail.AssetID;
                        model.PackageDetail.ID = itemDetail.ProductID;
                        model.PackageDetail.Name = itemDetail.ProductName;
                        model.PackageDetail.ProductType = itemDetail.ProductType;

                        ViewBag.Title = "Buying Additions for " + model.PackageDetail.Name;

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
                                        //ProductCateId = xx.ListCategory.Select(s => s.CategoryID).FirstOrDefault().ToString()
                                    });
                                });
                            }
                        }
                        // If item is product
                        else if (model.PackageDetail.ProductType == (byte)Commons.EProductType.Product)
                        {
                            if (itemDetail.ListCategory != null && itemDetail.ListCategory.Any())
                            {
                                var infoCategory = itemDetail.ListCategory.FirstOrDefault();
                                ViewBag.CategoryID = infoCategory.CategoryID;
                                ViewBag.CategoryName = infoCategory.CategoryName;
                            }
                            model.ListApplyAdditionProduct.Add(new ProductApplyAdditionPackage
                            {
                                ProductName = model.PackageDetail.Name,
                                ProductId = model.PackageDetail.ItemID,
                                //ProductCateId = model.PackageDetail.ListCategory.Select(s => s.CategoryID).FirstOrDefault().ToString()
                            });
                        }
                    }
                }
                //==========Get List Addition Type
                List<SelectListItem> ListAdditionType = new List<SelectListItem>();
                ListAdditionType = GetListAdditionType();
                ViewBag.ListAdditionType = ListAdditionType;
                model.CurrencySymbol = CurrencySymbol;

                return View("StoreBusiness/_BuyingAdditionsStoreBusiness", model);

            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("OurProducts BuyingAdditionsStoreBusiness: ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }

        public ActionResult GetListAdditionsForStoreBusiness(int AdditionType, List<ProductApplyAdditionPackage> ListApplyAdditionProduct)
        {
            try
            {
                PackageViewModels model = new PackageViewModels();

                model.ListProducts = _factory.GetListData(CurrentUser.UserId, "", (byte)Commons.EProductType.Addition);

                if (AdditionType > 0)
                {
                    model.ListProducts = model.ListProducts.Where(x => x.AdditionType == AdditionType).ToList();
                }

                //// Get Additions can apply for products in this package
                //if (ListApplyAdditionProduct != null && ListApplyAdditionProduct.Any())
                //{
                //    var ListCateApplicable = ListApplyAdditionProduct.Select(s => s.ProductCateId).Distinct().ToList();
                //    // Addition is a Hardware || Service || Function 
                //    // Addition is a Software || Location || Account has any product category of package in list category || IsFullCate == true
                //    model.ListProducts = model.ListProducts.Where(w => (w.AdditionType == (byte)Commons.EAdditionType.Hardware || w.AdditionType == (byte)Commons.EAdditionType.Service || w.AdditionType == (byte)Commons.EAdditionType.Function)
                //    || ((w.AdditionType == (byte)Commons.EAdditionType.Software || w.AdditionType == (byte)Commons.EAdditionType.Location || w.AdditionType == (byte)Commons.EAdditionType.Account) && (w.IsFullCate == true || (w.ListCategory != null && w.ListCategory.Any() && w.ListCategory.Where(ww => ListCateApplicable.Contains(ww.CategoryID)).Any())))).ToList();
                //}
                //else
                //{
                //    // Addition is a Hardware || Service || Function 
                //    model.ListProducts = model.ListProducts.Where(w => w.AdditionType == (byte)Commons.EAdditionType.Hardware || w.AdditionType == (byte)Commons.EAdditionType.Service || w.AdditionType == (byte)Commons.EAdditionType.Function).ToList();
                //}

                var gData = model.ListProducts.OrderBy(o => o.AdditionType).GroupBy(x => x.AdditionType).Select(x => new BuyingAdditionViewModels
                {
                    ID = x.Key,
                    ListItem = x.ToList()
                }).ToList();

                gData.ForEach(x =>
                {
                    x.Name = ((Commons.EAdditionType)Enum.ToObject(typeof(Commons.EAdditionType), x.ID)).ToString();
                    int OffSet = 0;
                    x.ListItem = x.ListItem.OrderBy(o => o.Sequence).ThenBy(oo => oo.Name).ToList();
                    x.ListItem.ForEach(z =>
                    {
                        z.Index = OffSet++;
                        //================
                        z.AdditionTypeName = x.Name;
                        if (z.ListPrice != null && z.ListPrice.Count > 0)
                        {
                            z.ListPrice.ForEach(xx =>
                            {
                                xx.NamePeriodType = PeriodName(xx.Period, xx.PeriodType);
                            });

                            var PriceMin = z.ListPrice.OrderBy(o => o.PeriodType).ThenBy(w => w.Period).ThenBy(w => w.Price).FirstOrDefault();
                            z.Price = PriceMin.Price;
                            z.Period = (int)PriceMin.Period;
                            z.PeriodType = PriceMin.PeriodType;

                            // Period Time != One Time
                            if (z.PeriodType != (byte)Commons.EPeriodType.OneTime)
                            {
                                z.PeriodTime = PriceMin.Period + " " + PriceMin.NamePeriodType;
                            }
                            else
                            {
                                z.PeriodTime = PriceMin.NamePeriodType;
                            }
                        }
                        // Get list product apply addition
                        if (ListApplyAdditionProduct != null && ListApplyAdditionProduct.Any())
                        {
                            if (z.AdditionType != (byte)Commons.EAdditionType.Hardware && z.AdditionType != (byte)Commons.EAdditionType.Service && z.AdditionType != (byte)Commons.EAdditionType.Function)
                            {
                                z.ListProductApplicable = new List<SelectListItem>();
                                //if (z.IsFullCate == true)
                                //{
                                //    ListApplyAdditionProduct.ForEach(y =>
                                //    {
                                //        z.ListProductApplicable.Add(new SelectListItem
                                //        {
                                //            Text = y.ProductName,
                                //            Value = y.ProductId
                                //        });
                                //    });
                                //}
                                //else
                                //{
                                //    var lstApplyAdditionProduct = ListApplyAdditionProduct.Where(w => (z.ListCategory.Select(s => s.CategoryID).ToList()).Contains(w.ProductCateId)).ToList();
                                //    lstApplyAdditionProduct.ForEach(y =>
                                //    {
                                //        z.ListProductApplicable.Add(new SelectListItem
                                //        {
                                //            Text = y.ProductName,
                                //            Value = y.ProductId
                                //        });
                                //    });
                                //}
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
                });
                gData = gData.OrderBy(x => x.Name).ToList();
                return PartialView("StoreBusiness/_AdditionsStoreBusiness", gData);
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("OurProducts GetListAdditionsForStoreBusiness: ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }
        public ActionResult AddItemAdditionStoreBusiness(OurProDuctsModel data)
        {
            try
            {
                OurProDuctsModel model = new OurProDuctsModel();
                model.ID = data.ID;
                model.Code = data.Code;
                model.Index = data.Index;
                model.AdditionType = data.AdditionType;
                model.AdditionTypeName = data.AdditionTypeName;
                model.Name = data.Name;
                model.Quantity = data.Quantity;
                model.Period = data.Period;
                model.PeriodType = data.PeriodType;
                model.Price = data.Price;
                model.ApplyProductId = data.ApplyProductId;
                model.AmountOfUnit = data.AmountOfUnit;
                model.Description = data.Description;
                model.ImageURL = data.ImageURL;
                model.Selected = true;
                // Amount Addition : quantity * price
                model.Amount = 0;
                if (model.Quantity > 0 && model.Price > 0)
                {
                    model.Amount = model.Quantity * model.Price;
                }
                if (data.PeriodType != (int)Commons.EPeriodType.OneTime)
                    model.PeriodTime = data.Period + " " + PeriodName(data.Period, data.PeriodType);
                else
                    model.PeriodTime = PeriodName(data.Period, data.PeriodType);

                return PartialView("StoreBusiness/_ItemAdditionStoreBusiness", model);
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("OurProducts AddItemAdditionStoreBusiness: ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }

        [ValidateInput(false)]
        public ActionResult AddToCartBuyingAdditionsStoreBusiness(PackageDetailViewModels model)
        {
            try
            {
                AddItemToOrderModels modelAddItems = new AddItemToOrderModels();
                modelAddItems.CusID = CurrentUser.UserId;
                modelAddItems.OrderID = ORDERID;
                //====================
                model.ListAdditionBuy = model.ListAdditionBuy.Where(x => x.Selected).ToList();
                List<Item> ListItems = new List<Item>();
                if (model.ListAdditionBuy != null && model.ListAdditionBuy.Any())
                {
                    Item addition = new Item();
                    model.ListAdditionBuy.ForEach(itm =>
                    {
                        //===============
                        addition = new Item();
                        addition.ID = "";
                        addition.ProductID = itm.ID;
                        addition.ProductName = itm.Name;
                        addition.ProductType = itm.ProductType;
                        addition.AdditionType = itm.AdditionType;
                        addition.ImageURL = itm.ImageURL;
                        addition.Period = itm.Period;
                        addition.PeriodType = itm.PeriodType;
                        addition.Quantity = itm.Quantity;
                        addition.Price = itm.Price;
                        addition.AmountOfUnit = itm.AmountOfUnit;
                        // Number of store = Number of unit
                        addition.NumOfStore = itm.AmountOfUnit;

                        addition.Description = itm.Description;
                        addition.ItemGroup = itm.ItemGroup;
                        addition.IsDelete = itm.IsDelete;

                        addition.ListItem = new List<Item>();
                        addition.ListStoreApply = new List<ApplyStore>();
                        addition.ListFunction = new List<ItemFunction>();
                        addition.ListAdditionApply = new List<AdditionApply>();

                        // Set product apply
                        if (!string.IsNullOrEmpty(itm.ApplyProductId))
                        {
                            addition.ListAdditionApply.Add(new AdditionApply
                            {
                                ID = itm.ApplyProductId
                            });
                        }
                        else if(model.ListApplyAdditionProduct != null && model.ListApplyAdditionProduct.Any())
                        {
                            addition.ListAdditionApply.Add(new AdditionApply
                            {
                                ID = model.ListApplyAdditionProduct.Select(ss => ss.ProductId).FirstOrDefault(),
                            });
                        }
                        //=========
                        ListItems.Add(addition);
                    });
                }
                modelAddItems.ListItems = ListItems;
                //====================
                string msg = "";
                bool success = false;
                OrderDetailModels OrderDetail = _yourcartFactory.AddItems(modelAddItems, ref success, ref msg);
                if (success)
                {
                    ORDERID = OrderDetail.ID;
                    if (OrderDetail.ListItems != null && OrderDetail.ListItems.Count > 0)
                        OrderDetail.TotalQuantity = (int)OrderDetail.ListItems.Sum(x => x.Quantity);
                    return Json(OrderDetail, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    if (!string.IsNullOrEmpty(msg))
                    {
                        if (msg.Equals("Payment is pending!") || msg.Equals("The receipt has been paid in full!"))
                        {
                            var Cookie = Request.Cookies["csc-order-v2"];
                            if (Cookie != null)
                            {
                                var Order = Cookie.Value;
                                var strOrder = Server.UrlDecode(Order);
                                var ListOrder = JsonConvert.DeserializeObject<List<CookieOrder>>(strOrder);
                                if (ListOrder != null && ListOrder.Any())
                                {
                                    var temp = ListOrder.FirstOrDefault(x => x.OrderId.Equals(ORDERID));
                                    if (temp != null)
                                    {
                                        ListOrder.Remove(temp);
                                        Cookie.Value = null;
                                        Response.Cookies.Add(Cookie);
                                        var strListOrder = JsonConvert.SerializeObject(ListOrder);
                                        Cookie.Value = strListOrder;
                                        Response.Cookies.Add(Cookie);
                                    }
                                }
                            }
                            ORDERID = "";
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, msg);
                        }
                    }
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, Commons.MsgErrorForClient);
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("OurProducts AddToCartBuyingAdditionsStoreBusiness: ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }
        #endregion

        #region //// Extend product in My Store & Business
        public ActionResult ExtendProductStoreBusiness(ProductExtendModel model)
        {
            try
            {
                if (!String.IsNullOrEmpty(model.AssetID))
                {
                    // model.AssetID = ProductID + ',' + AssetID
                    string[] lstID = null;
                    lstID = (model.AssetID).Split(',');
                    if (lstID.Count() == 2)
                    {
                        //===============
                        AddItemToOrderModels modelAddItems = new AddItemToOrderModels();
                        modelAddItems.CusID = CurrentUser.UserId;
                        modelAddItems.OrderID = ORDERID;
                        List<Item> ListItems = new List<Item>();
                        //===============
                        Item product = new Item();
                        product.ProductID = lstID[0];
                        product.Period = model.Period;
                        product.PeriodType = model.PeriodType;
                        product.Price = model.Price;
                        product.Quantity = 1;
                        product.IsExtend = true;
                        product.ListAdditionApply = new List<AdditionApply>();
                        // Set product apply
                        product.ListAdditionApply.Add(new AdditionApply
                        {
                            ID = lstID[1]
                        });

                        ListItems.Add(product);
                        modelAddItems.ListItems = ListItems;
                        //====================
                        string msg = "";
                        bool success = false;
                        OrderDetailModels OrderDetail = _yourcartFactory.AddItems(modelAddItems, ref success, ref msg);
                        if (success)
                        {
                            ORDERID = OrderDetail.ID;
                            OrderDetail.CusId = OrderDetail.CustomerID;
                            if (OrderDetail.ListItems != null && OrderDetail.ListItems.Count > 0)
                                OrderDetail.TotalQuantity = (int)OrderDetail.ListItems.Sum(x => x.Quantity);
                            return Json(OrderDetail, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(msg))
                            {
                                if (msg.Equals("Payment is pending!") || msg.Equals("The receipt has been paid in full!"))
                                {
                                    var Cookie = Request.Cookies["csc-order-v2"];
                                    if (Cookie != null)
                                    {
                                        var Order = Cookie.Value;
                                        var strOrder = Server.UrlDecode(Order);
                                        var ListOrder = JsonConvert.DeserializeObject<List<CookieOrder>>(strOrder);
                                        if (ListOrder != null && ListOrder.Any())
                                        {
                                            var temp = ListOrder.FirstOrDefault(x => x.OrderId.Equals(ORDERID));
                                            if (temp != null)
                                            {
                                                ListOrder.Remove(temp);
                                                Cookie.Value = null;
                                                Response.Cookies.Add(Cookie);
                                                var strListOrder = JsonConvert.SerializeObject(ListOrder);
                                                Cookie.Value = strListOrder;
                                                Response.Cookies.Add(Cookie);
                                            }
                                        }
                                    }
                                    ORDERID = "";
                                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, msg);
                                }
                            }
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, Commons.MsgErrorForClient);
                        }
                    }
                }
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, Commons.MsgErrorForClient);

            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("OurProduct ExtendProductStoreBusiness: ", ex);
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
            }
        }
        #endregion
    }
}