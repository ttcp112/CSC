using NSCSC.Shared;
using NSCSC.Shared.Factory.ManagementTool;
using NSCSC.Shared.Factory.Sandbox.Inventory;
using NSCSC.Shared.Models.ManagementTool.ListofProductFeatures;
using NSCSC.Shared.Utilities;
using NuWebNCloud.Shared.Utilities;
using System;
using System.IO;
using System.Net;
using System.Web.Mvc;


namespace NSCSC.Web.Controllers
{
    public class MTListofProductFeaturesController : HQController
    {
        // GET: MTListofProductFeatures      
        ListofProductFeaturesFactory _factory = null;
        public CategoriesFactory _categoryfacetory = null;
        public MTListofProductFeaturesController()
        {
            _factory = new ListofProductFeaturesFactory();
            _categoryfacetory = new CategoriesFactory();
            ViewBag.ListCategory = _factory.GetListCategory(CurrentUser.UserId);
        }
        public ActionResult Index()
        {
            try
            {
                ProductFeatureViewModels model = new ProductFeatureViewModels();
                return View(model);

            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("ListofProductFeatures_Index:", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }

        public ActionResult Search(ProductFeatureViewModels model)
        {
            try
            {
                var data = _factory.GetListofProductFeatures(CurrentUser.UserId);

                model.ListofProductFeatures = data;
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("ListofProductFeatures_Search: ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
            return PartialView("_ListData", model);
        }

        public ActionResult Create()
        {
            ProductFeatureDetailModels model = new ProductFeatureDetailModels();
            model.IsActive = true;
            return View(model);
        }
        [HttpPost]
        public ActionResult Create(ProductFeatureDetailModels model)
        {
            try
            {
                if (model.Name == null)
                {
                    ModelState.AddModelError("Name", "Name is Required");
                }

                byte[] ImageURLphotoByte = null;
                byte[] IconURLphotoByte = null;
                if (model.ImageURLUpload != null && model.ImageURLUpload.ContentLength > 0)
                {
                    Byte[] imgByte = new Byte[model.ImageURLUpload.ContentLength];
                    model.ImageURLUpload.InputStream.Read(imgByte, 0, model.ImageURLUpload.ContentLength);
                    model.ImageURLPictureByte = imgByte;
                    model.ImageURL = Guid.NewGuid() + Path.GetExtension(model.ImageURLUpload.FileName);
                    model.ImageURLUpload = null;
                    ImageURLphotoByte = imgByte;
                }
                if (model.IconURLUpload != null && model.IconURLUpload.ContentLength > 0)
                {
                    Byte[] imgBytes = new Byte[model.IconURLUpload.ContentLength];
                    model.IconURLUpload.InputStream.Read(imgBytes, 0, model.IconURLUpload.ContentLength);
                    model.IconURLPictureByte = imgBytes;
                    model.IconURL = Guid.NewGuid() + Path.GetExtension(model.IconURLUpload.FileName);
                    model.IconURLUpload = null;
                    IconURLphotoByte = imgBytes;
                }
                if (!ModelState.IsValid)
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return View(model);
                }
                string msg = "";
                bool result = _factory.InsertOrUpdateCategory(model, CurrentUser.UserId, ref msg);
                if (result)
                {
                    //Save Image on Server
                    if (!string.IsNullOrEmpty(model.ImageURL) && model.ImageURLPictureByte != null)
                    {
                        var originalDirectory = new DirectoryInfo(string.Format("{0}Uploads\\", Server.MapPath(@"\")));
                        var path = string.Format("{0}{1}", originalDirectory, model.ImageURL);
                        MemoryStream ms = new MemoryStream(ImageURLphotoByte, 0, ImageURLphotoByte.Length);
                        ms.Write(ImageURLphotoByte, 0, ImageURLphotoByte.Length);
                        System.Drawing.Image imageTmp = System.Drawing.Image.FromStream(ms, true);
                        ImageHelper.Me.SaveCroppedImage(imageTmp, path, model.ImageURL, ref ImageURLphotoByte);
                        model.ImageURLPictureByte = ImageURLphotoByte;
                        FTP.Upload(model.ImageURL, model.ImageURLPictureByte);
                        ImageHelper.Me.TryDeleteImageUpdated(path);
                    }

                    //Save Image on Server
                    if (!string.IsNullOrEmpty(model.IconURL) && model.IconURL != null)
                    {
                        var originalDirectory = new DirectoryInfo(string.Format("{0}Uploads\\", Server.MapPath(@"\")));
                        var path = string.Format("{0}{1}", originalDirectory, model.IconURL);
                        MemoryStream ms = new MemoryStream(IconURLphotoByte, 0, IconURLphotoByte.Length);
                        ms.Write(IconURLphotoByte, 0, IconURLphotoByte.Length);
                        System.Drawing.Image imageTmp = System.Drawing.Image.FromStream(ms, true);
                        ImageHelper.Me.SaveCroppedImage(imageTmp, path, model.IconURL, ref IconURLphotoByte);
                        model.IconURLPictureByte = IconURLphotoByte;
                        FTP.Upload(model.IconURL, model.IconURLPictureByte);
                        ImageHelper.Me.TryDeleteImageUpdated(path);
                    }


                    return RedirectToAction("Index");
                }
                else
                {

                    ModelState.AddModelError("Name", Commons.ErrorMsg /*msg*/);
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("ListofProductFeatures_Create : ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }

        [HttpGet]
        public PartialViewResult View(string id)
        {
            ProductFeatureDetailModels model = _factory.GetDetail(id);
            return PartialView("_View", model);
        }

        [HttpGet]
        public PartialViewResult Edit(string id)
        {
            ProductFeatureDetailModels model = _factory.GetDetail(id);
            return PartialView("_Edit", model);
        }

        [HttpPost]
        public ActionResult Edit(ProductFeatureDetailModels model)
        {
            try
            {

                if (model.Name == null)
                {
                    ModelState.AddModelError("Name", "Name is Required");
                }

                byte[] ImageURLphotoByte = null;
                byte[] IconURLphotoByte = null;
                if (model.ImageURLUpload != null && model.ImageURLUpload.ContentLength > 0)
                {
                    Byte[] imgByte = new Byte[model.ImageURLUpload.ContentLength];
                    model.ImageURLUpload.InputStream.Read(imgByte, 0, model.ImageURLUpload.ContentLength);
                    model.ImageURLPictureByte = imgByte;
                    model.ImageURL = Guid.NewGuid() + Path.GetExtension(model.ImageURLUpload.FileName);
                    model.ImageURLUpload = null;
                    ImageURLphotoByte = imgByte;
                }
                if (model.IconURLUpload != null && model.IconURLUpload.ContentLength > 0)
                {
                    Byte[] imgBytes = new Byte[model.IconURLUpload.ContentLength];
                    model.IconURLUpload.InputStream.Read(imgBytes, 0, model.IconURLUpload.ContentLength);
                    model.IconURLPictureByte = imgBytes;
                    model.IconURL = Guid.NewGuid() + Path.GetExtension(model.IconURLUpload.FileName);
                    model.IconURLUpload = null;
                    IconURLphotoByte = imgBytes;
                }
                if (!ModelState.IsValid)
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return PartialView("_Edit", model);
                }
                string msg = "";
                bool result = _factory.InsertOrUpdateCategory(model, CurrentUser.UserId, ref msg);
                if (result)
                {
                    //Save Image on Server
                    if (!string.IsNullOrEmpty(model.ImageURL) && model.ImageURLPictureByte != null)
                    {
                        var originalDirectory = new DirectoryInfo(string.Format("{0}Uploads\\", Server.MapPath(@"\")));
                        var path = string.Format("{0}{1}", originalDirectory, model.ImageURL);
                        MemoryStream ms = new MemoryStream(ImageURLphotoByte, 0, ImageURLphotoByte.Length);
                        ms.Write(ImageURLphotoByte, 0, ImageURLphotoByte.Length);
                        System.Drawing.Image imageTmp = System.Drawing.Image.FromStream(ms, true);
                        ImageHelper.Me.SaveCroppedImage(imageTmp, path, model.ImageURL, ref ImageURLphotoByte);
                        model.ImageURLPictureByte = ImageURLphotoByte;
                        FTP.Upload(model.ImageURL, model.ImageURLPictureByte);
                        ImageHelper.Me.TryDeleteImageUpdated(path);
                    }
                    //Save Image on Server
                    if (!string.IsNullOrEmpty(model.IconURL) && model.IconURL != null)
                    {
                        var originalDirectory = new DirectoryInfo(string.Format("{0}Uploads\\", Server.MapPath(@"\")));
                        var path = string.Format("{0}{1}", originalDirectory, model.IconURL);
                        MemoryStream ms = new MemoryStream(IconURLphotoByte, 0, IconURLphotoByte.Length);
                        ms.Write(IconURLphotoByte, 0, IconURLphotoByte.Length);
                        System.Drawing.Image imageTmp = System.Drawing.Image.FromStream(ms, true);
                        ImageHelper.Me.SaveCroppedImage(imageTmp, path, model.IconURL, ref IconURLphotoByte);
                        model.IconURLPictureByte = IconURLphotoByte;
                        FTP.Upload(model.IconURL, model.IconURLPictureByte);
                        ImageHelper.Me.TryDeleteImageUpdated(path);
                    }
                    return RedirectToAction("Index");
                }
                else
                {
                    model = _factory.GetDetail(model.ID);
                    ModelState.AddModelError("Name", Commons.ErrorMsg);
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return PartialView("_Edit", model);
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("ListofProductFeatures_Edit: ", ex);
                ModelState.AddModelError("Name", Commons.ErrorMsg /*msg*/);
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return PartialView("_Edit", model);
            }
        }

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
                NSLog.Logger.Error("ListofProductFeaturesDelete: ", ex);
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        [HttpGet]
        public PartialViewResult Delete(string id)
        {
            return PartialView("_Delete", _factory.GetDetail(id));
        }

        [HttpPost]
        public ActionResult Delete(ProductFeatureDetailModels model)
        {
            try
            {
                string msg = "";
                var result = _factory.Delete(model.ID, CurrentUser.UserId, ref msg);
                if (!result)
                {
                    ModelState.AddModelError("Name", Commons.ErrorMsg);
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return PartialView("_Delete", model);
                }
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("ListofProductFeatures_Delete: ", ex);
                ModelState.AddModelError("Name", Commons.ErrorMsg);
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return PartialView("_Delete", model);
            }
        }
        [HttpPost]
        public ActionResult ProductFeatureAction(string id)
        {
            try
            {
                string msg = "";
                var result = _factory.ProductFeatureAction(id, CurrentUser.UserId, ref msg);
                if (!result)
                {
                    ModelState.AddModelError("Name", Commons.ErrorMsg);
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return RedirectToAction("Index");
                }

                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("ListofProductFeatures: ", ex);
                ModelState.AddModelError("Name", Commons.ErrorMsg);
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return RedirectToAction("Index");
            }
        }
    }
}