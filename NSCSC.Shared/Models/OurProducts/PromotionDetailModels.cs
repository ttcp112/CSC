using System;

namespace NSCSC.Shared.Models.OurProducts
{
    public class PromotionDetailModels
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string ImageURL { get; set; }
        public string PromotionCode { get; set; }
        public int Priority { get; set; }
        public int Sequence { get; set; }
        public int MaximumAmount { get; set; }
        public bool IsUnlimited { get; set; }
        public DateTime ApplyFrom { get; set; }
        public DateTime ApplyTo { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public byte OperatorSpend { get; set; }
        public byte OperatorEarn { get; set; }
        public string RuleText { get; set; }
    }
}
