using System.Collections.Generic;

namespace NSCSC.Shared.Models.OurProducts.Price
{
    public class PriceListProductModels
    {
        public List<PeriodModels> ListPeriod { get; set; }
        public List<ProductPeriodModels> ListProduct { get; set; }

        public PriceListProductModels()
        {
            ListPeriod = new List<PeriodModels>();
            ListProduct = new List<ProductPeriodModels>();
        }
    }
}
