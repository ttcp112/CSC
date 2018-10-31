using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSCSC.Shared.Models.ManagementTool.ListofProductFeatures
{
    public class ProductFeatureModels 
    {
        public string ID { get; set; }
        public string CategoryName { get; set; }
        public string Name { get; set; }
        public int Sequence { get; set; }
        public bool IsActive { get; set; }
    }

    public class ProductFeatureViewModels
    {
        public List<ProductFeatureModels> ListofProductFeatures { get; set; }
        public ProductFeatureViewModels()
        {
            ListofProductFeatures = new List<ProductFeatureModels>();
        }
    }
}
