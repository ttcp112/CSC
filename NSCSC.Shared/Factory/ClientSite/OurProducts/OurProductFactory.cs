using Newtonsoft.Json;
using NSCSC.Shared.Models;
using NSCSC.Shared.Models.ClientSite.OurProducts;
using NSCSC.Shared.Models.ClientSite.OurProDucts;
using NSCSC.Shared.Models.OurProducts;
using NSCSC.Shared.Models.OurProducts.Addition;
using NSCSC.Shared.Models.OurProducts.Price;
using NSCSC.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using NSCSC.Shared.Models.ClientSite.MyStoreAndBusiness;

namespace NSCSC.Shared.Factory.ClientSite.OurProducts
{
    public class OurProductFactory
    {

        public List<PromotionDetailModels> GetPromotion(byte Etype = 0, bool IsPackage = false)
        {
            List<PromotionDetailModels> lstData = new List<PromotionDetailModels>();
            try
            {
                OurProductRequest paraBody = new OurProductRequest();
                paraBody.EType = Etype;
                paraBody.IsPackage = IsPackage;

                NSLog.Logger.Info("Get Promotions Request: ", paraBody);

                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.ClientSideOurProductGetPromo, null, paraBody);
                dynamic data = result.Data;
                var lstDataRaw = data["ListPromo"];
                var lstObject = JsonConvert.SerializeObject(lstDataRaw);
                lstData = JsonConvert.DeserializeObject<List<PromotionDetailModels>>(lstObject);
                NSLog.Logger.Info("Get Promotions Response", lstData);

                return lstData;
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("GetListPromotions_Fail", e);
                return lstData;
            }
        }

