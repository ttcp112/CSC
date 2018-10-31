using NSCSC.Shared;
using NSCSC.Shared.Factory.SandBox.Inventory;
using NSCSC.Shared.Factory.Settings;
using NSCSC.Shared.Models.Sandbox.Inventory.Product;
using NSCSC.Shared.Models.Settings.Tax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace NSCSC.Web.Controllers
{
    public class STaxController : HQController
    {
        // GET: STax
        private ProductFactory _productFactory = null;
        private TaxFactory _factory = null;
        public STaxController()
        {
            _factory = new TaxFactory();
            _productFactory = new ProductFactory();

        }
        public ActionResult Index()
        {
            try
            {
                TaxViewModels model = new TaxViewModels();
                return View(model);

            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("Tax_Index:", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }

        public ActionResult Search(TaxViewModels model)
        {
            try
            {
                var data = _factory.GetListCategory(CurrentUser.UserId);
                //data = data.Where(x => x.IsActive == true).ToList();
                model.ListTax = data;
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("Tax_Search: ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
            return PartialView("_ListData", model);
        }

        [HttpGet]
        public ActionResult Create()
        {
            TaxModels model = new TaxModels();
            model.IsActive = true;
            model.TaxType = (byte)Commons.ETaxType.AddOn;

            model.ItemDetail = "All";

            // Get list all item (product, package, addition)
            var lstDish = _productFactory.GetListData(null, 0);

            if (lstDish != null)
            {
                model.ListProduct = new List<ProductModels>();
                int offSet = 0;
                foreach (var item in lstDish)
                {
                    ProductModels product = new ProductModels()
                    {
                        ID = item.ID,
                        Name = item.Name,
                        Code = item.Code,
                        ProductType = item.ProductType,
                        OffSet = offSet
                    };
                    model.ListProduct.Add(product);
                    offSet++;
                }

                model.TotalAllItem = lstDish.Count(x => x.ProductType == (int)Commons.EProductType.Package || x.ProductType == (int)Commons.EProductType.Product || x.ProductType == (int)Commons.EProductType.Addition);
                model.TotalSpecipicItem = lstDish.Count;
            }

            return View(model);
        }
        [HttpPost]
        public ActionResult Create(TaxModels model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.TaxName))
                {
                    ModelState.AddModelError("TaxName", "Name field is required");

                }
                if (model.ListProduct != null && model.ListProduct.Any())
                {
                    model.ListProduct = model.ListProduct.Where(x => x.Status != (int)Commons.EStatus.Deleted).ToList();
                }
                //if (!model.ListProduct.Any())
                //{
                //    ModelState.AddModelError("itemdetail", "Please select Applicable items in item detail for Tax no");
                //}
                if (!ModelState.IsValid)
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return View(model);
                }
                string msg = "";
                bool result = _factory.InsertOrUpdateTax(model, CurrentUser.UserId, ref msg);
                if (result)
                {

                    return RedirectToAction("Index");
                }
                else
                {

                    ModelState.AddModelError("TaxName", Commons.ErrorMsg /*msg*/);
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("Tax_Create : ", ex);
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }

        [HttpGet]
        public PartialViewResult View(string id)
        {
            TaxModels model = _factory.GetDetail(id);
            model.ListProduct = model.ListProduct.OrderBy(x => x.ProductType).ToList();
            return PartialView("_View", model);
        }

        [HttpGet]
        public PartialViewResult Edit(string id)
        {
            TaxModels model = _factory.GetDetail(id);
            var lstDish = _productFactory.GetListData(null, 0);
            if(lstDish != null)
            {
                model.TotalAllItem = lstDish.Count(x => x.ProductType == (int)Commons.EProductType.Package || x.ProductType == (int)Commons.EProductType.Product || x.ProductType == (int)Commons.EProductType.Addition);
            }
            if (model.ListProduct != null)
            {
                model.TotalSpecipicItem = model.ListProduct.Count;
            }
            return PartialView("_Edit", model);
        }

        [HttpPost]
        public ActionResult Edit(TaxModels model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.TaxName))
                {
                    ModelState.AddModelError("TaxName", "Name field is required");
                }
                if (model.ListProduct != null && model.ListProduct.Any())
                {
                    model.ListProduct = model.ListProduct.Where(x => x.Status != (int)Commons.EStatus.Deleted).ToList();
                }
                //if (!model.ListProduct.Any())
                //{
                //    ModelState.AddModelError("itemdetail", "Please select Applicable items in item detail for Tax no");
                //}
                if (!ModelState.IsValid)
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return PartialView("_Edit", model);
                }
                string msg = "";
                bool result = _factory.InsertOrUpdateTax(model, CurrentUser.UserId, ref msg);
                if (result)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    //model = _factory.GetDetail(model.Id);
                    if (!string.IsNullOrEmpty(msg))
                        ModelState.AddModelError("TaxName", msg);
                    else
                        ModelState.AddModelError("TaxName", Commons.ErrorMsg);
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return PartialView("_Edit", model);
                }
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("Tax_Edit: ", ex);
                ModelState.AddModelError("TaxName", Commons.ErrorMsg /*msg*/);
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
                NSLog.Logger.Error("TaxDelete: ", ex);
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }


        [HttpGet]
        public PartialViewResult Delete(string id)
        {
            return PartialView("_Delete", _factory.GetDetail(id));
        }

        [HttpPost]
        public ActionResult Delete(TaxModels model)
        {
            try
            {
                string msg = "";
                var result = _factory.Delete(model.Id, CurrentUser.UserId, ref msg);
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
                NSLog.Logger.Error("Tax_Delete: ", ex);
                ModelState.AddModelError("Name", Commons.ErrorMsg);
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return PartialView("_Delete", model);
            }
        }
        //Load Products Item
        public ActionResult LoadItems(int ItemType)
        {
            var search = "";
            var lstDish = _productFactory.GetListData(search, ItemType);
            TaxModels model = new TaxModels();
            if (lstDish != null)
            {
                model.LstProduct = new List<ProductModels>();
                foreach (var item in lstDish)
                {
                    ProductModels product = new ProductModels()
                    {
                        ID = item.ID,
                        Name = item.Name,
                        Code = item.Code,
                        ProductType = ItemType
                    };
                    model.LstProduct.Add(product);
                }
            }
            return PartialView("_TabChooseItems", model);
        }
        // Add Products Items
        public ActionResult AddItems(TaxModels data)
        {
            TaxModels model = new TaxModels();
            if (data.LstProduct != null && data.LstProduct.Count > 0)
                model.LstProduct = new List<ProductModels>();

            for (int i = 0; i < data.LstProduct.Count; i++)
            {
                ProductModels item = new ProductModels();
                item.OffSet = data.currentItemOffset;
                item.ID = data.LstProduct[i].ID;
                item.Code = data.LstProduct[i].Code;
                item.Name = data.LstProduct[i].Name;
                item.ProductType = data.LstProduct[i].ProductType;
                model.ListProduct.Add(item);

                data.currentItemOffset++;
            }
            //model.offset = data.currentgroupOffSet;
            return PartialView("_ProductsItem", model);
        }

    }
}