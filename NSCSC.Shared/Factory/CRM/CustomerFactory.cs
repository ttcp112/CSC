using Newtonsoft.Json;
using NSCSC.Shared.Models;
using NSCSC.Shared.Models.ClientSite.YourCart;
//using NSCSC.Shared.Models.ClientSite.MyStoreAndBusiness;
using NSCSC.Shared.Models.CRM.Customers;
using NSCSC.Shared.Models.CRM.Customers.ProductManagement;
using NSCSC.Shared.Models.CRM.OrderReceiptManagement;
using NSCSC.Shared.Utilities;
using System;
using System.Collections.Generic;

namespace NSCSC.Shared.Factory.CRM
{
    public class CustomerFactory
    {
        private BaseFactory _baseFactory = null;

        public CustomerFactory()
        {
            _baseFactory = new BaseFactory();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SearchString"></param>
        /// <returns></returns>
        public List<CustomerModels> GetListData(string SearchString = null)
        {
            List<CustomerModels> listData = new List<CustomerModels>();
            try
            {
                CustomerRequest paraBody = new CustomerRequest();
                paraBody.SearchString = SearchString;

                NSLog.Logger.Info("ListCustomer Request: ", paraBody);

                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.customerAPIGetList, null, paraBody);
                dynamic data = result.Data;
                var lstDataRaw = data["ListCustomer"];
                var lstObject = JsonConvert.SerializeObject(lstDataRaw);
                listData = JsonConvert.DeserializeObject<List<CustomerModels>>(lstObject);

                NSLog.Logger.Info("CustomerGetListData", listData);

                return listData;
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("CustomerGetListData_Fail", e);
                return listData;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public CustomerDetailModels GetDetail(string ID)
        {
            CustomerDetailModels item = new CustomerDetailModels();
            MerchantModels item2 = new MerchantModels();
            try
            {
                CustomerRequest paraBody = new CustomerRequest();
                paraBody.ID = ID;

                NSLog.Logger.Info("CustomerDetail Request: ", paraBody); 

                 var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.customerAPIGetDetail, null, paraBody);
                dynamic data = result.Data;
                var lstDataRaw = data["CustomerDetail"];
                var lstDataRaw2 = data["MerchantDetail"];
                var lstObject = JsonConvert.SerializeObject(lstDataRaw);
                var lstObject2 = JsonConvert.SerializeObject(lstDataRaw2);
                item = JsonConvert.DeserializeObject<CustomerDetailModels>(lstObject);
                item2 = JsonConvert.DeserializeObject<MerchantModels>(lstObject2);
                if (item != null)
                {
                    item.Birthday = CommonHelper.ConvertToLocalTime(item.Birthday);
                    item.Anniversary = CommonHelper.ConvertToLocalTime(item.Anniversary);
                    item.JoinedDate = CommonHelper.ConvertToLocalTime(item.JoinedDate);
                }
                if (item2 != null)
                {
                    item.MerchantName = item2.Name;
                }
                lstDataRaw = data["ListReceipt"];
                lstObject = JsonConvert.SerializeObject(lstDataRaw);
                var ListReceipt = JsonConvert.DeserializeObject<List<OrderModels>>(lstObject);
                
                item.ListReceipt = ListReceipt;
                if(item.ListReceipt != null)
                {
                    item.ListReceipt.ForEach(x =>
                    {
                        x.CreatedDate = CommonHelper.ConvertToLocalTime(x.CreatedDate);
                        x.PaidTime = CommonHelper.ConvertToLocalTime(x.PaidTime);
                    });
                }
                NSLog.Logger.Info("CustomerGetDetail", item);

                return item;
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("CustomerGetDetail_Fail", e);
                return item;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool CreateOrEdit(CustomerDetailModels model, ref string msg)
        {
            try
            {
                CustomerRequest paraBody = new CustomerRequest();
                model.Birthday = CommonHelper.ConvertToUTCTime(model.Birthday);
                model.Anniversary = CommonHelper.ConvertToUTCTime(model.Anniversary);
                model.JoinedDate = CommonHelper.ConvertToUTCTime(model.JoinedDate);
                paraBody.CustomerDetail = model;
                paraBody.CreatedUser = model.CreateUser;

                NSLog.Logger.Info("CustomerCreateOrEdit Request: ", paraBody);

                //====================
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.customerAPICreateOrEdit, null, paraBody);
                if (result != null)
                {
                    if (result.Success)
                        return true;
                    else
                    {
                        msg = result.Message;
                        NSLog.Logger.Info("CustomerCreateOrEdit", result.Message);
                        return false;
                    }
                }
                else
                {
                    NSLog.Logger.Info("CustomerCreateOrEdit", result);
                    return false;
                }
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("CustomerCreateOrEdit_Fail", e);
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="CreateUser"></param>
        /// <param name="msg"></param>
        /// <param name="Reason"></param>
        /// <returns></returns>
        public bool Delete(string ID, string CreateUser, ref string msg, string Reason = "")
        {
            try
            {
                CustomerRequest paraBody = new CustomerRequest();
                paraBody.CreatedUser = CreateUser;
                paraBody.ID = ID;
                paraBody.ReasonDelete = Reason;

                NSLog.Logger.Info("CustomerDelete Request: ", paraBody);
                //====================
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.customerAPIDelete, null, paraBody);
                if (result != null)
                {
                    if (result.Success)
                        return true;
                    else
                    {
                        msg = result.Message;
                        NSLog.Logger.Info("CustomerDelete", result.Message);
                        return false;
                    }
                }
                else
                {
                    NSLog.Logger.Info("CustomerDelete", result);
                    return false;
                }
            }
            catch (Exception e)
            {
                msg = e.ToString();
                NSLog.Logger.Error("CustomerDelete_Fail", e);
                return false;
            }
        }

        public bool AssetAddDevice(string ID, string CreateUser, string UUID, ref string msg)
        {
            try
            {
                CustomerRequest paraBody = new CustomerRequest();
                paraBody.CreatedUser = CreateUser;
                paraBody.ID = ID;
                paraBody.UUID = UUID;

                NSLog.Logger.Info("AssetAddDevice Request: ", paraBody);
                //====================
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.customerAPIAssetAddDevice, null, paraBody);
                if (result != null)
                {
                    if (result.Success)
                        return true;
                    else
                    {
                        msg = result.Message;
                        NSLog.Logger.Info("AssetAddDevice", result.Message);
                        return false;
                    }
                }
                else
                {
                    NSLog.Logger.Info("AssetAddDevice", result);
                    return false;
                }
            }
            catch (Exception e)
            {
                msg = e.ToString();
                NSLog.Logger.Error("AssetAddDevice_Fail", e);
                return false;
            }
        }

        #region "Merchant & Store"
        public bool CreateOrEditMerchantStore(MerchantModels model, ref string msg)
        {
            try
            {
                CustomerRequest paraBody = new CustomerRequest();
                paraBody.Merchant = model;
                paraBody.CreatedUser = model.CreatedUser;
                //====================
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.customerAPIMerchantCreateOrEdit, null, paraBody);
                if (result != null)
                {
                    if (result.Success)
                        return true;
                    else
                    {
                        msg = result.Message;
                        NSLog.Logger.Info("CreateOrEditMerchantStore", result.Message);
                        return false;
                    }
                }
                else
                {
                    NSLog.Logger.Info("CreateOrEditMerchantStore", result);
                    return false;
                }

            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("CustomerFactory - CreateOrEditMerchantStore - :", ex);
                return false;
            }
        }

        public MerchantModels GetDetailMerchant(string Id)
        {
            MerchantModels model = new MerchantModels();
            try
            {
                CustomerRequest paraBody = new CustomerRequest();
                paraBody.ID = Id;

                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.customerAPIMerchantGet, null, paraBody);
                dynamic data = result.Data;
                var lstDataRaw = data["Merchant"];
                var lstObject = JsonConvert.SerializeObject(lstDataRaw);
                model = JsonConvert.DeserializeObject<MerchantModels>(lstObject);
                if (model != null)
                {
                    model.CreatedDate = CommonHelper.ConvertToLocalTime(model.CreatedDate);
                }
                NSLog.Logger.Info("GetDetailMerchant", model);
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("CustomerFactory ->GetDetailMerchant :", ex);
            }
            return model;
        }

