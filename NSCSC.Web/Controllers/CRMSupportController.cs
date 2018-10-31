using NSCSC.Shared.Factory.CRM;
using NSCSC.Shared.Factory.Sandbox;
using NSCSC.Shared.Models.CRM.Support;
using NSCSC.Web.App_Start;
using System;
using System.Linq;
using System.Web.Mvc;

namespace NSCSC.Web.Controllers
{
    [NuAuth]
    public class CRMSupportController : HQController
    {
        public SupportFactory _factory = null;
        private EmployeeFactory _factoryEmp = null;

        public CRMSupportController()
        {
            _factory = new SupportFactory();
            _factoryEmp = new EmployeeFactory();
        }

        // GET: CRMSupport
        public ActionResult Index()
        {
            try
            {
                SupportViewModels model = new SupportViewModels();
                return View(model);
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("SupportIndex: ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }

        public ActionResult Search(SupportViewModels model)
        {
            try
            {
                model.User = CurrentUser;
                var dataMsg = _factory.GetListMessage();
                model.ListMessage = dataMsg;
                var dataEmp = _factoryEmp.GetListEmployee(CurrentUser.UserId, CurrentUser.RoleLevel, "");
                dataEmp = dataEmp.Where(x => !x.ID.Equals(model.User.UserId)).ToList();
                Random rnd = new Random();
                dataEmp.ForEach(x =>
                {
                    x.Status = rnd.Next(0, 2);
                    x.ReceivedMessages = rnd.Next(0, 10);
                });
                dataEmp = dataEmp.OrderByDescending(x => x.ReceivedMessages).ToList();
                model.ListEmployee = dataEmp;
            }
            catch (Exception e)
            {
                NSLog.Logger.Error("SupportSearch: ", e);
                return new HttpStatusCodeResult(400, e.Message);
            }
            return PartialView("_ListData", model);
        }
    }
}