using Newtonsoft.Json;
using NSCSC.Shared;
using NSCSC.Shared.Factory.ClientSite;
using NSCSC.Shared.Factory.ClientSite.OurProducts;
using NSCSC.Shared.Models.ClientSite.MyProfile;
using NSCSC.Shared.Models.ClientSite.OurProDucts;
using NSCSC.Shared.Models.ClientSite.YourCart;
using NSCSC.Shared.Models.OurProducts;
using NSCSC.Shared.Models.OurProducts.Package;
using NSCSC.Shared.Models.OverView;
using NSCSC.Shared.Models.SandBox.Inventory.Product;
using NSCSC.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace NSCSC.Web.Clients.Controllers
{
    public class HomeController : ClientController
    {
        private OurProductFactory _ourFactory = null;
        private CheckIpAddressFactory _checkIpFactory = null;
        public HomeController()
        {
            _ourFactory = new OurProductFactory();
            _checkIpFactory = new CheckIpAddressFactory();
        }
        public ActionResult Index()
        {
            //try
            //{
                return RedirectToAction("Index", "Login");

            //    OverViewModels model = new OverViewModels();
            //    // get list promotion 
            //    var promo = _ourFactory.GetPromotion();
            //    if (promo != null)
            //        model.ListPromotions = promo;
            //    // get ip and check ip

            //    //var CountryCode = _checkIpFactory.GetCountryCode();
            //    if (CountryCode.Equals("SG"))
            //        model.IsIpSingapore = true;

            //    //convert datetime to LocalTime
            //    model.ListPromotions.ForEach(x => { x.ApplyFrom = CommonHelper.ConvertToLocalTime(x.ApplyFrom); x.ApplyTo = CommonHelper.ConvertToLocalTime(x.ApplyTo); });

            //    //get business
            //    var business = ClientStartBusinessSession;
            //    if (business != null && business.Any())
            //    {
            //        model.LeftTitle = business.Where(ww => !string.IsNullOrEmpty(ww.ParentID)).Select(ss => ss.Name).FirstOrDefault();
            //        model.LeftDescription = business.Where(ww => !string.IsNullOrEmpty(ww.ParentID)).Select(ss => ss.Description).FirstOrDefault();

            //        model.BusinessTitle = business.Where(ww => string.IsNullOrEmpty(ww.ParentID)).Select(ss => ss.Name).FirstOrDefault();
            //        model.BusinessDescription = business.Where(ww => string.IsNullOrEmpty(ww.ParentID)).Select(ss => ss.Description).FirstOrDefault();

            //    }

            //    return View(model);
            //}
            //catch (Exception ex)
            //{
            //    NSLog.Logger.Error("Index : ", ex);
            //    return new HttpStatusCodeResult(400, ex.Message);
            //}

        }

        //public ActionResult About()
        //{
        //    ViewBag.Message = "Your application description page.";
        //    return View();
        //}

        //public ActionResult Contact()
        //{
        //    ViewBag.Message = "Your contact page.";
        //    return View();
        //}

        public ActionResult Forgot()
        {
            return View();
        }

        public ActionResult StartYourBusiness()
        {
            var data = ClientPackageSession;
            return View(data);
        }
        public ActionResult StartYourFreeTrial()
        {
            OurProductFactory _factory = new OurProductFactory();
            var models = new List<OurProDuctsModel>();
            try
            {
                models = _factory.GetListData(CurrentUser.UserId, null, (byte)Commons.EProductType.Product, null, null);

                if (models != null && models.Count > 0)
                {
                    models = models.Where(x => x.ListCategory != null && x.ListCategory.Any(y => y.IsFreeTrial))
                                    .Select(m => new OurProDuctsModel
                                    {
                                        Name = m.Name,
                                        ImageURL = m.ImageURL,
                                        ID = m.ID,
                                        CategoryId = m.ListCategory != null && m.ListCategory.Count > 0 ? m.ListCategory.FirstOrDefault().CategoryID : "",
                                        CusId = CurrentUser.UserId
                                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("GetListProductByCategory : ", ex);
            }
            return View(models);
        }

        [HttpGet]
        public ActionResult AddToCartOfProductFreeTrial(string Id, string CateID)
        {
            OurProductFactory _factory = new OurProductFactory();
            YourCartFactory _yourcartFactory = new YourCartFactory();
            try
            {
                // get detail product by product id
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

                        model.PackageDetail.Period = 30;
                        model.PackageDetail.PeriodType = (int)Commons.EPeriodType.Day;
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
                    //List<SelectListItem> ListAdditionType = new List<SelectListItem>();
                    //ListAdditionType = GetListAdditionType();
                    //ViewBag.ListAdditionType = ListAdditionType;

                    model.PackageDetail.CurrencySymbol = CurrencySymbol;
                    model.PackageDetail.Price = 0;
                    model.PackageDetail.DiscountValue = 0;

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

                // add to cart
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
                // Get all composite (products + additions)
                if (model.PackageDetail.ListComposite == null)
                    model.PackageDetail.ListComposite = new List<ProductCompositeModels>();
                if (model.PackageDetail.ListProductOnPackage != null && model.PackageDetail.ListProductOnPackage.Any())
                    model.PackageDetail.ListComposite.AddRange(model.PackageDetail.ListProductOnPackage);

                package.ListComposite = model.PackageDetail.ListComposite;
                //=========
                ListItems.Add(package);
                modelAddItems.ListItems = ListItems;
                modelAddItems.IsFree = true;
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
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, msg);
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("AddToCartOfProduct: ", ex);
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
            }
        }
    }
}