        public List<ProductApplyStoreModels> GetListProductApplyStore(string CustomerID)
        {
            List<ProductApplyStoreModels> lstData = new List<ProductApplyStoreModels>();
            try
            {
                CustomerRequest paraBody = new CustomerRequest();
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
                CustomerRequest paraBody = new CustomerRequest();
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
        public StoreDetailModels GetStoreDetail(string StoreID)
        {
            StoreDetailModels lstdata = new StoreDetailModels();
            try
            {
                CustomerRequest paraBody = new CustomerRequest();
                paraBody.ID = StoreID;

                NSLog.Logger.Info("GetStoreDetail Request: ", paraBody);

                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.customerAPIStoreGet, null, paraBody);
                dynamic data = result.Data;
                var lstDataRaw = data["StoreDetail"];
                var lstObject = JsonConvert.SerializeObject(lstDataRaw);
                lstdata = JsonConvert.DeserializeObject<StoreDetailModels>(lstObject);
                NSLog.Logger.Info("GetStoreDetail", lstdata);
                return lstdata;
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("GetStoreDetail_Fail", ex);
                return lstdata;
            }
        }

        public List<StoreModels> GetListStore(string CustomerID)
        {
            List<StoreModels> lstData = new List<StoreModels>();
            try
            {
                CustomerRequest paraBody = new CustomerRequest();
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

        public bool CreateStoreInfo(StoreDetailModels model,ref string StoreIDReturn, ref string msg)
        {
            try
            {
                CustomerRequest paraBody = new CustomerRequest();
                paraBody.StoreInfo = model;
                paraBody.AssetID = string.IsNullOrEmpty(model.AssetID) ? model.tempAssetID : model.AssetID;
                paraBody.ID = model.CustomerID;
                paraBody.CreatedUser = model.CreateUser;

                NSLog.Logger.Info("CreateStoreInfo Request: ", paraBody);

                //====================
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.customerAPICreateStore, null, paraBody);
                if (result != null)
                {
                    if (result.Success) {
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
        public bool UpdateStoreInfo(StoreDetailModels model, ref string msg)
        {
            try
            {
                CustomerRequest paraBody = new CustomerRequest();
                paraBody.StoreInfo = model;
                paraBody.CreatedUser = model.CreateUser;
                NSLog.Logger.Info("UpdateStoreInfo Request: ", paraBody);

                //====================
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.customerAPIEditStore, null, paraBody);
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
        #endregion

        #region  ProductsManagement

        public ProductManagementModels GetListDataProductsManagement(string CustomerID)
        {
            ProductManagementModels item = new ProductManagementModels();

            List<HardwareServiceModels> ListHardwareService = new List<HardwareServiceModels>();
            List<ProductPackageModels> ListProductPackage = new List<ProductPackageModels>();
            List<AdditionFunctionModels> ListFunction = new List<AdditionFunctionModels>();
            List<AccountManagementModels> ListAccount = new List<AccountManagementModels>();
            try
            {
                CustomerRequest paraBody = new CustomerRequest();
                NSLog.Logger.Info("GetListDataProductsManagement Request: ", paraBody);
                paraBody.ID = CustomerID;
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.customerAPIProductsManagement, null, paraBody);
                dynamic data = result.Data;

                var lstDataRaw = data["ListHardwareService"];
                var lstObject = JsonConvert.SerializeObject(lstDataRaw);
                ListHardwareService = JsonConvert.DeserializeObject<List<HardwareServiceModels>>(lstObject);

                lstDataRaw = data["ListProductPackage"];
                lstObject = JsonConvert.SerializeObject(lstDataRaw);
                ListProductPackage = JsonConvert.DeserializeObject<List<ProductPackageModels>>(lstObject);

                lstDataRaw = data["ListFunction"];
                lstObject = JsonConvert.SerializeObject(lstDataRaw);
                ListFunction = JsonConvert.DeserializeObject<List<AdditionFunctionModels>>(lstObject);

                lstDataRaw = data["ListAccount"];
                lstObject = JsonConvert.SerializeObject(lstDataRaw);
                ListAccount = JsonConvert.DeserializeObject<List<AccountManagementModels>>(lstObject);

                NSLog.Logger.Info("GetListDataProductsManagement", item);
                if(ListHardwareService != null)
                {
                    ListHardwareService.ForEach(x => x.BoughtTime = x.BoughtTime.HasValue ? CommonHelper.ConvertToLocalTime(x.BoughtTime.Value) : x.BoughtTime);

                }

                if(ListProductPackage != null)
                {
                    ListProductPackage.ForEach(x =>
                    {
                        x.BoughtTime = x.BoughtTime.HasValue ? CommonHelper.ConvertToLocalTime(x.BoughtTime.Value) : x.BoughtTime;
                        x.ActivateTime = x.ActivateTime.HasValue ? CommonHelper.ConvertToLocalTime(x.ActivateTime.Value) : x.ActivateTime;
                        x.ExpiryDate = x.ExpiryDate.HasValue ? CommonHelper.ConvertToLocalTime(x.ExpiryDate.Value) : x.ExpiryDate;

                    });
                }

                if(ListAccount != null)
                {
                    ListAccount.ForEach(x =>
                    {
                        x.ActivateTime = x.ActivateTime.HasValue ? CommonHelper.ConvertToLocalTime(x.ActivateTime.Value) : x.ActivateTime;
                        x.ExpiryDate = x.ExpiryDate.HasValue ? CommonHelper.ConvertToLocalTime(x.ExpiryDate.Value) : x.ExpiryDate;
                    });
                }

                if(ListFunction != null)
                {
                    ListFunction.ForEach(x =>
                    {
                        x.ActivateTime = x.ActivateTime.HasValue ? CommonHelper.ConvertToLocalTime(x.ActivateTime.Value) : x.ActivateTime;
                        x.ExpiryDate = x.ExpiryDate.HasValue ? CommonHelper.ConvertToLocalTime(x.ExpiryDate.Value) : x.ExpiryDate;

                    });
                }

                item.ListHardwareService = ListHardwareService;
                item.ListProductPackage = ListProductPackage;
                item.ListFunction = ListFunction;
                item.ListAccount = ListAccount;
                
                return item;
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("GetListDataProductsManagement", e);
                return item;
            }
        }

        public bool AddSerialNumberHardwareService(string ID, List<SerialNumberDTO> ListSerialNo, string CreateUser, ref string msg)
        {
            try
            {
                CustomerRequest paraBody = new CustomerRequest();
                paraBody.CreatedUser = CreateUser;
                paraBody.ID = ID;
                paraBody.ListSerialNo = ListSerialNo;
                NSLog.Logger.Info("AddSerialNumberHardwareService Request: ", paraBody);
                //====================
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.CustomerAssetAddserialnumber, null, paraBody);
                if (result != null)
                {
                    if (result.Success)
                        return true;
                    else
                    {
                        msg = result.Message;
                        NSLog.Logger.Info("AddSerialNumberHardwareService", result.Message);
                        return false;
                    }
                }
                else
                {
                    NSLog.Logger.Info("AddSerialNumberHardwareService", result);
                    return false;
                }
            }
            catch (Exception e)
            {
                msg = e.ToString();
                NSLog.Logger.Error("AddSerialNumberHardwareService_Fail", e);
                return false;
            }
        }

        public bool ChangeStatusHardwareService(List<string> ListAsset, string CreateUser, ref string msg)
        {
            try
            {
                //CustomerRequest paraBody = new CustomerRequest();
                //paraBody.CreatedUser = CreateUser;
                //paraBody.ID = ID;

                ChangeStatusProductManagementRequest paraBody = new ChangeStatusProductManagementRequest();
                paraBody.ListAssetID = ListAsset;
                NSLog.Logger.Info("ChangeStatusHardwareService Request: ", paraBody);
                //====================
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.customerAPIProductsManagementChangeStatusHardwareService, null, paraBody);
                if (result != null)
                {
                    if (result.Success)
                        return true;
                    else
                    {
                        msg = result.Message;
                        NSLog.Logger.Info("ChangeStatusHardwareService", result.Message);
                        return false;
                    }
                }
                else
                {
                    NSLog.Logger.Info("ChangeStatusHardwareService", result);
                    return false;
                }
            }
            catch (Exception e)
            {
                msg = e.ToString();
                NSLog.Logger.Error("ChangeStatusHardwareService_Fail", e);
                return false;
            }
        }

        public bool ChangeStatusProductPackage(string ID, string CreateUser, ref string msg)
        {
            try
            {
                CustomerRequest paraBody = new CustomerRequest();
                paraBody.CreatedUser = CreateUser;
                paraBody.ID = ID;
                NSLog.Logger.Info("ChangeStatusProductPackage Request: ", paraBody);
                //====================
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.customerAPIProductsManagementChangeStatusProductPackage, null, paraBody);
                if (result != null)
                {
                    if (result.Success)
                        return true;
                    else
                    {
                        msg = result.Message;
                        NSLog.Logger.Info("ChangeStatusProductPackage", result.Message);
                        return false;
                    }
                }
                else
                {
                    NSLog.Logger.Info("ChangeStatusProductPackage", result);
                    return false;
                }
            }
            catch (Exception e)
            {
                msg = e.ToString();
                NSLog.Logger.Error("ChangeStatusProductPackage_Fail", e);
                return false;
            }
        }

        public bool ChangeStatusAccount(string ID, string CreateUser, ref string msg)
        {
            try
            {
                CustomerRequest paraBody = new CustomerRequest();
                paraBody.CreatedUser = CreateUser;
                paraBody.ID = ID;
                NSLog.Logger.Info("ChangeStatusAccount Request: ", paraBody);
                //====================
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.customerAPIProductsManagementChangeStatusAccount, null, paraBody);
                if (result != null)
                {
                    if (result.Success)
                        return true;
                    else
                    {
                        msg = result.Message;
                        NSLog.Logger.Info("ChangeStatusAccount", result.Message);
                        return false;
                    }
                }
                else
                {
                    NSLog.Logger.Info("ChangeStatusAccount", result);
                    return false;
                }
            }
            catch (Exception e)
            {
                msg = e.ToString();
                NSLog.Logger.Error("ChangeStatusAccount_Fail", e);
                return false;
            }
        }

        public bool ChangeStatusFunction(string ID, string CreateUser, ref string msg)
        {
            try
            {
                CustomerRequest paraBody = new CustomerRequest();
                paraBody.CreatedUser = CreateUser;
                paraBody.ID = ID;
                NSLog.Logger.Info("ChangeStatusFunction Request: ", paraBody);
                //====================
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.customerAPIProductsManagementChangeStatusFunction, null, paraBody);
                if (result != null)
                {
                    if (result.Success)
                        return true;
                    else
                    {
                        msg = result.Message;
                        NSLog.Logger.Info("ChangeStatusFunction", result.Message);
                        return false;
                    }
                }
                else
                {
                    NSLog.Logger.Info("ChangeStatusFunction", result);
                    return false;
                }
            }
            catch (Exception e)
            {
                msg = e.ToString();
                NSLog.Logger.Error("ChangeStatusFunction_Fail", e);
                return false;
            }
        }

        public bool ChangeBlockFunction(string ID, string CreateUser, ref string msg)
        {
            try
            {
                CustomerRequest paraBody = new CustomerRequest();
                paraBody.CreatedUser = CreateUser;
                paraBody.ID = ID;
                NSLog.Logger.Info("ChangeBlockFunction Request: ", paraBody);
                //====================
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.customerAPIProductsManagementChangeBlockFunction, null, paraBody);
                if (result != null)
                {
                    if (result.Success)
                        return true;
                    else
                    {
                        msg = result.Message;
                        NSLog.Logger.Info("ChangeBlockFunction", result.Message);
                        return false;
                    }
                }
                else
                {
                    NSLog.Logger.Info("ChangeBlockFunction", result);
                    return false;
                }
            }
            catch (Exception e)
            {
                msg = e.ToString();
                NSLog.Logger.Error("ChangeBlockFunction_Fail", e);
                return false;
            }
        }

