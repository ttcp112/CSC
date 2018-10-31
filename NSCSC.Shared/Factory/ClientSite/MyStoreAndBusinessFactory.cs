using Newtonsoft.Json;
using NSCSC.Shared.Models;
using NSCSC.Shared.Models.ClientSite.MyStoreAndBusiness;
using NSCSC.Shared.Utilities;
using System;
using System.Collections.Generic;

namespace NSCSC.Shared.Factory.ClientSite
{
    public class MyStoreAndBusinessFactory : BaseFactory
    {
        private BaseFactory _baseFactory = null;

        public MyStoreAndBusinessFactory()
        {
            _baseFactory = new BaseFactory();
        }

        #region For Merchant

        public List<MerchantModels> GetListMerchant(string CustomerID)
        {
            List<MerchantModels> listItem = new List<MerchantModels>();
            try
            {
                MyStoreAndBusinessRequest paraBody = new MyStoreAndBusinessRequest();
                paraBody.ID = CustomerID;
                NSLog.Logger.Info("GetListMerchant_Request: ", paraBody);

                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.ClientSiteStoreBusinessMerchantGetList, null, paraBody);
                dynamic data = result.Data;
                var lstDataRaw = data["ListMerchant"];
                var lstObject = JsonConvert.SerializeObject(lstDataRaw);
                listItem = JsonConvert.DeserializeObject<List<MerchantModels>>(lstObject);
                if (listItem == null)
                    listItem = new List<MerchantModels>();
                NSLog.Logger.Info("GetListMerchant_Response: ", listItem);

                return listItem;
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("GetListMerchant_Fail: ", e);
                return listItem;
            }
        }

