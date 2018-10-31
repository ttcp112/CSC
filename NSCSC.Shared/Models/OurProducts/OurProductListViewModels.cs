using NSCSC.Shared.Models.OurProducts.Price;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSCSC.Shared.Models.OurProducts
{
    public class OurProductListViewModels
    {
        public string CategoryID { get; set; }
        public PriceListProductModels PriceList { get; set; }
        public bool IsReCommen { get; set; }
        public string Name { get; set; }
        public OurProductListViewModels()
        {
            PriceList = new PriceListProductModels();
        }
    }
}
