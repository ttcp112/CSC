using NSCSC.Shared.Models.ClientSite.OurProDucts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSCSC.Shared.Models.OurProducts.Product
{
    public class ProductDetailViewModels
    {
        //public string CurrencySymbol { get; set; }
        //name for category
        public string Name { get; set; }
        public OurProDuctsModel ProductDetail { get; set; }
        public ProductDetailViewModels()
        {
            ProductDetail = new OurProDuctsModel();
        }
    }
}