        public MerchantModels GetMerchantInfo(string CustomerID)
        {
            MerchantModels item = new MerchantModels();
            try
            {
                MyStoreAndBusinessRequest paraBody = new MyStoreAndBusinessRequest();
                paraBody.ID = CustomerID;
                NSLog.Logger.Info("GetMerchantInfo Request: ", paraBody);

                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.ClientSiteStoreBusinessMerchantGetInfo, null, paraBody);
                dynamic data = result.Data;
                var lstDataRaw = data["MerchantInfo"];
                var lstObject = JsonConvert.SerializeObject(lstDataRaw);
                item = JsonConvert.DeserializeObject<MerchantModels>(lstObject);
                if (item == null)
                    item = new MerchantModels();
                NSLog.Logger.Info("GetMerchantInfo", item);

                return item;
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("GetMerchantInfo_Fail", e);
                return item;
            }
        }

        public bool UpdateMerchantInfo(MerchantModels model, string CustomerID, ref string msg)
        {
            try
            {
                MyStoreAndBusinessRequest paraBody = new MyStoreAndBusinessRequest();
                paraBody.MerchantInfo = model;
                paraBody.CustomerID = CustomerID;
                NSLog.Logger.Info("UpdateMerchantInfo Request: ", paraBody);

                //====================
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.ClientSiteStoreBusinessMerchantUpdateInfo, null, paraBody);
                if (result != null)
                {
                    if (result.Success)
                        return true;
                    else
                    {
                        msg = result.Message;
                        NSLog.Logger.Info("UpdateMerchantInfo", result.Message);
                        return false;
                    }
                }
                else
                {
                    NSLog.Logger.Info("UpdateMerchantInfo", result);
                    return false;
                }
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("UpdateMerchantInfo_Fail", e);
                return false;
            }
        }
        #endregion For Merchant

        #region For Store

        public List<StoreModels> GetListStore(string CustomerID)
        {
            List<StoreModels> lstData = new List<StoreModels>();
            try
            {
                MyStoreAndBusinessRequest paraBody = new MyStoreAndBusinessRequest();
                paraBody.ID = CustomerID;
                NSLog.Logger.Info("GetListStore Request: ", paraBody);

                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.ClientSiteStoreBusinessStoreGetList, null, paraBody);
                dynamic data = result.Data;
                var lstDataRaw = data["ListStore"];
                var lstObject = JsonConvert.SerializeObject(lstDataRaw);
                lstData = JsonConvert.DeserializeObject<List<StoreModels>>(lstObject);
                NSLog.Logger.Info("GetListStore", lstData);

                return lstData;
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("GetListStore_Fail", e);
                return lstData;
            }
        }

        public StoreDetailModels GetStoreInfo(string StoreId)
        {
            StoreDetailModels item = new StoreDetailModels();
            try
            {
                MyStoreAndBusinessRequest paraBody = new MyStoreAndBusinessRequest();
                paraBody.ID = StoreId;

                NSLog.Logger.Info("GetStoreInfo Request: ", paraBody);

                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.ClientSiteStoreBusinessStoreGetInfo, null, paraBody);
                dynamic data = result.Data;
                var lstDataRaw = data["StoreInfo"];
                var lstObject = JsonConvert.SerializeObject(lstDataRaw);
                item = JsonConvert.DeserializeObject<StoreDetailModels>(lstObject);
                NSLog.Logger.Info("GetStoreInfo", item);

                return item;
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("GetStoreInfo_Fail", e);
                return item;
            }
        }
        public bool StoreAssignProduct(List<StoreAssignProduct> item, string ID, bool IsStoreAssignProduct, string CreatedUser, ref string msg)
        {
            try
            {
                StoreAssignProductRequest paraBody = new StoreAssignProductRequest();
                paraBody.IsAssignProductToStore = IsStoreAssignProduct;
                paraBody.CreatedUser = CreatedUser;
                if (item.Count > 0)
                    paraBody.ListStoreAssignProduct = item;
                else
                    paraBody.ID = ID;
                NSLog.Logger.Info("StoreAssignProduct Request: ", paraBody);
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.ClientSiteStoreBusinessStoreAssignProduct, null, paraBody);
                if (result != null)
                {
                    if (result.Success)
                        return true;
                    else
                    {
                        msg = result.Message;
                        NSLog.Logger.Info("StoreAssignProduct", result.Message);
                        return false;
                    }
                }
                else
                {
                    NSLog.Logger.Info("StoreAssignProduct", result);
                    return false;
                }
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("StoreAssignProduct", e);
                return false;
            }
        }

        public bool UpdateStoreInfo(StoreDetailModels model, string CustomerID, ref string msg)
        {
            try
            {
                MyStoreAndBusinessRequest paraBody = new MyStoreAndBusinessRequest();
                paraBody.StoreInfo = model;
                paraBody.CustomerID = CustomerID;

                NSLog.Logger.Info("UpdateStoreInfo Request: ", paraBody);

                //====================
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.ClientSiteStoreBusinessStoreUpdateInfo, null, paraBody);
                if (result != null)
                {
                    if (result.Success)
                        return true;
                    else
                    {
                        msg = result.Message;
                        NSLog.Logger.Info("UpdateStoreInfo", result.Message);
                        return false;
                    }
                }
                else
                {
                    NSLog.Logger.Info("UpdateStoreInfo", result);
                    return false;
                }
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("UpdateStoreInfo", e);
                return false;
            }
        }

        public bool CreateStoreInfo(StoreDetailModels model, string CustomerID, ref string StoreIDReturn, ref string msg)
        {
            try
            {
                MyStoreAndBusinessRequest paraBody = new MyStoreAndBusinessRequest();
                paraBody.StoreInfo = model;
                //paraBody.AssetID = model.AssetID;
                paraBody.ListAssetID = model.ListAssetID;
                paraBody.ID = CustomerID;

                NSLog.Logger.Info("CreateStoreInfo Request: ", paraBody);

                //====================
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.ClientSiteStoreBusinessStoreCreate, null, paraBody);
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
                        NSLog.Logger.Info("CreateStoreInfo", result.Message);
                        return false;
                    }
                }
                else
                {
                    NSLog.Logger.Info("CreateStoreInfo", result);
                    return false;
                }
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("CreateStoreInfo", e);
                return false;
            }
        }

        public List<ProductApplyStoreModels> GetListProductApplyStore(string CustomerID)
        {
            List<ProductApplyStoreModels> lstData = new List<ProductApplyStoreModels>();
            try
            {
                MyStoreAndBusinessRequest paraBody = new MyStoreAndBusinessRequest();
                paraBody.ID = CustomerID;

                NSLog.Logger.Info("GetListProductApplyStore Request: ", paraBody);

                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.ClientSiteStoreBusinessGetListProductApplyStore, null, paraBody);
                dynamic data = result.Data;
                var lstDataRaw = data["ListProduct"];
                var lstObject = JsonConvert.SerializeObject(lstDataRaw);
                lstData = JsonConvert.DeserializeObject<List<ProductApplyStoreModels>>(lstObject);
                NSLog.Logger.Info("GetListProductApplyStore", lstData);

                return lstData;
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("GetListProductApplyStore_Fail", e);
                return lstData;
            }
        }
        public List<ProductApplyStoreModels> GetListProductForStore(string CustomerID, string StoreID)
        {
            List<ProductApplyStoreModels> lstData = new List<ProductApplyStoreModels>();
            try
            {
                MyStoreAndBusinessRequest paraBody = new MyStoreAndBusinessRequest();
                paraBody.ID = CustomerID;
                paraBody.StoreID = StoreID;

                NSLog.Logger.Info("GetListProductApplyStore Request: ", paraBody);

                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.ClientSiteStoreBusinessGetListProductApplyStore, null, paraBody);
                dynamic data = result.Data;
                var lstDataRaw = data["ListProduct"];
                var lstObject = JsonConvert.SerializeObject(lstDataRaw);
                lstData = JsonConvert.DeserializeObject<List<ProductApplyStoreModels>>(lstObject);
                NSLog.Logger.Info("GetListProductApplyStore", lstData);

                return lstData;
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("GetListProductApplyStore_Fail", e);
                return lstData;
            }
        }

        public bool ChangeStatusDevice(string DeviceID, ref string msg)
        {
            try
            {
                MyStoreAndBusinessRequest paraBody = new MyStoreAndBusinessRequest();
                paraBody.ID = DeviceID;
                NSLog.Logger.Info("ChangeStatusDevice Request", paraBody);
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.ClientSiteStoreBusinessChangeStatusDevice, null, paraBody);
                if (result != null)
                {
                    if (result.Success)
                        return true;
                    else
                    {
                        msg = result.Message;
                        NSLog.Logger.Info("ChangeStatusDevice", result.Message);
                        return false;
                    }
                }
                else
                {
                    NSLog.Logger.Info("ChangeStatusDevice", result);
                    return false;
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("ChangeStatusDevice_Error", ex);
                return false;
            }
        }



        public ProductPackageUserModels GetProductPackageDetail(string AssetID)
        {
            ProductPackageUserModels item = new ProductPackageUserModels();
            try
            {
                MyStoreAndBusinessRequest paraBody = new MyStoreAndBusinessRequest();
                paraBody.ID = AssetID;
                NSLog.Logger.Info("GetProductPackageDetail Request: ", paraBody);

                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.ClientSiteStoreBusinessGetProductPackageDetail, null, paraBody);
                dynamic data = result.Data;

                var lstDataRaw = data["ProductPackage"];
                var lstObject = JsonConvert.SerializeObject(lstDataRaw);
                item = JsonConvert.DeserializeObject<ProductPackageUserModels>(lstObject);

                // Convert local time
                if (item != null)
                {
                    item.ActiveTime = item.ActiveTime.HasValue ? CommonHelper.ConvertToLocalTime(item.ActiveTime.Value) : item.ActiveTime;
                    item.ExpiryDate = item.ExpiryDate.HasValue ? CommonHelper.ConvertToLocalTime(item.ExpiryDate.Value) : item.ExpiryDate;
                    if (item.ListProduct != null && item.ListProduct.Count > 0)
                    {
                        item.ListProduct.ForEach(product =>
                        {
                            product.ActiveTime = product.ActiveTime.HasValue ? CommonHelper.ConvertToLocalTime(product.ActiveTime.Value) : product.ActiveTime;
                            product.ExpiryDate = product.ExpiryDate.HasValue ? CommonHelper.ConvertToLocalTime(product.ExpiryDate.Value) : product.ExpiryDate;
                        });
                    }
                    if (item.ListDevice != null && item.ListDevice.Count > 0)
                    {
                        item.ListDevice.ForEach(device =>
                        {
                            device.ActiveTime = device.ActiveTime.HasValue ? CommonHelper.ConvertToLocalTime(device.ActiveTime.Value) : device.ActiveTime;
                        });
                    }
                }
                return item;
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("GetProductPackageDetail_Fail", ex);
                return item;
            }
        }

        public List<ProductPriceModels> GetPriceProduct(string ProductID)
        {
            var models = new List<ProductPriceModels>();
            try
            {
                MyStoreAndBusinessRequest paraBody = new MyStoreAndBusinessRequest();
                paraBody.ID = ProductID;
                NSLog.Logger.Info("GetPriceProduct Request: ", paraBody);

                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.ClientSiteStoreBusinessPriceList, null, paraBody);
                NSLog.Logger.Info("GetPriceProduct Response : ", result);
                dynamic data = result.Data;

                var lstDataRaw = data["ListPrice"];
                var lstObject = JsonConvert.SerializeObject(lstDataRaw);
                models = JsonConvert.DeserializeObject<List<ProductPriceModels>>(lstObject);
                return models;
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("GetPriceProduct ", ex);
                return models;
            }
        }

        #endregion For Store

        public UserGetListProductManagementModels GetListHardwareServiceAccountAddition(string CustomerID)
        {
            UserGetListProductManagementModels item = new UserGetListProductManagementModels();

            List<HardwareModels> ListHardware = new List<HardwareModels>();
            List<ServiceModels> ListService = new List<ServiceModels>();
            List<AccountManagementModels> ListAccount = new List<AccountManagementModels>();
            List<AdditionFunctionModels> ListFunction = new List<AdditionFunctionModels>();
            List<ProductPackageUserModels> ListPackage = new List<ProductPackageUserModels>();
            try
            {
                MyStoreAndBusinessRequest paraBody = new MyStoreAndBusinessRequest();
                paraBody.CustomerID = CustomerID;
                NSLog.Logger.Info("GetListHardwareServiceAccountAddition Request: ", paraBody);

                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.ClientSiteStoreBusinessHardwareServiceAccountAddition, null, paraBody);
                dynamic data = result.Data;

                var lstDataRaw = data["ListHardware"];
                var lstObject = JsonConvert.SerializeObject(lstDataRaw);
                ListHardware = JsonConvert.DeserializeObject<List<HardwareModels>>(lstObject);

                lstDataRaw = data["ListService"];
                lstObject = JsonConvert.SerializeObject(lstDataRaw);
                ListService = JsonConvert.DeserializeObject<List<ServiceModels>>(lstObject);

                lstDataRaw = data["ListAccount"];
                lstObject = JsonConvert.SerializeObject(lstDataRaw);
                ListAccount = JsonConvert.DeserializeObject<List<AccountManagementModels>>(lstObject);

                lstDataRaw = data["ListFunction"];
                lstObject = JsonConvert.SerializeObject(lstDataRaw);
                ListFunction = JsonConvert.DeserializeObject<List<AdditionFunctionModels>>(lstObject);

                lstDataRaw = data["ListProductPackage"];
                lstObject = JsonConvert.SerializeObject(lstDataRaw);
                ListPackage = JsonConvert.DeserializeObject<List<ProductPackageUserModels>>(lstObject);

                NSLog.Logger.Info("GetListHardwareServiceAccountAddition", item);
                item.ListHardware = ListHardware;
                item.ListService = ListService;
                item.ListAccount = ListAccount;
                item.ListFunction = ListFunction;
                item.ListProductPackage = ListPackage;
                return item;
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("GetListHardwareServiceAccountAddition_Fail", e);
                return item;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type">function|account</param>
        /// <param name="ID"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool ChangeStatusFunctionAccount(string type, string ID, ref string msg)
        {
            try
            {
                MyStoreAndBusinessRequest paraBody = new MyStoreAndBusinessRequest();
                paraBody.ID = ID;
                NSLog.Logger.Info("ChangeStatus" + type + " Request", paraBody);
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(
                    type.Equals("function") ? Commons.ClientSiteStoreBusinessChangeStatusFunction : Commons.ClientSiteStoreBusinessChangeStatusAccount,
                    null, paraBody);
                if (result != null)
                {
                    if (result.Success)
                        return true;
                    else
                    {
                        msg = result.Message;
                        NSLog.Logger.Info("ChangeStatus" + type, result.Message);
                        return false;
                    }
                }
                else
                {
                    NSLog.Logger.Info("ChangeStatus" + type, result);
                    return false;
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("ChangeStatus" + type + "_Error", ex);
                return false;
            }
        }
    }
}
