using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSCSC.Shared.Models.OurProducts
{
    public class OurProductsCategoryModels : CategoryDetailModels
    {
        public string ProductType { get; set; }
        public List<PromotionDetailModels> ListPromotions { get; set; }
        public OurProductsCategoryModels()
        {
            ListPromotions = new List<PromotionDetailModels>();
        }
    }
}
