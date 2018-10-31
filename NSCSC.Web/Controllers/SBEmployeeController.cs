using NSCSC.Shared.Models.Sandbox.Employee;
using NSCSC.Web.App_Start;
using System;
using System.Net;
using System.Web.Mvc;
using System.IO;
using NSCSC.Shared.Factory.Sandbox;
using NSCSC.Shared.Utilities;
using NuWebNCloud.Shared.Utilities;
using NSCSC.Shared;
using System.Linq;

namespace NSCSC.Web.Controllers
{
    [NuAuth]
    public class SBEmployeeController : HQController
    {
        // GET: SBEmployee
        private EmployeeFactory _factory = null;

        public SBEmployeeController()
        {
            _factory = new EmployeeFactory();
        }

        public ActionResult Index()
        {
            try
            {
                EmployeeViewModels model = new EmployeeViewModels();
                return View(model);
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("Employee_Index Error: ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }

        public ActionResult Search(EmployeeViewModels model)
        {
            try
            {
                var data = _factory.GetListEmployee(CurrentUser.UserId, CurrentUser.RoleLevel, model.SearchString);
                ViewBag.CurrentUserId = CurrentUser.UserId;
                model.ListItem = data;
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("Employee_Search Error: ", e);
                return new HttpStatusCodeResult(400, e.Message);
            }

            return PartialView("_ListData", model);
        }

        [HttpGet]
        public new PartialViewResult View(string id)
        {
            EmployeeDetailModels model = GetDetail(id);
            return PartialView("_View", model);
        }

        public EmployeeDetailModels GetDetail(string id)
        {
            try
            {
                EmployeeDetailModels model = _factory.GetDetail(id);
                return model;
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("Employee GetDetail Error: ", ex);
                return null;
            }
        }

        public ActionResult Create()
        {
            EmployeeDetailModels model = new EmployeeDetailModels();
            model.GetListRole(CurrentUser.RoleLevel);
            model.ListRole = model.ListRole.Where(w => w.Value != CurrentUser.RoleID).ToList();
            model.CreateUser = CurrentUser.UserId;
            model.ListCountry = _factory.GetListCountry();
            //get local country
            model.Country = model.ListCountry.Where(ww => ww.Alpha2Code == CurrentCountryCode).Select(ss => ss.Name).FirstOrDefault();
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(EmployeeDetailModels model)
        {
            try
            {
                if (model.PictureUpload != null && !Commons.ImageExtendFiler.Contains(model.PictureUpload.ContentType))
                {
                    ModelState.AddModelError("PictureUpload", Commons.ErrorMsgFilterImage);
                }

                if (!ModelState.IsValid)
                {
                    if ((ModelState["PictureUpload"]) != null && (ModelState["PictureUpload"]).Errors.Count > 0)
                        model.ImageURL = "";
                    model.ListCountry = _factory.GetListCountry();
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return View(model);
                }

                //=============================
                model.Email = model.Email.ToLower();
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
                        model.ImageURL = model.ImageURL.Replace(Commons.PublicImages, "").Replace(Commons.Image200_200, "");
                }
                string msg = "";
                var tmp = model.PictureByte;
                model.PictureByte = null;
                bool result = _factory.CreateOrEdit(model, ref msg);
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
                    //return RedirectToAction("Create");
                    ModelState.AddModelError("Error", msg);
                    model.ImageURL = "";
                    model.ListCountry = _factory.GetListCountry();
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("Employee_Create: ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }

        public PartialViewResult Edit(string id)
        {
            EmployeeDetailModels model = GetDetail(id);
            model.GetListRole(CurrentUser.RoleLevel);
            // Current user can not change role
            if(model.RoleID == CurrentUser.RoleID)
            {
                model.ListRole = model.ListRole.Where(w => w.Value == CurrentUser.RoleID).ToList();
            } else
            {
                model.ListRole = model.ListRole.Where(w => w.Value != CurrentUser.RoleID).ToList();
            }
            model.CreateUser = CurrentUser.UserId;
            model.ListCountry = _factory.GetListCountry();
            return PartialView("_Edit", model);
        }

        [HttpPost]
        public ActionResult Edit(EmployeeDetailModels model)
        {
            try
            {
                if (model.PictureUpload != null && !Commons.ImageExtendFiler.Contains(model.PictureUpload.ContentType))
                {
                    ModelState.AddModelError("PictureUpload", Commons.ErrorMsgFilterImage);
                }

                if (!ModelState.IsValid)
                {
                    if ((ModelState["PictureUpload"]) != null && (ModelState["PictureUpload"]).Errors.Count > 0)
                        model.ImageURL = "";

                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    model.ListCountry = _factory.GetListCountry();
                    return PartialView("_Edit", model);
                }

                //====================
                model.Email = model.Email.ToLower();
                byte[] photoByte = null;
                if (model.PictureUpload != null && model.PictureUpload.ContentLength > 0)
                {
                    Byte[] imgByte = new Byte[model.PictureUpload.ContentLength];
                    model.PictureUpload.InputStream.Read(imgByte, 0, model.PictureUpload.ContentLength);
                    model.PictureByte = imgByte;
                    model.ImageURL = Guid.NewGuid() + Path.GetExtension(model.PictureUpload.FileName);
                    model.PictureUpload = null;
                    photoByte = imgByte;
                } else
                {
                    if (!string.IsNullOrEmpty(model.ImageURL))
                        model.ImageURL = model.ImageURL.Replace(Commons.PublicImages, "").Replace(Commons.Image200_200, "");
                }
                string msg = "";
                var tmp = model.PictureByte;
                model.PictureByte = null;
                var result = _factory.CreateOrEdit(model, ref msg);
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
                    ModelState.AddModelError("Error", msg);
                    model.ListCountry = _factory.GetListCountry();
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return PartialView("_Edit", model);
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("Employee_Edit: ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }

        [HttpGet]
        public PartialViewResult Delete(string id)
        {
            EmployeeDetailModels model = GetDetail(id);
            return PartialView("_Delete", model);
        }

        [HttpPost]
        public ActionResult Delete(EmployeeModels model)
        {
            try
            {
                string msg = "";
                var result = _factory.Delete(model.ID, CurrentUser.UserId, ref msg);
                if (!result)
                {
                    ModelState.AddModelError("Name", msg);
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return PartialView("_Delete", model);
                }
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("Employee_Delete: ", ex);
                ModelState.AddModelError("Name", "Have an error when you delete a Employee");
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return PartialView("_Delete", model);
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
                NSLog.Logger.Error("Employee_Delete: ", ex);
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }
    }
}