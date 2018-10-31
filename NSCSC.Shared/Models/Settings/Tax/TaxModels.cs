using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSCSC.Shared.Models.Sandbox.Inventory.Product;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace NSCSC.Shared.Models.Settings.Tax
{
    public class TaxModels
    {
        public string Id { get; set; }
        [Required]
        public string TaxName { get; set; }
        public int TaxType { get; set; }
        public string TaxTypeName { get; set; }
        public List<SelectListItem> ListType { get; set; }
        [Required]
        [Range(0, 100, ErrorMessage = "Please enter a value from 0 to 100")]
        public double TaxPercent { get; set; }
        public bool IsActive { get; set; }
        public List<ProductModels> ListProduct { get; set; }
        public List<ProductModels> LstProduct { get; set; }
        public int offset { get; set; }
        public int currentItemOffset { get; set; }
        public int currentgroupOffSet { get; set; }
        public string ItemDetail { get; set; }
        public bool IsAllItem { get; set; }
        public bool IspecificItem { get; set; }
        public int TotalAllItem { get; set; }
        public int TotalSpecipicItem { get; set; }
        public TaxModels()
        {
            ListType = new List<SelectListItem>()
            {

                new SelectListItem(){ Text=Commons.TypeTaxAddOn, Value=Commons.ETaxType.AddOn.ToString("d")},
                new SelectListItem(){ Text=Commons.TypeTaxInclusive, Value=Commons.ETaxType.Inclusive.ToString("d")},                
            };
            ListProduct = new List<ProductModels>();
            LstProduct = new List<ProductModels>();
            IsAllItem = true;
        }
    }
    public class TaxViewModels
    {
        public List<TaxModels> ListTax { get; set; }
        public TaxViewModels()
        {
            ListTax = new List<TaxModels>();
        }
    }
}
