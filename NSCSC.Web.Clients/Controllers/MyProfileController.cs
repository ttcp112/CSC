using NSCSC.Shared;
using NSCSC.Shared.Factory.ClientSite;
using NSCSC.Shared.Filters;
using NSCSC.Shared.Models.ClientSite.MyProfile;
using NSCSC.Shared.Utilities;
using NSCSC.Web.Clients.App_Start;
using NuWebNCloud.Shared.Utilities;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace NSCSC.Web.Clients.Controllers
{
    [NuAuth]
    public class MyProfileController : ClientController
    {
        private MyProfileFactory _myProfileFactory = null;

        public MyProfileController()
        {
            _myProfileFactory = new MyProfileFactory();
        }

        public ActionResult Index()
        {
            try
            {
                CustomerDetailModels model = new CustomerDetailModels();
                model = _myProfileFactory.GetInfo(CurrentUser.UserId);
                //Reseller
                if (model.MerchantDetail == null && model.ListReceipt != null && model.ListReceipt.Count > 0)
                {
                    if (model.ListReceipt != null && model.ListReceipt.Count > 0)
                    {
                        model.ListReceipt.ForEach(x =>
                        {
                            x.ListReceipt.ForEach(z =>
                            {
                                z.CreatedDate = CommonHelper.ConvertToLocalTime(z.CreatedDate);
                                z.PaidTime = CommonHelper.ConvertToLocalTime(z.PaidTime);
                                //paymend
                                z.sPaidByMethod = string.Join("<br/>", z.PaidByMethod);
                                //status
                                if (z.OrderStatus == (byte)Commons.EOrderStatus.Completed)
                                    z.sOrderStatus = Commons.EOrderStatus.Completed.ToString();
                                else if (z.OrderStatus == (byte)Commons.EOrderStatus.Full_Refunded)
                                    z.sOrderStatus = Commons.EOrderStatus.Full_Refunded.ToString();
                                else if (z.OrderStatus == (byte)Commons.EOrderStatus.Half_Refunded)
                                    z.sOrderStatus = Commons.EOrderStatus.Half_Refunded.ToString();
                                z.CurrencySymbol = CurrencySymbol;
                                //-----------------
                                z.CustomerEmail = x.CustomerEmail;
                                z.MerchantName = x.MerchantName;
                            });
                        });
                    }
                }
                else //Customer
                {
                    if (model.ListReceipt.Count == 1)
                    {
                        model.ListReceipt[0].ListReceipt.ForEach(x =>
                        {
                            x.CreatedDate = CommonHelper.ConvertToLocalTime(x.CreatedDate);
                            x.PaidTime = CommonHelper.ConvertToLocalTime(x.PaidTime);
                            //paymend
                            x.sPaidByMethod = string.Join("<br/>", x.PaidByMethod);
                            //status
                            if (x.OrderStatus == (byte)Commons.EOrderStatus.Completed)
                                x.sOrderStatus = Commons.EOrderStatus.Completed.ToString();
                            else if (x.OrderStatus == (byte)Commons.EOrderStatus.Full_Refunded)
                                x.sOrderStatus = Commons.EOrderStatus.Full_Refunded.ToString();
                            else if (x.OrderStatus == (byte)Commons.EOrderStatus.Half_Refunded)
                                x.sOrderStatus = Commons.EOrderStatus.Half_Refunded.ToString();
                            x.CurrencySymbol = CurrencySymbol;
                        });
                    }
                }

                //==============
                if (model.MerchantDetail != null)
                {
                    if (model.MerchantDetail.Street != null)
                        model.MerchantDetail.Address = model.MerchantDetail.Street + ", ";
                    if (model.MerchantDetail.City != null)
                        model.MerchantDetail.Address += model.MerchantDetail.City + ", ";
                    if (model.MerchantDetail.Country != null)
                        model.MerchantDetail.Address += model.MerchantDetail.Country;
                }
                //Image
                model.CustomerDetail.ImageURL = string.IsNullOrEmpty(model.CustomerDetail.ImageURL) ? _ImgDummyCustomer : model.CustomerDetail.ImageURL;
                if (model.CustomerDetail != null)
                {
                    if (model.CustomerDetail.ImageURL != null)
                    {
                        CurrentUser.ImageUrl = model.CustomerDetail.ImageURL;
                        CurrentUser.UserName = model.CustomerDetail.Name;
                    }
                    return View("ShowUserInformation", model);
                }
                else
                    return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("PersonalInformation_Index:", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }

        public void GetListCountry()
        {
            var data = _myProfileFactory.GetListCountry();
            ViewBag.ListCountry = data;
        }

        public ActionResult Edit()
        {
            CustomerDetailModels customer = new CustomerDetailModels();
            Customermodel model = new Customermodel();
            customer = _myProfileFactory.GetInfo(CurrentUser.UserId);
            model = customer.CustomerDetail;
            model.ImageURL = string.IsNullOrEmpty(model.ImageURL) ? _ImgDummyCustomer : model.ImageURL;
            GetListCountry();
            if (model != null)
                return PartialView("_Edit", model);
            else
                return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult Edit(Customermodel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    GetListCountry();
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return PartialView("_Edit", model);
                }
                byte[] photoByte = null;
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
                        model.ImageURL = model.ImageURL.Replace(Commons.PublicImagesClient, "").Replace(_ImgDummyCustomer, "");
                }
                string msg = "";
                bool result = _myProfileFactory.UpdatePersonalInfo(model, CurrentUser.UserId, ref msg);
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
                        FTP.UploadClient(model.ImageURL, model.PictureByte);
                        ImageHelper.Me.TryDeleteImageUpdated(path);
                    }
                    return new HttpStatusCodeResult(HttpStatusCode.OK);
                }
                else
                {
                    GetListCountry();
                    ModelState.AddModelError("Name", Commons.ErrorMsg);
                    return PartialView("_Edit", model);
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("Personal_Infor : ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }

        public ActionResult ChangePassword(string ID, string CurrentPassword, string NewPassword)
        {
            try
            {
                string msg = "";
                var result = _myProfileFactory.ChangePassword(ID, CurrentUser.UserId, ref msg, CurrentPassword, NewPassword);
                if (!result)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("ChangePassword_Fail: ", ex);
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        //========================
        [EncryptedActionParameter]
        public ActionResult GetDetailReceipt(string id, string IsReseller)
        {
            try
            {
                OrderDetailModels model = new OrderDetailModels();
                string msg = "";
                model = _myProfileFactory.GetDetailReceipt(id, ref msg);
                model.CurrencySymbol = CurrencySymbol;
                model.CreatedDate = CommonHelper.ConvertToLocalTime(model.CreatedDate);
                model.PaidTime = CommonHelper.ConvertToLocalTime(model.PaidTime);
                model.TotalDiscount = Math.Abs(model.TotalDiscount);
                model.TotalPromotion = Math.Abs(model.TotalPromotion);
                model.sPaidByMethod = string.Join("<br/>", model.PaidByMethod.Select(x => x.Name));
                //--------------
                if (model.TaxInfo != null)
                {
                    if (model.TaxInfo.TaxType == (byte)Commons.ETaxType.AddOn)
                        model.TaxInfo.TaxTypeName = Commons.ETaxType.AddOn.ToString();
                    else if (model.TaxInfo.TaxType == (byte)Commons.ETaxType.Inclusive)
                        model.TaxInfo.TaxTypeName = Commons.ETaxType.Inclusive.ToString();
                    else
                        model.TaxInfo.TaxTypeName = Commons.ETaxType.TaxExempt.ToString();
                }
                //--------------
                model.ListItems = model.ListItems.OrderBy(x => x.ProductName).ToList();
                model.ListItems.ForEach(x =>
                {
                    x.PeriodName = GetPeriodName(x.Period, x.PeriodType);
                    x.ListComposite = x.ListComposite.OrderBy(o => o.ProductName).ToList();
                    x.ListItem = x.ListItem.OrderBy(o => o.ProductName).ToList();
                    x.ListItem.ForEach(z =>
                    {
                        z.PeriodName = GetPeriodName(z.Period, z.PeriodType);
                    });
                });
                return PartialView(IsReseller.ToLower().Equals("true") ? "_ReceiptDetailReseller" : "_ReceiptDetail", model);
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("Receipt_Get detail:", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }

        public string GetPeriodName(int Period, int PeriodType)
        {
            string _PeriodName = "";
            if (PeriodType != (byte)Commons.EPeriodType.None)
            {
                _PeriodName = (PeriodType == (byte)Commons.EPeriodType.OneTime)
                                                ? Commons.PeriodTypeOneTime : Period + " "
                                                + ((Commons.EPeriodType)Enum.ToObject(typeof(Commons.EPeriodType), PeriodType)).ToString()
                                                + (Period > 1 ? "s" : "");
            }
            return _PeriodName;
        }
    }
}





