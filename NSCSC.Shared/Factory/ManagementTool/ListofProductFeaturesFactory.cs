using Newtonsoft.Json;
using NSCSC.Shared.Factory.Sandbox.Inventory;
using NSCSC.Shared.Models;
using NSCSC.Shared.Models.ManagementTool.ListofProductFeatures;
using NSCSC.Shared.Models.Sandbox.Inventory.Category;
using NSCSC.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace NSCSC.Shared.Factory.ManagementTool
{
    public class ListofProductFeaturesFactory:BaseFactory
    {
        public CategoriesFactory _categoryfacetory = null;
        public string List { get; set; }
        public ListofProductFeaturesFactory()
        {
            _categoryfacetory = new CategoriesFactory();
        }
        // get list catagory
        public List<SelectListItem> GetListCategory(string userid)
        {
            List<SelectListItem> ListCategory = new List<SelectListItem>();
            List<CategoriesModels> lstcategory = new List<CategoriesModels>();
            try
            {

                if (userid != null)
                {
                    lstcategory = _categoryfacetory.GetListCategory(userid);
                    if(lstcategory != null && lstcategory.Count > 0)
                    {
                        foreach(var item in lstcategory)
                        {
                            ListCategory.Add(new SelectListItem()
                            {
                                Value= item.Id.ToString(),
                                Text= item.Name
                            });
                        }
                        ListCategory = ListCategory.OrderBy(x => x.Text).ToList();
                    }
                }            
              
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error(ex);
            }
            return ListCategory;
        }

        // GetListofProductFeatures
        public List<ProductFeatureModels> GetListofProductFeatures(string userid)
        {
            List<ProductFeatureModels> listData = new List<ProductFeatureModels>();
            try
            {
                ProductFeatureRequestModels paraBody = new ProductFeatureRequestModels();               
                paraBody.CreatedUser = userid;
                paraBody.SearchString = "";
                NSLog.Logger.Info("ListofProductFeatures Get List Request", paraBody);
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.ProductFeatureAPIGetList, null, paraBody);
                NSLog.Logger.Info("ListofProductFeatures Get List Request", result);
                dynamic data = result.Data;
                var lstC = data["ListProductFeature"];
                var lstContent = JsonConvert.SerializeObject(lstC);
                listData = JsonConvert.DeserializeObject<List<ProductFeatureModels>>(lstContent);
                NSLog.Logger.Info("ListofProductFeatureGetListData", listData);
                return listData;
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("ListofProductFeatures_Fail: ", e);
                return listData;
            }
        }
        // Create Catagory
        public bool InsertOrUpdateCategory(ProductFeatureDetailModels model, string userid, ref string msg)
        {
            try
            {
                ProductFeatureRequestModels paraBody = new ProductFeatureRequestModels();
                paraBody.CreatedUser = userid;
                paraBody.ID = model.ID;
                paraBody.ProductFeatureDetail = model;
                NSLog.Logger.Info("ListofProductFeatures Update Request", paraBody);
                //====================

                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.ProductFeatureAPICreateOrEdit, null, paraBody);
                NSLog.Logger.Info("ListofProductFeatures Update Request", result);
                if (result != null)
                {
                    if (result.Success)
                        return true;
                    else
                    {
                        msg = result.Message;
                        NSLog.Logger.Info("ListofProductFeaturesCreateorEdit", result.Message);
                        return false;
                    }
                }
                else
                {
                    NSLog.Logger.Info("ListofProductFeaturesCreateorEdit", result);
                    return false;
                }
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("ListofProductFeaturesInsertOrUpdate_Fail", e);
                return false;
            }
        }

        // Get Detail
        public ProductFeatureDetailModels GetDetail(string ID)
        {
            ProductFeatureDetailModels ProductFeatureDetail = new ProductFeatureDetailModels();
            try
            {
                ProductFeatureRequestModels paraBody = new ProductFeatureRequestModels();
                paraBody.ID = ID;
                NSLog.Logger.Info("ListofProductFeatures Get Detail Request", paraBody);
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.ProductFeatureAPIGetDetail, null, paraBody);
                NSLog.Logger.Info("ListofProductFeatures Get Detail Request", result);
                dynamic data = result.Data;
                var lstDataRaw = data["ProductFeatureDetail"];
                var lstContent = JsonConvert.SerializeObject(lstDataRaw);
                ProductFeatureDetail = JsonConvert.DeserializeObject<ProductFeatureDetailModels>(lstContent);              

                NSLog.Logger.Info("ListofProductFeatureGetDetailData", ProductFeatureDetail);
                if (result.Success)
                {
                   
                }
                NSLog.Logger.Info("ListofProductFeatureGetDetailData", ProductFeatureDetail);
                return ProductFeatureDetail;

            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("ListofProductFeatureGetDetailData_Fail", ex);
                return null;
            }

        }

        //Delete
        public bool Delete(string ID, string UserId, ref string msg, string ReasonDelete = null)
        {
            try
            {
                ProductFeatureRequestModels Para = new ProductFeatureRequestModels();
                Para.ID = ID;
                Para.CreatedUser = UserId;
                Para.ReasonDelete = ReasonDelete;
                NSLog.Logger.Info("ListofProductFeatures Delete Request", Para);
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.ProductFeatureAPIDelete, null, Para);
                NSLog.Logger.Info("ListofProductFeatures Delete Request", result);
                if (result != null)
                {
                    if (result.Success)
                        return true;
                    else
                    {
                        msg = result.Message;
                        NSLog.Logger.Info("ListofProductFeaturesDelete", result.Message);
                        return false;
                    }
                }
                else
                {
                    NSLog.Logger.Info("ListofProductFeaturesDelete", result);
                    return false;
                }
            }
            catch (Exception e)
            {
                msg = e.ToString();
                NSLog.Logger.Error("ListofProductFeatures_Delete: ", e);
                return false;
            }
        }

        // Action
        public bool ProductFeatureAction(string ID, string UserId, ref string msg)
        {
            try
            {
                ProductFeatureRequestModels Para = new ProductFeatureRequestModels();
                Para.ID = ID;
                Para.CreatedUser = UserId;
                NSLog.Logger.Info("ListofProductFeatures ProductFeatureAction Request", Para);
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.ProductFeatureAPIAction, null, Para);
                NSLog.Logger.Info("ListofProductFeatures ProductFeatureAction Request", result);
                if (result != null)
                {
                    if (result.Success)
                        return true;
                    else
                    {
                        msg = result.Message;
                        NSLog.Logger.Info("ProductFeatureAction", result.Message);
                        return false;
                    }
                }
                else
                {
                    NSLog.Logger.Info("ProductFeatureAction", result);
                    return false;
                }
            }
            catch (Exception e)
            {
                msg = e.ToString();
                NSLog.Logger.Error("ProductFeatureAction: ", e);
                return false;
            }
        }
    }
}
