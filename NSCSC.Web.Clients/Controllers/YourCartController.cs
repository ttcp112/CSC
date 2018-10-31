using Newtonsoft.Json;
using NSCSC.Payments.Paypal.DTO;
using NSCSC.Payments.Paypal.Request;
using NSCSC.Shared;
using NSCSC.Shared.Factory;
using NSCSC.Shared.Factory.ClientSite;
using NSCSC.Shared.Factory.ClientSite.OurProducts;
using NSCSC.Shared.Factory.ManagementTool;
using NSCSC.Shared.Factory.Settings.PaymentMethods;
//using static NSCSC.Payments.Paypal.DTO.NSPaymentResponse;
using NSCSC.Shared.Filters;
using NSCSC.Shared.Models.ClientSite;
using NSCSC.Shared.Models.ClientSite.MyProfile;
using NSCSC.Shared.Models.ClientSite.MyStoreAndBusiness;
using NSCSC.Shared.Models.ClientSite.OurProDucts;
using NSCSC.Shared.Models.ClientSite.YourCart;
using NSCSC.Shared.Models.ManagementTool.Feedbacks;
using NSCSC.Shared.Models.OurProducts;
using NSCSC.Shared.Models.SandBox.Inventory.Product;
using NSCSC.Shared.Utilities;
using NSCSC.Web.Clients.App_Start;
using NuWebNCloud.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace NSCSC.Web.Clients.Controllers
{
    //[NuAuth]
    public class YourCartController : ClientController
    {
        private YourCartFactory _factory = null;
        private OurProductFactory _OPfactory = null;
        private PaymentMethodFactory _PMfactory = null;
        private MyProfileFactory _PfFactory = null;
        private FeedbacksFactory _FBFactory = null;
        private MyStoreAndBusinessFactory _MyStoreBusinessfactory = null;
        private BaseFactory _baseFactory = null;
        CheckIpAddressFactory _checkIpFactory = null;
        // GET: YourCart
        public YourCartController()
        {
            _factory = new YourCartFactory();
            _OPfactory = new OurProductFactory();
            _PMfactory = new PaymentMethodFactory();
            _PfFactory = new MyProfileFactory();
            _FBFactory = new FeedbacksFactory();
            _baseFactory = new BaseFactory();
            _MyStoreBusinessfactory = new MyStoreAndBusinessFactory();
        }

        [NuAuth]
        public ActionResult Index()
        {
            return RedirectToAction("cart");

            ViewBag.IsCreateOrder = false;
            var data = _factory.GetOrderDetail(ORDERID, true);
            double TotalDiscount = 0;
            if (data != null)
            {
                if (data.IsPad)
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
                                ViewBag.IsCreateOrder = true;
                            }
                        }
                        //Cookie.Expires = DateTime.Now.AddDays(-1d);
                        // Reset value cookie
                    }
                }
                else
                {
                    if (data.ListItems != null && data.ListItems.Count > 0)
                    {
                        int OffSet = 0;
                        data.ListItems.ForEach(x =>
                        {
                            x.TotalPrice = x.Price + x.ListItem.Sum(ss => ss.Price);
                            x.CurrencySymbol = CurrencySymbol;
                            x.OffSet = OffSet++;
                            //Period

                            if (x.PeriodType != (int)Commons.EPeriodType.OneTime)
                                x.PeriodName = x.Period + " ";
                            x.PeriodName += PeriodName(x.Period, x.PeriodType);
                            //===============
                            if (x.ProductType == (byte)Commons.EProductType.Product)
                            {
                                //Product
                                if (x.ImageURL == null || x.ImageURL.Equals(""))
                                    x.ImageURL = _ImgDummyProduct;
                                // Normal Product
                                if (!x.IsExtend)
                                {
                                    //NumOfDevice -> Unlimited
                                    if (x.AmountOfUnit == -1)
                                        x.sAmountOfUnit = "Product key can be applied on unlimited devices";
                                    else
                                        x.sAmountOfUnit = "Product key can be applied on " + x.AmountOfUnit + " user device(s)";
                                    //NumOfAccount -> Unlimited
                                    if (x.NumOfAccount == -1)
                                        x.sNumOfAccount = "License Key of the product can be used to activate unlimited user account(s)";
                                    else
                                        x.sNumOfAccount = "License Key of the product can be used to activate " + x.NumOfAccount + " user account(s)";
                                    //==Function
                                    if (x.IsFullFunction)
                                        x.sFunction = "All functions included";
                                    else
                                        if (x.ListFunction != null)
                                        x.sFunction = string.Join(", ", x.ListFunction.Select(o => o.Name));
                                }
                            }
                            else if (x.ProductType == (byte)Commons.EProductType.Package)
                            {
                                //Package
                                if (x.ImageURL == null || x.ImageURL.Equals(""))
                                    x.ImageURL = _ImgDummyPackage;
                                //NumOfAccount -> Unlimited
                                if (x.NumOfAccount == -1)
                                    x.sNumOfAccount = "License Key of each product in this package can be used to activate unlimited user account(s)";
                                else
                                    x.sNumOfAccount = "License Key of each product in this package can be used to activate " + x.NumOfAccount + " user account(s)";
                            }
                            else if (x.ProductType == (byte)Commons.EProductType.Addition)
                            {
                                //Addition
                                if (x.ImageURL == null || x.ImageURL.Equals(""))
                                    x.ImageURL = _ImgDummyProduct;
                                //==========
                                if (x.AdditionType == (byte)Commons.EAdditionType.Software
                                || x.AdditionType == (byte)Commons.EAdditionType.Location
                                || x.AdditionType == (byte)Commons.EAdditionType.Account)
                                {
                                    if (x.ListAdditionApply != null && x.ListAdditionApply.Count > 0)
                                    {
                                        var AdditionApply = x.ListAdditionApply[0];
                                        ////Package
                                        //if (!AdditionApply.ProductParentID.Equals(""))
                                        //    x.AppliedOn = "<span class=\"text-success\">" + AdditionApply.Name + "</span>";
                                        //else//Product
                                        //    x.AppliedOn = "<span class=\"text-success\">" + AdditionApply.Name + "</span> <span class=\"fontBold\">in package:</span>: "
                                        //                    + "<span class=\"text-success\">" + AdditionApply.ParentName + "</span>";

                                        if (AdditionApply != null)
                                        {
                                            // Product in Package
                                            if (!string.IsNullOrEmpty(AdditionApply.ProductParentID))
                                                x.AppliedOn = "<span class=\"text-success\">" + AdditionApply.Name + "</span> <span class=\"fontBold\">in package:</span> "
                                                                + "<span class=\"text-success\">" + AdditionApply.ParentName + "</span>";
                                            else//Product
                                                x.AppliedOn = "<span class=\"text-success\">" + AdditionApply.Name + "</span>";
                                        }
                                    }
                                }
                                if (x.AdditionType == (byte)Commons.EAdditionType.Function)
                                {
                                    var ListCategory = _OPfactory.ProductGetCate();
                                    if (ListCategory != null && ListCategory.Count > 0)
                                    {
                                        x.ApplicableOn = string.Join(", ", ListCategory.Select(o => o.Name).ToList());
                                    }
                                }
                                //==Function
                                if (x.IsFullFunction)
                                    x.sFunction = "All functions included";
                                else
                                    if (x.ListFunction != null)
                                    x.sFunction = string.Join(", ", x.ListFunction.Select(o => o.Name));
                            }

                            //==============
                            if (x.ProductType != (byte)Commons.EProductType.Addition)
                            {
                                //update 09-10-2017
                                //x.ListComposite = x.ListItem.Where(o => o.ProductType == (byte)Commons.EItemGroup.Composite).ToList();
                                x.ListProductOfPackage = x.ListComposite.Where(o => o.ProductType == (byte)Commons.EProductType.Product).OrderBy(o => o.Sequence).ThenBy(o => x.ProductName).ToList();
                                if (x.ListProductOfPackage != null && x.ListProductOfPackage.Count > 0)
                                {
                                    int productOffSet = 0;
                                    x.ListProductOfPackage.ForEach(o =>
                                    {
                                        o.OffSet = productOffSet++;
                                        if (o.ImageURL == null || o.ImageURL.Equals(""))
                                            o.ImageURL = _ImgDummyProduct;

                                        if (o.Quantity == -1)
                                            o.sAmountOfUnit = "Product key can be applied on unlimited devices.";
                                        else
                                            o.sAmountOfUnit = "Product key can be applied on " + o.Quantity + " user device(s).";
                                    });
                                }
                                x.ListComposite = x.ListComposite.Where(o => o.ProductType != (byte)Commons.EProductType.Product).OrderBy(o => o.Sequence).ThenBy(o => x.ProductName).ToList();
                                x.ListAdditional = x.ListItem.Where(o => o.ProductType == (byte)Commons.EProductType.Addition).ToList();
                            }
                            //======

                            //Get Infor for Discount
                            if (!string.IsNullOrEmpty(x.DiscountID))
                            {
                                x.DiscountTypeName = (x.DiscountType == (byte)Commons.EValueType.Percent) ? "%" : "$";
                                x.DiscountDisplay = ReplaceDiscountCode(x.Description);

                                TotalDiscount += x.DiscountAmount;
                            }

                            //x.SubTotal = x.TotalPrice * x.Quantity;
                            //x.SubTotal = (x.Price * x.Quantity) + x.ListItem.Sum(ss => ss.Price * ss.Quantity); ;
                            x.SubTotal = ((x.Price) + x.ListItem.Sum(ss => ss.Price * ss.Quantity)) * x.Quantity;
                        });

                        //Get Discount
                        if (data.BillDiscount != null)
                        {
                            data.DiscountOnTotalCart = Math.Abs(data.BillDiscount.DiscountAmount);

                            //data.TotalDiscount = Math.Abs(data.BillDiscount.DiscountAmount);
                            data.TotalDiscount = Math.Abs(data.TotalDiscount);

                            //========
                            data.DiscountTotalBill.ID = data.BillDiscount.ID;
                            data.DiscountTotalBill.DiscountID = data.BillDiscount.DiscountID;
                            data.DiscountTotalBill.DiscountAmount = data.BillDiscount.DiscountAmount;
                            data.DiscountTotalBill.DiscountName = data.BillDiscount.DiscountName;
                            data.DiscountTotalBill.DiscountDisplay = ReplaceDiscountCode(data.BillDiscount.DiscountCode);
                            data.DiscountTotalBill.DiscountType = data.BillDiscount.DiscountType;
                            data.DiscountTotalBill.DiscountValue = data.BillDiscount.DiscountValue;
                            data.DiscountTotalBill.Description = data.BillDiscount.DiscountCode;
                            data.DiscountTotalBill.DiscountTypeName = (data.BillDiscount.DiscountType == (byte)Commons.EValueType.Percent) ? "%" : "$";
                        }

                        //Get Tax
                        if (data.TaxInfo != null)
                        {
                            data.sTaxName = "";
                            if (data.TaxInfo.TaxType == (byte)Commons.ETaxType.AddOn)
                                data.sTaxName = "Add On";
                            else
                                data.sTaxName = "Inclusive";
                            //=======
                            data.sTaxValue = data.TaxInfo.TaxPercent;
                        }
                    }
                }

                //Check order free trial
                Session["IsFree"] = data.IsFree;
            }

            data.CusId = CurrentUser.UserId;
            data.CurrencySymbol = CurrencySymbol;
            data.TotalQuantity = (data.ListItems == null ? 0 : (int)data.ListItems.Sum(x => x.Quantity));
            data.TotalPromotion = Math.Abs(data.TotalPromotion);
            data.ListItems = data.ListItems.OrderBy(oo => oo.ProductName).ToList();
            return View(data);
        }

        public ActionResult CouponCode(string DiscountCode, string ProductID, string OrderDetailID)
        {
            string msg = "";
            bool success = false;
            string OrderId = "";
            if (string.IsNullOrEmpty(ProductID))
                OrderId = ORDERID;
            DiscountModels discount = _factory.CouponCode(DiscountCode, OrderId, ProductID, OrderDetailID, ref success, ref msg);
            if (success)
                return Json(discount, JsonRequestBehavior.AllowGet);
            else
                return new HttpStatusCodeResult(400, msg
                    /*msg.Equals("The discount amount is larger than total amount of your cart. Please try again.") ?
                    msg : "The code is invalid. Please try again."*/);
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult UpdateYourCart(OrderDetailModels model)
        {
            //====================
            string msg = "";
            bool success = false;
            AddItemToOrderModels modelAddItems = new AddItemToOrderModels();
            modelAddItems.CusID = CurrentUser.UserId;
            modelAddItems.OrderID = ORDERID;
            modelAddItems.Note = model.Description;

            if (model.ListItems != null && model.ListItems.Count > 0)
            {
                model.ListItems.ForEach(x =>
                {
                    if (!x.IsDelete)
                        x.IsDelete = (x.Quantity == 0 ? true : false);
                    //========
                    //Update by trongntn  24-11-2017
                    if (x.IsDelete) //Reset List BuyingAddition for Product TypeProduct [Product]
                    {
                        if (Session["_ListBuyingAdditionItem"] != null)
                        {
                            var listTemp = (List<BuyingAdditionModels>)Session["_ListBuyingAdditionItem"];
                            var isExist = listTemp.Exists(z => x.ID.Equals(x.ProductID));
                            if (isExist)
                                listTemp.RemoveAll(z => z.ID.Equals(x.ProductID));
                            Session.Add("_ListBuyingAdditionItem", listTemp);
                        }
                    }
                });
            }
            modelAddItems.ListItems = model.ListItems;
            if (!string.IsNullOrEmpty(model.DiscountTotalBill.Description))
            {
                model.DiscountTotalBill.Quantity = 1;
                modelAddItems.ListItems.Add(model.DiscountTotalBill);
            }
            OrderDetailModels OrderDetail = _factory.AddItems(modelAddItems, ref success, ref msg);
            if (success)
            {
                ORDERID = OrderDetail.ID;
                if (OrderDetail.ListItems != null && OrderDetail.ListItems.Count > 0)
                    OrderDetail.TotalQuantity = (int)OrderDetail.ListItems.Sum(x => x.Quantity);
                else
                    Session["IsFree"] = false;// reset IsFree
                return Json(OrderDetail, JsonRequestBehavior.AllowGet);
            }
            else
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, Commons.MsgErrorForClient);
        }

        [NuAuth]
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult GoToCheckOut(OrderDetailModels model)
        {
            //====================
            string msg = "";
            bool success = false;
            AddItemToOrderModels modelAddItems = new AddItemToOrderModels();
            modelAddItems.CusID = CurrentUser.UserId;
            modelAddItems.OrderID = ORDERID;
            modelAddItems.Note = model.Description;
            modelAddItems.IsFree = model.IsFree;
            if (model.ListItems != null && model.ListItems.Count > 0)
            {
                model.ListItems.ForEach(x =>
                {
                    if (!x.IsDelete)
                        x.IsDelete = (x.Quantity == 0 ? true : false);
                    //========
                    //Update by trongntn  24-11-2017
                    if (x.IsDelete) //Reset List BuyingAddition for Product TypeProduct [Product]
                    {
                        if (Session["_ListBuyingAdditionItem"] != null)
                        {
                            var listTemp = (List<BuyingAdditionModels>)Session["_ListBuyingAdditionItem"];
                            var isExist = listTemp.Exists(z => z.ID.Equals(x.ProductID));
                            if (isExist)
                                listTemp.RemoveAll(z => z.ID.Equals(x.ProductID));
                            Session.Add("_ListBuyingAdditionItem", listTemp);
                        }
                    }
                });
            }
            modelAddItems.ListItems = model.ListItems;
            if (!string.IsNullOrEmpty(model.DiscountTotalBill.Description))
                modelAddItems.ListItems.Add(model.DiscountTotalBill);

            OrderDetailModels OrderDetail = _factory.AddItems(modelAddItems, ref success, ref msg);
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
                    if (msg.Equals("Payment is pending!") || msg.Equals("The receipt has been paid in full!") || msg.Equals("You have already used POZZ_Kiosk trial version. You need to buy our product/ package to continue your business."))
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

        /*Check Out*/
        [NuAuth]
        public ActionResult CheckOut(string StepType = null, string ItemID = null, string AssetID = null)
        {
            string payerId = Request.Params["PayerID"];
            ViewBag.TransactionId = "";
            ViewBag.Note = "";
            ViewBag.State = "";
            ViewBag.paymentIds = "";
            ViewBag.Message = "";
            if (!string.IsNullOrEmpty(payerId))
            {
                if (payerId.Equals(Security.KeyCyberSouce))
                {
                    var Status = Request.Params["decision"];
                    var Transaction_Id = Request.Params["transaction_id"];
                    ViewBag.TransactionId = string.IsNullOrEmpty(Transaction_Id) ? payerId : Transaction_Id;
                    if (Status.ToUpper().Equals("ACCEPT"))
                        ViewBag.State = "approved";
                    else if (Status.ToUpper().Equals("CANCEL"))
                    {
                        ViewBag.State = Status;
                        ViewBag.Message = "Your order was cancel.";
                        var message = Request.Params["message"];
                    }
                    else if (Status.ToUpper().Equals("DECLINE"))
                    {
                        ViewBag.State = Status;
                        var message = Request.Params["message"];
                        ViewBag.Message = "Your order was declined.";
                    }
                    else
                    {
                        ViewBag.State = Status;
                        ViewBag.Message = "Your order was error.";
                    }
                    ViewBag.PaymentMethodID = Session["PaymentMethodID"] as string;
                }
                else if (payerId.Equals(Security.KeyeNETS))
                {
                    var paramsKey = Request.Params;
                    var mes = Request.Params["message"];
                    var mesDe = HttpUtility.UrlDecode(mes);
                    var msgJson = JsonConvert.DeserializeObject<ResponseeNETS>(mesDe);
                    ViewBag.TransactionId = string.IsNullOrEmpty(msgJson.msg.merchantTxnRef) ? payerId : msgJson.msg.merchantTxnRef;
                    if (msgJson.msg.netsTxnStatus.Equals("0"))
                        ViewBag.State = "approved";
                    else
                    {
                        ViewBag.State = "error";
                        ViewBag.Message = msgJson.msg.netsTxnMsg;
                    }
                    ViewBag.PaymentMethodID = Session["PaymentMethodID"] as string;
                }
                else
                {
                    var paymentId = Session["guid"] as string;

                    var request = new NSPaymentExecuteRequest
                    {
                        PayerId = payerId,
                        PaymentId = paymentId
                    };
                    // Execute the payment and get result
                    var nsExecution = NSPayment.Execute(request);
                    if (!string.IsNullOrEmpty(nsExecution.State) && !nsExecution.State.Equals("approved"))
                    {
                        ViewBag.Message = "We encountered an issue on your payment. Please try again.";
                    }
                    ViewBag.TransactionId = nsExecution.TransactionID;
                    ViewBag.Note = nsExecution.Note;
                    ViewBag.State = nsExecution.State;
                    ViewBag.PaymentMethodID = Session["PaymentMethodID"] as string;
                }
            }

            var dataProduct = _factory.GetOrderDetail(ORDERID);
            if (dataProduct == null || (dataProduct.ListItems == null || dataProduct.ListItems.Count == 0))
                return RedirectToAction("Index");

            ViewBag.StepType = StepType;
            ViewBag.ItemID = ItemID;
            ViewBag.AssetID = AssetID;
            ViewBag.CusID = CurrentUser.UserId;
            ViewBag.OrderID = ORDERID;
            string OrderID = ORDERID;
            bool isCheckPopupCheckout = true;
            CheckOutViewModels model = new CheckOutViewModels();
            model.CurrencySymbol = CurrencySymbol;
            //Get List Product
            if (dataProduct != null)
            {
                if (dataProduct.ListItems != null && dataProduct.ListItems.Count > 0)
                {
                    int OffSet = 0;
                    // Get list normal item, not extend item
                    var lstItems = dataProduct.ListItems.Where(ww => !ww.IsExtend).ToList();

                    lstItems.ForEach(x =>
                    {
                        x.OffSet = OffSet++;
                        //===Store setup completed
                        int StoreCount = x.ListStoreApply == null ? 0 : x.ListStoreApply.Count;
                        if (x.ListItem != null && x.ListItem.Count > 0)
                        {
                            x.ListItem.ForEach(z =>
                            {
                                if (z.AdditionType == (int)Commons.EAdditionType.Location)
                                {
                                    if (z.AmountOfUnit == -1 || x.NumOfStore == -1)
                                        x.NumOfStore = -1;
                                    else
                                        x.NumOfStore += Convert.ToInt16((z.Quantity * z.AmountOfUnit));
                                }
                            });
                        }
                        if (StoreCount == 0 && x.NumOfStore == -1)
                        {
                            if (x.ProductType == (int)Commons.EProductType.Addition && x.AdditionType == (int)Commons.EAdditionType.Location)
                                isCheckPopupCheckout = false;
                        }
                        else
                        {
                            if (x.NumOfStore != -1 && x.NumOfStore != StoreCount)
                            {
                                if (x.ProductType == (int)Commons.EProductType.Addition && x.AdditionType != (int)Commons.EAdditionType.Location)
                                {
                                    // isCheckPopupCheckout = true;
                                }
                                else
                                {
                                    isCheckPopupCheckout = false;
                                }

                            }
                        }
                        if (x.NumOfStore == -1)
                        {
                            if (StoreCount > 0)
                            {
                                x.sStoreCompleted = "Already set up " + StoreCount + " store" + string.Format("{0}", StoreCount > 1 ? "s" : "");
                            }
                            else
                            {
                                x.sStoreCompleted = "Not yet set up any store";
                            }
                        }
                        else if (x.NumOfStore == StoreCount)
                            x.sStoreCompleted = "Store set up completed";
                        else
                        {
                            if (StoreCount > 0)
                            {
                                x.sStoreCompleted = "Already set up " + StoreCount + " in " + x.NumOfStore + " stores";
                            }
                            else
                            {
                                x.sStoreCompleted = "Not yet set up any store.";
                            }
                        }


                        //Period
                        if (x.PeriodType != (int)Commons.EPeriodType.OneTime)
                            x.PeriodName = x.Period + " " + PeriodName(x.Period, x.PeriodType);
                        else
                            x.PeriodName = PeriodName(x.Period, x.PeriodType);
                        //===============
                        if (x.ProductType == (byte)Commons.EProductType.Product)
                        {
                            //Product
                            if (x.ImageURL == null || x.ImageURL.Equals(""))
                                x.ImageURL = _ImgDummyProduct;

                            //NumOfDevice -> Unlimited
                            if (x.AmountOfUnit == -1 || x.NumOfStore == -1)
                                x.sAmountOfUnit = "You have unlimited store(s) to set up in this product.";
                            else
                                x.sAmountOfUnit = "You have " + x.NumOfStore + " store(s) to set up in this product.";
                        }
                        else if (x.ProductType == (byte)Commons.EProductType.Package)
                        {
                            //Package
                            if (x.ImageURL == null || x.ImageURL.Equals(""))
                                x.ImageURL = _ImgDummyPackage;

                            if (x.AmountOfUnit == -1 || x.NumOfStore == -1)
                                x.sAmountOfUnit = "You have unlimited store(s) to set up in this package.";
                            else
                                x.sAmountOfUnit = "You have " + x.NumOfStore + " store(s) to set up in this package.";
                        }
                        else if (x.ProductType == (byte)Commons.EProductType.Addition)
                        {
                            //Addition
                            if (x.ImageURL == null || x.ImageURL.Equals(""))
                                x.ImageURL = _ImgDummyProduct;

                            if (x.AdditionType == (int)Commons.EAdditionType.Location)
                            {
                                if (x.AmountOfUnit == -1 || x.NumOfStore == -1)
                                    x.sAmountOfUnit = "You have unlimited store(s) to set up in this addition location.";
                                else
                                    x.sAmountOfUnit = "You have " + x.NumOfStore + " store(s) to set up in this addition location.";
                            }
                        }
                        //============
                        if (x.ListStoreApply != null && x.ListStoreApply.Count > 0)
                        {
                            x.ListStoreApply.ForEach(o =>
                            {
                                if (o.IsExtend)
                                    o.sStatus = "Extend";
                                else
                                    o.sStatus = "New";
                                //=========
                                if (x.ProductType == (byte)Commons.EProductType.Product || x.ProductType == (byte)Commons.EProductType.Addition)
                                    o.sTitle = o.StoreName;
                                else if (x.ProductType == (byte)Commons.EProductType.Package)
                                    o.sTitle = o.StoreName;
                                //o.sTitle = o.StoreName + " - " + x.ProductName;
                            });
                        }
                    });
                }
            }
            ViewBag.isCheckPopupCheckout = isCheckPopupCheckout;
            //==============
            CheckOutView CheckOutInfo = new CheckOutView();
            CheckOutInfo.OrderID = OrderID;
            ////Customer Information
            var CustomerMerchantInfo = _PfFactory.GetInfoLite(CurrentUser.UserId);
            CheckOutInfo.ImageURL = CustomerMerchantInfo.CustomerDetail.ImageURL;
            if (CheckOutInfo.ImageURL == null || CheckOutInfo.ImageURL.Equals(""))
                CheckOutInfo.ImageURL = _ImgDummyCustomer;

            CheckOutInfo.FullName = CustomerMerchantInfo.CustomerDetail.Name;
            CheckOutInfo.Email = CustomerMerchantInfo.CustomerDetail.Email;
            CheckOutInfo.Merchant = CustomerMerchantInfo.MerchantDetail.Name;
            CheckOutInfo.Address = (!string.IsNullOrEmpty(CustomerMerchantInfo.MerchantDetail.Street) ? CustomerMerchantInfo.MerchantDetail.Street + ", " : "")
                                        + (!string.IsNullOrEmpty(CustomerMerchantInfo.MerchantDetail.City) ? CustomerMerchantInfo.MerchantDetail.City + ", " : "")
                                        + (!string.IsNullOrEmpty(CustomerMerchantInfo.MerchantDetail.Country) ? CustomerMerchantInfo.MerchantDetail.Country + ", " : "");
            if (!string.IsNullOrEmpty(CheckOutInfo.Address) && CheckOutInfo.Address.Length >= 2)
            {
                CheckOutInfo.Address = CheckOutInfo.Address.Remove(CheckOutInfo.Address.Length - 2, 1);
            }
            //Order Summary
            var data = dataProduct;// _factory.GetOrderDetail(OrderID);
            if (data != null)
            {
                CheckOutInfo.NumberOfItems = (int)data.ListItems.Sum(x => x.Quantity);
                var CountPackage = data.ListItems.Where(x => x.ProductType == (byte)Commons.EProductType.Package).Sum(x => x.Quantity);
                var CountProduct = data.ListItems.Where(x => x.ProductType == (byte)Commons.EProductType.Product).Sum(x => x.Quantity);
                var CountAddition = data.ListItems.Where(x => x.ProductType == (byte)Commons.EProductType.Addition).Sum(x => x.Quantity);
                CheckOutInfo.sNumberOfItems = ((CountPackage > 0) ? CountPackage + " x Package" + (CountPackage > 1 ? "s" : "") + ", " : "")
                                                + ((CountProduct > 0) ? CountProduct + " x Product" + (CountProduct > 1 ? "s" : "") + ", " : "")
                                                + ((CountAddition > 0) ? CountAddition + " x Addition" + (CountAddition > 1 ? "s" : "") + ", " : "");
                if (!string.IsNullOrEmpty(CheckOutInfo.sNumberOfItems) && CheckOutInfo.sNumberOfItems.Length >= 2)
                    CheckOutInfo.sNumberOfItems = CheckOutInfo.sNumberOfItems.Remove(CheckOutInfo.sNumberOfItems.Length - 2, 2);
                CheckOutInfo.SubTotal = data.SubTotal;

                //if (data.BillDiscount != null)
                //    data.TotalDiscount = data.BillDiscount.DiscountValue;
                if (data.TaxInfo != null)
                {
                    data.sTaxName = "";
                    if (data.TaxInfo.TaxType == (byte)Commons.ETaxType.AddOn)
                        data.sTaxName = "Add On";
                    else
                        data.sTaxName = "Inclusive";
                    //=======
                    data.sTaxValue = data.TaxInfo.TaxPercent;
                }
                if (dataProduct != null)
                    CheckOutInfo.Discount = Math.Abs(dataProduct.TotalDiscount);
                CheckOutInfo.TaxName = data.sTaxName;
                CheckOutInfo.TaxValue = data.sTaxValue;
                CheckOutInfo.Tax = data.Tax;

                CheckOutInfo.TotalOrder = data.Total;
                CheckOutInfo.Note = data.Description;
                CheckOutInfo.TotalPromotion = Math.Abs(data.TotalPromotion);
            }

            //List Payment
            var ListPayment = _PMfactory.GetData();
            if (ListPayment != null)
            {
                ListPayment = ListPayment.Where(x => x.Code == (byte)Commons.EPaymentCode.Parent && x.IsActive).ToList();
                if (ListPayment != null && ListPayment.Count > 0)
                {
                    ListPayment.ForEach(x =>
                    {
                        if (x.ImageURL == null || x.ImageURL.Equals(""))
                            x.ImageURL = _ImgDummyPackage;
                        x.ListChild = x.ListChild.Where(z => z.Code == (byte)Commons.EPaymentCode.External && z.IsActive).ToList();
                        if (x.ListChild != null && x.ListChild.Count > 0)
                        {
                            x.ListChild.ForEach(z =>
                            {
                                if (z.ImageURL == null || z.ImageURL.Equals(""))
                                    z.ImageURL = _ImgDummyPackage;

                                if (z.Name.ToLower().Equals(Commons.EPayment.PayPal.ToString().ToLower()))
                                {
                                    z.Payment = (int)Commons.EPayment.PayPal;
                                }
                                else if (z.Name.ToLower().Equals(Commons.EPayment.SecureAcceptance.ToString().ToLower()))
                                {
                                    z.Payment = (int)Commons.EPayment.SecureAcceptance;
                                }
                                else if (z.Name.ToLower().Equals(Commons.EPayment.eNETS.ToString().ToLower()))
                                {
                                    z.Payment = (int)Commons.EPayment.eNETS;
                                }
                            });

                        }
                    });
                }
            }
            CheckOutInfo.ListPayment = ListPayment;

            model.CheckOut = CheckOutInfo;
            model.InputStore.CusId = CurrentUser.UserId;
            model.ListProduct = dataProduct.ListItems.Where(ww => !ww.IsExtend).OrderBy(oo => oo.ProductName).ToList(); // Not extend items

            return View(model);
        }

        public ActionResult OrderPay(string OrderID, string PaymentMethodID, double AmountPay, string TransactionId, string Note)
        {
            string OrderIDReturn = "";
            string msg = "";
            bool result = _factory.OrderPay(OrderID, PaymentMethodID, AmountPay, CurrentUser.UserId, TransactionId, Note, ref OrderIDReturn, ref msg);
            if (result)
            {
                //Update by trongntn 24-11-2017
                //Reset List Buying Addition Item
                if (Session["_ListBuyingAdditionItem"] != null)
                    Session["_ListBuyingAdditionItem"] = null;
                //===========
                ORDERID = "";
                string str = "{'ID':'" + OrderIDReturn + "'}";
                JavaScriptSerializer jsSer = new JavaScriptSerializer();
                object obj = jsSer.Deserialize(str, typeof(object));
                return Json(obj, JsonRequestBehavior.AllowGet);
            }
            else
            {
                if (!string.IsNullOrEmpty(msg) && msg.Equals("The receipt has been paid in full!"))
                {
                    ORDERID = "";
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, msg);
                }
                return new HttpStatusCodeResult(HttpStatusCode.NotFound, msg);
            }
        }

        public ActionResult OrderPayWithCybersource(string PaymentMethodID)
        {
            Session.Add("PaymentMethodID", PaymentMethodID);
            return new HttpStatusCodeResult(HttpStatusCode.OK, "");
        }

        public string RadomCode()
        {
            Random R = new Random();
            return ((long)R.Next(0, 100000) * (long)R.Next(0, 100000)).ToString().PadLeft(10, '0');
        }

        public JsonResult OrderPayWitheNets(string OrderID, string PaymentMethodID, int Payment)
        {
            var data = _factory.GetOrderDetail(ORDERID, true);
            var obj = new object[2];
            if (data != null)
            {
                var secretKey = Commons.SecretKey;
                //var _txnEndUrl = WebConfigurationManager.AppSettings["URLENetsCSC"];
                var _txnEndUrl = Commons.URLENetsCSC;
                var objJson = new
                {
                    ss = Commons.SS,
                    msg = new
                    {
                        netsMid = Commons.NetsMid,
                        tid = "",
                        submissionMode = Commons.SubmissionMode,
                        //Note txnAmount : Transaction amount in cents for all currencies. E.g. for $10.00 in SGD, the txnAmount will be equal to 1000. For¥10,000 in JPY,
                        //the txnAmount will be equal to 1000000.
                        txnAmount = Convert.ToInt32(data.Total * 100),
                        merchantTxnRef = RadomCode(),
                        merchantTxnDtm = DateTime.Now.ToString("yyyyMMdd HH:mm:ss.fff"),
                        paymentType = Commons.PaymentType,
                        currencyCode = CurrencySymbol,
                        paymentMode = "",
                        merchantTimeZone = Commons.MerchantTimeZone,
                        b2sTxnEndURL = _txnEndUrl,
                        b2sTxnEndURLParam = "",
                        s2sTxnEndURL = _txnEndUrl,
                        s2sTxnEndURLParam = "",
                        clientType = Commons.ClientType,
                        supMsg = "",
                        netsMidIndicator = Commons.NetsMidIndicator,
                        ipAddress = Commons.IpAddress,
                        language = Commons.Language
                    }
                };
                var txnReq = JsonConvert.SerializeObject(objJson);
                var input = txnReq + secretKey;
                byte[] datautf8 = Encoding.UTF8.GetBytes(input);
                using (HashAlgorithm sha = new SHA256Managed())
                {
                    byte[] encryptedBytes = sha.TransformFinalBlock(datautf8, 0, datautf8.Length);
                    var s = Convert.ToBase64String(sha.Hash);
                    obj[0] = txnReq;
                    obj[1] = s;
                }
                Session.Add("PaymentMethodID", PaymentMethodID);
            }
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ENets()
        {
            string payerId = Request.Params["PayerID"];
            ENetsInfo model = new ENetsInfo();
            if (!string.IsNullOrEmpty(payerId) && payerId.Equals(Security.KeyeNETS))
            {
                var paramsKey = Request.Params;
                var mes = Request.Params["message"];
                var mesDe = HttpUtility.UrlDecode(mes);
                var msgJson = JsonConvert.DeserializeObject<ResponseeNETS>(mesDe);
                ViewBag.TransactionId = string.IsNullOrEmpty(msgJson.msg.merchantTxnRef) ? payerId : msgJson.msg.merchantTxnRef;
                if (msgJson.msg.netsTxnStatus.Equals("0"))
                    ViewBag.State = "1";
                else
                {
                    ViewBag.State = "0";
                    ViewBag.Message = msgJson.msg.netsTxnMsg;
                }
            }
            else
            {
                var _txtamount = Request["amount"] ?? string.Empty;
                var _txtCurrency = Request["currency"] ?? string.Empty;
                //var _txnEndUrl = WebConfigurationManager.AppSettings["URLENetsApp"];
                var _txnEndUrl = Commons.URLENetsApp;
                Int64 _amount = 0;
                if (!string.IsNullOrEmpty(_txtamount))
                {
                    _amount = Convert.ToInt64(_txtamount);
                }
                var secretKey = Commons.SecretKey;

                var objJson = new
                {
                    ss = Commons.SS,
                    msg = new
                    {
                        netsMid = Commons.NetsMid,
                        tid = "",
                        submissionMode = Commons.SubmissionMode,
                        //Note txnAmount : Transaction amount in cents for all currencies. E.g. for $10.00 in SGD, the txnAmount will be equal to 1000. For¥10,000 in JPY,
                        //the txnAmount will be equal to 1000000.
                        txnAmount = _amount,
                        merchantTxnRef = RadomCode(),
                        merchantTxnDtm = DateTime.Now.ToString("yyyyMMdd HH:mm:ss.fff"),
                        paymentType = Commons.PaymentType,
                        currencyCode = _txtCurrency,
                        paymentMode = "",
                        merchantTimeZone = Commons.MerchantTimeZone,
                        b2sTxnEndURL = _txnEndUrl,
                        b2sTxnEndURLParam = "",
                        s2sTxnEndURL = _txnEndUrl,
                        s2sTxnEndURLParam = "",
                        clientType = Commons.ClientType,
                        supMsg = "",
                        netsMidIndicator = Commons.NetsMidIndicator,
                        ipAddress = Commons.IpAddress,
                        language = Commons.Language
                    }
                };
                var txnReq = JsonConvert.SerializeObject(objJson);
                var input = txnReq + secretKey;
                byte[] datautf8 = Encoding.UTF8.GetBytes(input);
                using (HashAlgorithm sha = new SHA256Managed())
                {
                    byte[] encryptedBytes = sha.TransformFinalBlock(datautf8, 0, datautf8.Length);
                    var s = Convert.ToBase64String(sha.Hash);
                    model.txnReq = txnReq;
                    model.hMac = s;
                    model.currency = _txtCurrency;
                    model.amount = _amount;
                }
            }
            return View(model);
        }

        public ActionResult OrderPayPal(string OrderID, string PaymentMethodID, int Payment)
        {
            var data = _factory.GetOrderDetail(ORDERID, true);
            if (data != null)
            {
                data.TotalQuantity = (data.ListItems == null ? 0 : (int)data.ListItems.Sum(x => x.Quantity));
                data.TotalPromotion = Math.Abs(data.TotalPromotion);
                data.TotalDiscount = Math.Abs(data.TotalDiscount);
                List<NSItem> items = new List<NSItem>();
                if (data.ListItems != null && data.ListItems.Count > 0)
                {
                    data.ListItems.ForEach(x =>
                    {
                        items.Add(new NSItem
                        {
                            Name = x.ProductName,
                            Price = x.Price,
                            Quantity = x.Quantity,
                        });
                        if (x.ListItem != null && x.ListItem.Count > 0)
                        {
                            x.ListItem.ForEach(z =>
                            {
                                items.Add(new NSItem
                                {
                                    Name = z.ProductName,
                                    Price = z.Price,
                                    Quantity = z.Quantity,
                                });
                            });
                        }
                    });

                    items.Add(new NSItem
                    {
                        Name = "Discount",
                        Price = -(data.TotalDiscount),
                        Quantity = 1,
                    });

                    items.Add(new NSItem
                    {
                        Name = "Promotion",
                        Price = -(data.TotalPromotion),
                        Quantity = 1,
                    });
                }
                if (Payment == (int)Commons.EPayment.PayPal)
                {
                    string payerId = Request.Params["PayerID"];
                    if (string.IsNullOrEmpty(payerId))
                    {
                        // Step 1: If there is no Payer -> It's a new payment -> Create a payment

                        // Pass in products need to pay
                        var request = new NSPaymentCreateRequest.Builder()
                        .SetBaseURI(Request.Url.Scheme + "://" + Request.Url.Authority + "/YourCart/CheckOut?")
                        .SetCurrency(CurrencySymbol)     // no need to set if currency is SGD
                        .SetShipping(0)        // no need to set if shipping is 0
                        .SetTax((data.TaxInfo != null && data.TaxInfo.TaxType == (int)Commons.ETaxType.Inclusive) ? 0 : data.Tax)             // no need to set if tax is 0
                        .AddListItem(items).Build();

                        NSLog.Logger.Info("Payment PayPal Request ", request);
                        // Get a PayPal payment url to redirect
                        var nsCreation = NSPayment.Create(request);
                        Session.Add("guid", nsCreation.PaymentId);
                        Session.Add("PaymentMethodID", PaymentMethodID);
                        string str = "{'url':'" + nsCreation.Url + "'}";
                        JavaScriptSerializer jsSer = new JavaScriptSerializer();
                        object obj = jsSer.Deserialize(str, typeof(object));
                        return Json(obj, JsonRequestBehavior.AllowGet);

                    }
                    else
                    {
                        // Step 2: If the user already approved payment in PayPal page
                        var paymentId = Session["guid"] as string;
                        var request = new NSPaymentExecuteRequest
                        {
                            PayerId = payerId,
                            PaymentId = paymentId
                        };
                        // Execute the payment and get result
                        var nsExecution = NSPayment.Execute(request);
                        string str = "{'url':'','State':'" + nsExecution.State + "'}";
                        JavaScriptSerializer jsSer = new JavaScriptSerializer();
                        object obj = jsSer.Deserialize(str, typeof(object));
                        return Json(obj, JsonRequestBehavior.AllowGet);
                        //  return Json(new { url = "", status = "200", State= nsExecution.State, Note= nsExecution.Note, TransactionID = nsExecution.TransactionID });
                    }
                }

            }
            return new HttpStatusCodeResult(HttpStatusCode.NotFound, "");
        }

        [HttpPost]
        public ActionResult Feedback(CheckOutViewModels model)
        {
            try
            {
                model.Feedback.CreatedUser = CurrentUser.UserId;
                model.Feedback.CustomerID = CurrentUser.UserId;
                model.Feedback.CustomerName = CurrentUser.UserName;
                model.Feedback.CustomerEmail = CurrentUser.Email;
                model.Feedback.Subject = "Feedback of Client";
                model.Feedback.Time = DateTime.Now;
                model.Feedback.FeedbackType = (byte)Commons.EFeedbackType.Purchase;

                string msg = "";
                var resule = _FBFactory.CreateOrEdit(model.Feedback, ref msg);
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("Feedback: ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }

        [HttpPost]
        public JsonResult FeedbackAngular(FeedbackDTO model)
        {
            
            var resule = false;
            string msg = "";
            try
            {
                model.Subject = "Feedback of Client";
                model.Time = DateTime.Now;
                model.FeedbackType = (byte)Commons.EFeedbackType.Purchase;
                resule = _FBFactory.CreateOrEdit(model, ref msg);
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("Feedback: ", ex);
            }
            var obj = new
            {
                Result = resule,
                Msg = msg
            };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        #region /*Input Store Info*/
        [EncryptedActionParameter]
        [NuAuth]
        public ActionResult InputStoreInfo(string id)
        {
            var data = _factory.GetOrderDetail(ORDERID);
            var product = data.ListItems.Where(x => x.ID.Equals(id)).FirstOrDefault();

            InputStoreViewModels model = new InputStoreViewModels();
            model.ID = id;
            model.CusId = CurrentUser.UserId;

            if (product != null)
            {
                int _Industry = 0;
                model.OrderProductDetailID = product.ID;
                model.ProductType = product.ProductType;

                // model.NumOfStore = product.NumOfStore - (product.ListStoreApply == null ? 0 : product.ListStoreApply.Count);
                model.NumOfStore = product.NumOfStore == -1 ? product.NumOfStore : product.NumOfStore - (product.ListStoreApply == null ? 0 : product.ListStoreApply.Count);
                if (product.ListItem != null && product.ListItem.Count > 0)
                {
                    product.ListItem.ForEach(x =>
                    {
                        if (x.AdditionType == (int)Commons.EAdditionType.Location)
                        {
                            if (model.NumOfStore == -1 || x.AmountOfUnit == -1)
                                model.NumOfStore = -1;
                            else
                                model.NumOfStore += Convert.ToInt16(x.Quantity * x.AmountOfUnit);
                        }
                    });
                }
                var ListCounTry = _baseFactory.GetListCountry();
                if (product.NumOfStore == -1)
                    model.sNumOfStore = "Unlimited";
                else
                    model.sNumOfStore = model.NumOfStore.ToString(); //product.NumOfStore.ToString();


                List<StoreModels> GetListStore = _MyStoreBusinessfactory.GetListStore(CurrentUser.UserId);
                GetListStore = GetListStore.Where(x => !x.IsTemp).ToList();
                List<SelectListItem> ListCountryForStore = new List<SelectListItem>();
                if (product.ProductType == (int)Commons.EProductType.Product)
                {
                    if (product.Type == (int)Commons.EType.NuDisplay || product.Type == (int)Commons.EType.NuKiosk || product.Type == (int)Commons.EType.NuPOS)
                    {
                        GetListStore = GetListStore.Where(x => x.StoreType == (int)Commons.EStoreType.FnB).ToList();
                        _Industry = (int)Commons.EStoreType.FnB;
                    }
                    else if (product.Type == (int)Commons.EType.POZZ || product.Type == (int)Commons.EType.POZZ_Display || product.Type == (int)Commons.EType.POZZ_Kiosk)
                    {
                        GetListStore = GetListStore.Where(x => x.StoreType == (int)Commons.EStoreType.Beauty).ToList();
                        _Industry = (int)Commons.EStoreType.Beauty;
                    }
                    model.Type = product.Type;
                }
                else if (product.ProductType == (int)Commons.EProductType.Addition)
                {
                    if (product.ListAdditionApply != null && product.ListAdditionApply.Count > 0)
                    {
                        var item = product.ListAdditionApply.FirstOrDefault();
                        if (item.Type == (int)Commons.EType.NuDisplay || item.Type == (int)Commons.EType.NuKiosk || item.Type == (int)Commons.EType.NuPOS)
                        {
                            GetListStore = GetListStore.Where(x => x.StoreType == (int)Commons.EStoreType.FnB).ToList();
                            _Industry = (int)Commons.EStoreType.FnB;
                        }
                        else if (item.Type == (int)Commons.EType.POZZ || item.Type == (int)Commons.EType.POZZ_Display || item.Type == (int)Commons.EType.POZZ_Kiosk)
                        {
                            GetListStore = GetListStore.Where(x => x.StoreType == (int)Commons.EStoreType.Beauty).ToList();
                            _Industry = (int)Commons.EStoreType.Beauty;
                        }
                    }
                    model.Type = product.Type;
                }
                else if (product.ProductType == (int)Commons.EProductType.Package)
                {
                    GetListStore = GetListStore.OrderBy(x => x.StoreType).ToList();
                    product.ListComposite = product.ListComposite.Where(x => x.ProductType == (int)Commons.EProductType.Product).ToList();
                    if (product.ListComposite != null && product.ListComposite.Count > 0)
                    {
                        foreach (var item in product.ListComposite)
                        {
                            //ListApplyProductClient.Add(new SelectListItem
                            //{
                            //    Value = item.ProductID,
                            //    Text = item.ProductName
                            //});
                            model.ListTypePackage.Add(new SelectListItem
                            {
                                Value = item.ProductID,
                                Text = item.Type.ToString()
                            });
                        }
                        model.Type = product.ListComposite.FirstOrDefault().Type;
                    }
                }
                model.ListStoreSelect = GetListStore;
                if (model.ListStoreSelect != null && model.ListStoreSelect.Count > 0)
                {
                    model.ListStoreSelect.ForEach(x =>
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

                //===============
                if (product.ListStoreApply != null && product.ListStoreApply.Count > 0)
                {
                    List<SelectListItem> ListApplyProductClient = new List<SelectListItem>();

                    if (product.ProductType == (byte)Commons.EProductType.Package)
                    {
                        product.ListComposite = product.ListComposite.Where(x => x.ProductType == (byte)Commons.EProductType.Product).ToList();
                        if (product.ListComposite != null && product.ListComposite.Count > 0)
                        {
                            foreach (var item in product.ListComposite)
                                ListApplyProductClient.Add(new SelectListItem
                                {
                                    Value = item.ProductID,
                                    Text = item.ProductName
                                });
                        }
                    }
                    else if (product.ProductType == (byte)Commons.EProductType.Addition)
                    {
                        foreach (var itemad in product.ListAdditionApply)
                        {
                            ListApplyProductClient.Add(new SelectListItem
                            {
                                Value = itemad.ProductID,
                                Text = itemad.Name + (itemad.ParentName == null ? "" : " ( in " + itemad.ParentName + ")")
                            });
                        }
                    }
                    else
                    {
                        ListApplyProductClient.Add(new SelectListItem
                        {
                            Value = product.ProductID,
                            Text = product.ProductName
                        });
                    }
                    ViewBag.ListApplyProductClient = ListApplyProductClient;
                    ViewBag.CountProduct = ListApplyProductClient.Count;
                    int OffSet = 0;
                    _checkIpFactory = new CheckIpAddressFactory();
                    //var _CountryCode = _checkIpFactory.GetCountryCode();
                    ViewBag.CountryDefault = ListCounTry.Where(x => x.Alpha2Code.Equals(CountryCode)).FirstOrDefault().Name;
                    foreach (var item in product.ListStoreApply)
                    {
                        var storeInfo = _MyStoreBusinessfactory.GetStoreInfo(item.StoreID);
                        if (storeInfo != null)
                        {
                            int Industry = 0;
                            var _CounTry = ListCounTry.Where(x => x.Name.Equals(storeInfo.Country)).FirstOrDefault();
                            List<SelectListItem> TimeZone = new List<SelectListItem>();
                            if (_CounTry != null)
                            {
                                foreach (var _itemTimeZone in _CounTry.TimeZones)
                                {
                                    TimeZone.Add(new SelectListItem
                                    {
                                        Text = _itemTimeZone,
                                        Value = _itemTimeZone
                                    });
                                }
                            }
                            ViewBag.ListCounTry = ListCounTry;
                            ViewBag.ListCounTry = ViewBag.ListCounTry ?? "";
                            if (storeInfo.StoreType != 0)
                                Industry = storeInfo.StoreType;
                            else if (_Industry != 0)
                                Industry = _Industry;
                            bool _isActive = false;
                            if (item.IsExtend)
                                _isActive = item.IsExtend;
                            else
                                _isActive = storeInfo.IsActive;
                            model.ListStore.Add(new StoreDetailModels
                            {
                                Index = OffSet++,
                                StoreIDApply = item.ID,
                                IsNew = !item.IsExtend,

                                ID = storeInfo.ID,
                                Country = storeInfo.Country,
                                AssetID = item.ProductID,
                                City = storeInfo.City,
                                Email = storeInfo.Email,
                                GSTRegNo = storeInfo.GSTRegNo,
                                ImageURL = string.IsNullOrEmpty(storeInfo.ImageURL) ? _ImgDummyStore : storeInfo.ImageURL,
                                Industry = Industry.ToString(),
                                IsActive = _isActive,
                                Street = storeInfo.Street,
                                TimeZone = storeInfo.TimeZone,
                                Phone = storeInfo.Phone,
                                Name = storeInfo.Name,
                                ZipCode = storeInfo.ZipCode,
                                ListTimeZone = TimeZone,
                                // add new 
                                TempName = storeInfo.Name,
                                TempEmail = storeInfo.Email,
                                TempCity = storeInfo.City,
                                TempGSTRegNo = storeInfo.GSTRegNo,
                                TempPhone = storeInfo.Phone,
                                TempStreet = storeInfo.Street,
                                TempZipCode = storeInfo.ZipCode,
                                TempCounTry = storeInfo.Country,
                                TempTimeZone = storeInfo.TimeZone
                            });
                        }
                    }
                }
            }

            return View(model);
        }

        //Create Store
        [NuAuth]
        public ActionResult InputStoreInfoAddStore(int NumberStore, string ProductID, int Index)
        {
            InputStoreViewModels model = new InputStoreViewModels();
            model.ID = ProductID;

            //=============
            var data = _factory.GetOrderDetail(ORDERID);
            var product = data.ListItems.Where(x => x.ID.Equals(ProductID)).FirstOrDefault();
            model.ProductType = product.ProductType;
            List<SelectListItem> ListApplyProductClient = new List<SelectListItem>();
            String _Industry = "";
            if (product.ProductType == (byte)Commons.EProductType.Package)
            {
                product.ListComposite = product.ListComposite.Where(x => x.ProductType == (byte)Commons.EProductType.Product).ToList();
                if (product.ListComposite != null && product.ListComposite.Count > 0)
                {
                    foreach (var item in product.ListComposite)
                    {
                        ListApplyProductClient.Add(new SelectListItem
                        {
                            Value = item.ProductID,
                            Text = item.ProductName
                        });
                        model.ListTypePackage.Add(new SelectListItem
                        {
                            Value = item.ProductID,
                            Text = item.Type.ToString()
                        });
                    }
                    model.Type = product.ListComposite.FirstOrDefault().Type;
                }
            }
            else if (product.ProductType == (byte)Commons.EProductType.Addition)
            {
                if (product.AdditionType == (byte)Commons.EAdditionType.Location)
                {
                    foreach (var itemAddition in product.ListAdditionApply)
                    {
                        ListApplyProductClient.Add(new SelectListItem
                        {
                            Value = itemAddition.ID,
                            Text = itemAddition.Name + (itemAddition.ParentName == null ? "" : " ( in " + itemAddition.ParentName + ")")
                        });
                    }

                }
                else
                {
                    ListApplyProductClient.Add(new SelectListItem
                    {
                        Value = product.ProductID,
                        Text = product.ProductName
                    });
                }
                if (product.ListAdditionApply != null && product.ListAdditionApply.Count > 0)
                {
                    var item = product.ListAdditionApply.FirstOrDefault();
                    if (item.Type == (int)Commons.EType.NuDisplay || item.Type == (int)Commons.EType.NuKiosk || item.Type == (int)Commons.EType.NuPOS)
                        _Industry = Commons.EStoreType.FnB.ToString("d");
                    else
                    {
                        if (item.Type == (int)Commons.EType.POinS_Link_App)
                        {
                            //_Industry = Commons.EStoreType.FnB.ToString("d");
                            ViewBag.IsDisabledIndustry = false;
                        }
                        else
                            _Industry = Commons.EStoreType.Beauty.ToString("d");
                    }
                    model.Type = item.Type;
                }
            }
            else
            {
                ListApplyProductClient.Add(new SelectListItem
                {
                    Value = product.ProductID,
                    Text = product.ProductName
                });
                if (product.Type == (int)Commons.EType.NuKiosk || product.Type == (int)Commons.EType.NuPOS || product.Type == (int)Commons.EType.NuDisplay)
                    _Industry = Commons.EStoreType.FnB.ToString("d");
                else
                {
                    if (product.Type == (int)Commons.EType.POinS_Link_App)
                    {
                        //_Industry = Commons.EStoreType.FnB.ToString("d");
                        ViewBag.IsDisabledIndustry = false;
                    }
                    else
                        _Industry = Commons.EStoreType.Beauty.ToString("d");
                }
                model.Type = product.Type;
            }
            ViewBag.ListApplyProductClient = ListApplyProductClient;
            //==========
            _checkIpFactory = new CheckIpAddressFactory();
            var _listCounTry = _baseFactory.GetListCountry();
            //var CountryCode = _checkIpFactory.GetCountryCode();
            var _Country = "";
            List<SelectListItem> TimeZone = new List<SelectListItem>();

            if (_listCounTry != null)
            {
                var objCountry = _listCounTry.Where(x => x.Alpha2Code.Equals(CountryCode)).FirstOrDefault();
                _Country = objCountry != null ? objCountry.Name : "";
                var _TimeZone = objCountry.TimeZones;
                if (_TimeZone != null)
                {
                    foreach (var _itemTimezone in _TimeZone)
                    {
                        TimeZone.Add(new SelectListItem
                        {
                            Value = _itemTimezone,
                            Text = _itemTimezone
                        });
                    }

                }
            }
            ViewBag.ListCounTry = _listCounTry;
            ViewBag.ListCounTry = ViewBag.ListCounTry ?? "";
            ViewBag.TimeZones = ViewBag.TimeZones ?? "";
            for (int i = Index; i < NumberStore + Index; i++)
            {

                model.ListStore.Add(new StoreDetailModels
                {
                    Index = i,
                    Name = "Store " + (i + 1),
                    IsNew = true,
                    ImageURL = _ImgDummyStore,
                    Country = _Country,
                    ListTimeZone = TimeZone,
                    Industry = _Industry,
                    IsActive = true
                });
            }
            return PartialView("_ItemStoreTemp", model);
        }

        [NuAuth]
        public ActionResult GetDetailStoreToJSon(string StoreId)
        {
            StoreDetailModels store = new StoreDetailModels();
            store = _MyStoreBusinessfactory.GetStoreInfo(StoreId);
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

        //[NuAuth]
        //[HttpPost]
        //public ActionResult CreateStoreInfoTemp(InputStoreViewModels model)
        //{
        //    try
        //    {

        //    }catch(Exception ex) { }
        //}


        [NuAuth]
        [HttpPost]
        public ActionResult CreateStoreInfoTemp(InputStoreViewModels model)
        {
            //Dung update 14/12/2017
            try
            {
                List<ApplyStore> listApplyStore = new List<ApplyStore>();

                bool result = true;
                string msgResult = "";
                byte[] photoByte = null;
                bool IsValid = true;
                model.ListStore = model.ListStore.Where(x => !x.IsDelete).ToList();
                if (model.ListStore != null && model.ListStore.Count > 0)
                {
                    foreach (var item in model.ListStore.OrderBy(x => x.IsNew))
                    {

                        ApplyStore applyStore = new ApplyStore();
                        if (item.IsDelete) //Store Delete
                        {
                            //applyStore.ID = item.StoreIDApply;
                            applyStore.StoreID = item.ID;
                            applyStore.StoreName = item.StoreNameExist;
                            applyStore.IsDelete = item.IsDelete;
                            applyStore.IsExtend = !item.IsNew;
                        }
                        else
                        {
                            if (!item.IsNew)//Existing Store 
                            {

                                // set value 
                                item.Name = item.TempName;
                                item.Email = item.TempEmail;
                                item.GSTRegNo = item.TempGSTRegNo;
                                item.Phone = item.TempPhone;
                                item.City = item.TempCity;
                                item.Street = item.TempStreet;
                                item.ZipCode = item.TempZipCode;
                                item.Country = item.TempCounTry;
                                item.ListTimeZone = GetListTimeZone(item.TempCounTry);
                                item.TimeZone = item.TempTimeZone;
                                item.IsActive = true;

                                listStorePropertiesReject = new List<string>();
                                listStorePropertiesReject.Add("Name");
                                listStorePropertiesReject.Add("Email");
                                listStorePropertiesReject.Add("Phone");
                                listStorePropertiesReject.Add("Industry");
                                listStorePropertiesReject.Add("TimeZone");
                                PropertyReject();
                                //================
                                //applyStore.ID = item.StoreIDApply;
                                applyStore.StoreID = item.ID;
                                applyStore.StoreName = item.StoreNameExist;
                                applyStore.IsDelete = item.IsDelete;
                                applyStore.IsExtend = !item.IsNew;
                                applyStore.ProductID = item.ProductID;


                            }
                            else //Create New Store
                            {
                                if (string.IsNullOrEmpty(item.Name))
                                {
                                    result = false;
                                    ModelState.AddModelError("ListStore[" + item.Index + "].Name", "The Name is required");

                                }
                                if (string.IsNullOrEmpty(item.Email))
                                {
                                    result = false;
                                    ModelState.AddModelError("ListStore[" + item.Index + "].Email", "The email address is required");

                                }
                                if (string.IsNullOrEmpty(item.Phone))
                                {
                                    result = false;
                                    ModelState.AddModelError("ListStore[" + item.Index + "].Phone", "The Phone is required");

                                }
                                if (!ModelState.IsValid)
                                {
                                    IsValid = false;
                                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                                    var data = _factory.GetOrderDetail(ORDERID);
                                    var product = data.ListItems.Where(x => x.ID.Equals(model.ID)).FirstOrDefault();
                                    List<SelectListItem> ListApplyProductClient = new List<SelectListItem>();
                                    if (product != null && product.ProductType == (byte)Commons.EProductType.Package)
                                    {
                                        product.ListComposite = product.ListComposite.Where(x => x.ProductType == (byte)Commons.EProductType.Product).ToList();
                                        if (product.ListComposite != null && product.ListComposite.Count > 0)
                                        {
                                            foreach (var _item in product.ListComposite)
                                            {
                                                ListApplyProductClient.Add(new SelectListItem
                                                {
                                                    Value = _item.ProductID,
                                                    Text = _item.ProductName
                                                });
                                                model.ListTypePackage.Add(new SelectListItem
                                                {
                                                    Text = _item.Type.ToString(),
                                                    Value = _item.ProductID
                                                });
                                            }
                                        }
                                    }
                                    else if (product != null && product.ProductType == (byte)Commons.EProductType.Addition)
                                    {
                                        var _item = product.ListAdditionApply.FirstOrDefault();
                                        ListApplyProductClient.Add(new SelectListItem
                                        {
                                            Value = _item.ID,
                                            Text = _item.Name + (_item.ParentName == null ? "" : " ( in " + _item.ParentName + ")")
                                        });
                                    }
                                    else if (product != null && product.ProductType == (byte)Commons.EProductType.Product)
                                    {
                                        ListApplyProductClient.Add(new SelectListItem
                                        {
                                            Value = product.ProductID,
                                            Text = product.ProductName
                                        });
                                    }
                                    ViewBag.ListApplyProductClient = ListApplyProductClient;
                                    ViewBag.ListCounTry = _baseFactory.GetListCountry();
                                    ViewBag.ListCounTry = ViewBag.ListCounTry ?? "";
                                    item.ListTimeZone = GetListTimeZone(item.Country);
                                    if (model.Type == (int)Commons.EType.POinS_Link_App)
                                        ViewBag.IsDisabledIndustry = false;

                                }
                                if (IsValid)
                                {
                                    //====================
                                    if (item.PictureUpload != null && item.PictureUpload.ContentLength > 0)
                                    {
                                        Byte[] imgByte = new Byte[item.PictureUpload.ContentLength];
                                        item.PictureUpload.InputStream.Read(imgByte, 0, item.PictureUpload.ContentLength);
                                        item.PictureByte = imgByte;
                                        item.ImageURL = Guid.NewGuid() + Path.GetExtension(item.PictureUpload.FileName);
                                        item.PictureUpload = null;
                                        photoByte = imgByte;
                                    }
                                    else
                                    {
                                        if (!string.IsNullOrEmpty(item.ImageURL))
                                            item.ImageURL = item.ImageURL.Replace(Commons.PublicImagesClient, "").Replace(_ImgDummyStore, "");
                                    }
                                    item.ExpriedDate = Commons.MaxDate;
                                    var tmp = item.PictureByte;
                                    item.PictureByte = null;
                                    string msg = "";
                                    string StoreIDReturn = "";
                                    var success = _factory.CreateStoreInfoTemp(item, CurrentUser.UserId, ref msg, ref StoreIDReturn);
                                    if (success)
                                    {
                                        item.PictureByte = tmp;
                                        if (!string.IsNullOrEmpty(item.ImageURL) && item.PictureByte != null)
                                        {
                                            var originalDirectory = new DirectoryInfo(string.Format("{0}Uploads\\", Server.MapPath(@"\")));
                                            var path = string.Format("{0}{1}", originalDirectory, item.ImageURL);
                                            MemoryStream ms = new MemoryStream(photoByte, 0, photoByte.Length);
                                            ms.Write(photoByte, 0, photoByte.Length);
                                            System.Drawing.Image imageTmp = System.Drawing.Image.FromStream(ms, true);
                                            ImageHelper.Me.SaveCroppedImage(imageTmp, path, item.ImageURL, ref photoByte);
                                            item.PictureByte = photoByte;
                                            FTP.UploadClient(item.ImageURL, item.PictureByte);
                                            ImageHelper.Me.TryDeleteImageUpdated(path);
                                        }
                                        //========
                                        //applyStore.ID =  item.StoreIDApply;
                                        applyStore.StoreID = StoreIDReturn;
                                        applyStore.StoreName = item.Name;
                                        applyStore.IsDelete = item.IsDelete;
                                        applyStore.IsExtend = !item.IsNew;
                                        applyStore.ProductID = item.ProductID;
                                    }
                                    else
                                    {
                                        result = false;
                                        ModelState.AddModelError("MsgStore" + item.Index, msgResult);
                                        break;
                                    }
                                    //msgResult += "Store [" + item.Name + "]: " + msg + "<br/>";
                                }

                            }
                        }

                        //=======Add List Store Apply
                        listApplyStore.Add(applyStore);
                    }
                    if (!IsValid)
                        return PartialView("_ItemStoreTemp", model);
                }

                if (result)
                {
                    //Update ListStoreApplied
                    AddItemToOrderModels modelAddItems = new AddItemToOrderModels();
                    modelAddItems.CusID = CurrentUser.UserId;
                    modelAddItems.OrderID = ORDERID;
                    List<Item> ListItems = new List<Item>();
                    Item product = new Item();
                    var data = _factory.GetOrderDetail(ORDERID);
                    if (data != null && data.ListItems != null && data.ListItems.Count > 0)
                    {
                        product = data.ListItems.Where(x => x.ID.Equals(model.OrderProductDetailID)).FirstOrDefault();
                        if (product != null)
                        {
                            if (product.ListStoreApply != null && product.ListStoreApply.Count > 0)
                                product.ListStoreApply.ForEach(x => x.IsDelete = true);
                            product.ListStoreApply.AddRange(listApplyStore);
                        }
                    }
                    //product.ID = model.OrderProductDetailID;
                    //product.ProductType = (byte)Commons.EProductType.Product;
                    //product.ListStoreApply = listApplyStore;
                    ListItems.Add(product);
                    modelAddItems.ListItems = ListItems;
                    //====================
                    string msg = "";
                    bool success = false;
                    OrderDetailModels OrderDetail = _factory.AddItems(modelAddItems, ref success, ref msg);
                    if (success)
                    {
                        //ORDERID = OrderDetail.ID;
                        return Json(OrderDetail, JsonRequestBehavior.AllowGet);
                    }
                    else
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, Commons.MsgErrorForClient);
                    //End Update ListStoreApplied
                    //return new HttpStatusCodeResult(HttpStatusCode.OK);
                }
                else
                {
                    ModelState.AddModelError("MsgInputStoreInfo", msgResult);
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return PartialView("_ItemStoreTemp", model);
                    //return View(model);
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("CreateStoreInfo: ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }
        public List<SelectListItem> GetListTimeZone(string _country)
        {
            List<SelectListItem> _ListTimeZone = new List<SelectListItem>();
            var listTimezone = _baseFactory.GetListCountry().Where(x => x.Name.Equals(_country)).FirstOrDefault();
            if (listTimezone != null)
            {
                foreach (var itemTimezone in listTimezone.TimeZones)
                {
                    _ListTimeZone.Add(new SelectListItem
                    {
                        Value = itemTimezone,
                        Text = itemTimezone
                    });
                }
            }
            return _ListTimeZone;
        }
        public string PeriodName(double Period, int PeriodType)
        {
            string _PeriodName = "";
            if (PeriodType == (byte)Commons.EPeriodType.OneTime)
                _PeriodName = Commons.PeriodTypeOneTime;
            else
            {
                if (PeriodType == (byte)Commons.EPeriodType.Day)
                    _PeriodName = Commons.PeriodTypeDay;
                else if (PeriodType == (byte)Commons.EPeriodType.Month)
                    _PeriodName = Commons.PeriodTypeMonth;
                else if (PeriodType == (byte)Commons.EPeriodType.Year)
                    _PeriodName = Commons.PeriodTypeYear;
                if (Period > 1)
                    _PeriodName += "s";
            }
            return _PeriodName;
        }
        #endregion /*End Input Store Info*/
        /*End Check Out*/

        public ActionResult OrderMerge(string OrderIdFrom, string OrderIdTo)
        {
            var msg = string.Empty;
            try
            {
                var result = _factory.OrderMerge(OrderIdFrom, OrderIdTo, ref msg);
                if (result)
                    return new HttpStatusCodeResult(200, msg);
                else
                    return new HttpStatusCodeResult(400, msg);
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("OrderMerge ", ex);
                return new HttpStatusCodeResult(400, msg);
            }
        }

        public ActionResult DeleteOrder()
        {
            var msg = string.Empty;
            try
            {
                var result = _factory.DeleteOrder(ORDERID, CurrentUser.UserId, ref msg);
                if (result)
                {
                    ORDERID = "";
                    return new HttpStatusCodeResult(HttpStatusCode.OK);
                }
                else
                {
                    return new HttpStatusCodeResult(400, msg);
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("OrderDelete ", ex);
                return new HttpStatusCodeResult(400, msg);
            }
        }

        public string ReplaceDiscountCode(string code)
        {
            string result = "";
            try
            {
                if (string.IsNullOrEmpty(code))
                    result = "*******";
                else
                    result = "***" + code.Substring(code.Length - 4, code.Length - (code.Length - 4));
            }
            catch (Exception ex)
            {
                result = "*******";
            }
            return result;
        }
        public ActionResult GetTimeZones(string _Country, int index)
        {
            StoreDetailModels model = new StoreDetailModels();
            model.Index = index;
            ViewBag.TimeZones = "";
            if (_Country != "")
            {
                var Country = _baseFactory.GetListCountry();
                for (int i = 0; i < Country.Count; i++)
                {
                    if (Country[i].Name == _Country)
                    {
                        //ViewBag.TimeZones = Country[i].TimeZones;
                        foreach (var item in Country[i].TimeZones)
                        {
                            model.ListTimeZone.Add(new SelectListItem
                            {

                                Text = item,
                                Value = item
                            });
                        }
                    }
                }
            }
            //=======
            return PartialView("_TimeZones", model);
        }
        [HttpGet]
        public ActionResult InputInformation()
        {
            return View("InputInformation");
        }
        public ActionResult InputStore()
        {
            return PartialView("_InputStore");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ProductType"></param>
        /// <returns></returns>
        public JsonResult CreateOrEditStore(int ProductType, string CusID, string ProductID, bool isReseller)
        {
            if (isReseller)
            {
                var cusIdCookie = Request.Cookies["cusIDcookies"];
                if (cusIdCookie != null)
                {
                    var strId = Server.UrlDecode(cusIdCookie.Value);
                    CusID = JsonConvert.DeserializeObject<string>(strId);
                }
            }
            StoreDetailModels model = new StoreDetailModels();
            model = _MyStoreBusinessfactory.GetStoreInfo(null);
            int _Industry = (int)Commons.EStoreType.FnB;
            try
            {
                if (ProductType == (int)Commons.EType.POZZ || ProductType == (int)Commons.EType.POZZ_Display || ProductType == (int)Commons.EType.POZZ_Kiosk)
                {
                    _Industry = (int)Commons.EStoreType.Beauty;
                }
                var listproductapplied = _MyStoreBusinessfactory.GetListProductForStore(CusID, "");
                if (listproductapplied != null && listproductapplied.Count > 0)
                {
                    listproductapplied = listproductapplied.Where(w => w.Type == _Industry).ToList();
                    //------------------------------------

                    listproductapplied = listproductapplied.Where(ww => ww.ProductType != (int)Commons.EProductType.Addition).ToList();
                    int i = 0;
                    int OffSet = 0;
                    foreach (var item in listproductapplied)
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
                    model.ListProductApply.ForEach(o =>
                    {
                        o.IsApply = true;
                    });
                    model.ListProductApply.ForEach(x =>
                    {
                        if (x.ExpiredTime.HasValue)
                        {
                            x.ExpiredTime = CommonHelper.ConvertToLocalTime(x.ExpiredTime.Value);
                            x.sExpiredTime = x.ExpiredTime.ToString();
                        }

                        if (x.ActiveTime.HasValue)
                        {
                            x.ActiveTime = CommonHelper.ConvertToLocalTime(x.ActiveTime.Value);
                            x.sActiveTime = x.ActiveTime.ToString();
                        }

                    });
                    model.ApplyProductCount = model.ListProductApply.Count(x => x.IsApply) + " Product(s)";
                    model.ImageURL = string.IsNullOrEmpty(model.ImageURL) ? _ImgDummyStore : model.ImageURL;
                    model.Industry = _Industry.ToString(); //model.StoreType.ToString();
                    model.CusID = CusID;
                    model.IsNew = true;
                    model.IsExistStore = false;

                    //model.ProductID = ProductID;
                    //model.AssetID = ProductID;
                    //-------
                    var listCountry = _baseFactory.GetListCountry();
                    model.Countries = listCountry;
                    var Country = listCountry.Where(x => x.Name == model.Country).FirstOrDefault();
                    ViewBag.TimeZones = "";
                    if (Country != null)
                        model.TimeZones = Country.TimeZones;
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("CreateOrEditStore:", ex);
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult CreateStoreInfo(StoreDetailModels model, string OrderId)
        {
            OrderDetailModels OrderDetail = new OrderDetailModels();
            try
            {
                byte[] photoByte = null;
                //model.TimeZone = "UTC+07:00";
                //model.IsNew = true;
                //model.CusID = "d1f2dcd4-e0c4-488f-9a44-2ca2cedc0f7f";
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
                    result = _factory.CreateStoreInfoTemp(model, model.CusID, ref msg, ref StoreIDReturn);
                }
                else
                {
                    result = _MyStoreBusinessfactory.CreateStoreInfo(model, model.CusID, ref StoreIDReturn, ref msg);

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
                    OrderDetail = GetOrderDetail(OrderId, false);
                    if (OrderDetail != null && OrderDetail.ListItems != null && OrderDetail.ListItems.Any())
                    {
                        OrderDetail.ListItems.ForEach(o =>
                        {
                            o.NumOfStore = o.NumOfStore == -1 ? o.NumOfStore : o.NumOfStore - (o.ListStoreApply == null ? 0 : o.ListStoreApply.Count);
                            if (o.ListItem != null && o.ListItem.Count > 0)
                            {
                                o.ListItem.ForEach(x =>
                                {
                                    if (x.AdditionType == (int)Commons.EAdditionType.Location)
                                    {
                                        if (o.NumOfStore == -1 || x.AmountOfUnit == -1)
                                            o.NumOfStore = -1;
                                        else
                                            o.NumOfStore += Convert.ToInt16(x.Quantity * x.AmountOfUnit);
                                    }
                                });
                            }
                            var ListCounTry = _baseFactory.GetListCountry();
                            if (o.NumOfStore == -1)
                                o.sNumOfStore = "Unlimited";
                            else
                                o.sNumOfStore = o.NumOfStore.ToString(); //product.NumOfStore.ToString();
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("CreateStoreInfo:", ex);
            }
            return Json(OrderDetail, JsonRequestBehavior.AllowGet);
        }
        public ActionResult InputMerchantInfoForCusOfReseller()
        {
            return PartialView("_InputMerchantInfoForCusOfReseller");
        }
        public ActionResult Comleted(string OrderId)
        {
            bool result = true;
            string msg = "";
            var Html = "";
            var ReceiptNo = "";
            try
            {
                result = _factory.OrderDonePay(OrderId, ref msg);
                if(result)
                {
                    ORDERID = "";
                    var OrderDetail = _factory.GetOrderDetail(OrderId, false);
                    if(OrderDetail != null)
                    {
                        ReceiptNo = OrderDetail.ReceiptNo;
                    }
                    using (StringWriter writer = new StringWriter())
                    {
                        ViewEngineResult vResult = ViewEngines.Engines.FindPartialView(ControllerContext, "_Feedback");
                        ViewContext vContext = new ViewContext(this.ControllerContext, vResult.View, ViewData, new TempDataDictionary(), writer);
                        vResult.View.Render(vContext, writer);
                        Html =  writer.ToString();
                    }
                  ///  return PartialView("_Feedback");
                }
            }
            catch(Exception ex)
            {
                NSLog.Logger.Error("Comleted:", ex);
            }
            var obj = new
            {
                Result = result,
                Msg = msg,
                Html = Html,
                ReceiptNo = ReceiptNo
            };
            return Json(obj, JsonRequestBehavior.AllowGet);
            
        }
        public ActionResult ResellerRegisCustomer(AccountRegisterRequest data) {
            ResponseAccountRegisterModel rp = new ResponseAccountRegisterModel();
            string CustomerIDreturn = "";
            string MerchantIDreturn = "";
            string msg = "";
            data.ResellerID = CurrentUser.UserId;
            bool result =  _factory.ResellerRegisCustomer(data, ref CustomerIDreturn, ref MerchantIDreturn, ref msg);
            rp.IsSuccess = result;
            rp.CustomerID = CustomerIDreturn;
            rp.MerchantID = MerchantIDreturn;
            rp.msg = msg;
            var jsonResult = Json(rp, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        public ActionResult Finish(ref object data)
        {
            //DTO dtoFactory = ((Newtonsoft.Json.Linq.JObject)data).ToObject<DTO>();
            return PartialView("");
        }
        public ActionResult Submit(OrderDetailModels dataCheckout)
        {
            return RedirectToAction("MyStoreAndBusiness");
        }
        /* page cart */
        [NuAuth]
        public ActionResult Cart()
        {
            try
            {
                /* get list product of customer */
                var data = CurrentUser.ListProduct;
                if (data != null && data.Any())
                {
                    var dataProduct = data.Select(o => new ProductDetailModels
                    {
                        ImageURL = string.IsNullOrEmpty(o.ImageURL) ? ((o.ProductType == (int)Commons.EProductType.Product) ? _ImgDummyProduct : _ImgDummyPackage) : o.ImageURL,
                        Name = string.IsNullOrEmpty(o.Name) ? "" : o.Name.Replace("\t", " "),
                        ListFunction = o.ListFunction,
                        NumberOfAccount = o.NumberOfAccount,
                        AmountOfUnit = o.AmountOfUnit,
                        ListPrice = o.ListPrice,
                        ID = o.ID,
                        ProductType = o.ProductType,
                        AdditionType = o.AdditionType,
                        ListPeriodType = o.ListPeriodType,
                    }).ToList();
                    if (dataProduct != null && dataProduct.Any())
                    {
                        dataProduct.ForEach(o =>
                        {
                            if (o.ListPrice != null && o.ListPrice.Any())
                            {
                                o.ListPrice.ForEach(xx =>
                                {
                                    xx.NamePeriodType = PeriodName(xx.Period, xx.PeriodType);
                                });
                                var PriceMin = o.ListPrice.OrderBy(z => z.PeriodType).ThenBy(w => w.Period).ThenBy(w => w.Price).FirstOrDefault();
                                o.Price = PriceMin.Price;
                                o.Period = (int)PriceMin.Period;
                                o.PeriodType = PriceMin.PeriodType;
                                o.sPeriodType = PriceMin.PeriodType.ToString();
                                // Period Time != One Time
                                if (o.PeriodType != (byte)Commons.EPeriodType.OneTime)
                                {
                                    o.PeriodTime = PriceMin.Period + " " + PriceMin.NamePeriodType;
                                }
                                else
                                {
                                    o.PeriodTime = PriceMin.NamePeriodType;
                                }
                                o.CusID = CurrentUser.UserId;
                            }
                        });
                    }
                    ViewBag.ListProducts = dataProduct;

                    /* check order have item */
                    var tempOrderDetail = GetOrderDetail(ORDERID);
                    if (tempOrderDetail != null && tempOrderDetail.ListItems.Count > 0)
                    {
                        ViewBag.OrderDetail = tempOrderDetail;
                    }
                    else
                    {
                        //check cookie
                        var Cookie = Request.Cookies["csc-order-v2"];
                        if (Cookie != null)
                        {
                            Cookie.Value = null;
                            Response.Cookies.Add(Cookie);
                        }

                        // add to cart
                        AddItemToOrderModels modelAddItems = new AddItemToOrderModels();
                        modelAddItems.CusID = CurrentUser.UserId;
                        modelAddItems.OrderID = "";
                        List<Item> ListItems = new List<Item>();
                        //===============
                        var firstProduct = dataProduct.FirstOrDefault();
                        Item product = new Item();
                        product.ID = "";
                        product.ProductID = firstProduct.ID;
                        product.ProductName = firstProduct.Name;
                        product.ProductType = firstProduct.ProductType;
                        product.AdditionType = firstProduct.AdditionType;
                        product.ImageURL = firstProduct.ImageURL;
                        product.Period = firstProduct.Period;
                        product.PeriodType = firstProduct.PeriodType;
                        product.Quantity = 1;
                        product.Price = firstProduct.Price;
                        product.NumOfStore = firstProduct.IncludeStore;
                        product.NumOfAccount = firstProduct.NumberOfAccount;
                        product.AmountOfUnit = firstProduct.AmountOfUnit;
                        product.ListAdditionApply = new List<Shared.Models.ClientSite.MyProfile.AdditionApply>();
                        //=========
                        ListItems.Add(product);
                        modelAddItems.ListItems = ListItems;
                        modelAddItems.IsFree = false;
                        string msg = "";
                        bool success = false;
                        OrderDetailModels OrderDetail = _factory.AddItems(modelAddItems, ref success, ref msg);
                        if (success)
                        {
                            ORDERID = OrderDetail.ID;
                            if (OrderDetail.ListItems != null && OrderDetail.ListItems.Count > 0)
                            {
                                OrderDetail.CustomerID = CurrentUser.UserId;
                                OrderDetail.CurrencySymbol = CurrentUser.CurrencySymbol;
                                OrderDetail.TotalQuantity = (int)OrderDetail.ListItems.Sum(x => x.Quantity);
                                OrderDetail.ListItems.ForEach(o =>
                                {
                                    o.ImageURL = string.IsNullOrEmpty(o.ImageURL) ? ((o.ProductType == (int)Commons.EProductType.Product)
                                                    ? _ImgDummyProduct : _ImgDummyPackage) : o.ImageURL;
                                    o.ProductName = string.IsNullOrEmpty(o.ProductName) ? "" : o.ProductName;
                                    o.NamePeriodType = PeriodName(o.Period, o.PeriodType);
                                    if (o.PeriodType != (byte)Commons.EPeriodType.OneTime)
                                    {
                                        o.PeriodTime = o.Period + " " + o.NamePeriodType;
                                    }
                                    else
                                    {
                                        o.PeriodTime = o.NamePeriodType;
                                    }

                                    /* total price */
                                    o.TotalPrice = o.Quantity * o.Price;
                                    o.CusID = OrderDetail.CustomerID;
                                    o.sPeriodType = o.PeriodType.ToString();
                                    o.ListPeriodType = o.ListPrice.Select(x => new SelectListItem
                                    {
                                        Value = x.PeriodType.ToString(),
                                        Text = PeriodName(x.Period, x.PeriodType)
                                    }).OrderBy(x => x.Text).ToList(); //CurrentUser.ListProduct.Where(x => x.ID.Equals(o.ProductID)).Select(x => x.ListPeriodType).FirstOrDefault();
                                });
                            }
                            if (OrderDetail.BillDiscount != null)
                            {
                                OrderDetail.TotalDiscount = OrderDetail.BillDiscount.DiscountValue;
                                OrderDetail.DiscountType = OrderDetail.BillDiscount.DiscountType;
                            }
                            if (OrderDetail.TaxInfo != null)
                            {
                                if (OrderDetail.TaxInfo.TaxType == (byte)Commons.ETaxType.AddOn)
                                {
                                    OrderDetail.sTaxName = "(Add on " + OrderDetail.TaxInfo.TaxPercent + "%)";
                                }
                                if (OrderDetail.TaxInfo.TaxType == (byte)Commons.ETaxType.Inclusive)
                                {
                                    OrderDetail.sTaxName = "(Inclusive " + OrderDetail.TaxInfo.TaxPercent + "%)";
                                }
                                if (OrderDetail.TaxInfo.TaxType == (byte)Commons.ETaxType.TaxExempt)
                                {
                                    OrderDetail.sTaxName = "(Tax Exempt " + OrderDetail.TaxInfo.TaxPercent + "%)";
                                }
                            }
                            ViewBag.OrderDetail = OrderDetail;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("Cart_Error:", ex);
            }
            return View();
        }

        public OrderDetailModels GetOrderDetail(string OrderId, bool isCheckvalid = true)
        {
            OrderDetailModels OrderDetail = new OrderDetailModels();
            try
            {
                OrderDetail = _factory.GetOrderDetail(OrderId, isCheckvalid);
                if (OrderDetail != null)
                {
                    if (OrderDetail.ListItems != null && OrderDetail.ListItems.Count > 0)
                    {
                        OrderDetail.CurrencySymbol = CurrentUser.CurrencySymbol;
                        OrderDetail.CustomerID = CurrentUser.UserId;
                        OrderDetail.TotalQuantity = (int)OrderDetail.ListItems.Sum(x => x.Quantity);
                        OrderDetail.ListItems.ForEach(o =>
                        {
                            o.ProductName = string.IsNullOrEmpty(o.ProductName) ? "" : o.ProductName;
                            o.NamePeriodType = PeriodName(o.Period, o.PeriodType);
                            if (o.PeriodType != (byte)Commons.EPeriodType.OneTime)
                            {
                                o.PeriodTime = o.Period + " " + o.NamePeriodType;
                            }
                            else
                            {
                                o.PeriodTime = o.NamePeriodType;
                            }

                            /* total price */
                            o.TotalPrice = o.Quantity * o.Price;
                            o.CusID = OrderDetail.CustomerID;
                            o.sPeriodType = o.PeriodType.ToString();
                            o.ListPeriodType = o.ListPrice.Select(x => new SelectListItem
                            {
                                Value = x.PeriodType.ToString(),
                                Text = PeriodName(x.Period, x.PeriodType)
                            }).OrderBy(x => x.Text).ToList(); //CurrentUser.ListProduct.Where(x => x.ID.Equals(o.ProductID)).Select(x => x.ListPeriodType).FirstOrDefault();
                        });
                    }
                    if (OrderDetail.BillDiscount != null)
                    {
                        OrderDetail.TotalDiscount = OrderDetail.BillDiscount.DiscountValue;
                        OrderDetail.DiscountType = OrderDetail.BillDiscount.DiscountType;
                    }
                    if (OrderDetail.TaxInfo != null)
                    {
                        if (OrderDetail.TaxInfo.TaxType == (byte)Commons.ETaxType.AddOn)
                        {
                            OrderDetail.sTaxName = "(Add on " + OrderDetail.TaxInfo.TaxPercent + "%)";
                        }
                        if (OrderDetail.TaxInfo.TaxType == (byte)Commons.ETaxType.Inclusive)
                        {
                            OrderDetail.sTaxName = "(Inclusive " + OrderDetail.TaxInfo.TaxPercent + "%)";
                        }
                        if (OrderDetail.TaxInfo.TaxType == (byte)Commons.ETaxType.TaxExempt)
                        {
                            OrderDetail.sTaxName = "(Tax Exempt " + OrderDetail.TaxInfo.TaxPercent + "%)";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("GetOrderDetail", ex);
            }
            return OrderDetail;
        }
        public ActionResult CustomerCheckOut()
        {
            return View();
        }
        public ActionResult CustomerMakePayment()
        {
            return PartialView("_CustomerMakePayment");
        }

        [HttpPost]
        public JsonResult CustomerCreateStoreTemp(StoreDetailModels model, string OrderId)
        {
            bool success = false;
            string msg = "";
            try
            {
                byte[] photoByte = null;
                //model.TimeZone = "UTC+07:00";
                //model.IsNew = true;
                //model.CusID = "d1f2dcd4-e0c4-488f-9a44-2ca2cedc0f7f";
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
                List<string> LstAssetID = new List<string>();
                LstAssetID = model.ListProductApply.Where(w => w.IsApply).Select(o => o.AssetID).ToList();
                model.ListAssetID.AddRange(LstAssetID);                
                model.ProductID = model.AssetID;
                string StoreIDReturn = "";
                var tempPB = model.PictureByte;
                model.PictureByte = null;
                var result = true;
                result = _factory.CreateStoreInfoTemp(model, model.CusID, ref msg, ref StoreIDReturn);
                if (result)
                {
                    AddItemToOrderModels modelAddItems = new AddItemToOrderModels();
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
                    List<ApplyStore> listApplyStore = new List<ApplyStore>();
                    ApplyStore applyStore = new ApplyStore();
                    applyStore.StoreID = StoreIDReturn;
                    applyStore.StoreName = model.Name;
                    applyStore.IsDelete = model.IsDelete;
                    applyStore.IsExtend = !model.IsNew;
                    applyStore.ProductID = model.ProductID;
                    listApplyStore.Add(applyStore);
                    List<Item> ListItems = new List<Item>();
                    Item product = new Item();
                    var data = _factory.GetOrderDetail(ORDERID);
                    if (data != null && data.ListItems != null && data.ListItems.Count > 0 && product != null)
                    {
                        // product = data.ListItems.Where(x => x.ID.Equals(model.OrderProductDetailID)).FirstOrDefault();

                        if (product.ListStoreApply != null && product.ListStoreApply.Count > 0)
                            product.ListStoreApply.ForEach(x => x.IsDelete = true);
                        product.ListStoreApply.AddRange(listApplyStore);

                    }
                    ListItems.Add(product);
                    modelAddItems.ListItems = ListItems;
                    modelAddItems.CusID = model.CusID;
                    modelAddItems.OrderID = OrderId;
                    OrderDetailModels OrderDetail = _factory.AddItems(modelAddItems, ref success, ref msg);
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("CreateStoreInfo:", ex);
            }
            var rp = new { Success = success, Mes = msg };
            return Json(rp, JsonRequestBehavior.AllowGet);
        }

        public ActionResult _CustomerInputStoreInf()
        {
            return PartialView("_CustomerInputStoreInf");
        }
        public ActionResult CustomerDonePayment(OrderDetailModels dataCheckout, string paymentId, string TransactionId, string Note) {
            string OrderIDReturn = "";
            string msg = "";
            bool result = _factory.OrderPay(dataCheckout.ID, paymentId, dataCheckout.SubTotal, CurrentUser.UserId, TransactionId, Note, ref OrderIDReturn, ref msg);
            var rp = new { Success = result, OrderID = OrderIDReturn, Message = msg };
            return Json(rp, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetOrderDetailJson(string OrderId, bool IsReseller)
        {
            OrderDetailModels OrderDetail = new OrderDetailModels();
            try
            {
                OrderDetail = _factory.GetOrderDetail(OrderId, false);

                if (OrderDetail != null)
                {
                    if (OrderDetail.ListItems != null && OrderDetail.ListItems.Count > 0)
                    {
                        OrderDetail.CurrencySymbol = CurrentUser.CurrencySymbol;
                        OrderDetail.CustomerID = CurrentUser.UserId;
                        OrderDetail.TotalQuantity = (int)OrderDetail.ListItems.Sum(x => x.Quantity);
                        OrderDetail.ListItems.ForEach(o =>
                        {
                            o.NumOfStore = o.NumOfStore == -1 ? o.NumOfStore : o.NumOfStore - (o.ListStoreApply == null ? 0 : o.ListStoreApply.Count);
                            if (o.ListItem != null && o.ListItem.Count > 0)
                            {
                                o.ListItem.ForEach(x =>
                                {
                                    if (x.AdditionType == (int)Commons.EAdditionType.Location)
                                    {
                                        if (o.NumOfStore == -1 || x.AmountOfUnit == -1)
                                            o.NumOfStore = -1;
                                        else
                                            o.NumOfStore += Convert.ToInt16(x.Quantity * x.AmountOfUnit);
                                    }
                                });
                            }
                            var ListCounTry = _baseFactory.GetListCountry();
                            if (o.NumOfStore == -1)
                                o.sNumOfStore = "Unlimited";
                            else
                                o.sNumOfStore = o.NumOfStore.ToString(); //product.NumOfStore.ToString();

                            o.ProductName = string.IsNullOrEmpty(o.ProductName) ? "" : o.ProductName;
                            o.NamePeriodType = PeriodName(o.Period, o.PeriodType);
                            if (o.PeriodType != (byte)Commons.EPeriodType.OneTime)
                            {
                                o.PeriodTime = o.Period + " " + o.NamePeriodType;
                            }
                            else
                            {
                                o.PeriodTime = o.NamePeriodType;
                            }

                            /* total price */
                            o.TotalPrice = o.Quantity * o.Price;
                            o.CusID = OrderDetail.CustomerID;
                            o.sPeriodType = o.PeriodType.ToString();
                            o.ListPeriodType = o.ListPrice.Select(x => new SelectListItem
                            {
                                Value = x.PeriodType.ToString(),
                                Text = PeriodName(x.Period, x.PeriodType)
                            }).OrderBy(x => x.Text).ToList(); //CurrentUser.ListProduct.Where(x => x.ID.Equals(o.ProductID)).Select(x => x.ListPeriodType).FirstOrDefault();
                        });
                    }
                    if (OrderDetail.BillDiscount != null)
                    {
                        OrderDetail.TotalDiscount = OrderDetail.BillDiscount.DiscountValue;
                        OrderDetail.DiscountType = OrderDetail.BillDiscount.DiscountType;
                    }
                    if (OrderDetail.TaxInfo != null)
                    {
                        if (OrderDetail.TaxInfo.TaxType == (byte)Commons.ETaxType.AddOn)
                        {
                            OrderDetail.sTaxName = "(Add on " + OrderDetail.TaxInfo.TaxPercent + "%)";
                        }
                        if (OrderDetail.TaxInfo.TaxType == (byte)Commons.ETaxType.Inclusive)
                        {
                            OrderDetail.sTaxName = "(Inclusive " + OrderDetail.TaxInfo.TaxPercent + "%)";
                        }
                        if (OrderDetail.TaxInfo.TaxType == (byte)Commons.ETaxType.TaxExempt)
                        {
                            OrderDetail.sTaxName = "(Tax Exempt " + OrderDetail.TaxInfo.TaxPercent + "%)";
                        }
                    }
                    var CustomerMerchantInfo = _PfFactory.GetInfoLite(OrderDetail.CustomerID);
                    if (CustomerMerchantInfo != null)
                    {
                        if (CustomerMerchantInfo.CustomerDetail != null)
                        {
                            OrderDetail.CustomerDetail = CustomerMerchantInfo.CustomerDetail;
                        }
                        if (CustomerMerchantInfo.MerchantDetail != null)
                        {
                            OrderDetail.MerchantDetail = CustomerMerchantInfo.MerchantDetail;
                        }

                    }
                }
                var ListPayment = _PMfactory.GetData();
                if (ListPayment != null)
                {
                    ListPayment = ListPayment.Where(x => x.Code == (byte)Commons.EPaymentCode.Parent && x.IsActive).ToList();
                    if (ListPayment != null && ListPayment.Count > 0)
                    {
                        ListPayment.ForEach(x =>
                        {
                            if (x.ImageURL == null || x.ImageURL.Equals(""))
                                x.ImageURL = _ImgDummyPackage;
                            x.ListChild = x.ListChild.Where(z => z.Code == (byte)Commons.EPaymentCode.External && z.IsActive).ToList();
                            if (x.ListChild != null && x.ListChild.Count > 0)
                            {
                                x.ListChild.ForEach(z =>
                                {
                                    if (z.ImageURL == null || z.ImageURL.Equals(""))
                                        z.ImageURL = _ImgDummyPackage;

                                    if (z.Name.ToLower().Equals(Commons.EPayment.PayPal.ToString().ToLower()))
                                    {
                                        z.Payment = (int)Commons.EPayment.PayPal;
                                    }
                                    else if (z.Name.ToLower().Equals(Commons.EPayment.SecureAcceptance.ToString().ToLower()))
                                    {
                                        z.Payment = (int)Commons.EPayment.SecureAcceptance;
                                    }
                                    else if (z.Name.ToLower().Equals(Commons.EPayment.eNETS.ToString().ToLower()))
                                    {
                                        z.Payment = (int)Commons.EPayment.eNETS;
                                    }
                                });

                            }
                        });
                    }
                }
                OrderDetail.LstPayment = ListPayment;
                OrderDetail.IsReseller = IsReseller;
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("GetOrderDetail", ex);
            }
            //if (IsReseller)
           //     OrderDetail.CustomerDetail = new Customermodel();
            var jsonResult = Json(OrderDetail, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        [HttpPost]
        public JsonResult AddToCart(ProductDetailModels item, string OrderId)
        {
            OrderDetailModels OrderDetail = new OrderDetailModels();
            try
            {
                // add to cart
                AddItemToOrderModels modelAddItems = new AddItemToOrderModels();
                modelAddItems.CusID = item.CusID;
                modelAddItems.OrderID = OrderId;
                List<Item> ListItems = new List<Item>();
                Item product = new Item();
                product.ID = "";
                product.ProductID = item.ID;
                product.ProductName = item.Name;
                product.ProductType = item.ProductType;
                product.AdditionType = item.AdditionType;
                product.ImageURL = item.ImageURL;
                product.Period = item.Period;
                product.PeriodType = item.PeriodType;
                product.Quantity = 1;
                product.Price = item.Price;
                product.NumOfStore = item.IncludeStore;
                product.NumOfAccount = item.NumberOfAccount;
                product.AmountOfUnit = item.AmountOfUnit;
                product.ListAdditionApply = new List<Shared.Models.ClientSite.MyProfile.AdditionApply>();
                //=========
                ListItems.Add(product);
                modelAddItems.ListItems = ListItems;
                modelAddItems.IsFree = false;
                string msg = "";
                bool success = false;
                OrderDetail = _factory.AddItems(modelAddItems, ref success, ref msg);
                if (success)
                {
                    ORDERID = OrderDetail.ID;
                    if (OrderDetail.ListItems != null && OrderDetail.ListItems.Count > 0)
                    {
                        OrderDetail.CustomerID = item.CusID;
                        OrderDetail.TotalQuantity = (int)OrderDetail.ListItems.Sum(x => x.Quantity);
                        OrderDetail.ListItems.ForEach(o =>
                        {
                            o.ImageURL = string.IsNullOrEmpty(o.ImageURL) ? ((o.ProductType == (int)Commons.EProductType.Product)
                                                    ? _ImgDummyProduct : _ImgDummyPackage) : o.ImageURL;

                            o.NamePeriodType = PeriodName(o.Period, o.PeriodType);
                            if (o.PeriodType != (byte)Commons.EPeriodType.OneTime)
                            {
                                o.PeriodTime = o.Period + " " + o.NamePeriodType;
                            }
                            else
                            {
                                o.PeriodTime = o.NamePeriodType;
                            }

                            /* total price */
                            o.TotalPrice = o.Quantity * o.Price;
                            o.CusID = OrderDetail.CustomerID;
                            o.sPeriodType = o.PeriodType.ToString();
                            o.ListPeriodType = o.ListPrice.Select(x => new SelectListItem
                            {
                                Value = x.PeriodType.ToString(),
                                Text = PeriodName(x.Period, x.PeriodType)
                            }).OrderBy(x => x.Text).ToList();
                            // o.ListPeriodType = item.ListPeriodType;
                        });
                    }
                    if (OrderDetail.BillDiscount != null)
                    {
                        OrderDetail.TotalDiscount = OrderDetail.BillDiscount.DiscountValue;
                        OrderDetail.DiscountType = OrderDetail.BillDiscount.DiscountType;
                    }
                    if (OrderDetail.TaxInfo != null)
                    {
                        if (OrderDetail.TaxInfo.TaxType == (byte)Commons.ETaxType.AddOn)
                        {
                            OrderDetail.sTaxName = "(Add on " + OrderDetail.TaxInfo.TaxPercent + "%)";
                        }
                        if (OrderDetail.TaxInfo.TaxType == (byte)Commons.ETaxType.Inclusive)
                        {
                            OrderDetail.sTaxName = "(Inclusive " + OrderDetail.TaxInfo.TaxPercent + "%)";
                        }
                        if (OrderDetail.TaxInfo.TaxType == (byte)Commons.ETaxType.TaxExempt)
                        {
                            OrderDetail.sTaxName = "(Tax Exempt " + OrderDetail.TaxInfo.TaxPercent + "%)";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("Add_To_Cart :", ex);
            }
            return Json(OrderDetail, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult RemoveItemOrder(Item item, string OrderId)
        {
            OrderDetailModels OrderDetail = new OrderDetailModels();
            try
            {
                // add to cart
                AddItemToOrderModels modelAddItems = new AddItemToOrderModels();
                modelAddItems.CusID = item.CusID;
                modelAddItems.OrderID = OrderId;
                List<Item> ListItems = new List<Item>();
                Item product = new Item();
                product.ID = item.ID;
                product.ProductID = item.ID;
                product.ProductName = item.ProductName;
                product.ProductType = item.ProductType;
                product.AdditionType = item.AdditionType;
                product.ImageURL = item.ImageURL;
                product.Period = item.Period;
                product.PeriodType = item.PeriodType;
                product.Quantity = 1;
                product.Price = item.Price;
                product.IsDelete = true;
                //product.NumOfStore = firstProduct.NumOfStore;
                //  product.NumOfAccount = item.NumberOfAccount;
                product.AmountOfUnit = item.AmountOfUnit;
                product.ListAdditionApply = new List<Shared.Models.ClientSite.MyProfile.AdditionApply>();
                //=========
                ListItems.Add(product);
                modelAddItems.ListItems = ListItems;
                modelAddItems.IsFree = false;
                string msg = "";
                bool success = false;
                OrderDetail = _factory.AddItems(modelAddItems, ref success, ref msg);
                if (success)
                {
                    ORDERID = OrderDetail.ID;
                    if (OrderDetail.ListItems != null && OrderDetail.ListItems.Count > 0)
                    {
                        OrderDetail.CustomerID = item.CusID;
                        OrderDetail.TotalQuantity = (int)OrderDetail.ListItems.Sum(x => x.Quantity);
                        OrderDetail.ListItems.ForEach(o =>
                        {
                            o.NamePeriodType = PeriodName(o.Period, o.PeriodType);
                            if (o.PeriodType != (byte)Commons.EPeriodType.OneTime)
                            {
                                o.PeriodTime = o.Period + " " + o.NamePeriodType;
                            }
                            else
                            {
                                o.PeriodTime = o.NamePeriodType;
                            }

                            /* total price */
                            o.TotalPrice = o.Quantity * o.Price;
                            o.CusID = OrderDetail.CustomerID;
                            o.sPeriodType = o.PeriodType.ToString();
                            o.ListPeriodType = o.ListPrice.Select(x => new SelectListItem
                            {
                                Value = x.PeriodType.ToString(),
                                Text = PeriodName(x.Period, x.PeriodType)
                            }).OrderBy(x => x.Text).ToList();
                            // o.ListPeriodType = item.ListPeriodType;
                        });
                    }
                    if (OrderDetail.BillDiscount != null)
                    {
                        OrderDetail.TotalDiscount = OrderDetail.BillDiscount.DiscountValue;
                        OrderDetail.DiscountType = OrderDetail.BillDiscount.DiscountType;
                    }
                    if (OrderDetail.TaxInfo != null)
                    {
                        if (OrderDetail.TaxInfo.TaxType == (byte)Commons.ETaxType.AddOn)
                        {
                            OrderDetail.sTaxName = "(Add on " + OrderDetail.TaxInfo.TaxPercent + "%)";
                        }
                        if (OrderDetail.TaxInfo.TaxType == (byte)Commons.ETaxType.Inclusive)
                        {
                            OrderDetail.sTaxName = "(Inclusive " + OrderDetail.TaxInfo.TaxPercent + "%)";
                        }
                        if (OrderDetail.TaxInfo.TaxType == (byte)Commons.ETaxType.TaxExempt)
                        {
                            OrderDetail.sTaxName = "(Tax Exempt " + OrderDetail.TaxInfo.TaxPercent + "%)";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("RemoveItemOrder :", ex);
            }
            return Json(OrderDetail, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetListBuyAddition(string CusID, string ProductID, string OrderID)
        {
            var results = new List<OurProDuctsModel>();
            try
            {
                var data = _OPfactory.GetListData(null, null, (int)Commons.EProductType.Addition, null, null, null, null);
                if (data != null)
                {
                    data.ForEach(x =>
                    {
                        if (x.ListPrice != null && x.ListPrice.Count > 0)
                        {
                            var PriceMin = x.ListPrice.OrderBy(w => w.PeriodType).ThenBy(w => w.Period).ThenBy(w => w.Price).First();
                            x.Price = PriceMin.Price;
                            x.ListPrice.ForEach(o =>
                            {
                                o.NamePeriodType = PeriodName(o.Period, o.PeriodType);
                            });
                            x.sPeriodType = PriceMin.PeriodType.ToString();

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
                        x.OrderID = OrderID;
                        x.CusId = CusID;
                    });
                    var Parents = data.GroupBy(o => o.AdditionType).ToList();
                    Parents.ForEach(o =>
                    {
                        var sAdditionType = "";
                        if (o.Key == (byte)Commons.EAdditionType.Account)
                        {
                            sAdditionType = "Accounts";
                        }
                        if (o.Key == (byte)Commons.EAdditionType.Function)
                        {
                            sAdditionType = "Functions";
                        }
                        if (o.Key == (byte)Commons.EAdditionType.Hardware)
                        {
                            sAdditionType = "Hardwares";
                        }
                        if (o.Key == (byte)Commons.EAdditionType.Location)
                        {
                            sAdditionType = "Locations";
                        }
                        if (o.Key == (byte)Commons.EAdditionType.Service)
                        {
                            sAdditionType = "Services";
                        }
                        if (o.Key == (byte)Commons.EAdditionType.Software)
                        {
                            sAdditionType = "Softwares";
                        }
                        var _our = new OurProDuctsModel
                        {
                            AdditionType = o.Key,
                            sAdditionType = sAdditionType,
                            ListProductChild = data.Where(y => y.AdditionType == o.Key).ToList()
                        };
                        results.Add(_our);
                    });
                    results = results.OrderBy(o => o.AdditionType).ToList();
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("GetListBuyAddition:", ex);
            }
            List<SelectListItem> ListData = new List<SelectListItem>()
                {
                    new SelectListItem() { Text = Commons.EAdditionType.Account.ToString(), Value = Commons.EAdditionType.Account.ToString("d")},
                    new SelectListItem() { Text = Commons.EAdditionType.Function.ToString(), Value = Commons.EAdditionType.Function.ToString("d")},
                    new SelectListItem() { Text = Commons.EAdditionType.Hardware.ToString(), Value = Commons.EAdditionType.Hardware.ToString("d")},
                    new SelectListItem() { Text = Commons.EAdditionType.Location.ToString(), Value = Commons.EAdditionType.Location.ToString("d")},
                    new SelectListItem() { Text = Commons.EAdditionType.Service.ToString(), Value = Commons.EAdditionType.Service.ToString("d")},
                    new SelectListItem() { Text = Commons.EAdditionType.Software.ToString(), Value = Commons.EAdditionType.Software.ToString("d")}
                };
            var obj = new
            {
                result = results,
                Additions = ListData,
            };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddToCartOfAddition(List<OurProDuctsModel> items)
        {
            OrderDetailModels OrderDetail = new OrderDetailModels();
            try
            {
                AddItemToOrderModels modelAddItems = new AddItemToOrderModels();

                List<Item> ListItems = new List<Item>();
                //===============
                var data = new List<OurProDuctsModel>();
                if (items != null && items.Any())
                {
                    items.ForEach(o =>
                    {
                        var Item = o.ListProductChild.Where(c => c.Selected).ToList();
                        data.AddRange(Item);
                    });
                }
                if (data != null && data.Count > 0)
                {
                    var _first = data.FirstOrDefault();
                    modelAddItems.CusID = _first.CusId;
                    modelAddItems.OrderID = _first.OrderID;
                    foreach (var item in data)
                    {
                        Item addition = new Item();
                        addition.ID = "";
                        addition.ProductID = item.ID;
                        addition.ProductName = item.Name;
                        addition.ProductType = item.ProductType;
                        addition.AdditionType = item.AdditionType;
                        addition.ImageURL = item.ImageURL;
                        addition.Period = item.Period;
                        addition.PeriodType = Convert.ToInt32(item.sPeriodType);
                        addition.Quantity = 1;
                        addition.Price = item.Price;
                        addition.AmountOfUnit = item.AmountOfUnit;
                        addition.NumOfAccount = item.NumberOfAccount;
                        addition.NumOfStore = item.NumOfStore;

                        addition.PromotionID = item.PromotionID;
                        addition.PromotionName = item.PromotionName;
                        addition.PromotionAmount = item.PromotionAmount;
                        addition.IsApplyPromotion = item.IsApplyPromotion;

                        addition.DiscountID = item.DiscountID;
                        addition.DiscountName = item.DiscountName;
                        addition.DiscountAmount = item.DiscountAmount;
                        addition.DiscountValue = item.DiscountValue;
                        addition.DiscountType = item.DiscountType;

                        addition.Description = item.Description;
                        addition.ItemGroup = item.ItemGroup;
                        addition.IsDelete = item.IsDelete;

                        addition.ListItem = new List<Item>();
                        addition.ListStoreApply = new List<ApplyStore>();
                        addition.ListFunction = new List<ItemFunction>();

                        ListItems.Add(addition);
                    }
                    modelAddItems.ListItems = ListItems;
                    modelAddItems.IsFree = false;
                    string msg = "";
                    bool success = false;
                    OrderDetail = _factory.AddItems(modelAddItems, ref success, ref msg);
                    if (success)
                    {
                        ORDERID = OrderDetail.ID;
                        if (OrderDetail.ListItems != null && OrderDetail.ListItems.Count > 0)
                        {
                            OrderDetail.CustomerID = _first.CusId;
                            OrderDetail.TotalQuantity = (int)OrderDetail.ListItems.Sum(x => x.Quantity);
                            OrderDetail.ListItems.ForEach(o =>
                            {
                                o.NamePeriodType = PeriodName(o.Period, o.PeriodType);
                                if (o.PeriodType != (byte)Commons.EPeriodType.OneTime)
                                {
                                    o.PeriodTime = o.Period + " " + o.NamePeriodType;
                                }
                                else
                                {
                                    o.PeriodTime = o.NamePeriodType;
                                }

                                /* total price */
                                o.TotalPrice = o.Quantity * o.Price;
                                o.CusID = OrderDetail.CustomerID;
                                o.sPeriodType = o.PeriodType.ToString();
                                o.ListPeriodType = o.ListPrice.Select(x => new SelectListItem
                                {
                                    Value = x.PeriodType.ToString(),
                                    Text = PeriodName(x.Period, x.PeriodType)
                                }).OrderBy(x => x.Text).ToList();
                            });
                        }
                        if (OrderDetail.BillDiscount != null)
                        {
                            OrderDetail.TotalDiscount = OrderDetail.BillDiscount.DiscountValue;
                            OrderDetail.DiscountType = OrderDetail.BillDiscount.DiscountType;
                        }
                        if (OrderDetail.TaxInfo != null)
                        {
                            if (OrderDetail.TaxInfo.TaxType == (byte)Commons.ETaxType.AddOn)
                            {
                                OrderDetail.sTaxName = "(Add on " + OrderDetail.TaxInfo.TaxPercent + "%)";
                            }
                            if (OrderDetail.TaxInfo.TaxType == (byte)Commons.ETaxType.Inclusive)
                            {
                                OrderDetail.sTaxName = "(Inclusive " + OrderDetail.TaxInfo.TaxPercent + "%)";
                            }
                            if (OrderDetail.TaxInfo.TaxType == (byte)Commons.ETaxType.TaxExempt)
                            {
                                OrderDetail.sTaxName = "(Tax Exempt " + OrderDetail.TaxInfo.TaxPercent + "%)";
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("AddToCartOfAddition: ", ex);
            }
            return Json(OrderDetail, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult OrderPayCart(string OrderID)
        {
            string msg = "";
            bool result = _factory.OrderDonePay(OrderID, ref msg);
            if (result)
            {

                ORDERID = "";
                var OrderDetail = _factory.GetOrderDetail(OrderID, true);
                if (OrderDetail != null)
                {
                    if (OrderDetail.ListItems != null && OrderDetail.ListItems.Count > 0)
                    {
                        OrderDetail.CustomerID = CurrentUser.UserId;
                        OrderDetail.TotalQuantity = (int)OrderDetail.ListItems.Sum(x => x.Quantity);
                        OrderDetail.ListItems.ForEach(o =>
                        {
                            o.ProductName = string.IsNullOrEmpty(o.ProductName) ? "" : o.ProductName;
                            o.NamePeriodType = PeriodName(o.Period, o.PeriodType);
                            if (o.PeriodType != (byte)Commons.EPeriodType.OneTime)
                            {
                                o.PeriodTime = o.Period + " " + o.NamePeriodType;
                            }
                            else
                            {
                                o.PeriodTime = o.NamePeriodType;
                            }

                            /* total price */
                            o.TotalPrice = o.Quantity * o.Price;
                            o.CusID = OrderDetail.CustomerID;
                            o.sPeriodType = o.PeriodType.ToString();
                            o.ListPeriodType = o.ListPrice.Select(x => new SelectListItem
                            {
                                Value = x.PeriodType.ToString(),
                                Text = PeriodName(x.Period, x.PeriodType)
                            }).OrderBy(x => x.Text).ToList(); //CurrentUser.ListProduct.Where(x => x.ID.Equals(o.ProductID)).Select(x => x.ListPeriodType).FirstOrDefault();
                        });
                    }
                    if (OrderDetail.BillDiscount != null)
                    {
                        OrderDetail.TotalDiscount = OrderDetail.BillDiscount.DiscountValue;
                        OrderDetail.DiscountType = OrderDetail.BillDiscount.DiscountType;
                    }
                    if (OrderDetail.TaxInfo != null)
                    {
                        if (OrderDetail.TaxInfo.TaxType == (byte)Commons.ETaxType.AddOn)
                        {
                            OrderDetail.sTaxName = "(Add on " + OrderDetail.TaxInfo.TaxPercent + "%)";
                        }
                        if (OrderDetail.TaxInfo.TaxType == (byte)Commons.ETaxType.Inclusive)
                        {
                            OrderDetail.sTaxName = "(Inclusive " + OrderDetail.TaxInfo.TaxPercent + "%)";
                        }
                        if (OrderDetail.TaxInfo.TaxType == (byte)Commons.ETaxType.TaxExempt)
                        {
                            OrderDetail.sTaxName = "(Tax Exempt " + OrderDetail.TaxInfo.TaxPercent + "%)";
                        }
                    }
                }
                var obj = new
                {
                    ID = OrderID,
                    result = OrderDetail
                };
                return Json(obj, JsonRequestBehavior.AllowGet);
            }
            else
            {
                if (!string.IsNullOrEmpty(msg) && msg.Equals("The receipt has been paid in full!"))
                {
                    ORDERID = "";
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, msg);
                }
                return new HttpStatusCodeResult(HttpStatusCode.NotFound, msg);
            }
        }

        [HttpPost]
        public JsonResult UpdatePeriod(Item item, string OrderId)
        {
            OrderDetailModels OrderDetail = new OrderDetailModels();
            try
            {
                // add to cart
                AddItemToOrderModels modelAddItems = new AddItemToOrderModels();
                modelAddItems.CusID = item.CusID;
                modelAddItems.OrderID = OrderId;
                List<Item> ListItems = new List<Item>();
                Item product = new Item();
                product.ID = item.ID;
                product.ProductID = item.ID;
                product.ProductName = item.ProductName;
                product.ProductType = item.ProductType;
                product.AdditionType = item.AdditionType;
                product.ImageURL = item.ImageURL;
                product.Period = item.Period;
                product.PeriodType = Convert.ToInt32(item.sPeriodType);
                product.Quantity = 1;
                product.Price = item.Price;
                //product.NumOfStore = firstProduct.NumOfStore;
                //  product.NumOfAccount = item.NumberOfAccount;
                product.AmountOfUnit = item.AmountOfUnit;
                product.ListAdditionApply = new List<Shared.Models.ClientSite.MyProfile.AdditionApply>();
                //=========
                ListItems.Add(product);
                modelAddItems.ListItems = ListItems;
                modelAddItems.IsFree = false;
                string msg = "";
                bool success = false;
                OrderDetail = _factory.AddItems(modelAddItems, ref success, ref msg);
                if (success)
                {
                    ORDERID = OrderDetail.ID;
                    if (OrderDetail.ListItems != null && OrderDetail.ListItems.Count > 0)
                    {
                        OrderDetail.CustomerID = item.CusID;
                        OrderDetail.TotalQuantity = (int)OrderDetail.ListItems.Sum(x => x.Quantity);
                        OrderDetail.ListItems.ForEach(o =>
                        {
                            o.NamePeriodType = PeriodName(o.Period, o.PeriodType);
                            if (o.PeriodType != (byte)Commons.EPeriodType.OneTime)
                            {
                                o.PeriodTime = o.Period + " " + o.NamePeriodType;
                            }
                            else
                            {
                                o.PeriodTime = o.NamePeriodType;
                            }

                            /* total price */
                            o.TotalPrice = o.Quantity * o.Price;
                            o.CusID = OrderDetail.CustomerID;
                            o.sPeriodType = o.PeriodType.ToString();
                            o.ListPeriodType = o.ListPrice.Select(x => new SelectListItem
                            {
                                Value = x.PeriodType.ToString(),
                                Text = PeriodName(x.Period, x.PeriodType)
                            }).OrderBy(x => x.Text).ToList();
                            // o.ListPeriodType = item.ListPeriodType;
                        });
                    }
                    if (OrderDetail.BillDiscount != null)
                    {
                        OrderDetail.TotalDiscount = OrderDetail.BillDiscount.DiscountValue;
                        OrderDetail.DiscountType = OrderDetail.BillDiscount.DiscountType;
                    }
                    if (OrderDetail.TaxInfo != null)
                    {
                        if (OrderDetail.TaxInfo.TaxType == (byte)Commons.ETaxType.AddOn)
                        {
                            OrderDetail.sTaxName = "(Add on " + OrderDetail.TaxInfo.TaxPercent + "%)";
                        }
                        if (OrderDetail.TaxInfo.TaxType == (byte)Commons.ETaxType.Inclusive)
                        {
                            OrderDetail.sTaxName = "(Inclusive " + OrderDetail.TaxInfo.TaxPercent + "%)";
                        }
                        if (OrderDetail.TaxInfo.TaxType == (byte)Commons.ETaxType.TaxExempt)
                        {
                            OrderDetail.sTaxName = "(Tax Exempt " + OrderDetail.TaxInfo.TaxPercent + "%)";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("UpdatePeriod :", ex);
            }
            return Json(OrderDetail, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ParseJson(string item)
        {
            var data = new OrderDetailModels();
            try
            {
                if (!string.IsNullOrEmpty(item))
                {
                    data = JsonConvert.DeserializeObject<OrderDetailModels>(item);

                    if (data.ListItems != null && data.ListItems.Any())
                    {
                        data.ListItems.ForEach(x =>
                        {
                            x.ImageURL = string.IsNullOrEmpty(x.ImageURL) ? ((x.ProductType == (int)Commons.EProductType.Product)
                                                                ? _ImgDummyProduct : _ImgDummyPackage) : x.ImageURL;
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("ParseJson:", ex);
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ApplyCouponCode(string DiscountCode, string ProductID, string OrderDetailID, string OrderID, string CusID)
        {
            string msg = "";
            bool success = false;
            string OrderId = "";
            if (string.IsNullOrEmpty(ProductID))
                OrderId = OrderID;
            DiscountModels discount = _factory.CouponCode(DiscountCode, OrderId, ProductID, OrderDetailID, ref success, ref msg);
            if (success)
            {
                OrderDetailModels OrderDetail = new OrderDetailModels();
                AddItemToOrderModels modelAddItems = new AddItemToOrderModels();
                modelAddItems.CusID = CusID;
                modelAddItems.OrderID = OrderId;
                List<Item> ListItems = new List<Item>();
                Item product = new Item();
                product.ID = "";
                product.DiscountID = discount.ID;
                product.DiscountType = discount.ValueType;
                product.DiscountValue = discount.Value;
                product.DiscountName = discount.Name;
                ListItems.Add(product);
                modelAddItems.ListItems = ListItems;
                modelAddItems.IsFree = false;
                msg = "";
                success = false;
                OrderDetail = _factory.AddItems(modelAddItems, ref success, ref msg);
                if (success)
                {
                    ORDERID = OrderDetail.ID;
                    if (OrderDetail.ListItems != null && OrderDetail.ListItems.Count > 0)
                    {
                        OrderDetail.CustomerID = CusID;
                        OrderDetail.TotalQuantity = (int)OrderDetail.ListItems.Sum(x => x.Quantity);
                        OrderDetail.ListItems.ForEach(o =>
                        {
                            o.NamePeriodType = PeriodName(o.Period, o.PeriodType);
                            if (o.PeriodType != (byte)Commons.EPeriodType.OneTime)
                            {
                                o.PeriodTime = o.Period + " " + o.NamePeriodType;
                            }
                            else
                            {
                                o.PeriodTime = o.NamePeriodType;
                            }

                            /* total price */
                            o.TotalPrice = o.Quantity * o.Price;
                            o.CusID = OrderDetail.CustomerID;
                            o.sPeriodType = o.PeriodType.ToString();
                            o.ListPeriodType = o.ListPrice.Select(x => new SelectListItem
                            {
                                Value = x.PeriodType.ToString(),
                                Text = PeriodName(x.Period, x.PeriodType)
                            }).OrderBy(x => x.Text).ToList();
                            // o.ListPeriodType = item.ListPeriodType;
                        });
                    }
                    if (OrderDetail.BillDiscount != null)
                    {
                        OrderDetail.TotalDiscount = OrderDetail.BillDiscount.DiscountValue;
                        OrderDetail.DiscountType = OrderDetail.BillDiscount.DiscountType;
                    }
                    if (OrderDetail.TaxInfo != null)
                    {
                        if (OrderDetail.TaxInfo.TaxType == (byte)Commons.ETaxType.AddOn)
                        {
                            OrderDetail.sTaxName = "(Add on " + OrderDetail.TaxInfo.TaxPercent + "%)";
                        }
                        if (OrderDetail.TaxInfo.TaxType == (byte)Commons.ETaxType.Inclusive)
                        {
                            OrderDetail.sTaxName = "(Inclusive " + OrderDetail.TaxInfo.TaxPercent + "%)";
                        }
                        if (OrderDetail.TaxInfo.TaxType == (byte)Commons.ETaxType.TaxExempt)
                        {
                            OrderDetail.sTaxName = "(Tax Exempt " + OrderDetail.TaxInfo.TaxPercent + "%)";
                        }
                    }
                }
                return Json(OrderDetail, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var obj = new
                {
                    msg = msg
                };
                return Json(obj, JsonRequestBehavior.AllowGet);
            }

        }
    }
}