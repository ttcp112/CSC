using NSCSC.Shared;
using NSCSC.Shared.Factory.Settings.PaymentMethods;
using NSCSC.Shared.Models.Settings.PaymentMethods;
using NSCSC.Shared.Utilities;
using NuWebNCloud.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using NSCSC.Web.App_Start;

namespace NSCSC.Web.Controllers
{
    [NuAuth]
    public class SPaymentMethodsController : HQController
    {
        private PaymentMethodFactory _factory = null;
        public SPaymentMethodsController()
        {
            _factory = new PaymentMethodFactory();
        }
        // GET: PaymentMethods
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            PaymentMethodModels model = new PaymentMethodModels();
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(PaymentMethodModels model)
        {
            try
            {
                if (model.ListChild != null)
                {
                    model.ListChild = model.ListChild.Where(x => x.Status != (byte)Commons.EStatus.Deleted).ToList();
                    if(model.ListChild.Count <= 0 && model.Code == 0)
                    {
                        ModelState.AddModelError("ErrorSubPayment", "Payment Method must have as least or equal to one Sub Payment Method");
                    }
                }
                else
                {
                    var ischeck = model.ListChild.GroupBy(o => o.Name).Any(g => g.Count() > 1);
                    if (ischeck)
                    {
                        ModelState.AddModelError("errorCardName", "This name is duplicated to an existed payment method. Please try again");
                    }
                }
                if (!ModelState.IsValid)
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return View("Create", model);
                }              
                byte[] photoByte = null;
                /*** Processing Image *****/
                if (model.PictureUpload != null && model.PictureUpload.ContentLength > 0)
                {
                    Byte[] imgByte = new Byte[model.PictureUpload.ContentLength];
                    model.PictureUpload.InputStream.Read(imgByte, 0, model.PictureUpload.ContentLength);
                    model.PictureByte = imgByte;
                    model.ImageURL = Guid.NewGuid() + Path.GetExtension(model.PictureUpload.FileName);
                    model.PictureUpload = null;
                    photoByte = imgByte;
                    //Save Image on Server
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
                /*** Processing Image ListChild *****/
                if (model.ListChild != null && model.ListChild.Count > 0)
                {
                    for (int i = 0; i < model.ListChild.Count; i++)
                    {
                        if (model.ListChild[i].PictureUpload != null && model.ListChild[i].PictureUpload.ContentLength > 0)
                        {
                            Byte[] imgByte = new Byte[model.ListChild[i].PictureUpload.ContentLength];
                            model.ListChild[i].PictureUpload.InputStream.Read(imgByte, 0, model.ListChild[i].PictureUpload.ContentLength);
                            model.ListChild[i].PictureByte = imgByte;
                            model.ListChild[i].ImageURL = Guid.NewGuid() + Path.GetExtension(model.ListChild[i].PictureUpload.FileName);
                            model.ListChild[i].PictureUpload = null;
                            photoByte = imgByte;
                            //Save Image ListChild on Server 
                            var originalDirectory = new DirectoryInfo(string.Format("{0}Uploads\\", Server.MapPath(@"\")));
                            var path = string.Format("{0}{1}", originalDirectory, model.ListChild[i].ImageURL);
                            MemoryStream ms = new MemoryStream(photoByte, 0, photoByte.Length);
                            ms.Write(photoByte, 0, photoByte.Length);
                            System.Drawing.Image imageTmp = System.Drawing.Image.FromStream(ms, true);
                            ImageHelper.Me.SaveCroppedImage(imageTmp, path, model.ListChild[i].ImageURL, ref photoByte);
                            model.ListChild[i].PictureByte = photoByte;
                            FTP.Upload(model.ListChild[i].ImageURL, model.ListChild[i].PictureByte);
                            ImageHelper.Me.TryDeleteImageUpdated(path);
                        }
                    }
                }
                string msg = "";
                bool result = _factory.CreateOrEdit(model, ref msg);
                if (result)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    if (msg.Equals("This payment method name existed. Please try again."))
                    {
                        ModelState.AddModelError("Name", msg);
                    }
                    else
                    {
                        ModelState.AddModelError("errorCardName", msg);
                    }
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return View(model);
                }
            }
            catch(Exception ex)
            {
                NSLog.Logger.Error("Create : ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }

        public PartialViewResult Edit(string id)
        {
            PaymentMethodModels model = GetDetail(id);
            return PartialView("_Edit", model);
        }

        [HttpPost]
        public ActionResult Edit(PaymentMethodModels model)
        {
            try
            {
                if (model.ListChild != null)
                {
                    model.ListChild = model.ListChild.Where(x => x.Status != (byte)Commons.EStatus.Deleted).ToList();
                    if (model.ListChild.Count <= 0 && model.Code == 0)
                    {
                        ModelState.AddModelError("ErrorSubPayment", "Payment Method must have as least or equal to one Sub Payment Method");
                    }
                    else
                    {
                        var ischeck = model.ListChild.GroupBy(o => o.Name).Any(g => g.Count() > 1);
                        if (ischeck)
                        {
                            ModelState.AddModelError("errorCardName", "This name is duplicated to an existed payment method. Please try again");
                        }
                    }
                }
                if (!ModelState.IsValid)
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return PartialView("_Edit", model);
                }
                /*** Processing Image *****/
                byte[] photoByte = null;
                if (model.PictureUpload != null && model.PictureUpload.ContentLength > 0)
                {
                    Byte[] imgByte = new Byte[model.PictureUpload.ContentLength];
                    model.PictureUpload.InputStream.Read(imgByte, 0, model.PictureUpload.ContentLength);
                    model.PictureByte = imgByte;
                    model.ImageURL = Guid.NewGuid() + Path.GetExtension(model.PictureUpload.FileName);
                    model.PictureUpload = null;
                    photoByte = imgByte;
                    //Save Image on Server
                    var originalDirectory = new DirectoryInfo(string.Format("{0}Uploads\\", Server.MapPath(@"\")));
                    var path = string.Format("{0}{1}", originalDirectory, model.ImageURL);
                    MemoryStream ms = new MemoryStream(photoByte, 0, photoByte.Length);
                    ms.Write(photoByte, 0, photoByte.Length);
                    System.Drawing.Image imageTmp = System.Drawing.Image.FromStream(ms, true);
                    ImageHelper.Me.SaveCroppedImage(imageTmp, path, model.ImageURL, ref photoByte);
                    model.PictureByte = photoByte;
                    FTP.Upload(model.ImageURL, model.PictureByte);
                    ImageHelper.Me.TryDeleteImageUpdated(path);
                }else{
                    if (!string.IsNullOrEmpty(model.ImageURL))
                        model.ImageURL = model.ImageURL.Replace(Commons.PublicImages, "").Replace(Commons.Image100_50, "");
                }
                /*** Processing Image ListChild *****/
                if (model.ListChild != null && model.ListChild.Count > 0)
                {
                    for (int i = 0; i < model.ListChild.Count; i++)
                    {
                        if (model.ListChild[i].PictureUpload != null && model.ListChild[i].PictureUpload.ContentLength > 0)
                        {
                            Byte[] imgByte = new Byte[model.ListChild[i].PictureUpload.ContentLength];
                            model.ListChild[i].PictureUpload.InputStream.Read(imgByte, 0, model.ListChild[i].PictureUpload.ContentLength);
                            model.ListChild[i].PictureByte = imgByte;
                            model.ListChild[i].ImageURL = Guid.NewGuid() + Path.GetExtension(model.ListChild[i].PictureUpload.FileName);
                            model.ListChild[i].PictureUpload = null;
                            photoByte = imgByte;
                            //Save Image ListChild on Server 
                            var originalDirectory = new DirectoryInfo(string.Format("{0}Uploads\\", Server.MapPath(@"\")));
                            var path = string.Format("{0}{1}", originalDirectory, model.ListChild[i].ImageURL);
                            MemoryStream ms = new MemoryStream(photoByte, 0, photoByte.Length);
                            ms.Write(photoByte, 0, photoByte.Length);
                            System.Drawing.Image imageTmp = System.Drawing.Image.FromStream(ms, true);
                            ImageHelper.Me.SaveCroppedImage(imageTmp, path, model.ListChild[i].ImageURL, ref photoByte);
                            model.ListChild[i].PictureByte = photoByte;
                            FTP.Upload(model.ListChild[i].ImageURL, model.ListChild[i].PictureByte);
                            ImageHelper.Me.TryDeleteImageUpdated(path);
                        }else{
                            if (!string.IsNullOrEmpty(model.ListChild[i].ImageURL))
                                model.ListChild[i].ImageURL = model.ListChild[i].ImageURL.Replace(Commons.PublicImages, "").Replace(Commons.Image100_50, "");
                        }
                    }
                }

                string msg = "";
                bool result = _factory.CreateOrEdit(model, ref msg);
                if (result)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    if (msg.Equals("This payment method name existed. Please try again."))
                    {
                        ModelState.AddModelError("Name", msg);
                    }
                    else
                    {
                        ModelState.AddModelError("errorCardName", msg);
                    }
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return PartialView("_Edit", model);
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("Payment Method Edit: ", ex);
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return PartialView("_Edit", model);
            }
        }

        [HttpGet]
        public new PartialViewResult View(string id)
        {
            PaymentMethodModels model = GetDetail(id);
            return PartialView("_View", model);
        }

        public ActionResult Search()
        {
            var models = new List<PaymentMethodModels>();
            try
            {
                models = _factory.GetData();
                models.Where(x => x.Name.Equals("Cash")).ToList().ForEach(y => y.IsDefault = true);
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("PromotionSearch: ", e);
                return new HttpStatusCodeResult(400, e.Message);
            }
            return PartialView("_ListData", models);
        }

        public PaymentMethodModels GetDetail(string Id)
        {
            var model = new PaymentMethodModels();
            try
            {
                model = _factory.GetData().Where(x => x.ID == Id).FirstOrDefault();
                if (model.ListChild != null)
                {
                    for (int i = 0; i < model.ListChild.Count; i++)
                    {
                        var item = model.ListChild[i];
                        item.OffSet = i;
                    }
                }
            }
            catch(Exception ex)
            {
                NSLog.Logger.Error("GetDetail", ex);
            }
            return model;
        }

        public ActionResult AddChild(int currentOffset)
        {
            PaymentMethodModels group = new PaymentMethodModels();
            group.OffSet = currentOffset;
            return PartialView("_ItemCardForPaymentMethod", group);
        }

        public ActionResult DeletePopup(string ID, string PinCode, string Reason)
        {
            try
            {
                if (!PinCode.Equals(CurrentUser.PinCode))
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                var result = _factory.Delete(ID,  Reason);
                if (!result)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("Payment Method Delete: ", ex);
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }
    }
}