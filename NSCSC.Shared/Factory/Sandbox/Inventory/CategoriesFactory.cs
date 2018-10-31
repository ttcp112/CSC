
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSCSC.Shared.Models.Sandbox.Inventory.Category;
using NSCSC.Shared.Models;
using NSCSC.Shared.Utilities;
using Newtonsoft.Json;
using ClosedXML.Excel;

namespace NSCSC.Shared.Factory.Sandbox.Inventory
{
    public class CategoriesFactory : BaseFactory
    {

        private BaseFactory _baseFactory = null;

        public CategoriesFactory()
        {
            _baseFactory = new BaseFactory();

        }
        // get list catagory
        public List<CategoriesModels> GetListCategory(string userid)
        {
            List<CategoriesModels> listData = new List<CategoriesModels>();
            try
            {
                RequestCategoriesModels paraBody = new RequestCategoriesModels();
                paraBody.CreatedUser = userid;
                paraBody.SearchString = "";
                
                NSLog.Logger.Info("GetListCategory Request", paraBody);
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.CategoryAPIGetList, null, paraBody);
                NSLog.Logger.Info("GetListCategory Result", result);
                dynamic data = result.Data;
                var lstC = data["ListCategory"];
                var lstContent = JsonConvert.SerializeObject(lstC);
                listData = JsonConvert.DeserializeObject<List<CategoriesModels>>(lstContent);
                if (listData != null && listData.Any())
                {
                    listData = listData.OrderBy(oo => oo.Sequence).ThenBy(ss => ss.Name).ToList();
                }
                //NSLog.Logger.Info("CategoryGetListData", listData);
                return listData;
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("CategoryGetList_Fail: ", e);
                return listData;
            }
        }
        // Create Catagory
        public bool InsertOrUpdateCategory(CategoriesModels model, string userid, ref string msg)
        {
            try
            {
                RequestCategoriesModels paraBody = new RequestCategoriesModels();
                paraBody.CreatedUser = userid;
                paraBody.ID = model.Id;
                paraBody.CategoryDetail = model;
                paraBody.IsConfirm = model.IsConfirm;
                NSLog.Logger.Info("Update Category Request", paraBody);
                //====================

                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.CategoryAPICreateOrEdit, null, paraBody);
                NSLog.Logger.Info("Update Category Request", result);
                if (result != null)
                {
                    if (result.Success)
                        return true;
                    else
                    {
                        msg = result.Message;
                        NSLog.Logger.Info("CategoryCreateorEdit", result.Message);
                        return false;
                    }
                }
                else
                {
                    NSLog.Logger.Info("CategoryCreateorEdit", result);
                    return false;
                }
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("CategoryInsertOrUpdate_Fail", e);
                return false;
            }
        }

        // Get Detail
        public CategoriesModels GetDetail(string ID)
        {
            CategoriesModels CategoryDTO = new CategoriesModels();
            try
            {
                RequestCategoriesModels paraBody = new RequestCategoriesModels();
                paraBody.ID = ID;
                NSLog.Logger.Info("GetDetail Category Request", paraBody);
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.CategoryAPIGetDetail, null, paraBody);
                NSLog.Logger.Info("GetDetail Category Request", result);
                dynamic data = result.Data;
                var lstDataRaw = data["CategoryDetail"];
                var lstContent = JsonConvert.SerializeObject(lstDataRaw);
                CategoryDTO = JsonConvert.DeserializeObject<CategoriesModels>(lstContent);
                NSLog.Logger.Info("CategoryGetDetail", CategoryDTO);
                return CategoryDTO;

            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("CategoryGetDetail_Fail", ex);
                return null;
            }

        }

        //Delete
        public bool Delete(string ID, string UserId, ref string msg, string ReasonDelete = null)
        {
            try
            {
                RequestCategoriesModels Para = new RequestCategoriesModels();
                Para.ID = ID;
                Para.CreatedUser = UserId;
                Para.ReasonDelete = ReasonDelete;
                NSLog.Logger.Info("GetDetail Category Request", Para);
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.CategoryAPIDelete, null, Para);
                NSLog.Logger.Info("GetDetail Category Request", result);
                if (result != null)
                {
                    if (result.Success)
                        return true;
                    else
                    {
                        msg = result.Message;
                        NSLog.Logger.Info("CategoryDelete", result.Message);
                        return false;
                    }
                }
                else
                {
                    NSLog.Logger.Info("CategoryDelete", result);
                    return false;
                }
            }
            catch (Exception e)
            {
                msg = e.ToString();
                NSLog.Logger.Error("Category_Delete: ", e);
                return false;
            }
        }
        // Export
        //public StatusResponse Export(ref IXLWorksheet ws, string UserId)
        //{
        //    StatusResponse Response = new StatusResponse();
        //    try
        //    {           
        //        List<CategoriesModels> listData = new List<CategoriesModels>();
        //        RequestCategoriesModels paraBody = new RequestCategoriesModels();
        //        paraBody.CreatedUser = UserId;
        //        var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.CategoryAPIExport, null, paraBody);
        //        dynamic data = result.Data;
        //        var lstC = data["LstCategory"];
        //        var lstContent = JsonConvert.SerializeObject(lstC);
        //        listData = JsonConvert.DeserializeObject<List<CategoriesModels>>(lstContent);
        //        listData = listData.OrderBy(x => x.Name).ToList();
        //        int row = 1;
        //        string[] listSetMenuHeader = new string[] {
        //            "Index", "Category Name", "Status", "Sequence", "Category Code", "Free Trial",
        //            "Type", "Short Description", "Description", 
        //        };
        //        for (int i = 1; i <= listSetMenuHeader.Length; i++)
        //            ws.Cell(row, i).Value = listSetMenuHeader[i - 1];
        //        int cols = listSetMenuHeader.Length;
        //        //Item
        //        row = 2;
        //        int countIndex = 1;

        //        foreach (var item in listData)
        //        {
        //            ws.Cell("A" + row).Value = countIndex;
        //            ws.Cell("B" + row).Value = item.Name;
        //            ws.Cell("C" + row).Value = item.Active ? "Active" : "InActive";
        //            ws.Cell("D" + row).Value = item.Email;
        //            ws.Cell("E" + row).Value = "'" + item.Phone;
        //            ws.Cell("F" + row).Value = !item.Gender.HasValue ? "" : item.Gender.Value ? "Male" : "Female";
        //            ws.Cell("G" + row).Value = item.MaritalStatus ? "Married" : "Single";
        //            ws.Cell("H" + row).Value = item.MaritalStatus ? "'" + CommonHelper.ConvertToLocalTime(item.Aniversary).ToString("dd/MM/yyyy") : "";
        //            ws.Cell("I" + row).Value = item.IsWantMembership ? "Yes" : "No";

        //            row++;
        //            countIndex++;
        //        }
        //        FormatExcelExport(ws, row, cols);               


        //        Response.Status = true;
        //    }
        //    catch (Exception e)
        //    {
        //        Response.Status = false;
        //        Response.MsgError = e.Message;
        //    }
        //    finally
        //    {

        //    }
        //    return Response;
        //}
    }
}
