using NSCSC.Shared.Factory.Sandbox;
using NSCSC.Shared.Models;
using NSCSC.Shared.Models.Sandbox.Employee;
using NSCSC.Web.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace NSCSC.Web.Controllers
{
    public class ProfileController : HQController
    {
        private EmployeeFactory _factory = null;

        public ProfileController()
        {
            _factory = new EmployeeFactory();

        }
        // GET: Profile
        //public ActionResult Index()
        //{
        //    if (Session["User"] == null)
        //        return RedirectToAction("Index", "Home");
        //    UserModels user = new UserModels();

        //    return View(CurrentUser);
        //}

        public EmployeeDetailModels GetDetail(string id)
        {
            try
            {
                EmployeeDetailModels model = _factory.GetDetail(id);
                return model;
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("GetDetail error: ", ex);
                return null;
            }
        }

        public ActionResult ChangePassword()
        {
            EmployeeDetailModels model = GetDetail(CurrentUser.UserId);
            return View(model);
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

        [HttpPost]
        public ActionResult ChangePassword(EmployeeDetailModels model)
        {
            listStorePropertiesReject = new List<string>();
            listStorePropertiesReject.Add("ZipCode");
            listStorePropertiesReject.Add("Country");
            listStorePropertiesReject.Add("Phone");
            PropertyReject();

            string _newPassword = "", _oldPassword = "", _comfirmPassword = "";
            try
            {
                _newPassword = model.NewPassword;
                _oldPassword = model.OldPassword;
                _comfirmPassword = model.ConfirmPassword;
                if (string.IsNullOrEmpty(model.OldPassword))
                {
                    ModelState.AddModelError("OldPassword", "Current Password field is required");
                }

                if (string.IsNullOrEmpty(model.NewPassword))
                {
                    ModelState.AddModelError("NewPassword", "New Password field is required");
                }

                if (string.IsNullOrEmpty(model.ConfirmPassword))
                {
                    ModelState.AddModelError("ConfirmPassword", "Confirm New Password field is required");
                }

                if (!model.NewPassword.Equals(model.ConfirmPassword))
                {
                    ModelState.AddModelError("ConfirmPassword", "Confirm New Password and new password incorrect");
                }

                if (!ModelState.IsValid)
                {
                    model = GetDetail(model.ID);
                    model.OldPassword = _oldPassword;
                    model.NewPassword = _newPassword;
                    model.ConfirmPassword = _comfirmPassword;
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return View(model);
                }

                string msg = "";
                string mec = CurrentUser.OrganizationId;
                string useid = CurrentUser.UserId;
                var result = _factory.ChangePassword(model, ref msg, mec, useid);
                if (result)
                {
                    return RedirectToAction("Logout", "Home", new { area = "" });
                }
                else
                {
                    ModelState.AddModelError("ConfirmPassword", msg);
                    model = GetDetail(model.ID);
                    model.OldPassword = _oldPassword;
                    model.NewPassword = _newPassword;
                    model.ConfirmPassword = _comfirmPassword;
                    /*===*/
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("Employee_Change Password Error: " , ex);
                ModelState.AddModelError("ConfirmPassword", ex.Message);
                model = GetDetail(model.ID);
                model.OldPassword = _oldPassword;
                model.NewPassword = _newPassword;
                model.ConfirmPassword = _comfirmPassword;
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return View(model);
            }
        }
    }
}