using NSCSC.Shared;
using NSCSC.Shared.Factory;
using NSCSC.Shared.Factory.ClientSite;
using NSCSC.Shared.Factory.CRM;
using NSCSC.Shared.Factory.Settings.PaymentMethods;
using NSCSC.Shared.Models.ClientSite.MyProfile;
using NSCSC.Shared.Models.CRM.Customers;
using NSCSC.Shared.Models.CRM.Customers.ProductManagement;
using NSCSC.Shared.Models.CRM.OrderReceiptManagement;
using NSCSC.Shared.Models.Settings.PaymentMethods;
using NSCSC.Shared.Utilities;
using NSCSC.Web.App_Start;
using NSCSC.Payments.Paypal.DTO;
using NSCSC.Payments.Paypal.Request;
using NuWebNCloud.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using static NSCSC.Shared.Models.CRM.Customers.StoreDetailModels;
using NSCSC.Shared.Factory.SandBox.Inventory;
using NSCSC.Shared.Models.SandBox.Inventory.Product;

namespace NSCSC.Web.Controllers
{
    [NuAuth]
    public class CRMCustomersController : HQController
    {
        private CustomerFactory _factory = null;
        private BaseFactory _baseFactory = null;
        private CheckIpAddressFactory _checkIpFactory = null;
        private PaymentMethodFactory _factoryPayment = null;
        private ProductFactory _productFactory = null;

        public CRMCustomersController()
        {
            _factory = new CustomerFactory();
            _baseFactory = new BaseFactory();
            _checkIpFactory = new CheckIpAddressFactory();
            _factoryPayment = new PaymentMethodFactory();
            _productFactory = new ProductFactory();
            ViewBag.ListCountry = _baseFactory.GetListCountry();
            ViewBag.ListCountry = ViewBag.ListCountry ?? "";
        }