        public ProductPackageAdminModels GetListDataProductsPackage(string AssetId)
        {
            ProductPackageAdminModels item = new ProductPackageAdminModels();

            try
            {
                CustomerRequest paraBody = new CustomerRequest();
                NSLog.Logger.Info("GetListDataProductsPackage Request: ", paraBody);
                paraBody.ID = AssetId;
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.customerAPIProductPackage, null, paraBody);
                dynamic data = result.Data;

                var lstDataRaw = data["ProductPackage"];
                var lstObject = JsonConvert.SerializeObject(lstDataRaw);
                item = JsonConvert.DeserializeObject<ProductPackageAdminModels>(lstObject);

                NSLog.Logger.Info("GetListDataProductsPackage", item);
                if(item != null)
                {
                    item.ExpiryDate = item.ExpiryDate.HasValue ? CommonHelper.ConvertToLocalTime(item.ExpiryDate.Value) : item.ExpiryDate;

                    if(item.ListDevice != null)
                    {
                        item.ListDevice.ForEach(x =>
                        {
                            x.ActiveTime = x.ActiveTime.HasValue ? CommonHelper.ConvertToLocalTime(x.ActiveTime.Value) : x.ActiveTime;
                        });
                    }
                }
                return item;
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("GetListDataProductsManagement", e);
                return item;
            }
        }       
        public List<ProductPriceAdminModels> GetPriceProduct(string ProductID)
        {
            var models = new List<ProductPriceAdminModels>();
            try
            {
                CustomerRequest paraBody = new CustomerRequest();
                paraBody.ID = ProductID;
                NSLog.Logger.Info("GetPriceProduct Request: ", paraBody);

                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.ClientSiteStoreBusinessPriceList, null, paraBody);
                NSLog.Logger.Info("GetPriceProduct Response : ", result);
                dynamic data = result.Data;

                var lstDataRaw = data["ListPrice"];
                var lstObject = JsonConvert.SerializeObject(lstDataRaw);
                models = JsonConvert.DeserializeObject<List<ProductPriceAdminModels>>(lstObject);
                return models;
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("GetPriceProduct ", ex);
                return models;
            }
        }
        
        public NSCSC.Shared.Models.ClientSite.MyProfile.OrderDetailModels GetOrderDataProducts(string CustomerID, string AssetId, string Period, string PeriodType, string Price, string ProductAppliOn, string OrderID = null)
        {
            var models = new NSCSC.Shared.Models.ClientSite.MyProfile.OrderDetailModels();
            try
            {
                YourCartRequest paraBody = new YourCartRequest();
                paraBody.CreatedUser = "";
                paraBody.CusID = CustomerID;
                paraBody.OrderID = OrderID;
                Models.ClientSite.MyProfile.Item item = new Models.ClientSite.MyProfile.Item();
                item.ProductID = ProductAppliOn;
                item.Period = int.Parse(Period);
                item.Price = double.Parse(Price);
                item.Quantity = 1;
                item.IsExtend = true;
                item.PeriodType = int.Parse(PeriodType);
                item.ListAdditionApply.Add(new Models.ClientSite.MyProfile.AdditionApply()
                {
                    ID = AssetId
                });

                paraBody.ListItems.Add(item);
                paraBody.Note = "";
                NSLog.Logger.Info("Checkout | AddItems Request: ", paraBody);

                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.ClientSiteYourCartAddItems, null, paraBody);
                NSLog.Logger.Info("GetPriceProduct Response : ", result);
                dynamic data = result.Data;

                var lstDataRaw = data["OrderDetail"];
                var lstObject = JsonConvert.SerializeObject(lstDataRaw);
                models = JsonConvert.DeserializeObject<NSCSC.Shared.Models.ClientSite.MyProfile.OrderDetailModels>(lstObject);
                return models;
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("GetPriceProduct ", ex);
                return models;
            }
        }

        public NSCSC.Shared.Models.ClientSite.MyProfile.HistoriesPaid OrderPay(string OrderID, string PaymentMethodID, double AmountPay, string CreatedUser, ref string OrderIDReturn, ref string msg)
        {
            var model = new NSCSC.Shared.Models.ClientSite.MyProfile.HistoriesPaid();
            try
            {
                YourCartRequest paraBody = new YourCartRequest();
                paraBody.OrderID = OrderID;
                paraBody.PaymentMethodID = PaymentMethodID;
                paraBody.AmountPay = AmountPay;
                paraBody.IsManual = true;
                paraBody.CreatedUser = CreatedUser;
                NSLog.Logger.Info("OrderPayRequest", paraBody);
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.ClientSiteYourCartOrderPay, null, paraBody);
                NSLog.Logger.Info("OrderPayRequest", result);
                if (result != null)
                {
                    if (result.Success)
                    {
                        dynamic data = result.Data;
                        var ID = data["ID"];
                        if (ID == null)
                        {
                            var Remaining = data["Remaining"];
                            var TotalAmount = data["TotalAmount"];
                            var Change = data["Change"];
                            var ListDataRaw = data["ListPaidMethod"];
                            var lstObject = JsonConvert.SerializeObject(ListDataRaw);
                            model.Remaining = Convert.ToDouble(Remaining);
                            model.TotalAmount = Convert.ToDouble(TotalAmount);
                            model.Change = Convert.ToDouble(Change);                            
                            model.ListPaidMethod = JsonConvert.DeserializeObject<List<NSCSC.Shared.Models.ClientSite.MyProfile.PaymentDetail>>(lstObject);
                            
                        }
                        else
                        {
                            model.Remaining = 0;
                        }
                        return model;
                    }
                    else
                    {
                        msg = result.Message;
                        NSLog.Logger.Info("OrderPay", result.Message);
                        return model;
                    }
                }
                else
                {
                    NSLog.Logger.Info("OrderPay", result);
                    return model;
                }
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("OrderPay_Fail", e);
                return model;
            }
        }
        public bool OrderCancel(string ID, ref string msg)
        {
            try
            {
                YourCartRequest paraBody = new YourCartRequest();
                paraBody.ID = ID;
                NSLog.Logger.Info("OrderCancelPayRequest", paraBody);
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.ClientSiteYourCartOrderCancelPay, null, paraBody);
                NSLog.Logger.Info("OrderCancelPayRequest", result);
                if (result != null)
                {
                    if (result.Success)
                        return true;
                    else
                    {
                        msg = result.Message;
                        NSLog.Logger.Info("OrderCancelPay", result.Message);
                        return false;
                    }
                }
                else
                {
                    NSLog.Logger.Info("OrderCancelPay", result);
                    return false;
                }
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("OrderCancelPay_Fail", e);
                return false;
            }
        }
        public bool DoneOrderPay(string ID, string useid, ref string msg)
        {
            try
            {
                YourCartRequest paraBody = new YourCartRequest();
                paraBody.ID = ID;
                paraBody.CreatedUser = useid;
                paraBody.IsManual = true;
                NSLog.Logger.Info("OrderDonePayRequest", paraBody);
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.ClientSiteYourCartOrderDonePay, null, paraBody);
                NSLog.Logger.Info("OrderDonePayRequest", result);
                if (result != null)
                {
                    if (result.Success)
                        return true;
                    else
                    {
                        msg = result.Message;
                        NSLog.Logger.Info("OrderDonePay", result.Message);
                        return false;
                    }
                }
                else
                {
                    NSLog.Logger.Info("OrderDonePay", result);
                    return false;
                }
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("OrderDonePay_Fail", e);
                return false;
            }
        }

        public bool ChangeStatusDevice(string DeviceID, ref string msg)
        {
            try
            {
                CustomerRequest paraBody = new CustomerRequest();
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
        #endregion

        #region "Done Refund"
        public bool DoneRefundOrder(DoneRefundModels models, ref string msg)
        {
            try
            {
                DoneRefundRequest paraBody = new DoneRefundRequest();
                paraBody.OrderID = models.OrderID;
                paraBody.TotalAmount = models.TotalAmount;
                paraBody.Description = models.Description;
                paraBody.ListDetail = models.ListDetail;

                NSLog.Logger.Info("DoneRefundOrder Request: ", paraBody);
                //====================
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.CRMCustomerDoneRefund, null, paraBody);
                if (result != null)
                {
                    if (result.Success)
                        return true;
                    else
                    {
                        msg = result.Message;
                        NSLog.Logger.Info("DoneRefundOrder", result.Message);
                        return false;
                    }
                }
                else
                {
                    NSLog.Logger.Info("DoneRefundOrder", result);
                    return false;
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("DoneRefundOrder", ex);
                return false;
            }
        }
        #endregion
    }
}
