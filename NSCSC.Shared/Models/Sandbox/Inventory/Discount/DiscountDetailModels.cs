using NSCSC.Shared.Models.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace NSCSC.Shared.Models.SandBox.Inventory.Discount
{
    public class DiscountDetailModels : BaseModels
    {
        public string ID { get; set; }

        [Required]
        public string Name { get; set; }

        //public string Code { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }

        public int ValueType { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Please enter a value greater than or equal to 0")]
        public double Value { get; set; }

        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        //public DateTime ApplyFrom { get; set; }

        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        //public DateTime ApplyTo { get; set; }

        public int ApplyType { get; set; }

        public bool IsAllPackage { get; set; }
        public bool IsAllAddition { get; set; }
        public List<DiscountCategoryModels> ListDiscountCategory { get; set; }
        public List<DiscountCodeModels> ListDiscountCode { get; set; }

        public int PeriodType { get; set; }
        public int PeriodTime { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime PeriodDate { get; set; }

        /*Variable for Client*/
        public List<SelectListItem> ListValueType { get; set; }
        public bool IsTotalOrder { get; set; }
        public bool IsAllCategory { get; set; }
        public List<ItemDisCate> ClientListDisCate { get; set; }
        public List<SelectListItem> ListDiscountPeriodType { get; set; }
        /*End Variable for Client*/

        public DiscountDetailModels()
        {
            //ApplyFrom = DateTime.Now;
            //ApplyTo = DateTime.Now;

            IsActive = true;

            ListDiscountCategory = new List<DiscountCategoryModels>();
            ListDiscountCode = new List<DiscountCodeModels>();
            /**/
            //=================
            ValueType = (byte)Commons.EValueType.Currency;
            ListValueType = new List<SelectListItem>()
            {
                new SelectListItem() { Text = Commons.ValueTypeCurrency, Value = Commons.EValueType.Currency.ToString("d")},
                new SelectListItem() { Text = Commons.ValueTypePercent, Value = Commons.EValueType.Percent.ToString("d")}
            };

            PeriodDate = DateTime.Now;
            PeriodType = (byte)Commons.EDiscountPeriodType.Never;
            ListDiscountPeriodType = new List<SelectListItem>()
            {
                new SelectListItem() { Text = Commons.DiscountPeriodTypeNever, Value = Commons.EDiscountPeriodType.Never.ToString("d")},
                new SelectListItem() { Text = Commons.DiscountPeriodTypeTime, Value = Commons.EDiscountPeriodType.Time.ToString("d")},
                new SelectListItem() { Text = Commons.DiscountPeriodTypeDay, Value = Commons.EDiscountPeriodType.Day.ToString("d")}
            };

            IsTotalOrder = true;
            ClientListDisCate = new List<ItemDisCate>();
        }
    }

    public class ItemDisCate
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public bool IsSelect { get; set; }
        /*client*/
        public int OffSet { get; set; }
        public string Type { get; set; }
    }

    public class POSTDisCate
    {
        public List<ItemDisCate> ListItem { get; set; }
        public bool IsAllPackage { get; set; }
        public bool IsAllAddition { get; set; }

        public POSTDisCate()
        {
            ListItem = new List<ItemDisCate>();
        }
    }
}
