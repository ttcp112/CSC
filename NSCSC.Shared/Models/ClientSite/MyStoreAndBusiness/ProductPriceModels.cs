namespace NSCSC.Shared.Models.ClientSite.MyStoreAndBusiness
{
    public class ProductPriceModels
    {
        public string ID { get; set; }
        public double Price { get; set; }
        public double Period { get; set; }
        public int PeriodType { get; set; }
        public bool IsExtend { get; set; }
        public string PeriodName { get; set; }
        public bool IsSelected { get; set; }
        public string CurrencySymbol { get; set; }
        public string sPeriodName { get; set; }

    }
}