        // GET: CRMCustomers
        public ActionResult Index()
        {
            try
            {
                CustomerViewModels model = new CustomerViewModels();
                //List<SelectListItem> LstReseller = new List<SelectListItem>();
                //var data = _factory.GetListData().Where(o => o.CustomerType == (byte)Commons.ECustomerType.Reseller).ToList();
                //if (data != null)
                //{
                //    foreach (var item in data)
                //    {
                //        if (item.ListCus != null && item.ListCus.Any())
                //        {
                //            item.ListCus.ForEach(o =>
                //            {
                //                LstReseller.Add(new SelectListItem()
                //                {
                //                    Value = o.ID,
                //                    Text = o.Name
                //                });
                //            });
                //            model.ListCusType.AddRange(LstReseller);
                //        }
                //    }
                //}
                return View(model);
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("CustomerAdditionIndex: ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }

        public ActionResult Search(CustomerViewModels model)
        {
            try
            {
                var data = _factory.GetListData();
                if (data != null && data.Any())
                {
                    #region
                    //foreach (var item in data)
                    //{

                    //    if (item.CustomerType == (byte)Commons.ECustomerType.Consumer && item.ListCus != null && item.ListCus.Any())
                    //    {
                    //        item.ListCus.ForEach(o =>
                    //        {
                    //            o.CustomerType = item.CustomerType;
                    //
                    //            if (!string.IsNullOrEmpty(o.Phone) && o.Phone.Length > 4)
                    //            {
                    //                string tabs = new String('*', o.Phone.Length - 4);
                    //                o.Phone = tabs + o.Phone.Substring(o.Phone.Length - 4, 4);
                    //            }
                    //            //else
                    //            //    item.Phone = "********";
                    //            if (!string.IsNullOrEmpty(o.Email) && o.Email.Length > 4)
                    //            {
                    //                string tabs = new String('*', o.Email.Length - 4);
                    //                o.Email = o.Email.Substring(0, 4) + tabs;
                    //            }
                    //            //else
                    //            //    item.Email = "********";

                    //            if (string.IsNullOrEmpty(o.Name))
                    //            {
                    //                o.Name = o.Email;
                    //            }
                    //        });

                    //    }
                    //    else
                    //    {
                    //        if (item.CustomerType == (byte)Commons.ECustomerType.Reseller)
                    //        {
                    //            //model.ListItem.Add(item);
                    //            if (item.ListCus != null && item.ListCus.Any())
                    //            {
                    //                item.ListCus.ForEach(o =>
                    //                {
                    //                    o.CustomerType = item.CustomerType;                    //                
                    //                    if (!string.IsNullOrEmpty(o.Phone) && o.Phone.Length > 4)
                    //                    {
                    //                        string tabs = new String('*', o.Phone.Length - 4);
                    //                        o.Phone = tabs + o.Phone.Substring(o.Phone.Length - 4, 4);
                    //                    }
                    //                    //else
                    //                    //    item.Phone = "********";
                    //                    if (!string.IsNullOrEmpty(o.Email) && o.Email.Length > 4)
                    //                    {
                    //                        string tabs = new String('*', o.Email.Length - 4);
                    //                        o.Email = o.Email.Substring(0, 4) + tabs;
                    //                    }
                    //                    //else
                    //                    //    item.Email = "********";

                    //                    if (string.IsNullOrEmpty(o.Name))
                    //                    {
                    //                        o.Name = o.Email;
                    //                    }
                    //                });
                    //                model.ListItem.AddRange(item.ListCus);

                    //            }
                    //                              //            model.ListItem.Add(item);
                    //        }
                    //    }
                    //}
                    #endregion

                    #region
                  
                    if (model.CustomerType != (byte)Commons.ECustomerType.Customer)
                    {
                        data = data.Where(x => x.CustomerType == model.CustomerType).ToList();
                    }

                    model.ListItem = data;
                    model.ListItem.ForEach(x =>
                    {
                        x.Phone = (!string.IsNullOrEmpty(x.Phone) && x.Phone.Length > 4) ? new string('*', x.Phone.Length - 4) + x.Phone.Substring(x.Phone.Length - 4, 4) : "********";
                        x.Email = (!string.IsNullOrEmpty(x.Email) && x.Email.Length > 4) ? x.Email.Substring(0, 4) + new String('*', x.Email.Length - 4) : "********";
                    });
                    foreach (var item in model.ListItem)
                    {
                        item.CountItem = "(" + (item.ListCus.Count > 1 ? item.ListCus.Count + " " + "Customers" : item.ListCus.Count + " " + "Customer") + ")";
                        item.Type = item.Type = (item.CustomerType == (byte)Commons.ECustomerType.Consumer ? Commons.ECustomerType.Consumer.ToString() : item.CustomerType == (byte)Commons.ECustomerType.Reseller ? Commons.ECustomerType.Reseller.ToString() : "") + " ";

                        if (item.ListCus != null && item.ListCus.Count > 0)
                        {
                            item.ListCus = item.ListCus.OrderBy(x => x.Name).ToList();
                            item.ListCus.ForEach(x =>
                            {
                                x.Phone = (!string.IsNullOrEmpty(x.Phone) && x.Phone.Length > 4) ? new string('*', x.Phone.Length - 4) + x.Phone.Substring(x.Phone.Length - 4, 4) : "********";
                                x.Email = (!string.IsNullOrEmpty(x.Email) && x.Email.Length > 4) ? x.Email.Substring(0, 4) + new String('*', x.Email.Length - 4) : "********";
                            });
                        }
                    }
                    #endregion


                }
                else
                    model = new CustomerViewModels();
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("Search : ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
            //return PartialView("_ListData", model);
            return PartialView("_ListData", model);
        }

        public ActionResult Create()
        {
            try
            {
                CustomerBaseModels model = new CustomerBaseModels();
                model.CustomerDetail.ListAppProduct = _productFactory.GetListData("", (byte)Commons.EProductType.Product);
                if (model.CustomerDetail.ListAppProduct != null && model.CustomerDetail.ListAppProduct.Any())
                {
                    int _count = 0;
                    model.CustomerDetail.ListAppProduct.ForEach(o =>
                    {
                        o.OffSet = _count;
                        _count++;
                    });
                }
                //  var IpAddress = _checkIpFactory.GetIPAddress();
                //var CountryCode = _checkIpFactory.GetCountryCode();
                if (ViewBag.ListCountry != null)
                {
                    var ListCountry = ViewBag.ListCountry as List<CountryModel>;
                    var objCountry = ListCountry.Where(x => x.Alpha2Code.Equals(CurrentCountryCode)).FirstOrDefault();
                    model.CustomerDetail.HomeCountry = objCountry != null ? objCountry.Name : "";
                    model.CustomerDetail.OfficeCountry = objCountry != null ? objCountry.Name : "";
                }
                // model.CustomerDetail.HomeCountry = model.CustomerDetail.ListCounTry.Where(o => o.Alpha2Code == CountryCode).Select(s => s.Name).FirstOrDefault();
                //   model.CustomerDetail.OfficeCountry = model.CustomerDetail.ListCounTry.Where(o => o.Alpha2Code == CountryCode).Select(s => s.Name).FirstOrDefault();

                return View(model);
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("CreateCustomer: ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }

        /*============ Customer Detail*/
        [HttpPost]
        public ActionResult CreateCustomerDetail(CustomerBaseModels model)
        {
            try
            {
                //if (model.CustomerDetail.Birthday > model.CustomerDetail.Anniversary)
                //{
                //    ModelState.AddModelError("CustomerDetail.Birthday", "Birthday Date must be less than Anniversary Date.");
                //}
                model.CustomerDetail.MaritalStatus = model.CustomerDetail.ClientMaritalStatus == 1 ? true : false;
                if (!ModelState.IsValid)
                {
                    if ((ModelState["CustomerDetail.PictureUpload"]) != null && (ModelState["CustomerDetail.PictureUpload"]).Errors.Count > 0)
                        model.CustomerDetail.ImageURL = "";
                    model.CustomerDetail.ImageURL = Commons.Image100_100;
                    return View("Create", model);
                }

                byte[] photoByte = null;
                if (model.CustomerDetail.PictureUpload != null && model.CustomerDetail.PictureUpload.ContentLength > 0)
                {
                    Byte[] imgByte = new Byte[model.CustomerDetail.PictureUpload.ContentLength];
                    model.CustomerDetail.PictureUpload.InputStream.Read(imgByte, 0, model.CustomerDetail.PictureUpload.ContentLength);
                    model.CustomerDetail.PictureByte = imgByte;
                    model.CustomerDetail.ImageURL = Guid.NewGuid() + Path.GetExtension(model.CustomerDetail.PictureUpload.FileName);
                    model.CustomerDetail.PictureUpload = null;
                    photoByte = imgByte;
                }

                //For list product
                if (model.CustomerDetail.ListProduct != null && model.CustomerDetail.ListProduct.Any())
                {
                    model.CustomerDetail.ListProduct = model.CustomerDetail.ListProduct.Where(o => o.Status != (byte)Commons.EStatus.Deleted).ToList();
                }
                //============
                model.CustomerDetail.CreateUser = CurrentUser.UserId;
                string msg = "";
                var tmp = model.CustomerDetail.PictureByte;
                model.CustomerDetail.PictureByte = null;
                bool result = _factory.CreateOrEdit(model.CustomerDetail, ref msg);
                if (result)
                {
                    model.CustomerDetail.PictureByte = tmp;
                    if (!string.IsNullOrEmpty(model.CustomerDetail.ImageURL) && model.CustomerDetail.PictureByte != null)
                    {
                        var originalDirectory = new DirectoryInfo(string.Format("{0}Uploads\\", Server.MapPath(@"\")));
                        var path = string.Format("{0}{1}", originalDirectory, model.CustomerDetail.ImageURL);
                        MemoryStream ms = new MemoryStream(photoByte, 0, photoByte.Length);
                        ms.Write(photoByte, 0, photoByte.Length);
                        System.Drawing.Image imageTmp = System.Drawing.Image.FromStream(ms, true);
                        ImageHelper.Me.SaveCroppedImage(imageTmp, path, model.CustomerDetail.ImageURL, ref photoByte);
                        model.CustomerDetail.PictureByte = photoByte;
                        FTP.Upload(model.CustomerDetail.ImageURL, model.CustomerDetail.PictureByte);
                        ImageHelper.Me.TryDeleteImageUpdated(path);
                    }
                    return RedirectToAction("Index");
                }
                else
                {
                    if (msg.Equals("Duplicate email."))
                    {
                        ModelState.AddModelError("CustomerDetail.Email", "The Email is existed. Please try again!");
                    }
                    else
                    {
                        ModelState.AddModelError("CustomerDetail.Name", Commons.ErrorMsg /*msg*/);
                    }
                    //return RedirectToAction("Create");
                    return View("Create", model);
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("CreateCustomerDetail: ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }

        public NSCSC.Shared.Models.CRM.Customers.CustomerDetailModels GetDetail(string id)
        {
            try
            {
                NSCSC.Shared.Models.CRM.Customers.CustomerDetailModels model = _factory.GetDetail(id);
                if (model != null)
                {
                    if (model.ListReceipt != null && model.ListReceipt.Count > 0)
                    {
                        model.ListReceipt.ForEach(
                            x =>
                            {
                                //x.PaidByMethod = new List<string>(new string[] { "element1", "element2", "element3" });
                                x.PaidByMethodText = (x.PaidByMethod != null && x.PaidByMethod.Count > 0) ? string.Join(", ", x.PaidByMethod) : "";
                            }
                            );
                    }

                    if (model.MaritalStatus)
                        model.ClientMaritalStatus = 1;
                }
                return model;
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("CustomerDetail: ", ex);
                return null;
            }
        }

        [HttpGet]
        public new PartialViewResult View(string id)
        {
            CustomerBaseModels model = new CustomerBaseModels();
            model.CustomerDetail = GetDetail(id);
            var Merchant = GetDetailMerchant(id);
            if (Merchant.CustomerID == null)
                Merchant.CustomerID = model.CustomerDetail.ID;
            model.MerchantStore = Merchant;
            //========
            model.ProductManagement = _factory.GetListDataProductsManagement(id);
            if (model.ProductManagement != null)
            {
                //=========ListHardwareService
                if (model.ProductManagement.ListHardwareService != null && model.ProductManagement.ListHardwareService.Count > 0)
                {
                    model.ProductManagement.ListHardwareService.ForEach(x =>
                    {
                        x.Status = ((Commons.EServiceAssetStatus)Enum.ToObject(typeof(Commons.EServiceAssetStatus), x.State)).ToString();
                        x.sAdditionType = ((Commons.EAdditionType)Enum.ToObject(typeof(Commons.EAdditionType), x.AdditionType)).ToString();
                        if (x.AdditionType == (byte)Commons.EAdditionType.Service)
                            x.SerialNo = "";
                        else
                        {
                            if (x.State == (byte)Commons.EServiceAssetStatus.New)
                                x.SerialNo = "Updating";
                        }
                    });
                }
                //=========ListProductPackage
                if (model.ProductManagement.ListProductPackage != null && model.ProductManagement.ListProductPackage.Count > 0)
                {
                    model.ProductManagement.ListProductPackage.ForEach(x =>
                    {
                        x.sType = ((Commons.EProductType)Enum.ToObject(typeof(Commons.EProductType), x.Type)).ToString();
                        //if (!x.ExpiryDate.HasValue)
                        //    x.Status = Commons.EStatusAccountFunction.Inactive.ToString();
                        //else
                        //{
                        //    DateTime _curDate = DateTime.Now;
                        //    if (x.ExpiryDate < _curDate)
                        //        x.Status = Commons.EStatusAccountFunction.Expired.ToString();
                        //    else
                        //        x.Status = x.IsActive ? Commons.EStatusAccountFunction.Active.ToString() : Commons.EStatusAccountFunction.Inactive.ToString();
                        //}

                        if (x.IsActive)
                        {
                            //var minDate = new DateTime(1900, 01, 01, 12, 00, 00);
                            //if (!x.ExpiryDate.Equals(minDate))
                            if (DateTime.Compare(x.ExpiryDate.Value.Date, Commons.MinDate.Date) > 0)
                            {
                                DateTime _curDate = DateTime.Now;
                                if (x.ExpiryDate.Value < _curDate)
                                    x.Status = Commons.EStatusAccountFunction.Expired.ToString();
                                else
                                    x.Status = Commons.EStatusAccountFunction.Active.ToString();
                            }
                            else
                                x.Status = Commons.EStatusAccountFunction.Active.ToString();
                        }
                        else
                        {
                            x.Status = Commons.EStatusAccountFunction.Inactive.ToString();
                        }
                        //=======
                        if (!x.ActivateTime.HasValue)
                            x.sActivateTime = "Not Yet Activated";
                        else
                            x.sActivateTime = x.ActivateTime.Value.ToString(Commons.FormatDateTime);
                        //=======
                        if (!x.ExpiryDate.HasValue || DateTime.Compare(x.ExpiryDate.Value.Date, Commons.MinDate.Date) <= 0)
                            x.sExpiryDate = "";
                        else
                            x.sExpiryDate = x.ExpiryDate.Value.ToString(Commons.FormatDateTime);
                        //======
                        if (x.PeriodType == (byte)Commons.EPeriodType.OneTime)
                            x.sExpiryDate = "Never";
                    });
                }
                //=========ListAccountManagement
                if (model.ProductManagement.ListAccount != null && model.ProductManagement.ListAccount.Count > 0)
                {
                    model.ProductManagement.ListAccount.ForEach(x =>
                    {
                        if (!x.ExpiryDate.HasValue)
                            x.Status = Commons.EStatusAccountFunction.Inactive.ToString();
                        else
                        {
                            DateTime _curDate = DateTime.Now;
                            if (DateTime.Compare(x.ExpiryDate.Value.Date, Commons.MinDate.Date) > 0 && x.ExpiryDate.Value < _curDate)
                                x.Status = Commons.EStatusAccountFunction.Expired.ToString();
                            else
                                x.Status = x.IsActive ? Commons.EStatusAccountFunction.Active.ToString() : Commons.EStatusAccountFunction.Inactive.ToString();
                        }

                        //======
                        if (x.PeriodType == (byte)Commons.EPeriodType.OneTime)
                            x.sExpiryDate = "Never";
                        else
                        {
                            if (x.ExpiryDate.HasValue && DateTime.Compare(x.ExpiryDate.Value.Date, Commons.MinDate.Date) > 0)
                                x.sExpiryDate = x.ExpiryDate.Value.ToString(Commons.FormatDateTime);
                            else
                                x.sExpiryDate = "";
                        }
                        //======
                        if (x.ActivateTime.HasValue)
                            x.sActivateTime = x.ActivateTime.Value.ToString(Commons.FormatDateTime);
                        else
                            x.sActivateTime = "Not Yet Activated";
                    });
                }
                //=========List Additional Functions Management
                if (model.ProductManagement.ListFunction != null && model.ProductManagement.ListFunction.Count > 0)
                {
                    model.ProductManagement.ListFunction.ForEach(x =>
                    {
                        //if (!x.ExpiryDate.HasValue)
                        //    x.Status = Commons.EStatusAccountFunction.Inactive.ToString();
                        //else
                        //{
                        //    DateTime _curDate = DateTime.Now;
                        //    if (DateTime.Compare(x.ExpiryDate.Value.Date, Commons.MinDate.Date) > 0 && x.ExpiryDate.Value < _curDate)
                        //        x.Status = Commons.EStatusAccountFunction.Expired.ToString();
                        //    else
                        //    {
                        //        if (x.IsBlock)
                        //            x.Status = Commons.EStatusAccountFunction.Blocked.ToString();
                        //        else
                        //            x.Status = x.IsActive ? Commons.EStatusAccountFunction.Active.ToString() : Commons.EStatusAccountFunction.Inactive.ToString();
                        //    }

                        //}
                        ////==========
                        //if (x.PeriodType == (byte)Commons.EPeriodType.OneTime)
                        //    x.sExpiryDate = "Never";
                        //else
                        //{
                        //    if (x.ExpiryDate.HasValue && DateTime.Compare(x.ExpiryDate.Value.Date, Commons.MinDate.Date) > 0)
                        //        x.sExpiryDate = x.ExpiryDate.Value.ToString(Commons.FormatDateTime);
                        //    else
                        //        x.sExpiryDate = "";
                        //}

                        //if (x.ActivateTime.HasValue)
                        //    x.sActivateTime = x.ActivateTime.Value.ToString(Commons.FormatDateTime);
                        //else
                        //    x.sActivateTime = "Not Yet Activated";

                        x = GetAdditionFunctionInfoView(x);
                    });
                }
            }
            return PartialView("_View", model);
        }

        public PartialViewResult Edit(string id)
        {
            CustomerBaseModels model = new CustomerBaseModels();
            model.CurrencySymbol = CurrentUser.CurrencySymbol;
            model.CustomerDetail = GetDetail(id);
            var Merchant = GetDetailMerchant(id);
            if (Merchant.CustomerID == null)
                Merchant.CustomerID = model.CustomerDetail.ID;
            model.MerchantStore = Merchant;
            //========
            model.ProductManagement = _factory.GetListDataProductsManagement(id);
            if (model.ProductManagement != null)
            {
                //=========ListHardwareService
                if (model.ProductManagement.ListHardwareService != null && model.ProductManagement.ListHardwareService.Count > 0)
                {
                    model.ProductManagement.ListHardwareService.ForEach(x =>
                    {
                        x.Status = ((Commons.EServiceAssetStatus)Enum.ToObject(typeof(Commons.EServiceAssetStatus), x.State)).ToString();
                        x.sAdditionType = ((Commons.EAdditionType)Enum.ToObject(typeof(Commons.EAdditionType), x.AdditionType)).ToString();
                        if (x.AdditionType == (byte)Commons.EAdditionType.Service)
                            x.SerialNo = "";
                        else
                        {
                            if (x.State == (byte)Commons.EServiceAssetStatus.New)
                                x.SerialNo = "Updating";
                        }
                    });
                }
                //=========ListProductPackage
                if (model.ProductManagement.ListProductPackage != null && model.ProductManagement.ListProductPackage.Count > 0)
                {
                    model.ProductManagement.ListProductPackage.ForEach(x =>
                    {

                        x.sType = ((Commons.EProductType)Enum.ToObject(typeof(Commons.EProductType), x.Type)).ToString();
                        if (x.IsActive)
                        {
                            //var minDate = new DateTime(1900, 01, 01, 12, 00, 00);
                            //if (!x.ExpiryDate.Equals(minDate))
                            if (DateTime.Compare(x.ExpiryDate.Value.Date, Commons.MinDate.Date) > 0)
                            {
                                DateTime _curDate = DateTime.Now;
                                if (x.ExpiryDate.Value < _curDate)
                                    x.Status = Commons.EStatusAccountFunction.Expired.ToString();
                                else
                                    x.Status = Commons.EStatusAccountFunction.Active.ToString();
                            }
                            else
                                x.Status = Commons.EStatusAccountFunction.Active.ToString();
                        }
                        else
                        {
                            x.Status = Commons.EStatusAccountFunction.Inactive.ToString();
                        }
                        if (!x.ActivateTime.HasValue)
                            x.sActivateTime = "Not Yet Activated";
                        else
                            x.sActivateTime = x.ActivateTime.Value.ToString(Commons.FormatDateTime);
                        //=======
                        if (!x.ExpiryDate.HasValue || DateTime.Compare(x.ExpiryDate.Value.Date, Commons.MinDate.Date) <= 0)
                            x.sExpiryDate = "";
                        else
                            x.sExpiryDate = x.ExpiryDate.Value.ToString(Commons.FormatDateTime);
                        //======
                        if (x.PeriodType == (byte)Commons.EPeriodType.OneTime)
                            x.sExpiryDate = "Never";
                        //}

                    });
                }
                //=========ListAccountManagement
                if (model.ProductManagement.ListAccount != null && model.ProductManagement.ListAccount.Count > 0)
                {
                    model.ProductManagement.ListAccount.ForEach(x =>
                    {
                        if (!x.ExpiryDate.HasValue)
                            x.Status = Commons.EStatusAccountFunction.Inactive.ToString();
                        else
                        {
                            DateTime _curDate = DateTime.Now;
                            if (DateTime.Compare(x.ExpiryDate.Value.Date, Commons.MinDate.Date) > 0 && x.ExpiryDate.Value < _curDate)
                                x.Status = Commons.EStatusAccountFunction.Expired.ToString();
                            else
                                x.Status = x.IsActive ? Commons.EStatusAccountFunction.Active.ToString() : Commons.EStatusAccountFunction.Inactive.ToString();
                        }

                        //======
                        if (x.PeriodType == (byte)Commons.EPeriodType.OneTime)
                            x.sExpiryDate = "Never";
                        else
                        {
                            if (x.ExpiryDate.HasValue && DateTime.Compare(x.ExpiryDate.Value.Date, Commons.MinDate.Date) > 0)
                                x.sExpiryDate = x.ExpiryDate.Value.ToString(Commons.FormatDateTime);
                            else
                                x.sExpiryDate = "";
                        }
                        //======
                        if (x.ActivateTime.HasValue)
                            x.sActivateTime = x.ActivateTime.Value.ToString(Commons.FormatDateTime);
                        else
                            x.sActivateTime = "Not Yet Activated";
                    });
                }
                //=========List Additional Functions Management
                if (model.ProductManagement.ListFunction != null && model.ProductManagement.ListFunction.Count > 0)
                {
                    model.ProductManagement.ListFunction.ForEach(x =>
                    {
                        ////if (!x.ExpiryDate.HasValue)
                        ////    x.Status = Commons.EStatusAccountFunction.Inactive.ToString();
                        ////else
                        ////{
                        //    DateTime _curDate = DateTime.Now;
                        //    if (DateTime.Compare(x.ExpiryDate.Value.Date, Commons.MinDate.Date) > 0 && x.ExpiryDate.Value < _curDate)
                        //        x.Status = Commons.EStatusAccountFunction.Expired.ToString();
                        //    else
                        //    {
                        //        if (x.IsBlock)
                        //            x.Status = Commons.EStatusAccountFunction.Blocked.ToString();
                        //        else
                        //            x.Status = x.IsActive ? Commons.EStatusAccountFunction.Active.ToString() : Commons.EStatusAccountFunction.Inactive.ToString();
                        //    }

                        ////}
                        ////==========
                        //if (x.PeriodType == (byte)Commons.EPeriodType.OneTime)
                        //    x.sExpiryDate = "Never";
                        //else
                        //{
                        //    if (x.ExpiryDate.HasValue && DateTime.Compare(x.ExpiryDate.Value.Date, Commons.MinDate.Date) > 0)
                        //        x.sExpiryDate = x.ExpiryDate.Value.ToString(Commons.FormatDateTime);
                        //    else
                        //        x.sExpiryDate = "";
                        //}

                        //if (x.ActivateTime.HasValue)
                        //    x.sActivateTime = x.ActivateTime.Value.ToString(Commons.FormatDateTime);
                        //else
                        //    x.sActivateTime = "Not Yet Activated";

                        x = GetAdditionFunctionInfoView(x);
                    });
                }
            }
            model.CustomerDetail.ListAppProduct = _productFactory.GetListData("", (byte)Commons.EProductType.Product);
            if (model.CustomerDetail.ListAppProduct != null && model.CustomerDetail.ListAppProduct.Any())
            {
                int _count = 0;
                model.CustomerDetail.ListAppProduct.ForEach(o =>
                {
                    o.OffSet = _count;
                    _count++;
                });
            }
            if (model.CustomerDetail.ListProduct != null && model.CustomerDetail.ListProduct.Any())
            {
                int _count = 0;
                model.CustomerDetail.ListProduct.ForEach(o =>
                {
                    o.OffSet = _count;
                    if (o.ListPrice != null && o.ListPrice.Any())
                    {
                        int _countChild = 0;
                        o.ListPrice.ForEach(p =>
                        {
                            if (p.PeriodType == (byte)Commons.EPeriodType.Day)
                            {
                                p.NamePeriodType = Commons.EPeriodType.Day.ToString() + string.Format("{0}", p.Period > 1 ? "s" : "");
                            }
                            else
                            {
                                if (p.PeriodType == (byte)Commons.EPeriodType.Month)
                                {
                                    p.NamePeriodType = Commons.EPeriodType.Month.ToString() + string.Format("{0}", p.Period > 1 ? "s" : "");
                                }
                                else
                                {
                                    if (p.PeriodType == (byte)Commons.EPeriodType.Year)
                                    {
                                        p.NamePeriodType = Commons.EPeriodType.Year.ToString() + string.Format("{0}", p.Period > 1 ? "s" : "");
                                    }
                                    else
                                    {
                                        if (p.PeriodType == (byte)Commons.EPeriodType.OneTime)
                                        {
                                            p.NamePeriodType = Commons.EPeriodType.OneTime.ToString() + string.Format("{0}", p.Period > 1 ? "s" : "");
                                        }
                                    }
                                }
                            }
                            p.OffSet = _countChild;
                            p.ProductOffSet = o.OffSet;
                            _countChild++;
                        });
                    }
                    _count++;
                });
            }
            if (model.CustomerDetail.ListAppProduct != null && model.CustomerDetail.ListAppProduct.Any() && model.CustomerDetail.ListProduct != null && model.CustomerDetail.ListProduct.Any())
            {
                model.CustomerDetail.ListAppProduct.ForEach(o =>
                {
                    var IsCheck = model.CustomerDetail.ListProduct.Where(s => s.ID.Equals(o.ID)).FirstOrDefault();
                    if (IsCheck != null)
                    {
                        o.Ischeck = true;
                    }
                });
            }
            return PartialView("_Edit", model);
        }

        [HttpPost]
        public ActionResult EditCustomerDetail(CustomerBaseModels model)
        {
            try
            {
                //if (model.CustomerDetail.Birthday > model.CustomerDetail.Anniversary)
                //{
                //    ModelState.AddModelError("CustomerDetail.Birthday", "Birthday Date must be less than Anniversary Date.");
                //}
                model.CustomerDetail.MaritalStatus = model.CustomerDetail.ClientMaritalStatus == 1 ? true : false;

                if (!ModelState.IsValid)
                {
                    if ((ModelState["CustomerDetail.PictureUpload"]) != null && (ModelState["CustomerDetail.PictureUpload"]).Errors.Count > 0)
                        model.CustomerDetail.ImageURL = "";
                    model.CustomerDetail.ImageURL = Commons.Image100_100;
                    var Merchant = GetDetailMerchant(model.CustomerDetail.ID);
                    if (Merchant.CustomerID == null)
                        Merchant.CustomerID = model.CustomerDetail.ID;
                    model.MerchantStore = Merchant;
                    //========
                    model.ProductManagement = _factory.GetListDataProductsManagement(model.CustomerDetail.ID);
                    if (model.ProductManagement != null)
                    {
                        //=========ListHardwareService
                        if (model.ProductManagement.ListHardwareService != null && model.ProductManagement.ListHardwareService.Count > 0)
                        {
                            model.ProductManagement.ListHardwareService.ForEach(x =>
                            {
                                x.Status = ((Commons.EServiceAssetStatus)Enum.ToObject(typeof(Commons.EServiceAssetStatus), x.State)).ToString();
                                x.sAdditionType = ((Commons.EAdditionType)Enum.ToObject(typeof(Commons.EAdditionType), x.AdditionType)).ToString();
                                if (x.AdditionType == (byte)Commons.EAdditionType.Service)
                                    x.SerialNo = "";
                                else
                                {
                                    if (x.State == (byte)Commons.EServiceAssetStatus.New)
                                        x.SerialNo = "Updating";
                                }
                            });
                        }
                        //=========ListProductPackage
                        if (model.ProductManagement.ListProductPackage != null && model.ProductManagement.ListProductPackage.Count > 0)
                        {
                            model.ProductManagement.ListProductPackage.ForEach(x =>
                            {
                                //if (x.ReceiptNo.Equals("ON20171019-047"))
                                {
                                    x.sType = ((Commons.EProductType)Enum.ToObject(typeof(Commons.EProductType), x.Type)).ToString();
                                    //if (!x.ExpiryDate.HasValue)
                                    //    x.Status = Commons.EStatusAccountFunction.Inactive.ToString();
                                    //else
                                    //{
                                    //    DateTime _curDate = DateTime.Now;
                                    //    if (x.ExpiryDate < _curDate)
                                    //        x.Status = Commons.EStatusAccountFunction.Expired.ToString();
                                    //    else
                                    //        x.Status = x.IsActive ? Commons.EStatusAccountFunction.Active.ToString() : Commons.EStatusAccountFunction.Inactive.ToString();
                                    //}

                                    if (x.IsActive)
                                    {
                                        //var minDate = new DateTime(1900, 01, 01, 12, 00, 00);
                                        //if (!x.ExpiryDate.Equals(minDate))
                                        if (DateTime.Compare(x.ExpiryDate.Value.Date, Commons.MinDate.Date) > 0)
                                        {
                                            DateTime _curDate = DateTime.Now;
                                            if (x.ExpiryDate.Value < _curDate)
                                                x.Status = Commons.EStatusAccountFunction.Expired.ToString();
                                            else
                                                x.Status = Commons.EStatusAccountFunction.Active.ToString();
                                        }
                                        else
                                            x.Status = Commons.EStatusAccountFunction.Active.ToString();
                                    }
                                    else
                                    {
                                        x.Status = Commons.EStatusAccountFunction.Inactive.ToString();
                                    }
                                    //=======
                                    if (!x.ActivateTime.HasValue)
                                        x.sActivateTime = "Not Yet Activated";
                                    else
                                        x.sActivateTime = x.ActivateTime.Value.ToString(Commons.FormatDateTime);
                                    //=======
                                    if (!x.ExpiryDate.HasValue || DateTime.Compare(x.ExpiryDate.Value.Date, Commons.MinDate.Date) <= 0)
                                        x.sExpiryDate = "";
                                    else
                                        x.sExpiryDate = x.ExpiryDate.Value.ToString(Commons.FormatDateTime);
                                    //======
                                    if (x.PeriodType == (byte)Commons.EPeriodType.OneTime)
                                        x.sExpiryDate = "Never";
                                }

                            });
                        }
                        //=========ListAccountManagement
                        if (model.ProductManagement.ListAccount != null && model.ProductManagement.ListAccount.Count > 0)
                        {
                            model.ProductManagement.ListAccount.ForEach(x =>
                            {
                                if (!x.ExpiryDate.HasValue)
                                    x.Status = Commons.EStatusAccountFunction.Inactive.ToString();
                                else
                                {
                                    DateTime _curDate = DateTime.Now;
                                    if (DateTime.Compare(x.ExpiryDate.Value.Date, Commons.MinDate.Date) > 0 && x.ExpiryDate.Value < _curDate)
                                        x.Status = Commons.EStatusAccountFunction.Expired.ToString();
                                    else
                                        x.Status = x.IsActive ? Commons.EStatusAccountFunction.Active.ToString() : Commons.EStatusAccountFunction.Inactive.ToString();
                                }

                                //======
                                if (x.PeriodType == (byte)Commons.EPeriodType.OneTime)
                                    x.sExpiryDate = "Never";
                                else
                                {
                                    if (x.ExpiryDate.HasValue && DateTime.Compare(x.ExpiryDate.Value.Date, Commons.MinDate.Date) > 0)
                                        x.sExpiryDate = x.ExpiryDate.Value.ToString(Commons.FormatDateTime);
                                    else
                                        x.sExpiryDate = "";
                                }
                                //======
                                if (x.ActivateTime.HasValue)
                                    x.sActivateTime = x.ActivateTime.Value.ToString(Commons.FormatDateTime);
                                else
                                    x.sActivateTime = "Not Yet Activated";
                            });
                        }
                        //=========List Additional Functions Management
                        if (model.ProductManagement.ListFunction != null && model.ProductManagement.ListFunction.Count > 0)
                        {
                            model.ProductManagement.ListFunction.ForEach(x =>
                            {
                                ////if (!x.ExpiryDate.HasValue)
                                ////    x.Status = Commons.EStatusAccountFunction.Inactive.ToString();
                                ////else
                                ////{
                                //DateTime _curDate = DateTime.Now;
                                //if (DateTime.Compare(x.ExpiryDate.Value.Date, Commons.MinDate.Date) > 0 && x.ExpiryDate.Value < _curDate)
                                //    x.Status = Commons.EStatusAccountFunction.Expired.ToString();
                                //else
                                //{
                                //    if (x.IsBlock)
                                //        x.Status = Commons.EStatusAccountFunction.Blocked.ToString();
                                //    else
                                //        x.Status = x.IsActive ? Commons.EStatusAccountFunction.Active.ToString() : Commons.EStatusAccountFunction.Inactive.ToString();
                                //}

                                ////}
                                ////==========
                                //if (x.PeriodType == (byte)Commons.EPeriodType.OneTime)
                                //    x.sExpiryDate = "Never";
                                //else
                                //{
                                //    if (x.ExpiryDate.HasValue && DateTime.Compare(x.ExpiryDate.Value.Date, Commons.MinDate.Date) > 0)
                                //        x.sExpiryDate = x.ExpiryDate.Value.ToString(Commons.FormatDateTime);
                                //    else
                                //        x.sExpiryDate = "";
                                //}

                                //if (x.ActivateTime.HasValue)
                                //    x.sActivateTime = x.ActivateTime.Value.ToString(Commons.FormatDateTime);
                                //else
                                //    x.sActivateTime = "Not Yet Activated";

                                x = GetAdditionFunctionInfoView(x);
                            });
                        }
                    }
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return PartialView("_Edit", model);
                }
                //====================
                byte[] photoByte = null;
                if (model.CustomerDetail.PictureUpload != null && model.CustomerDetail.PictureUpload.ContentLength > 0)
                {
                    Byte[] imgByte = new Byte[model.CustomerDetail.PictureUpload.ContentLength];
                    model.CustomerDetail.PictureUpload.InputStream.Read(imgByte, 0, model.CustomerDetail.PictureUpload.ContentLength);
                    model.CustomerDetail.PictureByte = imgByte;
                    model.CustomerDetail.ImageURL = Guid.NewGuid() + Path.GetExtension(model.CustomerDetail.PictureUpload.FileName);
                    model.CustomerDetail.PictureUpload = null;
                    photoByte = imgByte;
                }
                else
                {
                    if (!string.IsNullOrEmpty(model.CustomerDetail.ImageURL))
                        model.CustomerDetail.ImageURL = model.CustomerDetail.ImageURL.Replace(Commons.PublicImages, "").Replace(Commons.Image100_50, "");
                }

                if (model.CustomerDetail.ListProduct != null && model.CustomerDetail.ListProduct.Any())
                {
                    model.CustomerDetail.ListProduct = model.CustomerDetail.ListProduct.Where(o => o.Status != (byte)Commons.EStatus.Deleted).ToList();
                }
                model.CustomerDetail.CreateUser = CurrentUser.UserId;
                string msg = "";
                var tmp = model.CustomerDetail.PictureByte;
                model.CustomerDetail.PictureByte = null;
                var result = _factory.CreateOrEdit(model.CustomerDetail, ref msg);
                if (result)
                {
                    model.CustomerDetail.PictureByte = tmp;
                    if (!string.IsNullOrEmpty(model.CustomerDetail.ImageURL) && model.CustomerDetail.PictureByte != null)
                    {
                        var originalDirectory = new DirectoryInfo(string.Format("{0}Uploads\\", Server.MapPath(@"\")));
                        var path = string.Format("{0}{1}", originalDirectory, model.CustomerDetail.ImageURL);
                        MemoryStream ms = new MemoryStream(photoByte, 0, photoByte.Length);
                        ms.Write(photoByte, 0, photoByte.Length);
                        System.Drawing.Image imageTmp = System.Drawing.Image.FromStream(ms, true);
                        ImageHelper.Me.SaveCroppedImage(imageTmp, path, model.CustomerDetail.ImageURL, ref photoByte);
                        model.CustomerDetail.PictureByte = photoByte;
                        FTP.Upload(model.CustomerDetail.ImageURL, model.CustomerDetail.PictureByte);
                        ImageHelper.Me.TryDeleteImageUpdated(path);
                    }
                    return RedirectToAction("Index");
                }
                else
                {
                    if (msg.Equals("Duplicate email."))
                    {
                        ModelState.AddModelError("CustomerDetail.Email", "The Email is existed. Please try again!");
                    }
                    else
                    {
                        ModelState.AddModelError("CustomerDetail.Name", Commons.ErrorMsg /*msg*/);
                    }
                    var Merchant = GetDetailMerchant(model.CustomerDetail.ID);
                    if (Merchant.CustomerID == null)
                        Merchant.CustomerID = model.CustomerDetail.ID;
                    model.MerchantStore = Merchant;
                    //========
                    model.ProductManagement = _factory.GetListDataProductsManagement(model.CustomerDetail.ID);
                    if (model.ProductManagement != null)
                    {
                        //=========ListHardwareService
                        if (model.ProductManagement.ListHardwareService != null && model.ProductManagement.ListHardwareService.Count > 0)
                        {
                            model.ProductManagement.ListHardwareService.ForEach(x =>
                            {
                                x.Status = ((Commons.EServiceAssetStatus)Enum.ToObject(typeof(Commons.EServiceAssetStatus), x.State)).ToString();
                                x.sAdditionType = ((Commons.EAdditionType)Enum.ToObject(typeof(Commons.EAdditionType), x.AdditionType)).ToString();
                                if (x.AdditionType == (byte)Commons.EAdditionType.Service)
                                    x.SerialNo = "";
                                else
                                {
                                    if (x.State == (byte)Commons.EServiceAssetStatus.New)
                                        x.SerialNo = "Updating";
                                }
                            });
                        }
                        //=========ListProductPackage
                        if (model.ProductManagement.ListProductPackage != null && model.ProductManagement.ListProductPackage.Count > 0)
                        {
                            model.ProductManagement.ListProductPackage.ForEach(x =>
                            {
                                //if (x.ReceiptNo.Equals("ON20171019-047"))
                                {
                                    x.sType = ((Commons.EProductType)Enum.ToObject(typeof(Commons.EProductType), x.Type)).ToString();
                                    //if (!x.ExpiryDate.HasValue)
                                    //    x.Status = Commons.EStatusAccountFunction.Inactive.ToString();
                                    //else
                                    //{
                                    //    DateTime _curDate = DateTime.Now;
                                    //    if (x.ExpiryDate < _curDate)
                                    //        x.Status = Commons.EStatusAccountFunction.Expired.ToString();
                                    //    else
                                    //        x.Status = x.IsActive ? Commons.EStatusAccountFunction.Active.ToString() : Commons.EStatusAccountFunction.Inactive.ToString();
                                    //}
                                    if (x.IsActive)
                                    {
                                        //var minDate = new DateTime(1900, 01, 01, 12, 00, 00);
                                        //if (!x.ExpiryDate.Equals(minDate))
                                        if (DateTime.Compare(x.ExpiryDate.Value.Date, Commons.MinDate.Date) > 0)
                                        {
                                            DateTime _curDate = DateTime.Now;
                                            if (x.ExpiryDate.Value < _curDate)
                                                x.Status = Commons.EStatusAccountFunction.Expired.ToString();
                                            else
                                                x.Status = Commons.EStatusAccountFunction.Active.ToString();
                                        }
                                        else
                                            x.Status = Commons.EStatusAccountFunction.Active.ToString();
                                    }
                                    else
                                    {
                                        x.Status = Commons.EStatusAccountFunction.Inactive.ToString();
                                    }
                                    //=======
                                    if (!x.ActivateTime.HasValue)
                                        x.sActivateTime = "Not Yet Activated";
                                    else
                                        x.sActivateTime = x.ActivateTime.Value.ToString(Commons.FormatDateTime);
                                    //=======
                                    if (!x.ExpiryDate.HasValue || DateTime.Compare(x.ExpiryDate.Value.Date, Commons.MinDate.Date) <= 0)
                                        x.sExpiryDate = "";
                                    else
                                        x.sExpiryDate = x.ExpiryDate.Value.ToString(Commons.FormatDateTime);
                                    //======
                                    if (x.PeriodType == (byte)Commons.EPeriodType.OneTime)
                                        x.sExpiryDate = "Never";
                                }

                            });
                        }
                        //=========ListAccountManagement
                        if (model.ProductManagement.ListAccount != null && model.ProductManagement.ListAccount.Count > 0)
                        {
                            model.ProductManagement.ListAccount.ForEach(x =>
                            {
                                if (!x.ExpiryDate.HasValue)
                                    x.Status = Commons.EStatusAccountFunction.Inactive.ToString();
                                else
                                {
                                    DateTime _curDate = DateTime.Now;
                                    if (DateTime.Compare(x.ExpiryDate.Value.Date, Commons.MinDate.Date) > 0 && x.ExpiryDate.Value < _curDate)
                                        x.Status = Commons.EStatusAccountFunction.Expired.ToString();
                                    else
                                        x.Status = x.IsActive ? Commons.EStatusAccountFunction.Active.ToString() : Commons.EStatusAccountFunction.Inactive.ToString();
                                }

                                //======
                                if (x.PeriodType == (byte)Commons.EPeriodType.OneTime)
                                    x.sExpiryDate = "Never";
                                else
                                {
                                    if (x.ExpiryDate.HasValue && DateTime.Compare(x.ExpiryDate.Value.Date, Commons.MinDate.Date) > 0)
                                        x.sExpiryDate = x.ExpiryDate.Value.ToString(Commons.FormatDateTime);
                                    else
                                        x.sExpiryDate = "";
                                }
                                //======
                                if (x.ActivateTime.HasValue)
                                    x.sActivateTime = x.ActivateTime.Value.ToString(Commons.FormatDateTime);
                                else
                                    x.sActivateTime = "Not Yet Activated";
                            });
                        }
                        //=========List Additional Functions Management
                        if (model.ProductManagement.ListFunction != null && model.ProductManagement.ListFunction.Count > 0)
                        {
                            model.ProductManagement.ListFunction.ForEach(x =>
                            {
                                ////if (!x.ExpiryDate.HasValue)
                                ////    x.Status = Commons.EStatusAccountFunction.Inactive.ToString();
                                ////else
                                ////{
                                //DateTime _curDate = DateTime.Now;
                                //if (DateTime.Compare(x.ExpiryDate.Value.Date, Commons.MinDate.Date) > 0 && x.ExpiryDate.Value < _curDate)
                                //    x.Status = Commons.EStatusAccountFunction.Expired.ToString();
                                //else
                                //{
                                //    if (x.IsBlock)
                                //        x.Status = Commons.EStatusAccountFunction.Blocked.ToString();
                                //    else
                                //        x.Status = x.IsActive ? Commons.EStatusAccountFunction.Active.ToString() : Commons.EStatusAccountFunction.Inactive.ToString();
                                //}

                                ////}
                                ////==========
                                //if (x.PeriodType == (byte)Commons.EPeriodType.OneTime)
                                //    x.sExpiryDate = "Never";
                                //else
                                //{
                                //    if (x.ExpiryDate.HasValue && DateTime.Compare(x.ExpiryDate.Value.Date, Commons.MinDate.Date) > 0)
                                //        x.sExpiryDate = x.ExpiryDate.Value.ToString(Commons.FormatDateTime);
                                //    else
                                //        x.sExpiryDate = "";
                                //}

                                //if (x.ActivateTime.HasValue)
                                //    x.sActivateTime = x.ActivateTime.Value.ToString(Commons.FormatDateTime);
                                //else
                                //    x.sActivateTime = "Not Yet Activated";

                                x = GetAdditionFunctionInfoView(x);
                            });
                        }
                    }
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return PartialView("_Edit", model);
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("CustomerEdit: ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }

        //[HttpGet]
        //public PartialViewResult Delete(string id)
        //{
        //    CustomerDetailModels model = GetDetail(id);
        //    return PartialView("_Delete", model);
        //}
        //[HttpPost]
        //public ActionResult Delete(CustomerDetailModels model)
        //{
        //    try
        //    {
        //        string msg = "";
        //        var result = _factory.Delete(model.ID, CurrentUser.UserId, ref msg);
        //        if (!result)
        //        {
        //            ModelState.AddModelError("Name", Commons.ErrorMsg /*msg*/);
        //            Response.StatusCode = (int)HttpStatusCode.BadRequest;
        //            return PartialView("_Delete", model);
        //        }
        //        return new HttpStatusCodeResult(HttpStatusCode.OK);
        //    }
        //    catch (Exception ex)
        //    {
        //        NSLog.Logger.Error("CustomerDelete: ", ex);
        //        ModelState.AddModelError("Name", "Have an error when you delete a customer");
        //        Response.StatusCode = (int)HttpStatusCode.BadRequest;
        //        return PartialView("_Delete", model);
        //    }
        //}

        public ActionResult DeletePopup(string ID, string PinCode, string Reason)
        {
            try
            {
                if (!PinCode.Equals(CurrentUser.PinCode))
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                string msg = "";
                var result = _factory.Delete(ID, CurrentUser.UserId, ref msg, Reason);
                if (!result)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("CustomerDelete: ", ex);
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        public ActionResult AddItemProducts(ProductOnGroupModels data)
        {
            CustomerBaseModels model = new CustomerBaseModels();
            if (data.ListProductOnGroup != null && data.ListProductOnGroup.Count() > 0)
                model.CustomerDetail.ListProduct = new List<ApplicationProductModel>();

            for (int i = 0; i < data.ListProductOnGroup.Count(); i++)
            {
                ApplicationProductModel key = new ApplicationProductModel();
                key.OffSet = data.CurrentOffset;
                key.ID = data.ListProductOnGroup[i].ID;
                key.Name = data.ListProductOnGroup[i].Name;
                key.IsActive = true;
                model.CustomerDetail.ListProduct.Add(key);
                data.CurrentOffset++;
            }

            return PartialView("_TabChooseProduct", model);
        }

        public ActionResult AddItemPrices(ProductPriceModels data)
        {
            ProductPriceModels model = new ProductPriceModels();
            model.OffSet = data.currentItemOffset;
            model.ProductOffSet = data.ProductOffSet;
            model.Period = data.Period;
            model.Price = data.Price;
            model.IsActive = true;
            model.NamePeriodType = data.NamePeriodType + (data.Period > 1 ? "s" : "");
            model.PeriodType = data.PeriodType;
            data.currentItemOffset++;
            return PartialView("_ItemPrice", model);
        }
        /*============ Merchant Store ================*/
        [HttpPost]
        public ActionResult CreateMerchantStore(CustomerBaseModels model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    if ((ModelState["MerchantStore.PictureUpload"]) != null && (ModelState["MerchantStore.PictureUpload"]).Errors.Count > 0)
                        model.MerchantStore.ImageURL = "";
                    model.MerchantStore.ImageURL = Commons.Image100_100;
                    return View(model);
                }

                byte[] photoByte = null;
                if (model.MerchantStore.PictureUpload != null && model.MerchantStore.PictureUpload.ContentLength > 0)
                {
                    Byte[] imgByte = new Byte[model.MerchantStore.PictureUpload.ContentLength];
                    model.MerchantStore.PictureUpload.InputStream.Read(imgByte, 0, model.MerchantStore.PictureUpload.ContentLength);
                    model.MerchantStore.PictureByte = imgByte;
                    model.MerchantStore.ImageURL = Guid.NewGuid() + Path.GetExtension(model.MerchantStore.PictureUpload.FileName);
                    model.MerchantStore.PictureUpload = null;
                    photoByte = imgByte;
                }
                //==============================================
                string msg = "";
                bool result = _factory.CreateOrEditMerchantStore(model.MerchantStore, ref msg);
                if (result)
                {
                    if (!string.IsNullOrEmpty(model.MerchantStore.ImageURL) && model.MerchantStore.PictureByte != null)
                    {
                        var originalDirectory = new DirectoryInfo(string.Format("{0}Uploads\\", Server.MapPath(@"\")));
                        var path = string.Format("{0}{1}", originalDirectory, model.MerchantStore.ImageURL);
                        MemoryStream ms = new MemoryStream(photoByte, 0, photoByte.Length);
                        ms.Write(photoByte, 0, photoByte.Length);
                        System.Drawing.Image imageTmp = System.Drawing.Image.FromStream(ms, true);
                        ImageHelper.Me.SaveCroppedImage(imageTmp, path, model.MerchantStore.ImageURL, ref photoByte);
                        model.MerchantStore.PictureByte = photoByte;
                        FTP.Upload(model.MerchantStore.ImageURL, model.MerchantStore.PictureByte);
                        ImageHelper.Me.TryDeleteImageUpdated(path);
                    }
                    return RedirectToAction("Index");
                }
                else
                {
                    //return RedirectToAction("Create");
                    //ModelState.AddModelError("MerchantStore.Name", Commons.ErrorMsg /*msg*/);
                    ModelState.AddModelError("MerchantStore.Name", msg);
                    return View("Create", model);
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("CreateMerchantStore: ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }

        [HttpPost]
        public ActionResult EditMerchantStore(CustomerBaseModels model)
        {
            string _CurrentURL = model.MerchantStore.ImageURL;
            //List<StoreModels> ListStore = new List<StoreModels>();
            try
            {
                model.MerchantStore.CreatedDate = CommonHelper.ConvertToUTCTime(model.MerchantStore.CreatedDate);
                model.MerchantStore.CreatedUser = CurrentUser.UserId;
                if (!ModelState.IsValid)
                {
                    if ((ModelState["MerchantStore.PictureUpload"]) != null && (ModelState["MerchantStore.PictureUpload"]).Errors.Count > 0)
                        model.MerchantStore.ImageURL = "";
                    model.MerchantStore.ImageURL = _CurrentURL;
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    model.IndexTab = 2;
                    model.CustomerDetail = GetDetail(model.MerchantStore.CustomerID);
                    model.CurrencySymbol = CurrentUser.CurrencySymbol;
                    model.ProductManagement = _factory.GetListDataProductsManagement(model.MerchantStore.CustomerID);
                    if (model.ProductManagement != null)
                    {
                        //=========ListHardwareService
                        if (model.ProductManagement.ListHardwareService != null && model.ProductManagement.ListHardwareService.Count > 0)
                        {
                            model.ProductManagement.ListHardwareService.ForEach(x =>
                            {
                                x.Status = ((Commons.EServiceAssetStatus)Enum.ToObject(typeof(Commons.EServiceAssetStatus), x.State)).ToString();
                                x.sAdditionType = ((Commons.EAdditionType)Enum.ToObject(typeof(Commons.EAdditionType), x.AdditionType)).ToString();
                                if (x.AdditionType == (byte)Commons.EAdditionType.Service)
                                    x.SerialNo = "";
                                else
                                {
                                    if (x.State == (byte)Commons.EServiceAssetStatus.New)
                                        x.SerialNo = "Updating";
                                }
                            });
                        }
                        //=========ListProductPackage
                        if (model.ProductManagement.ListProductPackage != null && model.ProductManagement.ListProductPackage.Count > 0)
                        {
                            model.ProductManagement.ListProductPackage.ForEach(x =>
                            {
                                //if (x.ReceiptNo.Equals("ON20171019-047"))
                                {
                                    x.sType = ((Commons.EProductType)Enum.ToObject(typeof(Commons.EProductType), x.Type)).ToString();
                                    //if (!x.ExpiryDate.HasValue)
                                    //    x.Status = Commons.EStatusAccountFunction.Inactive.ToString();
                                    //else
                                    //{
                                    //    DateTime _curDate = DateTime.Now;
                                    //    if (x.ExpiryDate < _curDate)
                                    //        x.Status = Commons.EStatusAccountFunction.Expired.ToString();
                                    //    else
                                    //        x.Status = x.IsActive ? Commons.EStatusAccountFunction.Active.ToString() : Commons.EStatusAccountFunction.Inactive.ToString();
                                    //}

                                    if (x.IsActive)
                                    {
                                        //var minDate = new DateTime(1900, 01, 01, 12, 00, 00);
                                        //if (!x.ExpiryDate.Equals(minDate))
                                        if (DateTime.Compare(x.ExpiryDate.Value.Date, Commons.MinDate.Date) > 0)
                                        {
                                            DateTime _curDate = DateTime.Now;
                                            if (x.ExpiryDate.Value < _curDate)
                                                x.Status = Commons.EStatusAccountFunction.Expired.ToString();
                                            else
                                                x.Status = Commons.EStatusAccountFunction.Active.ToString();
                                        }
                                        else
                                            x.Status = Commons.EStatusAccountFunction.Active.ToString();
                                    }
                                    else
                                    {
                                        x.Status = Commons.EStatusAccountFunction.Inactive.ToString();
                                    }
                                    //=======
                                    if (!x.ActivateTime.HasValue)
                                        x.sActivateTime = "Not Yet Activated";
                                    else
                                        x.sActivateTime = x.ActivateTime.Value.ToString(Commons.FormatDateTime);
                                    //=======
                                    if (!x.ExpiryDate.HasValue || DateTime.Compare(x.ExpiryDate.Value.Date, Commons.MinDate.Date) <= 0)
                                        x.sExpiryDate = "";
                                    else
                                        x.sExpiryDate = x.ExpiryDate.Value.ToString(Commons.FormatDateTime);
                                    //======
                                    if (x.PeriodType == (byte)Commons.EPeriodType.OneTime)
                                        x.sExpiryDate = "Never";
                                }

                            });
                        }
                        //=========ListAccountManagement
                        if (model.ProductManagement.ListAccount != null && model.ProductManagement.ListAccount.Count > 0)
                        {
                            model.ProductManagement.ListAccount.ForEach(x =>
                            {
                                if (!x.ExpiryDate.HasValue)
                                    x.Status = Commons.EStatusAccountFunction.Inactive.ToString();
                                else
                                {
                                    DateTime _curDate = DateTime.Now;
                                    if (DateTime.Compare(x.ExpiryDate.Value.Date, Commons.MinDate.Date) > 0 && x.ExpiryDate.Value < _curDate)
                                        x.Status = Commons.EStatusAccountFunction.Expired.ToString();
                                    else
                                        x.Status = x.IsActive ? Commons.EStatusAccountFunction.Active.ToString() : Commons.EStatusAccountFunction.Inactive.ToString();
                                }

                                //======
                                if (x.PeriodType == (byte)Commons.EPeriodType.OneTime)
                                    x.sExpiryDate = "Never";
                                else
                                {
                                    if (x.ExpiryDate.HasValue && DateTime.Compare(x.ExpiryDate.Value.Date, Commons.MinDate.Date) > 0)
                                        x.sExpiryDate = x.ExpiryDate.Value.ToString(Commons.FormatDateTime);
                                    else
                                        x.sExpiryDate = "";
                                }
                                //======
                                if (x.ActivateTime.HasValue)
                                    x.sActivateTime = x.ActivateTime.Value.ToString(Commons.FormatDateTime);
                                else
                                    x.sActivateTime = "Not Yet Activated";
                            });
                        }
                        //=========List Additional Functions Management
                        if (model.ProductManagement.ListFunction != null && model.ProductManagement.ListFunction.Count > 0)
                        {
                            model.ProductManagement.ListFunction.ForEach(x =>
                            {
                                ////if (!x.ExpiryDate.HasValue)
                                ////    x.Status = Commons.EStatusAccountFunction.Inactive.ToString();
                                ////else
                                ////{
                                //DateTime _curDate = DateTime.Now;
                                //if (DateTime.Compare(x.ExpiryDate.Value.Date, Commons.MinDate.Date) > 0 && x.ExpiryDate.Value < _curDate)
                                //    x.Status = Commons.EStatusAccountFunction.Expired.ToString();
                                //else
                                //{
                                //    if (x.IsBlock)
                                //        x.Status = Commons.EStatusAccountFunction.Blocked.ToString();
                                //    else
                                //        x.Status = x.IsActive ? Commons.EStatusAccountFunction.Active.ToString() : Commons.EStatusAccountFunction.Inactive.ToString();
                                //}

                                ////}
                                ////==========
                                //if (x.PeriodType == (byte)Commons.EPeriodType.OneTime)
                                //    x.sExpiryDate = "Never";
                                //else
                                //{
                                //    if (x.ExpiryDate.HasValue && DateTime.Compare(x.ExpiryDate.Value.Date, Commons.MinDate.Date) > 0)
                                //        x.sExpiryDate = x.ExpiryDate.Value.ToString(Commons.FormatDateTime);
                                //    else
                                //        x.sExpiryDate = "";
                                //}

                                //if (x.ActivateTime.HasValue)
                                //    x.sActivateTime = x.ActivateTime.Value.ToString(Commons.FormatDateTime);
                                //else
                                //    x.sActivateTime = "Not Yet Activated";

                                x = GetAdditionFunctionInfoView(x);
                            });
                        }
                    }
                    return PartialView("_Edit", model);
                }

                if (!string.IsNullOrEmpty(model.MerchantStore.ImageURL))
                {
                    model.MerchantStore.ImageURL = model.MerchantStore.ImageURL.Replace(Commons.PublicImages, "").Replace(Commons.Image100_100, "");
                }

                byte[] photoByte = null;
                if (model.MerchantStore.PictureUpload != null && model.MerchantStore.PictureUpload.ContentLength > 0)
                {
                    Byte[] imgByte = new Byte[model.MerchantStore.PictureUpload.ContentLength];
                    model.MerchantStore.PictureUpload.InputStream.Read(imgByte, 0, model.MerchantStore.PictureUpload.ContentLength);
                    model.MerchantStore.PictureByte = imgByte;
                    model.MerchantStore.ImageURL = Guid.NewGuid() + Path.GetExtension(model.MerchantStore.PictureUpload.FileName);
                    model.MerchantStore.PictureUpload = null;
                    photoByte = imgByte;
                }

                //==============================================
                string msg = "";
                bool result = _factory.CreateOrEditMerchantStore(model.MerchantStore, ref msg);
                if (result)
                {
                    if (!string.IsNullOrEmpty(model.MerchantStore.ImageURL) && model.MerchantStore.PictureByte != null)
                    {
                        var originalDirectory = new DirectoryInfo(string.Format("{0}Uploads\\", Server.MapPath(@"\")));
                        var path = string.Format("{0}{1}", originalDirectory, model.MerchantStore.ImageURL);
                        MemoryStream ms = new MemoryStream(photoByte, 0, photoByte.Length);
                        ms.Write(photoByte, 0, photoByte.Length);
                        System.Drawing.Image imageTmp = System.Drawing.Image.FromStream(ms, true);
                        ImageHelper.Me.SaveCroppedImage(imageTmp, path, model.MerchantStore.ImageURL, ref photoByte);
                        model.MerchantStore.PictureByte = photoByte;
                        FTP.Upload(model.MerchantStore.ImageURL, model.MerchantStore.PictureByte);
                        ImageHelper.Me.TryDeleteImageUpdated(path);
                    }
                    //return RedirectToAction("Index");
                    CustomerBaseModels modelBase = new CustomerBaseModels();
                    modelBase.CustomerDetail = GetDetail(model.MerchantStore.CustomerID);
                    var Merchant = GetDetailMerchant(model.MerchantStore.CustomerID);
                    if (Merchant.CustomerID == null)
                        Merchant.CustomerID = modelBase.CustomerDetail.ID;
                    modelBase.MerchantStore = Merchant;
                    //return new HttpStatusCodeResult(HttpStatusCode.OK);
                    modelBase.ProductManagement = _factory.GetListDataProductsManagement(model.MerchantStore.CustomerID);
                    if (modelBase.ProductManagement != null)
                    {
                        //=========ListHardwareService
                        if (modelBase.ProductManagement.ListHardwareService != null && modelBase.ProductManagement.ListHardwareService.Count > 0)
                        {
                            modelBase.ProductManagement.ListHardwareService.ForEach(x =>
                            {
                                x.Status = ((Commons.EServiceAssetStatus)Enum.ToObject(typeof(Commons.EServiceAssetStatus), x.State)).ToString();
                                x.sAdditionType = ((Commons.EAdditionType)Enum.ToObject(typeof(Commons.EAdditionType), x.AdditionType)).ToString();
                                if (x.AdditionType == (byte)Commons.EAdditionType.Service)
                                    x.SerialNo = "";
                                else
                                {
                                    if (x.State == (byte)Commons.EServiceAssetStatus.New)
                                        x.SerialNo = "Updating";
                                }
                            });
                        }
                        //=========ListProductPackage
                        if (modelBase.ProductManagement.ListProductPackage != null && modelBase.ProductManagement.ListProductPackage.Count > 0)
                        {
                            modelBase.ProductManagement.ListProductPackage.ForEach(x =>
                            {
                                x.sType = ((Commons.EProductType)Enum.ToObject(typeof(Commons.EProductType), x.Type)).ToString();
                                //if (!x.ExpiryDate.HasValue)
                                //    x.Status = Commons.EStatusAccountFunction.Inactive.ToString();
                                //else
                                //{
                                //    DateTime _curDate = DateTime.Now;
                                //    if (x.ExpiryDate < _curDate)
                                //        x.Status = Commons.EStatusAccountFunction.Expired.ToString();
                                //    else
                                //        x.Status = x.IsActive ? Commons.EStatusAccountFunction.Active.ToString() : Commons.EStatusAccountFunction.Inactive.ToString();
                                //}

                                if (x.IsActive)
                                {
                                    //var minDate = new DateTime(1900, 01, 01, 12, 00, 00);
                                    //if (!x.ExpiryDate.Equals(minDate))
                                    if (DateTime.Compare(x.ExpiryDate.Value.Date, Commons.MinDate.Date) > 0)
                                    {
                                        DateTime _curDate = DateTime.Now;
                                        if (x.ExpiryDate.Value < _curDate)
                                            x.Status = Commons.EStatusAccountFunction.Expired.ToString();
                                        else
                                            x.Status = Commons.EStatusAccountFunction.Active.ToString();
                                    }
                                    else
                                        x.Status = Commons.EStatusAccountFunction.Active.ToString();
                                }
                                else
                                {
                                    x.Status = Commons.EStatusAccountFunction.Inactive.ToString();
                                }
                                //=======
                                if (!x.ActivateTime.HasValue)
                                    x.sActivateTime = "Not Yet Activated";
                                else
                                    x.sActivateTime = x.ActivateTime.Value.ToString(Commons.FormatDateTime);
                                //=======
                                if (!x.ExpiryDate.HasValue || DateTime.Compare(x.ExpiryDate.Value.Date, Commons.MinDate.Date) <= 0)
                                    x.sExpiryDate = "";
                                else
                                    x.sExpiryDate = x.ExpiryDate.Value.ToString(Commons.FormatDateTime);
                                //======
                                if (x.PeriodType == (byte)Commons.EPeriodType.OneTime)
                                    x.sExpiryDate = "Never";
                            });
                        }
                        //=========ListAccountManagement
                        if (modelBase.ProductManagement.ListAccount != null && modelBase.ProductManagement.ListAccount.Count > 0)
                        {
                            modelBase.ProductManagement.ListAccount.ForEach(x =>
                            {
                                if (!x.ExpiryDate.HasValue)
                                    x.Status = Commons.EStatusAccountFunction.Inactive.ToString();
                                else
                                {
                                    DateTime _curDate = DateTime.Now;
                                    if (DateTime.Compare(x.ExpiryDate.Value.Date, Commons.MinDate.Date) > 0 && x.ExpiryDate.Value < _curDate)
                                        x.Status = Commons.EStatusAccountFunction.Expired.ToString();
                                    else
                                        x.Status = x.IsActive ? Commons.EStatusAccountFunction.Active.ToString() : Commons.EStatusAccountFunction.Inactive.ToString();
                                }

                                //======
                                if (x.PeriodType == (byte)Commons.EPeriodType.OneTime)
                                    x.sExpiryDate = "Never";
                                else
                                {
                                    if (x.ExpiryDate.HasValue && DateTime.Compare(x.ExpiryDate.Value.Date, Commons.MinDate.Date) > 0)
                                        x.sExpiryDate = x.ExpiryDate.Value.ToString(Commons.FormatDateTime);
                                    else
                                        x.sExpiryDate = "";
                                }
                                //======
                                if (x.ActivateTime.HasValue)
                                    x.sActivateTime = x.ActivateTime.Value.ToString(Commons.FormatDateTime);
                                else
                                    x.sActivateTime = "Not Yet Activated";
                            });
                        }
                        //=========List Additional Functions Management
                        if (modelBase.ProductManagement.ListFunction != null && modelBase.ProductManagement.ListFunction.Count > 0)
                        {
                            modelBase.ProductManagement.ListFunction.ForEach(x =>
                            {
                                //if (!x.ExpiryDate.HasValue)
                                //    x.Status = Commons.EStatusAccountFunction.Inactive.ToString();
                                //else
                                //{
                                //    DateTime _curDate = DateTime.Now;
                                //    if (DateTime.Compare(x.ExpiryDate.Value.Date, Commons.MinDate.Date) > 0 && x.ExpiryDate.Value < _curDate)
                                //        x.Status = Commons.EStatusAccountFunction.Expired.ToString();
                                //    else
                                //    {
                                //        if (x.IsBlock)
                                //            x.Status = Commons.EStatusAccountFunction.Blocked.ToString();
                                //        else
                                //            x.Status = x.IsActive ? Commons.EStatusAccountFunction.Active.ToString() : Commons.EStatusAccountFunction.Inactive.ToString();
                                //    }

                                //}
                                ////==========
                                //if (x.PeriodType == (byte)Commons.EPeriodType.OneTime)
                                //    x.sExpiryDate = "Never";
                                //else
                                //{
                                //    if (x.ExpiryDate.HasValue && DateTime.Compare(x.ExpiryDate.Value.Date, Commons.MinDate.Date) > 0)
                                //        x.sExpiryDate = x.ExpiryDate.Value.ToString(Commons.FormatDateTime);
                                //    else
                                //        x.sExpiryDate = "";
                                //}

                                //if (x.ActivateTime.HasValue)
                                //    x.sActivateTime = x.ActivateTime.Value.ToString(Commons.FormatDateTime);
                                //else
                                //    x.sActivateTime = "Not Yet Activated";

                                x = GetAdditionFunctionInfoView(x);
                            });
                        }
                    }
                    modelBase.IndexTab = 2;
                    return PartialView("_Edit", modelBase);
                }
                else
                {
                    //return RedirectToAction("Create");
                    //ModelState.AddModelError("MerchantStore.Name", Commons.ErrorMsg /*msg*/);
                    ModelState.AddModelError("MerchantStore.Name", msg);
                    model.MerchantStore.ImageURL = _CurrentURL;
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    model.CustomerDetail = GetDetail(model.MerchantStore.CustomerID);
                    model.CurrencySymbol = CurrentUser.CurrencySymbol;
                    model.ProductManagement = _factory.GetListDataProductsManagement(model.MerchantStore.CustomerID);
                    if (model.ProductManagement != null)
                    {
                        //=========ListHardwareService
                        if (model.ProductManagement.ListHardwareService != null && model.ProductManagement.ListHardwareService.Count > 0)
                        {
                            model.ProductManagement.ListHardwareService.ForEach(x =>
                            {
                                x.Status = ((Commons.EServiceAssetStatus)Enum.ToObject(typeof(Commons.EServiceAssetStatus), x.State)).ToString();
                                x.sAdditionType = ((Commons.EAdditionType)Enum.ToObject(typeof(Commons.EAdditionType), x.AdditionType)).ToString();
                                if (x.AdditionType == (byte)Commons.EAdditionType.Service)
                                    x.SerialNo = "";
                                else
                                {
                                    if (x.State == (byte)Commons.EServiceAssetStatus.New)
                                        x.SerialNo = "Updating";
                                }
                            });
                        }
                        //=========ListProductPackage
                        if (model.ProductManagement.ListProductPackage != null && model.ProductManagement.ListProductPackage.Count > 0)
                        {
                            model.ProductManagement.ListProductPackage.ForEach(x =>
                            {
                                //if (x.ReceiptNo.Equals("ON20171019-047"))
                                {
                                    x.sType = ((Commons.EProductType)Enum.ToObject(typeof(Commons.EProductType), x.Type)).ToString();
                                    //if (!x.ExpiryDate.HasValue)
                                    //    x.Status = Commons.EStatusAccountFunction.Inactive.ToString();
                                    //else
                                    //{
                                    //    DateTime _curDate = DateTime.Now;
                                    //    if (x.ExpiryDate < _curDate)
                                    //        x.Status = Commons.EStatusAccountFunction.Expired.ToString();
                                    //    else
                                    //        x.Status = x.IsActive ? Commons.EStatusAccountFunction.Active.ToString() : Commons.EStatusAccountFunction.Inactive.ToString();
                                    //}

                                    if (x.IsActive)
                                    {
                                        //var minDate = new DateTime(1900, 01, 01, 12, 00, 00);
                                        //if (!x.ExpiryDate.Equals(minDate))
                                        if (DateTime.Compare(x.ExpiryDate.Value.Date, Commons.MinDate.Date) > 0)
                                        {
                                            DateTime _curDate = DateTime.Now;
                                            if (x.ExpiryDate.Value < _curDate)
                                                x.Status = Commons.EStatusAccountFunction.Expired.ToString();
                                            else
                                                x.Status = Commons.EStatusAccountFunction.Active.ToString();
                                        }
                                        else
                                            x.Status = Commons.EStatusAccountFunction.Active.ToString();
                                    }
                                    else
                                    {
                                        x.Status = Commons.EStatusAccountFunction.Inactive.ToString();
                                    }
                                    //=======
                                    if (!x.ActivateTime.HasValue)
                                        x.sActivateTime = "Not Yet Activated";
                                    else
                                        x.sActivateTime = x.ActivateTime.Value.ToString(Commons.FormatDateTime);
                                    //=======
                                    if (!x.ExpiryDate.HasValue || DateTime.Compare(x.ExpiryDate.Value.Date, Commons.MinDate.Date) <= 0)
                                        x.sExpiryDate = "";
                                    else
                                        x.sExpiryDate = x.ExpiryDate.Value.ToString(Commons.FormatDateTime);
                                    //======
                                    if (x.PeriodType == (byte)Commons.EPeriodType.OneTime)
                                        x.sExpiryDate = "Never";
                                }

                            });
                        }
                        //=========ListAccountManagement
                        if (model.ProductManagement.ListAccount != null && model.ProductManagement.ListAccount.Count > 0)
                        {
                            model.ProductManagement.ListAccount.ForEach(x =>
                            {
                                if (!x.ExpiryDate.HasValue)
                                    x.Status = Commons.EStatusAccountFunction.Inactive.ToString();
                                else
                                {
                                    DateTime _curDate = DateTime.Now;
                                    if (DateTime.Compare(x.ExpiryDate.Value.Date, Commons.MinDate.Date) > 0 && x.ExpiryDate.Value < _curDate)
                                        x.Status = Commons.EStatusAccountFunction.Expired.ToString();
                                    else
                                        x.Status = x.IsActive ? Commons.EStatusAccountFunction.Active.ToString() : Commons.EStatusAccountFunction.Inactive.ToString();
                                }

                                //======
                                if (x.PeriodType == (byte)Commons.EPeriodType.OneTime)
                                    x.sExpiryDate = "Never";
                                else
                                {
                                    if (x.ExpiryDate.HasValue && DateTime.Compare(x.ExpiryDate.Value.Date, Commons.MinDate.Date) > 0)
                                        x.sExpiryDate = x.ExpiryDate.Value.ToString(Commons.FormatDateTime);
                                    else
                                        x.sExpiryDate = "";
                                }
                                //======
                                if (x.ActivateTime.HasValue)
                                    x.sActivateTime = x.ActivateTime.Value.ToString(Commons.FormatDateTime);
                                else
                                    x.sActivateTime = "Not Yet Activated";
                            });
                        }
                        //=========List Additional Functions Management
                        if (model.ProductManagement.ListFunction != null && model.ProductManagement.ListFunction.Count > 0)
                        {
                            model.ProductManagement.ListFunction.ForEach(x =>
                            {
                                ////if (!x.ExpiryDate.HasValue)
                                ////    x.Status = Commons.EStatusAccountFunction.Inactive.ToString();
                                ////else
                                ////{
                                //DateTime _curDate = DateTime.Now;
                                //if (DateTime.Compare(x.ExpiryDate.Value.Date, Commons.MinDate.Date) > 0 && x.ExpiryDate.Value < _curDate)
                                //    x.Status = Commons.EStatusAccountFunction.Expired.ToString();
                                //else
                                //{
                                //    if (x.IsBlock)
                                //        x.Status = Commons.EStatusAccountFunction.Blocked.ToString();
                                //    else
                                //        x.Status = x.IsActive ? Commons.EStatusAccountFunction.Active.ToString() : Commons.EStatusAccountFunction.Inactive.ToString();
                                //}

                                ////}
                                ////==========
                                //if (x.PeriodType == (byte)Commons.EPeriodType.OneTime)
                                //    x.sExpiryDate = "Never";
                                //else
                                //{
                                //    if (x.ExpiryDate.HasValue && DateTime.Compare(x.ExpiryDate.Value.Date, Commons.MinDate.Date) > 0)
                                //        x.sExpiryDate = x.ExpiryDate.Value.ToString(Commons.FormatDateTime);
                                //    else
                                //        x.sExpiryDate = "";
                                //}

                                //if (x.ActivateTime.HasValue)
                                //    x.sActivateTime = x.ActivateTime.Value.ToString(Commons.FormatDateTime);
                                //else
                                //    x.sActivateTime = "Not Yet Activated";

                                x = GetAdditionFunctionInfoView(x);
                            });
                        }
                    }
                    model.IndexTab = 2;
                    return PartialView("_Edit", model);
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("EditMerchantStore: ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }

        public MerchantModels GetDetailMerchant(string ID)
        {
            var model = new MerchantModels();
            try
            {
                model = _factory.GetDetailMerchant(ID);
                if (model.ListStore != null && model.ListStore.Count > 0)
                {
                    model.ListStore.ForEach(x =>
                    {
                        if (x.ExpiredDate.HasValue)
                        {
                            x.ExpiredDate = CommonHelper.ConvertToLocalTime(x.ExpiredDate.Value);
                        }
                    });
                }
                model.ImageURL = string.IsNullOrEmpty(model.ImageURL) ? Commons.Image100_100 : model.ImageURL;
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("CRMCustomersController -> GetDetailMerchant :", ex);
            }
            return model;
        }

        public ActionResult SelectStore(string CustomerID = "", string MerchantID = "")
        {
            List<ProductApplyStoreModels> models = new List<ProductApplyStoreModels>();
            try
            {
                var ListProductApplyStore = _factory.GetListProductApplyStore(CustomerID);
                ListProductApplyStore = ListProductApplyStore
                                                    .Where(x => x.IsUnlimitedLocation
                                                    || (!x.IsUnlimitedLocation && x.RemainingLocation > 0 && x.ExpiredTime > DateTime.Now)
                                                    || (!x.IsUnlimitedLocation && x.RemainingLocation > 0 && x.ExpiredTime == null)).ToList();
                if (ListProductApplyStore != null && ListProductApplyStore.Any())
                {
                    ListProductApplyStore.ForEach(x =>
                    {
                        string Period = x.PeriodType == (byte)Commons.EPeriodType.Day ? Commons.PeriodTypeDay :
                                        x.PeriodType == (byte)Commons.EPeriodType.Month ? Commons.PeriodTypeMonth :
                                        x.PeriodType == (byte)Commons.EPeriodType.Year ? Commons.PeriodTypeYear :
                                        x.PeriodType == (byte)Commons.EPeriodType.OneTime ? Commons.PeriodTypeOneTime :
                                        Commons.PeriodTypeNone;
                        x.sPeriod = (x.PeriodType == (byte)Commons.EPeriodType.OneTime ? Period : x.Period + " " + (x.Period > 1 ? Period + "s" : Period));
                        x.CustomerID = CustomerID;
                        x.MerchantID = MerchantID;
                        if (x.PeriodType == (byte)Commons.EPeriodType.OneTime)
                            x.sExpiredTime = "No expiry date";
                        else
                        {
                            if (x.ExpiredTime.HasValue)
                                x.sExpiredTime = x.ExpiredTime.Value.ToString(Commons.FormatDateTime);
                            else
                                x.sExpiredTime = "Not yet activated";
                        }

                        //=====
                        if (x.IsUnlimitedLocation)
                            x.sRemainingLocation = "This item has unlimited locations remaining";
                        else
                            x.sRemainingLocation = "This item still has " + x.RemainingLocation + " location(s) remaining";
                    });
                    ViewBag.isShow = ListProductApplyStore != null && ListProductApplyStore.Count > 0 ? true : false;
                    models = ListProductApplyStore;
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("SelectStore : ", ex);
            }

            return PartialView("_SelectProduct", models);
        }

        public ActionResult AddNewStore(string AssetID = "", string CustomerID = "", string MerchantID = "")
        {
            //Dung update source 22/1/2018
            StoreDetailModels model = new StoreDetailModels();
            try
            {
                var _ProductApplyStore = _factory.GetListProductApplyStore(CustomerID);
                var ProductApplyStore = _ProductApplyStore
                                                    .Where(x => x.AssetID.Equals(AssetID)
                                                            && (x.IsUnlimitedLocation || (!x.IsUnlimitedLocation && x.RemainingLocation > 0 && x.ExpiredTime > DateTime.Now)
                                                            || x.ExpiredTime == null)).FirstOrDefault();
                if (ProductApplyStore != null)
                {
                    model.ProductType = ProductApplyStore.ProductType;
                    if (ProductApplyStore.ProductType == (int)Commons.EProductType.Package)
                    {
                        if (ProductApplyStore.ListProduct != null && ProductApplyStore.ListProduct.Any())
                        {
                            var _product = ProductApplyStore.ListProduct
                                                        .Select(x => new SelectListItem()
                                                        {
                                                            Value = x.AssetID,
                                                            Text = x.ProductName
                                                        }).ToList();
                            var _typePackage = ProductApplyStore.ListProduct.Select(x => new SelectListItem()
                            {
                                Value = x.AssetID,
                                Text = x.Type.ToString()
                            }).ToList();
                            model.ListItemProduct = _product;
                            model.ListTypePackage = _typePackage;
                        }
                        else
                        {
                            var _product = new List<SelectListItem>()
                            {
                                new SelectListItem
                                {
                                    Value = ProductApplyStore.AssetID,
                                    Text = ProductApplyStore.ProductName
                                }
                            };
                            model.tempAssetID = model.ProductApplyStore.AssetID;
                            model.ListItemProduct = _product;
                        }
                        model.ParenIdPackage = AssetID;
                    }
                    else
                    {
                        if (ProductApplyStore.ProductType == (int)Commons.EProductType.Product)
                        {
                            //set Industry and get List StoreSelect
                            if (ProductApplyStore.Type == (int)Commons.EType.NuPOS || ProductApplyStore.Type == (int)Commons.EType.NuKiosk
                                || ProductApplyStore.Type == (int)Commons.EType.NuDisplay || ProductApplyStore.Type == (int)Commons.EType.POinS_Link_App)
                                model.Industry = Commons.EStoreType.FnB.ToString("d");
                            else
                                if (ProductApplyStore.Type == (int)Commons.EType.POZZ || ProductApplyStore.Type == (int)Commons.EType.POZZ_Display || ProductApplyStore.Type == (int)Commons.EType.POZZ_Kiosk)
                                model.Industry = Commons.EStoreType.Beauty.ToString("d");
                            //end getList Store select
                            var _product = new List<SelectListItem>()
                            {
                                new SelectListItem
                                {
                                    Value = ProductApplyStore.AssetID,
                                    Text = ProductApplyStore.ProductName
                                }
                            };
                            model.tempAssetID = ProductApplyStore.AssetID;
                            model.ListItemProduct = _product;
                            model.Type = ProductApplyStore.Type;
                        }
                        else
                        {
                            if (ProductApplyStore.AdditionApply != null)
                            {
                                var _product = new List<SelectListItem>() {
                                    new SelectListItem
                                    {
                                       Text = ProductApplyStore.AdditionApply.Name,
                                       Value = ProductApplyStore.AssetID
                                    }
                                };
                                model.ListItemProduct = _product;
                                if (ProductApplyStore.AdditionApply.Type == (int)Commons.EType.NuPOS || ProductApplyStore.AdditionApply.Type == (int)Commons.EType.NuKiosk
                                    || ProductApplyStore.AdditionApply.Type == (int)Commons.EType.NuDisplay || ProductApplyStore.AdditionApply.Type == (int)Commons.EType.POinS_Link_App)
                                    model.Industry = Commons.EStoreType.FnB.ToString("d");
                                else
                                    if (ProductApplyStore.AdditionApply.Type == (int)Commons.EType.POZZ ||
                                            ProductApplyStore.AdditionApply.Type == (int)Commons.EType.POZZ_Display || ProductApplyStore.AdditionApply.Type == (int)Commons.EType.POZZ_Kiosk)
                                    model.Industry = Commons.EStoreType.Beauty.ToString("d");
                                model.Type = ProductApplyStore.AdditionApply.Type;
                            }
                        }
                    }
                    model.ProductApplyStore = ProductApplyStore;
                }
                //var CountryCode = _checkIpFactory.GetCountryCode();
                if (ViewBag.ListCountry != null)
                {
                    var ListCountry = ViewBag.ListCountry as List<CountryModel>;
                    var objCountry = ListCountry.Where(x => x.Alpha2Code.Equals(CurrentCountryCode)).FirstOrDefault();
                    model.Country = objCountry != null ? objCountry.Name : "";
                    ViewBag.TimeZones = objCountry.TimeZones;
                }
                ViewBag.TimeZones = ViewBag.TimeZones ?? "";
                model.CustomerID = CustomerID;
                model.MerchantID = MerchantID;
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("AddNewStore : ", ex);
            }
            return PartialView("_FormAddNewStore", model);
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
        public ActionResult GetListProductAssignToStore(int Storetype, string AssetID, string CustomerID)
        {
            StoreDetailModels model = new StoreDetailModels();
            var _ProductApplyStore = _factory.GetListProductApplyStore(CustomerID);
            var _listproduct = _ProductApplyStore.Where(x => x.AssetID != AssetID).ToList();
            if (_listproduct != null && _listproduct.Count > 0)
            {
                int i = 0;
                foreach (var item in _listproduct)
                {
                    if (item.ProductType == (int)Commons.EProductType.Product)
                    {

                        if (item.Type == (int)Commons.EType.POinS_Link_App)
                        {
                            model.ListProductApply.Add(new ProductApply()
                            {
                                AssetID = item.AssetID,
                                ProductName = item.ProductName,
                                IsApply = true,
                                ExpiredTime = item.ExpiredTime,
                                ActiveTime = item.ActiveTime,
                                OffSet = i
                            });
                            i++;
                        }
                        else
                        {
                            if (Storetype == 1 && (item.Type == (int)Commons.EType.NuPOS
                                    || item.Type == (int)Commons.EType.NuKiosk || item.Type == (int)Commons.EType.NuDisplay))
                            {
                                model.ListProductApply.Add(new ProductApply()
                                {
                                    AssetID = item.AssetID,
                                    ProductName = item.ProductName,
                                    IsApply = true,
                                    ExpiredTime = item.ExpiredTime,
                                    ActiveTime = item.ActiveTime,
                                    OffSet = i
                                });
                                i++;
                            }
                            else
                            {
                                if (Storetype == 2 && (item.Type == (int)Commons.EType.POZZ
                                    || item.Type == (int)Commons.EType.POZZ_Display || item.Type == (int)Commons.EType.POZZ_Kiosk))
                                {
                                    model.ListProductApply.Add(new ProductApply()
                                    {
                                        AssetID = item.AssetID,
                                        ProductName = item.ProductName,
                                        IsApply = true,
                                        ExpiredTime = item.ExpiredTime,
                                        ActiveTime = item.ActiveTime,
                                        OffSet = i
                                    });
                                    i++;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (item.ProductType == (int)Commons.EProductType.Package)
                        {
                            if (item.ListProduct != null && item.ListProduct.Count > 0)
                            {
                                foreach (var _item in item.ListProduct.Where(x => x.AssetID != AssetID))
                                {
                                    if (_item.Type == (int)Commons.EType.POinS_Link_App)
                                    {
                                        model.ListProductApply.Add(new ProductApply()
                                        {
                                            AssetID = _item.AssetID,
                                            ProductName = _item.ProductName + " (in " + item.ProductName + ")",
                                            IsApply = true,
                                            ExpiredTime = _item.ExpiredTime,
                                            ActiveTime = item.ActiveTime,
                                            OffSet = i
                                        });
                                        i++;
                                    }
                                    else
                                    {
                                        if (Storetype == 1 && (_item.Type == (int)Commons.EType.NuPOS
                                                || _item.Type == (int)Commons.EType.NuKiosk || _item.Type == (int)Commons.EType.NuDisplay))
                                        {
                                            model.ListProductApply.Add(new ProductApply()
                                            {
                                                AssetID = _item.AssetID,
                                                ProductName = _item.ProductName + " (in " + item.ProductName + ")",
                                                IsApply = true,
                                                ExpiredTime = _item.ExpiredTime,
                                                ActiveTime = item.ActiveTime,
                                                OffSet = i
                                            });
                                            i++;
                                        }
                                        else
                                        {
                                            if (Storetype == 2 && (_item.Type == (int)Commons.EType.POZZ
                                                || _item.Type == (int)Commons.EType.POZZ_Display || _item.Type == (int)Commons.EType.POZZ_Kiosk))
                                            {
                                                model.ListProductApply.Add(new ProductApply()
                                                {
                                                    AssetID = _item.AssetID,
                                                    ProductName = _item.ProductName + " (in " + item.ProductName + ")",
                                                    IsApply = true,
                                                    ExpiredTime = _item.ExpiredTime,
                                                    ActiveTime = item.ActiveTime,
                                                    OffSet = i
                                                });
                                                i++;
                                            }
                                        }
                                    }

                                }
                            }
                        }
                        else
                        {
                            if (item.ProductType == (int)Commons.EProductType.Addition && item.AdditionApply != null)
                            {
                                if (item.AdditionApply.Type == (int)Commons.EType.POinS_Link_App)
                                {
                                    model.ListProductApply.Add(new ProductApply()
                                    {
                                        AssetID = item.AssetID,
                                        ProductName = item.AdditionApply.Name,
                                        IsApply = true,
                                        ExpiredTime = item.ExpiredTime,
                                        ActiveTime = item.ActiveTime,
                                        OffSet = i
                                    });
                                    i++;
                                }
                                else
                                {
                                    if (Storetype == 1 && (item.AdditionApply.Type == (int)Commons.EType.NuPOS
                                        || item.AdditionApply.Type == (int)Commons.EType.NuKiosk || item.AdditionApply.Type == (int)Commons.EType.NuDisplay))
                                    {
                                        model.ListProductApply.Add(new ProductApply()
                                        {
                                            AssetID = item.AssetID,
                                            ProductName = item.AdditionApply.Name,
                                            IsApply = true,
                                            ExpiredTime = item.ExpiredTime,
                                            ActiveTime = item.ActiveTime,
                                            OffSet = i
                                        });
                                        i++;
                                    }
                                    else
                                    {
                                        if (Storetype == 2 && (item.AdditionApply.Type == (int)Commons.EType.POZZ
                                        || item.AdditionApply.Type == (int)Commons.EType.POZZ_Kiosk || item.AdditionApply.Type == (int)Commons.EType.POZZ_Display))
                                        {
                                            model.ListProductApply.Add(new ProductApply()
                                            {
                                                AssetID = item.AssetID,
                                                ProductName = item.AdditionApply.Name,
                                                IsApply = true,
                                                ExpiredTime = item.ExpiredTime,
                                                ActiveTime = item.ActiveTime,
                                                OffSet = i
                                            });
                                            i++;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            model.ListProductApply.ForEach(x =>
            {
                if (x.ActiveTime.HasValue)
                    x.ActiveTime = CommonHelper.ConvertToLocalTime(x.ActiveTime.Value);
                if (x.ExpiredTime.HasValue)
                    x.ExpiredTime = CommonHelper.ConvertToLocalTime(x.ExpiredTime.Value);
            });
            return PartialView("_ItemStoreAssignToProduct", model);
        }
        public ActionResult GetListStoreSelect(int Type, string CustomerID)
        {
            StoreDetailModels model = new StoreDetailModels();
            var ListStore = _factory.GetListStore(CustomerID);
            // Only show stores with status == Expired || InUse
            ListStore = ListStore.Where(ww => ww.StoreStatus != (int)Commons.EStoreStatus.None && ww.StoreStatus != (int)Commons.EStoreStatus.Temporary).ToList();
            ListStore.ForEach(x =>
            {
                x.StoreStatusName = GetStoreStatusName(x.StoreStatus);
            });
            if (Type == (int)Commons.EType.NuDisplay || Type == (int)Commons.EType.NuKiosk || Type == (int)Commons.EType.NuPOS)
            {
                ListStore = ListStore.Where(x => x.StoreType == (int)Commons.EStoreType.FnB).ToList();
            }
            else
            {
                if (Type == (int)Commons.EType.POZZ || Type == (int)Commons.EType.POZZ_Kiosk || Type == (int)Commons.EType.POZZ_Display)
                {
                    ListStore = ListStore.Where(x => x.StoreType == (int)Commons.EStoreType.Beauty).ToList();
                }
                else
                    ListStore = ListStore.Where(x => x.StoreType != 0).ToList();
            }
            model.ListStore = ListStore;
            return PartialView("_ItemStoreSelect", model);
        }
        [HttpPost]
        public ActionResult CreateStoreInfo(StoreDetailModels model)
        {
            try
            {
                byte[] photoByte = null;
                //Create New Store
                if (model.IsNew)
                {
                    if (!ModelState.IsValid)
                    {
                        //Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        if (model.ProductApplyStore != null)
                        {
                            if (model.ProductApplyStore.ProductType == (int)Commons.EProductType.Package)
                            {
                                var _ProductApplyStore = _factory.GetListProductApplyStore(model.CustomerID);
                                var ProductApplyStore = _ProductApplyStore.Where(x => x.AssetID.Equals(model.ParenIdPackage)).FirstOrDefault();
                                model.ListTypePackage = ProductApplyStore.ListProduct.Select(x => new SelectListItem()
                                {
                                    Value = x.AssetID,
                                    Text = x.Type.ToString()
                                }).ToList();
                                if (model.ProductApplyStore.ListProduct != null && model.ProductApplyStore.ListProduct.Any())
                                {
                                    var _product = model.ProductApplyStore.ListProduct
                                                            .Select(x => new SelectListItem()
                                                            {
                                                                Value = x.AssetID,
                                                                Text = x.ProductName
                                                            }).ToList();
                                    model.ListItemProduct = _product;
                                }
                                else
                                {
                                    var _product = new List<SelectListItem>()
                                        {
                                            new SelectListItem
                                            {
                                                Value = model.ProductApplyStore.AssetID,
                                                Text = model.ProductApplyStore.ProductName
                                            }
                                        };
                                    model.tempAssetID = model.ProductApplyStore.AssetID;
                                    model.ListItemProduct = _product;
                                }
                                if (ProductApplyStore.ListProduct.Where(x => x.AssetID.Equals(model.AssetID)).FirstOrDefault().Type == (int)Commons.EType.POinS_Link_App)
                                    ViewBag.IsReturn = "IsReturn";
                            }
                            else
                            {
                                var _product = new List<SelectListItem>()
                                    {
                                        new SelectListItem
                                        {
                                            Value = model.ProductApplyStore.AssetID,
                                            Text = model.ProductApplyStore.ProductName
                                        }
                                    };
                                //if(model.Type == (int)Commons.EType.POinS_Link_App)
                                //    ViewBag.IsDisabledIndustry = false;
                                model.tempAssetID = model.ProductApplyStore.AssetID;
                                model.ListItemProduct = _product;
                            }


                            model.ProductApplyStore = model.ProductApplyStore;
                        }
                        var _Country = _baseFactory.GetListCountry().Where(x => x.Name.Equals(model.Country)).FirstOrDefault();
                        ViewBag.TimeZones = _Country.TimeZones;
                        ViewBag.TimeZones = ViewBag.TimeZones ?? "";
                        ViewBag.IsBack = true;
                        return PartialView("_FormAddNewStore", model);
                    }
                    //====================
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
                            model.ImageURL = model.ImageURL.Replace(Commons.PublicImages, "").Replace(Commons.Image100_100, "");
                        // model.ImageURL = model.ImageURL.Replace(Commons.PublicImagesClient, "").Replace(Commons.Image100_50, "");
                    }
                }
                else //Existing Store 
                {
                    listStorePropertiesReject = new List<string>();
                    listStorePropertiesReject.Add("Name");
                    listStorePropertiesReject.Add("Email");
                    listStorePropertiesReject.Add("Phone");
                    listStorePropertiesReject.Add("Industry");
                    listStorePropertiesReject.Add("TimeZone");
                    PropertyReject();
                    model.Name = model.tempName;
                    model.Email = model.tempEmail;
                    model.City = model.tempCity;
                    model.Country = model.tempCountry;
                    model.GSTRegNo = model.tempGSTRegNo;
                    model.TimeZone = model.tempTimeZone;
                    model.Phone = model.tempPhone;
                    model.Street = model.tempStreet;
                    model.ZipCode = model.tempZipCode;
                    model.IsActive = true;
                }

                string msg = "";
                string StoreIDReturn = "";
                model.CreateUser = CurrentUser.UserId;
                model.ProductID = model.tempAssetID;
                var result = _factory.CreateStoreInfo(model, ref StoreIDReturn, ref msg);
                if (result)
                {
                    //if (model.ListProductApply.Where(x => x.IsApply).ToList().Count > 0)
                    //{
                    //    List<StoreAssignProduct> _StoreAssignProduct = new List<StoreAssignProduct>();
                    //    foreach (var item in model.ListProductApply.Where(x => x.IsApply))
                    //    {
                    //        _StoreAssignProduct.Add(new StoreAssignProduct
                    //        {
                    //            AssetID = item.AssetID,
                    //            StoreID = StoreIDReturn
                    //        });
                    //    }
                    //    bool IsStoreAssignProduct = true;
                    //    var IsOk = _factory.StoreAssignProduct(_StoreAssignProduct, StoreIDReturn, IsStoreAssignProduct, ref msg);
                    //    if (!IsOk) {
                    //        //
                    //    }
                    //}
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
                            FTP.Upload(model.ImageURL, model.PictureByte);
                            ImageHelper.Me.TryDeleteImageUpdated(path);
                        }
                    }
                    // Create Store Success
                    //return new HttpStatusCodeResult(HttpStatusCode.OK);
                    CustomerBaseModels modelBase = new CustomerBaseModels();
                    modelBase.CustomerDetail = GetDetail(model.CustomerID);
                    var Merchant = GetDetailMerchant(model.CustomerID);
                    if (Merchant.CustomerID == null)
                        Merchant.CustomerID = modelBase.CustomerDetail.ID;
                    modelBase.MerchantStore = Merchant;
                    modelBase.IndexTab = 2;
                    return PartialView("_Edit", modelBase);
                }
                else
                {
                    //ModelState.AddModelError("Name", Commons.ErrorMsg);
                    ModelState.AddModelError("Name", msg);
                    if (model.ProductApplyStore != null)
                    {
                        if (model.ProductApplyStore.ProductType == (int)Commons.EProductType.Package)
                        {
                            var _ProductApplyStore = _factory.GetListProductApplyStore(model.CustomerID);
                            var ProductApplyStore = _ProductApplyStore.Where(x => x.AssetID.Equals(model.ParenIdPackage)).FirstOrDefault();
                            model.ListTypePackage = ProductApplyStore.ListProduct.Select(x => new SelectListItem()
                            {
                                Value = x.AssetID,
                                Text = x.Type.ToString()
                            }).ToList();
                            if (model.ProductApplyStore.ListProduct != null && model.ProductApplyStore.ListProduct.Any())
                            {
                                var _product = model.ProductApplyStore.ListProduct
                                                        .Select(x => new SelectListItem()
                                                        {
                                                            Value = x.AssetID,
                                                            Text = x.ProductName
                                                        }).ToList();
                                model.ListItemProduct = _product;
                            }
                            else
                            {
                                var _product = new List<SelectListItem>()
                                {
                                    new SelectListItem
                                    {
                                        Value = model.ProductApplyStore.AssetID,
                                        Text = model.ProductApplyStore.ProductName
                                    }
                                };
                                model.tempAssetID = model.ProductApplyStore.AssetID;
                                model.ListItemProduct = _product;
                            }
                        }
                        else
                        {
                            var _product = new List<SelectListItem>()
                            {
                                new SelectListItem
                                {
                                    Value = model.ProductApplyStore.AssetID,
                                    Text = model.ProductApplyStore.ProductName
                                }
                            };
                            model.tempAssetID = model.ProductApplyStore.AssetID;
                            model.ListItemProduct = _product;
                        }


                        model.ProductApplyStore = model.ProductApplyStore;
                    }
                    //  Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    var _Country = _baseFactory.GetListCountry().Where(x => x.Name.Equals(model.Country)).FirstOrDefault();
                    ViewBag.TimeZones = _Country.TimeZones;
                    ViewBag.TimeZones = ViewBag.TimeZones ?? "";
                    ViewBag.IsBack = true;
                    return PartialView("_FormAddNewStore", model);
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("CreateStoreInfo: ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }

        [HttpPost]
        public ActionResult EditStoreInfo(StoreDetailModels model)
        {
            try
            {
                byte[] photoByte = null;
                //Create New Store
                if (!ModelState.IsValid)
                {
                    // Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    GetTimeZones(model.Country);
                    return PartialView("_FormEditStore", model);
                }
                //====================
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
                        model.ImageURL = model.ImageURL.Replace(Commons.PublicImages, "").Replace(Commons.Image100_100, "");
                    //model.ImageURL = model.ImageURL.Replace(Commons.PublicImagesClient, "").Replace(Commons.Image100_50, "");
                }
                string msg = "";
                model.CreateUser = CurrentUser.UserId;
                var result = _factory.UpdateStoreInfo(model, ref msg);
                if (result)
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
                        FTP.Upload(model.ImageURL, model.PictureByte);
                        ImageHelper.Me.TryDeleteImageUpdated(path);
                    }
                    //return new HttpStatusCodeResult(HttpStatusCode.OK);
                    CustomerBaseModels modelBase = new CustomerBaseModels();
                    modelBase.CustomerDetail = GetDetail(model.CustomerID);
                    var Merchant = GetDetailMerchant(model.CustomerID);
                    if (Merchant.CustomerID == null)
                        Merchant.CustomerID = modelBase.CustomerDetail.ID;
                    modelBase.MerchantStore = Merchant;
                    modelBase.IndexTab = 2;
                    return PartialView("_Edit", modelBase);
                }
                else
                {
                    //ModelState.AddModelError("Name", Commons.ErrorMsg);
                    ModelState.AddModelError("Name", msg);
                    //  Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    GetTimeZones(model.Country);
                    return PartialView("_FormEditStore", model);
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("EditStoreInfo: ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }
        public ActionResult AssignProductToStore(StoreDetailModels model)
        {
            if (model.ListProductApply != null && model.ListProductApply.Count > 0)
            {
                List<StoreAssignProduct> _StoreAssignProduct = new List<StoreAssignProduct>();
                foreach (var item in model.ListProductApply.Where(x => x.IsApply))
                {
                    _StoreAssignProduct.Add(new StoreAssignProduct
                    {
                        AssetID = item.AssetID,
                        StoreID = model.ID
                    });
                }
                string mess = "";
                bool IsStoreAssignProduct = true;
                var IsOk = _factory.StoreAssignProduct(_StoreAssignProduct, model.ID, IsStoreAssignProduct, CurrentUser.UserId, ref mess);
                return Json(IsOk, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetDetailStoreToJSon(string StoreId)
        {
            StoreDetailModels store = new StoreDetailModels();
            try
            {
                var _store = _factory.GetStoreDetail(StoreId);
                store = _store;
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("GetDetailStoreToJSon : ", ex);
            }
            //=======
            return Json(store, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ViewStore(string StoreId, string CustomerID)
        {
            StoreDetailModels store = new StoreDetailModels();
            try
            {
                var _store = _factory.GetStoreDetail(StoreId);
                _store.Industry = _store.StoreType.ToString();
                store = _store;
                store.CustomerID = CustomerID;
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("GetDetailStoreToJSon : ", ex);
            }
            return PartialView("_ViewStore", store);
        }

        public ActionResult EditStore(string StoreId, string CustomerID)
        {
            StoreDetailModels store = new StoreDetailModels();
            try
            {
                var _store = _factory.GetStoreDetail(StoreId);
                _store.Industry = _store.StoreType.ToString() == Commons.EStoreType.FnB.ToString("d") ? Commons.EStoreType.FnB.ToString("d") : Commons.EStoreType.Beauty.ToString("d");
                store = _store;
                store.CustomerID = CustomerID;
                GetTimeZones(store.Country);
                var _listproduct = _factory.GetListProductForStore(CustomerID, StoreId);
                if (_listproduct != null && _listproduct.Count > 0)
                {
                    int i = 0;
                    foreach (var item in _listproduct)
                    {
                        if (item.ProductType == (int)Commons.EProductType.Product)
                        {
                            if (item.Type == (int)Commons.EType.POinS_Link_App)
                            {
                                store.ListProductApply.Add(new ProductApply()
                                {
                                    AssetID = item.AssetID,
                                    ProductName = item.ProductName,
                                    IsApply = item.IsAppliedStore,
                                    ExpiredTime = item.ExpiredTime,
                                    ActiveTime = item.ActiveTime,
                                    OffSet = i
                                });
                                i++;
                            }
                            else
                            {
                                if (store.StoreType == (int)Commons.EStoreType.FnB && (item.Type == (int)Commons.EType.NuPOS
                                        || item.Type == (int)Commons.EType.NuKiosk || item.Type == (int)Commons.EType.NuDisplay))
                                {
                                    store.ListProductApply.Add(new ProductApply()
                                    {
                                        AssetID = item.AssetID,
                                        ProductName = item.ProductName,
                                        IsApply = item.IsAppliedStore,
                                        ExpiredTime = item.ExpiredTime,
                                        ActiveTime = item.ActiveTime,
                                        OffSet = i
                                    });
                                    i++;
                                }
                                else
                                {
                                    if (store.StoreType == (int)Commons.EStoreType.Beauty && (item.Type == (int)Commons.EType.POZZ
                                        || item.Type == (int)Commons.EType.POZZ_Display || item.Type == (int)Commons.EType.POZZ_Kiosk))
                                    {
                                        store.ListProductApply.Add(new ProductApply()
                                        {
                                            AssetID = item.AssetID,
                                            ProductName = item.ProductName,
                                            IsApply = item.IsAppliedStore,
                                            ExpiredTime = item.ExpiredTime,
                                            ActiveTime = item.ActiveTime,
                                            OffSet = i
                                        });
                                        i++;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (item.ProductType == (int)Commons.EProductType.Package)
                            {
                                if (item.ListProduct != null && item.ListProduct.Count > 0)
                                {
                                    foreach (var _item in item.ListProduct)
                                    {
                                        if (_item.Type == (int)Commons.EType.POinS_Link_App)
                                        {
                                            store.ListProductApply.Add(new ProductApply()
                                            {
                                                AssetID = _item.AssetID,
                                                ProductName = _item.ProductName + " (in " + item.ProductName + ")",
                                                IsApply = _item.IsAppliedStore,
                                                ExpiredTime = _item.ExpiredTime,
                                                ActiveTime = item.ActiveTime,
                                                OffSet = i
                                            });
                                            i++;
                                        }
                                        else
                                        {
                                            if (store.StoreType == (int)Commons.EStoreType.FnB && (_item.Type == (int)Commons.EType.NuPOS
                                                    || _item.Type == (int)Commons.EType.NuKiosk || _item.Type == (int)Commons.EType.NuDisplay))
                                            {
                                                store.ListProductApply.Add(new ProductApply()
                                                {
                                                    AssetID = _item.AssetID,
                                                    ProductName = _item.ProductName + " (in " + item.ProductName + ")",
                                                    IsApply = _item.IsAppliedStore,
                                                    ExpiredTime = _item.ExpiredTime,
                                                    ActiveTime = item.ActiveTime,
                                                    OffSet = i
                                                });
                                                i++;
                                            }
                                            else
                                            {
                                                if (store.StoreType == (int)Commons.EStoreType.Beauty && (_item.Type == (int)Commons.EType.POZZ
                                                    || _item.Type == (int)Commons.EType.POZZ_Display || _item.Type == (int)Commons.EType.POZZ_Kiosk))
                                                {
                                                    store.ListProductApply.Add(new ProductApply()
                                                    {
                                                        AssetID = _item.AssetID,
                                                        ProductName = _item.ProductName + " (in " + item.ProductName + ")",
                                                        IsApply = _item.IsAppliedStore,
                                                        ExpiredTime = _item.ExpiredTime,
                                                        ActiveTime = item.ActiveTime,
                                                        OffSet = i
                                                    });
                                                    i++;
                                                }
                                            }
                                        }

                                    }
                                }
                            }
                            else
                            {
                                if (item.ProductType == (int)Commons.EProductType.Addition && item.AdditionApply != null)
                                {
                                    if (item.AdditionApply.Type == (int)Commons.EType.POinS_Link_App)
                                    {
                                        store.ListProductApply.Add(new ProductApply()
                                        {
                                            AssetID = item.AssetID,
                                            ProductName = item.AdditionApply.Name,
                                            IsApply = item.IsAppliedStore,
                                            ExpiredTime = item.ExpiredTime,
                                            ActiveTime = item.ActiveTime,
                                            OffSet = i
                                        });
                                        i++;
                                    }
                                    else
                                    {
                                        if (store.StoreType == (int)Commons.EStoreType.FnB && (item.AdditionApply.Type == (int)Commons.EType.NuPOS
                                            || item.AdditionApply.Type == (int)Commons.EType.NuKiosk || item.AdditionApply.Type == (int)Commons.EType.NuDisplay))
                                        {
                                            store.ListProductApply.Add(new ProductApply()
                                            {
                                                AssetID = item.AssetID,
                                                ProductName = item.AdditionApply.Name,
                                                IsApply = item.IsAppliedStore,
                                                ExpiredTime = item.ExpiredTime,
                                                ActiveTime = item.ActiveTime,
                                                OffSet = i
                                            });
                                            i++;
                                        }
                                        else
                                        {
                                            if (store.StoreType == (int)Commons.EStoreType.Beauty && (item.AdditionApply.Type == (int)Commons.EType.POZZ
                                            || item.AdditionApply.Type == (int)Commons.EType.POZZ_Kiosk || item.AdditionApply.Type == (int)Commons.EType.POZZ_Display))
                                            {
                                                store.ListProductApply.Add(new ProductApply()
                                                {
                                                    AssetID = item.AssetID,
                                                    ProductName = item.AdditionApply.Name,
                                                    IsApply = item.IsAppliedStore,
                                                    ExpiredTime = item.ExpiredTime,
                                                    ActiveTime = item.ActiveTime,
                                                    OffSet = i
                                                });
                                                i++;
                                            }
                                        }
                                    }

                                }
                            }
                        }
                    }
                    store.ListProductApply.ForEach(x =>
                    {
                        if (x.ActiveTime.HasValue)
                            x.ActiveTime = CommonHelper.ConvertToLocalTime(x.ActiveTime.Value);
                        if (x.ExpiredTime.HasValue)
                            x.ExpiredTime = CommonHelper.ConvertToLocalTime(x.ExpiredTime.Value);
                    });
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("GetDetailStoreToJSon : ", ex);
            }
            return PartialView("_FormEditStore", store);
        }

        public ActionResult GetTimeZones(string _Country)
        {
            try
            {
                ViewBag.TimeZones = "";
                if (_Country != "")
                {
                    var Country = _baseFactory.GetListCountry();
                    for (int i = 0; i < Country.Count; i++)
                    {
                        if (Country[i].Name == _Country)
                        {
                            ViewBag.TimeZones = Country[i].TimeZones;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("GetTimeZones : ", ex);
            }
            //=======
            return PartialView("_TimeZones");
        }
        [HttpPost]
        public ActionResult ListItemReceipt(List<ItemRefundModels> Items)
        {
            decimal _TotalRefund = 0;
            Items.ForEach(x =>
            {
                //x.sProductType = (x.ProductType == (int)Commons.EProductType.Addition ? Commons.EProductType.Addition.ToString() :
                //                x.ProductType == (int)Commons.EProductType.Discount ? Commons.EProductType.Discount.ToString() :
                //                x.ProductType == (int)Commons.EProductType.Package ? Commons.EProductType.Package.ToString() :
                //                x.ProductType == (int)Commons.EProductType.Product ? Commons.EProductType.Product.ToString() :
                //                Commons.EProductType.Promotion.ToString()
                //                );

                x.sProductType = ((Commons.EProductType)Enum.ToObject(typeof(Commons.EProductType), x.ProductType)).ToString();
                if (x.ProductType == (int)Commons.EProductType.Addition)
                {
                    x.sProductType = x.sProductType + " - " + ((Commons.EAdditionType)Enum.ToObject(typeof(Commons.EAdditionType), x.AdditionType)).ToString();
                }

                _TotalRefund = _TotalRefund + x.Price;
            }
                                                );
            Items.ForEach(x => x.TotalRefund = _TotalRefund);
            return PartialView("_ItemRefundConfirm", Items);
        }
        /*============ End Merchant Store ================*/

        #region /*============ Products Management ================*/
        [HttpPost]
        public ActionResult CreateProductsManagement(CustomerBaseModels model)
        {
            try
            {
                return null;
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("CreateProductsManagement: ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }

        //Dung edit 20/1/2018
        //[HttpPost]
        //public ActionResult EditProductsManagement(CustomerBaseModels model)
        //{
        //    try
        //    {
        //        return null;
        //    }
        //    catch (Exception ex)
        //    {
        //        NSLog.Logger.Error("EditProductsManagement: ", ex);
        //        return new HttpStatusCodeResult(400, ex.Message);
        //    }
        //}

        public ActionResult ChangeStatusHardwareService(string ID, string CustomerId)
        {
            try
            {
                var ListAsset = new List<string>();
                ListAsset.Add(ID);
                string msg = "";
                var result = _factory.ChangeStatusHardwareService(ListAsset, CurrentUser.UserId, ref msg);
                if (result)
                {
                    CustomerBaseModels model = new CustomerBaseModels();
                    model.CurrencySymbol = CurrentUser.CurrencySymbol;
                    model.CustomerDetail = GetDetail(CustomerId);
                    var Merchant = GetDetailMerchant(CustomerId);
                    if (Merchant.CustomerID == null)
                        Merchant.CustomerID = model.CustomerDetail.ID;
                    model.MerchantStore = Merchant;
                    //========
                    model.ProductManagement = _factory.GetListDataProductsManagement(CustomerId);
                    if (model.ProductManagement != null)
                    {
                        //=========ListHardwareService
                        if (model.ProductManagement.ListHardwareService != null && model.ProductManagement.ListHardwareService.Count > 0)
                        {
                            model.ProductManagement.ListHardwareService.ForEach(x =>
                            {
                                x.Status = ((Commons.EServiceAssetStatus)Enum.ToObject(typeof(Commons.EServiceAssetStatus), x.State)).ToString();
                                x.sAdditionType = ((Commons.EAdditionType)Enum.ToObject(typeof(Commons.EAdditionType), x.AdditionType)).ToString();
                                if (x.AdditionType == (byte)Commons.EAdditionType.Service)
                                    x.SerialNo = "";
                                else
                                {
                                    if (x.State == (byte)Commons.EServiceAssetStatus.New)
                                        x.SerialNo = "Updating";
                                }
                            });
                        }
                        //=========ListProductPackage
                        if (model.ProductManagement.ListProductPackage != null && model.ProductManagement.ListProductPackage.Count > 0)
                        {
                            model.ProductManagement.ListProductPackage.ForEach(x =>
                            {
                                x.sType = ((Commons.EProductType)Enum.ToObject(typeof(Commons.EProductType), x.Type)).ToString();
                                if (x.IsActive)
                                {
                                    //var minDate = new DateTime(1900, 01, 01, 12, 00, 00);
                                    //if (!x.ExpiryDate.Equals(minDate))
                                    if (DateTime.Compare(x.ExpiryDate.Value.Date, Commons.MinDate.Date) > 0)
                                    {
                                        DateTime _curDate = DateTime.Now;
                                        if (x.ExpiryDate.Value < _curDate)
                                            x.Status = Commons.EStatusAccountFunction.Expired.ToString();
                                        else
                                            x.Status = Commons.EStatusAccountFunction.Active.ToString();
                                    }
                                    else
                                        x.Status = Commons.EStatusAccountFunction.Active.ToString();
                                }
                                else
                                {
                                    x.Status = Commons.EStatusAccountFunction.Inactive.ToString();
                                }
                                //=======
                                if (!x.ActivateTime.HasValue)
                                    x.sActivateTime = "Not Yet Activated";
                                else
                                    x.sActivateTime = x.ActivateTime.Value.ToString(Commons.FormatDateTime);
                                //=======
                                if (!x.ExpiryDate.HasValue || DateTime.Compare(x.ExpiryDate.Value.Date, Commons.MinDate.Date) <= 0)
                                    x.sExpiryDate = "";
                                else
                                    x.sExpiryDate = x.ExpiryDate.Value.ToString(Commons.FormatDateTime);
                                //======
                                if (x.PeriodType == (byte)Commons.EPeriodType.OneTime)
                                    x.sExpiryDate = "Never";
                            });
                        }
                        //=========ListAccountManagement
                        if (model.ProductManagement.ListAccount != null && model.ProductManagement.ListAccount.Count > 0)
                        {
                            model.ProductManagement.ListAccount.ForEach(x =>
                            {
                                if (!x.ExpiryDate.HasValue)
                                    x.Status = Commons.EStatusAccountFunction.Inactive.ToString();
                                else
                                {
                                    DateTime _curDate = DateTime.Now;
                                    if (DateTime.Compare(x.ExpiryDate.Value.Date, Commons.MinDate.Date) > 0 && x.ExpiryDate.Value < _curDate)
                                        x.Status = Commons.EStatusAccountFunction.Expired.ToString();
                                    else
                                        x.Status = x.IsActive ? Commons.EStatusAccountFunction.Active.ToString() : Commons.EStatusAccountFunction.Inactive.ToString();
                                }

                                //======
                                if (x.PeriodType == (byte)Commons.EPeriodType.OneTime)
                                    x.sExpiryDate = "Never";
                                else
                                {
                                    if (x.ExpiryDate.HasValue && DateTime.Compare(x.ExpiryDate.Value.Date, Commons.MinDate.Date) > 0)
                                        x.sExpiryDate = x.ExpiryDate.Value.ToString(Commons.FormatDateTime);
                                    else
                                        x.sExpiryDate = "";
                                }
                                //======
                                if (x.ActivateTime.HasValue)
                                    x.sActivateTime = x.ActivateTime.Value.ToString(Commons.FormatDateTime);
                                else
                                    x.sActivateTime = "Not Yet Activated";
                            });
                        }
                        //=========List Additional Functions Management
                        if (model.ProductManagement.ListFunction != null && model.ProductManagement.ListFunction.Count > 0)
                        {
                            model.ProductManagement.ListFunction.ForEach(x =>
                            {
                                x = GetAdditionFunctionInfoView(x);
                            });
                        }
                    }
                    model.IndexTab = 3;
                    return PartialView("_ItemHardwaresAndServices", model);
                }
                // return new HttpStatusCodeResult(HttpStatusCode.OK);
                else
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, msg);
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("EditProductsManagement: ", ex);
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [HttpPost]
        public ActionResult AddSerialNumberHardwareService(List<SerialNumberDTO> ListSerialNumber, string ID, string CustomerId)
        {
            try
            {
                // Remove some special character of SerialNo
                if (ListSerialNumber != null && ListSerialNumber.Any())
                {
                    ListSerialNumber.ForEach(xy =>
                    {
                        xy.SerialNo = RemoveSpecChar(xy.SerialNo);
                    });
                }

                string msg = "";
                var result = _factory.AddSerialNumberHardwareService(ID, ListSerialNumber, CurrentUser.UserId, ref msg);
                if (result)
                {
                    CustomerBaseModels model = new CustomerBaseModels();
                    model.CurrencySymbol = CurrentUser.CurrencySymbol;
                    model.CustomerDetail = GetDetail(CustomerId);
                    var Merchant = GetDetailMerchant(CustomerId);
                    if (Merchant.CustomerID == null)
                        Merchant.CustomerID = model.CustomerDetail.ID;
                    model.MerchantStore = Merchant;
                    //========
                    model.ProductManagement = _factory.GetListDataProductsManagement(CustomerId);
                    if (model.ProductManagement != null)
                    {
                        //=========ListHardwareService
                        if (model.ProductManagement.ListHardwareService != null && model.ProductManagement.ListHardwareService.Count > 0)
                        {
                            model.ProductManagement.ListHardwareService.ForEach(x =>
                            {
                                x.Status = ((Commons.EServiceAssetStatus)Enum.ToObject(typeof(Commons.EServiceAssetStatus), x.State)).ToString();
                                x.sAdditionType = ((Commons.EAdditionType)Enum.ToObject(typeof(Commons.EAdditionType), x.AdditionType)).ToString();
                                if (x.AdditionType == (byte)Commons.EAdditionType.Service)
                                    x.SerialNo = "";
                                else
                                {
                                    if (x.State == (byte)Commons.EServiceAssetStatus.New)
                                        x.SerialNo = "Updating";
                                }
                            });
                        }
                        //=========ListProductPackage
                        if (model.ProductManagement.ListProductPackage != null && model.ProductManagement.ListProductPackage.Count > 0)
                        {
                            model.ProductManagement.ListProductPackage.ForEach(x =>
                            {
                                x.sType = ((Commons.EProductType)Enum.ToObject(typeof(Commons.EProductType), x.Type)).ToString();
                                //if (!x.ExpiryDate.HasValue)
                                //    x.Status = Commons.EStatusAccountFunction.Inactive.ToString();
                                //else
                                //{
                                //    DateTime _curDate = DateTime.Now;
                                //    if (x.ExpiryDate < _curDate)
                                //        x.Status = Commons.EStatusAccountFunction.Expired.ToString();
                                //    else
                                //        x.Status = x.IsActive ? Commons.EStatusAccountFunction.Active.ToString() : Commons.EStatusAccountFunction.Inactive.ToString();
                                //}
                                if (x.IsActive)
                                {
                                    //var minDate = new DateTime(1900, 01, 01, 12, 00, 00);
                                    //if (!x.ExpiryDate.Equals(minDate))
                                    if (DateTime.Compare(x.ExpiryDate.Value.Date, Commons.MinDate.Date) > 0)
                                    {
                                        DateTime _curDate = DateTime.Now;
                                        if (x.ExpiryDate.Value < _curDate)
                                            x.Status = Commons.EStatusAccountFunction.Expired.ToString();
                                        else
                                            x.Status = Commons.EStatusAccountFunction.Active.ToString();
                                    }
                                    else
                                        x.Status = Commons.EStatusAccountFunction.Active.ToString();
                                }
                                else
                                {
                                    x.Status = Commons.EStatusAccountFunction.Inactive.ToString();
                                }
                                //=======
                                if (!x.ActivateTime.HasValue)
                                    x.sActivateTime = "Not Yet Activated";
                                else
                                    x.sActivateTime = x.ActivateTime.Value.ToString(Commons.FormatDateTime);
                                //=======
                                if (!x.ExpiryDate.HasValue || DateTime.Compare(x.ExpiryDate.Value.Date, Commons.MinDate.Date) <= 0)
                                    x.sExpiryDate = "";
                                else
                                    x.sExpiryDate = x.ExpiryDate.Value.ToString(Commons.FormatDateTime);
                                //======
                                if (x.PeriodType == (byte)Commons.EPeriodType.OneTime)
                                    x.sExpiryDate = "Never";
                            });
                        }
                        //=========ListAccountManagement
                        if (model.ProductManagement.ListAccount != null && model.ProductManagement.ListAccount.Count > 0)
                        {
                            model.ProductManagement.ListAccount.ForEach(x =>
                            {
                                if (!x.ExpiryDate.HasValue)
                                    x.Status = Commons.EStatusAccountFunction.Inactive.ToString();
                                else
                                {
                                    DateTime _curDate = DateTime.Now;
                                    if (DateTime.Compare(x.ExpiryDate.Value.Date, Commons.MinDate.Date) > 0 && x.ExpiryDate.Value < _curDate)
                                        x.Status = Commons.EStatusAccountFunction.Expired.ToString();
                                    else
                                        x.Status = x.IsActive ? Commons.EStatusAccountFunction.Active.ToString() : Commons.EStatusAccountFunction.Inactive.ToString();
                                }

                                //======
                                if (x.PeriodType == (byte)Commons.EPeriodType.OneTime)
                                    x.sExpiryDate = "Never";
                                else
                                {
                                    if (x.ExpiryDate.HasValue && DateTime.Compare(x.ExpiryDate.Value.Date, Commons.MinDate.Date) > 0)
                                        x.sExpiryDate = x.ExpiryDate.Value.ToString(Commons.FormatDateTime);
                                    else
                                        x.sExpiryDate = "";
                                }
                                //======
                                if (x.ActivateTime.HasValue)
                                    x.sActivateTime = x.ActivateTime.Value.ToString(Commons.FormatDateTime);
                                else
                                    x.sActivateTime = "Not Yet Activated";
                            });
                        }
                        //=========List Additional Functions Management
                        if (model.ProductManagement.ListFunction != null && model.ProductManagement.ListFunction.Count > 0)
                        {
                            model.ProductManagement.ListFunction.ForEach(x =>
                            {
                                ////if (!x.ExpiryDate.HasValue)
                                ////    x.Status = Commons.EStatusAccountFunction.Inactive.ToString();
                                ////else
                                ////{
                                //    DateTime _curDate = DateTime.Now;
                                //    if (DateTime.Compare(x.ExpiryDate.Value.Date, Commons.MinDate.Date) > 0 && x.ExpiryDate.Value < _curDate)
                                //        x.Status = Commons.EStatusAccountFunction.Expired.ToString();
                                //    else
                                //    {
                                //        if (x.IsBlock)
                                //            x.Status = Commons.EStatusAccountFunction.Blocked.ToString();
                                //        else
                                //            x.Status = x.IsActive ? Commons.EStatusAccountFunction.Active.ToString() : Commons.EStatusAccountFunction.Inactive.ToString();
                                //    }

                                ////}
                                ////==========
                                //if (x.PeriodType == (byte)Commons.EPeriodType.OneTime)
                                //    x.sExpiryDate = "Never";
                                //else
                                //{
                                //    if (x.ExpiryDate.HasValue && DateTime.Compare(x.ExpiryDate.Value.Date, Commons.MinDate.Date) > 0)
                                //        x.sExpiryDate = x.ExpiryDate.Value.ToString(Commons.FormatDateTime);
                                //    else
                                //        x.sExpiryDate = "";
                                //}

                                //if (x.ActivateTime.HasValue)
                                //    x.sActivateTime = x.ActivateTime.Value.ToString(Commons.FormatDateTime);
                                //else
                                //    x.sActivateTime = "Not Yet Activated";

                                x = GetAdditionFunctionInfoView(x);
                            });
                        }
                    }
                    model.IndexTab = 3;
                    return PartialView("_ItemHardwaresAndServices", model);
                }
                //return new HttpStatusCodeResult(HttpStatusCode.OK);
                else
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, msg);
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("EditProductsManagement: ", ex);
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        public ActionResult ChangeStatusProductPackage(string ID, string CustomerId)
        {
            try
            {
                string msg = "";
                var result = _factory.ChangeStatusProductPackage(ID, CurrentUser.UserId, ref msg);
                if (result)
                {
                    CustomerBaseModels model = new CustomerBaseModels();
                    model.CurrencySymbol = CurrentUser.CurrencySymbol;
                    model.CustomerDetail = GetDetail(CustomerId);
                    var Merchant = GetDetailMerchant(CustomerId);
                    if (Merchant.CustomerID == null)
                        Merchant.CustomerID = model.CustomerDetail.ID;
                    model.MerchantStore = Merchant;
                    //========
                    model.ProductManagement = _factory.GetListDataProductsManagement(CustomerId);
                    if (model.ProductManagement != null)
                    {
                        //=========ListHardwareService
                        if (model.ProductManagement.ListHardwareService != null && model.ProductManagement.ListHardwareService.Count > 0)
                        {
                            model.ProductManagement.ListHardwareService.ForEach(x =>
                            {
                                x.Status = ((Commons.EServiceAssetStatus)Enum.ToObject(typeof(Commons.EServiceAssetStatus), x.State)).ToString();
                                x.sAdditionType = ((Commons.EAdditionType)Enum.ToObject(typeof(Commons.EAdditionType), x.AdditionType)).ToString();
                                if (x.AdditionType == (byte)Commons.EAdditionType.Service)
                                    x.SerialNo = "";
                                else
                                {
                                    if (x.State == (byte)Commons.EServiceAssetStatus.New)
                                        x.SerialNo = "Updating";
                                }
                            });
                        }
                        //=========ListProductPackage
                        if (model.ProductManagement.ListProductPackage != null && model.ProductManagement.ListProductPackage.Count > 0)
                        {
                            model.ProductManagement.ListProductPackage.ForEach(x =>
                            {
                                x.sType = ((Commons.EProductType)Enum.ToObject(typeof(Commons.EProductType), x.Type)).ToString();
                                //if (!x.ExpiryDate.HasValue)
                                //    x.Status = Commons.EStatusAccountFunction.Inactive.ToString();
                                //else
                                //{
                                //    DateTime _curDate = DateTime.Now;
                                //    if (x.ExpiryDate < _curDate)
                                //        x.Status = Commons.EStatusAccountFunction.Expired.ToString();
                                //    else
                                //        x.Status = x.IsActive ? Commons.EStatusAccountFunction.Active.ToString() : Commons.EStatusAccountFunction.Inactive.ToString();
                                //}

                                if (x.IsActive)
                                {
                                    //var minDate = new DateTime(1900, 01, 01, 12, 00, 00);
                                    //if (!x.ExpiryDate.Equals(minDate))
                                    if (DateTime.Compare(x.ExpiryDate.Value.Date, Commons.MinDate.Date) > 0)
                                    {
                                        DateTime _curDate = DateTime.Now;
                                        if (x.ExpiryDate.Value < _curDate)
                                            x.Status = Commons.EStatusAccountFunction.Expired.ToString();
                                        else
                                            x.Status = Commons.EStatusAccountFunction.Active.ToString();
                                    }
                                    else
                                        x.Status = Commons.EStatusAccountFunction.Active.ToString();
                                }
                                else
                                {
                                    x.Status = Commons.EStatusAccountFunction.Inactive.ToString();
                                }
                                //=======
                                if (!x.ActivateTime.HasValue)
                                    x.sActivateTime = "Not Yet Activated";
                                else
                                    x.sActivateTime = x.ActivateTime.Value.ToString(Commons.FormatDateTime);
                                //=======
                                if (!x.ExpiryDate.HasValue || DateTime.Compare(x.ExpiryDate.Value.Date, Commons.MinDate.Date) <= 0)
                                    x.sExpiryDate = "";
                                else
                                    x.sExpiryDate = x.ExpiryDate.Value.ToString(Commons.FormatDateTime);
                                //======
                                if (x.PeriodType == (byte)Commons.EPeriodType.OneTime)
                                    x.sExpiryDate = "Never";
                            });
                        }
                        //=========ListAccountManagement
                        if (model.ProductManagement.ListAccount != null && model.ProductManagement.ListAccount.Count > 0)
                        {
                            model.ProductManagement.ListAccount.ForEach(x =>
                            {
                                if (!x.ExpiryDate.HasValue)
                                    x.Status = Commons.EStatusAccountFunction.Inactive.ToString();
                                else
                                {
                                    DateTime _curDate = DateTime.Now;
                                    if (DateTime.Compare(x.ExpiryDate.Value.Date, Commons.MinDate.Date) > 0 && x.ExpiryDate.Value < _curDate)
                                        x.Status = Commons.EStatusAccountFunction.Expired.ToString();
                                    else
                                        x.Status = x.IsActive ? Commons.EStatusAccountFunction.Active.ToString() : Commons.EStatusAccountFunction.Inactive.ToString();
                                }

                                //======
                                if (x.PeriodType == (byte)Commons.EPeriodType.OneTime)
                                    x.sExpiryDate = "Never";
                                else
                                {
                                    if (x.ExpiryDate.HasValue && DateTime.Compare(x.ExpiryDate.Value.Date, Commons.MinDate.Date) > 0)
                                        x.sExpiryDate = x.ExpiryDate.Value.ToString(Commons.FormatDateTime);
                                    else
                                        x.sExpiryDate = "";
                                }
                                //======
                                if (x.ActivateTime.HasValue)
                                    x.sActivateTime = x.ActivateTime.Value.ToString(Commons.FormatDateTime);
                                else
                                    x.sActivateTime = "Not Yet Activated";
                            });
                        }
                        //=========List Additional Functions Management
                        if (model.ProductManagement.ListFunction != null && model.ProductManagement.ListFunction.Count > 0)
                        {
                            model.ProductManagement.ListFunction.ForEach(x =>
                            {
                                ////if (!x.ExpiryDate.HasValue)
                                ////    x.Status = Commons.EStatusAccountFunction.Inactive.ToString();
                                ////else
                                ////{
                                ////var minDate = new DateTime(1900, 01, 01, 12, 00, 00);
                                ////if (!x.ExpiryDate.Equals(minDate))
                                //if (DateTime.Compare(x.ExpiryDate.Value.Date, Commons.MinDate.Date) > 0)
                                //{
                                //    DateTime _curDate = DateTime.Now;
                                //    if (x.ExpiryDate.Value < _curDate)
                                //        x.Status = Commons.EStatusAccountFunction.Expired.ToString();
                                //    else
                                //    {
                                //        if (x.IsBlock)
                                //            x.Status = Commons.EStatusAccountFunction.Blocked.ToString();
                                //        else
                                //            x.Status = x.IsActive ? Commons.EStatusAccountFunction.Active.ToString() : Commons.EStatusAccountFunction.Inactive.ToString();
                                //    }
                                //}
                                //else
                                //{
                                //    x.Status = Commons.EStatusAccountFunction.Active.ToString();
                                //}

                                ////}
                                ////==========
                                //if (x.PeriodType == (byte)Commons.EPeriodType.OneTime)
                                //    x.sExpiryDate = "Never";
                                //else
                                //{
                                //    if (x.ExpiryDate.HasValue && DateTime.Compare(x.ExpiryDate.Value.Date, Commons.MinDate.Date) > 0)
                                //    {
                                //        x.sExpiryDate = x.ExpiryDate.Value.ToString(Commons.FormatDateTime);
                                //    } else
                                //    {
                                //        x.sExpiryDate = "";
                                //    }
                                //}

                                //if (x.ActivateTime.HasValue)
                                //    x.sActivateTime = x.ActivateTime.Value.ToString(Commons.FormatDateTime);
                                //else
                                //    x.sActivateTime = "Not Yet Activated";

                                x = GetAdditionFunctionInfoView(x);
                            });
                        }
                    }
                    model.IndexTab = 3;
                    return PartialView("_ItemProductAndPackages", model);
                }
                //return new HttpStatusCodeResult(HttpStatusCode.OK);
                else
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, msg);
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("EditProductsManagement: ", ex);
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        public ActionResult ChangeStatusAccount(string ID, string CustomerID)
        {
            try
            {
                string msg = "";
                var result = _factory.ChangeStatusAccount(ID, CurrentUser.UserId, ref msg);
                if (result)
                {
                    CustomerBaseModels model = new CustomerBaseModels();
                    model.CurrencySymbol = CurrentUser.CurrencySymbol;
                    model.CustomerDetail = GetDetail(CustomerID);
                    var Merchant = GetDetailMerchant(CustomerID);
                    if (Merchant.CustomerID == null)
                        Merchant.CustomerID = model.CustomerDetail.ID;
                    model.MerchantStore = Merchant;
                    //========
                    model.ProductManagement = _factory.GetListDataProductsManagement(CustomerID);
                    if (model.ProductManagement != null)
                    {
                        //=========ListHardwareService
                        if (model.ProductManagement.ListHardwareService != null && model.ProductManagement.ListHardwareService.Count > 0)
                        {
                            model.ProductManagement.ListHardwareService.ForEach(x =>
                            {
                                x.Status = ((Commons.EServiceAssetStatus)Enum.ToObject(typeof(Commons.EServiceAssetStatus), x.State)).ToString();
                                x.sAdditionType = ((Commons.EAdditionType)Enum.ToObject(typeof(Commons.EAdditionType), x.AdditionType)).ToString();
                                if (x.AdditionType == (byte)Commons.EAdditionType.Service)
                                    x.SerialNo = "";
                                else
                                {
                                    if (x.State == (byte)Commons.EServiceAssetStatus.New)
                                        x.SerialNo = "Updating";
                                }
                            });
                        }
                        //=========ListProductPackage
                        if (model.ProductManagement.ListProductPackage != null && model.ProductManagement.ListProductPackage.Count > 0)
                        {
                            model.ProductManagement.ListProductPackage.ForEach(x =>
                            {
                                x.sType = ((Commons.EProductType)Enum.ToObject(typeof(Commons.EProductType), x.Type)).ToString();
                                //if (!x.ExpiryDate.HasValue)
                                //    x.Status = Commons.EStatusAccountFunction.Inactive.ToString();
                                //else
                                //{
                                //    DateTime _curDate = DateTime.Now;
                                //    if (x.ExpiryDate < _curDate)
                                //        x.Status = Commons.EStatusAccountFunction.Expired.ToString();
                                //    else
                                //        x.Status = x.IsActive ? Commons.EStatusAccountFunction.Active.ToString() : Commons.EStatusAccountFunction.Inactive.ToString();
                                //}

                                if (x.IsActive)
                                {
                                    //var minDate = new DateTime(1900, 01, 01, 12, 00, 00);
                                    //if (!x.ExpiryDate.Equals(minDate))
                                    if (DateTime.Compare(x.ExpiryDate.Value.Date, Commons.MinDate.Date) > 0)
                                    {
                                        DateTime _curDate = DateTime.Now;
                                        if (x.ExpiryDate.Value < _curDate)
                                            x.Status = Commons.EStatusAccountFunction.Expired.ToString();
                                        else
                                            x.Status = Commons.EStatusAccountFunction.Active.ToString();
                                    }
                                    else
                                        x.Status = Commons.EStatusAccountFunction.Active.ToString();
                                }
                                else
                                {
                                    x.Status = Commons.EStatusAccountFunction.Inactive.ToString();
                                }
                                //=======
                                if (!x.ActivateTime.HasValue)
                                    x.sActivateTime = "Not Yet Activated";
                                else
                                    x.sActivateTime = x.ActivateTime.Value.ToString(Commons.FormatDateTime);
                                //=======
                                if (!x.ExpiryDate.HasValue || DateTime.Compare(x.ExpiryDate.Value.Date, Commons.MinDate.Date) <= 0)
                                    x.sExpiryDate = "";
                                else
                                    x.sExpiryDate = x.ExpiryDate.Value.ToString(Commons.FormatDateTime);
                                //======
                                if (x.PeriodType == (byte)Commons.EPeriodType.OneTime)
                                    x.sExpiryDate = "Never";
                            });
                        }
                        //=========ListAccountManagement
                        if (model.ProductManagement.ListAccount != null && model.ProductManagement.ListAccount.Count > 0)
                        {
                            model.ProductManagement.ListAccount.ForEach(x =>
                            {
                                if (!x.ExpiryDate.HasValue)
                                    x.Status = Commons.EStatusAccountFunction.Inactive.ToString();
                                else
                                {
                                    DateTime _curDate = DateTime.Now;
                                    if (DateTime.Compare(x.ExpiryDate.Value.Date, Commons.MinDate.Date) > 0 && x.ExpiryDate.Value < _curDate)
                                        x.Status = Commons.EStatusAccountFunction.Expired.ToString();
                                    else
                                        x.Status = x.IsActive ? Commons.EStatusAccountFunction.Active.ToString() : Commons.EStatusAccountFunction.Inactive.ToString();
                                }

                                //======
                                if (x.PeriodType == (byte)Commons.EPeriodType.OneTime)
                                    x.sExpiryDate = "Never";
                                else
                                {
                                    if (x.ExpiryDate.HasValue && DateTime.Compare(x.ExpiryDate.Value.Date, Commons.MinDate.Date) > 0)
                                        x.sExpiryDate = x.ExpiryDate.Value.ToString(Commons.FormatDateTime);
                                    else
                                        x.sExpiryDate = "";
                                }
                                //======
                                if (x.ActivateTime.HasValue)
                                    x.sActivateTime = x.ActivateTime.Value.ToString(Commons.FormatDateTime);
                                else
                                    x.sActivateTime = "Not Yet Activated";
                            });
                        }
                        //=========List Additional Functions Management
                        if (model.ProductManagement.ListFunction != null && model.ProductManagement.ListFunction.Count > 0)
                        {
                            model.ProductManagement.ListFunction.ForEach(x =>
                            {
                                ////if (!x.ExpiryDate.HasValue)
                                ////    x.Status = Commons.EStatusAccountFunction.Inactive.ToString();
                                ////else
                                ////{
                                ////var minDate = new DateTime(1900, 01, 01, 12, 00, 00);
                                ////if (!x.ExpiryDate.Equals(minDate))
                                //if (DateTime.Compare(x.ExpiryDate.Value.Date, Commons.MinDate.Date) > 0)
                                //{
                                //    DateTime _curDate = DateTime.Now;
                                //    if (x.ExpiryDate.Value < _curDate)
                                //        x.Status = Commons.EStatusAccountFunction.Expired.ToString();
                                //    else
                                //    {
                                //        if (x.IsBlock)
                                //            x.Status = Commons.EStatusAccountFunction.Blocked.ToString();
                                //        else
                                //            x.Status = x.IsActive ? Commons.EStatusAccountFunction.Active.ToString() : Commons.EStatusAccountFunction.Inactive.ToString();
                                //    }
                                //}
                                //else
                                //{
                                //    x.Status = Commons.EStatusAccountFunction.Active.ToString();
                                //}

                                ////}
                                ////==========
                                //if (x.PeriodType == (byte)Commons.EPeriodType.OneTime)
                                //    x.sExpiryDate = "Never";
                                //else
                                //{
                                //    if (x.ExpiryDate.HasValue && DateTime.Compare(x.ExpiryDate.Value.Date, Commons.MinDate.Date) > 0)
                                //    {
                                //        x.sExpiryDate = x.ExpiryDate.Value.ToString(Commons.FormatDateTime);
                                //    } else
                                //    {
                                //        x.sExpiryDate = "";
                                //    }
                                //}

                                //if (x.ActivateTime.HasValue)
                                //    x.sActivateTime = x.ActivateTime.Value.ToString(Commons.FormatDateTime);
                                //else
                                //    x.sActivateTime = "Not Yet Activated";

                                x = GetAdditionFunctionInfoView(x);
                            });
                        }
                    }
                    model.IndexTab = 3;
                    //return PartialView("_ItemProductFunction", model);
                    return PartialView("_ItemAccounts", model);
                }
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, msg);
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("EditProductsManagement: ", ex);
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        public ActionResult ChangeStatusFunction(string ID, string CustomerID)
        {
            try
            {
                string msg = "";
                var result = _factory.ChangeStatusFunction(ID, CurrentUser.UserId, ref msg);
                if (result)
                {
                    CustomerBaseModels model = new CustomerBaseModels();
                    model.CurrencySymbol = CurrentUser.CurrencySymbol;
                    model.CustomerDetail = GetDetail(CustomerID);
                    var Merchant = GetDetailMerchant(CustomerID);
                    if (Merchant.CustomerID == null)
                        Merchant.CustomerID = model.CustomerDetail.ID;
                    model.MerchantStore = Merchant;
                    //========
                    model.ProductManagement = _factory.GetListDataProductsManagement(CustomerID);
                    if (model.ProductManagement != null)
                    {
                        //=========ListHardwareService
                        if (model.ProductManagement.ListHardwareService != null && model.ProductManagement.ListHardwareService.Count > 0)
                        {
                            model.ProductManagement.ListHardwareService.ForEach(x =>
                            {
                                x.Status = ((Commons.EServiceAssetStatus)Enum.ToObject(typeof(Commons.EServiceAssetStatus), x.State)).ToString();
                                x.sAdditionType = ((Commons.EAdditionType)Enum.ToObject(typeof(Commons.EAdditionType), x.AdditionType)).ToString();
                                if (x.AdditionType == (byte)Commons.EAdditionType.Service)
                                    x.SerialNo = "";
                                else
                                {
                                    if (x.State == (byte)Commons.EServiceAssetStatus.New)
                                        x.SerialNo = "Updating";
                                }
                            });
                        }
                        //=========ListProductPackage
                        if (model.ProductManagement.ListProductPackage != null && model.ProductManagement.ListProductPackage.Count > 0)
                        {
                            model.ProductManagement.ListProductPackage.ForEach(x =>
                            {
                                x.sType = ((Commons.EProductType)Enum.ToObject(typeof(Commons.EProductType), x.Type)).ToString();
                                //if (!x.ExpiryDate.HasValue)
                                //    x.Status = Commons.EStatusAccountFunction.Inactive.ToString();
                                //else
                                //{
                                //    DateTime _curDate = DateTime.Now;
                                //    if (x.ExpiryDate < _curDate)
                                //        x.Status = Commons.EStatusAccountFunction.Expired.ToString();
                                //    else
                                //        x.Status = x.IsActive ? Commons.EStatusAccountFunction.Active.ToString() : Commons.EStatusAccountFunction.Inactive.ToString();
                                //}

                                if (x.IsActive)
                                {
                                    //var minDate = new DateTime(1900, 01, 01, 12, 00, 00);
                                    //if (!x.ExpiryDate.Equals(minDate))
                                    if (DateTime.Compare(x.ExpiryDate.Value.Date, Commons.MinDate.Date) > 0)
                                    {
                                        DateTime _curDate = DateTime.Now;
                                        if (x.ExpiryDate.Value < _curDate)
                                            x.Status = Commons.EStatusAccountFunction.Expired.ToString();
                                        else
                                            x.Status = Commons.EStatusAccountFunction.Active.ToString();
                                    }
                                    else
                                        x.Status = Commons.EStatusAccountFunction.Active.ToString();
                                }
                                else
                                {
                                    x.Status = Commons.EStatusAccountFunction.Inactive.ToString();
                                }
                                //=======
                                if (!x.ActivateTime.HasValue)
                                    x.sActivateTime = "Not Yet Activated";
                                else
                                    x.sActivateTime = x.ActivateTime.Value.ToString(Commons.FormatDateTime);
                                //=======
                                if (!x.ExpiryDate.HasValue || DateTime.Compare(x.ExpiryDate.Value.Date, Commons.MinDate.Date) <= 0)
                                    x.sExpiryDate = "";
                                else
                                    x.sExpiryDate = x.ExpiryDate.Value.ToString(Commons.FormatDateTime);
                                //======
                                if (x.PeriodType == (byte)Commons.EPeriodType.OneTime)
                                    x.sExpiryDate = "Never";
                            });
                        }
                        //=========ListAccountManagement
                        if (model.ProductManagement.ListAccount != null && model.ProductManagement.ListAccount.Count > 0)
                        {
                            model.ProductManagement.ListAccount.ForEach(x =>
                            {
                                if (!x.ExpiryDate.HasValue)
                                    x.Status = Commons.EStatusAccountFunction.Inactive.ToString();
                                else
                                {
                                    DateTime _curDate = DateTime.Now;
                                    if (DateTime.Compare(x.ExpiryDate.Value.Date, Commons.MinDate.Date) > 0 && x.ExpiryDate.Value < _curDate)
                                        x.Status = Commons.EStatusAccountFunction.Expired.ToString();
                                    else
                                        x.Status = x.IsActive ? Commons.EStatusAccountFunction.Active.ToString() : Commons.EStatusAccountFunction.Inactive.ToString();
                                }

                                //======
                                if (x.PeriodType == (byte)Commons.EPeriodType.OneTime)
                                    x.sExpiryDate = "Never";
                                else
                                {
                                    if (x.ExpiryDate.HasValue && DateTime.Compare(x.ExpiryDate.Value.Date, Commons.MinDate.Date) > 0)
                                        x.sExpiryDate = x.ExpiryDate.Value.ToString(Commons.FormatDateTime);
                                    else
                                        x.sExpiryDate = "";
                                }
                                //======
                                if (x.ActivateTime.HasValue)
                                    x.sActivateTime = x.ActivateTime.Value.ToString(Commons.FormatDateTime);
                                else
                                    x.sActivateTime = "Not Yet Activated";
                            });
                        }
                        //=========List Additional Functions Management
                        if (model.ProductManagement.ListFunction != null && model.ProductManagement.ListFunction.Count > 0)
                        {
                            model.ProductManagement.ListFunction.ForEach(x =>
                            {
                                ////if (!x.ExpiryDate.HasValue)
                                ////    x.Status = Commons.EStatusAccountFunction.Inactive.ToString();
                                ////else
                                ////{
                                ////var minDate = new DateTime(1900, 01, 01, 12, 00, 00);
                                ////if (!x.ExpiryDate.Equals(minDate))
                                //if (DateTime.Compare(x.ExpiryDate.Value.Date, Commons.MinDate.Date) > 0)
                                //{
                                //    DateTime _curDate = DateTime.Now;
                                //    if (x.ExpiryDate.Value < _curDate)
                                //        x.Status = Commons.EStatusAccountFunction.Expired.ToString();
                                //    else
                                //    {
                                //        if (x.IsBlock)
                                //            x.Status = Commons.EStatusAccountFunction.Blocked.ToString();
                                //        else
                                //            x.Status = x.IsActive ? Commons.EStatusAccountFunction.Active.ToString() : Commons.EStatusAccountFunction.Inactive.ToString();
                                //    }
                                //}
                                //else
                                //{
                                //    x.Status = Commons.EStatusAccountFunction.Active.ToString();
                                //}

                                ////}
                                ////==========
                                //if (x.PeriodType == (byte)Commons.EPeriodType.OneTime)
                                //    x.sExpiryDate = "Never";
                                //else
                                //{
                                //    if (x.ExpiryDate.HasValue && DateTime.Compare(x.ExpiryDate.Value.Date, Commons.MinDate.Date) > 0)
                                //    {
                                //        x.sExpiryDate = x.ExpiryDate.Value.ToString(Commons.FormatDateTime);
                                //    } else
                                //    {
                                //        x.sExpiryDate = "";
                                //    }
                                //}

                                //if (x.ActivateTime.HasValue)
                                //    x.sActivateTime = x.ActivateTime.Value.ToString(Commons.FormatDateTime);
                                //else
                                //    x.sActivateTime = "Not Yet Activated";

                                x = GetAdditionFunctionInfoView(x);
                            });
                        }
                    }
                    model.IndexTab = 3;
                    return PartialView("_ItemProductFunction", model);
                }
                else
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, msg);
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("EditProductsManagement: ", ex);
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        public ActionResult ChangeBlockFunction(string ID, string CustomerID)
        {
            try
            {
                string msg = "";
                var result = _factory.ChangeBlockFunction(ID, CurrentUser.UserId, ref msg);
                if (result)
                {
                    CustomerBaseModels model = new CustomerBaseModels();
                    model.CurrencySymbol = CurrentUser.CurrencySymbol;
                    model.CustomerDetail = GetDetail(CustomerID);
                    var Merchant = GetDetailMerchant(CustomerID);
                    if (Merchant.CustomerID == null)
                        Merchant.CustomerID = model.CustomerDetail.ID;
                    model.MerchantStore = Merchant;
                    //========
                    model.ProductManagement = _factory.GetListDataProductsManagement(CustomerID);
                    if (model.ProductManagement != null)
                    {
                        //=========ListHardwareService
                        if (model.ProductManagement.ListHardwareService != null && model.ProductManagement.ListHardwareService.Count > 0)
                        {
                            model.ProductManagement.ListHardwareService.ForEach(x =>
                            {
                                x.Status = ((Commons.EServiceAssetStatus)Enum.ToObject(typeof(Commons.EServiceAssetStatus), x.State)).ToString();
                                x.sAdditionType = ((Commons.EAdditionType)Enum.ToObject(typeof(Commons.EAdditionType), x.AdditionType)).ToString();
                                if (x.AdditionType == (byte)Commons.EAdditionType.Service)
                                    x.SerialNo = "";
                                else
                                {
                                    if (x.State == (byte)Commons.EServiceAssetStatus.New)
                                        x.SerialNo = "Updating";
                                }
                            });
                        }
                        //=========ListProductPackage
                        if (model.ProductManagement.ListProductPackage != null && model.ProductManagement.ListProductPackage.Count > 0)
                        {
                            model.ProductManagement.ListProductPackage.ForEach(x =>
                            {
                                x.sType = ((Commons.EProductType)Enum.ToObject(typeof(Commons.EProductType), x.Type)).ToString();
                                //if (!x.ExpiryDate.HasValue)
                                //    x.Status = Commons.EStatusAccountFunction.Inactive.ToString();
                                //else
                                //{
                                //    DateTime _curDate = DateTime.Now;
                                //    if (x.ExpiryDate < _curDate)
                                //        x.Status = Commons.EStatusAccountFunction.Expired.ToString();
                                //    else
                                //        x.Status = x.IsActive ? Commons.EStatusAccountFunction.Active.ToString() : Commons.EStatusAccountFunction.Inactive.ToString();
                                //}

                                if (x.IsActive)
                                {
                                    //var minDate = new DateTime(1900, 01, 01, 12, 00, 00);
                                    //if (!x.ExpiryDate.Equals(minDate))
                                    if (DateTime.Compare(x.ExpiryDate.Value.Date, Commons.MinDate.Date) > 0)
                                    {
                                        DateTime _curDate = DateTime.Now;
                                        if (x.ExpiryDate.Value < _curDate)
                                            x.Status = Commons.EStatusAccountFunction.Expired.ToString();
                                        else
                                            x.Status = Commons.EStatusAccountFunction.Active.ToString();
                                    }
                                    else
                                        x.Status = Commons.EStatusAccountFunction.Active.ToString();
                                }
                                else
                                {
                                    x.Status = Commons.EStatusAccountFunction.Inactive.ToString();
                                }
                                //=======
                                if (!x.ActivateTime.HasValue)
                                    x.sActivateTime = "Not Yet Activated";
                                else
                                    x.sActivateTime = x.ActivateTime.Value.ToString(Commons.FormatDateTime);
                                //=======
                                if (!x.ExpiryDate.HasValue || DateTime.Compare(x.ExpiryDate.Value.Date, Commons.MinDate.Date) <= 0)
                                    x.sExpiryDate = "";
                                else
                                    x.sExpiryDate = x.ExpiryDate.Value.ToString(Commons.FormatDateTime);
                                //======
                                if (x.PeriodType == (byte)Commons.EPeriodType.OneTime)
                                    x.sExpiryDate = "Never";
                            });
                        }
                        //=========ListAccountManagement
                        if (model.ProductManagement.ListAccount != null && model.ProductManagement.ListAccount.Count > 0)
                        {
                            model.ProductManagement.ListAccount.ForEach(x =>
                            {
                                if (!x.ExpiryDate.HasValue)
                                    x.Status = Commons.EStatusAccountFunction.Inactive.ToString();
                                else
                                {
                                    DateTime _curDate = DateTime.Now;
                                    if (DateTime.Compare(x.ExpiryDate.Value.Date, Commons.MinDate.Date) > 0 && x.ExpiryDate.Value < _curDate)
                                        x.Status = Commons.EStatusAccountFunction.Expired.ToString();
                                    else
                                        x.Status = x.IsActive ? Commons.EStatusAccountFunction.Active.ToString() : Commons.EStatusAccountFunction.Inactive.ToString();
                                }

                                //======
                                if (x.PeriodType == (byte)Commons.EPeriodType.OneTime)
                                    x.sExpiryDate = "Never";
                                else
                                {
                                    if (x.ExpiryDate.HasValue && DateTime.Compare(x.ExpiryDate.Value.Date, Commons.MinDate.Date) > 0)
                                        x.sExpiryDate = x.ExpiryDate.Value.ToString(Commons.FormatDateTime);
                                    else
                                        x.sExpiryDate = "";
                                }
                                //======
                                if (x.ActivateTime.HasValue)
                                    x.sActivateTime = x.ActivateTime.Value.ToString(Commons.FormatDateTime);
                                else
                                    x.sActivateTime = "Not Yet Activated";
                            });
                        }
                        //=========List Additional Functions Management
                        if (model.ProductManagement.ListFunction != null && model.ProductManagement.ListFunction.Count > 0)
                        {
                            model.ProductManagement.ListFunction.ForEach(x =>
                            {
                                ////if (!x.ExpiryDate.HasValue)
                                ////    x.Status = Commons.EStatusAccountFunction.Inactive.ToString();
                                ////else
                                ////{
                                ////var minDate = new DateTime(1900, 01, 01, 12, 00, 00);
                                ////if (!x.ExpiryDate.Equals(minDate))
                                //if (DateTime.Compare(x.ExpiryDate.Value.Date, Commons.MinDate.Date) > 0)
                                //{
                                //    DateTime _curDate = DateTime.Now;
                                //    if (x.ExpiryDate.Value < _curDate)
                                //        x.Status = Commons.EStatusAccountFunction.Expired.ToString();
                                //    else
                                //    {
                                //        if (x.IsBlock)
                                //            x.Status = Commons.EStatusAccountFunction.Blocked.ToString();
                                //        else
                                //            x.Status = x.IsActive ? Commons.EStatusAccountFunction.Active.ToString() : Commons.EStatusAccountFunction.Inactive.ToString();
                                //    }
                                //}
                                //else
                                //{
                                //    x.Status = Commons.EStatusAccountFunction.Active.ToString();
                                //}

                                ////}
                                ////==========
                                //if (x.PeriodType == (byte)Commons.EPeriodType.OneTime)
                                //    x.sExpiryDate = "Never";
                                //else
                                //{
                                //    if (x.ExpiryDate.HasValue && DateTime.Compare(x.ExpiryDate.Value.Date, Commons.MinDate.Date) > 0)
                                //    {
                                //        x.sExpiryDate = x.ExpiryDate.Value.ToString(Commons.FormatDateTime);
                                //    } else
                                //    {
                                //        x.sExpiryDate = "";
                                //    }
                                //}

                                //if (x.ActivateTime.HasValue)
                                //    x.sActivateTime = x.ActivateTime.Value.ToString(Commons.FormatDateTime);
                                //else
                                //    x.sActivateTime = "Not Yet Activated";

                                x = GetAdditionFunctionInfoView(x);
                            });
                        }
                    }
                    model.IndexTab = 3;
                    return PartialView("_ItemProductFunction", model);
                }
                else
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, msg);
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("EditProductsManagement: ", ex);
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        public ActionResult GetProductDetail(string Id, string CustomerID)
        {
            var models = new ProductPackageAdminModels();
            try
            {
                models = _factory.GetListDataProductsPackage(Id);
                var _ListStore = _factory.GetListStore(CustomerID);
                _ListStore = _ListStore.Where(x => x.StoreType != 0).ToList();
                if (models.Type == (int)Commons.EProductType.Product)
                {
                    if (models.Category.Type == (int)Commons.EType.NuPOS || models.Category.Type == (int)Commons.EType.NuKiosk || models.Category.Type == (int)Commons.EType.NuDisplay)
                        _ListStore = _ListStore.Where(x => x.StoreType == (int)Commons.EStoreType.FnB).ToList();
                    else
                        if (models.Category.Type == (int)Commons.EType.POZZ || models.Category.Type == (int)Commons.EType.POZZ_Display || models.Category.Type == (int)Commons.EType.POZZ_Kiosk)
                        _ListStore = _ListStore.Where(x => x.StoreType == (int)Commons.EStoreType.Beauty).ToList();
                    if (_ListStore != null && _ListStore.Count > 0)
                    {
                        AssignToStore _AssignToStore = new AssignToStore();
                        _AssignToStore.AssetID = Id;
                        int i = 0;
                        foreach (var item in _ListStore)
                        {
                            bool _isApply = false;
                            string _industry = "";
                            _industry = item.StoreType == 1 ? Commons.StoreTypeFnB : item.StoreType == 2 ? Commons.StoreTypeBeauty : "";
                            foreach (var _item in models.ListStoreApply)
                            {
                                if (_item.StoreID == item.ID)
                                    _isApply = true;
                            }
                            _AssignToStore.ListAssignStore.Add(new AssignStore()
                            {
                                Offset = i,
                                StoreName = item.Name,
                                StoreID = item.ID,
                                Address = item.Address,
                                IsApply = _isApply,
                                ExpiredDate = item.ExpiredDate,
                                StoreType = item.StoreType,
                                Industry = _industry,
                                ActivatedDate = item.ActivatedDate
                            });
                            i++;
                        }
                        _AssignToStore.ListAssignStore.ForEach(x =>
                        {
                            if (x.ActivatedDate.HasValue)
                                x.ActivatedDate = CommonHelper.ConvertToLocalTime(x.ActivatedDate.Value);
                            if (x.ExpiredDate.HasValue)
                                x.ExpiredDate = CommonHelper.ConvertToLocalTime(x.ExpiredDate.Value);
                        });
                        models.AssignStore = _AssignToStore;
                    }
                }
                else
                {
                    if (models.Type == (int)Commons.EProductType.Package)
                    {
                        foreach (var _Product in models.ListProduct)
                        {
                            var _liststores = _ListStore;
                            if (_Product.Category.Type == (int)Commons.EType.NuPOS || _Product.Category.Type == (int)Commons.EType.NuKiosk || _Product.Category.Type == (int)Commons.EType.NuDisplay)
                                _liststores = _liststores.Where(x => x.StoreType == (int)Commons.EStoreType.FnB).ToList();
                            else
                                if (_Product.Category.Type == (int)Commons.EType.POZZ || _Product.Category.Type == (int)Commons.EType.POZZ_Display || _Product.Category.Type == (int)Commons.EType.POZZ_Kiosk)
                                _liststores = _liststores.Where(x => x.StoreType == (int)Commons.EStoreType.Beauty).ToList();
                            if (_liststores != null && _liststores.Count > 0)
                            {
                                AssignToStore _AssignToStore = new AssignToStore();
                                _AssignToStore.AssetID = _Product.AssetID;
                                int i = 0;
                                foreach (var item in _liststores)
                                {
                                    bool _isApply = false;
                                    string _industry = "";
                                    _industry = item.StoreType == 1 ? Commons.StoreTypeFnB : item.StoreType == 2 ? Commons.StoreTypeBeauty : "";
                                    foreach (var _item in _Product.ListStoreApply)
                                    {
                                        if (_item.StoreID == item.ID)
                                            _isApply = true;
                                    }
                                    _AssignToStore.ListAssignStore.Add(new AssignStore()
                                    {
                                        Offset = i,
                                        StoreName = item.Name,
                                        StoreID = item.ID,
                                        Address = item.Address,
                                        IsApply = _isApply,
                                        ExpiredDate = item.ExpiredDate,
                                        StoreType = item.StoreType,
                                        Industry = _industry,
                                        ActivatedDate = item.ActivatedDate
                                    });
                                    i++;
                                }
                                _AssignToStore.ListAssignStore.ForEach(x =>
                                {
                                    if (x.ActivatedDate.HasValue)
                                        x.ActivatedDate = CommonHelper.ConvertToLocalTime(x.ActivatedDate.Value);
                                    if (x.ExpiredDate.HasValue)
                                        x.ExpiredDate = CommonHelper.ConvertToLocalTime(x.ExpiredDate.Value);
                                });
                                _Product.AssignStore = _AssignToStore;
                            }
                        }
                    }
                }
                models.fPeriod = models.PeriodType == (int)Commons.EPeriodType.Day ? (models.Period > 1 ? Commons.EPeriodType.Day.ToString() + "s" : Commons.EPeriodType.Day.ToString()) :
                                models.PeriodType == (int)Commons.EPeriodType.Month ? (models.Period > 1 ? Commons.EPeriodType.Month.ToString() + "s" : Commons.EPeriodType.Month.ToString()) :
                                models.PeriodType == (int)Commons.EPeriodType.None ? Commons.EPeriodType.None.ToString() :
                                models.PeriodType == (int)Commons.EPeriodType.OneTime ? Commons.EPeriodType.OneTime.ToString() :
                                (models.Period > 1 ? Commons.EPeriodType.Year.ToString() + "s" : Commons.EPeriodType.Year.ToString());

                models.ListComposite.ForEach(x =>
                {
                    x.fState = x.State == (int)Commons.EServiceAssetStatus.Done ? Commons.EServiceAssetStatus.Done.ToString() :
                               x.State == (int)Commons.EServiceAssetStatus.New ? Commons.EServiceAssetStatus.New.ToString() :
                               Commons.EServiceAssetStatus.Recalled.ToString();

                    x.fType = x.Type == (int)Commons.EAdditionType.Account ? Commons.EAdditionType.Account.ToString() :
                              x.Type == (int)Commons.EAdditionType.Function ? Commons.EAdditionType.Function.ToString() :
                              x.Type == (int)Commons.EAdditionType.Hardware ? Commons.EAdditionType.Hardware.ToString() :
                              x.Type == (int)Commons.EAdditionType.Location ? Commons.EAdditionType.Location.ToString() :
                              x.Type == (int)Commons.EAdditionType.Service ? Commons.EAdditionType.Service.ToString() :
                                        Commons.EAdditionType.Software.ToString();
                });

                models.ListAddition.ForEach(x =>
                {
                    x.fState = x.State == (int)Commons.EServiceAssetStatus.Done ? Commons.EServiceAssetStatus.Done.ToString() :
                               x.State == (int)Commons.EServiceAssetStatus.New ? Commons.EServiceAssetStatus.New.ToString() :
                               Commons.EServiceAssetStatus.Recalled.ToString();

                    x.fType = x.Type == (int)Commons.EAdditionType.Account ? Commons.EAdditionType.Account.ToString() :
                              x.Type == (int)Commons.EAdditionType.Function ? Commons.EAdditionType.Function.ToString() :
                              x.Type == (int)Commons.EAdditionType.Hardware ? Commons.EAdditionType.Hardware.ToString() :
                              x.Type == (int)Commons.EAdditionType.Location ? Commons.EAdditionType.Location.ToString() :
                              x.Type == (int)Commons.EAdditionType.Service ? Commons.EAdditionType.Service.ToString() :
                                        Commons.EAdditionType.Software.ToString();
                });

            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("GetProductDetail", ex);
            }
            return PartialView("_ItemProductDetail", models);
        }
        public ActionResult AssignStoreToProduct(AssignToStore model)
        {
            try
            {
                if (model.ListAssignStore != null && model.ListAssignStore.Count > 0)
                {
                    List<StoreAssignProduct> _StoreAssignProduct = new List<StoreAssignProduct>();
                    foreach (var item in model.ListAssignStore.Where(x => x.IsApply))
                    {
                        _StoreAssignProduct.Add(new StoreAssignProduct
                        {
                            AssetID = model.AssetID,
                            StoreID = item.StoreID
                        });
                    }
                    string mess = "";
                    bool IsStoreAssignProduct = false;
                    var results = _factory.StoreAssignProduct(_StoreAssignProduct, model.AssetID, IsStoreAssignProduct, CurrentUser.UserId, ref mess);
                    return Json(results, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("Assign store to Product", ex);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ExtentProductMana(string AssetId, string ProductId)
        {
            var models = new ProductPackageAdminModels();
            var ArrPeriod = new Dictionary<int, string>();
            ArrPeriod.Add((int)Commons.EPeriodType.Day, Commons.EPeriodType.Day.ToString());
            ArrPeriod.Add((int)Commons.EPeriodType.Month, Commons.EPeriodType.Month.ToString());
            ArrPeriod.Add((int)Commons.EPeriodType.Year, Commons.EPeriodType.Year.ToString());
            ArrPeriod.Add((int)Commons.EPeriodType.OneTime, Commons.EPeriodType.OneTime.ToString());
            try
            {
                models = _factory.GetListDataProductsPackage(AssetId);

                models.fPeriod = models.PeriodType == (int)Commons.EPeriodType.Day ? (models.Period > 1 ? Commons.EPeriodType.Day.ToString() + "s" : Commons.EPeriodType.Day.ToString()) :
                                models.PeriodType == (int)Commons.EPeriodType.Month ? (models.Period > 1 ? Commons.EPeriodType.Month.ToString() + "s" : Commons.EPeriodType.Month.ToString()) :
                                models.PeriodType == (int)Commons.EPeriodType.None ? Commons.EPeriodType.None.ToString() :
                                models.PeriodType == (int)Commons.EPeriodType.OneTime ? Commons.EPeriodType.OneTime.ToString() :
                               (models.Period > 1 ? Commons.EPeriodType.Year.ToString() + "s" : Commons.EPeriodType.Year.ToString());

                models.ListComposite.ForEach(x =>
                {
                    x.fState = x.State == (int)Commons.EServiceAssetStatus.Done ? Commons.EServiceAssetStatus.Done.ToString() :
                               x.State == (int)Commons.EServiceAssetStatus.New ? Commons.EServiceAssetStatus.New.ToString() :
                               Commons.EServiceAssetStatus.Recalled.ToString();

                    x.fType = x.Type == (int)Commons.EAdditionType.Account ? Commons.EAdditionType.Account.ToString() :
                              x.Type == (int)Commons.EAdditionType.Function ? Commons.EAdditionType.Function.ToString() :
                              x.Type == (int)Commons.EAdditionType.Hardware ? Commons.EAdditionType.Hardware.ToString() :
                              x.Type == (int)Commons.EAdditionType.Location ? Commons.EAdditionType.Location.ToString() :
                              x.Type == (int)Commons.EAdditionType.Service ? Commons.EAdditionType.Service.ToString() :
                                        Commons.EAdditionType.Software.ToString();
                });

                models.ListAddition.ForEach(x =>
                {
                    x.fState = x.State == (int)Commons.EServiceAssetStatus.Done ? Commons.EServiceAssetStatus.Done.ToString() :
                               x.State == (int)Commons.EServiceAssetStatus.New ? Commons.EServiceAssetStatus.New.ToString() :
                               Commons.EServiceAssetStatus.Recalled.ToString();

                    x.fType = x.Type == (int)Commons.EAdditionType.Account ? Commons.EAdditionType.Account.ToString() :
                              x.Type == (int)Commons.EAdditionType.Function ? Commons.EAdditionType.Function.ToString() :
                              x.Type == (int)Commons.EAdditionType.Hardware ? Commons.EAdditionType.Hardware.ToString() :
                              x.Type == (int)Commons.EAdditionType.Location ? Commons.EAdditionType.Location.ToString() :
                              x.Type == (int)Commons.EAdditionType.Service ? Commons.EAdditionType.Service.ToString() :
                                        Commons.EAdditionType.Software.ToString();
                });

                if (models.Type == (int)Commons.EProductType.Package)
                {
                    if (models.ListProduct != null)
                    {
                        var _product = models.ListProduct
                                                  .Where(x => x.IsActive)
                                                  .Select(x => new SelectListItem
                                                  {
                                                      Value = x.ItemID,
                                                      Text = x.ItemName
                                                  })
                                                  .ToList();
                        models.ListItemProduct = _product;
                    }
                    else
                    {
                        var _product = new List<SelectListItem>()
                        {
                            new SelectListItem
                            {
                                Value = models.ItemID,
                                Text = models.ItemName
                            }
                        };
                        models.ListItemProduct = _product;
                    }

                }
                else
                {
                    var _product = new List<SelectListItem>()
                        {
                            new SelectListItem
                            {
                                Value = models.ItemID,
                                Text = models.ItemName
                            }
                        };
                    models.ListItemProduct = _product;
                }

                var ListPrice = _factory.GetPriceProduct(ProductId);
                if (ListPrice != null)
                {
                    var CurrencySymbol = CurrentUser.CurrencySymbol.ToString();
                    ListPrice = ListPrice.Where(x => x.IsExtend)
                                            .Select(x => new ProductPriceAdminModels()
                                            {
                                                ID = x.ID,
                                                IsExtend = x.IsExtend,
                                                Period = x.Period,
                                                PeriodType = x.PeriodType,
                                                Price = x.Price,
                                                PeriodName = ArrPeriod.ContainsKey(x.PeriodType) ? ArrPeriod.FirstOrDefault(y => y.Key == x.PeriodType).Value : "",
                                                CurrencySymbol = CurrencySymbol
                                            })
                                            .ToList();
                    models.ListPrices = ListPrice;
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("ExtentProductMana", ex);
            }

            return PartialView("_ItemProductExtend", models);
        }
        public ActionResult CheckOutProductMana(string CustomerID, string AssetId, string Period, string PeriodType, string Price, string ProductAppliOn, DateTime? ExpiryDate)
        {
            var models = new OrderDetailModels();
            try
            {
                models = _factory.GetOrderDataProducts(CustomerID, AssetId, Period, PeriodType, Price, ProductAppliOn);
                NSCSC.Shared.Models.CRM.Customers.CustomerDetailModels model = _factory.GetDetail(CustomerID);
                models.CustomerDetail.Name = model.Name;
                models.CustomerDetail.Email = model.Email;
                models.CustomerDetail.Phone = model.Phone;
                models.ImageURL = model.ImageURL;
                models.CustomerDetail.MerchantName = model.MerchantName;
                models.TotalBill = models.Total + models.Tax;
                models.ExpiryDate = ExpiryDate;
                models.CurrencySymbol = CurrentUser.CurrencySymbol;
                List<PaymentMethodModels> LstPayment = new List<PaymentMethodModels>();
                LstPayment = _factoryPayment.GetData();
                LstPayment = LstPayment.Where(x => x.IsActive).ToList();
                models.LstPayment.AddRange(LstPayment);
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("CheckoutProductMana", ex);
            }
            return PartialView("_ItemProductCheckOut", models);
        }

        //PayPal
        public ActionResult OrderPayPal(string OrderID)
        {
            YourCartFactory _factoryO = new YourCartFactory();
            var data = _factoryO.GetOrderDetail(OrderID, true);
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
                    items.Add(new NSItem
                    {
                        Name = "Tax",
                        Price = data.Tax,
                        Quantity = 1
                    });
                }
                return Json(items, JsonRequestBehavior.AllowGet);
            }
            return new HttpStatusCodeResult(HttpStatusCode.NotFound, "");
        }

        public ActionResult AddPayMethod(string OrderID, string PaymentMethodID, string AmountPay)
        {
            var model = new HistoriesPaid();
            List<PaymentDetail> LstpayItem = new List<PaymentDetail>();
            try
            {
                double amountPay = 0;
                amountPay = Convert.ToDouble(AmountPay);
                string OrderIDReturn = "";
                string msg = "";
                model = _factory.OrderPay(OrderID, PaymentMethodID, amountPay, CurrentUser.UserId, ref OrderIDReturn, ref msg);
                model.Received = model.TotalAmount - model.Remaining;
                model.CurrencySymbol = CurrentUser.CurrencySymbol;
                if (model.ListPaidMethod != null && model.ListPaidMethod.Count() > 0)
                {
                    model.ListPaidMethod.ForEach(s => s.CurrencySymbol = CurrentUser.CurrencySymbol);
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("AddPayMethod", ex);
            }
            if (model.Remaining >= 0)
            {
                return PartialView("_ItemAmount", model);
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        public ActionResult DoneOrder(string OrderID, DateTime? ExpiryDate, double Period, int PeriodType, string ProductAppliedOnName)
        {
            try
            {
                DonePayCheckOut model = new DonePayCheckOut();
                string msg = "";
                var result = _factory.DoneOrderPay(OrderID, CurrentUser.UserId, ref msg);
                if (result)
                {
                    model.ExpiryDate = ExpiryDate;
                    model.Period = Period;
                    model.PeriodType = PeriodType;
                    model.ProductAppliedName = ProductAppliedOnName;
                    if (PeriodType == Convert.ToInt32(Commons.EPeriodType.Day.ToString("d")))
                    {
                        model.PeriodName = Commons.EPeriodType.Day.ToString();
                        if (ExpiryDate < DateTime.Now)
                        {
                            model.ExpiryDate = DateTime.Now.AddDays(Period);
                        }
                        else
                        {
                            model.ExpiryDate = Convert.ToDateTime(ExpiryDate).AddDays(Period);
                        }
                    }
                    if (PeriodType == Convert.ToInt32(Commons.EPeriodType.Month.ToString("d")))
                    {
                        model.PeriodName = Commons.EPeriodType.Month.ToString();
                        if (ExpiryDate < DateTime.Now)
                        {
                            model.ExpiryDate = DateTime.Now.AddMonths(Convert.ToInt32(Period));
                        }
                        else
                        {
                            model.ExpiryDate = Convert.ToDateTime(ExpiryDate).AddMonths(Convert.ToInt32(Period));
                        }
                    }
                    if (PeriodType == Convert.ToInt32(Commons.EPeriodType.Year.ToString("d")))
                    {
                        model.PeriodName = Commons.EPeriodType.Year.ToString();
                        if (ExpiryDate < DateTime.Now)
                        {
                            model.ExpiryDate = DateTime.Now.AddYears(Convert.ToInt32(Period));
                        }
                        else
                        {
                            model.ExpiryDate = Convert.ToDateTime(ExpiryDate).AddYears(Convert.ToInt32(Period));
                        }
                    }
                    if (PeriodType == Convert.ToInt32(Commons.EPeriodType.OneTime.ToString("d")))
                    {
                        model.PeriodName = Commons.EPeriodType.OneTime.ToString();
                    }
                    if (PeriodType == Convert.ToInt32(Commons.EPeriodType.None.ToString("d")))
                    {
                        model.PeriodName = Commons.EPeriodType.None.ToString();
                    }
                    return PartialView("_ModalDonePay", model);
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, msg);
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("CancelOrder : ", ex);
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        // OrderPay
        public ActionResult OrderPay(string OrderID, string PaymentMethodID, double AmountPay, string TransactionId, string Note, DateTime? ExpiryDate, double Period, int PeriodType, string ProductAppliedOnName)
        {
            string msg = "";
            var OrderIDReturn = "";
            YourCartFactory _factoryO = new YourCartFactory();
            bool result = _factoryO.OrderPay(OrderID, PaymentMethodID, AmountPay, CurrentUser.UserId, TransactionId, Note, ref OrderIDReturn, ref msg);
            if (result)
            {
                DonePayCheckOut model = new DonePayCheckOut();
                model.ExpiryDate = ExpiryDate;
                model.Period = Period;
                model.PeriodType = PeriodType;
                model.ProductAppliedName = ProductAppliedOnName;
                if (PeriodType == Convert.ToInt32(Commons.EPeriodType.Day.ToString("d")))
                {
                    model.PeriodName = Commons.EPeriodType.Day.ToString();
                    if (ExpiryDate < DateTime.Now)
                    {
                        model.ExpiryDate = DateTime.Now.AddDays(Period);
                    }
                    else
                    {
                        model.ExpiryDate = Convert.ToDateTime(ExpiryDate).AddDays(Period);
                    }
                }
                if (PeriodType == Convert.ToInt32(Commons.EPeriodType.Month.ToString("d")))
                {
                    model.PeriodName = Commons.EPeriodType.Month.ToString();
                    if (ExpiryDate < DateTime.Now)
                    {
                        model.ExpiryDate = DateTime.Now.AddMonths(Convert.ToInt32(Period));
                    }
                    else
                    {
                        model.ExpiryDate = Convert.ToDateTime(ExpiryDate).AddMonths(Convert.ToInt32(Period));
                    }
                }
                if (PeriodType == Convert.ToInt32(Commons.EPeriodType.Year.ToString("d")))
                {
                    model.PeriodName = Commons.EPeriodType.Year.ToString();
                    if (ExpiryDate < DateTime.Now)
                    {
                        model.ExpiryDate = DateTime.Now.AddYears(Convert.ToInt32(Period));
                    }
                    else
                    {
                        model.ExpiryDate = Convert.ToDateTime(ExpiryDate).AddYears(Convert.ToInt32(Period));
                    }
                }
                if (PeriodType == Convert.ToInt32(Commons.EPeriodType.OneTime.ToString("d")))
                {
                    model.PeriodName = Commons.EPeriodType.OneTime.ToString();
                }
                if (PeriodType == Convert.ToInt32(Commons.EPeriodType.None.ToString("d")))
                {
                    model.PeriodName = Commons.EPeriodType.None.ToString();
                }
                return PartialView("_ModalDonePay", model);
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound, msg);
            }
        }

        public ActionResult CancelCheckout(string OrderID)
        {
            try
            {
                string msg = "";
                var result = _factory.OrderCancel(OrderID, ref msg);
                if (result)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.OK);
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, msg);
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("CancelOrder : ", ex);
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
            }
        }
        public ActionResult ResetOrder(string OrderID)
        {
            HistoriesPaid model = new HistoriesPaid();
            try
            {
                string msg = "";
                var result = _factory.OrderCancel(OrderID, ref msg);
                if (result)
                {
                    return PartialView("_ItemAmount", model);
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, msg);
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("CancelOrder : ", ex);
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        public ActionResult ChangeStatusDevice(string DeviceID = "", string AssetID = "")
        {
            try
            {
                string msg = "";
                var result = _factory.ChangeStatusDevice(DeviceID, ref msg);
                if (result)
                {
                    var models = new ProductPackageAdminModels();
                    models = _factory.GetListDataProductsPackage(AssetID);
                    models.fPeriod = models.PeriodType == (int)Commons.EPeriodType.Day ? Commons.EPeriodType.Day.ToString() :
                                    models.PeriodType == (int)Commons.EPeriodType.Month ? Commons.EPeriodType.Month.ToString() :
                                    models.PeriodType == (int)Commons.EPeriodType.None ? Commons.EPeriodType.None.ToString() :
                                    models.PeriodType == (int)Commons.EPeriodType.OneTime ? Commons.EPeriodType.OneTime.ToString() :
                                    Commons.EPeriodType.Year.ToString();

                    models.ListComposite.ForEach(x =>
                    {
                        x.fState = x.State == (int)Commons.EServiceAssetStatus.Done ? Commons.EServiceAssetStatus.Done.ToString() :
                                   x.State == (int)Commons.EServiceAssetStatus.New ? Commons.EServiceAssetStatus.New.ToString() :
                                   Commons.EServiceAssetStatus.Recalled.ToString();

                        x.fType = x.Type == (int)Commons.EAdditionType.Account ? Commons.EAdditionType.Account.ToString() :
                                  x.Type == (int)Commons.EAdditionType.Function ? Commons.EAdditionType.Function.ToString() :
                                  x.Type == (int)Commons.EAdditionType.Hardware ? Commons.EAdditionType.Hardware.ToString() :
                                  x.Type == (int)Commons.EAdditionType.Location ? Commons.EAdditionType.Location.ToString() :
                                  x.Type == (int)Commons.EAdditionType.Service ? Commons.EAdditionType.Service.ToString() :
                                            Commons.EAdditionType.Software.ToString();
                    });

                    models.ListAddition.ForEach(x =>
                    {
                        x.fState = x.State == (int)Commons.EServiceAssetStatus.Done ? Commons.EServiceAssetStatus.Done.ToString() :
                                   x.State == (int)Commons.EServiceAssetStatus.New ? Commons.EServiceAssetStatus.New.ToString() :
                                   Commons.EServiceAssetStatus.Recalled.ToString();

                        x.fType = x.Type == (int)Commons.EAdditionType.Account ? Commons.EAdditionType.Account.ToString() :
                                  x.Type == (int)Commons.EAdditionType.Function ? Commons.EAdditionType.Function.ToString() :
                                  x.Type == (int)Commons.EAdditionType.Hardware ? Commons.EAdditionType.Hardware.ToString() :
                                  x.Type == (int)Commons.EAdditionType.Location ? Commons.EAdditionType.Location.ToString() :
                                  x.Type == (int)Commons.EAdditionType.Service ? Commons.EAdditionType.Service.ToString() :
                                            Commons.EAdditionType.Software.ToString();
                    });
                    return PartialView("_ItemProductDetail", models);
                }
                else
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, msg);
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("ChangeStatusDevice : ", ex);
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        #endregion /*============ End Products Management ================*/

        //public string GetCurrency()
        //{
        //    string currency = "";
        //    CurrencyViewModels model = new CurrencyViewModels();
        //    var _factoryCurrency = new CurrencyFactory();
        //    try
        //    {

        //        var listData = _factoryCurrency.GetListData();
        //        listData = listData.Where(x => x.IsActive == true).ToList();
        //        model.ListItem = listData;
        //        currency = model.ListItem[0].Symbol;
        //    }
        //    catch (Exception e)
        //    {
        //        NSLog.Logger.Error("CurrencySearch: ", e);
        //    }
        //    return currency;
        //}

        public ActionResult GetDetailReceipt(string id)
        {
            var _myProfileFactory = new MyProfileFactory();
            try
            {
                OrderDetailModels model = new OrderDetailModels();
                string msg = "";
                model = _myProfileFactory.GetDetailReceipt(id, ref msg);
                model.CurrencySymbol = CurrentUser.CurrencySymbol;
                model.TotalDiscount = Math.Abs(model.TotalDiscount);
                model.TotalPromotion = Math.Abs(model.TotalPromotion);

                // Tax information
                ViewBag.TaxInfo = "";
                if (model != null && model.TaxInfo != null)
                {
                    ViewBag.TaxInfo = ((Commons.ETaxType)Enum.ToObject(typeof(Commons.ETaxType), model.TaxInfo.TaxType)).ToString() + " - " + model.TaxInfo.TaxPercent + "%";
                }

                if (model != null)
                {
                    model.PaidTime = CommonHelper.ConvertToLocalTime(model.PaidTime);
                }
                int offset = 0;
                int offsetRefund = 0, offsetTotalRefund = 0;
                model.ListItems.ForEach(ss =>
                {
                    ss.OffSet = offset++;
                    // Can refund: products, packages, addtions and additions(optional)
                    if (ss.ProductType == (int)Commons.EProductType.Product || ss.ProductType == (int)Commons.EProductType.Package || ss.ProductType == (int)Commons.EProductType.Addition)
                    {
                        ss.IsShowCheckRefurn = true;
                        offsetRefund++;
                    }

                    if (ss.IsRefund)
                    {
                        offsetTotalRefund++;
                    }

                    if (ss.ListComposite != null && ss.ListComposite.Any())
                        ss.ListComposite = ss.ListComposite.OrderBy(o => o.ProductName).ToList();
                    ss.PeriodName = GetPeriodName(ss.Period, ss.PeriodType);
                    if (ss.ListItem != null && ss.ListItem.Any())
                    {
                        ss.ListItem = ss.ListItem.OrderBy(o => o.ProductName).ToList();
                        ss.ListItem.ForEach(z =>
                        {
                            z.PeriodName = GetPeriodName(z.Period, z.PeriodType);
                        });
                    }
                });
                model.ListItems = model.ListItems.OrderBy(o => o.ProductName).ToList();
                if (offsetRefund == offsetTotalRefund && offsetRefund != 0 && offsetTotalRefund != 0)
                {
                    model.IsShowButtonRefund = true;
                }

                return PartialView("_ModalReceiptDetail", model);
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("Receipt_Get detail:", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }

        [HttpPost]
        public ActionResult DoneRefund(DoneRefundModels data)
        {
            try
            {
                string msg = string.Empty;
                var result = _factory.DoneRefundOrder(data, ref msg);
                if (result)
                {
                    // return new HttpStatusCodeResult(HttpStatusCode.OK);
                    var _myProfileFactory = new MyProfileFactory();
                    OrderDetailModels model = new OrderDetailModels();
                    // string msg = "";
                    model = _myProfileFactory.GetDetailReceipt(data.OrderID, ref msg);
                    model.CurrencySymbol = CurrentUser.CurrencySymbol;

                    // Tax information
                    ViewBag.TaxInfo = "";
                    if (model != null && model.TaxInfo != null)
                    {
                        ViewBag.TaxInfo = ((Commons.ETaxType)Enum.ToObject(typeof(Commons.ETaxType), model.TaxInfo.TaxType)).ToString() + " - " + model.TaxInfo.TaxPercent + "%";
                    }

                    int offset = 0;
                    int offsetRefund = 0, offsetTotalRefund = 0;
                    model.ListItems.ForEach(ss =>
                    {
                        ss.OffSet = offset++;
                        // Can refund: products, packages, addtions and additions(optional)
                        if (ss.ProductType == (int)Commons.EProductType.Product || ss.ProductType == (int)Commons.EProductType.Package || ss.ProductType == (int)Commons.EProductType.Addition)
                        {
                            ss.IsShowCheckRefurn = true;
                            offsetRefund++;
                        }
                        if (ss.IsRefund)
                        {
                            offsetTotalRefund++;
                        }

                        if (ss.ListComposite != null && ss.ListComposite.Any())
                            ss.ListComposite = ss.ListComposite.OrderBy(o => o.ProductName).ToList();
                        ss.PeriodName = GetPeriodName(ss.Period, ss.PeriodType);
                        if (ss.ListItem != null && ss.ListItem.Any())
                        {
                            ss.ListItem = ss.ListItem.OrderBy(o => o.ProductName).ToList();
                            ss.ListItem.ForEach(z =>
                            {
                                z.PeriodName = GetPeriodName(z.Period, z.PeriodType);
                            });
                        }
                    });
                    model.ListItems = model.ListItems.OrderBy(o => o.ProductName).ToList();
                    if (offsetRefund == offsetTotalRefund && offsetRefund != 0 && offsetTotalRefund != 0)
                    {
                        model.IsShowButtonRefund = true;
                    }
                    return PartialView("_ModalReceiptDetail", model);
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, msg);
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("DoneRefund : ", ex);
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        public PartialViewResult CloseStoreDetail(string id, string maction)
        {
            CustomerBaseModels model = new CustomerBaseModels();
            model.IndexTab = 2;
            model.CurrencySymbol = CurrentUser.CurrencySymbol;
            model.CustomerDetail = GetDetail(id);
            var Merchant = GetDetailMerchant(id);
            if (Merchant.CustomerID == null)
                Merchant.CustomerID = model.CustomerDetail.ID;
            model.MerchantStore = Merchant;
            //========
            model.ProductManagement = _factory.GetListDataProductsManagement(id);
            if (model.ProductManagement != null)
            {
                //=========ListHardwareService
                if (model.ProductManagement.ListHardwareService != null && model.ProductManagement.ListHardwareService.Count > 0)
                {
                    model.ProductManagement.ListHardwareService.ForEach(x =>
                    {
                        x.Status = ((Commons.EServiceAssetStatus)Enum.ToObject(typeof(Commons.EServiceAssetStatus), x.State)).ToString();
                        x.sAdditionType = ((Commons.EAdditionType)Enum.ToObject(typeof(Commons.EAdditionType), x.AdditionType)).ToString();
                        if (x.AdditionType == (byte)Commons.EAdditionType.Service)
                            x.SerialNo = "";
                        else
                        {
                            if (x.State == (byte)Commons.EServiceAssetStatus.New)
                                x.SerialNo = "Updating";
                        }
                    });
                }
                //=========ListProductPackage
                if (model.ProductManagement.ListProductPackage != null && model.ProductManagement.ListProductPackage.Count > 0)
                {
                    model.ProductManagement.ListProductPackage.ForEach(x =>
                    {
                        //if (x.ReceiptNo.Equals("ON20171019-047"))
                        {
                            x.sType = ((Commons.EProductType)Enum.ToObject(typeof(Commons.EProductType), x.Type)).ToString();
                            //if (!x.ExpiryDate.HasValue)
                            //    x.Status = Commons.EStatusAccountFunction.Inactive.ToString();
                            //else
                            //{
                            //    DateTime _curDate = DateTime.Now;
                            //    if (x.ExpiryDate < _curDate)
                            //        x.Status = Commons.EStatusAccountFunction.Expired.ToString();
                            //    else
                            //        x.Status = x.IsActive ? Commons.EStatusAccountFunction.Active.ToString() : Commons.EStatusAccountFunction.Inactive.ToString();
                            //}
                            //=======

                            if (x.IsActive)
                            {
                                //var minDate = new DateTime(1900, 01, 01, 12, 00, 00);
                                //if (!x.ExpiryDate.Equals(minDate))
                                if (DateTime.Compare(x.ExpiryDate.Value.Date, Commons.MinDate.Date) > 0)
                                {
                                    DateTime _curDate = DateTime.Now;
                                    if (x.ExpiryDate.Value < _curDate)
                                        x.Status = Commons.EStatusAccountFunction.Expired.ToString();
                                    else
                                        x.Status = Commons.EStatusAccountFunction.Active.ToString();
                                }
                                else
                                    x.Status = Commons.EStatusAccountFunction.Active.ToString();
                            }
                            else
                            {
                                x.Status = Commons.EStatusAccountFunction.Inactive.ToString();
                            }
                            if (!x.ActivateTime.HasValue)
                                x.sActivateTime = "Not Yet Activated";
                            else
                                x.sActivateTime = x.ActivateTime.Value.ToString(Commons.FormatDateTime);
                            //=======
                            if (!x.ExpiryDate.HasValue || DateTime.Compare(x.ExpiryDate.Value.Date, Commons.MinDate.Date) <= 0)
                                x.sExpiryDate = "";
                            else
                                x.sExpiryDate = x.ExpiryDate.Value.ToString(Commons.FormatDateTime);
                            //======
                            if (x.PeriodType == (byte)Commons.EPeriodType.OneTime)
                                x.sExpiryDate = "Never";
                        }

                    });
                }
                //=========ListAccountManagement
                if (model.ProductManagement.ListAccount != null && model.ProductManagement.ListAccount.Count > 0)
                {
                    model.ProductManagement.ListAccount.ForEach(x =>
                    {
                        if (!x.ExpiryDate.HasValue)
                            x.Status = Commons.EStatusAccountFunction.Inactive.ToString();
                        else
                        {
                            DateTime _curDate = DateTime.Now;
                            if (DateTime.Compare(x.ExpiryDate.Value.Date, Commons.MinDate.Date) > 0 && x.ExpiryDate.Value < _curDate)
                                x.Status = Commons.EStatusAccountFunction.Expired.ToString();
                            else
                                x.Status = x.IsActive ? Commons.EStatusAccountFunction.Active.ToString() : Commons.EStatusAccountFunction.Inactive.ToString();
                        }

                        //======
                        if (x.PeriodType == (byte)Commons.EPeriodType.OneTime)
                            x.sExpiryDate = "Never";
                        else
                        {
                            if (x.ExpiryDate.HasValue && DateTime.Compare(x.ExpiryDate.Value.Date, Commons.MinDate.Date) > 0)
                                x.sExpiryDate = x.ExpiryDate.Value.ToString(Commons.FormatDateTime);
                            else
                                x.sExpiryDate = "";
                        }
                        //======
                        if (x.ActivateTime.HasValue)
                            x.sActivateTime = x.ActivateTime.Value.ToString(Commons.FormatDateTime);
                        else
                            x.sActivateTime = "Not Yet Activated";
                    });
                }
                //=========List Additional Functions Management
                if (model.ProductManagement.ListFunction != null && model.ProductManagement.ListFunction.Count > 0)
                {
                    model.ProductManagement.ListFunction.ForEach(x =>
                    {
                        ////if (!x.ExpiryDate.HasValue)
                        ////    x.Status = Commons.EStatusAccountFunction.Inactive.ToString();
                        ////else
                        ////{
                        //DateTime _curDate = DateTime.Now;
                        //if (DateTime.Compare(x.ExpiryDate.Value.Date, Commons.MinDate.Date) > 0 && x.ExpiryDate.Value < _curDate)
                        //    x.Status = Commons.EStatusAccountFunction.Expired.ToString();
                        //else
                        //{
                        //    if (x.IsBlock)
                        //        x.Status = Commons.EStatusAccountFunction.Blocked.ToString();
                        //    else
                        //        x.Status = x.IsActive ? Commons.EStatusAccountFunction.Active.ToString() : Commons.EStatusAccountFunction.Inactive.ToString();
                        //}

                        ////}
                        ////==========
                        //if (x.PeriodType == (byte)Commons.EPeriodType.OneTime)
                        //    x.sExpiryDate = "Never";
                        //else
                        //{
                        //    if (x.ExpiryDate.HasValue && DateTime.Compare(x.ExpiryDate.Value.Date, Commons.MinDate.Date) > 0)
                        //        x.sExpiryDate = x.ExpiryDate.Value.ToString(Commons.FormatDateTime);
                        //    else
                        //        x.sExpiryDate = "";
                        //}

                        //if (x.ActivateTime.HasValue)
                        //    x.sActivateTime = x.ActivateTime.Value.ToString(Commons.FormatDateTime);
                        //else
                        //    x.sActivateTime = "Not Yet Activated";

                        x = GetAdditionFunctionInfoView(x);
                    });
                }
            }
            if (maction.Equals("edit"))
                return PartialView("_Edit", model);
            else
                return PartialView("_View", model);
        }

        // Get Status name of store
        public string GetStoreStatusName(int Status)
        {
            string StatusName = "";
            switch (Status)
            {
                case (int)Commons.EStoreStatus.None:
                    StatusName = "None";
                    break;
                case (int)Commons.EStoreStatus.InUse:
                    StatusName = "In Use";
                    break;
                case (int)Commons.EStoreStatus.Expired:
                    StatusName = "Expired";
                    break;
                case (int)Commons.EStoreStatus.Temporary:
                    StatusName = "Temporary";
                    break;
            }
            return StatusName;
        }

        // Get Addition Function information for view
        public AdditionFunctionModels GetAdditionFunctionInfoView(AdditionFunctionModels Model)
        {
            if (Model.IsBlock)
            {
                Model.Status = "Blocked";
            }
            else
            {
                if (Model.IsActive)
                {
                    Model.Status = "Activated";
                }
                else
                {
                    Model.Status = "Inactivated";
                }
            }
            Model.sActivateTime = "Not Yet Activated";
            Model.sExpiryDate = "";
            if (Model.PeriodType == (int)Commons.EPeriodType.OneTime)
            {
                Model.sExpiryDate = "Never";
            }

            if (Model.ExpiryDate.HasValue && (Model.ExpiryDate.Value.Date > Commons.MinDate.Date))
            {
                if (Model.ActivateTime.HasValue)
                {
                    Model.sActivateTime = Model.ActivateTime.Value.ToString(Commons.FormatDateTime);
                }

                if (Model.PeriodType != (int)Commons.EPeriodType.OneTime)
                {
                    Model.sExpiryDate = Model.ExpiryDate.Value.ToString(Commons.FormatDateTime);
                }

                if (Model.ExpiryDate.Value <= DateTime.Now)
                {
                    Model.Status = "Expired";
                }
            }
            return Model;
        }

        // Get Name of Period of product for view 
        public string GetPeriodName(int Period, int PeriodType)
        {
            //string PeriodName = "";
            //if (PeriodType > 0)
            //{
            //    PeriodName = ((Commons.EPeriodType)Enum.ToObject(typeof(Commons.EPeriodType), PeriodType)).ToString();
            //    if (PeriodType != (int)Commons.EPeriodType.OneTime)
            //    {
            //        PeriodName = Period + " " + PeriodName;
            //        if (Period > 1)
            //        {
            //            PeriodName = PeriodName + "s";
            //        }
            //    }
            //}
            //return PeriodName;
            string _PeriodName = "";
            if (PeriodType != (byte)Commons.EPeriodType.None)
            {
                _PeriodName = (PeriodType == (byte)Commons.EPeriodType.OneTime)
                                                ? Commons.PeriodTypeOneTime : Period + " " + ((Commons.EPeriodType)Enum.ToObject(typeof(Commons.EPeriodType), PeriodType)).ToString()
                                                            + (Period > 1 ? "s" : "");
            }
            return _PeriodName;
        }
    }
}