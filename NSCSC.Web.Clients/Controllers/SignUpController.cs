using NSCSC.Shared;
using NSCSC.Shared.Factory;
using NSCSC.Shared.Factory.ClientSite;
using NSCSC.Shared.Factory.ClientSite.Register;
using NSCSC.Shared.Models;
using NSCSC.Shared.Models.ClientSite;
using NSCSC.Shared.Models.CRM.Customers;
using NSCSC.Shared.Utilities;
using NuWebNCloud.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace NSCSC.Web.Clients.Controllers
{
    public class SignUpController : ClientController
    {
        RegisterFactory _factory = null;
        BaseFactory _BaseFactory = null;
        private CheckIpAddressFactory _checkIpFactory = null;
        public SignUpController()
        {
            _factory = new RegisterFactory();
            _BaseFactory = new BaseFactory();
            _checkIpFactory = new CheckIpAddressFactory();
        }
        // GET: SignUp
        public ActionResult Index()
        {
            if (Session["UserClientSite"] != null)
            {
                return RedirectToAction("Index", "Home", new { area = "" });
            }
            RegisterModels model = new RegisterModels();
            model.ListCounTry = _BaseFactory.GetListCountry();
           // var CountryCode = _checkIpFactory.GetCountryCode();
            if (model.ListCounTry != null)
            {
                var ListCountry = model.ListCounTry as List<CountryModel>;
                var objCountry = ListCountry.Where(x => x.Alpha2Code.Equals(CountryCode)).FirstOrDefault();
                model.MerchantDetail.Country = model.CustomerDetail.HomeCountry = model.CustomerDetail.OfficeCountry =  objCountry != null ? objCountry.Name : "";
                model.MerchantDetail.CreatedDate = CommonHelper.ConvertToLocalTime(model.MerchantDetail.CreatedDate);
                model.MerchantDetail.ExpiredDate = CommonHelper.ConvertToLocalTime(model.MerchantDetail.ExpiredDate);               
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult Index(RegisterModels model)
        {
            try
            {
                string msg = "";
                byte[] photoByte1 = null;
                byte[] photoByte2 = null;

                //Customer Image
                if (!string.IsNullOrEmpty(model.CustomerDetail.ImageURL))
                { 
                    model.CustomerDetail.ImageURL = model.CustomerDetail.ImageURL.Replace(CommonHelper.PublicImages, "").Replace(Commons.Image200_200, "");
                }
                if (model.CustomerDetail.PictureUpload != null && model.CustomerDetail.PictureUpload.ContentLength > 0)
                {
                    Byte[] imgByte1 = new Byte[model.CustomerDetail.PictureUpload.ContentLength];
                    model.CustomerDetail.PictureUpload.InputStream.Read(imgByte1, 0, model.CustomerDetail.PictureUpload.ContentLength);
                    model.CustomerDetail.PictureByte = imgByte1;
                    model.CustomerDetail.ImageURL = Guid.NewGuid() + Path.GetExtension(model.CustomerDetail.PictureUpload.FileName);
                    model.CustomerDetail.PictureUpload = null;
                    photoByte1 = imgByte1;
                }
                //Merchant Image
                if (!string.IsNullOrEmpty(model.MerchantDetail.ImageURL))
                { 
                    model.MerchantDetail.ImageURL = model.MerchantDetail.ImageURL.Replace(CommonHelper.PublicImages, "").Replace(Commons.Image200_200, "");
                }
                if (model.MerchantDetail.PictureUpload != null && model.MerchantDetail.PictureUpload.ContentLength > 0)
                {
                    Byte[] imgByte2 = new Byte[model.MerchantDetail.PictureUpload.ContentLength];
                    model.MerchantDetail.PictureUpload.InputStream.Read(imgByte2, 0, model.MerchantDetail.PictureUpload.ContentLength);
                    model.MerchantDetail.PictureByte = imgByte2;
                    model.MerchantDetail.ImageURL = Guid.NewGuid() + Path.GetExtension(model.MerchantDetail.PictureUpload.FileName);
                    model.MerchantDetail.PictureUpload = null;
                    photoByte2 = imgByte2;
                }
                bool result = _factory.Create(model, ref msg);
                if (result)
                {
                    if (!string.IsNullOrEmpty(model.CustomerDetail.ImageURL) && photoByte1 != null)
                    {
                        var originalDirectory = new DirectoryInfo(string.Format("{0}Uploads\\", Server.MapPath(@"\")));
                        var path = string.Format("{0}{1}", originalDirectory, model.CustomerDetail.ImageURL);
                        MemoryStream ms = new MemoryStream(photoByte1, 0, photoByte1.Length);
                        ms.Write(photoByte1, 0, photoByte1.Length);
                        System.Drawing.Image imageTmp = System.Drawing.Image.FromStream(ms, true);

                        ImageHelper.Me.SaveCroppedImage(imageTmp, path, model.CustomerDetail.ImageURL, ref photoByte1);

                        FTP.UploadClient(model.CustomerDetail.ImageURL, photoByte1);

                        ImageHelper.Me.TryDeleteImageUpdated(path);
                    }
                    if (!string.IsNullOrEmpty(model.MerchantDetail.ImageURL) && photoByte2 != null)
                    {
                        var originalDirectory = new DirectoryInfo(string.Format("{0}Uploads\\", Server.MapPath(@"\")));
                        var path = string.Format("{0}{1}", originalDirectory, model.MerchantDetail.ImageURL);
                        MemoryStream ms = new MemoryStream(photoByte2, 0, photoByte2.Length);
                        ms.Write(photoByte2, 0, photoByte2.Length);
                        System.Drawing.Image imageTmp = System.Drawing.Image.FromStream(ms, true);

                        ImageHelper.Me.SaveCroppedImage(imageTmp, path, model.MerchantDetail.ImageURL, ref photoByte2);

                        FTP.UploadClient(model.MerchantDetail.ImageURL, photoByte2);

                        ImageHelper.Me.TryDeleteImageUpdated(path);
                    }
                    VerificationModels models = new VerificationModels();
                    models.ReSendEmail = model.CustomerDetail.Email;
                    return new HttpStatusCodeResult(HttpStatusCode.OK);
                }
                else
                {
                    //ModelState.AddModelError("CustomerDetail.Email", msg);
                    model.CustomerDetail.ImageURL = "";
                    model.MerchantDetail.ImageURL = "";
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("RegisterCreate: ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }
        [HttpGet]
        public ActionResult Verification(string email)
        {
            VerificationModels model = new VerificationModels();
            model.ReSendEmail = email;
            return View(model);
        }
        [HttpPost]
        public ActionResult ResendMail(ResendEmailModels model)
        {
            try
            {
                string msg = "";
                bool result = _factory.ResendMail(model.Email, ref msg);
                if (result)
                {
                    VerificationModels models = new VerificationModels();
                    models.ReSendEmail = model.Email;
                    return new HttpStatusCodeResult(HttpStatusCode.OK);
                }
                else
                {
                    ModelState.AddModelError("VerificationCode", msg);
                    VerificationModels models = new VerificationModels();
                    models.ReSendEmail = model.Email;
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("Verify: ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }
        [HttpPost]
        public ActionResult CheckEmail(string email)
        {
            try
            {
                string msg = "";
                bool result = _factory.CheckEmail(email, ref msg);
                if (result)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.OK);
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("CheckEmail: ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }
        [HttpPost]
        public ActionResult Verify(VerificationModels model)
        {
            try
            {
                string msg = "";
                var result = _factory.Verify(model, ref msg);
                if (result != null)
                {
                    bool isValid = (result != null && !string.IsNullOrEmpty(result.ID));
                    if(isValid)
                    {
                        UserSession userSession = new UserSession();
                        userSession.Email = result.Email;
                        userSession.UserName = result.Name;
                        userSession.UserId = result.ID;
                        userSession.IsAuthenticated = true;
                        userSession.ImageUrl = result.ImageURL;
                        Session.Add("UserClientSite", userSession);
                    }
                    model = new VerificationModels();
                    return RedirectToAction("Verification", model);
                }
                else
                {
                    model.Result = false;
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("Verify: ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }        
    }
}