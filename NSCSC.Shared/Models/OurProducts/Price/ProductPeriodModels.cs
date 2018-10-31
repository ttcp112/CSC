using System.Collections.Generic;

namespace NSCSC.Shared.Models.OurProducts.Price
{
    public class ProductPeriodModels
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public int ProductType { get; set; }
        public int AdditionType { get; set; }
        public int NumberOfDevice { get; set; }
        public List<AdditionPeriodModels> ListAddition { get; set; }
        public List<PeriodModels> ListPeriod { get; set; }
        public List<string> ListApplyOn { get; set; }
        public ProductPeriodModels()
        {
            ListAddition = new List<AdditionPeriodModels>();
            ListPeriod = new List<PeriodModels>();
        }
    }
}