        public List<CategoryDetailModels> ProductGetCate(string cateId = null)
        {
            List<CategoryDetailModels> listData = new List<CategoryDetailModels>();
            try
            {
                ProductGetCateRequest paraBody = new ProductGetCateRequest();
                paraBody.IsActive = true;
                paraBody.ID = cateId;

                NSLog.Logger.Info("ProductGetCate Request: ", paraBody);

                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.ClientSideOurProductGetCate, null, paraBody);
                dynamic data = result.Data;
                var lstDataRaw = data["ListCategory"];
                var lstObject = JsonConvert.SerializeObject(lstDataRaw);
                listData = JsonConvert.DeserializeObject<List<CategoryDetailModels>>(lstObject);

                NSLog.Logger.Info("ProductGetCate", listData);
                if (listData != null && listData.Any())
                {
                    listData = listData.OrderBy(oo => oo.Name).ThenBy(ss => ss.Sequence).ToList();
                }
                return listData;
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("ProductGetCate_Fail", e);
                return listData;
            }
        }

        /// <summary>
        /// Function [GetListData]
        /// CustomerID for Reseller
        /// </summary>
        /// <param name="CustomerID"></param>
        /// <param name="SearchString"></param>
        /// <param name="ProductType"></param>
        /// <param name="ListCateID"></param>
        /// <param name="ListProductID"></param>
        /// <param name="ProductID"></param>
        /// <param name="ListType"></param>
        /// <returns></returns>
        public List<OurProDuctsModel> GetListData(string CustomerID, string SearchString = null, int ProductType = 0,
            List<string> ListCateID = null, List<string> ListProductID = null, string ProductID = null, List<int> ListType = null)
        {
            List<OurProDuctsModel> listData = new List<OurProDuctsModel>();
            try
            {
                ProductGetRequest paraBody = new ProductGetRequest();
                paraBody.ProductType = ProductType;
                paraBody.SearchString = SearchString;
                paraBody.ListCateID = ListCateID;
                paraBody.ListProductID = ListProductID;
                paraBody.ListType = ListType;
                //===Get Detail
                paraBody.ID = ProductID;
                paraBody.CustomerID = CustomerID;

                NSLog.Logger.Info("ProductGetListData Request: ", paraBody);

                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.ClientSideOurProductGet, null, paraBody);
                dynamic data = result.Data;
                var lstDataRaw = data["ListProduct"];
                var lstObject = JsonConvert.SerializeObject(lstDataRaw);
                listData = JsonConvert.DeserializeObject<List<OurProDuctsModel>>(lstObject);

                NSLog.Logger.Info("ProductGetListData", listData);

                if (listData != null && listData.Any())
                {
                    listData = listData.OrderBy(oo => oo.Name).ThenBy(ss => ss.Sequence).ToList();
                }
                return listData;
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("ProductGetListData_Fail", e);
                return listData;
            }
        }

        /*Trongntn*/
        /// <summary>
        ///  when click button PRICE LIST
        /// </summary>
        /// <param name="ProductType"> Product | Package | Addition </param>
        /// <param name="AdditionType"> Hardwares | Softwares | Locations | Services | Accounts | Functions </param>
        /// <param name="CategoryID"> -> for Type = Product when click Category go to Product List</param>
        /// <returns></returns>
        public PriceListProductModels GetListPrice(int ProductType, int AdditionType, string CategoryID)
        {
            PriceListProductModels item = new PriceListProductModels();

            List<PeriodModels> ListPeriod = new List<PeriodModels>();
            List<ProductPeriodModels> ListProduct = new List<ProductPeriodModels>();
            try
            {
                OurProductRequest paraBody = new OurProductRequest();
                paraBody.ProductType = ProductType;
                paraBody.AdditionType = AdditionType;
                paraBody.CategoryID = CategoryID;

                NSLog.Logger.Info("GetListPrice Request: ", paraBody);

                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.ClientSiteOurProductGetPriceList, null, paraBody);
                dynamic data = result.Data;

                var lstDataRaw = data["ListPeriod"];
                var lstObject = JsonConvert.SerializeObject(lstDataRaw);
                ListPeriod = JsonConvert.DeserializeObject<List<PeriodModels>>(lstObject);

                lstDataRaw = data["ListProduct"];
                lstObject = JsonConvert.SerializeObject(lstDataRaw);
                ListProduct = JsonConvert.DeserializeObject<List<ProductPeriodModels>>(lstObject);

                NSLog.Logger.Info("GetListPrice", item);
                item.ListPeriod = ListPeriod;
                item.ListProduct = ListProduct;

                return item;
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("GetListPrice_Fail", e);
                return item;
            }
        }

        public List<ProductApplyAdditionModels> ProductGetProductApplyAddition(string AdditionID, string CustomerID)
        {
            List<ProductApplyAdditionModels> listData = new List<ProductApplyAdditionModels>();
            try
            {
                ProductGetRequest paraBody = new ProductGetRequest();
                paraBody.ID = CustomerID;
                paraBody.AdditionID = AdditionID;
                NSLog.Logger.Info("ProductGetProductApplyAddition Request: ", paraBody);
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.ClientSiteOurProductGetProductApplyAddition, null, paraBody);
                dynamic data = result.Data;
                var lstDataRaw = data["ListProduct"];
                var lstObject = JsonConvert.SerializeObject(lstDataRaw);
                listData = JsonConvert.DeserializeObject<List<ProductApplyAdditionModels>>(lstObject);
                if (listData != null && listData.Any())
                {
                    // Convert datetime
                    listData.ForEach(xy =>
                    {
                        xy.BoughtTime = CommonHelper.ConvertToLocalTime(xy.BoughtTime);
                        if (xy.ExpiredTime.HasValue)
                        {
                            xy.ExpiredTime = CommonHelper.ConvertToLocalTime(xy.ExpiredTime.Value);
                        }
                        if (xy.ListProduct != null && xy.ListProduct.Any())
                        {
                            xy.ListProduct.ForEach(xyz =>
                            {
                                xyz.BoughtTime = CommonHelper.ConvertToLocalTime(xyz.BoughtTime);
                                if (xyz.ExpiredTime.HasValue)
                                {
                                    xyz.ExpiredTime = CommonHelper.ConvertToLocalTime(xyz.ExpiredTime.Value);
                                }
                            });
                        }
                    });
                }
                NSLog.Logger.Info("ProductGetProductApplyAddition", listData);
                return listData;
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("ProductGetProductApplyAddition_Fail", e);
                return listData;
            }
        }

        // Get information of Product for Buy Addition from My Store & Business
        public List<ProductApplyStoreModels> GetProductApplyAdditionsStoreBusiness(string AssetID, string CustomerID)
        {
            List<ProductApplyStoreModels> lstData = new List<ProductApplyStoreModels>();
            try
            {
                MyStoreAndBusinessRequest paraBody = new MyStoreAndBusinessRequest();
                paraBody.ID = CustomerID;
                paraBody.AssetID = AssetID;

                NSLog.Logger.Info("GetProductApplyAdditionsStoreBusiness Request: ", paraBody);

                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.ClientSiteStoreBusinessGetListProductApplyStore, null, paraBody);
                dynamic data = result.Data;
                var lstDataRaw = data["ListProduct"];
                var lstObject = JsonConvert.SerializeObject(lstDataRaw);
                lstData = JsonConvert.DeserializeObject<List<ProductApplyStoreModels>>(lstObject);
                NSLog.Logger.Info("GetProductApplyAdditionsStoreBusiness", lstData);

                return lstData;
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("GetProductApplyAdditionsStoreBusiness_Fail", e);
                return lstData;
            }
        }
    }
}