using NSCSC.Shared.Models.Base;
using NSCSC.Shared.Models.Sandbox.Inventory.Product;
using NSCSC.Shared.Models.SandBox.Inventory.Discount;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace NSCSC.Shared.Models.SandBox.Inventory.Product
{
    public class ProductDetailModels : BaseModels
    {
        public int Index { get; set; }
        public string ID { get; set; }
        public int Type { get; set; }
        public int ProductType { get; set; }
        public int AdditionType { get; set; }

        public string CategoryName { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Please enter a value greater than or equal to 0.")]
        public int Sequence { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [StringLength(10,ErrorMessage ="The product code maximum length 10 characters")]
        public string Code { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value greater than or equal to 1.")]
        public int AmountOfUnit { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime SaleFrom { get; set; } = DateTime.Now;

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime SaleTo { get; set; } = DateTime.Now;

        public bool IsActive { get; set; }
        public bool IsPublic { get; set; }
        public bool IsExtend { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value greater than or equal to 1.")]
        public int IncludeStore { get; set; }

        public bool IsDisplayWeb { get; set; }
        public bool IsIntegrate { get; set; }
        public bool IsIncludeLocalServer { get; set; }
        public bool IsIncludeCloudServer { get; set; }
        public string Description { get; set; }
        public int PeriodType { get; set; }
        public List<SelectListItem> ListPeriodType { get; set; }
        public string CategoryId { get; set; }
        public List<ProductCategoryModels> ListCategory { get; set; }
        public List<SelectListItem> Categories { get; set; }
        public List<ProductCompositeModels> ListComposite { get; set; }
        
        public List<ProductPriceModels> ListPrice { get; set; }

        public List<ProductPriceModels> ListPriceExtend { get; set; }

        public List<ProductModels> ListProducts { get; set; }

        public bool IsUnlimitedAmountOfUnit { get; set; }
        public bool IsUnlimitedIncludeStore { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value greater than or equal to 1.")]
        public int NumberOfAccount { get; set; }
        public bool IsUnlimitedNumberOfAccount { get; set; }
        public List<ProductFunctionModels> ListFunction { get; set; }

        /*Variable for Client*/
        public List<SelectListItem> ListAdditionType { get; set; }
        public List<ItemDisCate> ClientListDisCate { get; set; }
        public List<ProductCompositeModels> ClientListProduct { get; set; }
        public List<ProductCompositeModels> ClientListAddHardware { get; set; }
        public List<ProductCompositeModels> ClientListAddService { get; set; }
        public List<SelectListItem> ListFuncProductType { get; set; }
        // update 05/09/2018
        public double Price { get; set; }
        public int Period { get; set; }
        public string PeriodTime { get; set; }
        public string CusID { get; set; }
        public string sPeriodType { get; set; }
        /*End Variable for Client*/

        public ProductDetailModels()
        {
            /**** Init value *****/
            this.IsPublic = true;
            this.IsActive = true;
            this.IncludeStore = 1;
            this.IsExtend = true;
            this.AmountOfUnit = 1;
            this.NumberOfAccount = 1;
            
            ListCategory = new List<ProductCategoryModels>();
            ListComposite = new List<ProductCompositeModels>();
            ListPrice = new List<ProductPriceModels>();
            ListProducts = new List<ProductModels>();
            Categories = new List<SelectListItem>();
            ListPriceExtend = new List<ProductPriceModels>();
            ListFunction = new List<ProductFunctionModels>();
            ListPeriodType = new List<SelectListItem>()
            {
                //new SelectListItem() { Value = Commons.EPeriodType.None.ToString("d"), Text = Commons.PeriodTypeNone },
                new SelectListItem() { Value = Commons.EPeriodType.Day.ToString("d"), Text = Commons.PeriodTypeDay },
                new SelectListItem() { Value = Commons.EPeriodType.Month.ToString("d"), Text = Commons.PeriodTypeMonth },
                new SelectListItem() { Value = Commons.EPeriodType.Year.ToString("d") ,Text = Commons.PeriodTypeYear },
                new SelectListItem() { Value = Commons.EPeriodType.OneTime.ToString("d"),Text = Commons.PeriodTypeOneTime }
            };

            AdditionType = (byte)Commons.EAdditionType.Hardware;
            ListAdditionType = new List<SelectListItem>()
            {
                new SelectListItem() {  Text =  Commons.EAdditionType.Hardware.ToString(), Value = Commons.EAdditionType.Hardware.ToString("d") },
                new SelectListItem() {  Text =  Commons.EAdditionType.Software.ToString(), Value = Commons.EAdditionType.Software.ToString("d") },
                new SelectListItem() {  Text =  Commons.EAdditionType.Service.ToString(), Value = Commons.EAdditionType.Service.ToString("d") },
                new SelectListItem() {  Text =  Commons.EAdditionType.Location.ToString(), Value = Commons.EAdditionType.Location.ToString("d") },
                new SelectListItem() {  Text =  Commons.EAdditionType.Account.ToString(), Value = Commons.EAdditionType.Account.ToString("d") },
                new SelectListItem() {  Text =  Commons.EAdditionType.Function.ToString(), Value = Commons.EAdditionType.Function.ToString("d") }
            };

            /*Variable for Client*/
            ClientListDisCate = new List<ItemDisCate>();
            ClientListProduct = new List<ProductCompositeModels>();
            ClientListAddHardware = new List<ProductCompositeModels>();
            ClientListAddService = new List<ProductCompositeModels>();

            Type = (byte)Commons.EType.NuPOS;
            ListFuncProductType = new List<SelectListItem>()
            {
                //new SelectListItem() {  Text =  Commons.FunctionProductTypeNuDisplay, Value = Commons.EType.NuDisplay.ToString("d") },
                new SelectListItem() {  Text =  Commons.FunctionProductTypeNuKiosk, Value = Commons.EType.NuKiosk.ToString("d") },
                new SelectListItem() {  Text =  Commons.FunctionProductTypeNuPOS, Value = Commons.EType.NuPOS.ToString("d") },

                new SelectListItem() {  Text =  Commons.FunctionProductTypePOinSLinkApp, Value = Commons.EType.POinS_Link_App.ToString("d") },

                new SelectListItem() {  Text =  Commons.FunctionProductTypePOZZ, Value = Commons.EType.POZZ.ToString("d") },
                //new SelectListItem() {  Text =  Commons.FunctionProductTypePOZZDisplay, Value = Commons.EType.POZZ_Display.ToString("d") },
                //new SelectListItem() {  Text =  Commons.FunctionProductTypePOZZKiosk, Value = Commons.EType.POZZ_Kiosk.ToString("d") }
            };
        }
    }

    public class POSTItem
    {
        public int ProductType { get; set; }
        public List<ProductCompositeModels> ListItem { get; set; }
        public POSTItem()
        {
            ListItem = new List<ProductCompositeModels>();
        }
    }
}
