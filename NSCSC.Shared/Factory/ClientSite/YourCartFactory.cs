using Newtonsoft.Json;
using NSCSC.Shared.Models;
using NSCSC.Shared.Models.ClientSite;
using NSCSC.Shared.Models.ClientSite.MyProfile;
using NSCSC.Shared.Models.ClientSite.MyStoreAndBusiness;
using NSCSC.Shared.Models.ClientSite.YourCart;
using NSCSC.Shared.Utilities;
using System;

namespace NSCSC.Shared.Factory.ClientSite
{
    public class YourCartFactory : BaseFactory
    {
        private BaseFactory _baseFactory = null;

        public YourCartFactory()
        {
            _baseFactory = new BaseFactory();
        }

        public OrderDetailModels AddItems(AddItemToOrderModels model, ref bool success, ref string msg)
        {
            OrderDetailModels item = new OrderDetailModels();
            try
            {
                YourCartRequest paraBody = new YourCartRequest();
                paraBody.CreatedUser = model.CusID;
                paraBody.CusID = model.CusID;
                paraBody.OrderID = model.OrderID;
                paraBody.ListItems = model.ListItems;
                paraBody.Note = model.Note;
               // paraBody.isFree = model.IsFree;
                paraBody.IsFree = model.IsFree;
                NSLog.Logger.Info("YourCart | AddItems Request: ", paraBody);

                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.ClientSiteYourCartAddItems, null, paraBody);
                dynamic data = result.Data;
                success = result.Success;
                msg = result.Message;

                var lstDataRaw = data["OrderDetail"];
                var lstObject = JsonConvert.SerializeObject(lstDataRaw);
                item = JsonConvert.DeserializeObject<OrderDetailModels>(lstObject);
                if (item == null)
                    item = new OrderDetailModels();
                NSLog.Logger.Info("YourCart | AddItems", item);

                return item;
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("YourCart | AddItems_Fail", e);
                return item;
            }
        }

