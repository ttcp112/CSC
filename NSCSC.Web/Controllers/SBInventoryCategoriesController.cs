using NSCSC.Shared;
using NSCSC.Shared.Factory.Sandbox.Inventory;
using NSCSC.Shared.Models.Sandbox.Inventory.Category;
using NSCSC.Shared.Utilities;
using NuWebNCloud.Shared.Utilities;
using System;
using System.IO;
using System.Net;
using System.Web.Mvc;
using System.Linq;

namespace NSCSC.Web.Controllers
{
    public class SBInventoryCategoriesController : HQController
    {
        // GET: Categories
        private CategoriesFactory _factory = null;
        public SBInventoryCategoriesController()
        {
            _factory = new CategoriesFactory();

        }
        public ActionResult Index()
        {
            try
            {
                CategoriesViewModels model = new CategoriesViewModels();
                return View(model);

            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("Category_Index:", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }

        public ActionResult Search(CategoriesViewModels model)
        {
            try
            {
                var data = _factory.GetListCategory(CurrentUser.UserId);

                model.ListCategory = data;
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("Category_Search: ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
            return PartialView("_ListData", model);
        }

        public ActionResult Create()
        {
            CategoriesModels model = new CategoriesModels();
            model.IsActive = true;
            model.IsFreeTrial = true;
            model.Type = (byte)Commons.EType.NuPOS;
            return PartialView("Create", model);
        }
        [HttpPost]
        public ActionResult Create(CategoriesModels model)
        {
            try
            {

                if (string.IsNullOrEmpty(model.Name))
                {
                    ModelState.AddModelError("Name", "Name field is required");
                }
                if (string.IsNullOrEmpty(model.ShortDescription))
                {
                    ModelState.AddModelError("ShortDescription", "Short Description field is required");
                }
                if (model.PictureUpload != null && !Commons.ImageExtendFiler.Contains(model.PictureUpload.ContentType))
                {
                    ModelState.AddModelError("PictureUpload", Commons.ErrorMsgFilterImage);
                }
                if (!ModelState.IsValid)
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return PartialView("Create", model);
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
                string msg = "";
                var tmp = model.PictureByte;
                model.PictureByte = null;
                bool result = _factory.InsertOrUpdateCategory(model, CurrentUser.UserId, ref msg);
                if (result)
                {
                    model.PictureByte = tmp;

                    //Save Image on Server
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

                    return RedirectToAction("Index");
                }
                else
                {

                    //ModelState.AddModelError("Name", Commons.ErrorMsg /*msg*/);
                    if (!string.IsNullOrEmpty(msg) && msg.Equals("There is already one category with the same product type that allows trial version. Do you want to change it?"))
                    {
                        model.IsConfirm = true;
                    }
                    else
                    {
                        model.IsConfirm = false;
                        ModelState.AddModelError("Error", msg);
                    }

                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    //return View(model);
                    return PartialView("Create", model);
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("Category_Create : ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }

        [HttpGet]
        public PartialViewResult View(string id)
        {
            CategoriesModels model = _factory.GetDetail(id);
            return PartialView("_View", model);
        }

        [HttpGet]
        public PartialViewResult Edit(string id)
        {
            CategoriesModels model = _factory.GetDetail(id);
            return PartialView("_Edit", model);
        }

        [HttpPost]
        public ActionResult Edit(CategoriesModels model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Name))
                {
                    ModelState.AddModelError("Name", "Name field is required");
                }
                if (string.IsNullOrEmpty(model.ShortDescription))
                {
                    ModelState.AddModelError("ShortDescription", "Short Description field is required");
                }
                if (model.PictureUpload != null && !Commons.ImageExtendFiler.Contains(model.PictureUpload.ContentType))
                {
                    ModelState.AddModelError("PictureUpload", Commons.ErrorMsgFilterImage);
                }
                if (!ModelState.IsValid)
                {
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
                        model.ImageURL = model.ImageURL.Replace(Commons.PublicImages, "").Replace(Commons.Image100_50, "");
                }
                string msg = "";
                var tmp = model.PictureByte;
                model.PictureByte = null;
                bool result = _factory.InsertOrUpdateCategory(model, CurrentUser.UserId, ref msg);
                if (result)
                {
                    model.PictureByte = tmp;
                    ////Save Image on Server
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
                    return RedirectToAction("Index");
                }
                else
                {
                    model = _factory.GetDetail(model.Id);
                    //ModelState.AddModelError("Name", Commons.ErrorMsg /*msg*/);
                    ModelState.AddModelError("Error", msg);
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return PartialView("_Edit", model);
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("Category_Edit: ", ex);
                //ModelState.AddModelError("Name", Commons.ErrorMsg);
                ModelState.AddModelError("Error", Commons.ErrorMsg);
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
                NSLog.Logger.Error("CategoryDelete: ", ex);
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }



        [HttpGet]
        public PartialViewResult Delete(string id)
        {
            return PartialView("_Delete", _factory.GetDetail(id));
        }

        [HttpPost]
        public ActionResult Delete(CategoriesModels model)
        {
            try
            {
                string msg = "";
                var result = _factory.Delete(model.Id, CurrentUser.UserId, ref msg);
                if (!result)
                {
                    //ModelState.AddModelError("Name", Commons.ErrorMsg);
                    ModelState.AddModelError("Name", msg);
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return PartialView("_Delete", model);
                }
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("Category_Delete: ", ex);
                ModelState.AddModelError("Name", Commons.ErrorMsg);
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return PartialView("_Delete", model);
            }
        }

        //public ActionResult Export()
        //{
        //    CategoriesModels model = new CategoriesModels();
        //    return View(model);
        //}

        //[HttpPost]
        //public ActionResult Export(CategoriesModels model)
        //{
        //    try
        //    {
        //        XLWorkbook wb = new XLWorkbook();
        //        var wscategory = wb.Worksheets.Add("Category");               
        //        StatusResponse response = _factory.Export(ref wscategory, CurrentUser.UserId);
        //        if (!response.Status)
        //        {
        //            ModelState.AddModelError("", response.MsgError);
        //            return View(model);
        //        }

        //        ViewBag.wb = wb;
        //        Response.Clear();
        //        Response.ClearContent();
        //        Response.ClearHeaders();
        //        Response.Charset = UTF8Encoding.UTF8.WebName;
        //        Response.ContentEncoding = UTF8Encoding.UTF8;
        //        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //        Response.AddHeader("content-disposition", String.Format(@"attachment;filename={0}.xlsx", CommonHelper.GetExportFileName("Category").Replace(" ", "_")));

        //        using (var memoryStream = new System.IO.MemoryStream())
        //        {
        //            wb.SaveAs(memoryStream);
        //            memoryStream.WriteTo(HttpContext.Response.OutputStream);
        //            memoryStream.Close();
        //        }
        //        HttpContext.Response.End();
        //        return RedirectToAction("Export");
        //    }
        //    catch (Exception e)
        //    {
        //        _logger.Error("Category_Export: " + e);
        //        return new HttpStatusCodeResult(400, e.Message);
        //    }
        //}
    }


}
