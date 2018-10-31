using NSCSC.Shared.Models.Sandbox.Inventory.Product;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace NSCSC.Shared.Models
{
    public class PromotionModels
    {
        public string ID { get; set; }
        [Required(ErrorMessage ="The Name field is required")]
        public string Name { get; set; }
        public string ImageURL { get; set; }
        [DataType(DataType.Upload)]
        public HttpPostedFileBase PictureUpload { get; set; }
        public byte[] PictureByte { get; set; }

        [Required(ErrorMessage ="The Promotion Code field is required")]
        [MaxLength(10,ErrorMessage ="Promotion Code maximum length 10 characters")]
        public string PromotionCode { get; set; }

        [Range(0,9999, ErrorMessage ="Priority number must equal or larger than 0")]
        public int Priority { get; set; }
        [Range(0,9999, ErrorMessage ="Sequence number must equal or larger than 0")]
        public int Sequence { get; set; }
        public int MaximumAmount { get; set; }
        public bool IsUnlimited { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime ApplyFrom { get; set; } = DateTime.Now;
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime ApplyTo { get; set; } = DateTime.Now;
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public byte OperatorSpend { get; set; }
        public byte OperatorEarn { get; set; }
        public List<SpendRuleModels> ListSpendRule { get; set; }
        public List<EarnRuleModels> ListEarnRule { get; set; }

        public PromotionModels()
        {
            ListSpendRule = new List<SpendRuleModels>();
            ListEarnRule = new List<EarnRuleModels>();
            IsActive = true;
            Sequence = 0;
        }
    }
    public class SpendRuleModels
    {
        public string ID { get; set; }
        public int SpendType { get; set; }
        public int SpendOnType { get; set; }
        public double Amount { get; set; }
        public int OffSet { get; set; }
        public int Status { get; set; }
        public int? Condition { get; set; }
        public string ItemDetail { get; set; }
        public List<SelectListItem> ListSpendType { get; set; }
        public List<SelectListItem> ListSpendOnType { get; set; }
        public List<ProductModels> ListProduct { get; set; }

        public int currentgroupOffSet { get; set; }
        public int currentItemOffset { get; set; }
        public SpendRuleModels()
        {
            SpendType = (byte)Commons.ESpendType.BuyItem;
            SpendOnType = (byte)Commons.ESpendOnType.AnyItem;
            ListSpendType = new List<SelectListItem>()
            {
                new SelectListItem() { Text = "Buy Item", Value = Commons.ESpendType.BuyItem.ToString("d")},
                new SelectListItem() { Text = "Spend Money", Value = Commons.ESpendType.SpendMoney.ToString("d")},
            };
            ListSpendOnType = new List<SelectListItem>()
            {
                new SelectListItem() { Text = "Any Item", Value = Commons.ESpendOnType.AnyItem.ToString("d")},
                new SelectListItem() { Text = "Specific Item", Value = Commons.ESpendOnType.SpecificItem.ToString("d")},
                new SelectListItem() { Text = "Total Bill", Value = Commons.ESpendOnType.TotalBill.ToString("d")},
            };
            ListProduct = new List<ProductModels>();
        }
    }

    public class EarnRuleModels
    {
        public string ID { get; set; }
        public int EarnType { get; set; }
        public int Quantity { get; set; }
        public int DiscountType { get; set; }
        public double DiscountValue { get; set; }
        public List<ProductModels> ListProduct { get; set; }

        public int OffSet { get; set; }
        public int? Condition { get; set; }
        public int Status { get; set; }
        public List<SelectListItem> ListDiscountType { get; set; }
        public List<SelectListItem> ListEarnType { get; set; }
        public string ItemDetail { get; set; }
        public int currentItemOffset { get; set; }
        public int currentgroupOffSet { get; set; }

        public EarnRuleModels()
        {
            ListProduct = new List<ProductModels>();
            DiscountType = (byte)Commons.EValueType.Percent;
            ListDiscountType = new List<SelectListItem>()
            {
                new SelectListItem() { Text = Commons.DiscountPercent, Value = Commons.EValueType.Percent.ToString("d")},
                new SelectListItem() { Text = Commons.DiscountValue, Value = Commons.EValueType.Currency.ToString("d")},
            };
            EarnType = (byte)Commons.EEarnType.SpentItem;
            ListEarnType = new List<SelectListItem>()
            {
                new SelectListItem() { Text = Commons.EarnTypeSpentItem, Value = Commons.EEarnType.SpentItem.ToString("d")},
                new SelectListItem() { Text = Commons.EarnTypeSpecificItem, Value = Commons.EEarnType.SpecificItem.ToString("d")},
                new SelectListItem() { Text = Commons.EarnTypeTotalBill, Value = Commons.EEarnType.TotalBill.ToString("d")},
            };
        }
    }
}
