using NSCSC.Shared.Factory.ClientSite;
using NSCSC.Shared.Factory.ClientSite.OurProducts;
using NSCSC.Shared.Models.OverView;
using System;
using System.Web.Mvc;

namespace NSCSC.Web.Clients.Controllers
{
    public class OverViewController : ClientController
    {
        private OurProductFactory _ourFactory = null;
        private CheckIpAddressFactory _checkIpFactory = null;
        public OverViewController()
        {
            _ourFactory = new OurProductFactory();
            _checkIpFactory = new CheckIpAddressFactory();
        }
        // GET: OverView
        public ActionResult Index()
        {
            try
            {
                OverViewModels model = new OverViewModels();
                // get list promotion 
                var promo = _ourFactory.GetPromotion();
                if (promo != null)
                    model.ListPromotions = promo;
                // get ip and check ip
                //var IpAddress = _checkIpFactory.GetIPAddress();
                //var CountryCode = _checkIpFactory.GetCountryCode();
                if (CountryCode.Equals("SG"))
                    model.IsIpSingapore = true;
                return View(model);
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("Index : ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }
    }
}