using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSCSC.Shared.Models.SandBox.Inventory.Product
{
    public class ProductRequest : RequestBaseModels
    {
        public int ProductType { get; set; }
        public int Type { get; set; } // Type product of category load function
        public ProductDetailModels ProductDetail { get; set; }
    }
}
