using Newtonsoft.Json;
using NSCSC.Shared.Models;
using NSCSC.Shared.Models.Sandbox.Inventory.Product;
using NSCSC.Shared.Models.SandBox.Inventory.Product;
using NSCSC.Shared.Utilities;
using NuWebNCloud.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSCSC.Shared.Factory.SandBox.Inventory
{
    public class ProductFactory
    {
        private BaseFactory _baseFactory = null;

        public ProductFactory()
        {
            _baseFactory = new BaseFactory();
        }

        /// <summary>
        /// Result will return list Product
        /// </summary>
        /// <param name="SearchString"></param>
        /// <returns></returns>
        public List<ProductModels> GetListData(string SearchString = null, int ProductType = 0)
        {
            List<ProductModels> listData = new List<ProductModels>();
            try
            {
                ProductRequest paraBody = new ProductRequest();
                paraBody.ProductType = ProductType;
                paraBody.SearchString = SearchString;

                NSLog.Logger.Info("ProductGetListData Request: ", paraBody);

                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.ProductAPIGetList, null, paraBody);
                dynamic data = result.Data;
                var lstDataRaw = data["ListProduct"];
                var lstObject = JsonConvert.SerializeObject(lstDataRaw);
                listData = JsonConvert.DeserializeObject<List<ProductModels>>(lstObject);

                NSLog.Logger.Info("ProductGetListData", listData);

                return listData;
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("ProductGetListData_Fail", e);
                return listData;
            }
        }

        /// <summary>
        /// Result will return Product
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ProductDetailModels GetDetail(string ID)
        {
            ProductDetailModels item = new ProductDetailModels();
            try
            {
                ProductRequest paraBody = new ProductRequest();
                paraBody.ID = ID;

                NSLog.Logger.Info("ProductDetail Request: ", paraBody);

                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.ProductAPIGetDetail, null, paraBody);
                dynamic data = result.Data;
                var lstDataRaw = data["ProductDetail"];
                var lstObject = JsonConvert.SerializeObject(lstDataRaw);
                item = JsonConvert.DeserializeObject<ProductDetailModels>(lstObject);
                if (item != null)
                {
                    item.SaleFrom = CommonHelper.ConvertToLocalTime(item.SaleFrom);
                    item.SaleTo = CommonHelper.ConvertToLocalTime(item.SaleTo);
                }
                NSLog.Logger.Info("ProductGetDetail", item);

                return item;
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("ProductGetDetail_Fail", e);
                return item;
            }
        }

        /// <summary>
        /// Create Or Edit Product
        /// </summary>
        /// <param name="model"></param>
        /// <param name="msg"></param>
        /// <returns> return value type bool </returns>
        public bool CreateOrEdit(ProductDetailModels model, ref string msg)
        {
            try
            {
                ProductRequest paraBody = new ProductRequest();
                model.SaleFrom = CommonHelper.ConvertToUTCTime(model.SaleFrom);
                model.SaleTo = CommonHelper.ConvertToUTCTime(model.SaleTo);

                paraBody.ProductDetail = model;
                paraBody.CreatedUser = model.CreateUser;

                NSLog.Logger.Info("ProductCreateOrEdit Request: ", paraBody);
                //====================
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.ProductAPICreateOrEdit, null, paraBody);
                if (result != null)
                {
                    if (result.Success)
                        return true;
                    else
                    {
                        msg = result.Message;
                        NSLog.Logger.Info("ProductCreateOrEdit", result.Message);
                        return false;
                    }
                }
                else
                {
                    NSLog.Logger.Info("ProductCreateOrEdit", result);
                    return false;
                }
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("ProductCreateOrEdit_Fail", e);
                return false;
            }
        }


        public List<FunctionModels> GetProductFunctions(int Type)
        {
            var listData = new List<FunctionModels>();
            try
            {
                ProductRequest paraBody = new ProductRequest();
                paraBody.Type = Type;

                NSLog.Logger.Info("ListFunction Request: ", paraBody);

                //=================================
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.ProductAPIGetFunction, null, paraBody);
                dynamic data = result.Data;
                var lstDataRaw = data["ListFunction"];
                var lstObject = JsonConvert.SerializeObject(lstDataRaw);
                listData = JsonConvert.DeserializeObject<List<FunctionModels>>(lstObject);

            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("GetProductFunctions", ex);
            }
            return listData;
        }

        /// <summary>
        /// Delete Product
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="CreateUser"></param>
        /// <param name="msg"></param>
        /// <returns> return value type bool </returns>
        public bool Delete(string ID, string CreateUser, ref string msg, string Reason = "")
        {
            try
            {
                ProductRequest paraBody = new ProductRequest();
                paraBody.CreatedUser = CreateUser;
                paraBody.ID = ID;
                paraBody.ReasonDelete = Reason;

                NSLog.Logger.Info("ProductDelete Request: ", paraBody);

                //====================
                var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.ProductAPIDelete, null, paraBody);
                if (result != null)
                {
                    if (result.Success)
                        return true;
                    else
                    {
                        msg = result.Message;
                        NSLog.Logger.Info("ProductDelete", result.Message);
                        return false;
                    }
                }
                else
                {
                    NSLog.Logger.Info("ProductDelete", result);
                    return false;
                }
            }
            catch (Exception e)
            {
                msg = e.ToString();
                NSLog.Logger.Error("ProductDelete_Fail", e);
                return false;
            }
        }

        //public List<DishImportResultItem> ImportPackage(string filePath, FileInfo[] listFileInfo, out int totalRowExel, List<string> storeIndexes, ref string msg)
        //{
        //    totalRowExel = 0;
        //    List<DishImportResultItem> importItems = new List<DishImportResultItem>();

        //    DataTable dtDish = ReadExcelFile(filePath, "Dishes");
        //    DataTable dtTab = ReadExcelFile(filePath, "Tabs");
        //    DataTable dtModifier = ReadExcelFile(filePath, "Modifiers");

        //    string tmpExcelPath = System.Web.HttpContext.Current.Server.MapPath("~/ImportExportTemplate") + "/SBInventoryDish.xlsx";
        //    DataTable dtDishTmp = ReadExcelFile(tmpExcelPath, "Dishes");
        //    DataTable dtTabTmp = ReadExcelFile(tmpExcelPath, "Tabs");
        //    DataTable dtModifierTmp = ReadExcelFile(tmpExcelPath, "Modifiers");

        //    if (dtDish.Columns.Count != dtDishTmp.Columns.Count)
        //    {
        //        msg = Commons._MsgDoesNotMatchFileExcel;
        //        return importItems;
        //    }
        //    if (dtTab.Columns.Count != dtTabTmp.Columns.Count)
        //    {
        //        msg = Commons._MsgDoesNotMatchFileExcel;
        //        return importItems;
        //    }
        //    if (dtModifier.Columns.Count != dtModifierTmp.Columns.Count)
        //    {
        //        msg = Commons._MsgDoesNotMatchFileExcel;
        //        return importItems;
        //    }

        //    List<DishImportItem> dishes = GetListObject<DishImportItem>(dtDish);
        //    List<DishGroupImportItem> groups = GetListObject<DishGroupImportItem>(dtTab);
        //    List<DishModifierImportItem> modifiers = GetListObject<DishModifierImportItem>(dtModifier);
        //    // validate tab Dish, 
        //    ValidateRowDish(ref dishes);
        //    ValidateRowGroupDish(ref groups);
        //    ValidateRowModifier(ref modifiers);

        //    List<ProductModels> listData = new List<ProductModels>();

        //    DishImportResultItem itemErr = null;
        //    bool flagInsert = true;
        //    string msgError = "";

        //    foreach (var store in storeIndexes)
        //    {
        //        foreach (DataRow item in dtDish.Rows)
        //        {
        //            flagInsert = true;
        //            msgError = "";

        //            if (item[0].ToString().Equals(""))
        //                continue;
        //            int index = int.Parse(item[0].ToString());

        //            string ImageUrl = "";
        //            if (!string.IsNullOrEmpty(item[26].ToString()))
        //            {
        //                FileInfo file = listFileInfo.FirstOrDefault(m => m.Name.ToLower() == item[26].ToString().ToLower());
        //                if (file != null)
        //                {
        //                    if (file.Length > Commons._MaxSizeFileUploadImg)
        //                    {
        //                        flagInsert = false;
        //                        msgError = Commons._MsgAllowedSizeImg +"<br/>";
        //                    }
        //                    else
        //                    {
        //                        ImageUrl = Guid.NewGuid() + file.Extension;
        //                        byte[] photoByte = null;
        //                        photoByte = file.ReadFully();
        //                        //Save Image on Server
        //                        if (!string.IsNullOrEmpty(ImageUrl) && photoByte != null)
        //                        {
        //                            var originalDirectory = new DirectoryInfo(string.Format("{0}Uploads\\", System.Web.HttpContext.Current.Server.MapPath(@"\")));
        //                            var path = string.Format("{0}{1}", originalDirectory, ImageUrl);
        //                            MemoryStream ms = new MemoryStream(photoByte, 0, photoByte.Length);
        //                            ms.Write(photoByte, 0, photoByte.Length);
        //                            System.Drawing.Image imageTmp = System.Drawing.Image.FromStream(ms, true);
        //                            ImageHelper.Me.SaveCroppedImage(imageTmp, path, ImageUrl, ref photoByte);
        //                            FTP.Upload(ImageUrl, photoByte);
        //                            ImageHelper.Me.TryDeleteImageUpdated(path);
        //                        }
        //                    }
        //                }
        //            }

        //            List<GroupProductModels> lstGProduct = new List<GroupProductModels>();
        //            //====
        //            DataRow[] GProducs = dtTab.Select("[" + "Dish Index" + "] = " + index + "");
        //            int _sequen = 0;
        //            foreach (DataRow gProduct in GProducs)
        //            {
        //                int tabIndex = int.Parse(gProduct[0].ToString());
        //                //========== Get List ProductOnGroupModels
        //                List<ProductOnGroupModels> ListProOnGroup = new List<ProductOnGroupModels>();
        //                DataRow[] GoProducs = dtModifier.Select("[" + "Tab Index") + "] = " + tabIndex + "";
        //                foreach (DataRow GroupOnProduct in GoProducs)
        //                {

        //                    ProductOnGroupModels goPModel = new ProductOnGroupModels()
        //                    {
        //                        ProductName = GroupOnProduct[3].ToString(),
        //                        ExtraPrice = string.IsNullOrEmpty(GroupOnProduct[4].ToString()) ? 0 : Math.Round(double.Parse(GroupOnProduct[4].ToString()), 2)
        //                    };
        //                    //Validation
        //                    if (string.IsNullOrEmpty(GroupOnProduct[1].ToString()))
        //                    {
        //                        flagInsert = false;
        //                        msgError += "<br/>" + "Seq is required";
        //                    }
        //                    if (string.IsNullOrEmpty(GroupOnProduct[2].ToString()))
        //                    {
        //                        flagInsert = false;
        //                        msgError += "<br/>" + "Tab Index is required";
        //                    }
        //                    if (string.IsNullOrEmpty(goPModel.ProductName))
        //                    {
        //                        flagInsert = false;
        //                        msgError += "<br/>" + "Modifier Name is required";
        //                    }
        //                    if (flagInsert)
        //                    {
        //                        ListProOnGroup.Add(goPModel);
        //                    }
        //                }
        //                //=======
        //                int modifierType = gProduct[6].ToString().Trim().Equals(
        //                                                    Commons.ModifierForced).ToString())
        //                                                ? (byte)Commons.EModifierType.Forced : (byte)Commons.EModifierType.Optional;

        //                GroupProductModels gPModel = new GroupProductModels()
        //                {
        //                    Sequence = int.Parse(gProduct[1].ToString()),
        //                    Name = gProduct[4].ToString(),
        //                    Description = gProduct[5].ToString(),
        //                    Minimum = 0,
        //                    Maximum = int.Parse(gProduct[6].ToString()),
        //                    ListProductOnGroup = ListProOnGroup,
        //                    Type = modifierType
        //                };

        //                //Validation
        //                if (string.IsNullOrEmpty(gProduct[1].ToString()))
        //                {
        //                    flagInsert = false;
        //                    msgError += "<br/>" + "Seq is required";
        //                }
        //                if (string.IsNullOrEmpty(gProduct[2].ToString()))
        //                {
        //                    flagInsert = false;
        //                    msgError += "<br/>" + "Dish Index is required";
        //                }
        //                if (string.IsNullOrEmpty(gPModel.Name))
        //                {
        //                    flagInsert = false;
        //                    msgError += "<br/>" + "Dish Name is required";
        //                }
        //                if (string.IsNullOrEmpty(gProduct[4].ToString()))
        //                {
        //                    flagInsert = false;
        //                    msgError += "<br/>" + "Tab name is required";
        //                }
        //                if (string.IsNullOrEmpty(gProduct[6].ToString()))
        //                {
        //                    flagInsert = false;
        //                    msgError += "<br/>" + "Quantity is required";
        //                }
        //                if (string.IsNullOrEmpty(gProduct[7].ToString()))
        //                {
        //                    flagInsert = false;
        //                    msgError += "<br/>" + "Type is required";
        //                }
        //                if (flagInsert)
        //                {
        //                    lstGProduct.Add(gPModel);
        //                }
        //            }

        //            int defaulStatus = 1;
        //            string dStatus = item[28].ToString();
        //            if (dStatus.Equals(Commons.EItemState.PendingStatus.ToString()))
        //            {
        //                defaulStatus = (int)Commons.EItemState.PendingStatus;
        //            }
        //            else if (dStatus.Equals(Commons.EItemState.CompleteStatus.ToString()))
        //            {
        //                defaulStatus = (int)Commons.EItemState.CompleteStatus;
        //            }
        //            else if (dStatus.Equals(Commons.EItemState.ServedStatus.ToString()))
        //            {
        //                defaulStatus = (int)Commons.EItemState.ServedStatus;
        //            }

        //            string msgItem = "";
        //            DateTime ExpiredDate = DateTime.Now;
        //            if (!item[21].ToString().Equals("Unlimited"))
        //            {
        //                ExpiredDate = DateTimeHelper.GetDateImport(item[21].ToString(), ref msgItem);
        //                if (!msgItem.Equals(""))
        //                {
        //                    flagInsert = false;
        //                    msgError = msgItem);
        //                }
        //            }
        //            else
        //            {
        //                ExpiredDate = Commons._ExpiredDate;
        //            }
        //            ProductModels model = new ProductModels();
        //            _sequen = 0;
        //            if (!string.IsNullOrEmpty(item[1].ToString()))
        //                int.TryParse(item[1].ToString(), out _sequen);
        //            model.Index = index.ToString();
        //            model.OrderByIndex = _sequen;
        //            model.Name = item[2].ToString();
        //            model.ProductCode = item[3].ToString();
        //            model.BarCode = item[4].ToString();
        //            model.IsAllowedDiscount = string.IsNullOrEmpty(item[5].ToString()) ? true : bool.Parse(item[5].ToString().Equals("Yes")).ToString() ? true : false;

        //            model.IsCheckedStock = string.IsNullOrEmpty(item[6].ToString()) ? false : bool.Parse(item[6].ToString().Equals("Yes")).ToString() ? true : false;
        //            model.Description = item[7].ToString();

        //            model.PrintOutText = item[9].ToString();
        //            model.Cost = item[11].ToString().Equals("") ? 0 : double.Parse(item[11].ToString());
        //            model.Unit = item[12].ToString().Equals("") ? 0 : int.Parse(item[12].ToString());

        //            model.Measure = item[13].ToString().Equals("") ? "$" : item[13].ToString();

        //            model.DefaultPrice = item[14].ToString().Equals("") ? 0 : Math.Round(double.Parse(item[14].ToString()), 2);

        //            model.Quantity = item[15].ToString().Equals("") ? 0 : double.Parse(item[15].ToString());
        //            model.Limit = item[16].ToString().Equals("") ? 0 : int.Parse(item[16].ToString());

        //            model.CategoryName = item[17].ToString();
        //            model.IsActive = string.IsNullOrEmpty(item[18].ToString() ? true
        //                : bool.Parse(item[18].ToString().Equals("Yes")).ToString() ? true : false;

        //            model.IsAllowedOpenPrice = string.IsNullOrEmpty(item[19].ToString()) ? false : bool.Parse(item[19].ToString().Equals("Yes")).ToString() ? true : false;

        //            model.IsPrintedOnCheck = string.IsNullOrEmpty(item[20].ToString()) ? true : bool.Parse(item[20].ToString().Equals("Yes")).ToString() ? true : false;


        //            model.ExpiredDate = string.IsNullOrEmpty(item[21].ToString()) ? Commons._ExpiredDate : ExpiredDate;

        //            model.HasServiceCharge = string.IsNullOrEmpty(item[22].ToString()) ? true : bool.Parse(item[22].ToString().Equals("Yes")).ToString() ? true : false;

        //            model.ServiceCharge = item[23].ToString().Equals("") ? 0 : double.Parse(item[23].ToString());

        //            model.IsForce = bool.Parse(item[24].ToString().Equals("Yes")).ToString()) ? true : false;

        //            model.StoreID = store;
        //            model.ImageURL = ImageUrl;
        //            model.Printer = item[27].ToString();

        //            model.DefaultState = string.IsNullOrEmpty(dStatus) ? (byte)Commons.EItemState.PendingStatus : (byte)defaulStatus;

        //            model.IsComingSoon = string.IsNullOrEmpty(item[29].ToString()) ? false : bool.Parse(item[29].ToString().Equals("Yes")).ToString() ? true : false;

        //            model.IsShowMessage = string.IsNullOrEmpty(item[30].ToString()) ? false : bool.Parse(item[30].ToString().Equals("Yes")).ToString() ? true : false;

        //            model.Info = item[31].ToString();
        //            model.IsAddition = string.IsNullOrEmpty(item[33].ToString() ? false : bool.Parse(item[33].ToString().Equals("Yes")).ToString() ? true : false;

        //            model.Message = item[36].ToString();
        //            model.ListGroup = lstGProduct;

        //            model.ColorCode = string.IsNullOrEmpty(item[34].ToString()) ? "#000000" : item[34].ToString();
        //            model.ProductTypeCode = (byte)Commons.EProductType.Dish;

        //            string sPOS = item[35].ToString();
        //            if (!string.IsNullOrEmpty(sPOS))
        //            {
        //                foreach (var itemPOS in sPOS.Split(';'))
        //                {
        //                    if (!itemPOS.Equals("All"))
        //                    {
        //                        model.ListProductSeason.Add(new ProductSeasonDTO
        //                        {
        //                            SeasonName = itemPOS,
        //                            IsPOS = true
        //                        });
        //                    }
        //                }
        //            }
        //            if (!string.IsNullOrEmpty(item[27].ToString()))
        //            {
        //                foreach (var itemPrinter in item[27].ToString().Split(';'))
        //                {
        //                    model.ListPrinter.Add(new PrinterOnProductModels
        //                    {
        //                        PrinterName = itemPrinter
        //                    });
        //                }
        //            }
        //            //============
        //            if (string.IsNullOrEmpty(model.Name))
        //            {
        //                flagInsert = false;
        //                msgItem = "Dish Name is required");
        //                msgError += "<br/>" + msgItem;
        //            }
        //            if (string.IsNullOrEmpty(model.ProductCode))
        //            {
        //                flagInsert = false;
        //                msgItem = "Dish Code is required");
        //                msgError += "<br/>" + msgItem;
        //            }
        //            if (model.ServiceCharge < 0 || model.ServiceCharge > 100)
        //            {
        //                flagInsert = false;
        //                msgItem = "Service Charge must between 0 and 100");
        //                msgError += "<br/>" + msgItem;
        //            }
        //            if (model.OrderByIndex < 0)
        //            {
        //                flagInsert = false;
        //                msgItem = "Please enter a value Sequence larger or equal to 0");
        //                msgError += "<br/>" + msgItem;
        //            }
        //            if (model.Quantity < 0)
        //            {
        //                flagInsert = false;
        //                msgItem = "Please enter a value Quantity larger or equal to 0");
        //                msgError += "<br/>" + msgItem;
        //            }
        //            if (model.DefaultPrice < 0)
        //            {
        //                flagInsert = false;
        //                msgItem = "Please enter a value Default Price larger or equal to 0");
        //                msgError += "<br/>" + msgItem;
        //            }
        //            if (model.Cost < 0)
        //            {
        //                flagInsert = false;
        //                msgItem = "Please enter a value Cost larger or equal to 0");
        //                msgError += "<br/>" + msgItem;
        //            }
        //            if (model.Limit < 0)
        //            {
        //                flagInsert = false;
        //                msgItem = "Please enter a value Limit larger or equal to 0");
        //                msgError += "<br/>" + msgItem;
        //            }
        //            if (model.Unit < 0)
        //            {
        //                flagInsert = false;
        //                msgItem = "Please enter a value Unit larger or equal to 0");
        //                msgError += "<br/>" + msgItem;
        //            }
        //            if (model.Cost > model.DefaultPrice)
        //            {
        //                flagInsert = false;
        //                msgItem = "Default Price must larger than cost");
        //                msgError += "<br/>" + msgItem;
        //            }
        //            if (string.IsNullOrEmpty(model.CategoryName))
        //            {
        //                flagInsert = false;
        //                msgItem = "Please re-enter Category for this dish");
        //                msgError += "<br/>" + msgItem;
        //            }
        //            //==========
        //            if (flagInsert)
        //            {
        //                listData.Add(model);
        //            }
        //            else
        //            {
        //                DishErrorItem itemerr = new DishErrorItem();
        //                itemerr.GroupName = model.Index;
        //                itemerr.ErrorMessage = "Row") +":" + index + msgError;

        //                itemErr = new DishImportResultItem();
        //                itemErr.Name = model.Name;
        //                itemErr.ListFailStoreName.Add("");
        //                itemErr.ErrorItems.Add(itemerr);
        //                importItems.Add(itemErr);
        //            }
        //        }
        //    }

        //    //try
        //    //{
        //    ProductApiModels paraBody = new ProductApiModels();
        //    paraBody.CreatedUser = Commons.CreateUser;
        //    paraBody.ListProduct = listData;

        //    //====================
        //    var result = (ResponseApiModels)ApiResponse.Post<ResponseApiModels>(Commons.ProductAPIImport, null, paraBody);
        //    if (result != null)
        //    {
        //        dynamic data = result.Data;
        //        var lstC = data["ListProperty"];
        //        var lstContent = JsonConvert.SerializeObject(lstC);
        //        var listError = JsonConvert.DeserializeObject<List<ImportResult>>(lstContent);

        //        //=====
        //        DishImportResultItem importItem = new DishImportResultItem();
        //        importItem.Name = "<strong style=\"color: #d9534f;\">" + "Have been" + " [" + (listData.Count - listError.Count) + "] " + "row(s) import Successful" + "<strong>";
        //        importItems.Insert(0, importItem);
        //        //=====End
        //        foreach (ImportResult itemError in listError)
        //        {
        //            DishErrorItem item = new DishErrorItem();
        //            item.GroupName = itemError.Index;
        //            item.ErrorMessage = "Row" + ": " + itemError.Index + "<br/>" + itemError.Error;

        //            importItem = new DishImportResultItem();
        //            importItem.Name = itemError.Property;
        //            importItem.ListFailStoreName.Add(itemError.StoreName);
        //            importItem.ErrorItems.Add(item);
        //            importItems.Add(importItem);
        //        }
        //        if (importItems.Count == 0)
        //        {
        //            importItem = new DishImportResultItem();
        //            importItem.Name = "Dishes";
        //            importItem.ListSuccessStoreName.Add("Import Dish Successful");
        //            importItems.Add(importItem);
        //        }
        //    }
        //    return importItems;
        //}

        //public StatusResponse ExportPackage(ref IXLWorksheet ws, List<string> lstStore)
        //{
        //    StatusResponse Response = new StatusResponse();
        //    try
        //    {
        //        List<ProductDetailModels> listData = new List<ProductDetailModels>();
        //        ProductRequest paraBody = new ProductRequest();
        //        paraBody.CreatedUser = Commons.CreateUser;
        //        var result = (NSApiResponse)ApiResponse.Post<NSApiResponse>(Commons.ExportProduct, null, paraBody);
        //        dynamic data = result.Data;
        //        var lstDataRaw = data["ListProduct"];
        //        var lstObject = JsonConvert.SerializeObject(lstDataRaw);
        //        listData = JsonConvert.DeserializeObject<List<ProductDetailModels>>(lstObject);
        //        int cols = 8;
        //        //Header
        //        int row = 1;
        //        ws.Cell("A" + row).Value = "Index";
        //        ws.Cell("B" + row).Value = "Product Name";
        //        ws.Cell("C" + row).Value = "Status";
        //        ws.Cell("D" + row).Value = "Product Type";
        //        ws.Cell("E" + row).Value = "Amount";
        //        ws.Cell("F" + row).Value = "Allow Open Product";
        //        ws.Cell("G" + row).Value = "Description";
        //        //Item
        //        row = 2;
        //        int countIndex = 1;
        //        foreach (var item in listData)
        //        {
        //            ws.Cell("A" + row).Value = countIndex;
        //            ws.Cell("B" + row).Value = item.Name;
        //            ws.Cell("C" + row).Value = item.IsActive ? "Active" : "InActive";
        //            //ws.Cell("D" + row).Value = item.Type == (byte)Commons.EValueType.Currency ? "Currency" : "Percent";
        //            ws.Cell("E" + row).Value = item.Value;
        //            //ws.Cell("F" + row).Value = item.IsAllowOpenValue ? "Yes" : "No";
        //            ws.Cell("G" + row).Value = item.Description;
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