        public DiscountModels CouponCode(string DiscountCode, string OrderID, string ProductID, string OrderDetailID, ref bool success, ref string msg)
        {
            DiscountModels item = new DiscountModels();
            try
            {
                YourCartRequest paraBody = new YourCartRequest();
                paraBody.DiscountCode = DiscountCode;
                paraBody.OrderID = OrderID;
                paraBody.OrderDetailID = OrderDetailID;
                paraBody.ID = ProductID;
                NSLog.Logger.Info("CouponCode Request: ", paraBody);

                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.ClientSiteYourCartCouponCode, null, paraBody);
                NSLog.Logger.Info("CouponCode Result: ", result);
                dynamic data = result.Data;

                success = result.Success;
                msg = result.Message;

                var lstDataRaw = data["Discount"];
                var lstObject = JsonConvert.SerializeObject(lstDataRaw);
                item = JsonConvert.DeserializeObject<DiscountModels>(lstObject);
                if (item == null)
                    item = new DiscountModels();
                NSLog.Logger.Info("CouponCode", item);

                return item;
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("CouponCode_Fail", e);
                return item;
            }
        }

        public OrderDetailModels GetOrderDetail(string OrderID, bool? isCheckvalid = null)
        {
            OrderDetailModels item = new OrderDetailModels();
            try
            {
                YourCartRequest paraBody = new YourCartRequest();
                paraBody.ID = OrderID;
                paraBody.isCheckvalid = isCheckvalid;

                NSLog.Logger.Info("YourCart | GetOrderDetail Request: ", paraBody);

                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.ClientSiteYourCartGetOrderDetail, null, paraBody);
                if (result.Success && result.Message != null && result.Message.Equals("The receipt has been paid in full!"))
                {
                    item.IsPad = true;
                }

                dynamic data = result.Data;

                var lstDataRaw = data["OrderDetail"];
                var lstObject = JsonConvert.SerializeObject(lstDataRaw);
                item = JsonConvert.DeserializeObject<OrderDetailModels>(lstObject);
                if (item == null)
                    item = new OrderDetailModels();
                NSLog.Logger.Info("YourCart | GetOrderDetail", item);

                return item;
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("YourCart | GetOrderDetail_Fail", e);
                return item;
            }
        }

        public bool CreateStoreInfoTemp(StoreDetailModels model, string CustomerID, ref string msg, ref string StoreIDReturn)
        {
            try
            {
                MyStoreAndBusinessRequest paraBody = new MyStoreAndBusinessRequest();
                paraBody.StoreInfo = model;
                paraBody.AssetID = model.AssetID;
                paraBody.ListAssetID = model.ListAssetID;
                paraBody.ID = CustomerID;

                NSLog.Logger.Info("CreateStoreInfoTemp Request: ", paraBody);

                //====================
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.ClientSiteInputStoreCreateStoreTemp, null, paraBody);
                NSLog.Logger.Info("CreateStoreInfoTemp Result: ", result);
                if (result != null)
                {
                    if (result.Success)
                    {
                        dynamic data = result.Data;
                        StoreIDReturn = data["ID"];
                        return true;
                    }
                    else
                    {
                        msg = result.Message;
                        NSLog.Logger.Info("CreateStoreInfoTemp", result.Message);
                        return false;
                    }
                }
                else
                {
                    NSLog.Logger.Info("CreateStoreInfoTemp", result);
                    return false;
                }
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("CreateStoreInfoTemp_Fail: ", e);
                return false;
            }
        }

        //Order
        public bool OrderPay(string OrderID, string PaymentMethodID, double AmountPay, string CreatedUser, string TransactionId, string note, ref string OrderIDReturn, ref string msg)
        {
            try
            {
                YourCartRequest paraBody = new YourCartRequest();
                paraBody.OrderID = OrderID;
                paraBody.AmountPay = AmountPay;
                paraBody.CreatedUser = CreatedUser;
                paraBody.PaymentMethodID = PaymentMethodID;
                paraBody.TransactionID = TransactionId;
                paraBody.Note = note;
                if (AmountPay == 0)
                    paraBody.IsZero = true;
                NSLog.Logger.Info("OrderPayRequest", paraBody);
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.ClientSiteYourCartOrderPay, null, paraBody);
                NSLog.Logger.Info("OrderPayResult", result);
                if (result != null)
                {
                    if (result.Success)
                    {
                        dynamic data = result.Data;
                        OrderIDReturn = data["ID"];
                        return true;
                    }
                    else
                    {
                        msg = result.Message;
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("OrderPay_Fail", e);
                return false;
            }
        }

        public bool OrderCancelPay(string ID, ref string msg)
        {
            try
            {
                YourCartRequest paraBody = new YourCartRequest();
                paraBody.ID = ID;
                NSLog.Logger.Info("OrderCancelPayRequest", paraBody);
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.ClientSiteYourCartOrderCancelPay, null, paraBody);
                NSLog.Logger.Info("OrderCancelPayResult", result);
                if (result != null)
                {
                    if (result.Success)
                        return true;
                    else
                    {
                        msg = result.Message;
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("OrderCancelPay_Fail", e);
                return false;
            }
        }

        public bool OrderDonePay(string ID, ref string msg)
        {
            try
            {
                YourCartRequest paraBody = new YourCartRequest();
                paraBody.ID = ID;
                NSLog.Logger.Info("OrderDonePayRequest", paraBody);
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.ClientSiteYourCartOrderDonePay, null, paraBody);
                NSLog.Logger.Info("OrderDonePayResult", result);
                if (result != null)
                {
                    if (result.Success)
                        return true;
                    else
                    {
                        msg = result.Message;
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("OrderDonePay_Fail", e);
                return false;
            }
        }

        public bool OrderMerge(string OrderIDFrom, string OrderIDTo, ref string msg)
        {
            try
            {
                MergeOrderRequest paraBody = new MergeOrderRequest();
                paraBody.OrderIDFrom = OrderIDFrom;
                paraBody.OrderIDTo = OrderIDTo;
                NSLog.Logger.Info("OrderMerge Request", paraBody);
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.CRMCustomerOrderMerge, null, paraBody);
                NSLog.Logger.Info("OrderMerge Response", result);
                if (result != null)
                {
                    if (result.Success)
                        return true;
                    else
                    {
                        msg = result.Message;
                        NSLog.Logger.Info("OrderMerge", result.Message);
                        return false;
                    }
                }
                else
                {
                    NSLog.Logger.Info("OrderMerge", result);
                    return false;
                }
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("OrderMerge_Fail", e);
                return false;
            }
        }
        public bool DeleteOrder(string Id, string useId, ref string msg)
        {
            try
            {
                YourCartRequest paraBody = new YourCartRequest();
                paraBody.ID = Id;
                paraBody.CreatedUser = useId;
                NSLog.Logger.Info("OrderDelete Request", paraBody);
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.ClientSiteYourCartDeleteOrder, null, paraBody);
                NSLog.Logger.Info("OrderDelete Response", result);
                if (result != null)
                {
                    if (result.Success)
                        return true;
                    else
                    {
                        msg = result.Message;
                        NSLog.Logger.Info("OrderDelete", result.Message);
                        return false;
                    }
                }
                else
                {
                    NSLog.Logger.Info("OrderDelete", result);
                    return false;
                }
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("OrderDelete_Fail", e);
                return false;
            }
        }
        public bool ResellerRegisCustomer(AccountRegisterRequest paraBody, ref string CustomerIDreturn, ref string MerchantIDreturn, ref string msg) {
            try
            {
                paraBody.CustomerDetail.Password = CommonHelper.GetSHA512(paraBody.CustomerDetail.Password);
                paraBody.CustomerDetail.ID = string.IsNullOrEmpty(paraBody.CustomerID) ? null : paraBody.CustomerID;
                paraBody.Merchant.ID = string.IsNullOrEmpty(paraBody.MerchantID) ? null : paraBody.MerchantID;

                NSLog.Logger.Info("ResellerRegisCustomer Request", paraBody);
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.ClientSideToRegister, null, paraBody);
                NSLog.Logger.Info("ResellerRegisCustomer Response", result);
                if (result != null)
                {
                    if (result.Success)
                    {
                        dynamic data = result.Data;
                        CustomerIDreturn = data["CustomerID"];
                        MerchantIDreturn = data["MerchantID"];
                        return true;
                    }
                       
                    else
                    {
                        msg = result.Message;
                        NSLog.Logger.Info("OrderDelete", result.Message);
                        return false;
                    }
                }
                else
                {
                    NSLog.Logger.Info("OrderDelete", result);
                    return false;
                }
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("OrderDelete_Fail", e);
                return false;
            }
        }
    }
}
